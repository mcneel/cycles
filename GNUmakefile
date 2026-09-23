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

# arm64 only. Blender stopped publishing Intel macOS dependencies after 4.5 - the
# lib-macos_x64 release branches end there and its main was last touched in June 2025,
# missing fourteen of the packages the arm64 set has. Building Cycles 5.x for Intel would
# mean maintaining that dependency stack ourselves, and Intel Mac support is not wanted.
# Override if that ever changes:  make release MAC_ARCHS="x86_64;arm64"
# Alembic, Hydra, the standalone GUI and USD stay off, matching Windows - Rhino does not
# use them through Cycles, and each one is another dependency in the payload.
#
# Embree, OSL, OpenImageDenoise and OpenVDB/NanoVDB used to be switched off here. Those
# were build workarounds from the 3.5 era - "temporarily disable ... in order to get Cycles
# kernels building", and "Metal building fixes" - not decisions about what Mac should ship.
# Windows never disabled any of them, so Mac was quietly missing denoising, volumes, OSL and
# Embree's BVH. They build and render fine on 5.2, so they are back on and the two platforms
# now agree.

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

# --- macOS payload: one command ---------------------------------------------------
#
#   make payload
#
# fetches the dependency libraries, builds, applies the two payload fixups, and copies
# the result into big_libs. The individual steps are available separately below.

MAC_LIB_DIR:=lib/macos_arm64
MAC_LIB_URL:=https://projects.blender.org/blender/lib-macos_arm64.git
BIG_LIBS:=../../../../../big_libs/RhinoCycles/ccycles/osx/release

# Blender's dependency libraries. The submodule is declared `update = none`, so
# `git submodule update --init` skips it and a fresh checkout has an empty lib/ - which
# is why this target exists rather than being a line in the README nobody finds. Only
# the pinned commit is fetched, shallow: the full history is enormous and unwanted.
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

# --delete, not `cp -r`: cp merges the new source/ over the old one and leaves both
# kernel generations in place, and on Mac source/ completeness is what decides whether
# Metal can compile its kernels at all.
publish:
	@test -f "$(INSTALL_DIR)/libccycles.dylib" || { echo "publish: nothing built - run make release" >&2; exit 1; }
	mkdir -p "$(BIG_LIBS)"
	rsync -a --delete "$(INSTALL_DIR)/lib/" "$(BIG_LIBS)/lib/"
	rsync -a --delete "$(INSTALL_DIR)/source/" "$(BIG_LIBS)/source/"
	cp "$(INSTALL_DIR)/libccycles.dylib" "$(BIG_LIBS)/"
	@echo "publish: payload copied into $(BIG_LIBS)"

payload: deps release publish

# --- macOS local payload: what the "Debug Cycles" / "Release Cycles" schemes run ---
#
#   make local
#
# The Mac counterpart of Windows' Debug+Cycles and ReleaseDebuggable+Cycles. Builds
# RelWithDebInfo, as both of those do - release-speed kernels, and still symbols for
# stepping into ccycles - into big_libs' osx/local/, which is gitignored. It has its own
# build and install folders, so it never disturbs a `make release` tree kept for
# publishing, and never touches the committed osx/release/ payload.
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
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="$(MAC_ARCHS)" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 -DWITH_CYCLES_ALEMBIC=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DCMAKE_BUILD_TYPE=Release .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_PAYLOAD)

debug:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="$(MAC_ARCHS)" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 -DWITH_CYCLES_ALEMBIC=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DCMAKE_BUILD_TYPE=Debug .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_PAYLOAD)

# The install prefix is passed explicitly: CMakeLists.txt defaults it to ./install, which
# is only right while INSTALL_DIR is left at its default.
relwithdebinfo:
	mkdir -p $(BUILD_DIR)
	cd $(BUILD_DIR) && cmake $(BUILD_CMAKE_ARGS) -DCMAKE_OSX_ARCHITECTURES="$(MAC_ARCHS)" -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4 -DWITH_CYCLES_ALEMBIC=OFF -DWITH_CYCLES_USD=OFF -DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF -DWITH_CYCLES_STANDALONE_GUI=OFF -DCMAKE_BUILD_TYPE=RelWithDebInfo -DCMAKE_INSTALL_PREFIX="$(abspath $(INSTALL_DIR))" .. && cmake --build . -j $(PARALLEL_JOBS) --target install
	$(FIX_PAYLOAD)

# INSTALL_DIR too: it is not overwritten, only added to, so libraries from a previous
# configuration linger there and get published. A build with Embree off still shipped the
# Embree dylib from an earlier build with it on.
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
