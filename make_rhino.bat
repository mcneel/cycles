@echo off

REM Convenience wrapper for CMake commands

setlocal enableextensions enabledelayedexpansion

if "%BUILD_DIR%" == "" set BUILD_DIR=build
set PYTHON=python
set COMMAND=%1
set ARG1=%2
set ARG2=%3
set CMAKE_EXE=cmake
if exist C:\Tools\cmake329\bin\cmake.exe set CMAKE_EXE=C:\Tools\cmake329\bin\cmake.exe

set OPTIX="C:\ProgramData\NVIDIA Corporation\OptiX SDK 7.6.0"
set DPCPP_ROOT="..\lib\win64_vc15\dpcpp"
set LEVELZERO_ROOT="..\lib\win64_vc15\level-zero"
set MSVC_REDIST_DIR="C:/Program Files (x86)/Microsoft Visual Studio/2019/Professional/VC/Redist/MSVC/14.29.30133"
set WINDOWS_KITS_DIR="C:/Program Files (x86)/Windows Kits/10"
set "SVN_TIMEOUT_SECONDS=120"
if not "%CYCLES_SVN_TIMEOUT_SECONDS%" == "" set "SVN_TIMEOUT_SECONDS=%CYCLES_SVN_TIMEOUT_SECONDS%"
set "SVN_RETRY_DELAY_SECONDS=15"
if not "%CYCLES_SVN_RETRY_DELAY_SECONDS%" == "" set "SVN_RETRY_DELAY_SECONDS=%CYCLES_SVN_RETRY_DELAY_SECONDS%"

call :ensure_required_svn_libs
if !ERRORLEVEL! NEQ 0 (
	echo Failed to prepare required SVN libraries. Exiting.
	exit /b !ERRORLEVEL!
)

REM Supported build configs:
REM    debug          # Build with CPU-only support
REM    debug cuda     # Build .PTX files 
REM    debug cudabin  # Build all CUDA binaries
REM    debug legacycudabin # Legacy alias for cudabin + optional legacy CUDA archs
REM    debug oneapi   # Build with OneAPI support
REM    debug oneapiaot # Build with oneAPI AOT binaries (requires ocloc)
REM    debug hip      # Build with HIP support
REM    debug all      # Build with support for all devices (+ .PTX files and all CUDA binaries)
REM Optional 3rd argument:
REM    legacycuda     # Include sm_30/sm_35/sm_37 in cudabin/all modes (requires CUDA 10/11 toolkits)

set LEGACY_CUDABIN_ENABLED=No
if /I "%ARG1%" == "legacycudabin" (
	set ARG1=cudabin
	set LEGACY_CUDABIN_ENABLED=Yes
)
if /I "%ARG2%" == "legacycuda" set LEGACY_CUDABIN_ENABLED=Yes
if /I "%ARG2%" == "legacycudabin" set LEGACY_CUDABIN_ENABLED=Yes

set BUILD_CUDA_CMD= ^
	-DWITH_CYCLES_CUDA_BINARIES=ON ^
	-DWITH_CYCLES_DEVICE_OPTIX=ON ^
	-DCYCLES_CUDA_BINARIES_ARCH="compute_52" ^
	-DOPTIX_ROOT_DIR=%OPTIX%

if not "%ARG1%" == "cuda" (
	set BUILD_CUDA_CMD=
)

set BUILD_CUDABIN_CMD= ^
	-DWITH_CYCLES_CUDA_BINARIES=ON ^
	-DWITH_CYCLES_DEVICE_OPTIX=ON ^
	-DCYCLES_CUDA_BINARIES_ARCH="sm_37;sm_50;sm_52;sm_60;sm_61;sm_70;sm_75;sm_86;compute_75" ^
	-DOPTIX_ROOT_DIR=%OPTIX%
	
if not "%ARG1%" == "cudabin" (
	if not "%ARG1%" == "all" (
		set BUILD_CUDABIN_CMD=
	)
)

set "CUDA10_NVCC_PATH=%CYCLES_CUDA10_NVCC_EXECUTABLE%"
if "%CUDA10_NVCC_PATH%" == "" set "CUDA10_NVCC_PATH=%CUDA10_NVCC_EXECUTABLE%"
if "%CUDA10_NVCC_PATH%" == "" if exist "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.2\bin\nvcc.exe" set "CUDA10_NVCC_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.2\bin\nvcc.exe"
if "%CUDA10_NVCC_PATH%" == "" if exist "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.1\bin\nvcc.exe" set "CUDA10_NVCC_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.1\bin\nvcc.exe"

set "CUDA10_TOOLKIT_PATH=%CYCLES_CUDA10_TOOLKIT_ROOT_DIR%"
if "%CUDA10_TOOLKIT_PATH%" == "" set "CUDA10_TOOLKIT_PATH=%CUDA10_TOOLKIT_ROOT_DIR%"
if "%CUDA10_TOOLKIT_PATH%" == "" if exist "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.2" set "CUDA10_TOOLKIT_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.2"
if "%CUDA10_TOOLKIT_PATH%" == "" if exist "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.1" set "CUDA10_TOOLKIT_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v10.1"

set "CUDA11_NVCC_PATH=%CYCLES_CUDA11_NVCC_EXECUTABLE%"
if "%CUDA11_NVCC_PATH%" == "" set "CUDA11_NVCC_PATH=%CUDA11_NVCC_EXECUTABLE%"
if "%CUDA11_NVCC_PATH%" == "" if exist "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.8\bin\nvcc.exe" set "CUDA11_NVCC_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.8\bin\nvcc.exe"
if "%CUDA11_NVCC_PATH%" == "" if exist "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.7\bin\nvcc.exe" set "CUDA11_NVCC_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.7\bin\nvcc.exe"

set "CUDA11_TOOLKIT_PATH=%CYCLES_CUDA11_TOOLKIT_ROOT_DIR%"
if "%CUDA11_TOOLKIT_PATH%" == "" set "CUDA11_TOOLKIT_PATH=%CUDA11_TOOLKIT_ROOT_DIR%"
if "%CUDA11_TOOLKIT_PATH%" == "" if exist "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.8" set "CUDA11_TOOLKIT_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.8"
if "%CUDA11_TOOLKIT_PATH%" == "" if exist "C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.7" set "CUDA11_TOOLKIT_PATH=C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA\v11.7"

set LEGACY_CUDA_TOOLKITS_CMD=
if not "%CUDA10_NVCC_PATH%" == "" (
	set LEGACY_CUDA_TOOLKITS_CMD=!LEGACY_CUDA_TOOLKITS_CMD! ^
	-DCUDA10_NVCC_EXECUTABLE="%CUDA10_NVCC_PATH%"
)
if not "%CUDA10_TOOLKIT_PATH%" == "" (
	set LEGACY_CUDA_TOOLKITS_CMD=!LEGACY_CUDA_TOOLKITS_CMD! ^
	-DCUDA10_TOOLKIT_ROOT_DIR="%CUDA10_TOOLKIT_PATH%"
)
if not "%CUDA11_NVCC_PATH%" == "" (
	set LEGACY_CUDA_TOOLKITS_CMD=!LEGACY_CUDA_TOOLKITS_CMD! ^
	-DCUDA11_NVCC_EXECUTABLE="%CUDA11_NVCC_PATH%"
)
if not "%CUDA11_TOOLKIT_PATH%" == "" (
	set LEGACY_CUDA_TOOLKITS_CMD=!LEGACY_CUDA_TOOLKITS_CMD! ^
	-DCUDA11_TOOLKIT_ROOT_DIR="%CUDA11_TOOLKIT_PATH%"
)

set BUILD_LEGACY_CUDABIN_CMD= ^
	-DWITH_CYCLES_CUDA_BINARIES=ON ^
	-DWITH_CYCLES_DEVICE_OPTIX=ON ^
	-DCYCLES_CUDA_BINARIES_ARCH="sm_30;sm_35;sm_37;sm_50;sm_52;sm_60;sm_61;sm_70;sm_75;sm_86;compute_75" ^
	-DOPTIX_ROOT_DIR=%OPTIX% ^
	!LEGACY_CUDA_TOOLKITS_CMD!

set LEGACY_FOR_THIS_BUILD=No
if /I "%ARG1%" == "cudabin" set LEGACY_FOR_THIS_BUILD=Yes
if /I "%ARG1%" == "all" set LEGACY_FOR_THIS_BUILD=Yes

if "%LEGACY_CUDABIN_ENABLED%" == "Yes" (
	if "%LEGACY_FOR_THIS_BUILD%" == "Yes" (
		if "%CUDA10_NVCC_PATH%" == "" (
			echo WARNING: CUDA10 nvcc not detected. sm_30 binaries require CUDA 10 or earlier and will be skipped.
		)
		if "%CUDA11_NVCC_PATH%" == "" (
			echo WARNING: CUDA11 nvcc not detected. sm_35/sm_37 binaries require CUDA 11 or earlier and may be skipped.
		)
	) else (
		set BUILD_LEGACY_CUDABIN_CMD=
	)
) else (
	set BUILD_LEGACY_CUDABIN_CMD=
)

if "%LEGACY_CUDABIN_ENABLED%" == "Yes" if "%LEGACY_FOR_THIS_BUILD%" == "No" (
	echo WARNING: legacycuda flag ignored for mode "%ARG1%". Use it with cudabin or all.
)
if "%LEGACY_CUDABIN_ENABLED%" == "Yes" if "%ARG1%" == "cuda" (
	echo WARNING: legacycuda only affects cubin builds, not ptx-only cuda mode.
)

if /I "%ARG2%" == "legacycudabin" (
	echo WARNING: third argument "legacycudabin" is deprecated. Use "legacycuda".
)

set BUILDING_ONEAPI=Yes
set BUILD_ONEAPI_CMD= ^
	-DWITH_CYCLES_DEVICE_ONEAPI=ON ^
	-DSYCL_ROOT_DIR=%DPCPP_ROOT% ^
	-DLEVEL_ZERO_ROOT_DIR=%LEVELZERO_ROOT% ^
	-DMSVC_REDIST_DIR=%MSVC_REDIST_DIR% ^
	-DWINDOWS_KITS_DIR=%WINDOWS_KITS_DIR%
set BUILD_ONEAPI_BINARIES_CMD=

if "%ARG1%" == "oneapiaot" (
	set BUILD_ONEAPI_BINARIES_CMD= ^
	-DWITH_CYCLES_ONEAPI_BINARIES=ON ^
	-DCYCLES_ONEAPI_SPIR64_GEN_DEVICES="dg2"

	if exist ..\lib\win64_vc15\dpcpp\lib\ocloc (
		set BUILD_ONEAPI_BINARIES_CMD=!BUILD_ONEAPI_BINARIES_CMD! ^
		-DOCLOC_INSTALL_DIR="..\lib\win64_vc15\dpcpp\lib\ocloc"
	) else (
		if not "%OCLOC_INSTALL_DIR%" == "" (
			set BUILD_ONEAPI_BINARIES_CMD=!BUILD_ONEAPI_BINARIES_CMD! ^
			-DOCLOC_INSTALL_DIR="%OCLOC_INSTALL_DIR%"
		) else (
			echo WARNING: oneapiaot requested but ocloc was not found in ..\lib\win64_vc15\dpcpp\lib\ocloc
			echo WARNING: AOT oneAPI binaries will be disabled unless OCLOC_INSTALL_DIR is provided.
		)
	)
)

if not "%ARG1%" == "oneapi" (
	if not "%ARG1%" == "oneapiaot" (
		if not "%ARG1%" == "all" (
			set BUILDING_ONEAPI=No
			set BUILD_ONEAPI_CMD=
		)
	)
)

set CMAKE_GENERATOR_CMD=
if "%BUILDING_ONEAPI%" == "Yes" (
	set CMAKE_GENERATOR_CMD= ^
	-G "Visual Studio 16 2019" ^
	-A x64
)

set BUILD_HIP_CMD= ^
	-DWITH_CYCLES_DEVICE_HIP=ON

if not "%ARG1%" == "hip" (
	if not "%ARG1%" == "all" (
		set BUILD_HIP_CMD=
	)
)

if "%COMMAND%" == "" (
	set COMMAND=release
)

if "%COMMAND%" == "release" (
	set BUILDING=Yes
	set CONFIG=RelWithDebInfo
) else if "%COMMAND%" == "debug" (
	set BUILDING=Yes
	set CONFIG=Debug
)

if "%BUILDING%" == "Yes" (
	"%CMAKE_EXE%" -B %BUILD_DIR% ^
	%CMAKE_GENERATOR_CMD% ^
	-DWITH_CYCLES_ALEMBIC=OFF ^
	-DWITH_CYCLES_USD=OFF ^
	-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF ^
	-DWITH_CYCLES_CUDA_BINARIES=OFF ^
	-DWITH_CYCLES_DEVICE_OPTIX=OFF ^
	%BUILD_CUDA_CMD% ^
	%BUILD_CUDABIN_CMD% ^
	%BUILD_LEGACY_CUDABIN_CMD% ^
	%BUILD_ONEAPI_CMD% ^
	%BUILD_ONEAPI_BINARIES_CMD% ^
	%BUILD_HIP_CMD% ^
	&& cd %BUILD_DIR% && "%CMAKE_EXE%" --build . --target install --config %CONFIG%
) else if "%COMMAND%" == "clean" (
	cd %BUILD_DIR% && "%CMAKE_EXE%" --build . --target install --config Clean
) else if "%COMMAND%" == "test" (
	cd %BUILD_DIR% && ctest --config Release
) else if "%COMMAND%" == "update" (
	%PYTHON% src/cmake/make_update.py
) else if "%COMMAND%" == "format" (
	%PYTHON% src/cmake/make_format.py
) else (
  echo Command "%COMMAND%" unknown
)

if %ERRORLEVEL% EQU 0 (
	echo Command successful. Updating version info...
	pushd "%~dp0.."
	powershell -NoProfile -ExecutionPolicy Bypass -File ".\versioninfo_changer.ps1"
	set "VERSIONINFO_ERROR=!ERRORLEVEL!"
	popd

	if not "!VERSIONINFO_ERROR!" == "0" (
		echo versioninfo_changer.ps1 failed with error code !VERSIONINFO_ERROR!. Skipping copy.
	) else (
		echo Copying files...

		for %%I in ("%~dp0install") do set "INSTALL_DIR=%%~fI"
		for %%I in ("%~dp0..\..\..\..\..\..\big_libs\RhinoCycles\ccycles\win\release") do set "DLL_DEST=%%~fI"
		set "KERNEL_DEST=!DLL_DEST!\lib"

		if not exist "!DLL_DEST!" mkdir "!DLL_DEST!"
		if not exist "!KERNEL_DEST!" mkdir "!KERNEL_DEST!"

		for %%F in (
			ccycles.dll
			cycles_kernel_oneapi_jit.dll
			sycl6.dll
			pi_level_zero.dll
			xptifw.dll
			ze_loader.dll
		) do (
			if exist "!INSTALL_DIR!\%%F" (
				copy /Y "!INSTALL_DIR!\%%F" "!DLL_DEST!\" >nul
			) else (
				echo WARNING: Missing "!INSTALL_DIR!\%%F"
			)
		)

		if exist "!INSTALL_DIR!\lib" (
			copy /Y "!INSTALL_DIR!\lib\*.*" "!KERNEL_DEST!\" >nul
		) else (
			echo WARNING: Missing "!INSTALL_DIR!\lib"
		)
	)
) else (
	echo Command failed with error code %ERRORLEVEL%. Skipping copy.
)

pushd "%~dp0"
echo Running Docker HIP image build...
docker build -f DockerfileHIP -t ubuntu-cycles-hip-build .
set "DOCKER_BUILD_ERROR=!ERRORLEVEL!"

if "!DOCKER_BUILD_ERROR!" == "0" (
	echo Docker build successful.
	echo Running Docker HIP container...
	docker run -it -v D:/dev/github/mcneel/rhino/8.x:/rhino/rhino-8.x ubuntu-cycles-hip-build
	set "DOCKER_RUN_ERROR=!ERRORLEVEL!"

	if "!DOCKER_RUN_ERROR!" == "0" (
		echo Docker run successful.
		echo Copying Docker-generated kernel artifacts...
		for %%I in ("%~dp0install") do set "INSTALL_DIR=%%~fI"
		for %%I in ("%~dp0..\..\..\..\..\..\big_libs\RhinoCycles\ccycles\win\release") do set "DLL_DEST=%%~fI"
		set "KERNEL_DEST=!DLL_DEST!\lib"
		if not exist "!KERNEL_DEST!" mkdir "!KERNEL_DEST!"

		if exist "!INSTALL_DIR!\lib" (
			copy /Y "!INSTALL_DIR!\lib\*.*" "!KERNEL_DEST!\" >nul
			echo Kernel artifact copy successful.
		) else (
			echo WARNING: Missing "!INSTALL_DIR!\lib". No kernel artifacts copied.
		)
	) else (
		echo Docker run failed with error code !DOCKER_RUN_ERROR!.
	)
) else (
	echo Docker build failed with error code !DOCKER_BUILD_ERROR!.
)
popd

goto :eof

:ensure_required_svn_libs
where svn >nul 2>&1
if !ERRORLEVEL! NEQ 0 (
	echo ERROR: svn was not found on PATH.
	exit /b 1
)

for /f "tokens=3" %%V in ('findstr /R /C:"^[ ]*#define[ ]*CYCLES_BLENDER_LIBRARIES_VERSION" "%~dp0src\util\version.h"') do set "CYCLES_LIB_VERSION=%%V"
if not defined CYCLES_LIB_VERSION (
	echo ERROR: Could not determine CYCLES_BLENDER_LIBRARIES_VERSION from src\util\version.h.
	exit /b 1
)

set "SVN_LIB_BASE_URL=https://svn.blender.org/svnroot/bf-blender/tags/blender-!CYCLES_LIB_VERSION!-release/lib"
for %%I in ("%~dp0..\lib") do set "LIB_ROOT=%%~fI"

echo Ensuring required SVN libraries in "!LIB_ROOT!"...
call :sync_required_svn_lib linux_x86_64_glibc_228
if !ERRORLEVEL! NEQ 0 exit /b !ERRORLEVEL!
call :sync_required_svn_lib win64_vc15
if !ERRORLEVEL! NEQ 0 exit /b !ERRORLEVEL!
echo Required SVN libraries are ready.
exit /b 0

:sync_required_svn_lib
set "LIB_NAME=%~1"
set /a LIB_ATTEMPT=0

:sync_required_svn_lib_retry
set /a LIB_ATTEMPT+=1
set "LIB_PATH=!LIB_ROOT!\!LIB_NAME!"
set "LIB_URL=!SVN_LIB_BASE_URL!/!LIB_NAME!"

echo [SVN] Ensuring !LIB_NAME! ^(attempt !LIB_ATTEMPT!^)

if exist "!LIB_PATH!" (
	call :has_required_lib_content "!LIB_PATH!"
	if !ERRORLEVEL! EQU 0 (
		echo [SVN] !LIB_NAME! already has required content.
		exit /b 0
	)
)

if exist "!LIB_PATH!\.svn" (
	svn --non-interactive --config-option servers:global:http-timeout=!SVN_TIMEOUT_SECONDS! cleanup "!LIB_PATH!" >nul 2>&1
	svn --non-interactive --config-option servers:global:http-timeout=!SVN_TIMEOUT_SECONDS! update "!LIB_PATH!"
) else (
	if not exist "!LIB_ROOT!" mkdir "!LIB_ROOT!" >nul 2>&1
	svn --non-interactive --config-option servers:global:http-timeout=!SVN_TIMEOUT_SECONDS! checkout --force "!LIB_URL!" "!LIB_PATH!"
)

if !ERRORLEVEL! NEQ 0 (
	echo [SVN] !LIB_NAME! sync failed. Retrying in !SVN_RETRY_DELAY_SECONDS! seconds...
	timeout /t !SVN_RETRY_DELAY_SECONDS! /nobreak >nul
	goto :sync_required_svn_lib_retry
)

call :has_required_lib_content "!LIB_PATH!"
if !ERRORLEVEL! NEQ 0 (
	echo [SVN] !LIB_NAME! appears incomplete. Retrying in !SVN_RETRY_DELAY_SECONDS! seconds...
	timeout /t !SVN_RETRY_DELAY_SECONDS! /nobreak >nul
	goto :sync_required_svn_lib_retry
)

echo [SVN] !LIB_NAME! ready.
exit /b 0

:has_required_lib_content
set "CHECK_LIB_PATH=%~1"
if not exist "!CHECK_LIB_PATH!\openimageio\include\OpenImageIO\imageio.h" exit /b 1
if not exist "!CHECK_LIB_PATH!\openimageio\lib" exit /b 1
if not exist "!CHECK_LIB_PATH!\openexr\include\OpenEXR\ImfVersion.h" exit /b 1
exit /b 0
