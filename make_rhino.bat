@echo off

REM Convenience wrapper for CMake commands

setlocal enableextensions enabledelayedexpansion

set BUILD_DIR=build
set PYTHON=python
set COMMAND=%1
set ARG1=%2

set OPTIX="C:\optix76"
set LEVELZERO_INC="..\..\..\..\..\..\big_libs\RhinoCycles\ccycles\win\release\level-zero\include"
set LEVELZERO_LIB="..\..\..\..\..\..\big_libs\RhinoCycles\ccycles\win\release\level-zero\lib"
set MSVC_REDIST_DIR="C:/Program Files (x86)/Microsoft Visual Studio/2019/Professional/VC/Redist/MSVC/14.29.30133"
set MSVC_REDIST_DIR="C:/Program Files/Microsoft Visual Studio/2022/Professional/VC/Redist/MSVC/14.44.35112/x86/Microsoft.VC143.CRT/"
set MSVC_REDIST_DIR="C:/Program Files/Microsoft Visual Studio/2022/Professional/VC/Redist/MSVC/14.44.35112/"
set WINDOWS_KITS_DIR="C:/Program Files (x86)/Windows Kits/10"

REM Supported build configs:
REM    debug          # Build with CPU-only support
REM    debug cuda     # Build .PTX files
REM    debug cudabin  # Build all CUDA binaries
REM    debug oneapi   # Build with OneAPI support
REM    debug hip      # Build with HIP support
REM    debug all      # Build with support for all devices (+ .PTX files and all CUDA binaries)

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
	-DCYCLES_CUDA_BINARIES_ARCH="sm_50;sm_52;sm_60;sm_61;sm_70;sm_75;sm_86;sm_89;sm_90;sm_100;sm_101;compute_75" ^
	-DOPTIX_ROOT_DIR=%OPTIX%

if not "%ARG1%" == "cudabin" (
	if not "%ARG1%" == "all" (
		set BUILD_CUDABIN_CMD=""
	)
)

set BUILDING_ONEAPI="No"
set BUILD_ONEAPI_CMD= ^
	-DWITH_CYCLES_DEVICE_ONEAPI=OFF ^
	-DWITH_CYCLES_ONEAPI_BINARIES=OFF ^
	-DMSVC_REDIST_DIR=%MSVC_REDIST_DIR% ^
	-DLEVEL_ZERO_INCLUDE_DIR=%LEVELZERO_INC% ^
	-DLEVEL_ZERO_LIBRARY=%LEVELZERO_LIB% ^
	-DWINDOWS_KITS_DIR=%WINDOWS_KITS_DIR%

if not "%ARG1%" == "oneapi" (
	if not "%ARG1%" == "all" (
		set BUILDING_ONEAPI="No"
		set BUILD_ONEAPI_CMD=""
	)
)


REM HIPRT needs investigation - linking errors in kernel generation
set BUILD_HIP_CMD= ^
	-DWITH_CYCLES_DEVICE_HIP=ON ^
	-DWITH_CYCLES_HIP_BINARIES=ON ^
	-DWITH_CYCLES_DEVICE_HIPRT=OFF ^
	-DHIP_ROOT_DIR="C:/rocm/6.4" ^
	-DHIPRT_ROOT_DIR="C:/rocm/6.4" ^
	-DCYCLES_HIP_BINARIES_ARCH="gfx900;gfx906;gfx90c;gfx902;gfx1010;gfx1011;gfx1012;gfx1030;gfx1031;gfx1032;gfx1034;gfx1035;gfx1036;gfx1100;gfx1101;gfx1102;gfx1103;gfx1150;gfx1151;gfx1152;gfx1200;gfx1201" ^
	-DHIP_HIPCC_EXECUTABLE="C:/rocm/6.4/bin/hipcc.exe"

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
	set BUILD_DIR=build_release
	set INSTALL_DIR=install_release
	set CONFIG=RelWithDebInfo
	REM set CONFIG=Release
) else if "%COMMAND%" == "debug" (
	set BUILDING=Yes
	set BUILD_DIR=build_debug
	set INSTALL_DIR=install_debug
	set CONFIG=Debug
)

echo BUILDING CONFIG %CONFIG%

if "%BUILDING%" == "Yes" (
	cmake -G Ninja -DCMAKE_BUILD_TYPE=%CONFIG% -B %BUILD_DIR% ^
	-DCMAKE_INSTALL_PREFIX=%INSTALL_DIR% ^
	-DWITH_CYCLES_DEVICE_ONEAPI=OFF ^
	-DWITH_CYCLES_ALEMBIC=OFF ^
	-DWITH_CYCLES_USD=OFF ^
	-DWITH_CYCLES_OSL=OFF ^
	-DWITH_CYCLES_OPENCOLORIO=OFF ^
	-DWITH_CYCLES_OPENSUBDIV=OFF ^
	-DWITH_CYCLES_OPENIMAGEDENOISE=OFF ^
	-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF ^
	-DWITH_CYCLES_CUDA_BINARIES=ON ^
	-DWITH_CYCLES_DEVICE_OPTIX=ON ^
	-DCUDA_NVCC_FLAGS=--allow-unsupported-compiler ^
	%BUILD_CUDA_CMD% ^
	%BUILD_CUDABIN_CMD% ^
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
