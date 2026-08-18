[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    # release = what ships (Release kernels + RelWithDebInfo ccycles.dll/pdb).
    # wrapper = wrapper-only fast rebuild; leaves kernels and kernel sources untouched.
    [ValidateSet("release", "wrapper")]
    [string]$BuildType = "release",
    [string]$RhinoBranchName
)

$ErrorActionPreference = "Stop"

$scriptRoot = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
. (Join-Path $scriptRoot "..\rhino_branch_info.ps1")

$branchInfo = Resolve-RhinoBranchInfo -StartPath $scriptRoot -RhinoBranchName $RhinoBranchName
$logPath = Join-Path $scriptRoot "make_rhino_all.log"
try {
    if (Test-Path $logPath) {
        Remove-Item -LiteralPath $logPath -Force -ErrorAction SilentlyContinue
    }
}
catch {
}

$transcriptStarted = $false
try {
    Start-Transcript -Path $logPath -Force | Out-Null
    $transcriptStarted = $true
}
catch {
}

$script:LogPath = $logPath
$script:TranscriptStarted = $transcriptStarted
$script:StageTimings = New-Object System.Collections.Generic.List[object]
$script:OutputErrors = New-Object System.Collections.Generic.List[string]

function Write-Log {
    param([Parameter(Mandatory = $true)][AllowEmptyString()][string]$Message)

    $line = "{0} {1}" -f (Get-Date -Format "yyyy-MM-dd HH:mm:ss"), $Message
    Write-Host $line

    if (-not $script:TranscriptStarted) {
        Add-Content -LiteralPath $script:LogPath -Value $line
    }
}

function PathsEqual {
    param(
        [Parameter(Mandatory = $true)][string]$A,
        [Parameter(Mandatory = $true)][string]$B
    )

    $fullA = [System.IO.Path]::GetFullPath($A)
    $fullB = [System.IO.Path]::GetFullPath($B)
    return [string]::Equals($fullA, $fullB, [System.StringComparison]::OrdinalIgnoreCase)
}

function Assert-AllowedCleanupPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    $fullPath = [System.IO.Path]::GetFullPath($Path)
    foreach ($allowed in $script:AllowedCleanupPaths) {
        if (PathsEqual -A $fullPath -B $allowed) {
            return
        }
    }

    throw "Refusing to clean non-whitelisted path '$fullPath'. Allowed paths: $($script:AllowedCleanupPaths -join ', ')"
}

function Invoke-TimedStage {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][scriptblock]$Action
    )

    Write-Log ""
    Write-Log "=== $Name ==="

    $stageSw = [System.Diagnostics.Stopwatch]::StartNew()
    $status = "success"

    try {
        & $Action
    }
    catch {
        $status = "failed"
        throw
    }
    finally {
        $stageSw.Stop()
        [void]$script:StageTimings.Add([PSCustomObject]@{
                Name    = $Name
                Seconds = [Math]::Round($stageSw.Elapsed.TotalSeconds, 1)
                Status  = $status
            })
        Write-Log ("=== {0} ({1:N1}s, {2}) ===" -f $Name, $stageSw.Elapsed.TotalSeconds, $status)
    }
}

function Resolve-VsBuildEnv {
    # Auto-detect the newest installed Visual Studio that this CMake can target (VS2019/VS2022;
    # VS18+ has no CMake generator yet) and derive the generator name + MSVC redist dir from it.
    $vswhere = Join-Path ${env:ProgramFiles(x86)} "Microsoft Visual Studio\Installer\vswhere.exe"
    if (-not (Test-Path $vswhere)) {
        throw "vswhere.exe not found at '$vswhere'; cannot locate Visual Studio."
    }

    $query = @("-products", "*", "-requires", "Microsoft.VisualStudio.Component.VC.Tools.x86.x64",
        "-version", "[16.0,18.0)", "-latest", "-property")
    $installPath = (& $vswhere @query "installationPath") | Select-Object -First 1
    $installVersion = (& $vswhere @query "installationVersion") | Select-Object -First 1
    if ([string]::IsNullOrWhiteSpace($installPath)) {
        throw "No Visual Studio 2019/2022 with the C++ toolset (VC.Tools.x86.x64) was found."
    }

    $major = [int]($installVersion.Split('.')[0])
    $generator = switch ($major) {
        17 { "Visual Studio 17 2022" }
        16 { "Visual Studio 16 2019" }
        default { throw "Unsupported Visual Studio major version '$major' for this CMake's generators." }
    }

    $redistRoot = Join-Path $installPath "VC\Redist\MSVC"
    if (-not (Test-Path $redistRoot)) {
        throw "MSVC redist folder not found at '$redistRoot'."
    }
    $redistDir = Get-ChildItem $redistRoot -Directory |
        Where-Object { $_.Name -match '^\d+\.\d+\.\d+$' } |
        Sort-Object { [version]$_.Name } |
        Select-Object -Last 1 -ExpandProperty FullName
    if ([string]::IsNullOrWhiteSpace($redistDir)) {
        throw "No numbered MSVC redist version folder found under '$redistRoot'."
    }

    return [PSCustomObject]@{
        Generator = $generator
        RedistDir = ($redistDir -replace '\\', '/')
    }
}

$buildDirInput = if ($env:BUILD_DIR) { $env:BUILD_DIR } else { "build" }
$buildDir = if ([System.IO.Path]::IsPathRooted($buildDirInput)) { [System.IO.Path]::GetFullPath($buildDirInput) } else { [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $buildDirInput)) }
$expectedBuildDir = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot "build"))
$installDir = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot "install"))
$expectedInstallDir = $installDir
$hipBuildDir = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot "build_hip"))
$expectedHipBuildDir = $hipBuildDir

$guardErrors = New-Object System.Collections.Generic.List[string]
if (-not (PathsEqual -A $buildDir -B $expectedBuildDir)) {
    [void]$guardErrors.Add("For safety, BUILD_DIR must resolve to '$expectedBuildDir'. Current value resolves to '$buildDir'.")
}
if (-not (PathsEqual -A $installDir -B $expectedInstallDir)) {
    [void]$guardErrors.Add("For safety, install directory must resolve to '$expectedInstallDir'. Current value resolves to '$installDir'.")
}

$script:AllowedCleanupPaths = @($expectedBuildDir, $expectedInstallDir, $expectedHipBuildDir)

$libRoot = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot "..\lib"))
$rhinoCyclesRoot = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot "..\..\..\..\..\..\big_libs\RhinoCycles"))
$dllDest = Join-Path $rhinoCyclesRoot "ccycles\win\release"
$kernelDestinations = @(
    (Join-Path $dllDest "lib")
)

$cmakeExe = if (Test-Path "C:\Tools\cmake329\bin\cmake.exe") { "C:\Tools\cmake329\bin\cmake.exe" } else { "cmake" }
$optixRoot = "C:\ProgramData\NVIDIA Corporation\OptiX SDK 7.6.0"
# Absolute, so the build no longer depends on being launched from the cycles folder.
$dpcppRoot = ([System.IO.Path]::GetFullPath((Join-Path $libRoot "win64_vc15\dpcpp"))) -replace '\\', '/'
$levelZeroRoot = ([System.IO.Path]::GetFullPath((Join-Path $libRoot "win64_vc15\level-zero"))) -replace '\\', '/'
$vsBuildEnv = Resolve-VsBuildEnv
$vsGenerator = $vsBuildEnv.Generator
$msvcRedistDir = $vsBuildEnv.RedistDir
$windowsKitsDir = "C:/Program Files (x86)/Windows Kits/10"
$rhinoBranchRoot = $branchInfo.BranchRoot
$rhinoBranchName = $branchInfo.BranchName
$rhinoMajorVersion = $branchInfo.MajorVersion
$dockerHostRoot = $rhinoBranchRoot -replace '\\', '/'
$dockerContainerRoot = "/rhino/repo"
$dockerVolume = "${dockerHostRoot}:$dockerContainerRoot"

$buildMode = $BuildType.ToLowerInvariant()

$buildConfig = switch ($buildMode) {
    "release" { "Release" }
    "wrapper" { "RelWithDebInfo" }
    default { throw "Unsupported build mode '$BuildType'." }
}

$wrappersConfig = if ($buildMode -eq "release") { "RelWithDebInfo" } else { $null }
$isWrapperMode = ($buildMode -eq "wrapper")
$script:SvnRetryCount = 500
$script:SvnRetryDelaySeconds = 1

function Test-RequiredLibContent {
    param([Parameter(Mandatory = $true)][string]$LibPath)
    if (-not (Test-Path (Join-Path $LibPath "openimageio\include\OpenImageIO\imageio.h"))) { return $false }
    if (-not (Test-Path (Join-Path $LibPath "openimageio\lib"))) { return $false }
    if (-not (Test-Path (Join-Path $LibPath "openexr\include\OpenEXR\ImfVersion.h"))) { return $false }
    return $true
}

function Get-CyclesLibrariesVersion {
    $versionHeaderPath = Join-Path $scriptRoot "src\util\version.h"
    if (-not (Test-Path $versionHeaderPath)) {
        throw "Could not find src\util\version.h."
    }

    $versionMatch = Select-String -Path $versionHeaderPath -Pattern '^\s*#define\s+CYCLES_BLENDER_LIBRARIES_VERSION\s+(\S+)'
    if (-not $versionMatch -or -not $versionMatch.Matches -or $versionMatch.Matches.Count -eq 0) {
        throw "Could not determine CYCLES_BLENDER_LIBRARIES_VERSION from src\util\version.h."
    }

    return $versionMatch.Matches[0].Groups[1].Value.Trim('"')
}

function Get-SvnRetryDelaySeconds {
    return $script:SvnRetryDelaySeconds
}

function Invoke-SvnCommand {
    param([Parameter(Mandatory = $true)][string[]]$Arguments)

    & svn @Arguments 2>&1 | Out-Host
    return $LASTEXITCODE
}

function Ensure-SvnLib {
    param(
        [Parameter(Mandatory = $true)][string]$LibName,
        [Parameter(Mandatory = $true)][string]$SvnLibBaseUrl
    )

    $libPath = Join-Path $libRoot $LibName
    if ((Test-Path $libPath) -and (Test-RequiredLibContent -LibPath $libPath)) {
        Write-Log "[SVN] $LibName ready."
        return
    }

    if (-not (Test-Path $libRoot)) {
        New-Item -Path $libRoot -ItemType Directory -Force | Out-Null
    }

    $maxAttempts = $script:SvnRetryCount + 1
    $lastSvnExitCode = 0

    for ($attempt = 1; $attempt -le $maxAttempts; $attempt++) {
        $workingCopyExists = Test-Path (Join-Path $libPath ".svn")
        $operation = if ($workingCopyExists) { "update" } else { "checkout" }
        Write-Log "[SVN] $LibName attempt $attempt/$maxAttempts ($operation)."

        if ($workingCopyExists) {
            $cleanupExitCode = Invoke-SvnCommand -Arguments @("--non-interactive", "cleanup", $libPath)
            if ($cleanupExitCode -ne 0) {
                $lastSvnExitCode = $cleanupExitCode
            }
            else {
                $lastSvnExitCode = Invoke-SvnCommand -Arguments @("--non-interactive", "update", $libPath)
            }
        }
        else {
            $lastSvnExitCode = Invoke-SvnCommand -Arguments @("--non-interactive", "checkout", "--force", "$SvnLibBaseUrl/$LibName", $libPath)
        }

        if ($lastSvnExitCode -eq 0 -and (Test-RequiredLibContent -LibPath $libPath)) {
            Write-Log "[SVN] $LibName ready."
            return
        }

        if ($attempt -lt $maxAttempts) {
            $reason = if ($lastSvnExitCode -ne 0) { "svn exit code $lastSvnExitCode" } else { "required files are still missing" }
            $delaySeconds = Get-SvnRetryDelaySeconds
            Write-Log "[SVN] $LibName attempt $attempt/$maxAttempts failed ($reason). Retrying in $delaySeconds seconds."
            Start-Sleep -Seconds $delaySeconds
        }
    }

    throw "Failed to prepare SVN library '$LibName' after $maxAttempts attempt(s)."
}

function Remove-DirectorySafe {
    param([Parameter(Mandatory = $true)][string]$Path)

    Assert-AllowedCleanupPath -Path $Path

    if (-not (Test-Path $Path)) {
        Write-Log "Cleanup skipped; folder not found: '$Path'."
        return
    }

    $previousProgressPreference = $ProgressPreference
    try {
        $ProgressPreference = "SilentlyContinue"
        try {
            Remove-Item -LiteralPath $Path -Recurse -Force -ErrorAction Stop
            Write-Log "Cleaned '$Path'."
        }
        catch {
            $stalePath = "$Path.stale.$([DateTime]::Now.ToString('yyyyMMdd_HHmmss'))"
            try {
                Rename-Item -LiteralPath $Path -NewName (Split-Path -Leaf $stalePath) -ErrorAction Stop
                Write-Log "WARNING: Could not clean '$Path'. Renamed to '$stalePath'."
            }
            catch {
                Write-Log "WARNING: Could not clean '$Path'. Continuing."
            }
        }
    }
    finally {
        $ProgressPreference = $previousProgressPreference
    }
}

function Copy-PrimaryBinaries {
    $files = @(
        "ccycles.dll",
        "ccycles.pdb",
        "cycles_kernel_oneapi_jit.dll",
        "sycl6.dll",
        "pi_level_zero.dll",
        "xptifw.dll",
        "ze_loader.dll"
    )

    foreach ($file in $files) {
        $source = Join-Path $installDir $file
        if (Test-Path $source) {
            Copy-Item -LiteralPath $source -Destination $dllDest -Force
            $destination = Join-Path $dllDest $file
            $stamp = (Get-Item -LiteralPath $destination).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
            Write-Log "Copied $file ($stamp)."
        }
        else {
            # Do not stall the build, but record it so it is reported loudly at the end.
            [void]$script:OutputErrors.Add("Missing build output: $source")
            Write-Log "ERROR: Missing build output '$source'."
        }
    }
}

function Copy-KernelArtifacts {
    # PurgePatterns names the kernel families this run owns, so binaries for an arch
    # that is no longer built cannot linger in the destination and get loaded later.
    # A family is only purged when this run actually produced one, otherwise a failed
    # or skipped stage would wipe good kernels (e.g. the HIP fatbins without Docker).
    param([string[]]$PurgePatterns = @())

    $installLibDir = Join-Path $installDir "lib"
    if (-not (Test-Path $installLibDir)) {
        Write-Log "WARNING: Missing '$installLibDir'."
        return
    }

    foreach ($destination in $kernelDestinations) {
        New-Item -ItemType Directory -Path $destination -Force | Out-Null

        foreach ($pattern in $PurgePatterns) {
            $fresh = @(Get-ChildItem -Path $installLibDir -Filter $pattern -File -ErrorAction SilentlyContinue)
            if ($fresh.Count -eq 0) {
                Write-Log "Not purging '$pattern' in '$destination'; this build produced none."
                continue
            }

            $freshNames = $fresh.Name
            Get-ChildItem -Path $destination -Filter $pattern -File -ErrorAction SilentlyContinue |
                Where-Object { $freshNames -notcontains $_.Name } |
                ForEach-Object {
                    Remove-Item -LiteralPath $_.FullName -Force
                    Write-Log "Purged stale kernel '$($_.Name)' from '$destination'."
                }
        }

        Copy-Item -Path (Join-Path $installLibDir "*") -Destination $destination -Force
    }
}

function Copy-KernelSources {
    # RhinoCyclesKernelCompiler compiles these at runtime; if they drift from
    # ccycles.dll the KernelData layouts mismatch and GPU compiles break.
    param([switch]$Optional)

    $installSourceDir = Join-Path $installDir "source"
    if (-not (Test-Path $installSourceDir)) {
        if ($Optional) {
            # Only the CMake install target stages install\source, so wrapper builds
            # legitimately have none; they leave the deployed sources as they are.
            Write-Log "WARNING: No kernel source tree at '$installSourceDir'; kernel sources NOT refreshed."
        }
        else {
            [void]$script:OutputErrors.Add("Missing build output: $installSourceDir")
            Write-Log "ERROR: Missing kernel source tree '$installSourceDir'."
        }
        return
    }

    $sourceDest = Join-Path $dllDest "source"
    robocopy $installSourceDir $sourceDest /MIR /NFL /NDL /NJH /NJS /NP | Out-Null
    if ($LASTEXITCODE -ge 8) {
        [void]$script:OutputErrors.Add("Failed to copy kernel sources to '$sourceDest' (robocopy $LASTEXITCODE).")
        Write-Log "ERROR: robocopy of kernel sources failed ($LASTEXITCODE)."
    }
    else {
        Write-Log "Copied kernel source tree to '$sourceDest'."
    }
}

function Copy-AllOutputs {
    New-Item -ItemType Directory -Path $dllDest -Force | Out-Null
    foreach ($destination in $kernelDestinations) {
        New-Item -ItemType Directory -Path $destination -Force | Out-Null
    }

    Copy-PrimaryBinaries
    # CUDA cubins and the OptiX/compute PTX all come from this Windows build.
    Copy-KernelArtifacts -PurgePatterns @("kernel_sm_*.cubin", "kernel_*.ptx")
    Copy-KernelSources
}

function Copy-WrapperOnlyOutputs {
    New-Item -ItemType Directory -Path $dllDest -Force | Out-Null

    $wrapperDllSource = Join-Path $installDir "ccycles.dll"
    if (-not (Test-Path $wrapperDllSource)) {
        throw "Expected wrapper DLL was not found: '$wrapperDllSource'."
    }
    $wrapperDllDest = Join-Path $dllDest "ccycles.dll"
    Copy-Item -LiteralPath $wrapperDllSource -Destination $wrapperDllDest -Force
    $stamp = (Get-Item -LiteralPath $wrapperDllDest).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
    Write-Log "Copied ccycles.dll ($stamp)."

    $wrapperPdbSource = Join-Path $installDir "ccycles.pdb"
    if (Test-Path $wrapperPdbSource) {
        $wrapperPdbDest = Join-Path $dllDest "ccycles.pdb"
        Copy-Item -LiteralPath $wrapperPdbSource -Destination $wrapperPdbDest -Force
        Write-Log "Copied ccycles.pdb."
    }
    else {
        Write-Log "WARNING: Wrapper symbols were not found at '$wrapperPdbSource'."
    }

    Copy-KernelSources -Optional
}

function Promote-DebuggableWrappersToInstall {
    param([Parameter(Mandatory = $true)][string]$WrapperConfig)

    # Wrapper target is ccycles.dll; keep device/kernel binaries from Release install.
    $wrapperDllSource = Join-Path $buildDir "bin\$WrapperConfig\ccycles.dll"
    $wrapperDllDest = Join-Path $installDir "ccycles.dll"
    if (-not (Test-Path $wrapperDllSource)) {
        throw "Expected wrapper DLL was not found: '$wrapperDllSource'."
    }
    Copy-Item -LiteralPath $wrapperDllSource -Destination $wrapperDllDest -Force
    Write-Log "Promoted debuggable wrapper: '$wrapperDllDest' from '$wrapperDllSource'."
}

function Promote-CcyclesPdbToInstall {
    param([Parameter(Mandatory = $true)][string[]]$ConfigCandidates)

    foreach ($config in $ConfigCandidates) {
        if ([string]::IsNullOrWhiteSpace($config)) {
            continue
        }

        $pdbSource = Join-Path $buildDir "bin\$config\ccycles.pdb"
        if (Test-Path $pdbSource) {
            $pdbDest = Join-Path $installDir "ccycles.pdb"
            Copy-Item -LiteralPath $pdbSource -Destination $pdbDest -Force
            Write-Log "Promoted wrapper symbols: '$pdbDest' from '$pdbSource'."
            return
        }
    }

    Write-Log "ccycles.pdb not found in candidate configs ($($ConfigCandidates -join ', '))."
}

function Ensure-DockerReady {
    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        Write-Log "Docker CLI was not found. Skipping Docker HIP step."
        return $false
    }

    & docker version *> $null
    if ($LASTEXITCODE -eq 0) {
        return $true
    }

    Write-Log "Starting Docker Desktop..."
    & docker desktop start | Out-Host

    $deadline = (Get-Date).AddSeconds(120)
    while ((Get-Date) -lt $deadline) {
        & docker version *> $null
        if ($LASTEXITCODE -eq 0) {
            return $true
        }
        Start-Sleep -Seconds 3
    }

    Write-Log "Docker Desktop did not become ready. Skipping Docker HIP step."
    return $false
}

function Run-DockerHipFlow {
    Push-Location $scriptRoot
    try {
        & docker build -f DockerfileHIP -t ubuntu-cycles-hip-build . | Out-Host
        if ($LASTEXITCODE -ne 0) {
            Write-Log "Docker build failed with error code $LASTEXITCODE."
            return $LASTEXITCODE
        }

        $dockerRunArgs = @("run")
        try {
            if ((-not [Console]::IsInputRedirected) -and (-not [Console]::IsOutputRedirected)) {
                $dockerRunArgs += "-it"
            }
        }
        catch {
        }
        $dockerRunArgs += @("-v", $dockerVolume, "ubuntu-cycles-hip-build")

        & docker @dockerRunArgs | Out-Host
        if ($LASTEXITCODE -ne 0) {
            Write-Log "Docker run failed with error code $LASTEXITCODE."
            return $LASTEXITCODE
        }

        # Only the Docker stage builds HIP fatbins, so only it may purge them.
        Copy-KernelArtifacts -PurgePatterns @("kernel_gfx*.fatbin")
        return 0
    }
    finally {
        Pop-Location
    }
}

$overallSw = [System.Diagnostics.Stopwatch]::StartNew()
$exitCode = 1

try {
    Write-Log "Log file: $logPath"
    Write-Log "Script root: $scriptRoot"
    Write-Log "Build mode: $buildMode"
    Write-Log "Build config: $buildConfig"
    Write-Log "VS generator: $vsGenerator"
    Write-Log "MSVC redist dir: $msvcRedistDir"
    if ($isWrapperMode) {
        Write-Log "Wrapper mode: wrapper-only build (CUDA/OptiX/HIP enabled with no kernel binaries, no oneAPI device, no Docker stage; version info still applied)."
    }
    if ($wrappersConfig) {
        Write-Log "Wrapper override config: $wrappersConfig"
    }
    Write-Log "Build dir: $buildDir"
    Write-Log "Install dir: $installDir"
    Write-Log "HIP build dir: $hipBuildDir"
    Write-Log "Rhino branch root: $rhinoBranchRoot"
    Write-Log "Rhino branch: $rhinoBranchName (major $rhinoMajorVersion, source $($branchInfo.Source))"
    Write-Log "Docker volume: $dockerVolume"
    Write-Log "SVN retries after first failure: $script:SvnRetryCount (fixed $script:SvnRetryDelaySeconds second delay)."
    Write-Log "Allowed cleanup paths: $($script:AllowedCleanupPaths -join ', ')"

    if ($guardErrors.Count -gt 0) {
        throw ($guardErrors -join " ")
    }

    Invoke-TimedStage -Name "Ensure SVN Libraries" -Action {
        if (-not (Get-Command svn -ErrorAction SilentlyContinue)) {
            throw "svn was not found on PATH."
        }
        $cyclesLibVersion = Get-CyclesLibrariesVersion
        $svnLibBaseUrl = "https://svn.blender.org/svnroot/bf-blender/tags/blender-$cyclesLibVersion-release/lib"
        if (-not $isWrapperMode) {
            Ensure-SvnLib -LibName "linux_x86_64_glibc_228" -SvnLibBaseUrl $svnLibBaseUrl
        }
        Ensure-SvnLib -LibName "win64_vc15" -SvnLibBaseUrl $svnLibBaseUrl
    }

    Invoke-TimedStage -Name "Clean Build Folders" -Action {
        Remove-DirectorySafe -Path $buildDir
        Remove-DirectorySafe -Path $installDir
        Remove-DirectorySafe -Path $hipBuildDir
    }

    Invoke-TimedStage -Name "Configure + Build ($buildMode)" -Action {
        $cmakeArgs = @(
            "-S", $scriptRoot,
            "-B", $buildDir,
            "-G", $vsGenerator,
            "-A", "x64",
            # v142 (14.29) toolset: the bundled DPC++ (Clang 16) can't compile the VS2022 14.4x STL
            # (needs Clang 19+). The cycles-only tree has no Rhino >=VS2022 guard. Drop when DPC++ is updated.
            "-T", "v142",
            "-DWITH_CYCLES_ALEMBIC=OFF",
            "-DWITH_CYCLES_USD=OFF",
            "-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF",
            "-DSYCL_ROOT_DIR=$dpcppRoot",
            "-DLEVEL_ZERO_ROOT_DIR=$levelZeroRoot",
            "-DMSVC_REDIST_DIR=$msvcRedistDir",
            "-DWINDOWS_KITS_DIR=$windowsKitsDir"
        )

        if ($isWrapperMode) {
            $cmakeArgs += @(
                "-DWITH_CYCLES_DEVICE_CUDA=ON",
                "-DWITH_CYCLES_DEVICE_OPTIX=ON",
                "-DWITH_CYCLES_CUDA_BINARIES=OFF",
                "-DCYCLES_CUDA_BINARIES_ARCH=sm_50;sm_52;sm_60;sm_61;sm_70;sm_75;sm_86;sm_89;sm_120;compute_75",
                "-DOPTIX_ROOT_DIR=$optixRoot",
                "-DWITH_CYCLES_DEVICE_HIP=ON",
                "-DWITH_CYCLES_HIP_BINARIES=OFF",
                "-DWITH_CYCLES_DEVICE_ONEAPI=OFF",
                "-DWITH_CYCLES_ONEAPI_BINARIES=OFF"
            )
        }
        else {
            $cmakeArgs += @(
                "-DWITH_CYCLES_CUDA_BINARIES=ON",
                "-DWITH_CYCLES_DEVICE_OPTIX=ON",
                "-DCYCLES_CUDA_BINARIES_ARCH=sm_50;sm_52;sm_60;sm_61;sm_70;sm_75;sm_86;sm_89;sm_120;compute_75",
                "-DOPTIX_ROOT_DIR=$optixRoot",
                "-DWITH_CYCLES_DEVICE_ONEAPI=ON",
                "-DWITH_CYCLES_DEVICE_HIP=ON"
            )
        }

        & $cmakeExe @cmakeArgs | Out-Host
        if ($LASTEXITCODE -ne 0) {
            throw "CMake configure failed with code $LASTEXITCODE."
        }

        Push-Location $buildDir
        try {
            $buildTarget = if ($isWrapperMode) { "ccycles" } else { "install" }
            & $cmakeExe --build . --target $buildTarget --config $buildConfig | Out-Host
            $buildExitCode = $LASTEXITCODE
        }
        finally {
            Pop-Location
        }

        if ($buildExitCode -ne 0) {
            if ($isWrapperMode) {
                throw "Wrapper build failed with code $buildExitCode."
            }
            throw "CMake build/install failed with code $buildExitCode."
        }

        if ($isWrapperMode) {
            # Stage the wrapper into the install dir so versioninfo_changer.ps1
            # can stamp version metadata onto ccycles.dll there.
            New-Item -ItemType Directory -Path $installDir -Force | Out-Null
            Promote-DebuggableWrappersToInstall -WrapperConfig $buildConfig
            Promote-CcyclesPdbToInstall -ConfigCandidates @($buildConfig)
            return
        }

        if ($wrappersConfig) {
            Push-Location $buildDir
            try {
                & $cmakeExe --build . --target ccycles --config $wrappersConfig | Out-Host
                $wrapperBuildExitCode = $LASTEXITCODE
            }
            finally {
                Pop-Location
            }

            if ($wrapperBuildExitCode -ne 0) {
                throw "Wrapper build failed with code $wrapperBuildExitCode."
            }

            Promote-DebuggableWrappersToInstall -WrapperConfig $wrappersConfig
        }

        $pdbCandidates = @()
        if ($wrappersConfig) {
            $pdbCandidates += $wrappersConfig
        }
        $pdbCandidates += $buildConfig
        Promote-CcyclesPdbToInstall -ConfigCandidates $pdbCandidates
    }

    Invoke-TimedStage -Name "Update Version Info" -Action {
        Push-Location ([System.IO.Path]::GetFullPath((Join-Path $scriptRoot "..")))
        try {
            & powershell -NoProfile -ExecutionPolicy Bypass -File ".\versioninfo_changer.ps1" -RhinoBranchName $rhinoBranchName | Out-Host
            if ($LASTEXITCODE -ne 0) {
                Write-Log "WARNING: versioninfo_changer.ps1 failed with code $LASTEXITCODE."
            }
        }
        finally {
            Pop-Location
        }
    }

    Invoke-TimedStage -Name "Copy Build Outputs" -Action {
        if ($isWrapperMode) {
            Copy-WrapperOnlyOutputs
        }
        else {
            Copy-AllOutputs
        }
    }

    if (-not $isWrapperMode) {
        Invoke-TimedStage -Name "Docker HIP Build" -Action {
            if (Ensure-DockerReady) {
                $dockerExitCode = Run-DockerHipFlow
                if ($dockerExitCode -ne 0) {
                    Write-Log "Docker step failed with code $dockerExitCode. Build outputs are already copied."
                }
            }
            else {
                Write-Log "Skipping Docker HIP step."
            }
        }
    }

    $exitCode = 0
}
catch {
    Write-Log "ERROR: $($_.Exception.Message)"
    $exitCode = 1
}
finally {
    $overallSw.Stop()

    Write-Log ""
    Write-Log "=== Timing Summary ==="
    foreach ($entry in $script:StageTimings) {
        Write-Log ("{0}: {1:N1}s ({2})" -f $entry.Name, $entry.Seconds, $entry.Status)
    }
    Write-Log ("Total elapsed: {0:N1}s" -f $overallSw.Elapsed.TotalSeconds)

    if ($script:OutputErrors.Count -gt 0) {
        if ($exitCode -eq 0) { $exitCode = 1 }
        Write-Log ""
        Write-Log "########################################################################"
        Write-Log ("###  BUILD OUTPUT ERROR: {0} required file(s) missing  ###" -f $script:OutputErrors.Count)
        foreach ($err in $script:OutputErrors) {
            Write-Log "###    - $err"
        }
        Write-Log "###  (other outputs may have copied fine; the above did NOT)"
        Write-Log "########################################################################"
    }

    Write-Log "Final exit code: $exitCode"

    if ($script:TranscriptStarted) {
        try {
            Stop-Transcript | Out-Null
        }
        catch {
        }
    }
}

exit $exitCode
