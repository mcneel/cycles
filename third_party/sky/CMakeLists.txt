# SPDX-FileCopyrightText: 2002-2022 Blender Authors
#
# SPDX-License-Identifier: BSD-3-Clause

set(INC
  include
)

set(INC_SYS

)

set(SRC
  source/sky_model.cpp
  source/sky_nishita.cpp

  include/sky_model.h
  source/sky_float3.h
  source/sky_model_data.h
)

include_directories(
  ${INC} ${INC_SYS}
)

add_library(extern_sky "${SRC}" "${INC}" "${INC_SYS}")
