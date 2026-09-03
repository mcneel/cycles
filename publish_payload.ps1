#requires -Version 5.1
<#
.SYNOPSIS
    Builds the complete Cycles payload, checks it, and stages it in big_libs.

.DESCRIPTION
    The other half of build_cycles.ps1. That script serves a developer testing a
    change on their own hardware; this one produces what everyone else runs.

    Everything in a plain Rhino build comes from the payload in big_libs, so most
    developers never build Cycles at all. That only works if the payload is complete -
    and the ways it silently was not are what this script exists to prevent:

      * A build with an SDK missing quietly drops that backend. This script names every
        device explicitly, and build_cycles.ps1 throws on a missing toolkit rather than
        disabling it, so a partial publish fails instead of shipping.
      * A local build's architecture narrowing would ship one fatbin for the machine
        that ran it. -AllArches restores the full shipping set.
      * Nothing checked that the files were actually there. The payload shipped for a
        while with no CUDA cubins at all, and nobody noticed because no build step had
        an opinion about what a payload should contain.

    The check is by name against kernel_arches.ps1, the same lists the build was
    configured from, so it cannot pass by verifying against itself.

    It does not commit. It stages and prints the command, because publishing a payload
    is a deliberate act and the commit message should say which kernel change prompted
    it.

    This is also what the build agent will run when someone gets to it. Nothing here
    needs a GPU: nvcc and hipcc both cross-compile, and this machine builds all 22 HIP
    targets with a single AMD APU in it. Testing them is another matter, which is what
    tools/run_checks.ps1 -Render is for.

.PARAMETER Configuration
    Release (the default) writes the tracked payload. Debug writes the debug one, which
    is gitignored - useful for producing a full debug payload locally, but it is not
    something to publish.

.PARAMETER SkipBuild
    Check and stage what is already in the payload directory, without building. For
    re-running the checks, or after a build that was interrupted at the staging step.

.PARAMETER BuildDir
    Passed through to build_cycles.ps1. Defaults to a separate directory from the one
    a developer's own builds use, so a publish never invalidates their incremental
    build - the architecture lists differ, so sharing one directory would mean a full
    kernel rebuild in both directions, every time.

.EXAMPLE
    .\publish_payload.ps1

.EXAMPLE
    .\publish_payload.ps1 -SkipBuild
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release',

    [switch]$SkipBuild,

    [string]$BuildDir = 'build_publish'
)

$ErrorActionPreference = 'Stop'
$cyclesRoot = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }

. (Join-Path $cyclesRoot 'kernel_arches.ps1')

function Write-Step($msg) { Write-Host "`n== $msg" -ForegroundColor Cyan }
function Write-Ok($what, $detail) { Write-Host ("   {0,-22} {1}" -f $what, $detail) -ForegroundColor Green }
function Write-Warn($what, $detail) { Write-Host ("   {0,-22} {1}" -f $what, $detail) -ForegroundColor DarkYellow }
function Write-Bad($what, $detail) { Write-Host ("   {0,-22} {1}" -f $what, $detail) -ForegroundColor Red }

# ------------------------------------------------------------------------ locations

# cycles -> RDK -> Plug-ins -> rhino4 -> src4 -> repo root
$repoRoot = Resolve-Path (Join-Path $cyclesRoot '..\..\..\..\..')
$payloadName = if ($Configuration -eq 'Debug') { 'debug' } else { 'release' }
$payloadDir = Join-Path $repoRoot "big_libs\RhinoCycles\ccycles\win\$payloadName"
$libDir = Join-Path $payloadDir 'lib'

Write-Step "Publishing the $payloadName payload"
Write-Ok 'payload' $payloadDir

if ($Configuration -eq 'Debug') {
    Write-Warn 'note' 'the debug payload is gitignored; this will build one but not stage it'
}

# ---------------------------------------------------------------------------- build

if ($SkipBuild) {
    Write-Step "Skipping the build (-SkipBuild)"
}
else {
    # Every device named explicitly rather than left to detection. build_cycles.ps1
    # throws when a named toolkit is missing, which is exactly what publishing wants:
    # a machine that cannot produce a complete payload should stop, not ship a partial
    # one. Detection is for a developer's own build.
    #
    # Called as PowerShell rather than through pwsh -File, because a comma-separated
    # -Devices list arrives as a single string that way and ValidateSet rejects it.
    Write-Step "Building every backend for every shipping architecture"
    Write-Host "   this is the slow one - 22 HIP fatbins alone are about an hour" -ForegroundColor DarkGray

    & (Join-Path $cyclesRoot 'build_cycles.ps1') `
        -Configuration $Configuration `
        -Devices cpu, cuda, optix, hip, oneapi `
        -AllArches `
        -InstallDir $payloadDir `
        -BuildDir $BuildDir
}

# ---------------------------------------------------------------------------- verify

Write-Step "Checking the payload"

$missing = [System.Collections.Generic.List[string]]::new()

function Test-PayloadFile($relative, $label) {
    $full = Join-Path $payloadDir $relative
    if (Test-Path $full) { return $true }
    $missing.Add("$label ($relative)")
    return $false
}

# Host binaries. cycles_kernel_oneapi_jit.dll is the oneAPI kernel and the oneAPI
# device support in one artifact - there is no switch that produces one without the
# other - so its absence means Intel GPUs are unsupported in this payload.
$hostOk = $true
foreach ($f in 'ccycles.dll', 'cycles_kernel_oneapi_jit.dll') {
    if (-not (Test-PayloadFile $f 'host binary')) { $hostOk = $false }
}
if ($hostOk) { Write-Ok 'host binaries' 'ccycles.dll, cycles_kernel_oneapi_jit.dll' }

# HIP fatbins. Compressed only: HIPDevice::compile_kernel probes
# "lib/<name>_<arch>.fatbin.zst" and never looks at the uncompressed intermediate, so
# an uncompressed file in here is not a kernel as far as Cycles is concerned.
$hipFound = 0
foreach ($arch in $CyclesHipShippingArches) {
    if (Test-PayloadFile "lib\kernel_$arch.fatbin.zst" "HIP kernel $arch") { $hipFound++ }
}
if ($hipFound -eq $CyclesHipShippingArches.Count) {
    Write-Ok 'HIP kernels' "$hipFound / $($CyclesHipShippingArches.Count)"
}

# CUDA cubins and the PTX fallback. A virtual architecture (compute_*) produces .ptx,
# a real one (sm_*) produces .cubin.
$cudaFound = 0
foreach ($arch in $CyclesCudaShippingArches) {
    $ext = if ($arch -like 'compute_*') { 'ptx' } else { 'cubin' }
    if (Test-PayloadFile "lib\kernel_$arch.$ext.zst" "CUDA kernel $arch") { $cudaFound++ }
}
if ($cudaFound -eq $CyclesCudaShippingArches.Count) {
    Write-Ok 'CUDA kernels' "$cudaFound / $($CyclesCudaShippingArches.Count)"
}

# OptiX modules.
$optixFound = 0
foreach ($m in $CyclesOptixModules) {
    if (Test-PayloadFile "lib\$m.ptx.zst" "OptiX module $m") { $optixFound++ }
}
if ($optixFound -eq $CyclesOptixModules.Count) {
    Write-Ok 'OptiX modules' "$optixFound / $($CyclesOptixModules.Count)"
}

if ($missing.Count) {
    Write-Bad 'incomplete' "$($missing.Count) expected file(s) missing"
    $missing | Select-Object -First 25 | ForEach-Object { Write-Host "      - $_" -ForegroundColor Red }
    if ($missing.Count -gt 25) { Write-Host "      ... and $($missing.Count - 25) more" -ForegroundColor Red }
    throw ("This payload is incomplete and must not be committed. A missing backend " +
           "usually means its SDK was not found; a missing architecture means the build " +
           "of that kernel failed. Check the build log above rather than re-running.")
}

# -------------------------------------------------------------------------- manifest

Write-Step "Writing the manifest"

# The kernel source hash covers src/kernel *and* src/util. Cycles' own dependency
# tracking does not: the fatbin and cubin rules depend on cycles_kernel's interface
# sources, which is src/kernel only - but kernel/types.h includes util/projection.h and
# util/static_assert.h, so an edit under src/util changes the kernels without
# invalidating them. An incremental build will happily hand you stale kernels for it.
# Hashing both means the manifest notices what the build does not.
function Get-KernelSourceHash {
    $roots = @(
        (Join-Path $cyclesRoot 'src\kernel')
        (Join-Path $cyclesRoot 'src\util')
    )
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try {
        $buffer = [System.Text.StringBuilder]::new()
        foreach ($root in $roots) {
            Get-ChildItem $root -Recurse -File |
                Sort-Object FullName |
                ForEach-Object {
                    $rel = $_.FullName.Substring($cyclesRoot.Length).Replace('\', '/')
                    $fileHash = [BitConverter]::ToString($sha.ComputeHash([IO.File]::ReadAllBytes($_.FullName))).Replace('-', '')
                    [void]$buffer.AppendLine("$rel $fileHash")
                }
        }
        $bytes = [Text.Encoding]::UTF8.GetBytes($buffer.ToString())
        return [BitConverter]::ToString($sha.ComputeHash($bytes)).Replace('-', '').ToLowerInvariant()
    }
    finally { $sha.Dispose() }
}

# CYCLES_VERSION_STRING is composed from the numeric macros rather than being a literal,
# so read those. Note this reports 5.3.0 on a 5.2 tree and that is correct: upstream
# bumps the version straight after tagging, so src/util/version.h already reads 5.3.0 at
# the v5.2.0 tag itself, which is the commit this fork merged. Do not "fix" it to match
# the documents.
function Get-CyclesVersion {
    $header = Join-Path $cyclesRoot 'src\util\version.h'
    if (-not (Test-Path $header)) { return 'unknown' }
    $parts = foreach ($part in 'MAJOR', 'MINOR', 'PATCH') {
        $m = Select-String -Path $header -Pattern "^\s*#define\s+CYCLES_VERSION_$part\s+(\d+)" |
            Select-Object -First 1
        if ($m) { $m.Matches[0].Groups[1].Value } else { return 'unknown' }
    }
    return ($parts -join '.')
}

function Get-GitDescribe {
    Push-Location $cyclesRoot
    try {
        $sha = & git rev-parse --short HEAD 2>$null
        $branch = & git rev-parse --abbrev-ref HEAD 2>$null
        $dirty = & git status --porcelain 2>$null
        return [ordered]@{
            commit = if ($sha) { $sha.Trim() } else { 'unknown' }
            branch = if ($branch) { $branch.Trim() } else { 'unknown' }
            # A payload built from a dirty tree cannot be reproduced from the commit it
            # names, which is worth knowing later even though it is normal while
            # iterating.
            dirty  = [bool]$dirty
        }
    }
    finally { Pop-Location }
}

$manifest = [ordered]@{
    schema        = 1
    builtUtc      = (Get-Date).ToUniversalTime().ToString('o')
    configuration = $Configuration
    cyclesVersion = Get-CyclesVersion
    source        = Get-GitDescribe

    # What a payload contains, so that a build inheriting it can say what it supports
    # rather than guessing, and so a narrower payload can be recognised as narrower.
    devices       = @('cpu', 'cuda', 'optix', 'hip', 'oneapi')
    arches        = [ordered]@{
        hip   = $CyclesHipShippingArches
        cuda  = $CyclesCudaShippingArches
        optix = $CyclesOptixModules
    }

    # Compare this against a fresh Get-KernelSourceHash to know whether the kernels in
    # a payload were built from the kernel sources currently in the tree. That is the
    # check that stops a kernel change merging without a republish, and the one that
    # tells a developer their local HIP kernels are now stale.
    kernelSourceHash = Get-KernelSourceHash
}

$manifestPath = Join-Path $payloadDir 'ccycles_payload.json'
$manifest | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $manifestPath -Encoding UTF8
Write-Ok 'manifest' $manifestPath
Write-Ok 'kernel source hash' $manifest.kernelSourceHash.Substring(0, 16)

# ------------------------------------------------------------------- version resources

# openvdb.dll and cycles_kernel_oneapi_jit.dll come out of the precompiled Blender
# library bundle, so we ship them without building them and the only way to attach
# version resources is after the fact. ccycles.dll does not need this - its VERSIONINFO
# and SxS manifest are compiled in at link time.
#
# This step had no caller at all after make_rhino_all.ps1 stopped being used, so those
# two DLLs were shipping unstamped. It needs ResourceHacker, which is not part of any
# other requirement here, so a missing one is a warning rather than a failure.
Write-Step "Stamping version resources"

$versionScript = Join-Path $cyclesRoot 'versioninfo_changer.ps1'
if (-not (Test-Path $versionScript)) {
    Write-Warn 'skipped' 'versioninfo_changer.ps1 not found'
}
elseif (-not (Get-Command 'ResourceHacker' -ErrorAction SilentlyContinue) -and
        -not (Get-Command 'ResourceHacker.exe' -ErrorAction SilentlyContinue)) {
    Write-Warn 'skipped' 'ResourceHacker is not on PATH; openvdb.dll and the oneAPI JIT DLL will ship unstamped'
}
else {
    & $versionScript -InstallDir $payloadDir
    if ($LASTEXITCODE -ne 0) { Write-Warn 'failed' "versioninfo_changer.ps1 exited $LASTEXITCODE" }
    else { Write-Ok 'stamped' 'openvdb.dll, cycles_kernel_oneapi_jit.dll' }
}

# --------------------------------------------------------------------------- staging

if ($Configuration -eq 'Debug') {
    Write-Step "Done - debug payload built and checked, nothing to stage (it is gitignored)"
    return
}

Write-Step "Staging in big_libs"

Push-Location $repoRoot
try {
    & git add -- 'big_libs/RhinoCycles/ccycles/win/release'
    $staged = @(& git diff --cached --name-only -- 'big_libs/RhinoCycles/ccycles/win/release')
    Write-Ok 'staged' "$($staged.Count) file(s)"
}
finally { Pop-Location }

Write-Host ""
Write-Host "Payload is complete and staged. Commit it with a message naming the kernel" -ForegroundColor Cyan
Write-Host "change that made it necessary, then open a PR:" -ForegroundColor Cyan
Write-Host ""
Write-Host "    git -C `"$repoRoot`" commit -m `"Cycles: republish the payload for <change>`"" -ForegroundColor White
Write-Host ""
