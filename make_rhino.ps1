[CmdletBinding()]
param(
    [Parameter(Position = 0)]
    [string]$Command = "release",

    [Parameter(Position = 1)]
    [string]$Arg1 = "",

    [Parameter(Position = 2)]
    [string]$Arg2 = "",

    [Parameter()]
    [switch]$SkipDocker
)

$scriptRoot = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
$buildDirInput = if ($env:BUILD_DIR) { $env:BUILD_DIR } else { "build" }
$buildDir = if ([System.IO.Path]::IsPathRooted($buildDirInput)) { $buildDirInput } else { [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $buildDirInput)) }

$script:Config = @{
    PythonExe      = "python"
    CMakeExe       = if (Test-Path "C:\Tools\cmake329\bin\cmake.exe") { "C:\Tools\cmake329\bin\cmake.exe" } else { "cmake" }
    OptixRoot      = "C:\ProgramData\NVIDIA Corporation\OptiX SDK 7.6.0"
    DpcppRoot      = "..\lib\win64_vc15\dpcpp"
    LevelZeroRoot  = "..\lib\win64_vc15\level-zero"
    MsvcRedistDir  = "C:/Program Files (x86)/Microsoft Visual Studio/2019/Professional/VC/Redist/MSVC/14.29.30133"
    WindowsKitsDir = "C:/Program Files (x86)/Windows Kits/10"
    DockerVolume   = "D:/dev/github/mcneel/rhino/8.x:/rhino/rhino-8.x"
}

# Utilities
function Get-EnvInt {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][int]$DefaultValue
    )

    $value = [System.Environment]::GetEnvironmentVariable($Name)
    $parsed = 0
    if ([int]::TryParse($value, [ref]$parsed)) {
        return $parsed
    }

    return $DefaultValue
}

function Get-FullPathFromScript {
    param([Parameter(Mandatory = $true)][string]$RelativePath)
    return [System.IO.Path]::GetFullPath((Join-Path $scriptRoot $RelativePath))
}

function Get-FirstEnvOrExistingPath {
    param(
        [Parameter(Mandatory = $true)][string[]]$EnvironmentVariables,
        [Parameter(Mandatory = $true)][string[]]$FallbackPaths
    )

    foreach ($name in $EnvironmentVariables) {
        $value = [System.Environment]::GetEnvironmentVariable($name)
        if (-not [string]::IsNullOrWhiteSpace($value)) {
            return $value
        }
    }

    foreach ($candidate in $FallbackPaths) {
        if (Test-Path $candidate) {
            return $candidate
        }
    }

    return ""
}

function Ensure-Directory {
    param([Parameter(Mandatory = $true)][string]$Path)
    if (-not (Test-Path $Path)) {
        New-Item -Path $Path -ItemType Directory -Force | Out-Null
    }
}

function Invoke-InDirectory {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][scriptblock]$Action,
        [string]$MissingPathMessage = ""
    )

    if (-not (Test-Path $Path)) {
        if ([string]::IsNullOrWhiteSpace($MissingPathMessage)) {
            Write-Host "Directory `"$Path`" does not exist."
        }
        else {
            Write-Host $MissingPathMessage
        }
        return 1
    }

    Push-Location $Path
    try {
        & $Action | Out-Host
        return $LASTEXITCODE
    }
    finally {
        Pop-Location
    }
}

# SVN preparation
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
        Write-Host "ERROR: Could not find src\util\version.h."
        return ""
    }

    $versionMatch = Select-String -Path $versionHeaderPath -Pattern '^\s*#define\s+CYCLES_BLENDER_LIBRARIES_VERSION\s+(\S+)'
    if (-not $versionMatch -or -not $versionMatch.Matches -or $versionMatch.Matches.Count -eq 0) {
        Write-Host "ERROR: Could not determine CYCLES_BLENDER_LIBRARIES_VERSION from src\util\version.h."
        return ""
    }

    return $versionMatch.Matches[0].Groups[1].Value.Trim('"')
}

function Sync-RequiredSvnLib {
    param(
        [Parameter(Mandatory = $true)][string]$LibName,
        [Parameter(Mandatory = $true)][string]$LibRoot,
        [Parameter(Mandatory = $true)][string]$SvnLibBaseUrl,
        [Parameter(Mandatory = $true)][int]$SvnTimeoutSeconds,
        [Parameter(Mandatory = $true)][int]$SvnRetryDelaySeconds
    )

    $attempt = 0
    while ($true) {
        $attempt += 1
        $libPath = Join-Path $LibRoot $LibName
        $libUrl = "$SvnLibBaseUrl/$LibName"

        Write-Host "[SVN] Ensuring $LibName (attempt $attempt)"

        if ((Test-Path $libPath) -and (Test-RequiredLibContent -LibPath $libPath)) {
            Write-Host "[SVN] $LibName already has required content."
            return 0
        }

        if (Test-Path (Join-Path $libPath ".svn")) {
            & svn --non-interactive --config-option "servers:global:http-timeout=$SvnTimeoutSeconds" cleanup $libPath *> $null
            & svn --non-interactive --config-option "servers:global:http-timeout=$SvnTimeoutSeconds" update $libPath
        }
        else {
            Ensure-Directory -Path $LibRoot
            & svn --non-interactive --config-option "servers:global:http-timeout=$SvnTimeoutSeconds" checkout --force $libUrl $libPath
        }

        if ($LASTEXITCODE -ne 0) {
            Write-Host "[SVN] $LibName sync failed. Retrying in $SvnRetryDelaySeconds seconds..."
            Start-Sleep -Seconds $SvnRetryDelaySeconds
            continue
        }

        if (-not (Test-RequiredLibContent -LibPath $libPath)) {
            Write-Host "[SVN] $LibName appears incomplete. Retrying in $SvnRetryDelaySeconds seconds..."
            Start-Sleep -Seconds $SvnRetryDelaySeconds
            continue
        }

        Write-Host "[SVN] $LibName ready."
        return 0
    }
}

function Ensure-RequiredSvnLibs {
    param(
        [Parameter(Mandatory = $true)][int]$SvnTimeoutSeconds,
        [Parameter(Mandatory = $true)][int]$SvnRetryDelaySeconds
    )

    if (-not (Get-Command svn -ErrorAction SilentlyContinue)) {
        Write-Host "ERROR: svn was not found on PATH."
        return 1
    }

    $cyclesLibVersion = Get-CyclesLibrariesVersion
    if ([string]::IsNullOrWhiteSpace($cyclesLibVersion)) {
        return 1
    }

    $svnLibBaseUrl = "https://svn.blender.org/svnroot/bf-blender/tags/blender-$cyclesLibVersion-release/lib"
    $libRoot = Get-FullPathFromScript "..\lib"
    Write-Host "Ensuring required SVN libraries in `"$libRoot`"..."

    $linuxResult = Sync-RequiredSvnLib -LibName "linux_x86_64_glibc_228" -LibRoot $libRoot -SvnLibBaseUrl $svnLibBaseUrl -SvnTimeoutSeconds $SvnTimeoutSeconds -SvnRetryDelaySeconds $SvnRetryDelaySeconds
    if ($linuxResult -ne 0) { return $linuxResult }

    $winResult = Sync-RequiredSvnLib -LibName "win64_vc15" -LibRoot $libRoot -SvnLibBaseUrl $svnLibBaseUrl -SvnTimeoutSeconds $SvnTimeoutSeconds -SvnRetryDelaySeconds $SvnRetryDelaySeconds
    if ($winResult -ne 0) { return $winResult }

    Write-Host "Required SVN libraries are ready."
    return 0
}

function Remove-DirectoryQuick {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [int]$TimeoutSeconds = 20
    )

    $result = [PSCustomObject]@{
        Success      = $false
        TimedOut     = $false
        ExitCode     = $null
        ErrorMessage = ""
    }

    if (-not (Test-Path $Path)) {
        $result.Success = $true
        return $result
    }

    try {
        $proc = Start-Process -FilePath "cmd.exe" -ArgumentList "/d /c rd /s /q `"$Path`"" -NoNewWindow -PassThru
        if (-not $proc.WaitForExit($TimeoutSeconds * 1000)) {
            try { $proc.Kill() } catch { }
            $result.TimedOut = $true
            $result.ErrorMessage = "Timed out after $TimeoutSeconds seconds."
            return $result
        }
        $result.ExitCode = $proc.ExitCode
    }
    catch {
        $result.ErrorMessage = $_.Exception.Message
        return $result
    }

    if (-not (Test-Path $Path)) {
        $result.Success = $true
    }
    else {
        $result.ErrorMessage = "Directory still exists after rd /s /q."
    }

    return $result
}

function Get-DirectoryStats {
    param([Parameter(Mandatory = $true)][string]$Path)

    $files = 0
    $directories = 0

    try {
        Get-ChildItem -LiteralPath $Path -Recurse -Force -ErrorAction SilentlyContinue | ForEach-Object {
            if ($_.PSIsContainer) {
                $directories += 1
            }
            else {
                $files += 1
            }
        }
    }
    catch {
    }

    return [PSCustomObject]@{
        Files       = $files
        Directories = $directories
    }
}

function Remove-DirectoryItemByItem {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [int]$MaxErrors = 8
    )

    $errors = New-Object System.Collections.Generic.List[string]

    try {
        $items = Get-ChildItem -LiteralPath $Path -Force -ErrorAction SilentlyContinue
    }
    catch {
        $errors.Add("Failed to enumerate '$Path': $($_.Exception.Message)")
        return $errors.ToArray()
    }

    foreach ($item in $items) {
        try {
            Remove-Item -LiteralPath $item.FullName -Recurse -Force -ErrorAction Stop
        }
        catch {
            if ($errors.Count -lt $MaxErrors) {
                $errors.Add("$($item.FullName): $($_.Exception.Message)")
            }
        }
    }

    try {
        Remove-Item -LiteralPath $Path -Recurse -Force -ErrorAction Stop
    }
    catch {
    }

    return $errors.ToArray()
}

function Clear-BuildFoldersAfterSvn {
    $pathsToClean = @(
        (Get-FullPathFromScript "build"),
        (Get-FullPathFromScript "install")
    )

    foreach ($pathToClean in $pathsToClean) {
        if (-not (Test-Path $pathToClean)) {
            Write-Host "Cleanup skipped; folder not found: `"$pathToClean`""
            continue
        }

        Write-Host "Cleaning `"$pathToClean`"..."
        $quickResult = Remove-DirectoryQuick -Path $pathToClean
        if ($quickResult.Success) {
            continue
        }

        $stats = Get-DirectoryStats -Path $pathToClean
        Write-Host "WARNING: Fast cleanup failed for `"$pathToClean`". $($quickResult.ErrorMessage)"
        Write-Host "WARNING: Remaining entries: $($stats.Files) files, $($stats.Directories) directories."

        $deleteErrors = Remove-DirectoryItemByItem -Path $pathToClean
        if (-not (Test-Path $pathToClean)) {
            Write-Host "Cleanup succeeded after detailed retry."
            continue
        }

        if ($deleteErrors.Count -gt 0) {
            Write-Host "WARNING: Sample cleanup blockers:"
            foreach ($deleteError in $deleteErrors) {
                Write-Host "  $deleteError"
            }
        }

        # Final fallback: keep build moving even when files are locked.
        $stalePath = "$pathToClean.stale.$([DateTime]::Now.ToString('yyyyMMdd_HHmmss'))"
        try {
            Rename-Item -Path $pathToClean -NewName (Split-Path -Leaf $stalePath) -ErrorAction Stop
            Write-Host "WARNING: Could not delete `"$pathToClean`". Renamed to `"$stalePath`"."
        }
        catch {
            Write-Host "WARNING: Could not clean `"$pathToClean`". Continuing."
        }
    }

    return 0
}

# Artifact copy
function Get-ArtifactPaths {
    $rhinoCyclesRoot = Get-FullPathFromScript "..\..\..\..\..\..\big_libs\RhinoCycles"
    $dllDest = Join-Path $rhinoCyclesRoot "ccycles\win\release"

    return @{
        InstallDir          = Get-FullPathFromScript "install"
        DllDest             = $dllDest
        KernelDestinations  = @(
            (Join-Path $dllDest "lib"),
            (Join-Path $rhinoCyclesRoot "lib")
        )
    }
}

function Get-PrimaryBinaryFileNames {
    return @(
        "ccycles.dll",
        "cycles_kernel_oneapi_jit.dll",
        "sycl6.dll",
        "pi_level_zero.dll",
        "xptifw.dll",
        "ze_loader.dll"
    )
}

function Copy-PrimaryBinaries {
    param(
        [Parameter(Mandatory = $true)][string]$InstallDir,
        [Parameter(Mandatory = $true)][string]$DllDest
    )

    $filesToCopy = Get-PrimaryBinaryFileNames

    $copiedCount = 0
    $missingCount = 0
    $failedCount = 0

    foreach ($file in $filesToCopy) {
        $source = Join-Path $InstallDir $file
        if (Test-Path $source) {
            if (Copy-FileWithRetry -SourceFile $source -DestinationDirectory $DllDest) {
                $copiedCount += 1
            }
            else {
                $failedCount += 1
            }
        }
        else {
            Write-Host "WARNING: Missing `"$source`""
            $missingCount += 1
        }
    }

    Write-Host "Primary binary copy summary: copied $copiedCount, missing $missingCount, failed $failedCount."
}

function Sync-PrimaryBinariesIfStale {
    param(
        [Parameter(Mandatory = $true)][string]$InstallDir,
        [Parameter(Mandatory = $true)][string]$DllDest
    )

    $filesToSync = Get-PrimaryBinaryFileNames
    $upToDateCount = 0
    $copiedCount = 0
    $missingCount = 0
    $failedCount = 0

    foreach ($file in $filesToSync) {
        $source = Join-Path $InstallDir $file
        $destination = Join-Path $DllDest $file

        if (-not (Test-Path $source)) {
            Write-Host "WARNING: Missing `"$source`""
            $missingCount += 1
            continue
        }

        $needsCopy = $true
        if (Test-Path $destination) {
            $sourceInfo = Get-Item -LiteralPath $source
            $destinationInfo = Get-Item -LiteralPath $destination
            if (
                $sourceInfo.Length -eq $destinationInfo.Length -and
                $sourceInfo.LastWriteTimeUtc.Ticks -eq $destinationInfo.LastWriteTimeUtc.Ticks
            ) {
                $needsCopy = $false
            }
        }

        if (-not $needsCopy) {
            $upToDateCount += 1
            continue
        }

        if (Copy-FileWithRetry -SourceFile $source -DestinationDirectory $DllDest -MaxAttempts 25 -RetryDelaySeconds 2) {
            $copiedCount += 1
        }
        else {
            $failedCount += 1
        }
    }

    Write-Host "Primary binary final sync summary: up-to-date $upToDateCount, copied $copiedCount, missing $missingCount, failed $failedCount."
}

function Copy-FileWithRetry {
    param(
        [Parameter(Mandatory = $true)][string]$SourceFile,
        [Parameter(Mandatory = $true)][string]$DestinationDirectory,
        [int]$MaxAttempts = 5,
        [int]$RetryDelaySeconds = 2
    )

    $destinationFile = Join-Path $DestinationDirectory (Split-Path -Leaf $SourceFile)

    for ($attempt = 1; $attempt -le $MaxAttempts; $attempt++) {
        try {
            Copy-Item -LiteralPath $SourceFile -Destination $DestinationDirectory -Force -ErrorAction Stop
            $copiedItem = Get-Item -LiteralPath $destinationFile -ErrorAction Stop
            Write-Host "Copied `"$SourceFile`" -> `"$destinationFile`" ($($copiedItem.LastWriteTime.ToString('yyyy-MM-dd HH:mm:ss')))."
            return $true
        }
        catch {
            if ($attempt -lt $MaxAttempts) {
                Write-Host "WARNING: Copy attempt $attempt/$MaxAttempts failed for `"$destinationFile`". Retrying in $RetryDelaySeconds seconds."
                Start-Sleep -Seconds $RetryDelaySeconds
            }
            else {
                Write-Host "WARNING: Failed to copy `"$SourceFile`" to `"$destinationFile`" after $MaxAttempts attempts."
                Write-Host "WARNING: $($_.Exception.Message)"
            }
        }
    }

    return $false
}

function Copy-KernelArtifacts {
    param(
        [Parameter(Mandatory = $true)][string]$InstallDir,
        [Parameter(Mandatory = $true)][string[]]$Destinations
    )

    $installLibDir = Join-Path $InstallDir "lib"
    if (-not (Test-Path $installLibDir)) {
        Write-Host "WARNING: Missing `"$installLibDir`""
        return $false
    }

    $libEntries = Get-ChildItem -Path $installLibDir -Force -ErrorAction SilentlyContinue
    if (-not $libEntries) {
        return $true
    }

    foreach ($destination in $Destinations) {
        Ensure-Directory -Path $destination
        Copy-Item -Path (Join-Path $installLibDir "*") -Destination $destination -Force
    }

    return $true
}

function Invoke-VersionInfoUpdate {
    $versionScriptDir = Get-FullPathFromScript ".."
    Push-Location $versionScriptDir
    try {
        & powershell -NoProfile -ExecutionPolicy Bypass -File ".\versioninfo_changer.ps1"
        return $LASTEXITCODE
    }
    finally {
        Pop-Location
    }
}

function Invoke-PostBuildCopy {
    param([Parameter(Mandatory = $true)][hashtable]$ArtifactPaths)

    Write-Host "Command successful. Updating version info..."
    $versionInfoError = Invoke-VersionInfoUpdate
    if ($versionInfoError -ne 0) {
        Write-Host "versioninfo_changer.ps1 failed with error code $versionInfoError. Skipping copy."
        return
    }

    Write-Host "Copying files..."
    Ensure-Directory -Path $ArtifactPaths.DllDest
    foreach ($destination in $ArtifactPaths.KernelDestinations) {
        Ensure-Directory -Path $destination
    }

    Copy-PrimaryBinaries -InstallDir $ArtifactPaths.InstallDir -DllDest $ArtifactPaths.DllDest
    [void](Copy-KernelArtifacts -InstallDir $ArtifactPaths.InstallDir -Destinations $ArtifactPaths.KernelDestinations)
}

# Build option composition
function Get-DeviceBuildArguments {
    param(
        [AllowEmptyString()][string]$Arg1Value = "",
        [AllowEmptyString()][string]$Arg2Value = ""
    )

    $arg1Lower = ([string]$Arg1Value).ToLowerInvariant()
    $arg2Lower = ([string]$Arg2Value).ToLowerInvariant()

    $legacyCudaBinEnabled = $false
    if ($arg1Lower -eq "legacycudabin") {
        $arg1Lower = "cudabin"
        $legacyCudaBinEnabled = $true
    }
    if ($arg2Lower -eq "legacycuda" -or $arg2Lower -eq "legacycudabin") {
        $legacyCudaBinEnabled = $true
    }

    $deviceArgs = @()
    if ($arg1Lower -eq "cuda") {
        $deviceArgs += @(
            "-DWITH_CYCLES_CUDA_BINARIES=ON",
            "-DWITH_CYCLES_DEVICE_OPTIX=ON",
            "-DCYCLES_CUDA_BINARIES_ARCH=compute_52",
            "-DOPTIX_ROOT_DIR=$($script:Config.OptixRoot)"
        )
    }

    if ($arg1Lower -eq "cudabin" -or $arg1Lower -eq "all") {
        $deviceArgs += @(
            "-DWITH_CYCLES_CUDA_BINARIES=ON",
            "-DWITH_CYCLES_DEVICE_OPTIX=ON",
            "-DCYCLES_CUDA_BINARIES_ARCH=sm_37;sm_50;sm_52;sm_60;sm_61;sm_70;sm_75;sm_86;compute_75",
            "-DOPTIX_ROOT_DIR=$($script:Config.OptixRoot)"
        )
    }

    $cuda10NvccPath = Get-FirstEnvOrExistingPath -EnvironmentVariables @("CYCLES_CUDA10_NVCC_EXECUTABLE", "CUDA10_NVCC_EXECUTABLE") -FallbackPaths @(
        "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.2\bin\nvcc.exe",
        "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.1\bin\nvcc.exe"
    )
    $cuda10ToolkitPath = Get-FirstEnvOrExistingPath -EnvironmentVariables @("CYCLES_CUDA10_TOOLKIT_ROOT_DIR", "CUDA10_TOOLKIT_ROOT_DIR") -FallbackPaths @(
        "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.2",
        "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.1"
    )
    $cuda11NvccPath = Get-FirstEnvOrExistingPath -EnvironmentVariables @("CYCLES_CUDA11_NVCC_EXECUTABLE", "CUDA11_NVCC_EXECUTABLE") -FallbackPaths @(
        "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.8\bin\nvcc.exe",
        "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.7\bin\nvcc.exe"
    )
    $cuda11ToolkitPath = Get-FirstEnvOrExistingPath -EnvironmentVariables @("CYCLES_CUDA11_TOOLKIT_ROOT_DIR", "CUDA11_TOOLKIT_ROOT_DIR") -FallbackPaths @(
        "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.8",
        "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.7"
    )

    $legacyForThisBuild = ($arg1Lower -eq "cudabin" -or $arg1Lower -eq "all")
    if ($legacyCudaBinEnabled -and $legacyForThisBuild) {
        if ([string]::IsNullOrWhiteSpace($cuda10NvccPath)) {
            Write-Host "WARNING: CUDA10 nvcc not detected. sm_30 binaries require CUDA 10 or earlier and will be skipped."
        }
        if ([string]::IsNullOrWhiteSpace($cuda11NvccPath)) {
            Write-Host "WARNING: CUDA11 nvcc not detected. sm_35/sm_37 binaries require CUDA 11 or earlier and may be skipped."
        }

        $legacyArgs = @(
            "-DWITH_CYCLES_CUDA_BINARIES=ON",
            "-DWITH_CYCLES_DEVICE_OPTIX=ON",
            "-DCYCLES_CUDA_BINARIES_ARCH=sm_30;sm_35;sm_37;sm_50;sm_52;sm_60;sm_61;sm_70;sm_75;sm_86;compute_75",
            "-DOPTIX_ROOT_DIR=$($script:Config.OptixRoot)"
        )

        if (-not [string]::IsNullOrWhiteSpace($cuda10NvccPath)) { $legacyArgs += "-DCUDA10_NVCC_EXECUTABLE=$cuda10NvccPath" }
        if (-not [string]::IsNullOrWhiteSpace($cuda10ToolkitPath)) { $legacyArgs += "-DCUDA10_TOOLKIT_ROOT_DIR=$cuda10ToolkitPath" }
        if (-not [string]::IsNullOrWhiteSpace($cuda11NvccPath)) { $legacyArgs += "-DCUDA11_NVCC_EXECUTABLE=$cuda11NvccPath" }
        if (-not [string]::IsNullOrWhiteSpace($cuda11ToolkitPath)) { $legacyArgs += "-DCUDA11_TOOLKIT_ROOT_DIR=$cuda11ToolkitPath" }

        $deviceArgs += $legacyArgs
    }

    if ($legacyCudaBinEnabled -and -not $legacyForThisBuild) {
        Write-Host "WARNING: legacycuda flag ignored for mode `"$arg1Lower`". Use it with cudabin or all."
    }
    if ($legacyCudaBinEnabled -and $arg1Lower -eq "cuda") {
        Write-Host "WARNING: legacycuda only affects cubin builds, not ptx-only cuda mode."
    }
    if ($arg2Lower -eq "legacycudabin") {
        Write-Host "WARNING: third argument `"legacycudabin`" is deprecated. Use `"legacycuda`"."
    }

    $buildOneApi = ($arg1Lower -eq "oneapi" -or $arg1Lower -eq "oneapiaot" -or $arg1Lower -eq "all")
    if ($buildOneApi) {
        $deviceArgs += @(
            "-DWITH_CYCLES_DEVICE_ONEAPI=ON",
            "-DSYCL_ROOT_DIR=$($script:Config.DpcppRoot)",
            "-DLEVEL_ZERO_ROOT_DIR=$($script:Config.LevelZeroRoot)",
            "-DMSVC_REDIST_DIR=$($script:Config.MsvcRedistDir)",
            "-DWINDOWS_KITS_DIR=$($script:Config.WindowsKitsDir)"
        )

        if ($arg1Lower -eq "oneapiaot") {
            $deviceArgs += @(
                "-DWITH_CYCLES_ONEAPI_BINARIES=ON",
                "-DCYCLES_ONEAPI_SPIR64_GEN_DEVICES=dg2"
            )

            $defaultOclocPath = "..\lib\win64_vc15\dpcpp\lib\ocloc"
            if (Test-Path (Get-FullPathFromScript $defaultOclocPath)) {
                $deviceArgs += "-DOCLOC_INSTALL_DIR=$defaultOclocPath"
            }
            elseif (-not [string]::IsNullOrWhiteSpace($env:OCLOC_INSTALL_DIR)) {
                $deviceArgs += "-DOCLOC_INSTALL_DIR=$($env:OCLOC_INSTALL_DIR)"
            }
            else {
                Write-Host "WARNING: oneapiaot requested but ocloc was not found in ..\lib\win64_vc15\dpcpp\lib\ocloc"
                Write-Host "WARNING: AOT oneAPI binaries will be disabled unless OCLOC_INSTALL_DIR is provided."
            }
        }
    }

    if ($arg1Lower -eq "hip" -or $arg1Lower -eq "all") {
        $deviceArgs += "-DWITH_CYCLES_DEVICE_HIP=ON"
    }

    $generatorArgs = @()
    if ($buildOneApi) {
        $generatorArgs = @("-G", "Visual Studio 16 2019", "-A", "x64")
    }

    return @{
        GeneratorArgs = $generatorArgs
        DeviceArgs    = $deviceArgs
    }
}

# Command execution
function Invoke-ConfigureAndBuild {
    param(
        [Parameter(Mandatory = $true)][string]$BuildConfig,
        [Parameter(Mandatory = $true)][hashtable]$BuildOptions
    )

    $cmakeArgs = @(
        "-B", $buildDir
    ) + $BuildOptions.GeneratorArgs + @(
        "-DWITH_CYCLES_ALEMBIC=OFF",
        "-DWITH_CYCLES_USD=OFF",
        "-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF",
        "-DWITH_CYCLES_CUDA_BINARIES=OFF",
        "-DWITH_CYCLES_DEVICE_OPTIX=OFF"
    ) + $BuildOptions.DeviceArgs

    & $script:Config.CMakeExe @cmakeArgs | Out-Host
    if ($LASTEXITCODE -ne 0) {
        return $LASTEXITCODE
    }

    if (-not (Test-Path $buildDir)) {
        Write-Host "Build directory `"$buildDir`" was not created."
        return 1
    }

    return (Invoke-InDirectory -Path $buildDir -MissingPathMessage "Build directory `"$buildDir`" was not created." -Action {
            & $script:Config.CMakeExe --build . --target install --config $BuildConfig
        })
}

function Invoke-MainCommand {
    param(
        [Parameter(Mandatory = $true)][string]$CommandName,
        [Parameter(Mandatory = $true)][hashtable]$BuildOptions
    )

    $effectiveCommand = if ([string]::IsNullOrWhiteSpace($CommandName)) { "release" } else { $CommandName }
    $commandLower = $effectiveCommand.ToLowerInvariant()

    switch ($commandLower) {
        "release" { return (Invoke-ConfigureAndBuild -BuildConfig "RelWithDebInfo" -BuildOptions $BuildOptions) }
        "debug"   { return (Invoke-ConfigureAndBuild -BuildConfig "Debug" -BuildOptions $BuildOptions) }
        "clean"   {
            return (Invoke-InDirectory -Path $buildDir -MissingPathMessage "Build directory `"$buildDir`" does not exist." -Action {
                    & $script:Config.CMakeExe --build . --target install --config Clean
                })
        }
        "test"    {
            return (Invoke-InDirectory -Path $buildDir -MissingPathMessage "Build directory `"$buildDir`" does not exist." -Action {
                    & ctest --config Release
                })
        }
        "update"  {
            & $script:Config.PythonExe (Get-FullPathFromScript "src/cmake/make_update.py") | Out-Host
            return $LASTEXITCODE
        }
        "format"  {
            & $script:Config.PythonExe (Get-FullPathFromScript "src/cmake/make_format.py") | Out-Host
            return $LASTEXITCODE
        }
        default {
            Write-Host "Command `"$effectiveCommand`" unknown"
            return 1
        }
    }
}

function Test-DockerServerReady {
    & docker version *> $null
    return ($LASTEXITCODE -eq 0)
}

function Wait-DockerServerReady {
    param(
        [Parameter(Mandatory = $true)][int]$TimeoutSeconds,
        [Parameter(Mandatory = $true)][int]$PollSeconds
    )

    $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
    while ((Get-Date) -lt $deadline) {
        if (Test-DockerServerReady) {
            return $true
        }
        Start-Sleep -Seconds $PollSeconds
    }

    return $false
}

function Ensure-DockerReadyForHipFlow {
    param(
        [int]$StartupTimeoutSeconds = 120,
        [int]$PollSeconds = 3
    )

    if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
        Write-Host "Docker CLI was not found on PATH. Skipping Docker HIP step."
        Write-Host "Install Docker Desktop or rerun with -SkipDocker."
        return $false
    }

    if (Test-DockerServerReady) {
        return $true
    }

    $dockerContext = ""
    try {
        $dockerContext = (& docker context show 2>$null)
    }
    catch {
        $dockerContext = ""
    }

    if ([string]::IsNullOrWhiteSpace($dockerContext)) {
        Write-Host "Docker daemon is not reachable."
    }
    else {
        Write-Host "Docker daemon is not reachable for context `"$dockerContext`"."
    }

    Write-Host "Attempting to start Docker Desktop..."
    & docker desktop start | Out-Host
    if ($LASTEXITCODE -ne 0) {
        Write-Host "Unable to start Docker Desktop automatically. Start Docker Desktop and rerun, or use -SkipDocker."
        return $false
    }

    if (-not (Wait-DockerServerReady -TimeoutSeconds $StartupTimeoutSeconds -PollSeconds $PollSeconds)) {
        Write-Host "Docker Desktop did not become ready within $StartupTimeoutSeconds seconds."
        Write-Host "Start Docker Desktop manually and rerun, or use -SkipDocker."
        return $false
    }

    Write-Host "Docker daemon is ready."
    return $true
}

function Invoke-DockerHipFlow {
    param([Parameter(Mandatory = $true)][hashtable]$ArtifactPaths)

    Push-Location $scriptRoot
    try {
        Write-Host "Running Docker HIP image build..."
        & docker build -f DockerfileHIP -t ubuntu-cycles-hip-build . | Out-Host
        $dockerBuildError = $LASTEXITCODE
        if ($dockerBuildError -ne 0) {
            Write-Host "Docker build failed with error code $dockerBuildError."
            return $dockerBuildError
        }

        Write-Host "Docker build successful."
        Write-Host "Running Docker HIP container..."
        $useInteractiveRun = $false
        try {
            $useInteractiveRun = (-not [Console]::IsInputRedirected) -and (-not [Console]::IsOutputRedirected)
        }
        catch {
            $useInteractiveRun = $false
        }

        $dockerRunArgs = @("run")
        if ($useInteractiveRun) {
            $dockerRunArgs += "-it"
        }
        $dockerRunArgs += @("-v", $script:Config.DockerVolume, "ubuntu-cycles-hip-build")

        & docker @dockerRunArgs | Out-Host
        $dockerRunError = $LASTEXITCODE
        if ($dockerRunError -ne 0) {
            Write-Host "Docker run failed with error code $dockerRunError."
            return $dockerRunError
        }

        Write-Host "Docker run successful."
        Write-Host "Copying Docker-generated kernel artifacts..."
        if (Copy-KernelArtifacts -InstallDir $ArtifactPaths.InstallDir -Destinations $ArtifactPaths.KernelDestinations) {
            Write-Host "Kernel artifact copy successful."
        }
        else {
            Write-Host "No kernel artifacts copied."
        }

        return 0
    }
    finally {
        Pop-Location
    }
}

# Main flow
$svnTimeoutSeconds = Get-EnvInt -Name "CYCLES_SVN_TIMEOUT_SECONDS" -DefaultValue 120
$svnRetryDelaySeconds = Get-EnvInt -Name "CYCLES_SVN_RETRY_DELAY_SECONDS" -DefaultValue 15

$ensureResult = Ensure-RequiredSvnLibs -SvnTimeoutSeconds $svnTimeoutSeconds -SvnRetryDelaySeconds $svnRetryDelaySeconds
if ($ensureResult -ne 0) {
    Write-Host "Failed to prepare required SVN libraries. Exiting."
    exit $ensureResult
}

$cleanupResult = Clear-BuildFoldersAfterSvn
if ($cleanupResult -ne 0) {
    Write-Host "Failed to clean build folders after SVN update. Exiting."
    exit $cleanupResult
}

$buildOptions = Get-DeviceBuildArguments -Arg1Value $Arg1 -Arg2Value $Arg2
$commandExitCode = Invoke-MainCommand -CommandName $Command -BuildOptions $buildOptions

$artifactPaths = Get-ArtifactPaths
if ($commandExitCode -eq 0) {
    Invoke-PostBuildCopy -ArtifactPaths $artifactPaths
}
else {
    Write-Host "Command failed with error code $commandExitCode. Skipping copy."
}

$finalExitCode = $commandExitCode
if ($SkipDocker) {
    Write-Host "Skipping Docker HIP step."
}
else {
    if (-not (Ensure-DockerReadyForHipFlow)) {
        Write-Host "Skipping Docker HIP step."
    }
    else {
        $dockerExitCode = Invoke-DockerHipFlow -ArtifactPaths $artifactPaths
        if ($dockerExitCode -ne 0) {
            Write-Host "Docker step failed with error code $dockerExitCode, continuing with command exit code $commandExitCode."
        }
    }
}

if ($commandExitCode -eq 0) {
    Write-Host "Running final primary binary sync verification..."
    Sync-PrimaryBinariesIfStale -InstallDir $artifactPaths.InstallDir -DllDest $artifactPaths.DllDest
}

exit $finalExitCode
