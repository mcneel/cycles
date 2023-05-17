@echo off

REM Convenience wrapper for CMake commands

setlocal enableextensions enabledelayedexpansion

set BUILD_DIR=build
set PYTHON=python
set COMMAND=%1

if "%COMMAND%" == "" (
  set COMMAND=release
)

if "%COMMAND%" == "release" (
	cmake -B %BUILD_DIR% -DWITH_CYCLES_ALEMBIC=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_DEVICE_ONEAPI=ON -DWITH_CYCLES_DEVICE_HIP=ON -DWITH_CYCLES_CUDA_BINARIES=ON -DCYCLES_CUDA_BINARIES_ARCH=compute_52 && cd %BUILD_DIR% && cmake --build . --target install --config Release
) else if "%COMMAND%" == "debug" (
	cmake -B %BUILD_DIR% -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_DEVICE_ONEAPI=ON -DWITH_CYCLES_DEVICE_HIP=ON -DWITH_CYCLES_CUDA_BINARIES=ON -DCYCLES_CUDA_BINARIES_ARCH=compute_52 && cd %BUILD_DIR% && cmake --build . --target install --config Debug
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
