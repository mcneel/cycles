#requires -Version 5.1
<#
.SYNOPSIS
    Configures, builds and installs Cycles + ccycles for Rhino.

.DESCRIPTION
    Finds Visual Studio 2022+ with vswhere and builds in its developer shell, with Ninja
    unless -Generator vs. GPU SDKs are probed; a missing one only switches off that
    backend's kernels, as CUDA and HIP device support is compiled in regardless and uses
    the payload's kernels. OptiX is the exception: it needs its SDK and nvcc.

    Kernels are built only for this machine's GPUs, since no others can be tested here.
    Pass -AllArches for the shipping set in kernel_arches.ps1.

.PARAMETER Configuration
    Debug, Release or RelWithDebInfo. Release builds CMake's RelWithDebInfo, for usable
    symbols.

.PARAMETER Devices
    Which backends to build *kernels* for. Defaults to whatever toolkits are detected;
    'cpu' builds no GPU kernels. Device support does not depend on it: a -Devices cpu
    ccycles.dll still drives CUDA and HIP with the payload's kernels.

.PARAMETER InstallDir
    Where to place ccycles.dll and its dependencies. Defaults to the Rhino
    Plug-ins output directory for the chosen configuration.

.PARAMETER CudaBinaries
    Fall back to upstream's CYCLES_CUDA_BINARIES_ARCH rather than PTX only. Only for
    comparing against what upstream would build.

.PARAMETER AllArches
    Build kernels for every architecture Cycles ships, not only this machine's GPUs.
    For publishing a payload.

.PARAMETER Force
    Install into the requested payload even when this build makes fewer kernels than it
    holds, deleting its now-wrong manifest. Without it, such a build goes to a sibling
    "local" payload, which RhinoCyclesCore prefers and git ignores.

.PARAMETER ConfigureOnly
    Run the CMake configure step and stop, leaving a solution to open in VS.

.PARAMETER AllowLibraryMismatch
    Build even though lib\windows_x64 is not the pinned library bundle (or holds unpulled
    LFS pointers). For debugging a bundle change, not for getting past the message.

.PARAMETER Generator
    ninja (the default) or vs. MSBuild runs one target's custom steps in order, so the
    GPU kernels compiled one at a time; Ninja runs them in parallel. "Ninja Multi-Config"
    keeps --config working. -Generator vs gives a Cycles.sln to open in the IDE.

.PARAMETER Jobs
    How many compiles to run at once. Defaults to a figure from this machine's memory, not
    its cores: a HIP kernel's clang peaks around 1.2 GB, the oneAPI link around 8 GB.

.EXAMPLE
    .\build_cycles.ps1 -Configuration Release

.EXAMPLE
    .\build_cycles.ps1 -Devices cpu -ConfigureOnly

.EXAMPLE
    .\build_cycles.ps1 -Generator vs -ConfigureOnly
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release', 'RelWithDebInfo')]
    [string]$Configuration = 'Release',

    [ValidateSet('cpu', 'cuda', 'optix', 'hip', 'oneapi')]
    [string[]]$Devices,

    [string]$InstallDir,

    [switch]$CudaBinaries,

    [switch]$AllArches,

    [switch]$Force,

    [switch]$ConfigureOnly,

    [switch]$AllowLibraryMismatch,

    [ValidateSet('ninja', 'vs')]
    [string]$Generator = 'ninja',

    [ValidateRange(1, 256)]
    [int]$Jobs,

    [string]$BuildDir = 'build'
)

$ErrorActionPreference = 'Stop'
$cyclesRoot = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }

function Write-Step($msg) { Write-Host "`n== $msg" -ForegroundColor Cyan }
function Write-Found($what, $where) { Write-Host ("   {0,-12} {1}" -f $what, $where) -ForegroundColor Green }
function Write-Missing($what, $why) { Write-Host ("   {0,-12} not found - {1}" -f $what, $why) -ForegroundColor DarkYellow }

# CMake reads backslashes in cache values as escapes (FindCUDA then reports a syntax
# error), so every path handed to -D is normalised.
function ConvertTo-CMakePath([string]$p) { return $p.Replace('\', '/') }

# ---------------------------------------------------------------- prerequisites

Write-Step "Checking prerequisites"

# Visual Studio anchors everything: CMake and Ninja ship in its "C++ CMake tools for
# Windows" component, which Rhino's .vsconfig installs. git and python are only needed
# to fetch the library bundle, so they are checked there.
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhere)) { throw "vswhere.exe not found. Install Visual Studio 2022 or newer." }

# No upper version bound: Rhino 9's C++ projects already ask for v145 (VS 2026).
$vsPath = & $vswhere -latest -products * `
    -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 `
    -version '[17.0,)' -property installationPath
if (-not $vsPath) {
    throw "No Visual Studio 2022 or newer with the C++ toolset found. Install the 'Desktop development with C++' workload."
}

$vsMajor = & $vswhere -latest -products * `
    -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 `
    -version '[17.0,)' -property installationVersion
$vsMajor = [int](($vsMajor -split '\.')[0])

# CMake: PATH first (unlike Ninja below), as this tree pins an older CMake and one on
# PATH was put there deliberately; then the copy inside Visual Studio.
$cmakeCandidates = @(
    (Get-Command cmake -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source)
    (Join-Path $vsPath 'Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe')
) | Where-Object { $_ -and (Test-Path $_) }

if (-not $cmakeCandidates) {
    throw ("cmake was not found on PATH or inside '$vsPath'. Add the 'C++ CMake tools " +
           "for Windows' component in the Visual Studio installer, or run bootstrap.exe " +
           "from the root of the Rhino repo, which installs it from Rhino's .vsconfig.")
}
$cmakeExe = $cmakeCandidates | Select-Object -First 1

# Skip a CMake too old for Ninja Multi-Config (3.17+).
if ($Generator -eq 'ninja') {
    $ninjaCapable = $cmakeCandidates |
        Where-Object { (& $_ --help 2>$null) -match 'Ninja Multi-Config' } |
        Select-Object -First 1
    if (-not $ninjaCapable) {
        Write-Missing 'Ninja Multi-Config' "not offered by $cmakeExe, falling back to the Visual Studio generator"
        $Generator = 'vs'
    }
    elseif ($ninjaCapable -ne $cmakeExe) {
        Write-Host ("   {0,-12} skipping {1}: no Ninja Multi-Config" -f 'cmake', $cmakeExe) -ForegroundColor DarkYellow
        $cmakeExe = $ninjaCapable
    }
}
Write-Found 'cmake' $cmakeExe

# Take the newest VS generator CMake offers at or below the installed VS major: the pinned
# CMake may not know the newest VS (3.31 has no "Visual Studio 18 2026").
$cmakeGenerators = @(
    & $cmakeExe --help 2>$null |
        Select-String -Pattern '^\s*\*?\s*(Visual Studio (\d+) \d+)' |
        ForEach-Object {
            [pscustomobject]@{
                Name  = $_.Matches[0].Groups[1].Value.Trim()
                Major = [int]$_.Matches[0].Groups[2].Value
            }
        }
)

$vsGenerator = $cmakeGenerators |
    Where-Object { $_.Major -le $vsMajor } |
    Sort-Object Major -Descending |
    Select-Object -First 1 -ExpandProperty Name

if (-not $vsGenerator) {
    throw ("CMake offers no Visual Studio generator at or below version $vsMajor. " +
           "Installed CMake is $((& $cmakeExe --version | Select-Object -First 1)); " +
           "either install a newer CMake or a Visual Studio it supports.")
}

Write-Found "VS $vsMajor" $vsPath

# Prefer Visual Studio's own Ninja over PATH, where unrelated toolchains (Strawberry Perl)
# put theirs; PATH is the fallback.
$ninjaExe = $null
if ($Generator -eq 'ninja') {
    $ninjaExe = @(
        (Join-Path $vsPath 'Common7\IDE\CommonExtensions\Microsoft\CMake\Ninja\ninja.exe')
        (Get-Command ninja -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source)
    ) | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1

    if (-not $ninjaExe) {
        Write-Missing 'ninja' 'not found, falling back to the Visual Studio generator'
        $Generator = 'vs'
    }
}

if ($Generator -eq 'ninja') {
    $cmakeGenerator = 'Ninja Multi-Config'
    Write-Found 'generator' "$cmakeGenerator ($ninjaExe)"
}
else {
    $cmakeGenerator = $vsGenerator
    Write-Found 'generator' $cmakeGenerator
}

# Enter the VS developer shell: the bundle's clang++ for the oneAPI kernel finds MSVC and
# the Windows SDK only through the environment. MSVC is pinned to 14.4x because nvcc
# rejects newer host compilers (CUDA 12.9 vs VS 18's default 14.51), and Ninja uses
# whichever cl.exe is on PATH.
$devShell = Join-Path $vsPath 'Common7\Tools\Microsoft.VisualStudio.DevShell.dll'
$msvcVer = @('14.44', '14.43', '14.42', '14.41', '14.40') |
    Where-Object {
        Get-ChildItem (Join-Path $vsPath 'VC\Tools\MSVC') -Directory -ErrorAction SilentlyContinue |
            Where-Object Name -Like "$_.*"
    } | Select-Object -First 1

if (-not $msvcVer) {
    Write-Missing 'MSVC 14.4x' ('not installed; the newest toolset will be used and a CUDA build ' +
                                'will likely be rejected by nvcc. Add "MSVC v143 - VS 2022 C++ ' +
                                'x64/x86 build tools" in the Visual Studio installer.')
}

if (Test-Path $devShell) {
    if (-not $env:VSINSTALLDIR) {
        $devCmdArgs = '-arch=x64 -host_arch=x64'
        if ($msvcVer) { $devCmdArgs += " -vcvars_ver=$msvcVer" }
        Import-Module $devShell
        Enter-VsDevShell -VsInstallPath $vsPath -SkipAutomaticLocation -DevCmdArguments $devCmdArgs | Out-Null
        Write-Found 'VS devshell' "entered (x64$(if ($msvcVer) { ", MSVC $msvcVer" }))"
    }
    else {
        Write-Found 'VS devshell' "already active ($env:VSINSTALLDIR)"
    }
}
else {
    Write-Missing 'VS devshell' "$devShell not found; the oneAPI kernel build will fail"
}

# ------------------------------------------------------------------ libraries

# Precompiled libraries: lib/<platform> (a git submodule), else a non-empty sibling
# ../lib (the 3.5-era layout), else fetch the pinned bundle.
$libModern = Join-Path $cyclesRoot 'lib\windows_x64'
$libLegacy = Join-Path (Split-Path -Parent $cyclesRoot) 'lib'

# lib/windows_x64 is 'update = none', so a fresh clone leaves it an empty directory:
# present means non-empty.
function Test-HasContent([string]$Dir) {
    (Test-Path -LiteralPath $Dir -PathType Container) -and
        [bool](Get-ChildItem -LiteralPath $Dir -Force | Select-Object -First 1)
}

Write-Step "Checking precompiled libraries"
if (Test-HasContent $libModern) {
    Write-Found 'libraries' $libModern
}
elseif (Test-HasContent $libLegacy) {
    Write-Found 'libraries' "$libLegacy (legacy layout)"
}
else {
    Write-Host "   Libraries missing - fetching the pinned bundle" -ForegroundColor Yellow
    $makeUpdate = Join-Path $cyclesRoot 'src\cmake\make_update.py'
    if (-not (Test-Path $makeUpdate)) { throw "make_update.py not found at '$makeUpdate'." }

    # Only checked here, so a developer who has the bundle never needs git or python.
    foreach ($tool in 'git', 'python') {
        if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
            throw ("'$tool' is needed to fetch the precompiled Cycles libraries and was " +
                   "not found on PATH. Run bootstrap.exe from the root of the Rhino repo.")
        }
    }
    # make_update.py checks out the pinned library commit; --no-cycles stops it running
    # 'git pull --rebase' on cycles-core. -WorkingDirectory, as Set-Location does not
    # reach a child process.
    $proc = Start-Process -FilePath 'python' -ArgumentList "`"$makeUpdate`"", '--no-cycles' `
        -WorkingDirectory $cyclesRoot -NoNewWindow -Wait -PassThru
    if ($proc.ExitCode -ne 0) { throw "Fetching the libraries failed with exit code $($proc.ExitCode)." }
    if (-not (Test-HasContent $libModern) -and -not (Test-HasContent $libLegacy)) {
        throw "make_update.py completed but no library folder appeared in '$libModern'."
    }
}

# ---------------------------------------------------------- library bundle pin
#
# Present is not enough: another bundle silently links other library versions.
# A mismatch stops the build unless -AllowLibraryMismatch.
$bundleCheck = Join-Path $cyclesRoot 'tools\check_lib_bundle.ps1'
if (Test-Path $bundleCheck) {
    & $bundleCheck -CyclesRoot $cyclesRoot
    $bundleExit = $LASTEXITCODE
    if ($bundleExit -eq 1 -or $bundleExit -eq 2) {
        if ($AllowLibraryMismatch) {
            Write-Host "   continuing because -AllowLibraryMismatch was given. This build is not what everyone else builds." -ForegroundColor Red
        }
        else {
            throw "lib\windows_x64 is not the pinned library bundle - see the banner above for the fix, or pass -AllowLibraryMismatch to build against it anyway."
        }
    }
}

# -------------------------------------------------------------------- devices

Write-Step "Detecting GPU toolkits"

$detected = [System.Collections.Generic.List[string]]::new()

# Probes treat an environment variable naming an uninstalled toolkit as unset, and
# fall back to the on-disk search.

# Enumerate every CUDA toolkit rather than trust CUDA_PATH, which names whichever was
# installed last, possibly the older one. CUDA_PATH_V* entries go stale too.
function Get-CudaToolkits {
    $candidates = [System.Collections.Generic.List[string]]::new()
    foreach ($scope in 'Machine', 'User') {
        $vars = [Environment]::GetEnvironmentVariables($scope)
        foreach ($k in $vars.Keys) { if ($k -like 'CUDA_PATH*') { $candidates.Add($vars[$k]) } }
    }
    if ($env:CUDA_PATH) { $candidates.Add($env:CUDA_PATH) }
    Get-ChildItem 'C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA' -Directory -ErrorAction SilentlyContinue |
        ForEach-Object { $candidates.Add($_.FullName) }

    $found = @{}
    foreach ($c in ($candidates | Where-Object { $_ } | Select-Object -Unique)) {
        $nvcc = Join-Path $c 'bin\nvcc.exe'
        if (-not (Test-Path $nvcc)) { continue }
        $out = & $nvcc --version 2>$null | Select-String 'release ([0-9]+)\.([0-9]+)'
        if (-not $out) { continue }
        $ver = [version]("{0}.{1}" -f $out.Matches[0].Groups[1].Value, $out.Matches[0].Groups[2].Value)
        $key = $c.TrimEnd('\')
        if (-not $found.ContainsKey($key)) { $found[$key] = $ver }
    }
    $found.GetEnumerator() | Sort-Object Value -Descending |
        ForEach-Object { [pscustomobject]@{ Path = $_.Key; Version = $_.Value } }
}

$cudaToolkits = @(Get-CudaToolkits)
$cudaPath = ($cudaToolkits | Select-Object -First 1).Path
if ($cudaPath) {
    $detected.Add('cuda')
    Write-Found 'CUDA' ("{0}  (v{1})" -f $cudaPath, ($cudaToolkits | Select-Object -First 1).Version)
}
else { Write-Missing 'CUDA' 'set CUDA_PATH to enable' }

# Optional CUDA 11: Cycles builds the compute_7x PTX fallback with it when present,
# which keeps the minimum driver low.
$cuda11 = $cudaToolkits | Where-Object { $_.Version.Major -eq 11 } | Select-Object -First 1
$cuda11Path = if ($cuda11) { $cuda11.Path } else { $null }
if ($cuda11Path) { Write-Found 'CUDA 11' ("{0}  (v{1})" -f $cuda11Path, $cuda11.Version) }
else { Write-Missing 'CUDA 11' 'optional; default PTX kernel will raise the minimum driver version' }

# Cycles 5's OptiX kernels need OptiX 8.0+ (optixTraverse, optixHitObject*). An older
# SDK is treated as absent, so OptiX is compiled out rather than failing the kernel build.
$optixMinVersion = [version]'8.0.0'
function Get-OptixVersion([string]$root) {
    $header = Join-Path $root 'include\optix.h'
    if (-not (Test-Path $header)) { return $null }
    $m = Select-String -Path $header -Pattern '^#define OPTIX_VERSION\s+(\d+)' | Select-Object -First 1
    if (-not $m) { return $null }
    $v = [int]$m.Matches[0].Groups[1].Value
    return [version]('{0}.{1}.{2}' -f [math]::Floor($v / 10000), [math]::Floor(($v % 10000) / 100), ($v % 100))
}

$optixSdks = @(
    $(if ($env:OPTIX_ROOT_DIR) { $env:OPTIX_ROOT_DIR })
    Get-ChildItem 'C:\ProgramData\NVIDIA Corporation' -Directory -Filter 'OptiX SDK *' -ErrorAction SilentlyContinue |
        ForEach-Object FullName
) | Where-Object { $_ -and (Test-Path $_) } | Select-Object -Unique |
    ForEach-Object { [pscustomobject]@{ Path = $_; Version = Get-OptixVersion $_ } }

# OPTIX_ROOT_DIR wins when it is new enough; otherwise the newest installed SDK.
$optixSdk = @($optixSdks | Where-Object { $_.Path -eq $env:OPTIX_ROOT_DIR -and $_.Version -ge $optixMinVersion }) +
            @($optixSdks | Where-Object { $_.Version -ge $optixMinVersion } | Sort-Object Version -Descending) |
    Select-Object -First 1
$optixPath = if ($optixSdk) { $optixSdk.Path } else { $null }

if ($optixPath) { $detected.Add('optix'); Write-Found 'OptiX' ("{0}  (v{1})" -f $optixPath, $optixSdk.Version) }
else {
    $tooOld = $optixSdks | Where-Object { $_.Version } | ForEach-Object { "v$($_.Version)" }
    if ($tooOld) { Write-Missing 'OptiX' ("only {0} found, {1}+ needed; set OPTIX_ROOT_DIR to enable" -f ($tooOld -join ', '), $optixMinVersion) }
    else { Write-Missing 'OptiX' 'set OPTIX_ROOT_DIR to enable' }
}

# HIP: prefer ROCm 6.x, which Blender builds and tests against; 7.x is used as-is.
$hipPath = $env:HIP_PATH
if (-not $hipPath -or -not (Test-Path $hipPath)) { $hipPath = $null }
$hipCandidates = @()
foreach ($base in 'C:\rocm', 'C:\Program Files\AMD\ROCm') {
    $hipCandidates += Get-ChildItem $base -Directory -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName
}
$hip6 = $hipCandidates | Where-Object { (Split-Path -Leaf $_) -like '6.*' } | Sort-Object -Descending | Select-Object -First 1

# An explicitly set HIP_PATH wins, so a different SDK can be tried.
if ($env:HIP_PATH -and (Test-Path $env:HIP_PATH)) {
    $hipPath = $env:HIP_PATH
    Write-Host ("   {0,-12} using HIP_PATH as given: {1}" -f 'HIP', $hipPath) -ForegroundColor DarkYellow
}
elseif (-not $hipPath) { $hipPath = $hip6 }
# HIP is in the default set: kernel errors inside AMD's amd_hip_vector_types.h were ours,
# not ROCm's (see svm_rhino_procedurals.h).
if ($hipPath -and (Test-Path $hipPath)) {
    $hipAvailable = $true
    $detected.Add('hip')
    Write-Found 'HIP' $hipPath
}
else { $hipAvailable = $false; Write-Missing 'HIP' 'set HIP_PATH to enable' }

$levelZeroRoot = @($libModern, $libLegacy) |
    ForEach-Object { Join-Path $_ 'level-zero' } |
    Where-Object { Test-Path $_ } | Select-Object -First 1
# oneAPI is enabled when detected, deliberately, despite RH-91240 (crash on exit), which
# the 4.4 line hid by turning oneAPI off without finding the cause. The kernel build's
# undefined intel_* warnings are a starting point. -Devices cpu,cuda,optix,hip skips it
# (from PowerShell only; through powershell -File, as ccycles.vcxproj does, it is one string).
if ($levelZeroRoot) { $detected.Add('oneapi'); Write-Found 'oneAPI' $levelZeroRoot }
else { Write-Missing 'oneAPI' 'level-zero not present in the library bundle' }

# CYCLES_DEVICES (e.g. cpu,hip) restricts the device set through the environment, the only
# route when ccycles.vcxproj calls this through powershell -File. Changing it forces one full rebuild.
if (-not $Devices -and $env:CYCLES_DEVICES) {
    $Devices = @($env:CYCLES_DEVICES -split '[,;\s]+' | Where-Object { $_ })
    Write-Host "CYCLES_DEVICES=$($env:CYCLES_DEVICES): restricting the device set"
}

if (-not $Devices) {
    $Devices = if ($detected.Count) { $detected.ToArray() } else { @('cpu') }
    Write-Host "   -> kernels for: $($Devices -join ', ')" -ForegroundColor Cyan
}
else {
    Write-Host "   -> requested: $($Devices -join ', ')" -ForegroundColor Cyan
    foreach ($d in $Devices) {
        # Named devices throw rather than drop the backend, so publish_payload.ps1 stops
        # instead of shipping a partial payload. The messages say what to install.
        if ($d -eq 'hip' -and -not $hipAvailable) {
            throw ("'hip' was requested but no ROCm install was found. Install the HIP SDK " +
                   "for Windows - no AMD hardware is needed, hipcc cross-compiles - from " +
                   "https://www.amd.com/en/developer/resources/rocm-hub/hip-sdk.html, or " +
                   "point HIP_PATH at an existing one. Cycles builds against ROCm 6.x. " +
                   "Drop 'hip' from -Devices to build without AMD kernels, but note that a " +
                   "payload without them is not publishable.")
        }
        # OptiX needs its headers (host code) and nvcc (PTX kernels). Checked against the
        # detected paths, since the device decisions are made below.
        if ($d -eq 'optix' -and -not ($optixPath -and $cudaPath)) {
            throw ("'optix' was requested but " + $(if (-not $optixPath) {
                       "no OptiX SDK $optixMinVersion+ headers were found. bootstrap.exe /cycles " +
                       "fetches them from NVIDIA, or set OPTIX_ROOT_DIR"
                   } else {
                       "no CUDA toolkit was found, and the OptiX kernels are PTX built by " +
                       "nvcc. bootstrap.exe /cycles installs it"
                   }) + ".")
        }
        if ($d -ne 'cpu' -and $detected -notcontains $d) {
            throw ("'$d' was requested but its toolkit was not detected. bootstrap.exe " +
                   "/cycles installs the GPU SDKs, or set the matching environment " +
                   "variable, or drop it from -Devices.")
        }
    }
}

# -------------------------------------------------------- device support vs kernels
#
# CUDA and HIP device support is host code that loads the driver APIs dynamically (cuew,
# hipew), so it is always compiled in and uses the payload's kernels if this build makes
# none; kernels need nvcc and hipcc. OptiX needs its SDK headers and nvcc. oneAPI's device
# and kernel are one DLL (cycles_kernel_oneapi_jit.dll), so it cannot be split.
$deviceCuda  = $true
$deviceHip   = $true
$deviceOneApi = [bool]$levelZeroRoot
$deviceOptix = [bool]($optixPath -and $cudaPath)

if (-not $optixPath) {
    Write-Missing 'OptiX device' "no OptiX $optixMinVersion+ SDK; OptiX support is compiled out of this build"
}
elseif (-not $cudaPath) {
    Write-Missing 'OptiX device' 'OptiX kernels are built by nvcc and no CUDA toolkit was found'
}

$kernelCuda  = ($Devices -contains 'cuda') -and [bool]$cudaPath
$kernelHip   = ($Devices -contains 'hip') -and [bool]$hipPath
$kernelOptix = ($Devices -contains 'optix') -and $deviceOptix

# ------------------------------------------------------- which architectures to build
#
# By default only this machine's GPUs, since no others can be tested here; -AllArches
# builds the shipping set, for publishing only. amdgpu-arch and nvidia-smi name the
# architectures; adapter enumeration cannot.
#
# Which GPU vendors are present: adapter enumeration answers that with no SDK installed,
# which is exactly the case worth warning about.
function Get-LocalGpuVendors {
    $vendors = [System.Collections.Generic.HashSet[string]]::new()
    $adapters = @(Get-CimInstance Win32_VideoController -ErrorAction SilentlyContinue)

    foreach ($a in $adapters) {
        $text = "$($a.AdapterCompatibility) $($a.Name)"
        if ($text -match 'NVIDIA') { [void]$vendors.Add('nvidia') }
        if ($text -match 'Advanced Micro Devices|\bAMD\b|Radeon') { [void]$vendors.Add('amd') }
        if ($text -match '\bIntel\b') { [void]$vendors.Add('intel') }
    }

    return $vendors
}

function Get-LocalHipArches {
    if (-not $hipPath) { return @() }
    $exe = Join-Path $hipPath 'bin\amdgpu-arch.exe'
    if (-not (Test-Path $exe)) { return @() }
    $out = & $exe 2>$null
    if ($LASTEXITCODE -ne 0) { return @() }
    return @($out | ForEach-Object { $_.Trim() } | Where-Object { $_ -match '^gfx[0-9a-f]+$' } | Select-Object -Unique)
}

function Get-LocalCudaArches {
    $smi = Get-Command nvidia-smi -ErrorAction SilentlyContinue
    if (-not $smi) { return @() }
    $out = & $smi.Source --query-gpu=compute_cap --format=csv,noheader 2>$null
    if ($LASTEXITCODE -ne 0) { return @() }
    return @($out |
        ForEach-Object { $_.Trim() } |
        Where-Object { $_ -match '^(\d+)\.(\d+)$' } |
        ForEach-Object { "sm_$($Matches[1])$($Matches[2])" } |
        Select-Object -Unique)
}

# MSVC 14.4x is required only when kernels are built: nvcc rejects newer MSVC and ROCm
# 6.4's clang cannot parse 14.51's cmath, both failing deep inside compiler headers.
if (-not $msvcVer -and ($kernelCuda -or $kernelHip -or $kernelOptix)) {
    throw ("MSVC 14.4x is not installed, and this build compiles GPU kernels, which will " +
           "fail inside the compilers' own headers rather than anywhere useful. nvcc " +
           "rejects MSVC newer than 2022, and ROCm 6.4's clang cannot parse 14.51's " +
           "cmath. Install 'MSVC v143 - VS 2022 C++ x64/x86 build tools' in the Visual " +
           "Studio installer, or run bootstrap.exe from the root of the Rhino repo, " +
           "which applies Rhino's .vsconfig and includes it. To build without kernels " +
           "instead, pass -Devices cpu.")
}

$hipArches = @()
$cudaArches = @()

# Shipping architecture lists, shared with publish_payload.ps1 so they cannot drift.
. (Join-Path $cyclesRoot 'kernel_arches.ps1')

if ($AllArches) {
    if ($kernelHip) {
        $hipArches = $CyclesHipShippingArches
        Write-Found 'HIP arch' "$($hipArches.Count) shipping targets"
    }
    if ($kernelCuda) {
        $cudaArches = $CyclesCudaShippingArches
        Write-Found 'CUDA arch' "$($cudaArches.Count) shipping targets"
    }
}
else {
    if ($kernelHip) {
        $hipArches = Get-LocalHipArches
        if ($hipArches.Count) { Write-Found 'HIP arch' ($hipArches -join ', ') }
        else {
            Write-Missing 'HIP arch' 'no AMD GPU found by amdgpu-arch; skipping HIP kernels'
            $kernelHip = $false
        }
    }
    if ($kernelCuda) {
        $cudaArches = Get-LocalCudaArches
        if ($cudaArches.Count) { Write-Found 'CUDA arch' ($cudaArches -join ', ') }
        else {
            Write-Missing 'CUDA arch' 'no NVIDIA GPU found by nvidia-smi; skipping CUDA kernels'
            $kernelCuda = $false
            # OptiX PTX needs no narrowing, but with no NVIDIA card there is nothing to
            # test it on; the payload's OptiX kernels keep working.
            if ($kernelOptix) {
                Write-Missing 'OptiX kernels' 'no NVIDIA GPU present; using the ones in the payload'
                $kernelOptix = $false
            }
        }
    }
}

# ------------------------------------------------------------------ configure

$cmakeConfig = if ($Configuration -eq 'Release') { 'RelWithDebInfo' } else { $Configuration }

if (-not $InstallDir) {
    # cycles-core -> RDK -> Plug-ins -> rhino4 -> src4
    $src4 = Resolve-Path (Join-Path $cyclesRoot '..\..\..\..') -ErrorAction SilentlyContinue
    if ($src4 -and (Test-Path (Join-Path $src4 'rhino4'))) {
        $InstallDir = Join-Path $src4 "bin\$Configuration\Plug-ins"
    }
    else {
        $InstallDir = Join-Path $cyclesRoot 'install'
    }
}
elseif ($InstallDir -notmatch '^([A-Za-z]:[\\/]|\\\\)') {
    # IsPathRooted is not enough: "C:Users\..." is relative to drive C's current
    # directory. Require a drive plus separator, or a UNC path.
    throw "-InstallDir must be an absolute path, got '$InstallDir'. If this came from a build script, check that backslashes survived quoting - forward slashes are safest."
}

# ---------------------------------------------------------------- payload guard
#
# A narrow local build must not replace the shared payload in big_libs that
# ccycles.vcxproj installs into, so it goes to a sibling gitignored "local" payload,
# which RhinoCyclesCore prefers. The debug payload is gitignored already. -Force
# overwrites and deletes the manifest, which would no longer be true.
if (-not $AllArches) {
    $targetManifest = Join-Path $InstallDir 'ccycles_payload.json'
    $targetIsLocal = (Split-Path -Leaf $InstallDir) -eq 'debug'

    if ((Test-Path $targetManifest) -and -not $targetIsLocal) {
        $existing = Get-Content -LiteralPath $targetManifest -Raw | ConvertFrom-Json

        # Narrower means the payload names a kernel this build will not produce.
        $builtHip = if ($kernelHip) { @($hipArches) } else { @() }
        $builtCuda = if ($kernelCuda) { @($cudaArches) } else { @() }
        $builtOptix = if ($kernelOptix) { @($existing.arches.optix) } else { @() }

        $shortfall = @()
        $shortfall += @($existing.arches.hip | Where-Object { $builtHip -notcontains $_ })
        $shortfall += @($existing.arches.cuda | Where-Object { $builtCuda -notcontains $_ })
        $shortfall += @($existing.arches.optix | Where-Object { $builtOptix -notcontains $_ })

        if ($shortfall.Count) {
            if ($Force) {
                Write-Step "Overwriting the committed payload (-Force)"
                Write-Host ("   {0,-12} {1}" -f 'manifest', 'removed; this payload no longer holds what it described') -ForegroundColor DarkYellow
                Remove-Item -LiteralPath $targetManifest -Force
            }
            else {
                $localDir = Join-Path (Split-Path -Parent $InstallDir) 'local'
                Write-Step "Installing to the local payload instead"
                Write-Host ("   this build makes {0} fewer kernel(s) than the payload in" -f $shortfall.Count) -ForegroundColor DarkYellow
                Write-Host  "   big_libs holds, so it would leave a payload that no longer" -ForegroundColor DarkYellow
                Write-Host  "   matches its manifest. Writing a local one, which your Rhino" -ForegroundColor DarkYellow
                Write-Host  "   prefers and git ignores." -ForegroundColor DarkYellow
                Write-Host ""
                Write-Host  "   publish_payload.ps1 builds the full set; -Force overwrites." -ForegroundColor DarkGray
                $InstallDir = $localDir
            }
        }
    }
}

Write-Step "Configuring ($cmakeConfig)"
Write-Host "   install -> $(ConvertTo-CMakePath $InstallDir)"

if (-not [System.IO.Path]::IsPathRooted($BuildDir)) { $BuildDir = Join-Path $cyclesRoot $BuildDir }

# CMake refuses a build directory made by another generator, confusingly; switching
# -Generator is normal, so clear it.
$cacheFile = Join-Path $BuildDir 'CMakeCache.txt'
if (Test-Path $cacheFile) {
    $match = Select-String -Path $cacheFile -Pattern '^CMAKE_GENERATOR:INTERNAL=(.*)$' |
        Select-Object -First 1
    $existing = if ($match) { $match.Matches[0].Groups[1].Value } else { $null }
    if ($existing -and $existing -ne $cmakeGenerator) {
        Write-Host "   regenerating: was '$existing', now '$cmakeGenerator'" -ForegroundColor DarkYellow
        Remove-Item -LiteralPath $BuildDir -Recurse -Force
    }
}

$cmakeArgs = @(
    '-S', (ConvertTo-CMakePath $cyclesRoot)
    '-B', (ConvertTo-CMakePath $BuildDir)
    '-G', $cmakeGenerator
    "-DCMAKE_INSTALL_PREFIX=$(ConvertTo-CMakePath $InstallDir)"
    '-DWITH_CYCLES_ALEMBIC=OFF'
    '-DWITH_CYCLES_USD=OFF'
    '-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF'
    # Rhino has its own denoisers (ccsession.cpp sets DENOISER_NONE), and RDK ships its
    # own OIDN; this one added 62 MB to the payload.
    '-DWITH_CYCLES_OPENIMAGEDENOISE=OFF'
    # SVM only (ccsession.cpp forces SHADINGSYSTEM_SVM); OSL added 72 MB of DLLs.
    '-DWITH_CYCLES_OSL=OFF'
)

# CYCLES_NATIVE_ONLY=1 builds the CPU kernel for this machine only, not every SIMD variant:
# much faster kernel iteration, but not portable. Changing it forces one full rebuild.
if ($env:CYCLES_NATIVE_ONLY -eq '1') {
    Write-Host 'CYCLES_NATIVE_ONLY=1: building only this machine CPU architecture'
    $cmakeArgs += '-DWITH_CYCLES_NATIVE_ONLY=ON'
}
else {
    $cmakeArgs += '-DWITH_CYCLES_NATIVE_ONLY=OFF'
}

if ($Generator -eq 'ninja') {
    # Ninja rejects -A; the x64 developer shell already put the right cl.exe on PATH.
    $cmakeArgs += "-DCMAKE_MAKE_PROGRAM=$(ConvertTo-CMakePath $ninjaExe)"
}
else {
    $cmakeArgs += '-A', 'x64'
}

# Every device switch is set ON or OFF explicitly, or the CMake cache keeps whatever an
# earlier configure enabled.

if ($deviceOptix) {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_OPTIX=ON', "-DOPTIX_ROOT_DIR=$(ConvertTo-CMakePath $optixPath)"
} else {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_OPTIX=OFF'
}

# CUDA device support is unconditional (cuew loads the driver at runtime). It also keeps
# find_package(CUDA) running, which the OptiX kernels need.
$cmakeArgs += '-DWITH_CYCLES_DEVICE_CUDA=ON'

# Upstream builds OptiX PTX only with WITH_CYCLES_CUDA_BINARIES, so OptiX kernels alone
# need it too; the arch list stays cheap, as OptiX PTX needs no cubins.
if ($kernelCuda -or $kernelOptix) {
    $cmakeArgs += '-DWITH_CYCLES_CUDA_BINARIES=ON'
    # Explicit toolkit: FindCUDA trusts CUDA_PATH, and a stale one silently turns CUDA
    # binaries off.
    $cmakeArgs += "-DCUDA_TOOLKIT_ROOT_DIR=$(ConvertTo-CMakePath $cudaPath)"
    if ($cuda11Path) {
        $cmakeArgs += "-DCUDA11_TOOLKIT_ROOT_DIR=$(ConvertTo-CMakePath $cuda11Path)"
        $cmakeArgs += "-DCUDA11_NVCC_EXECUTABLE=$(ConvertTo-CMakePath (Join-Path $cuda11Path 'bin/nvcc.exe'))"
    }
    if ($kernelCuda -and $cudaArches.Count) {
        # This machine's cards only.
        $cmakeArgs += "-DCYCLES_CUDA_BINARIES_ARCH=$($cudaArches -join ';')"
    }
    elseif (-not $CudaBinaries) {
        # One PTX kernel the driver JITs for any card; also the OptiX-only case, since
        # nvcc still needs an architecture.
        $cmakeArgs += '-DCYCLES_CUDA_BINARIES_ARCH=compute_52'
    }
    # else: -CudaBinaries with no narrowing leaves the upstream default list.
} else {
    $cmakeArgs += '-DWITH_CYCLES_CUDA_BINARIES=OFF'
    if ($cudaPath) { $cmakeArgs += "-DCUDA_TOOLKIT_ROOT_DIR=$(ConvertTo-CMakePath $cudaPath)" }
}

# HIP device support is unconditional for the same reason: external_libs.cmake forces
# WITH_HIP_DYNLOAD ON, so device/hip links against the bundled hipew and needs no ROCm.
$cmakeArgs += '-DWITH_CYCLES_DEVICE_HIP=ON'

if ($kernelHip) {
    $cmakeArgs += "-DHIP_ROOT_DIR=$(ConvertTo-CMakePath $hipPath)"
    # WITH_CYCLES_HIP_BINARIES defaults to OFF: the device alone builds no HIP kernels.
    $cmakeArgs += '-DWITH_CYCLES_HIP_BINARIES=ON'
    if ($hipArches.Count) {
        $cmakeArgs += "-DCYCLES_HIP_BINARIES_ARCH=$($hipArches -join ';')"
    }
} else {
    $cmakeArgs += '-DWITH_CYCLES_HIP_BINARIES=OFF'
    if ($hipPath) { $cmakeArgs += "-DHIP_ROOT_DIR=$(ConvertTo-CMakePath $hipPath)" }
}

if ($deviceOneApi) {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_ONEAPI=ON'
    $cmakeArgs += "-D_LEVEL_ZERO_INCLUDE_DIR=$(ConvertTo-CMakePath (Join-Path $levelZeroRoot 'include'))"
    $cmakeArgs += "-D_LEVEL_ZERO_LIBRARY=$(ConvertTo-CMakePath (Join-Path $levelZeroRoot 'lib'))"
} else {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_ONEAPI=OFF'
}

Write-Host "   cmake $($cmakeArgs -join ' ')" -ForegroundColor DarkGray

& $cmakeExe @cmakeArgs
if ($LASTEXITCODE -ne 0) { throw "CMake configure failed with exit code $LASTEXITCODE." }

if ($ConfigureOnly) {
    if ($Generator -eq 'ninja') {
        Write-Step "Configured. Build with -Generator vs if you want a Cycles.sln to open."
    }
    else {
        Write-Step "Configured. Open $BuildDir\Cycles.sln in Visual Studio, or re-run without -ConfigureOnly."
    }
    return
}

# Jobs from total memory, not cores: each HIP clang peaks near 1.2 GB and Ninja would
# start them all at once. Total rather than free memory, so the count is reproducible.
# -Jobs overrides.
if (-not $Jobs) {
    $cores = [int]$env:NUMBER_OF_PROCESSORS
    if (-not $cores) { $cores = 4 }

    $totalMB = 0
    try {
        $totalMB = [int]((Get-CimInstance Win32_ComputerSystem -ErrorAction Stop).TotalPhysicalMemory / 1MB)
    }
    catch {
        # No CIM - fall back to the old flat cap rather than guessing wildly.
        $totalMB = 0
    }

    if ($totalMB -gt 0) {
        # Reserve the oneAPI link's ~8 GB (one job, which Ninja cannot budget) plus room
        # for the OS, and divide the rest by the per-job peak; +1 is the oneAPI job.
        # Documented peaks, not measured here.
        $reserveMB = 12288  # oneAPI link ~8 GB, plus ~4 GB for the OS and an editor
        $perJobMB = 1228    # a HIP clang peaks near 1.2 GB
        $byMemory = [Math]::Floor(($totalMB - $reserveMB) / $perJobMB) + 1
        $Jobs = [Math]::Max(2, [Math]::Min($cores - 2, $byMemory))
        Write-Host ("   {0,-12} {1} jobs ({2} GB, {3} cores)" -f 'parallelism', $Jobs,
                    [Math]::Round($totalMB / 1024), $cores) -ForegroundColor DarkGray

        # Below ~14 GB the floor of two jobs wins; warn, as swapping through the oneAPI
        # link looks like a hang.
        if ($byMemory -lt 2) {
            Write-Missing 'memory' ("{0} GB is below what a full Cycles build wants; expect swapping" -f
                                    [Math]::Round($totalMB / 1024))
        }
    }
    else {
        $Jobs = [Math]::Max(2, [Math]::Min($cores - 2, 12))
    }
}

Write-Step "Building ($cmakeConfig, $Jobs jobs)"
# Teed to a log so a failure can be summarised; in Visual Studio it is a wall of output.
# EAP Continue: Windows PowerShell turns redirected stderr into errors, and Stop would end here.
$buildLog = Join-Path $BuildDir 'build_cycles_last.log'
$eap = $ErrorActionPreference
$ErrorActionPreference = 'Continue'
& $cmakeExe --build $BuildDir --config $cmakeConfig --target install --parallel $Jobs 2>&1 |
    ForEach-Object { "$_" } | Tee-Object -FilePath $buildLog
$buildExit = $LASTEXITCODE
$ErrorActionPreference = $eap
if ($buildExit -ne 0) {
    # MSBuild turns a line in this form into an Error List entry.
    $logLines = @(Get-Content -LiteralPath $buildLog -ErrorAction SilentlyContinue)
    $failed = @($logLines | Where-Object { $_ -like 'FAILED: *' } |
        ForEach-Object { Split-Path -Leaf ((($_ -replace '^FAILED: ', '') -split ' ')[0]) } | Select-Object -Unique)
    $firstError = $logLines | Where-Object { $_ -notlike 'FAILED: *' -and $_ -match '(?i)(:\s*(fatal )?error\b|\berror [A-Z]+\d+:)' } |
        Select-Object -First 1
    $what = if ($failed.Count) { "at $($failed -join ', ')" } else { "with exit code $buildExit" }
    $why = if ($firstError) { ": $($firstError.Trim().Substring(0, [Math]::Min(200, $firstError.Trim().Length)))" } else { '' }
    Write-Host "build_cycles.ps1 : error CYC0001: Cycles build failed $what$why. Full log: $buildLog"
    throw "Build failed with exit code $buildExit."
}

# SxS manifests (see fix-cycles-sxs.ps1), only on files this install wrote, as InstallDir
# may hold other plug-ins' DLLs. Every build: the install restores the original files.
$installManifest = Join-Path $BuildDir 'install_manifest.txt'
if (-not (Test-Path -LiteralPath $installManifest)) {
    throw "CMake wrote no install_manifest.txt in $BuildDir, so there is no telling which DLLs this install wrote."
}
$installRoot = [System.IO.Path]::GetFullPath($InstallDir).TrimEnd('\')
$installedDlls = @(Get-Content -LiteralPath $installManifest |
    Where-Object { $_ -like '*.dll' } |
    ForEach-Object { [System.IO.Path]::GetFullPath($_) } |
    Where-Object { [System.IO.Path]::GetDirectoryName($_) -ieq $installRoot })
if (-not $installedDlls.Count) { throw "install_manifest.txt names no DLL in $installRoot." }
Write-Step "Side-by-side manifests for the DLLs ccycles.dll loads"
& (Join-Path $cyclesRoot 'fix-cycles-sxs.ps1') -PayloadDir $InstallDir -Files $installedDlls

# CMake's install rules do not carry the HIP fatbins, so copy them - the .fatbin.zst
# ones, the only name HIPDevice::compile_kernel looks for.
$hipBuilt = Join-Path $BuildDir "src/kernel/device/hip"
if (Test-Path $hipBuilt) {
    $fatbins = @(Get-ChildItem $hipBuilt -Filter "*.fatbin.zst" -ErrorAction SilentlyContinue)
    if ($fatbins.Count -gt 0) {
        $libDir = Join-Path $InstallDir "lib"
        $null = New-Item -ItemType Directory -Force -Path $libDir
        $fatbins | Copy-Item -Destination $libDir -Force
        Write-Step "Deployed $($fatbins.Count) HIP fatbin(s) to $(ConvertTo-CMakePath $libDir)"
    }
    else {
        Write-Step "No HIP fatbins in the build tree - HIP will use whatever is already deployed"
    }
}

# --------------------------------------------------- inherit the kernels we did not build
#
# CUDA and HIP device support is always compiled in, but a debug or local payload starts
# empty, and a device with no kernels can take Rhino down. So copy in the committed
# payload's kernels this build did not make (never into release, which publish_payload.ps1
# owns), and say they predate local kernel edits.
function Copy-InheritedKernels {
    param(
        [Parameter(Mandatory)][string]$TargetPayload,
        [bool]$Hip,
        [bool]$Cuda,
        [bool]$Optix
    )

    if ((Split-Path -Leaf $TargetPayload) -eq 'release') { return 0 }

    # The committed payload in the Rhino tree first, then next to the target, since
    # -InstallDir can point anywhere.
    $candidates = @(
        (Join-Path $cyclesRoot '..\..\..\..\..\big_libs\RhinoCycles\ccycles\win\release\lib'),
        (Join-Path (Join-Path (Split-Path -Parent $TargetPayload) 'release') 'lib')
    )
    $releaseLib = $candidates | Where-Object { Test-Path $_ } | Select-Object -First 1
    if (-not $releaseLib) { return 0 }
    $releaseLib = (Resolve-Path $releaseLib).Path
    $targetLib = Join-Path $TargetPayload 'lib'

    $wanted = [System.Collections.Generic.List[string]]::new()
    if ($Hip) {
        foreach ($a in $CyclesHipShippingArches) { $wanted.Add("kernel_$a.fatbin.zst") }
    }
    if ($Cuda) {
        foreach ($a in $CyclesCudaShippingArches) {
            $ext = if ($a -like 'compute_*') { 'ptx' } else { 'cubin' }
            $wanted.Add("kernel_$a.$ext.zst")
        }
    }
    if ($Optix) {
        foreach ($m in $CyclesOptixModules) { $wanted.Add("$m.ptx.zst") }
    }

    $inherited = 0
    foreach ($name in $wanted) {
        if (Test-Path (Join-Path $targetLib $name)) { continue }
        $src = Join-Path $releaseLib $name
        if (-not (Test-Path $src)) { continue }
        $null = New-Item -ItemType Directory -Force -Path $targetLib
        Copy-Item -LiteralPath $src -Destination (Join-Path $targetLib $name) -Force
        $inherited++
    }

    return $inherited
}

# Fill CUDA and HIP whatever -Devices says: their device support is always compiled in.
# OptiX stays tied to its device, which is compiled out without the SDK.
$inherited = Copy-InheritedKernels -TargetPayload $InstallDir -Hip $true -Cuda $true -Optix $deviceOptix
if ($inherited) {
    Write-Step "Filled $inherited kernel(s) from the committed payload"
    Write-Host "   For devices this build supports but compiled no kernels for. They are as" -ForegroundColor DarkYellow
    Write-Host "   old as the last publish, so they do not contain local kernel changes." -ForegroundColor DarkYellow

    # Warn loudly when a GPU here runs inherited kernels, as a kernel edit then renders
    # identically. Not an error: host-side work needs no kernel compiler.
    $vendors = Get-LocalGpuVendors
    $blind = @()
    if ($vendors.Contains('amd') -and -not $kernelHip) {
        $blind += 'AMD (install the ROCm HIP SDK to build HIP kernels)'
    }
    if ($vendors.Contains('nvidia') -and -not $kernelCuda) {
        $blind += 'NVIDIA (install the CUDA toolkit to build CUDA and OptiX kernels)'
    }

    if ($blind.Count) {
        Write-Host ""
        Write-Host "   Note that a GPU in this machine will run inherited kernels:" -ForegroundColor Yellow
        foreach ($b in $blind) { Write-Host "     - $b" -ForegroundColor Yellow }
        Write-Host "   So a change to kernel code will NOT show up in renders on that GPU," -ForegroundColor Yellow
        Write-Host "   however many times you rebuild. bootstrap.exe /cycles installs these." -ForegroundColor Yellow
    }
}

# Notes for Rhino's kernel compile log (deployed beside ccycles.dll): what this build lacks
# for this machine's GPUs, and whether the committed payload's kernels come from other
# kernel sources - with this ccycles.dll those can render wrong or crash.
$committedManifest = Join-Path (Join-Path (Split-Path -Parent $InstallDir) 'release') 'ccycles_payload.json'
$recordedHash = $null
$treeHash = $null
if (Test-Path $committedManifest) {
    $recordedHash = (Get-Content -LiteralPath $committedManifest -Raw | ConvertFrom-Json).kernelSourceHash
    if ($recordedHash) { $treeHash = Get-CyclesKernelSourceHash -CyclesRoot $cyclesRoot }
}
$kernelSourcesDiffer = [bool]($recordedHash -and $treeHash -ne $recordedHash)

$notesFile = Join-Path $InstallDir 'ccycles_build_notes.txt'
if ((Split-Path -Leaf $InstallDir) -eq 'release') {
    # The committed payload ships to users; it must never carry one machine's notes.
    Remove-Item -LiteralPath $notesFile -Force -ErrorAction SilentlyContinue
}
else {
    $gpus = @(Get-LocalGpuVendors)
    $notes = [System.Collections.Generic.List[string]]::new()
    $risk = if ($kernelSourcesDiffer) { 'built from different kernel sources than this tree - renders may be wrong or Rhino may crash.' }
            else { 'so kernel edits do not show.' }

    if ($gpus -contains 'nvidia') {
        if (-not $deviceOptix) {
            $old = @($optixSdks | Where-Object { $_.Version } | ForEach-Object { "v$($_.Version)" })
            $why = if ($optixPath) { 'no CUDA toolkit found' }
                   elseif ($old.Count) { "OptiX SDK $($old -join ', ') is too old, $optixMinVersion+ needed" }
                   else { 'no OptiX SDK found' }
            $notes.Add("NVIDIA GPU: no OptiX device in this build - $why. Run bootstrap.exe /cycles.")
        }
        $stale = @()
        if (-not $kernelCuda) { $stale += 'CUDA' }
        if ($deviceOptix -and -not $kernelOptix) { $stale += 'OptiX' }
        if ($stale.Count) {
            $fix = if (-not $cudaPath) { ' Run bootstrap.exe /cycles.' } else { '' }
            $notes.Add("NVIDIA GPU: runs $($stale -join ' and ') kernels from the committed payload, $risk$fix")
        }
    }
    if ($gpus -contains 'amd' -and -not $kernelHip) {
        $fix = if (-not $hipPath) { ' Install the ROCm HIP SDK.' } else { '' }
        $notes.Add("AMD GPU: runs HIP kernels from the committed payload, $risk$fix")
    }
    # HIP_PATH is used as given, and Cycles is only tested against ROCm 6.x.
    $rocm = if ($hipPath) { Split-Path -Leaf $hipPath.TrimEnd('\') } else { '' }
    if ($gpus -contains 'amd' -and $kernelHip -and $rocm -match '^(\d+)\.\d+' -and $Matches[1] -ne '6') {
        $notes.Add("AMD GPU: HIP kernels built with ROCm $rocm ($hipPath); Cycles is tested against 6.x. If AMD renders look wrong, point HIP_PATH at a ROCm 6.x.")
    }
    if ($gpus -contains 'intel' -and -not $deviceOneApi) {
        $notes.Add('Intel GPU: no oneAPI device in this build - level-zero is missing from the library bundle.')
    }
    if (-not $notes.Count) { $notes.Add('All GPU kernels for this machine were built from this tree.') }

    $notes.Insert(0, ('Built locally {0} ({1}), payload "{2}".' -f (Get-Date).ToString('yyyy-MM-dd HH:mm'), $cmakeConfig, (Split-Path -Leaf $InstallDir)))
    $null = New-Item -ItemType Directory -Force -Path $InstallDir
    [System.IO.File]::WriteAllLines($notesFile, $notes)
}

Write-Step "Done - installed to $(ConvertTo-CMakePath $InstallDir)"

# Point at publish_payload.ps1, which nothing else announces, and say whether the committed
# payload's kernels still match this tree; if not, merging needs a republish.
# Informational only: the hard stop belongs on the pull request.
if (-not $AllArches) {
    Write-Host "   This built kernels for this machine only. To produce a payload for" -ForegroundColor DarkGray
    Write-Host "   everyone - every backend, every shipping architecture, checked and" -ForegroundColor DarkGray
    Write-Host "   staged in big_libs - run publish_payload.ps1." -ForegroundColor DarkGray

    if ($kernelSourcesDiffer) {
        Write-Host ""
        Write-Host "   The committed payload's kernels were built from different kernel" -ForegroundColor Yellow
        Write-Host "   sources than this tree has. If that is your change, the payload needs" -ForegroundColor Yellow
        Write-Host "   republishing before it merges - otherwise everyone on a plain build" -ForegroundColor Yellow
        Write-Host "   gets your ccycles.dll with the old kernels." -ForegroundColor Yellow
        Write-Host "     payload: $($recordedHash.Substring(0, 16))" -ForegroundColor DarkGray
        Write-Host "     tree:    $($treeHash.Substring(0, 16))" -ForegroundColor DarkGray
    }
}
