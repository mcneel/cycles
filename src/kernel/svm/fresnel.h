/* SPDX-FileCopyrightText: 2011-2022 Blender Foundation
 *
 * SPDX-License-Identifier: Apache-2.0 */

#pragma once

#include "kernel/closure/bsdf_util.h"

#include "kernel/svm/node_types.h"
#include "kernel/svm/util.h"

CCL_NAMESPACE_BEGIN

/* Fresnel Node */

ccl_device_noinline void svm_node_fresnel(ccl_private ShaderData *sd,
                                          ccl_private float *ccl_restrict stack,
                                          const ccl_global SVMNodeFresnel &ccl_restrict node)
{
  float eta = stack_load(stack, node.ior);
  const float3 normal_in = stack_load_float3_default(stack, node.normal_offset, sd->N);

  eta = fmaxf(eta, 1e-5f);
  //eta = (sd->flag & SD_BACKFACING) ? 1.0f / eta : eta;
  /* Rhino: flip for back faces. normal_in is const in 5.2, so work on a copy. */
  const float3 normal = (sd->flag & SD_BACKFACING) ? -normal_in : normal_in;

  const float f = fresnel_dielectric_cos(dot(sd->wi, normal), eta);

  stack_store_float(stack, node.out_offset, f);
}

/* Layer Weight Node */

ccl_device_noinline void svm_node_layer_weight(ccl_private ShaderData *sd,
                                               ccl_private float *ccl_restrict stack,
                                               const ccl_global SVMNodeLayerWeight &ccl_restrict
                                                   node)
{
  float blend = stack_load(stack, node.blend);
  const float3 normal_in = stack_load_float3_default(stack, node.normal_offset, sd->N);
  const float3 normal = normal_in;

  float f;

  if (node.weight_type == NODE_LAYER_WEIGHT_FRESNEL) {
    float eta = fmaxf(1.0f - blend, 1e-5f);
    eta = (sd->flag & SD_BACKFACING) ? eta : 1.0f / eta;

    f = fresnel_dielectric_cos(dot(sd->wi, normal), eta);
  }
  else {
    f = fabsf(dot(sd->wi, normal));

    if (blend != 0.5f) {
      blend = clamp(blend, 0.0f, 1.0f - 1e-5f);
      blend = (blend < 0.5f) ? 2.0f * blend : 0.5f / (1.0f - blend);

      f = powf(f, blend);
    }

    f = 1.0f - f;
  }

  stack_store_float(stack, node.out_offset, f);
}

CCL_NAMESPACE_END
