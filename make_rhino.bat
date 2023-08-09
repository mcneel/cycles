@echo off

REM Convenience wrapper for CMake commands

setlocal enableextensions enabledelayedexpansion

set BUILD_DIR=build
set PYTHON=python
set COMMAND=%1
set ARG1=%2

set OPTIX="C:\ProgramData\NVIDIA Corporation\OptiX SDK 7.3.0"
set LEVELZERO_INC="..\lib\win64_vc15\level-zero\include"
set LEVELZERO_LIB="..\lib\win64_vc15\level-zero\lib"
set MSVC_REDIST_DIR="C:/Program Files (x86)/Microsoft Visual Studio/2019/Professional/VC/Redist/MSVC/14.29.30133"
set WINDOWS_KITS_DIR="C:/Program Files (x86)/Windows Kits/10"

REM Supported build configs:
REM    debug          # Build with CPU-only support
REM    debug cuda     # Build .PTX files 
REM    debug cudabin  # Build all CUDA binaries
REM    debug oneapi   # Build with OneAPI support
REM    debug hip      # Build with HIP support
REM    debug all      # Build with support for all devices (+ .PTX files)
REM    debug allbin   # Build with support for all devices (+ all CUDA binaries)

set BUILD_CUDA_CMD= ^
	-DWITH_CYCLES_CUDA_BINARIES=ON ^
	-DWITH_CYCLES_DEVICE_OPTIX=ON ^
	-DCYCLES_CUDA_BINARIES_ARCH="compute_52" ^
	-DOPTIX_ROOT_DIR=%OPTIX%

if not "%ARG1%" == "cuda" (
	if not "%ARG1%" == "all" (
	  set BUILD_CUDA_CMD=""
	)
)

set BUILD_CUDABIN_CMD= ^
	-DWITH_CYCLES_CUDA_BINARIES=ON ^
	-DWITH_CYCLES_DEVICE_OPTIX=ON ^
	-DCYCLES_CUDA_BINARIES_ARCH="sm_37;sm_50;sm_52;sm_60;sm_61;sm_70;sm_75;sm_86;compute_75" ^
	-DOPTIX_ROOT_DIR=%OPTIX%
	
if not "%ARG1%" == "cudabin" (
	if not "%ARG1%" == "allbin" (
		set BUILD_CUDABIN_CMD=""
	)
)

set BUILDING_ONEAPI="Yes"
set BUILD_ONEAPI_CMD= ^
	-DWITH_CYCLES_DEVICE_ONEAPI=ON ^
	-D_LEVEL_ZERO_INCLUDE_DIR=%LEVELZERO_INC% ^
	-D_LEVEL_ZERO_LIBRARY=%LEVELZERO_LIB% ^
	-DMSVC_REDIST_DIR=%MSVC_REDIST_DIR% ^
	-DWINDOWS_KITS_DIR=%WINDOWS_KITS_DIR%

if not "%ARG1%" == "oneapi" (
	if not "%ARG1%" == "all" (
		set BUILDING_ONEAPI="No"
		set BUILD_ONEAPI_CMD=""
	)
)

set BUILD_HIP_CMD= ^
	-DWITH_CYCLES_DEVICE_HIP=ON

if not "%ARG1%" == "hip" (
	if not "%ARG1%" == "all" (
		set BUILD_HIP_CMD=""
	)
)

if "%COMMAND%" == "" (
	set COMMAND=release
)

if "%COMMAND%" == "release" (
	set BUILDING=Yes
	set CONFIG=Release
) else if "%COMMAND%" == "debug" (
	set BUILDING=Yes
	set CONFIG=Debug
)

if "%BUILDING%" == "Yes" (
	cmake -B %BUILD_DIR% ^
	-DWITH_CYCLES_ALEMBIC=OFF ^
	-DWITH_CYCLES_USD=OFF ^
	-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF ^
	-DWITH_CYCLES_CUDA_BINARIES=OFF ^
	-DWITH_CYCLES_DEVICE_OPTIX=ON ^
	%BUILD_CUDA_CMD% ^
	%BUILD_CUDABIN_CMD% ^
	%BUILD_ONEAPI_CMD% ^
	%BUILD_HIP_CMD% ^
	&& cd %BUILD_DIR% && cmake --build . --target install --config %CONFIG%
) else if "%COMMAND%" == "clean" (
	cd %BUILD_DIR% && cmake --build . --target install --config Clean
) else if "%COMMAND%" == "test" (
	cd %BUILD_DIR% && ctest --config Release
) else if "%COMMAND%" == "update" (
	%PYTHON% src/cmake/make_update.py
) else if "%COMMAND%" == "format" (
	%PYTHON% src/cmake/make_format.py
) else (
  echo Command "%COMMAND%" unknown
)

if %BUILDING_ONEAPI% == "Yes" (
  echo **************************************************************************************************************
  echo NOTE: If you got compile errors like "error C2338: DPCPP does not support C++ version earlier than C++17.",  *
  echo then do the following: Add the /Zc:__cplusplus compiler option to the cycles_device.vcxproj compiler options *
  echo to force the compiler to set the correct value to the macro.                                                 *
  echo **************************************************************************************************************
)
