/**
Copyright 2014 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

#include "svm_rhino_types.h"

CCL_NAMESPACE_BEGIN

ccl_device void svm_rhino_node_matrix_math(KernelGlobals kg,
                                     ccl_private ShaderData *sd,
                                     ccl_private float *stack,
                                     uint itype,
                                     uint vec_offset,
                                     uint out_offset,
                                     ccl_private int *offset)
{
  NodeMatrixMath type = (NodeMatrixMath)itype;
  float3 v = stack_load_float3(stack, vec_offset);

  Transform tfm;
  tfm.x = read_node_float(kg, offset);
  tfm.y = read_node_float(kg, offset);
  tfm.z = read_node_float(kg, offset);

  float3 r;
  switch (type) {
    case NODE_MATRIX_MATH_DIRECTION: {
      r = transform_direction(&tfm, v);
      break;
    }
    case NODE_MATRIX_MATH_PERSPECTIVE: {
#ifndef __KERNEL_GPU__
      ProjectionTransform pt(tfm);
      r = transform_perspective(&pt, v);
#endif
      break;
    }
    case NODE_MATRIX_MATH_DIR_TRANSPOSED: {
      r = transform_direction_transposed(&tfm, v);
      break;
    }
    default:
      r = transform_point(&tfm, v);
  }
  stack_store_float3(stack, out_offset, r);
}

CCL_NAMESPACE_END
