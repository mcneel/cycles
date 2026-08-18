[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet("release", "debug", "release_debuggable", "hybrid", "minimal")]
    [string]$BuildType = "release",
    [string]$RhinoBranchName
)

$ErrorActionPreference = "Stop"

$scriptRoot = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
. (Join-Path $scriptRoot "rhino_branch_info.ps1")

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

# Toolchain discovery. Everything below used to be a hardcoded absolute path,
# which meant the release build only ran on one specific machine.
$cmakeExe = (Get-Command cmake -ErrorAction SilentlyContinue).Source
if (-not $cmakeExe) { throw "cmake was not found on PATH." }

$vswhereExe = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhereExe)) { throw "vswhere.exe not found. Install Visual Studio 2022." }
$vsInstallPath = & $vswhereExe -latest -products * `
    -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 `
    -version '[17.0,18.0)' -property installationPath
if (-not $vsInstallPath) { throw "No Visual Studio 2022 with the C++ toolset found." }
$cmakeGenerator = "Visual Studio 17 2022"

$optixRoot = $env:OPTIX_ROOT_DIR
if (-not $optixRoot) {
    $optixRoot = Get-ChildItem 'C:\ProgramData\NVIDIA Corporation' -Directory -Filter 'OptiX SDK *' -ErrorAction SilentlyContinue |
        Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
}

# Cycles 5.x puts precompiled libraries in lib/<platform> inside the repo;
# 3.5 used a sibling ../lib/win64_vc15 populated from SVN.
$libModernRoot = Join-Path $scriptRoot "lib\windows_x64"
$libLegacyRoot = Join-Path $scriptRoot "..\lib\win64_vc15"
$libBundleRoot = if (Test-Path $libModernRoot) { $libModernRoot } else { $libLegacyRoot }
$dpcppRoot = Join-Path $libBundleRoot "dpcpp"
$levelZeroRoot = Join-Path $libBundleRoot "level-zero"

# The oneAPI kernel build needs these to locate the redistributables it links
# against; derive them from the detected VS install instead of pinning 14.29.30133.
$msvcRedistDir = Get-ChildItem (Join-Path $vsInstallPath 'VC\Redist\MSVC') -Directory -ErrorAction SilentlyContinue |
    Sort-Object Name -Descending | Select-Object -First 1 -ExpandProperty FullName
$windowsKitsDir = "C:/Program Files (x86)/Windows Kits/10"
$rhinoBranchRoot = $branchInfo.BranchRoot
$rhinoBranchName = $branchInfo.BranchName
$rhinoMajorVersion = $branchInfo.MajorVersion
$dockerHostRoot = $rhinoBranchRoot -replace '\\', '/'
$dockerContainerRoot = "/rhino/repo"
$dockerVolume = "${dockerHostRoot}:$dockerContainerRoot"

$buildMode = $BuildType.ToLowerInvariant()

$buildConfig = switch ($buildMode) {
    "debug" { "Debug" }
    "release" { "Release" }
    "release_debuggable" { "RelWithDebInfo" }
    "hybrid" { "Release" }
    "minimal" { "Release" }
    default { throw "Unsupported build mode '$BuildType'." }
}

$wrappersConfig = if ($buildMode -eq "hybrid" -or $buildMode -eq "minimal") { "RelWithDebInfo" } else { $null }
$isMinimalMode = ($buildMode -eq "minimal")
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
    $files = @("ccycles.dll")
    if (-not $isMinimalMode) {
        $files += @(
            "cycles_kernel_oneapi_jit.dll",
            "sycl6.dll",
            "pi_level_zero.dll",
            "xptifw.dll",
            "ze_loader.dll"
        )
    }

    foreach ($file in $files) {
        $source = Join-Path $installDir $file
        if (Test-Path $source) {
            Copy-Item -LiteralPath $source -Destination $dllDest -Force
            $destination = Join-Path $dllDest $file
            $stamp = (Get-Item -LiteralPath $destination).LastWriteTime.ToString("yyyy-MM-dd HH:mm:ss")
            Write-Log "Copied $file ($stamp)."
        }
        else {
            Write-Log "WARNING: Missing '$source'."
        }
    }
}

function Copy-KernelArtifacts {
    $installLibDir = Join-Path $installDir "lib"
    if (-not (Test-Path $installLibDir)) {
        Write-Log "WARNING: Missing '$installLibDir'."
        return
    }

    foreach ($destination in $kernelDestinations) {
        New-Item -ItemType Directory -Path $destination -Force | Out-Null
        Copy-Item -Path (Join-Path $installLibDir "*") -Destination $destination -Force
    }
}

function Copy-AllOutputs {
    New-Item -ItemType Directory -Path $dllDest -Force | Out-Null
    foreach ($destination in $kernelDestinations) {
        New-Item -ItemType Directory -Path $destination -Force | Out-Null
    }

    Copy-PrimaryBinaries
    Copy-KernelArtifacts
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

        Copy-KernelArtifacts
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
    if ($isMinimalMode) {
        Write-Log "Minimal mode: ccycles.dll only (CUDA/OptiX/HIP/oneAPI disabled, hybrid-style wrapper promotion enabled)."
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

    Invoke-TimedStage -Name "Ensure Libraries" -Action {
        # Cycles 4.2+ fetches precompiled libraries as a Git LFS submodule under
        # lib/<platform> via 'make update'. Before that they came from
        # svn.blender.org, which Blender has decommissioned - hence the 500-retry
        # loop this stage used to run. Prefer 'make update' whenever the tree
        # supports it and only fall back to SVN on the old layout.
        $makeBat = Join-Path $scriptRoot "make.bat"
        $usesGitLfsLibraries = Test-Path (Join-Path $scriptRoot ".gitmodules")

        if ($usesGitLfsLibraries -and (Test-Path $makeBat)) {
            if (Test-Path $libModernRoot) {
                Write-Log "[libs] $libModernRoot already present."
                return
            }
            Write-Log "[libs] Running 'make update'."
            $updateProc = Start-Process -FilePath 'cmd.exe' -ArgumentList '/c', "`"$makeBat`"", 'update' `
                -WorkingDirectory $scriptRoot -NoNewWindow -Wait -PassThru
            if ($updateProc.ExitCode -ne 0) {
                throw "'make update' failed with exit code $($updateProc.ExitCode)."
            }
            if (-not (Test-Path $libModernRoot)) {
                throw "'make update' completed but '$libModernRoot' is missing."
            }
            Write-Log "[libs] Ready."
            return
        }

        Write-Log "WARNING: falling back to the legacy SVN library checkout. svn.blender.org has been decommissioned; this stage is expected to fail until the tree is updated to Cycles 4.2 or newer."
        if (-not (Get-Command svn -ErrorAction SilentlyContinue)) {
            throw "svn was not found on PATH, and this tree is too old to use 'make update'."
        }
        $cyclesLibVersion = Get-CyclesLibrariesVersion
        $svnLibBaseUrl = "https://svn.blender.org/svnroot/bf-blender/tags/blender-$cyclesLibVersion-release/lib"
        if (-not $isMinimalMode) {
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
            "-G", $cmakeGenerator,
            "-A", "x64",
            "-DWITH_CYCLES_ALEMBIC=OFF",
            "-DWITH_CYCLES_USD=OFF",
            "-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF",
            "-DSYCL_ROOT_DIR=$dpcppRoot",
            "-DLEVEL_ZERO_ROOT_DIR=$levelZeroRoot",
            "-DWINDOWS_KITS_DIR=$windowsKitsDir"
        )
        if ($msvcRedistDir) { $cmakeArgs += "-DMSVC_REDIST_DIR=$msvcRedistDir" }

        if ($isMinimalMode) {
            $cmakeArgs += @(
                "-DWITH_CYCLES_DEVICE_CUDA=OFF",
                "-DWITH_CYCLES_DEVICE_OPTIX=OFF",
                "-DWITH_CYCLES_CUDA_BINARIES=OFF",
                "-DWITH_CYCLES_DEVICE_HIP=OFF",
                "-DWITH_CYCLES_HIP_BINARIES=OFF",
                "-DWITH_CYCLES_DEVICE_ONEAPI=OFF",
                "-DWITH_CYCLES_ONEAPI_BINARIES=OFF"
            )
        }
        else {
            # CYCLES_CUDA_BINARIES_ARCH is deliberately left at the upstream
            # default. The list pinned here previously still named sm_37, which
            # Cycles dropped in 4.2, and it lagged behind newer architectures.
            $cmakeArgs += @(
                "-DWITH_CYCLES_CUDA_BINARIES=ON",
                "-DWITH_CYCLES_DEVICE_OPTIX=ON",
                "-DWITH_CYCLES_DEVICE_ONEAPI=ON",
                "-DWITH_CYCLES_DEVICE_HIP=ON"
            )
            if ($optixRoot) { $cmakeArgs += "-DOPTIX_ROOT_DIR=$optixRoot" }
            else { Write-Log "WARNING: no OptiX SDK found; set OPTIX_ROOT_DIR. OptiX device will fail to configure." }
        }

        & $cmakeExe @cmakeArgs | Out-Host
        if ($LASTEXITCODE -ne 0) {
            throw "CMake configure failed with code $LASTEXITCODE."
        }

        Push-Location $buildDir
        try {
            & $cmakeExe --build . --target install --config $buildConfig | Out-Host
            $buildExitCode = $LASTEXITCODE
        }
        finally {
            Pop-Location
        }

        if ($buildExitCode -ne 0) {
            throw "CMake build/install failed with code $buildExitCode."
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
        Push-Location $scriptRoot
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
        Copy-AllOutputs
    }

    Invoke-TimedStage -Name "Docker HIP Build" -Action {
        if ($isMinimalMode) {
            Write-Log "Skipping Docker HIP step in minimal mode."
            return
        }

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
