# Convenience wrapper for CMake commands

ifeq ($(OS),Windows_NT)
	$(error On Windows, use "cmd //c make.bat" instead of "make")
endif

OS:=$(shell uname -s)

ifndef BUILD_CMAKE_ARGS
	BUILD_CMAKE_ARGS:=
endif

ifndef BUILD_DIR
	BUILD_DIR:=./build
endif

ifndef PYTHON
	PYTHON:=python3
endif

ifndef PARALLEL_JOBS
	PARALLEL_JOBS:=1
	ifeq ($(OS), Linux)
		PARALLEL_JOBS:=$(shell nproc)
	endif
	ifneq (,$(filter $(OS),Darwin FreeBSD))
		PARALLEL_JOBS:=$(shell sysctl -n hw.ncpu)
	endif
endif

all: release

release:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="x86_64;arm64" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 -DWITH_CYCLES_EMBREE=OFF -DWITH_CYCLES_OSL=OFF -DWITH_CYCLES_OPENIMAGEDENOISE=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DCMAKE_BUILD_TYPE=Release .. && cmake --build . -j $(PARALLEL_JOBS) --target install

debug:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="x86_64;arm64" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 -DWITH_CYCLES_EMBREE=OFF -DWITH_CYCLES_OSL=OFF -DWITH_CYCLES_OPENIMAGEDENOISE=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DCMAKE_BUILD_TYPE=Debug .. && cmake --build . -j $(PARALLEL_JOBS) --target install

clean:
	rm -rf $(BUILD_DIR)

test:
	cd $(BUILD_DIR) && ctest

update:
	$(PYTHON) src/cmake/make_update.py

format:
	$(PYTHON) src/cmake/make_format.py
