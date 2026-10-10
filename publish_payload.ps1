#requires -Version 5.1
<#
.SYNOPSIS
    Builds the complete Cycles payload, checks it, and stages it in big_libs.

.DESCRIPTION
    build_cycles.ps1 is for testing on your own hardware; this produces the payload every
    other build uses. Every device is named and -AllArches sets the full shipping list, so
    a missing SDK fails instead of dropping a backend; the files are then checked by name
    against kernel_arches.ps1.

    It stages but does not commit: the message should name the kernel change that made a
    republish necessary. No GPU needed: nvcc and hipcc cross-compile.

.PARAMETER Configuration
    Release (the default) writes the tracked payload. Debug writes the gitignored debug
    one, for local use only.

.PARAMETER SkipBuild
    Check and stage what is already in the payload directory, without building.

.PARAMETER BuildDir
    Passed through to build_cycles.ps1. Separate from a developer's own build directory:
    the architecture lists differ, so sharing one would force full kernel rebuilds.

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
    # Devices named, not detected, so build_cycles.ps1 throws on a missing toolkit.
    # Called in-process: via pwsh -File, -Devices arrives as one string ValidateSet rejects.
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

# Host binaries. cycles_kernel_oneapi_jit.dll is the oneAPI kernel and device support in
# one; without it there is no Intel GPU support.
$hostOk = $true
foreach ($f in 'ccycles.dll', 'cycles_kernel_oneapi_jit.dll') {
    if (-not (Test-PayloadFile $f 'host binary')) { $hostOk = $false }
}
if ($hostOk) { Write-Ok 'host binaries' 'ccycles.dll, cycles_kernel_oneapi_jit.dll' }

# SxS manifests (see fix-cycles-sxs.ps1). build_cycles.ps1 writes them; this throws for a
# payload that reached this folder some other way.
if ($hostOk) {
    & (Join-Path $cyclesRoot 'fix-cycles-sxs.ps1') -PayloadDir $payloadDir -Check
    Write-Ok 'SxS manifests' 'every DLL that loads a shared one has its own'
}

# HIP fatbins. Compressed only: HIPDevice::compile_kernel never looks for a plain .fatbin.
$hipFound = 0
foreach ($arch in $CyclesHipShippingArches) {
    if (Test-PayloadFile "lib\kernel_$arch.fatbin.zst" "HIP kernel $arch") { $hipFound++ }
}
if ($hipFound -eq $CyclesHipShippingArches.Count) {
    Write-Ok 'HIP kernels' "$hipFound / $($CyclesHipShippingArches.Count)"
}

# CUDA cubins and the PTX fallback.
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

# ------------------------------------------------------------------ NVIDIA driver floor

# RH-98331: a driver lists the device, then fails on PTX newer than it knows. Hold each
# shipped PTX's .version to the floor its backend needs anyway: OptiX PTX to the SDK's,
# CUDA PTX to the cubins'. Nothing here reads zstd, so the build's PTX is checked, proven
# to be the shipped one by recompressing it (zstd_compress is deterministic).
Write-Step "Checking the NVIDIA driver floor"

$buildRoot = if ([IO.Path]::IsPathRooted($BuildDir)) { $BuildDir } else { Join-Path $cyclesRoot $BuildDir }
$cmakeCache = Join-Path $buildRoot 'CMakeCache.txt'
if (-not (Test-Path $cmakeCache)) {
    throw "Cannot check the PTX: there is no build in $buildRoot. Publish without -SkipBuild."
}

function Get-CacheValue([string]$name) {
    $m = Select-String -Path $cmakeCache -Pattern "^$([regex]::Escape($name)):[A-Z]+=(.*)$" |
        Select-Object -First 1
    if ($m) { return $m.Matches[0].Groups[1].Value }
    return ''
}

$zstd = Get-ChildItem (Join-Path $buildRoot 'bin') -Recurse -Filter 'zstd_compress.exe' -ErrorAction SilentlyContinue |
    Sort-Object LastWriteTime -Descending | Select-Object -First 1
if (-not $zstd) { throw "Cannot check the PTX: no zstd_compress.exe under $buildRoot\bin." }

function Get-ShippedPtxIsa([string]$built, [string]$shipped) {
    $name = Split-Path -Leaf $shipped
    if (-not (Test-Path $built)) { throw "Cannot check ${name}: $built is missing. Publish without -SkipBuild." }
    $tmp = [IO.Path]::GetTempFileName()
    try {
        & $zstd.FullName $built $tmp | Out-Null
        if ($LASTEXITCODE -ne 0) { throw "zstd_compress failed on $built." }
        if ((Get-FileHash -LiteralPath $tmp).Hash -ne (Get-FileHash -LiteralPath $shipped).Hash) {
            throw "$name in the payload is not the one built in $buildRoot. Publish without -SkipBuild."
        }
    }
    finally { Remove-Item -LiteralPath $tmp -ErrorAction SilentlyContinue }
    $line = Select-String -Path $built -Pattern '^\.version\s+(\d+\.\d+)' | Select-Object -First 1
    if (-not $line) { throw "No .version line in $built." }
    return $line.Matches[0].Groups[1].Value
}

function Get-PtxMinimumDriver([string]$isa, [string]$what) {
    if (-not $CyclesPtxIsaMinimumDriver.Contains($isa)) {
        throw ("$what is PTX $isa, which kernel_arches.ps1 does not know. Find the driver that " +
               "CUDA release needs in its release notes and add the row.")
    }
    return [int]$CyclesPtxIsaMinimumDriver[$isa]
}

# OptiX: the SDK version from its own header, the PTX version from each module.
$optixHeader = Join-Path (Get-CacheValue 'OPTIX_ROOT_DIR') 'include\optix.h'
$optixDefine = Select-String -Path $optixHeader -Pattern '^#define\s+OPTIX_VERSION\s+(\d+)' -ErrorAction SilentlyContinue |
    Select-Object -First 1
if (-not $optixDefine) { throw "Cannot read OPTIX_VERSION from $optixHeader." }
$optixNumber = [int]$optixDefine.Matches[0].Groups[1].Value
$optixSdk = '{0}.{1}' -f [int][math]::Floor($optixNumber / 10000), [int][math]::Floor(($optixNumber % 10000) / 100)
if (-not $CyclesOptixSdkMinimumDriver.Contains($optixSdk)) {
    throw ("OptiX SDK $optixSdk is not in kernel_arches.ps1. Add the driver it needs from its release " +
           "notes there and in device_optix_minimum_driver() (src/device/optix/device.cpp).")
}
$optixDriver = [int]$CyclesOptixSdkMinimumDriver[$optixSdk]

$optixIsa = $null; $optixPtxDriver = 0
foreach ($m in $CyclesOptixModules) {
    $isa = Get-ShippedPtxIsa (Join-Path $buildRoot "src\kernel\device\optix\$m.ptx") (Join-Path $libDir "$m.ptx.zst")
    $driver = Get-PtxMinimumDriver $isa "OptiX module $m"
    if ($driver -ge $optixPtxDriver) { $optixPtxDriver = $driver; $optixIsa = $isa }
}
if ($optixPtxDriver -gt $optixDriver) {
    throw ("The OptiX PTX is $optixIsa, which needs driver $optixPtxDriver, but OptiX SDK $optixSdk " +
           "runs from $optixDriver. A driver in between lists OptiX and then fails with 'Unsupported " +
           "PTX version' (RH-98331). Build the OptiX kernels with an older CUDA toolkit.")
}
Write-Ok 'OptiX' "SDK $optixSdk needs R$optixDriver; PTX $optixIsa needs R$optixPtxDriver"

# CUDA: the cubins' floor from the toolkit that built them, the PTX fallback's from its header.
$nvcc = Get-CacheValue 'CUDA_NVCC_EXECUTABLE'
$cudaRelease = (& $nvcc --version 2>$null | Select-String -Pattern 'release (\d+)\.(\d+)' | Select-Object -First 1)
if (-not $cudaRelease) { throw "Cannot read the CUDA version from $nvcc --version." }
$cudaToolkit = '{0}.{1}' -f $cudaRelease.Matches[0].Groups[1].Value, $cudaRelease.Matches[0].Groups[2].Value
$cudaMajor = $cudaRelease.Matches[0].Groups[1].Value
if (-not $CyclesCudaMajorMinimumDriver.Contains($cudaMajor)) {
    throw "CUDA $cudaMajor is not in kernel_arches.ps1. Add the driver its cubins need there."
}
$cubinDriver = [int]$CyclesCudaMajorMinimumDriver[$cudaMajor]

$cudaIsa = $null; $cudaPtxDriver = 0
foreach ($arch in $CyclesCudaShippingArches | Where-Object { $_ -like 'compute_*' }) {
    $isa = Get-ShippedPtxIsa (Join-Path $buildRoot "src\kernel\device\cuda\kernel_$arch.ptx") (Join-Path $libDir "kernel_$arch.ptx.zst")
    $driver = Get-PtxMinimumDriver $isa "CUDA kernel $arch"
    if ($driver -gt $cubinDriver) {
        throw ("kernel_$arch.ptx is PTX $isa, which needs driver $driver, but the CUDA $cudaToolkit cubins " +
               "run from $cubinDriver, so the fallback would fail first (RH-98331). Build it with the " +
               "CUDA 11 toolkit, as build_cycles.ps1 does when one is installed.")
    }
    if ($driver -ge $cudaPtxDriver) { $cudaPtxDriver = $driver; $cudaIsa = $isa }
}
Write-Ok 'CUDA' "cubins (CUDA $cudaToolkit) need R$cubinDriver; PTX $cudaIsa needs R$cudaPtxDriver"

$nvidiaFloor = [ordered]@{
    cudaToolkit        = $cudaToolkit
    cudaMinimumDriver  = $cubinDriver
    cudaPtxIsa         = $cudaIsa
    optixSdk           = $optixSdk
    optixMinimumDriver = $optixDriver
    optixPtxIsa        = $optixIsa
}

# --------------------------------------------------------------------------- prune

# CMake's install never deletes, so a kernel we stopped shipping would ship forever.
# Only lib/kernel_* files the shipping lists do not name are removed.
Write-Step "Pruning kernels we no longer ship"

$expected = [System.Collections.Generic.HashSet[string]]::new([StringComparer]::OrdinalIgnoreCase)
foreach ($arch in $CyclesHipShippingArches) { [void]$expected.Add("kernel_$arch.fatbin.zst") }
foreach ($arch in $CyclesCudaShippingArches) {
    $ext = if ($arch -like 'compute_*') { 'ptx' } else { 'cubin' }
    [void]$expected.Add("kernel_$arch.$ext.zst")
}
foreach ($m in $CyclesOptixModules) { [void]$expected.Add("$m.ptx.zst") }

$stale = @(Get-ChildItem $libDir -File -Filter 'kernel_*' -ErrorAction SilentlyContinue |
    Where-Object { -not $expected.Contains($_.Name) })

if ($stale.Count) {
    foreach ($f in $stale) {
        Write-Warn 'removing' $f.Name
        Remove-Item -LiteralPath $f.FullName -Force
    }
}
else { Write-Ok 'nothing stale' 'every kernel in lib/ is one we ship' }

# -------------------------------------------------------------------------- manifest

Write-Step "Writing the manifest"

# Read the numeric macros; CYCLES_VERSION_STRING is not a literal. 5.3.0 on this 5.2 tree
# is correct: upstream bumps version.h right after tagging. Do not "fix" it.
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
        # Only paths that enter the build: scratch and build dirs beside them would make
        # every payload dirty. Untracked files under them do count.
        $dirty = & git status --porcelain -- src/ cmake/ CMakeLists.txt third_party/ 2>$null
        return [ordered]@{
            commit = if ($sha) { $sha.Trim() } else { 'unknown' }
            branch = if ($branch) { $branch.Trim() } else { 'unknown' }
            # Built from modified sources: not reproducible from the commit it names.
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

    # So a build inheriting the payload knows what it supports, and a narrower one shows.
    devices       = @('cpu', 'cuda', 'optix', 'hip', 'oneapi')
    arches        = [ordered]@{
        hip   = $CyclesHipShippingArches
        cuda  = $CyclesCudaShippingArches
        optix = $CyclesOptixModules
    }

    # Compared with the tree's hash to catch a kernel change merged without a republish,
    # and stale local HIP kernels.
    kernelSourceHash = Get-CyclesKernelSourceHash -CyclesRoot $cyclesRoot

    # Oldest NVIDIA driver per backend, for the system requirements (RH-98331).
    nvidia = $nvidiaFloor
}

$manifestPath = Join-Path $payloadDir 'ccycles_payload.json'
$manifest | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $manifestPath -Encoding UTF8
Write-Ok 'manifest' $manifestPath
Write-Ok 'kernel source hash' $manifest.kernelSourceHash.Substring(0, 16)

# ------------------------------------------------------------------- version resources

# Nothing to do: ccycles.dll links in its VERSIONINFO and SxS manifest
# (src/ccycles/CMakeLists.txt), and the DLLs it loads get theirs from fix-cycles-sxs.ps1.

# --------------------------------------------------------------------------- staging

if ($Configuration -eq 'Debug') {
    Write-Step "Done - debug payload built and checked, nothing to stage (it is gitignored)"
    return
}

Write-Step "Staging in big_libs"

# big_libs is a submodule, so stage inside it. That means two commits (payload, then the
# Rhino repo's submodule pointer); both are printed, not made.
$bigLibs = Join-Path $repoRoot 'big_libs'
$payloadRel = "RhinoCycles/ccycles/win/$payloadName"

Push-Location $bigLibs
try {
    & git add -- $payloadRel
    if ($LASTEXITCODE -ne 0) {
        throw "git add failed inside the big_libs submodule (exit $LASTEXITCODE)."
    }
    $staged = @(& git diff --cached --name-only -- $payloadRel)
}
finally { Pop-Location }

if (-not $staged.Count) {
    # Not a failure: the build is byte-identical to what big_libs already holds.
    Write-Warn 'nothing staged' 'this payload is identical to the one already committed'
    Write-Step "Done - payload checked, nothing to publish"
    return
}

Write-Ok 'staged' "$($staged.Count) file(s) in the big_libs submodule"

Write-Host ""
Write-Host "Payload is complete and staged. Commit it in big_libs first, then record the" -ForegroundColor Cyan
Write-Host "new submodule pointer in the Rhino repo, naming the kernel change that made a" -ForegroundColor Cyan
Write-Host "republish necessary:" -ForegroundColor Cyan
Write-Host ""
Write-Host "    git -C `"$bigLibs`" commit -m `"Cycles: republish the payload for <change>`"" -ForegroundColor White
Write-Host "    git -C `"$repoRoot`" add big_libs" -ForegroundColor White
Write-Host "    git -C `"$repoRoot`" commit -m `"Cycles: bump big_libs for the republished payload`"" -ForegroundColor White
Write-Host ""
