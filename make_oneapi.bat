@echo off

REM Convenience wrapper for oneAPI-only Windows build that outputs a DLL.

setlocal enableextensions enabledelayedexpansion

set "BUILD_DIR=build_oneapi"
set "PYTHON=python"
set "COMMAND=%1"
set "VARIANT=%2"

if "%COMMAND%" == "" (
  set "COMMAND=release"
)

if /I "%COMMAND%" == "release" (
  set "CONFIG=RelWithDebInfo"
) else if /I "%COMMAND%" == "debug" (
  set "CONFIG=Debug"
) else if /I "%COMMAND%" == "update" (
  set "CONFIG="
) else if /I "%COMMAND%" == "clean" (
  set "CONFIG="
) else (
  echo Command "%COMMAND%" unknown
  echo Usage: make_oneapi.bat [release^|debug^|update^|clean] [aot^|jit]
  exit /b 1
)

set "CYCLES_LIB_PLATFORMS=win64_vc15"

if /I "%VARIANT%" == "jit" (
  set "ONEAPI_BINARIES=OFF"
) else (
  set "ONEAPI_BINARIES=ON"
)

if /I "%COMMAND%" == "update" (
  %PYTHON% src/cmake/make_update.py --no-cycles
  exit /b %ERRORLEVEL%
)

if /I "%COMMAND%" == "clean" (
  if exist %BUILD_DIR% (
    cd %BUILD_DIR% && cmake --build . --target install --config Clean
    exit /b %ERRORLEVEL%
  )
  echo %BUILD_DIR% does not exist, skipping clean.
  exit /b 0
)

if not defined LEVELZERO_INC (
  set "LEVELZERO_INC=..\lib\win64_vc15\level-zero\include"
)
if not defined LEVELZERO_LIB (
  set "LEVELZERO_LIB=..\lib\win64_vc15\level-zero\lib"
)
if not defined MSVC_REDIST_DIR (
  set "MSVC_REDIST_DIR=C:/Program Files (x86)/Microsoft Visual Studio/2019/Professional/VC/Redist/MSVC/14.29.30133"
)
if not defined WINDOWS_KITS_DIR (
  set "WINDOWS_KITS_DIR=C:/Program Files (x86)/Windows Kits/10"
)

%PYTHON% src/cmake/make_update.py --no-cycles
if errorlevel 1 exit /b %ERRORLEVEL%

cmake -B %BUILD_DIR% ^
  -DWITH_CYCLES_ALEMBIC=OFF ^
  -DWITH_CYCLES_EMBREE=OFF ^
  -DWITH_CYCLES_OPENCOLORIO=OFF ^
  -DWITH_CYCLES_OPENIMAGEDENOISE=OFF ^
  -DWITH_CYCLES_OPENSUBDIV=OFF ^
  -DWITH_CYCLES_OPENVDB=OFF ^
  -DWITH_CYCLES_NANOVDB=OFF ^
  -DWITH_CYCLES_OSL=OFF ^
  -DWITH_CYCLES_USD=OFF ^
  -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF ^
  -DWITH_CYCLES_CUDA_BINARIES=OFF ^
  -DWITH_CYCLES_DEVICE_OPTIX=OFF ^
  -DWITH_CUDA_DYNLOAD=OFF ^
  -DWITH_CYCLES_DEVICE_ONEAPI=ON ^
  -DWITH_CYCLES_ONEAPI_BINARIES=%ONEAPI_BINARIES% ^
  -D_LEVEL_ZERO_INCLUDE_DIR=%LEVELZERO_INC% ^
  -D_LEVEL_ZERO_LIBRARY=%LEVELZERO_LIB% ^
  -DMSVC_REDIST_DIR="%MSVC_REDIST_DIR%" ^
  -DWINDOWS_KITS_DIR="%WINDOWS_KITS_DIR%" ^
  && cd %BUILD_DIR% && cmake --build . --target install --config %CONFIG%

if errorlevel 1 exit /b %ERRORLEVEL%

if /I "%ONEAPI_BINARIES%" == "ON" (
  echo Built install\cycles_kernel_oneapi_aot.dll
) else (
  echo Built install\cycles_kernel_oneapi_jit.dll
)
