#requires -Version 5.1
<#
.SYNOPSIS
    Configures, builds and installs Cycles + ccycles for Rhino.

.DESCRIPTION
    Replaces make_rhino.bat and build_cycles_for_rhino.ps1.

    Everything that used to be a hardcoded absolute path is now discovered:

      * Visual Studio    - via vswhere, newest install with the C++ toolset,
                           VS2022 or later. Its toolchain is used through a
                           developer shell; the build itself is driven by Ninja
                           unless -Generator vs is passed. The old scripts needed
                           VS2022 Professional *and* VS2019 BuildTools side by
                           side, because the win64_vc15 library bundle was built
                           with the VS2019 ABI.
      * CUDA / OptiX /
        HIP / oneAPI     - probed from the usual environment variables and
                           install locations. A missing SDK switches off that
                           backend's *kernels*, not the backend: CUDA and HIP
                           device support is compiled in either way, so the
                           kernels already in the payload keep serving the GPU.
                           OptiX is the exception - it needs its SDK headers
                           and nvcc, with no dynamic-loading path around either.

    Kernels are built for the GPUs in this machine, not for every architecture
    Cycles supports. Rebuilding kernels is how you test a kernel change, and a
    kernel for a card you do not own cannot be tested - so an AMD-only machine
    should not spend an hour on 18 HIP fatbins plus the CUDA and OptiX kernels
    every time a kernel header changes. Pass -AllArches for the shipping set.
      * MSVC redist and
        Windows Kits     - dropped entirely; CMake locates these itself.

    The shipping architecture lists for CUDA and HIP live in kernel_arches.ps1
    and are passed with -D, so upstream's CMakeLists keeps taking merges
    cleanly. They are a statement about which GPU generations Rhino supports, so
    they belong somewhere a person can read them - and upstream's defaults are
    wrong for us in both directions: its HIP list has no RDNA4, and its CUDA
    list still names Kepler.

.PARAMETER Configuration
    Debug, Release or RelWithDebInfo. Release maps to RelWithDebInfo in the
    CMake build so we keep usable symbols, matching the old behaviour.

.PARAMETER Devices
    Which backends to build *kernels* for. Defaults to whatever toolkits are
    detected. Pass 'cpu' to build no GPU kernels at all.

    Note this no longer decides which devices Cycles supports: CUDA and HIP
    device support is compiled in either way, and -Devices cpu produces a
    ccycles.dll that still drives both, using the kernels in the payload. What
    it controls is what this build compiles.

.PARAMETER InstallDir
    Where to place ccycles.dll and its dependencies. Defaults to the Rhino
    Plug-ins output directory for the chosen configuration.

.PARAMETER CudaBinaries
    Fall back to upstream's CYCLES_CUDA_BINARIES_ARCH rather than PTX only.
    Rarely wanted: -AllArches already builds Rhino's shipping list, and a local
    build takes its architectures from the cards in this machine. This is the
    escape hatch for comparing against what upstream would have produced.

.PARAMETER AllArches
    Build kernels for every architecture Cycles ships, rather than only for the
    GPUs in this machine. This is what publishing a payload wants; a developer
    testing a kernel change does not.

.PARAMETER Force
    Install into the requested payload even when this build makes fewer kernels
    than the payload already holds. Without it, such a build is redirected to a
    sibling "local" payload, which RhinoCyclesCore prefers and git ignores, so
    the committed payload is never replaced by kernels for one machine's GPU.
    With it, the payload's manifest is deleted, because it would no longer
    describe what is there.

.PARAMETER ConfigureOnly
    Run the CMake configure step and stop, leaving a solution to open in VS.

.PARAMETER Generator
    ninja (the default) or vs.

    Ninja is much faster here, and the GPU kernels are why. Every architecture's
    kernel is an add_custom_command on one target, and MSBuild runs a project's
    custom build steps strictly in order - its /m parallelism works across
    projects, not within one. So the 18 HIP fatbins compiled one at a time, at
    3m20s each: an hour of a 24-core machine sitting mostly idle. Ninja has no
    such restriction and runs them concurrently.

    "Ninja Multi-Config" is used rather than plain Ninja so --config keeps
    working exactly as it does with the Visual Studio generators.

    Pass -Generator vs to get a Cycles.sln to open in the IDE. Note that the
    generator is an implementation detail of this script either way: ccycles.vcxproj
    is a Makefile-style project that shells out to here, so Visual Studio never
    sees what CMake generated.

.PARAMETER Jobs
    How many compiles to run at once. Defaults to a cap rather than the core
    count, because the kernel compilers are memory-hungry - each clang building a
    HIP kernel peaks around 1.2 GB, and letting all 18 architectures go at once
    wants more RAM than a 32 GB machine has to spare.

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

# CMake treats a backslash as an escape inside cache values, so a Windows path
# passed straight through breaks its own find modules - FindCUDA.cmake reports
# a syntax error rather than a bad path. Normalise every path handed to -D.
function ConvertTo-CMakePath([string]$p) { return $p.Replace('\', '/') }

# ---------------------------------------------------------------- prerequisites

Write-Step "Checking prerequisites"

# Visual Studio is the anchor for everything else. CMake and Ninja both ship inside it,
# in the "C++ CMake tools for Windows" component that Rhino's own .vsconfig already
# installs - so a machine that has run bootstrap.exe holds the entire toolchain even
# with an empty PATH.
#
# This used to demand cmake, git and python on PATH up front and throw otherwise, which
# failed a check a correctly set up machine passes: VS's cmake.exe is not on PATH by
# default. git and python are not needed to build at all. They are needed only to fetch
# the precompiled library bundle, so they are checked in that step instead, and only
# when the bundle is actually missing.
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhere)) { throw "vswhere.exe not found. Install Visual Studio 2022 or newer." }

# No upper bound on the version. This used to pin [17.0,18.0) - VS 2022 only -
# which would have stopped Cycles building at all the moment Rhino moved on:
# Rhino 9's C++ projects already ask for PlatformToolset v145, which ships with
# VS 2026. Take the newest install that has the C++ toolset and derive the CMake
# generator from it rather than hardcoding one.
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

# CMake: prefer whatever is on PATH, then fall back to the copy inside Visual Studio.
# PATH wins here - unlike Ninja below - because this tree deliberately pins an older
# CMake than the newest release, and a developer who installed one on PATH meant it.
# The fallback is what lets a bootstrapped machine build with nothing else installed.
$cmakeExe = @(
    (Get-Command cmake -ErrorAction SilentlyContinue | Select-Object -ExpandProperty Source)
    (Join-Path $vsPath 'Common7\IDE\CommonExtensions\Microsoft\CMake\CMake\bin\cmake.exe')
) | Where-Object { $_ -and (Test-Path $_) } | Select-Object -First 1

if (-not $cmakeExe) {
    throw ("cmake was not found on PATH or inside '$vsPath'. Add the 'C++ CMake tools " +
           "for Windows' component in the Visual Studio installer, or run bootstrap.exe " +
           "from the root of the Rhino repo, which installs it from Rhino's .vsconfig.")
}
Write-Found 'cmake' $cmakeExe

# The generator is chosen from what CMake actually offers, not derived from the
# Visual Studio version. Deriving it looked obvious and was wrong: on a machine
# with VS 18 it produced "Visual Studio 18 2026", which CMake 3.31 has never
# heard of, and the configure died with "Could not create named generator".
# CMake gains new Visual Studio generators in its own time, and this tree pins
# an older CMake on purpose, so the two versions cannot be assumed to match.
#
# Ask CMake, keep the Visual Studio entries, and take the newest whose major
# version is one we actually have installed.
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

# Ninja ships inside the Visual Studio install, under the C++ CMake tools
# component, and that copy is preferred over anything on PATH: this machine had a
# ninja 1.12 bundled with Strawberry Perl sitting ahead of Visual Studio's 1.13,
# and silently building Cycles with whatever unrelated toolchain happens to be
# first on PATH is not a dependency worth having. PATH stays as a fallback for a
# machine where the VS component is not installed.
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

# Enter the Visual Studio developer environment.
#
# The oneAPI kernel is compiled by the clang++ shipped in the library bundle,
# and that compiler locates the MSVC toolchain and Windows SDK through the
# environment rather than through CMake. Without this it fails with
# "unable to find a Visual Studio installation". The devshell module is
# resolved from the detected install rather than hardcoded, unlike the previous
# scripts which pinned a VS2022 Professional path and then entered a VS2019
# BuildTools environment.
#
# The MSVC toolset is pinned rather than left at the newest installed, because
# CUDA decides for itself which host compilers it will accept. CUDA 12.9 rejects
# MSVC 14.5x outright - "Only the versions between 2017 and 2022 (inclusive) are
# supported" - and VS 18 installs 14.51 as its default.
#
# This never showed while the build ran through the Visual Studio generator,
# which pinned the toolset itself: CMake 3.31 has no VS 18 generator, so it fell
# back to "Visual Studio 17 2022" and nvcc was handed a 14.4x cl.exe whatever the
# devshell said. Ninja has no such indirection - it uses the cl.exe on PATH - so
# the constraint has to be stated here instead of arrived at by accident.
#
# Pinning also makes the build consistent for the first time: under the VS
# generator the oneAPI kernel's clang++ picked up the devshell's 14.51 while
# everything else compiled with 14.4x.
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

# Cycles 5.x pulls precompiled libraries into lib/<platform> as a git submodule.
# Cycles 3.5 expected them in a sibling ../lib checked out from Blender's SVN
# server, which no longer exists - so on the old tree this will correctly tell
# the user the tree needs updating rather than failing deep inside CMake.
$libModern = Join-Path $cyclesRoot 'lib\windows_x64'
$libLegacy = Join-Path (Split-Path -Parent $cyclesRoot) 'lib'

Write-Step "Checking precompiled libraries"
if (Test-Path $libModern) {
    Write-Found 'libraries' $libModern
}
elseif (Test-Path $libLegacy) {
    Write-Found 'libraries' "$libLegacy (legacy layout)"
}
else {
    Write-Host "   Libraries missing - running 'make update'" -ForegroundColor Yellow
    $makeBat = Join-Path $cyclesRoot 'make.bat'
    if (-not (Test-Path $makeBat)) { throw "make.bat not found at '$makeBat'." }

    # git and python are needed here and nowhere else - 'make update' drives
    # make_update.py, which fetches the library submodule. Checking them at this point
    # rather than up front means a developer whose bundle is already checked out (the
    # normal case after bootstrap.exe, which inits submodules) never has to have them.
    foreach ($tool in 'git', 'python') {
        if (-not (Get-Command $tool -ErrorAction SilentlyContinue)) {
            throw ("'$tool' is needed to fetch the precompiled Cycles libraries and was " +
                   "not found on PATH. Run bootstrap.exe from the root of the Rhino repo, " +
                   "or fetch the bundle yourself with 'git submodule update --init " +
                   "--recursive' in $cyclesRoot.")
        }
    }
    # Launch with an explicit working directory: Set-Location/Push-Location does
    # not change the working directory a child process inherits.
    $proc = Start-Process -FilePath 'cmd.exe' -ArgumentList '/c', "`"$makeBat`"", 'update' `
        -WorkingDirectory $cyclesRoot -NoNewWindow -Wait -PassThru
    if ($proc.ExitCode -ne 0) { throw "'make update' failed with exit code $($proc.ExitCode)." }
    if (-not (Test-Path $libModern) -and -not (Test-Path $libLegacy)) {
        throw "'make update' completed but no library folder appeared. On Cycles 3.5 this is expected: it fetches from Blender's decommissioned SVN server. Update to Cycles 4.2 or newer, which uses Git LFS."
    }
}

# -------------------------------------------------------------------- devices

Write-Step "Detecting GPU toolkits"

$detected = [System.Collections.Generic.List[string]]::new()

# Note the "-or -not (Test-Path ...)" in each probe. A stale environment
# variable pointing at an uninstalled toolkit is common on machines that have
# had several SDK versions - treat it as absent and fall back to the on-disk
# search rather than reporting the toolkit missing.

# Enumerate every CUDA toolkit on the machine rather than trusting CUDA_PATH.
# Installing a second toolkit repoints CUDA_PATH at whichever was installed
# last, so on a machine with both 11.8 and 12.9 it can easily name the older
# one - which would silently drop the newest architectures from the build.
# Machine-level CUDA_PATH_V* entries also go stale when a toolkit is removed,
# hence the nvcc.exe existence check.
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

# Optional second toolkit. Cycles builds the default compute_7x PTX kernel -
# the fallback path for every NVIDIA card - with CUDA 11 when it is available,
# which keeps the minimum driver version users need low. Without it that kernel
# is built with the primary toolkit and the driver floor rises.
$cuda11 = $cudaToolkits | Where-Object { $_.Version.Major -eq 11 } | Select-Object -First 1
$cuda11Path = if ($cuda11) { $cuda11.Path } else { $null }
if ($cuda11Path) { Write-Found 'CUDA 11' ("{0}  (v{1})" -f $cuda11Path, $cuda11.Version) }
else { Write-Missing 'CUDA 11' 'optional; default PTX kernel will raise the minimum driver version' }

$optixPath = $env:OPTIX_ROOT_DIR
if (-not $optixPath -or -not (Test-Path $optixPath)) {
    $optixPath = Get-ChildItem 'C:\ProgramData\NVIDIA Corporation' -Directory -Filter 'OptiX SDK *' -ErrorAction SilentlyContinue |
        Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
}
if ($optixPath -and (Test-Path $optixPath)) { $detected.Add('optix'); Write-Found 'OptiX' $optixPath }
else { Write-Missing 'OptiX' 'set OPTIX_ROOT_DIR to enable' }

# HIP: prefer ROCm 6.x. Blender builds and tests Cycles against 6.x; 7.x is
# neither rejected nor validated by the Cycles CMake, it is simply used as-is.
$hipPath = $env:HIP_PATH
if (-not $hipPath -or -not (Test-Path $hipPath)) { $hipPath = $null }
$hipCandidates = @()
foreach ($base in 'C:\rocm', 'C:\Program Files\AMD\ROCm') {
    $hipCandidates += Get-ChildItem $base -Directory -ErrorAction SilentlyContinue | Select-Object -ExpandProperty FullName
}
$hip6 = $hipCandidates | Where-Object { (Split-Path -Leaf $_) -like '6.*' } | Sort-Object -Descending | Select-Object -First 1

# An explicitly set HIP_PATH wins. This used to force ROCm 6.x whenever HIP_PATH
# pointed elsewhere, on the assumption that Cycles targets 6.x - which made the
# variable useless for its one real purpose, trying a different SDK. Worth
# keeping: chasing the kernel compile failure through several ROCm versions was
# only possible because of it, even though the fault turned out not to be ROCm's.
if ($env:HIP_PATH -and (Test-Path $env:HIP_PATH)) {
    $hipPath = $env:HIP_PATH
    Write-Host ("   {0,-12} using HIP_PATH as given: {1}" -f 'HIP', $hipPath) -ForegroundColor DarkYellow
}
elseif (-not $hipPath) { $hipPath = $hip6 }
# HIP was kept out of the automatic set for a while because its kernels failed to
# compile, which looked like a ROCm problem - the error surfaces inside AMD's
# amd_hip_vector_types.h. It was ours: a float4 divided by an int in
# svm_rhino_procedurals.h, which routes through HIP's operator/=(U) template and
# out through an overload 5.2 added. See the comment at that line. All 18
# architectures build against ROCm 6.4 now, so HIP is back in the default set.
if ($hipPath -and (Test-Path $hipPath)) {
    $hipAvailable = $true
    $detected.Add('hip')
    Write-Found 'HIP' $hipPath
}
else { $hipAvailable = $false; Write-Missing 'HIP' 'set HIP_PATH to enable' }

$levelZeroRoot = @($libModern, $libLegacy) |
    ForEach-Object { Join-Path $_ 'level-zero' } |
    Where-Object { Test-Path $_ } | Select-Object -First 1
# oneAPI is enabled when detected, deliberately.
#
# History, because it is not obvious and cost time to rediscover: RH-91240 was
# Rhino crashing on exit, and the fix taken on the 4.4 line was simply to turn
# OneAPI and SYCL8 off. That silenced the symptom without anyone establishing
# the cause, and the decision was recorded only in a commit message - so this
# script, written against the 3.5 tree, re-enabled it without knowing.
#
# It stays enabled on purpose: an uninvestigated crash is a reason to
# investigate, not to ship without Intel GPU support indefinitely. Two things
# worth knowing when it is looked at:
#
#   * the oneAPI kernel build warns about undefined intel_has_committed_hit,
#     intel_get_hit_triangle_primitive_id and intel_ray_query_abandon, and says
#     it "may result in runtime errors" - a plausible starting point
#   * .\build_cycles.ps1 -Devices cpu,cuda,optix,hip reproduces the old
#     behaviour if a build has to ship before the crash is understood. Note that
#     form only works when the script is called from PowerShell; passed through
#     pwsh -File, as ccycles.vcxproj does, a comma-separated list arrives as one
#     string and ValidateSet rejects it.
if ($levelZeroRoot) { $detected.Add('oneapi'); Write-Found 'oneAPI' $levelZeroRoot }
else { Write-Missing 'oneAPI' 'level-zero not present in the library bundle' }

# CYCLES_DEVICES restricts the device set the same way CYCLES_NATIVE_ONLY restricts CPU
# architectures, and for the same reason: it can be set through the environment, which is
# the only route that works when ccycles.vcxproj calls this script through pwsh -File. On a
# machine whose GPU is AMD, the CUDA cubins and OptiX PTX are pure cost on every kernel
# touch - CYCLES_DEVICES=cpu,hip drops them. Changing it re-configures and forces one full
# rebuild; builds after that are much quicker.
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
        # These throw rather than quietly dropping the backend, which is the whole point
        # of naming devices explicitly: publish_payload.ps1 does it so that a machine
        # which cannot produce a complete payload stops instead of shipping a partial
        # one. Say what to install, though - "set HIP_PATH" is no use to someone who
        # does not have ROCm at all, which is the common case for this message.
        if ($d -eq 'hip' -and -not $hipAvailable) {
            throw ("'hip' was requested but no ROCm install was found. Install the HIP SDK " +
                   "for Windows - no AMD hardware is needed, hipcc cross-compiles - from " +
                   "https://www.amd.com/en/developer/resources/rocm-hub/hip-sdk.html, or " +
                   "point HIP_PATH at an existing one. Cycles builds against ROCm 6.x. " +
                   "Drop 'hip' from -Devices to build without AMD kernels, but note that a " +
                   "payload without them is not publishable.")
        }
        # OptiX needs both its headers and a CUDA toolkit: the host code includes
        # optix.h and the kernels are PTX built by nvcc. Checked here against the
        # detected paths, since the device decisions below have not been made yet.
        if ($d -eq 'optix' -and -not ($optixPath -and $cudaPath)) {
            throw ("'optix' was requested but " + $(if (-not $optixPath) {
                       "the OptiX SDK headers were not found. bootstrap.exe /cycles fetches " +
                       "them from NVIDIA, or set OPTIX_ROOT_DIR"
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
# These are two different questions and the script used to answer only one.
#
#   Device support is host code. Cycles loads the CUDA and HIP driver APIs
#   dynamically - WITH_CUDA_DYNLOAD defaults ON and external_libs.cmake forces
#   WITH_HIP_DYNLOAD ON - so device/cuda and device/hip compile and link against the
#   bundled cuew and hipew headers, with no toolkit installed at all.
#
#   Kernel binaries are the cubins, PTX and fatbins. Those need nvcc and hipcc.
#
# Conflating them was the worst trap in the old behaviour: no CUDA toolkit meant
# WITH_CYCLES_DEVICE_CUDA=OFF, so a developer with an NVIDIA card and no SDK built a
# ccycles.dll that could not use their own GPU - and, because the install overwrites
# the payload in big_libs, lost the working kernels that were already there. Now the
# device stays compiled in and the existing kernels in the payload keep serving it.
#
# OptiX is the exception. Its host code includes the SDK headers (OPTIX_INCLUDE_DIR)
# and its kernels are PTX built by nvcc, so it needs the OptiX SDK *and* a CUDA
# toolkit. There is no dynamic-loading path that avoids either.
#
# oneAPI cannot be split at all: its device support and its kernel DLL are one
# artifact - cycles_kernel_oneapi_jit.dll, built by the SYCL clang++ in the library
# bundle - so there is no switch that keeps Intel support while skipping the build.
# It needs nothing installed, but it is the long serial job in a full build. Splitting
# it would mean editing upstream's kernel/device/oneapi/CMakeLists.txt.
$deviceCuda  = $true
$deviceHip   = $true
$deviceOneApi = [bool]$levelZeroRoot
$deviceOptix = [bool]($optixPath -and $cudaPath)

if (-not $optixPath) {
    Write-Missing 'OptiX device' 'no SDK headers; OptiX support is compiled out of this build'
}
elseif (-not $cudaPath) {
    Write-Missing 'OptiX device' 'OptiX kernels are built by nvcc and no CUDA toolkit was found'
}

$kernelCuda  = ($Devices -contains 'cuda') -and [bool]$cudaPath
$kernelHip   = ($Devices -contains 'hip') -and [bool]$hipPath
$kernelOptix = ($Devices -contains 'optix') -and $deviceOptix

# ------------------------------------------------------- which architectures to build
#
# By default build only what this machine can actually run, which is the whole point
# of a local build: you rebuild kernels to test a change, and a kernel for a card you
# do not own cannot be tested. An AMD-only machine paying for 18 HIP fatbins plus the
# CUDA and OptiX kernels on every kernel touch is roughly an hour for nothing.
#
# -AllArches builds the full shipping set. That is what publishing wants, and nothing
# else should use it.
#
# Detection uses the vendors' own tools rather than adapter enumeration:
# amdgpu-arch (ships with ROCm) prints the gfx target of each installed AMD GPU, and
# nvidia-smi (ships with the driver) reports each NVIDIA card's compute capability.
# Win32_VideoController is not usable for this - it also lists things like the
# "Microsoft Remote Display Adapter", and it does not name a kernel architecture.
# Which GPU vendors are physically in this machine, which is a different question from
# which architectures to build for. Adapter enumeration cannot answer the second - it
# never names a kernel architecture, and it lists things like the Microsoft Remote
# Display Adapter - but it answers this one, and unlike amdgpu-arch and nvidia-smi it
# works with no SDK and no vendor driver tooling installed. That matters, because the
# case worth warning about is precisely a machine with a GPU whose SDK is missing.
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

# The MSVC toolset check, deferred to here because it only matters once we know whether
# any kernels are being built. Both kernel compilers reject the newest MSVC: nvcc says
# "Only the versions between 2017 and 2022 (inclusive) are supported", and ROCm 6.4's
# clang fails inside __clang_cuda_math_forward_declares.h against MSVC 14.51's cmath.
# VS 18 installs 14.51 by default, so a machine that never applied Rhino's .vsconfig has
# no 14.4x at all - and the failure surfaces as a wall of errors inside compiler headers
# with nothing pointing at the cause.
#
# A warning was not enough for that. It is an error, but only for a build that will
# actually invoke a kernel compiler: someone building CPU-only Cycles on a machine
# without the toolset is not doing anything wrong.
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

# The shipping architecture lists live in kernel_arches.ps1, dot-sourced here and by
# publish_payload.ps1. One file so the build and the payload check cannot drift: a
# publish that verified against its own copy of the list would pass while shipping
# something else.
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
            # The OptiX kernels are architecture-independent PTX rather than per-arch
            # cubins, so nothing about them needs narrowing - but with no NVIDIA card
            # present there is nothing to test them on either, and they are nine nvcc
            # invocations. Skip them for the same reason as the cubins. Device support
            # stays compiled in, so the payload's OptiX kernels keep working.
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
    # This repository is Rhino's RDK/cycles submodule, so:
    #   RDK/cycles -> RDK -> Plug-ins -> rhino4 -> src4
    # It used to be one deeper, nested inside CCSycles, which is why this walked
    # five levels rather than four.
    $src4 = Resolve-Path (Join-Path $cyclesRoot '..\..\..\..') -ErrorAction SilentlyContinue
    if ($src4 -and (Test-Path (Join-Path $src4 'rhino4'))) {
        $InstallDir = Join-Path $src4 "bin\$Configuration\Plug-ins"
    }
    else {
        $InstallDir = Join-Path $cyclesRoot 'install'
    }
}
elseif ($InstallDir -notmatch '^([A-Za-z]:[\\/]|\\\\)') {
    # IsPathRooted is not enough: .NET calls "C:Users\..." rooted, but Windows
    # reads it as relative to the current directory on drive C:, so a path that
    # lost its separators installs somewhere surprising and still reports
    # success. Require a drive plus separator, or a UNC path.
    throw "-InstallDir must be an absolute path, got '$InstallDir'. If this came from a build script, check that backslashes survived quoting - forward slashes are safest."
}

# ---------------------------------------------------------------- payload guard
#
# A local build is narrow on purpose - kernels for this machine's GPUs and nothing
# else - and ccycles.vcxproj installs into the payload in big_libs, because that is
# how a Cycles build reaches the plug-in output for everyone. Those two together are
# a trap: building ReleaseDebuggable+Cycles to test one kernel edit would replace the
# shared payload with a ccycles.dll built from a dirty tree and kernels for one card,
# and nothing but git status would say so.
#
# So a narrow build does not write a payload that describes itself as complete. It
# writes to a sibling "local" payload instead, which is gitignored and which
# RhinoCyclesCore prefers over release and debug when it exists. The developer's Rhino
# picks up exactly what they built; the committed payload is untouched.
#
# The debug payload is exempt: it is already gitignored and only a Debug build uses
# it, so it is local by definition.
#
# -Force writes the requested payload anyway, and then deletes its manifest, because a
# payload that has been partly overwritten no longer contains what the manifest says.
# publish_payload.ps1 writes a fresh one.
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

# CMake refuses to reuse a build directory that was generated by a different
# generator, and says so in a way that reads like a broken checkout. Switching
# between -Generator ninja and vs is a normal thing to do, so recognise the case
# and clear the directory instead of making the developer work it out.
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
)

# Cycles builds the CPU kernel once per SIMD variant (SSE4.2, AVX2 and so on) even though
# a machine only ever runs one of them, and that is most of the cost of touching a kernel
# header - about twenty minutes here. CYCLES_NATIVE_ONLY=1 builds only this machine's
# architecture, which is what you want while iterating on kernel code. The result is not
# portable to other CPUs, so it is opt-in and off by default. Flipping it re-configures and
# forces one full rebuild; every build after that is far quicker.
if ($env:CYCLES_NATIVE_ONLY -eq '1') {
    Write-Host 'CYCLES_NATIVE_ONLY=1: building only this machine CPU architecture'
    $cmakeArgs += '-DWITH_CYCLES_NATIVE_ONLY=ON'
}
else {
    $cmakeArgs += '-DWITH_CYCLES_NATIVE_ONLY=OFF'
}

if ($Generator -eq 'ninja') {
    # Ninja takes the architecture from the compiler it is handed, not from -A,
    # which it rejects. The developer shell was entered as x64 above, so cl.exe
    # and link.exe on PATH are already the right ones.
    $cmakeArgs += "-DCMAKE_MAKE_PROGRAM=$(ConvertTo-CMakePath $ninjaExe)"
}
else {
    $cmakeArgs += '-A', 'x64'
}

# Every device switch below is set explicitly, ON or OFF, on every configure. Only ever
# adding =ON meant the CMake cache remembered whatever a previous run enabled, so
# -Devices cpu still built HIP in an existing build directory - the flags described the
# difference from last time rather than what was asked for.

if ($deviceOptix) {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_OPTIX=ON', "-DOPTIX_ROOT_DIR=$(ConvertTo-CMakePath $optixPath)"
} else {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_OPTIX=OFF'
}

# CUDA device support is unconditional: cuew loads the driver API at runtime, so this
# links with no toolkit present. It also keeps find_package(CUDA) running, which the
# OptiX kernels depend on - with the device off, CUDA_NVCC_EXECUTABLE stays empty and
# cuda_add_common_flags is called with an empty version argument, which CMake reports
# only as "invoked with incorrect arguments".
$cmakeArgs += '-DWITH_CYCLES_DEVICE_CUDA=ON'

# The OptiX kernels ride on WITH_CYCLES_CUDA_BINARIES upstream: kernel/device/optix
# guards its PTX rules with "if(WITH_CYCLES_CUDA_BINARIES AND WITH_CYCLES_DEVICE_OPTIX)".
# So asking for OptiX kernels without CUDA binaries builds nothing, silently. Turn the
# switch on for either, and let the architecture list stay cheap when only OptiX is
# wanted - the OptiX modules are architecture-independent PTX and do not need cubins.
if ($kernelCuda -or $kernelOptix) {
    $cmakeArgs += '-DWITH_CYCLES_CUDA_BINARIES=ON'
    # Pass the toolkit explicitly. CMake's FindCUDA otherwise relies on
    # CUDA_PATH/PATH, which silently fails when a stale CUDA_PATH points at an
    # uninstalled version - it reports "CUDA compiler not found" and quietly
    # turns CUDA binaries back off.
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
        # One PTX kernel, which the driver JITs for whatever card shows up: cheap and
        # portable. This is also the OptiX-only case - nvcc still has to be handed an
        # architecture, but no cubins are wanted.
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
    # Device support and kernel binaries are separate switches, and
    # WITH_CYCLES_HIP_BINARIES defaults to OFF. Enabling only the device once gave a
    # build with HIP compiled in and no HIP kernels anywhere, so Cycles found no usable
    # AMD device at all - which crashed Rhino on a machine whose GPU the 3.5 build
    # renders with quite happily. That case is now covered by the payload's kernels
    # instead, but the two switches still have to be set deliberately.
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

# Bare --parallel means "use every core", which is fine for C++ but not for the
# GPU kernels: each clang compiling a HIP architecture peaks around 1.2 GB, and
# Ninja will happily start all 18 at once. On a 24-core, 32 GB machine that is
# roughly 21 GB of compilers plus everything else. Cap it, leaving headroom, and
# let -Jobs override for a machine that can take more.
if (-not $Jobs) {
    $cores = [int]$env:NUMBER_OF_PROCESSORS
    if (-not $cores) { $cores = 4 }
    $Jobs = [Math]::Max(2, [Math]::Min($cores - 2, 12))
}

Write-Step "Building ($cmakeConfig, $Jobs jobs)"
& $cmakeExe --build $BuildDir --config $cmakeConfig --target install --parallel $Jobs
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE." }

# CMake install rules do not carry the HIP fatbins, so whatever kernels were in the
# payload stay there. In this tree they were three weeks old, which meant every HIP
# render ran kernels that did not contain the fixes being tested - a black bump-mapped
# surface on HIP survived nine hypotheses for exactly that reason. Copy the freshly
# built ones in alongside the rest of the payload.
#
# Deploy the *compressed* fatbins, not the uncompressed intermediates. The build tree
# holds both - kernel_gfx1100.fatbin and kernel_gfx1100.fatbin.zst - but the runtime
# only ever looks for the compressed name: HIPDevice::compile_kernel probes
# "lib/<name>_<arch>.fatbin.zst" and nothing else. Copying "*.fatbin" therefore
# deployed 18 files Cycles never opens and left the stale .zst in place, which is the
# same stale-kernel symptom this step was added to fix.
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
# This is what makes "a missing SDK costs you kernels, not the backend" actually true.
#
# Device support for CUDA and HIP is compiled in whether or not their toolkits are
# installed, on the grounds that the kernels already in the payload keep serving those
# GPUs. That holds for the committed release payload - but a debug or local payload is
# created from scratch and starts empty, so on a fresh clone the assumption fails: a
# developer with an NVIDIA card and no CUDA toolkit would get a Rhino whose ccycles.dll
# has the CUDA device compiled in and no CUDA kernels anywhere. That is worse than the
# old behaviour, which compiled the device out and fell back to the CPU cleanly. It is
# also the state the HIP comment above describes, where Cycles found no usable device
# at all and took Rhino down with it.
#
# So fill the gaps from the committed payload. Only kernels this build did not produce,
# only for devices this build actually enabled, and never into the release payload
# itself - publish_payload.ps1 owns that one and builds the full set.
#
# The inherited kernels come from whenever the payload was last published, so they can
# predate local kernel edits. That is said out loud rather than papered over: it is the
# right trade against having no kernels, but it is exactly the mixture that makes a
# render look fine while testing old code.
function Copy-InheritedKernels {
    param(
        [Parameter(Mandatory)][string]$TargetPayload,
        [bool]$Hip,
        [bool]$Cuda,
        [bool]$Optix
    )

    if ((Split-Path -Leaf $TargetPayload) -eq 'release') { return 0 }

    $releaseLib = Join-Path (Join-Path (Split-Path -Parent $TargetPayload) 'release') 'lib'
    $targetLib = Join-Path $TargetPayload 'lib'
    if (-not (Test-Path $releaseLib)) { return 0 }

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

$inherited = Copy-InheritedKernels -TargetPayload $InstallDir -Hip $deviceHip -Cuda $deviceCuda -Optix $deviceOptix
if ($inherited) {
    Write-Step "Filled $inherited kernel(s) from the committed payload"
    Write-Host "   For devices this build supports but compiled no kernels for. They are as" -ForegroundColor DarkYellow
    Write-Host "   old as the last publish, so they do not contain local kernel changes." -ForegroundColor DarkYellow

    # The case that actually costs someone an afternoon: a GPU is in this machine and
    # the kernels it will run were inherited rather than built, so a kernel edit renders
    # identically and nothing obvious says why. Say it loudly, and name the SDK.
    #
    # Not an error. It is a perfectly reasonable state - a developer working on the host
    # side has no reason to install a kernel compiler - and it is what makes a missing
    # SDK survivable at all. It is only a trap for someone who just edited a kernel.
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

Write-Step "Done - installed to $(ConvertTo-CMakePath $InstallDir)"

# Say where the full build lives. Nothing else announces publish_payload.ps1, and a
# developer who has just changed kernel code has no way to know it exists - the payload
# guard mentions it, but only when it fires, which a Debug+Cycles build never sees.
if (-not $AllArches) {
    Write-Host "   This built kernels for this machine only. To produce a payload for" -ForegroundColor DarkGray
    Write-Host "   everyone - every backend, every shipping architecture, checked and" -ForegroundColor DarkGray
    Write-Host "   staged in big_libs - run publish_payload.ps1." -ForegroundColor DarkGray
}
