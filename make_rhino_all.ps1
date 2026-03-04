[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [ValidateSet("Release", "Debug")]
    [string]$BuildType = "Release"
)

$ErrorActionPreference = "Stop"

$scriptRoot = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
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

$guardErrors = New-Object System.Collections.Generic.List[string]
if (-not (PathsEqual -A $buildDir -B $expectedBuildDir)) {
    [void]$guardErrors.Add("For safety, BUILD_DIR must resolve to '$expectedBuildDir'. Current value resolves to '$buildDir'.")
}
if (-not (PathsEqual -A $installDir -B $expectedInstallDir)) {
    [void]$guardErrors.Add("For safety, install directory must resolve to '$expectedInstallDir'. Current value resolves to '$installDir'.")
}

$script:AllowedCleanupPaths = @($expectedBuildDir, $expectedInstallDir)

$libRoot = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot "..\lib"))
$rhinoCyclesRoot = [System.IO.Path]::GetFullPath((Join-Path $scriptRoot "..\..\..\..\..\..\big_libs\RhinoCycles"))
$dllDest = Join-Path $rhinoCyclesRoot "ccycles\win\release"
$kernelDestinations = @(
    (Join-Path $dllDest "lib"),
    (Join-Path $rhinoCyclesRoot "lib")
)

$cmakeExe = if (Test-Path "C:\Tools\cmake329\bin\cmake.exe") { "C:\Tools\cmake329\bin\cmake.exe" } else { "cmake" }
$optixRoot = "C:\ProgramData\NVIDIA Corporation\OptiX SDK 7.6.0"
$dpcppRoot = "..\lib\win64_vc15\dpcpp"
$levelZeroRoot = "..\lib\win64_vc15\level-zero"
$msvcRedistDir = "C:/Program Files (x86)/Microsoft Visual Studio/2019/Professional/VC/Redist/MSVC/14.29.30133"
$windowsKitsDir = "C:/Program Files (x86)/Windows Kits/10"
$dockerVolume = "D:/dev/github/mcneel/rhino/8.x:/rhino/rhino-8.x"
$buildConfig = if ($BuildType -ieq "Debug") { "Debug" } else { "RelWithDebInfo" }

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

    if (Test-Path (Join-Path $libPath ".svn")) {
        & svn --non-interactive cleanup $libPath | Out-Host
        & svn --non-interactive update $libPath | Out-Host
    }
    else {
        & svn --non-interactive checkout --force "$SvnLibBaseUrl/$LibName" $libPath | Out-Host
    }

    if ($LASTEXITCODE -ne 0 -or -not (Test-RequiredLibContent -LibPath $libPath)) {
        throw "Failed to prepare SVN library '$LibName'."
    }

    Write-Log "[SVN] $LibName ready."
}

function Remove-DirectorySafe {
    param([Parameter(Mandatory = $true)][string]$Path)

    Assert-AllowedCleanupPath -Path $Path

    if (-not (Test-Path $Path)) {
        Write-Log "Cleanup skipped; folder not found: '$Path'."
        return
    }

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

function Copy-PrimaryBinaries {
    $files = @(
        "ccycles.dll",
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
    Write-Log "Build type: $BuildType"
    Write-Log "Build dir: $buildDir"
    Write-Log "Install dir: $installDir"
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
        Ensure-SvnLib -LibName "linux_x86_64_glibc_228" -SvnLibBaseUrl $svnLibBaseUrl
        Ensure-SvnLib -LibName "win64_vc15" -SvnLibBaseUrl $svnLibBaseUrl
    }

    Invoke-TimedStage -Name "Clean Build Folders" -Action {
        Remove-DirectorySafe -Path $buildDir
        Remove-DirectorySafe -Path $installDir
    }

    Invoke-TimedStage -Name "Configure + Build ($BuildType, all devices)" -Action {
        $cmakeArgs = @(
            "-B", $buildDir,
            "-G", "Visual Studio 16 2019",
            "-A", "x64",
            "-DWITH_CYCLES_ALEMBIC=OFF",
            "-DWITH_CYCLES_USD=OFF",
            "-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF",
            "-DWITH_CYCLES_CUDA_BINARIES=ON",
            "-DWITH_CYCLES_DEVICE_OPTIX=ON",
            "-DCYCLES_CUDA_BINARIES_ARCH=sm_37;sm_50;sm_52;sm_60;sm_61;sm_70;sm_75;sm_86;compute_75",
            "-DOPTIX_ROOT_DIR=$optixRoot",
            "-DWITH_CYCLES_DEVICE_ONEAPI=ON",
            "-DSYCL_ROOT_DIR=$dpcppRoot",
            "-DLEVEL_ZERO_ROOT_DIR=$levelZeroRoot",
            "-DMSVC_REDIST_DIR=$msvcRedistDir",
            "-DWINDOWS_KITS_DIR=$windowsKitsDir",
            "-DWITH_CYCLES_DEVICE_HIP=ON"
        )

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
    }

    Invoke-TimedStage -Name "Update Version Info" -Action {
        Push-Location ([System.IO.Path]::GetFullPath((Join-Path $scriptRoot "..")))
        try {
            & powershell -NoProfile -ExecutionPolicy Bypass -File ".\versioninfo_changer.ps1" | Out-Host
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
