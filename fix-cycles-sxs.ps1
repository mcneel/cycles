#requires -Version 5.1
<#
.SYNOPSIS
    Gives each payload DLL that loads a member of the ccycles side-by-side assembly its own
    manifest, so it binds to the payload's copy and not a same-named DLL already loaded.

.DESCRIPTION
    The Windows counterpart of fix-cycles-tbb.sh. ccycles.dll's manifest
    (src/ccycles/ccycles_manifest.xml.in) only covers its own imports; the DLLs it loads
    resolve theirs by bare name. RDK's OIDN loads oneTBB 2021.11 as tbb12.dll, too old for
    embree4.dll, so ccycles.dll failed to load with error 127.

    Members are read from the built ccycles.dll, so CCYCLES_SXS_FILES in
    src/ccycles/CMakeLists.txt stays the only list. Only the manifest resource is written
    (UpdateResource, as mt.exe does); an existing one is extended. Idempotent.

    build_cycles.ps1 runs this on just the files each install wrote, so other plug-ins'
    DLLs in bin\<Config>\Plug-ins are left alone; publish_payload.ps1 runs it with -Check.
    By hand, only for a payload built some other way:
        .\fix-cycles-sxs.ps1 -PayloadDir <payload>        # fix
        .\fix-cycles-sxs.ps1 -PayloadDir <payload> -Check # verify only

.PARAMETER PayloadDir
    The folder holding ccycles.dll and the DLLs it loads.

.PARAMETER Files
    The DLLs to consider. Defaults to every DLL in PayloadDir, which is only right for a
    folder that holds nothing but the payload.

.PARAMETER Check
    Change nothing; fail if a DLL lacks the manifest it should have.
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$PayloadDir,
    [string[]]$Files,
    [switch]$Check
)

$ErrorActionPreference = 'Stop'

if (-not ('CyclesSxs' -as [type])) {
    # C# 5 only: ccycles.vcxproj runs build_cycles.ps1 under Windows PowerShell 5.1.
    Add-Type -TypeDefinition @'
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public static class CyclesSxs
{
    const uint LOAD_LIBRARY_AS_DATAFILE = 0x2;
    const uint LOAD_LIBRARY_AS_IMAGE_RESOURCE = 0x20;
    const int ERROR_RESOURCE_TYPE_NOT_FOUND = 1813;
    const int ERROR_RESOURCE_NAME_NOT_FOUND = 1814;
    static readonly IntPtr RT_MANIFEST = (IntPtr)24;
    // ISOLATIONAWARE_MANIFEST_RESOURCE_ID - the manifest the loader reads for a DLL.
    static readonly IntPtr MANIFEST_ID = (IntPtr)2;
    // What the linker and mt.exe use when a DLL has no manifest yet.
    public const int DefaultLanguage = 1033;

    delegate bool EnumLangProc(IntPtr module, IntPtr type, IntPtr name, ushort lang, IntPtr param);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern IntPtr LoadLibraryExW(string path, IntPtr file, uint flags);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool FreeLibrary(IntPtr module);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool EnumResourceLanguagesW(IntPtr module, IntPtr type, IntPtr name, EnumLangProc proc, IntPtr param);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr FindResourceExW(IntPtr module, IntPtr type, IntPtr name, ushort lang);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr LoadResource(IntPtr module, IntPtr res);
    [DllImport("kernel32.dll")]
    static extern IntPtr LockResource(IntPtr data);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern uint SizeofResource(IntPtr module, IntPtr res);
    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    static extern IntPtr BeginUpdateResourceW(string path, bool deleteExisting);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool UpdateResourceW(IntPtr update, IntPtr type, IntPtr name, ushort lang, byte[] data, uint size);
    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool EndUpdateResourceW(IntPtr update, bool discard);

    // The DLL's manifest, or null when it has none. language is the resource's language,
    // or -1 when there is no manifest.
    public static string ReadManifest(string path, out int language)
    {
        language = -1;
        IntPtr module = LoadLibraryExW(path, IntPtr.Zero, LOAD_LIBRARY_AS_DATAFILE | LOAD_LIBRARY_AS_IMAGE_RESOURCE);
        if (module == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error(), "cannot open " + path);
        try
        {
            var languages = new List<ushort>();
            EnumLangProc collect = (m, t, n, lang, p) => { languages.Add(lang); return true; };
            if (!EnumResourceLanguagesW(module, RT_MANIFEST, MANIFEST_ID, collect, IntPtr.Zero))
            {
                int error = Marshal.GetLastWin32Error();
                if (error == ERROR_RESOURCE_TYPE_NOT_FOUND || error == ERROR_RESOURCE_NAME_NOT_FOUND) return null;
                throw new Win32Exception(error, "cannot list the manifests of " + path);
            }
            if (languages.Count == 0) return null;
            if (languages.Count > 1) throw new InvalidDataException(path + " has " + languages.Count + " manifests (one per language); expected one");

            IntPtr res = FindResourceExW(module, RT_MANIFEST, MANIFEST_ID, languages[0]);
            if (res == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error(), "cannot find the manifest of " + path);
            IntPtr data = LockResource(LoadResource(module, res));
            byte[] bytes = new byte[SizeofResource(module, res)];
            Marshal.Copy(data, bytes, 0, bytes.Length);
            language = languages[0];
            return new UTF8Encoding(false).GetString(bytes).TrimStart((char)0xFEFF).TrimEnd('\0');
        }
        finally { FreeLibrary(module); }
    }

    public static void WriteManifest(string path, int language, string xml)
    {
        byte[] bytes = new UTF8Encoding(false).GetBytes(xml);
        IntPtr update = BeginUpdateResourceW(path, false);
        if (update == IntPtr.Zero) throw new Win32Exception(Marshal.GetLastWin32Error(), "cannot open " + path + " for writing");
        if (!UpdateResourceW(update, RT_MANIFEST, MANIFEST_ID, (ushort)language, bytes, (uint)bytes.Length))
        {
            int error = Marshal.GetLastWin32Error();
            EndUpdateResourceW(update, true);
            throw new Win32Exception(error, "cannot write the manifest of " + path);
        }
        if (!EndUpdateResourceW(update, false)) throw new Win32Exception(Marshal.GetLastWin32Error(), "cannot save " + path);
    }

    // The DLL names in the import and delay-import tables.
    public static string[] Imports(string path)
    {
        byte[] b = File.ReadAllBytes(path);
        int pe = BitConverter.ToInt32(b, 0x3C);
        if (BitConverter.ToUInt32(b, pe) != 0x00004550) throw new InvalidDataException(path + " is not a PE file");
        int sectionCount = BitConverter.ToUInt16(b, pe + 6);
        int optional = pe + 24;
        int directories = optional + (BitConverter.ToUInt16(b, optional) == 0x20B ? 112 : 96);
        int sections = optional + BitConverter.ToUInt16(b, pe + 20);
        var names = new List<string>();
        ReadDescriptors(b, sections, sectionCount, BitConverter.ToInt32(b, directories + 1 * 8), 20, 12, names);
        ReadDescriptors(b, sections, sectionCount, BitConverter.ToInt32(b, directories + 13 * 8), 32, 4, names);
        return names.ToArray();
    }

    static void ReadDescriptors(byte[] b, int sections, int sectionCount, int rva, int size, int nameAt, List<string> names)
    {
        if (rva == 0) return;
        for (int o = Offset(b, sections, sectionCount, rva); ; o += size)
        {
            bool empty = true;
            for (int i = 0; i < size; i++) if (b[o + i] != 0) { empty = false; break; }
            if (empty) return;
            int name = Offset(b, sections, sectionCount, BitConverter.ToInt32(b, o + nameAt));
            names.Add(Encoding.ASCII.GetString(b, name, Array.IndexOf(b, (byte)0, name) - name));
        }
    }

    static int Offset(byte[] b, int sections, int sectionCount, int rva)
    {
        for (int i = 0; i < sectionCount; i++)
        {
            int s = sections + i * 40;
            int virtualSize = BitConverter.ToInt32(b, s + 8), address = BitConverter.ToInt32(b, s + 12);
            int rawSize = BitConverter.ToInt32(b, s + 16), raw = BitConverter.ToInt32(b, s + 20);
            if (rva >= address && rva < address + Math.Max(virtualSize, rawSize)) return rva - address + raw;
        }
        throw new InvalidDataException("RVA 0x" + rva.ToString("X") + " lies outside every section");
    }
}
'@
}

$asmV1 = 'urn:schemas-microsoft-com:asm.v1'

# The manifest a DLL should have: whatever it already carries, minus any identity and
# file list, plus ours. Deterministic, so a second run finds nothing to do.
function Get-DesiredManifest([string]$Existing, [string]$AssemblyName, [string[]]$Members) {
    $doc = New-Object System.Xml.XmlDocument
    if ($Existing) { $doc.LoadXml($Existing) }
    else {
        $doc.LoadXml("<assembly xmlns=`"$asmV1`" manifestVersion=`"1.0`"/>")
        [void]$doc.InsertBefore($doc.CreateXmlDeclaration('1.0', 'UTF-8', 'yes'), $doc.DocumentElement)
    }
    $root = $doc.DocumentElement

    foreach ($node in @($root.ChildNodes)) {
        if ($node.LocalName -eq 'assemblyIdentity') {
            # Another identity means a manifest we did not write; do not guess its purpose.
            if ($node.GetAttribute('name') -ne $AssemblyName) {
                throw "already has a manifest for assembly '$($node.GetAttribute('name'))'; not overwriting it"
            }
            [void]$root.RemoveChild($node)
        }
        elseif ($node.LocalName -eq 'file') { [void]$root.RemoveChild($node) }
    }

    $identity = $doc.CreateElement('assemblyIdentity', $asmV1)
    $identity.SetAttribute('type', 'win32')
    $identity.SetAttribute('name', $AssemblyName)
    $identity.SetAttribute('version', '1.0.0.0')
    $identity.SetAttribute('processorArchitecture', 'amd64')
    $after = $root.PrependChild($identity)
    foreach ($m in $Members) {
        $file = $doc.CreateElement('file', $asmV1)
        $file.SetAttribute('name', $m)
        $after = $root.InsertAfter($file, $after)
    }

    $settings = New-Object System.Xml.XmlWriterSettings
    $settings.Indent = $true
    $settings.IndentChars = '  '
    $settings.Encoding = New-Object System.Text.UTF8Encoding($false)
    $stream = New-Object System.IO.MemoryStream
    $writer = [System.Xml.XmlWriter]::Create($stream, $settings)
    try { $doc.Save($writer) } finally { $writer.Dispose() }
    return $settings.Encoding.GetString($stream.ToArray())
}

$PayloadDir = (Resolve-Path -LiteralPath $PayloadDir).Path
$ccycles = Join-Path $PayloadDir 'ccycles.dll'
if (-not (Test-Path -LiteralPath $ccycles)) { throw "fix-cycles-sxs: no ccycles.dll in $PayloadDir" }

$lang = 0
$ccyclesManifest = [CyclesSxs]::ReadManifest($ccycles, [ref]$lang)
if (-not $ccyclesManifest) { throw "fix-cycles-sxs: $ccycles has no side-by-side manifest - was it built from src/ccycles/CMakeLists.txt?" }
$memberDoc = New-Object System.Xml.XmlDocument
$memberDoc.LoadXml($ccyclesManifest)
$members = @($memberDoc.DocumentElement.ChildNodes | Where-Object { $_.LocalName -eq 'file' } | ForEach-Object { $_.GetAttribute('name') })
if (-not $members.Count) { throw "fix-cycles-sxs: the manifest of $ccycles lists no members" }

if (-not $Files) { $Files = @(Get-ChildItem -LiteralPath $PayloadDir -Filter '*.dll' -File | ForEach-Object { $_.FullName }) }

$problems = @()
foreach ($path in $Files) {
    $name = Split-Path -Leaf $path
    if ($name -ieq 'ccycles.dll') { continue }

    $imports = [CyclesSxs]::Imports($path)
    $needed = @($members | Where-Object { $m = $_; $m -ine $name -and ($imports | Where-Object { $_ -ieq $m }) })
    if (-not $needed.Count) { continue }

    $assembly = 'ccycles.' + [System.IO.Path]::GetFileNameWithoutExtension($name)
    $existing = [CyclesSxs]::ReadManifest($path, [ref]$lang)
    try { $desired = Get-DesiredManifest $existing $assembly (@($name) + $needed) }
    catch { throw "fix-cycles-sxs: ${name}: $($_.Exception.Message)" }

    $detail = $needed -join ', '
    if ($existing -eq $desired) {
        Write-Host ("   {0,-30} {1}" -f $name, $detail) -ForegroundColor Green
    }
    elseif ($Check) {
        Write-Host ("   {0,-30} {1} - NO MANIFEST" -f $name, $detail) -ForegroundColor Red
        $problems += $name
    }
    else {
        if ($lang -lt 0) { $lang = [CyclesSxs]::DefaultLanguage }
        [CyclesSxs]::WriteManifest($path, $lang, $desired)
        Write-Host ("   {0,-30} {1} - manifest written" -f $name, $detail) -ForegroundColor Green
    }
}

if ($problems.Count) {
    throw ("fix-cycles-sxs: $($problems.Count) payload DLL(s) would bind to whatever copy of a shared " +
           "DLL is already loaded: $($problems -join ', '). Run .\fix-cycles-sxs.ps1 -PayloadDir `"$PayloadDir`".")
}
