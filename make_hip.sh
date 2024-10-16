#!/bin/bash

set -e

BUILD_DIR=build_hip
PYTHON=python3
COMMAND=$1

#rm -rf $BUILD_DIR

if [ -z "$COMMAND" ]
then
  COMMAND=Release
fi

#$PYTHON src/cmake/make_update.py

cmake -B $BUILD_DIR \
-DWITH_CYCLES_ALEMBIC=OFF \
-DWITH_CYCLES_EMBREE=OFF \
-DWITH_CYCLES_OPENCOLORIO=OFF \
-DWITH_CYCLES_OPENIMAGEDENOISE=OFF \
-DWITH_CYCLES_OPENSUBDIV=OFF \
-DWITH_CYCLES_OPENVDB=OFF \
-DWITH_CYCLES_NANOVDB=OFF \
-DWITH_CYCLES_OSL=OFF \
-DWITH_CYCLES_USD=OFF \
-DWITH_CYCLES_HYDRA_RENDER_DELEGATE=OFF \
-DWITH_CYCLES_CUDA_BINARIES=OFF \
-DWITH_CYCLES_DEVICE_OPTIX=OFF \
-DWITH_CUDA_DYNLOAD=OFF \
-DWITH_CYCLES_DEVICE_HIP=ON \
-DWITH_CYCLES_HIP_BINARIES=ON \
-DCYCLES_HIP_BINARIES_ARCH="gfx900;gfx906;gfx90c;gfx902;gfx1010;gfx1011;gfx1012;gfx1030;gfx1031;gfx1032;gfx1034;gfx1035;gfx1036;gfx1100;gfx1101;gfx1102;gfx1103;gfx1150;gfx1151;gfx1152;gfx1200;gfx1201" \
-DHIP_HIPCC_EXECUTABLE="/opt/rocm-6.2.2/bin/hipcc" \
&& cd $BUILD_DIR && cmake --build . -j 8 --target install --config $COMMAND
