#!/usr/bin/env bash
# Builds OpenImageIO_Cycles for macOS: Blender's OpenImageIO build without libheif
# (RH-99462). The macOS counterpart of build_oiio.ps1, which explains the choices.
#
#   tools/build_oiio.sh [version]        default 3.1.13.1; must match the bundle's
#
# OIIO_WORK_DIR and OIIO_INSTALL_DIR override the work tree (default build_oiio/) and the
# install (default big_libs/OpenImageIO-<version>-Cycles/macos_arm64).
set -euo pipefail

VERSION="${1:-3.1.13.1}"
HERE="$(cd "$(dirname "$0")" && pwd)"
CYCLES="$(dirname "$HERE")"
LIB="$CYCLES/lib/macos_arm64"
WORK="${OIIO_WORK_DIR:-$CYCLES/build_oiio}"
DEPS="$WORK/deps"
# cycles-core -> RDK -> Plug-ins -> rhino4 -> src4 -> repo root
INSTALL="${OIIO_INSTALL_DIR:-$CYCLES/../../../../../big_libs/OpenImageIO-$VERSION-Cycles/macos_arm64}"
JOBS="$(sysctl -n hw.ncpu)"
COMMON=(-DCMAKE_BUILD_TYPE=Release -DCMAKE_OSX_ARCHITECTURES=arm64 -DCMAKE_OSX_DEPLOYMENT_TARGET=12.4
        -DCMAKE_POSITION_INDEPENDENT_CODE=ON)

step() { printf '==> %s\n' "$*"; }

header="$LIB/openimageio/include/OpenImageIO/oiioversion.h"
[ -f "$header" ] || { echo "No library bundle at $LIB. Fetch it first (BUILDING.md)." >&2; exit 1; }
bundle="$(for n in MAJOR MINOR PATCH TWEAK; do sed -n "s/^#define OIIO_VERSION_$n \([0-9]*\).*/\1/p" "$header"; done | paste -sd. -)"
[ "$bundle" = "$VERSION" ] || { echo "The bundle's OpenImageIO is $bundle, not $VERSION." >&2; exit 1; }

step "OpenImageIO $VERSION source"
SRC="$WORK/src"
[ -d "$SRC/.git" ] || git clone --quiet --depth 1 --branch "v$VERSION" \
    https://github.com/AcademySoftwareFoundation/OpenImageIO.git "$SRC"
for patch in "$HERE"/oiio/*.diff; do
    if git -C "$SRC" apply --check --reverse "$patch" 2>/dev/null; then echo "   $(basename "$patch"): already applied"
    else git -C "$SRC" apply "$patch"; echo "   $(basename "$patch"): applied"; fi
done

# Static libraries OpenImageIO links in that the bundle does not carry.
dep() {
    local name="$1" repo="$2" tag="$3"; shift 3
    step "$name $tag"
    [ -d "$WORK/deps-src/$name/.git" ] || git clone --quiet --depth 1 --branch "$tag" "$repo" "$WORK/deps-src/$name"
    cmake -S "$WORK/deps-src/$name" -B "$WORK/deps-build/$name" "${COMMON[@]}" -DBUILD_SHARED_LIBS=OFF \
        -DCMAKE_INSTALL_PREFIX="$DEPS/$name" "$@"
    cmake --build "$WORK/deps-build/$name" -j "$JOBS"
    cmake --install "$WORK/deps-build/$name"
}

dep libdeflate https://github.com/ebiggers/libdeflate.git v1.23 \
    -DLIBDEFLATE_BUILD_SHARED_LIB=OFF -DLIBDEFLATE_BUILD_GZIP=OFF
dep libtiff https://gitlab.com/libtiff/libtiff.git v4.7.1 \
    -DZLIB_LIBRARY="$LIB/zlib/lib/libz.a" -DZLIB_INCLUDE_DIR="$LIB/zlib/include" \
    -DJPEG_LIBRARY="$LIB/jpeg/lib/libjpeg.a" -DJPEG_INCLUDE_DIR="$LIB/jpeg/include" \
    -DDeflate_LIBRARY="$DEPS/libdeflate/lib/libdeflate.a" -DDeflate_INCLUDE_DIR="$DEPS/libdeflate/include" \
    -Dzlib=ON -Djpeg=ON -Dlibdeflate=ON -Dlzma=OFF -Djbig=OFF -Dzstd=OFF -Dwebp=OFF -Dlerc=OFF -Dcxx=OFF \
    -Dtiff-tools=OFF -Dtiff-tests=OFF -Dtiff-contrib=OFF -Dtiff-docs=OFF

step Configure
cmake -S "$SRC" -B "$WORK/build" "${COMMON[@]}" -DCMAKE_CXX_STANDARD=20 \
    -DCMAKE_INSTALL_PREFIX="$INSTALL" \
    -DOIIO_LIBNAME_SUFFIX=_Cycles -DBUILD_SHARED_LIBS=ON -DLINKSTATIC=ON \
    "-DOpenImageIO_REQUIRED_DEPS=WebP;JPEG;TIFF;OpenEXR;PNG;OpenJPEG;fmt;Robinmap;ZLIB;pugixml;openjph;TBB" \
    -DOpenImageIO_BUILD_MISSING_DEPS=Robinmap \
    -DTIFF_LIBRARY="$DEPS/libtiff/lib/libtiff.a" -DTIFF_INCLUDE_DIR="$DEPS/libtiff/include" \
    -DCMAKE_SHARED_LINKER_FLAGS="$DEPS/libdeflate/lib/libdeflate.a -headerpad_max_install_names" \
    -DUSE_LIBHEIF=OFF -DUSE_PYTHON=OFF -DOIIO_BUILD_TOOLS=OFF \
    -DUSE_NUKE=OFF -DUSE_OPENVDB=OFF -DUSE_FREETYPE=OFF -DUSE_DCMTK=OFF -DUSE_TBB=ON -DUSE_QT=OFF \
    -DUSE_GIF=OFF -DUSE_OPENCV=OFF -DUSE_OPENJPEG=ON -DUSE_FFMPEG=OFF -DUSE_PTEX=OFF -DUSE_LIBRAW=OFF \
    -DUSE_JXL=OFF -DUSE_OPENCOLORIO=ON -DUSE_WEBP=ON -DSTOP_ON_WARNING=OFF \
    -DOIIO_BUILD_TESTS=OFF -DBUILD_TESTING=OFF -DBUILD_DOCS=OFF -DINSTALL_DOCS=OFF -DINSTALL_FONTS=OFF \
    -DZLIB_LIBRARY="$LIB/zlib/lib/libz.a" -DZLIB_INCLUDE_DIR="$LIB/zlib/include" \
    -DPNG_LIBRARY="$LIB/png/lib/libpng.a" -DPNG_PNG_INCLUDE_DIR="$LIB/png/include" \
    -DJPEG_LIBRARY="$LIB/jpeg/lib/libjpeg.a" -DJPEG_INCLUDE_DIR="$LIB/jpeg/include" \
    -DOpenJPEG_ROOT="$LIB/openjpeg" -DCMAKE_IGNORE_PATH="$LIB/openjpeg/lib/cmake/openjpeg-2.5" \
    -DOpenJPEG_DIR=OpenJPEG_DIR-NOTFOUND \
    -DUSE_EXTERNAL_PUGIXML=ON -Dpugixml_DIR="$LIB/pugixml/lib/cmake/pugixml" \
    -DPUGIXML_LIBRARY="$LIB/pugixml/lib/libpugixml.a" -DPUGIXML_INCLUDE_DIR="$LIB/pugixml/include" \
    -DOpenColorIO_DIR="$LIB/opencolorio/lib/cmake/OpenColorIO" \
    -DFMT_INCLUDE_DIR="$LIB/fmt/include" -Dfmt_ROOT="$LIB/fmt" \
    -DWebP_ROOT="$LIB/webp" -DOpenEXR_ROOT="$LIB/openexr" -DImath_ROOT="$LIB/imath" \
    -DTBB_ROOT="$LIB/tbb" -Dopenjph_DIR="$LIB/openjph/lib/cmake/openjph"

step Build
cmake --build "$WORK/build" -j "$JOBS"

step "Install to $INSTALL"
rm -rf "$INSTALL"
cmake --install "$WORK/build"

dylib="$(ls "$INSTALL"/lib/libOpenImageIO_Cycles.*.dylib | head -1)"
if strings "$dylib" | grep -q heif_context; then echo "$dylib still contains libheif." >&2; exit 1; fi
if otool -L "$dylib" | grep -qi aom; then echo "$dylib still links aom." >&2; exit 1; fi
echo "   OK: $(cd "$INSTALL/lib" && ls libOpenImageIO*.dylib | tr '\n' ' '); no libheif, no aom"
