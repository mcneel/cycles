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

# --------------------------------------------------------------------------- prune

# CMake's install step adds and overwrites; it never deletes. So a kernel for an
# architecture we have stopped shipping stays in the payload, gets committed, and ships
# forever - kernel_compute_52.ptx.zst survived exactly that way after the CUDA list
# changed, a Maxwell PTX sitting next to the cubins that replaced it.
#
# Only files in lib/ that look like kernels are considered, and only ones the shipping
# lists do not name. Everything else in the payload - the OpenImageIO and OpenVDB DLLs
# at the root, the installed source/ tree, shader/ - is left alone.
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
        # Only the paths that enter the build count. Asking git about the whole tree
        # made every payload dirty: this directory also holds the render harness,
        # smoke-test leftovers and build directories, and untracked scratch beside the
        # source says nothing about what was compiled. Untracked files under these
        # paths do count - a new kernel file not yet added is a real difference.
        $dirty = & git status --porcelain -- src/ cmake/ CMakeLists.txt third_party/ 2>$null
        return [ordered]@{
            commit = if ($sha) { $sha.Trim() } else { 'unknown' }
            branch = if ($branch) { $branch.Trim() } else { 'unknown' }
            # A payload built from modified sources cannot be reproduced from the commit
            # it names, which is worth knowing later even though it is normal while
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

    # Compare this against a fresh Get-CyclesKernelSourceHash to know whether the kernels in
    # a payload were built from the kernel sources currently in the tree. That is the
    # check that stops a kernel change merging without a republish, and the one that
    # tells a developer their local HIP kernels are now stale.
    kernelSourceHash = Get-CyclesKernelSourceHash -CyclesRoot $cyclesRoot
}

$manifestPath = Join-Path $payloadDir 'ccycles_payload.json'
$manifest | ConvertTo-Json -Depth 6 | Set-Content -LiteralPath $manifestPath -Encoding UTF8
Write-Ok 'manifest' $manifestPath
Write-Ok 'kernel source hash' $manifest.kernelSourceHash.Substring(0, 16)

# ------------------------------------------------------------------- version resources

# There is no resource-stamping step. ccycles.dll compiles its VERSIONINFO and its
# side-by-side assembly manifest in at link time (src/ccycles/CMakeLists.txt), and that
# manifest already names openvdb.dll, tbb12.dll, tbbmalloc*, the OpenImageIO pair,
# OpenColorIO, the oneAPI JIT DLL and sycl8.dll - so everything the old ResourceHacker
# pass pinned now resolves inside the ccycles assembly context. See git history for
# versioninfo_changer.ps1 if the reasoning is ever needed again.

# --------------------------------------------------------------------------- staging

if ($Configuration -eq 'Debug') {
    Write-Step "Done - debug payload built and checked, nothing to stage (it is gitignored)"
    return
}

Write-Step "Staging in big_libs"

# big_libs is a submodule, not a folder inside the Rhino repo. Staging the payload from
# the parent fails outright - "Pathspec ... is in submodule 'big_libs'" - so this has to
# run inside it. Publishing therefore takes two commits: the payload in big_libs, then
# the new submodule pointer in the Rhino repo. Both are printed below; neither is made
# here, because the message should name the kernel change that made a republish
# necessary.
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
    # Not a failure. It means the kernels and binaries just built are byte-identical to
    # what big_libs already holds, so there is nothing to publish.
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
