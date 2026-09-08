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

# Two things have to be done to a freshly built Mac payload before it is fit to copy
# into big_libs, and both are easy to forget:
#
#   fix-cycles-rpaths.sh  the build bakes machine-specific absolute rpaths into
#                         libccycles.dylib, which break every deployed copy (RH-96549).
#   fix-cycles-tbb.sh     the payload ships oneTBB as libtbb.dylib, the same filename
#                         Rhino uses for the TBB 2020.3 that USD needs. Whichever lands
#                         last in Contents/Frameworks wins and the loser's consumers
#                         resolve against a library that cannot satisfy them (RH-98415).
#
# Running them here means regenerating the prebuilt cannot skip them. MacDotNetMakefile
# has a guard for the first; there is none for the second, so this is its only enforcement.
ifeq ($(OS), Darwin)
FIX_PAYLOAD:=./fix-cycles-rpaths.sh $(INSTALL_DIR)/libccycles.dylib && ./fix-cycles-tbb.sh $(INSTALL_DIR)
else
FIX_PAYLOAD:=true
endif

all: release

release:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="x86_64;arm64" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 -DWITH_CYCLES_EMBREE=OFF -DWITH_CYCLES_OSL=OFF -DWITH_CYCLES_OPENIMAGEDENOISE=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DWITH_CYCLES_OPENVDB=OFF -DWITH_CYCLES_NANOVDB=OFF -DCMAKE_BUILD_TYPE=Release .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_PAYLOAD)

debug:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="x86_64;arm64" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 -DWITH_CYCLES_EMBREE=OFF -DWITH_CYCLES_OSL=OFF -DWITH_CYCLES_OPENIMAGEDENOISE=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DWITH_CYCLES_OPENVDB=OFF -DWITH_CYCLES_NANOVDB=OFF -DCMAKE_BUILD_TYPE=Debug .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_PAYLOAD)

clean:
	rm -rf $(BUILD_DIR)

test:
	cd $(BUILD_DIR) && ctest --output-on-failure

update:
	$(PYTHON) src/cmake/make_update.py

update_legacy:
	$(PYTHON) src/cmake/make_update.py --legacy

format:
	$(PYTHON) src/cmake/make_format.py
