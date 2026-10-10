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

# arm64 only: Blender stopped publishing Intel macOS dependencies after 4.5.
# Override if that changes:  make release MAC_ARCHS="x86_64;arm64"
# Same flags as build_cycles.ps1. Rhino uses none of these; it has its own denoiser and
# renders with SVM only (ccsession.cpp). Embree and OpenVDB stay on, as on Windows.
# After changing them, `make clean` before `make payload`, or stale dylibs get published.
CYCLES_CMAKE_FLAGS:=-DWITH_CYCLES_ALEMBIC=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DWITH_CYCLES_OPENIMAGEDENOISE=OFF -DWITH_CYCLES_OSL=OFF

ifndef MAC_ARCHS
	MAC_ARCHS:=arm64
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

# Every Mac build gets both fixups, so a regenerated payload cannot skip them:
#   fix-cycles-rpaths.sh  machine-specific absolute rpaths break deployed copies (RH-96549).
#   fix-cycles-tbb.sh     oneTBB as libtbb.dylib clashes with Rhino's TBB 2020.3 (RH-98415);
#                         nothing else guards this one.
ifeq ($(OS), Darwin)
FIX_PAYLOAD:=./fix-cycles-rpaths.sh $(INSTALL_DIR)/libccycles.dylib && ./fix-cycles-tbb.sh $(INSTALL_DIR)
else
FIX_PAYLOAD:=true
endif

# --- macOS payload ------------------------------------------------------------------
# `make payload`: fetch deps, build release, copy into big_libs.

MAC_LIB_DIR:=lib/macos_arm64
MAC_LIB_URL:=https://projects.blender.org/blender/lib-macos_arm64.git
BIG_LIBS:=../../../../../big_libs/RhinoCycles/ccycles/osx/release

# The lib submodule is `update = none`, so a fresh checkout has an empty lib/.
# Fetch only the pinned commit, shallow: the full history is enormous.
deps:
	@if [ -d "$(MAC_LIB_DIR)/tbb" ]; then \
		echo "deps: $(MAC_LIB_DIR) already present"; \
	else \
		sha=`git ls-tree HEAD $(MAC_LIB_DIR) | awk '{print $$3}'`; \
		echo "deps: fetching $(MAC_LIB_DIR) at $$sha (about 2.4 GB, once)"; \
		mkdir -p "$(MAC_LIB_DIR)"; \
		if [ ! -d "$(MAC_LIB_DIR)/.git" ]; then \
			git -C "$(MAC_LIB_DIR)" init -q .; \
			git -C "$(MAC_LIB_DIR)" remote add origin $(MAC_LIB_URL); \
		fi; \
		git -C "$(MAC_LIB_DIR)" fetch --depth 1 origin $$sha; \
		git -C "$(MAC_LIB_DIR)" checkout -q FETCH_HEAD; \
	fi

# --delete, not `cp -r`: cp would mix old and new kernel sources in source/, which Metal
# compiles its kernels from.
publish:
	@test -f "$(INSTALL_DIR)/libccycles.dylib" || { echo "publish: nothing built - run make release" >&2; exit 1; }
	mkdir -p "$(BIG_LIBS)"
	rsync -a --delete "$(INSTALL_DIR)/lib/" "$(BIG_LIBS)/lib/"
	rsync -a --delete "$(INSTALL_DIR)/source/" "$(BIG_LIBS)/source/"
	cp "$(INSTALL_DIR)/libccycles.dylib" "$(BIG_LIBS)/"
	@echo "publish: payload copied into $(BIG_LIBS)"

payload: deps release publish

# --- macOS local payload: `make local`, run by the "Debug Cycles"/"Release Cycles" schemes ---
# RelWithDebInfo, like Windows' +Cycles configs, into gitignored osx/local/, with its own
# build and install dirs so the release tree and the committed osx/release/ stay untouched.
# MacDotNetMakefile deploys local/ instead of release/ while it is the newer of the two.
LOCAL_BUILD_DIR:=./build-local
LOCAL_INSTALL_DIR:=./install-local
BIG_LIBS_LOCAL:=../../../../../big_libs/RhinoCycles/ccycles/osx/local

local: deps
	$(MAKE) relwithdebinfo BUILD_DIR=$(LOCAL_BUILD_DIR) INSTALL_DIR=$(LOCAL_INSTALL_DIR)
	$(MAKE) publish INSTALL_DIR=$(LOCAL_INSTALL_DIR) BIG_LIBS=$(BIG_LIBS_LOCAL)

all: release

release:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="$(MAC_ARCHS)" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 $(CYCLES_CMAKE_FLAGS) -DCMAKE_BUILD_TYPE=Release .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_PAYLOAD)

debug:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="$(MAC_ARCHS)" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 $(CYCLES_CMAKE_FLAGS) -DCMAKE_BUILD_TYPE=Debug .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_PAYLOAD)

# Explicit prefix: CMakeLists.txt defaults it to ./install, wrong once INSTALL_DIR is overridden.
relwithdebinfo:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="$(MAC_ARCHS)" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 $(CYCLES_CMAKE_FLAGS) -DCMAKE_BUILD_TYPE=RelWithDebInfo -DCMAKE_INSTALL_PREFIX="$(abspath $(INSTALL_DIR))" .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_PAYLOAD)

# clean removes INSTALL_DIR too: installs only add to it, so stale libraries get published.
.PHONY: all release debug relwithdebinfo clean test deps publish payload local

clean:
	rm -rf $(BUILD_DIR) $(INSTALL_DIR)

test:
	cd $(BUILD_DIR) && ctest --output-on-failure

update:
	$(PYTHON) src/cmake/make_update.py

update_legacy:
	$(PYTHON) src/cmake/make_update.py --legacy

format:
	$(PYTHON) src/cmake/make_format.py
