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

ifndef INSTALL_DIR
	INSTALL_DIR:=./install
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

# On Mac the build bakes machine-specific absolute rpaths into libccycles.dylib, which
# breaks the deployed copies. Run the portability fixup as part of the build so it cannot
# be skipped when regenerating the prebuilt in big_libs (RH-96549).
ifeq ($(OS), Darwin)
FIX_RPATHS:=./fix-cycles-rpaths.sh $(INSTALL_DIR)/libccycles.dylib
else
FIX_RPATHS:=true
endif

all: release

release:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="x86_64;arm64" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 -DWITH_CYCLES_EMBREE=OFF -DWITH_CYCLES_OSL=OFF -DWITH_CYCLES_OPENIMAGEDENOISE=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DWITH_CYCLES_OPENVDB=OFF -DWITH_CYCLES_NANOVDB=OFF -DCMAKE_BUILD_TYPE=Release .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_RPATHS)

debug:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="x86_64;arm64" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 -DWITH_CYCLES_EMBREE=OFF -DWITH_CYCLES_OSL=OFF -DWITH_CYCLES_OPENIMAGEDENOISE=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DWITH_CYCLES_OPENVDB=OFF -DWITH_CYCLES_NANOVDB=OFF -DCMAKE_BUILD_TYPE=Debug .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_RPATHS)

clean:
	rm -rf $(BUILD_DIR) $(INSTALL_DIR)

test:
	cd $(BUILD_DIR) && ctest

update:
	$(PYTHON) src/cmake/make_update.py

format:
	$(PYTHON) src/cmake/make_format.py
