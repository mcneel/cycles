#requires -Version 5.1
<#
.SYNOPSIS
    Configures, builds and installs Cycles + ccycles for Rhino.

.DESCRIPTION
    Replaces make_rhino.bat and build_cycles_for_rhino.ps1.

    Everything that used to be a hardcoded absolute path is now discovered:

      * Visual Studio    - via vswhere, newest install with the C++ toolset,
                           VS2022 or later, with the CMake generator derived
                           from it. The old scripts needed VS2022 Professional
                           *and* VS2019 BuildTools side by side, because the
                           win64_vc15 library bundle was built with the VS2019
                           ABI.
      * CUDA / OptiX /
        HIP / oneAPI     - probed from the usual environment variables and
                           install locations. Anything not found is switched
                           off rather than failing the build, so a developer
                           with no GPU SDKs installed still gets a working
                           CPU-only Cycles.
      * MSVC redist and
        Windows Kits     - dropped entirely; CMake locates these itself.

    The CUDA architecture list is deliberately NOT pinned here any more. Upstream
    Cycles keeps CYCLES_CUDA_BINARIES_ARCH current with what the toolkit and the
    renderer actually support; the old hardcoded list still named sm_37, which
    has not been supported since Cycles 4.2.

.PARAMETER Configuration
    Debug, Release or RelWithDebInfo. Release maps to RelWithDebInfo in the
    CMake build so we keep usable symbols, matching the old behaviour.

.PARAMETER Devices
    Which GPU backends to enable. Defaults to whatever is detected on the
    machine. Pass 'cpu' to force a CPU-only build.

.PARAMETER InstallDir
    Where to place ccycles.dll and its dependencies. Defaults to the Rhino
    Plug-ins output directory for the chosen configuration.

.PARAMETER CudaBinaries
    Build the full set of CUDA cubins instead of PTX only. Slow; used for
    release builds.

.PARAMETER ConfigureOnly
    Run the CMake configure step and stop, leaving a solution to open in VS.

.EXAMPLE
    .\build_cycles.ps1 -Configuration Release

.EXAMPLE
    .\build_cycles.ps1 -Devices cpu -ConfigureOnly
#>
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release', 'RelWithDebInfo')]
    [string]$Configuration = 'Release',

    [ValidateSet('cpu', 'cuda', 'optix', 'hip', 'oneapi')]
    [string[]]$Devices,

    [string]$InstallDir,

    [switch]$CudaBinaries,

    [switch]$ConfigureOnly,

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

foreach ($tool in 'cmake', 'git', 'python') {
    $cmd = Get-Command $tool -ErrorAction SilentlyContinue
    if (-not $cmd) { throw "'$tool' is not on PATH. Install it and re-run." }
    Write-Found $tool $cmd.Source
}

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
    & cmake --help 2>$null |
        Select-String -Pattern '^\s*\*?\s*(Visual Studio (\d+) \d+)' |
        ForEach-Object {
            [pscustomobject]@{
                Name  = $_.Matches[0].Groups[1].Value.Trim()
                Major = [int]$_.Matches[0].Groups[2].Value
            }
        }
)

$cmakeGenerator = $cmakeGenerators |
    Where-Object { $_.Major -le $vsMajor } |
    Sort-Object Major -Descending |
    Select-Object -First 1 -ExpandProperty Name

if (-not $cmakeGenerator) {
    throw ("CMake offers no Visual Studio generator at or below version $vsMajor. " +
           "Installed CMake is $((& cmake --version | Select-Object -First 1)); " +
           "either install a newer CMake or a Visual Studio it supports.")
}

Write-Found "VS $vsMajor" $vsPath
Write-Found 'generator' $cmakeGenerator

# Enter the Visual Studio developer environment.
#
# The oneAPI kernel is compiled by the clang++ shipped in the library bundle,
# and that compiler locates the MSVC toolchain and Windows SDK through the
# environment rather than through CMake. Without this it fails with
# "unable to find a Visual Studio installation". The devshell module is
# resolved from the detected install rather than hardcoded, unlike the previous
# scripts which pinned a VS2022 Professional path and then entered a VS2019
# BuildTools environment.
$devShell = Join-Path $vsPath 'Common7\Tools\Microsoft.VisualStudio.DevShell.dll'
if (Test-Path $devShell) {
    if (-not $env:VSINSTALLDIR) {
        Import-Module $devShell
        Enter-VsDevShell -VsInstallPath $vsPath -SkipAutomaticLocation -DevCmdArguments '-arch=x64 -host_arch=x64' | Out-Null
        Write-Found 'VS devshell' 'entered (x64)'
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
# variable useless for its one real purpose, trying a different SDK. ROCm 6.4's
# amd_hip_vector_types.h does not compile against this tree, so being unable to
# select another version is a dead end rather than a safeguard.
if ($env:HIP_PATH -and (Test-Path $env:HIP_PATH)) {
    $hipPath = $env:HIP_PATH
    Write-Host ("   {0,-12} using HIP_PATH as given: {1}" -f 'HIP', $hipPath) -ForegroundColor DarkYellow
}
elseif (-not $hipPath) { $hipPath = $hip6 }
# HIP is detected but kept out of the automatic set, because its kernels do not
# currently build: ROCm 6.4 and 7.1 both fail compiling AMD's own
# amd_hip_vector_types.h against this tree ("non-const lvalue reference cannot
# bind to a temporary"), on the first architecture, with matching hipcc and
# headers. Leaving it in the default set fails every build, including the ones
# driven from Rhino.sln.
#
# This is a regression to fix, not a decision: the 3.5 build renders on AMD
# hardware. It is survivable only because GETDEVICE is bounds-checked now, so a
# build without HIP degrades to CPU instead of crashing Rhino. Pass
# -Devices cpu,hip to work on it.
if ($hipPath -and (Test-Path $hipPath)) {
    $hipAvailable = $true
    Write-Found 'HIP' "$hipPath (not enabled by default - kernels do not compile, see RH-97816)"
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

if (-not $Devices) {
    $Devices = if ($detected.Count) { $detected.ToArray() } else { @('cpu') }
    Write-Host "   -> enabling: $($Devices -join ', ')" -ForegroundColor Cyan
}
else {
    Write-Host "   -> requested: $($Devices -join ', ')" -ForegroundColor Cyan
    foreach ($d in $Devices) {
        # hip is deliberately absent from $detected, so check its own flag.
        if ($d -eq 'hip') {
            if (-not $hipAvailable) {
                throw "'hip' was requested but no ROCm install was found. Set HIP_PATH."
            }
            continue
        }
        if ($d -ne 'cpu' -and $detected -notcontains $d) {
            throw "'$d' was requested but its toolkit was not detected. Install it, set the matching environment variable, or drop it from -Devices."
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

Write-Step "Configuring ($cmakeConfig)"
Write-Host "   install -> $(ConvertTo-CMakePath $InstallDir)"

if (-not [System.IO.Path]::IsPathRooted($BuildDir)) { $BuildDir = Join-Path $cyclesRoot $BuildDir }

$cmakeArgs = @(
    '-S', (ConvertTo-CMakePath $cyclesRoot)
    '-B', (ConvertTo-CMakePath $BuildDir)
    '-G', $cmakeGenerator
    '-A', 'x64'
    "-DCMAKE_INSTALL_PREFIX=$(ConvertTo-CMakePath $InstallDir)"
    '-DWITH_CYCLES_ALEMBIC=OFF'
    '-DWITH_CYCLES_USD=OFF'
    '-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF'
)

if ($Devices -contains 'optix') {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_OPTIX=ON', "-DOPTIX_ROOT_DIR=$(ConvertTo-CMakePath $optixPath)"
} else {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_OPTIX=OFF'
}

if ($Devices -contains 'cuda') {
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
    # Leave CYCLES_CUDA_BINARIES_ARCH at the upstream default unless a full
    # cubin build was asked for; PTX-only keeps iteration times sane.
    if (-not $CudaBinaries) { $cmakeArgs += '-DCYCLES_CUDA_BINARIES_ARCH=compute_52' }
} else {
    $cmakeArgs += '-DWITH_CYCLES_CUDA_BINARIES=OFF'
}

if ($Devices -contains 'hip') {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_HIP=ON'
    $cmakeArgs += "-DHIP_ROOT_DIR=$(ConvertTo-CMakePath $hipPath)"
    # Device support and kernel binaries are separate switches, exactly as they
    # are for CUDA, and WITH_CYCLES_HIP_BINARIES defaults to OFF. Enabling only
    # the device gave a build with HIP compiled in and no HIP kernels, so Cycles
    # found no usable AMD device at all - which is what crashed Rhino on a
    # machine whose GPU the 3.5 build renders with quite happily.
    $cmakeArgs += '-DWITH_CYCLES_HIP_BINARIES=ON'
} else {
    # Every device switch is set explicitly, ON or OFF, on every configure.
    # Only ever adding =ON meant the CMake cache remembered whatever a previous
    # run enabled, so -Devices cpu still built HIP in an existing build
    # directory - the flags described the difference from last time rather than
    # what was asked for.
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_HIP=OFF'
    $cmakeArgs += '-DWITH_CYCLES_HIP_BINARIES=OFF'
}
if ($Devices -contains 'oneapi') {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_ONEAPI=ON'
    $cmakeArgs += "-D_LEVEL_ZERO_INCLUDE_DIR=$(ConvertTo-CMakePath (Join-Path $levelZeroRoot 'include'))"
    $cmakeArgs += "-D_LEVEL_ZERO_LIBRARY=$(ConvertTo-CMakePath (Join-Path $levelZeroRoot 'lib'))"
} else {
    $cmakeArgs += '-DWITH_CYCLES_DEVICE_ONEAPI=OFF'
}

Write-Host "   cmake $($cmakeArgs -join ' ')" -ForegroundColor DarkGray

& cmake @cmakeArgs
if ($LASTEXITCODE -ne 0) { throw "CMake configure failed with exit code $LASTEXITCODE." }

if ($ConfigureOnly) {
    Write-Step "Configured. Open $BuildDir\Cycles.sln in Visual Studio, or re-run without -ConfigureOnly."
    return
}

Write-Step "Building ($cmakeConfig)"
& cmake --build $BuildDir --config $cmakeConfig --target install --parallel
if ($LASTEXITCODE -ne 0) { throw "Build failed with exit code $LASTEXITCODE." }

Write-Step "Done - installed to $(ConvertTo-CMakePath $InstallDir)"
