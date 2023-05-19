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
set OCLOC="C:\ProgramData\Intel\ocloc_win_dgpu_101.3802"
set MSVC_REDIST_DIR="C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\VC\Redist\MSVC\v142"

set BUILD_CUDA_CMD= ^
	-DWITH_CYCLES_CUDA_BINARIES=ON ^
	-DWITH_CYCLES_DEVICE_OPTIX=ON ^
	-DCYCLES_CUDA_BINARIES_ARCH=compute_52 ^
	-DOPTIX_ROOT_DIR=%OPTIX%

if not "%ARG1%" == "cuda" (
	if not "%ARG1%" == "all" (
	  set BUILD_CUDA_CMD=""
	)
)

set BUILD_ONEAPI_CMD= ^
	-DWITH_CYCLES_DEVICE_ONEAPI=ON ^
	-DWITH_CYCLES_ONEAPI_BINARIES=ON ^
	-D_LEVEL_ZERO_INCLUDE_DIR=%LEVELZERO_INC% ^
	-D_LEVEL_ZERO_LIBRARY=%LEVELZERO_LIB% ^
	-DOCLOC_INSTALL_DIR=%OCLOC% ^
	-DMSVC_REDIST_DIR=%MSVC_REDIST_DIR%

if not "%ARG1%" == "all" (
	set BUILD_ONEAPI_CMD=""
)

if "%COMMAND%" == "" (
  set COMMAND=release
)

if "%COMMAND%" == "release" (
	cmake -B %BUILD_DIR% ^
	-DWITH_CYCLES_ALEMBIC=OFF ^
	-DWITH_CYCLES_USD=OFF ^
	-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF ^
	-DWITH_CYCLES_DEVICE_HIP=OFF ^
	%BUILD_CUDA_CMD% ^
	%BUILD_ONEAPI_CMD% ^
	&& cd %BUILD_DIR% && cmake --build . --target install --config Release
	
) else if "%COMMAND%" == "debug" (
	cmake -B %BUILD_DIR% ^
	-DWITH_CYCLES_ALEMBIC=OFF ^
	-DWITH_CYCLES_USD=OFF ^
	-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF ^
	-DWITH_CYCLES_DEVICE_HIP=OFF ^
	%BUILD_CUDA_CMD% ^
	%BUILD_ONEAPI_CMD% ^
	&& cd %BUILD_DIR% && cmake --build . --target install --config Debug
	
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