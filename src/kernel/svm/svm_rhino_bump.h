/**
Copyright 2026 Robert McNeel and Associates

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

#pragma once

CCL_NAMESPACE_BEGIN

/* Bump the way Rhino's display does it: a Sobel on the height at the 8 texels around the point,
 * so a height change of 1 per texel tilts the normal by atan(8), independent of object size
 * and units. RHINO_NODE_BUMP_OFFSETS finds the world offsets to the neighbours, the nodes feeding
 * the height are evaluated once per neighbour with their texture coordinates shifted by
 * RHINO_NODE_BUMP_SHIFT, and RHINO_NODE_BUMP turns the 8 heights into the normal. */

/* Neighbour k in texel steps along u and v, row by row around the centre:
 *   0 1 2
 *   3 . 4
 *   5 6 7 */
ccl_device_inline float2 svm_rhino_bump_neighbour(const int k)
{
  const int i = (k < 4) ? k : k + 1;
  return make_float2((float)(i % 3) - 1.0f, (float)(i / 3) - 1.0f);
}

/* Move the shading point by a world offset while one copy's texture coordinates are evaluated,
 * then back. The barycentrics move with it, for mesh UVs. */
ccl_device_noinline void svm_rhino_node_bump_shift(KernelGlobals kg,
                                                   ccl_private ShaderData *sd,
                                                   ccl_private float *stack,
                                                   uint4 node)
{
  uint offset_offset, state_p_offset, state_uv_offset, begin;
  svm_unpack_node_uchar4(node.y, &offset_offset, &state_p_offset, &state_uv_offset, &begin);

  if (begin) {
    stack_store_float3(stack, state_p_offset, sd->P);
    stack_store_float3(stack, state_uv_offset, make_float3(sd->u, sd->v, 0.0f));

    const float3 dP = stack_load_float3(stack, offset_offset);
    sd->P += dP;
#ifdef __DPDU__
    if (sd->type & PRIMITIVE_TRIANGLE) {
      differential3 d;
      d.dx = dP;
      d.dy = zero_float3();
      differential du, dv;
      differential_dudv(&du, &dv, sd->dPdu, sd->dPdv, d, sd->Ng);
      sd->u += du.dx;
      sd->v += dv.dx;
    }
#endif
  }
  else {
    sd->P = stack_load_float3(stack, state_p_offset);
    const float3 uv = stack_load_float3(stack, state_uv_offset);
    sd->u = uv.x;
    sd->v = uv.y;
  }
}

/* dP.dx and dP.dy as world offsets, so the texture coordinates along them can be evaluated
 * with RHINO_NODE_BUMP_SHIFT, the same way as the neighbours'. */
ccl_device_noinline void svm_rhino_node_bump_differentials(ccl_private ShaderData *sd,
                                                           ccl_private float *stack,
                                                           uint4 node)
{
#ifdef __RAY_DIFFERENTIALS__
  const differential3 dP = differential_from_compact(sd->Ng, sd->dP);
  stack_store_float3(stack, node.y, dP.dx);
  stack_store_float3(stack, node.z, dP.dy);
#else
  stack_store_float3(stack, node.y, zero_float3());
  stack_store_float3(stack, node.z, zero_float3());
#endif
}

/* World offsets from the point to its 8 neighbouring texels, from the texture coordinates at
 * the point and along dP.dx and dP.dy. Also outputs the world step of one texel along u and v. */
ccl_device_noinline int svm_rhino_node_bump_offsets(KernelGlobals kg,
                                                    ccl_private ShaderData *sd,
                                                    ccl_private float *stack,
                                                    uint4 node,
                                                    int offset)
{
  uint uvw_c_offset, uvw_x_offset, uvw_y_offset, snap;
  svm_unpack_node_uchar4(node.y, &uvw_c_offset, &uvw_x_offset, &uvw_y_offset, &snap);
  uint out_offset[8];
  svm_unpack_node_uchar4(node.z, &out_offset[0], &out_offset[1], &out_offset[2], &out_offset[3]);
  svm_unpack_node_uchar4(node.w, &out_offset[4], &out_offset[5], &out_offset[6], &out_offset[7]);

  const uint4 data = read_node(kg, &offset);
  const uint axis_u_offset = data.x, axis_v_offset = data.y;

  /* Maps texture coordinates to texels. */
  Transform texel;
  texel.x = read_node_float(kg, &offset);
  texel.y = read_node_float(kg, &offset);
  texel.z = read_node_float(kg, &offset);

  float3 axis_u = zero_float3();
  float3 axis_v = zero_float3();
  float2 centre = zero_float2();

#ifdef __RAY_DIFFERENTIALS__
  const differential3 dP = differential_from_compact(sd->Ng, sd->dP);
  const float3 uvw_c = stack_load_float3(stack, uvw_c_offset);
  const float3 tx = transform_direction(&texel, stack_load_float3(stack, uvw_x_offset) - uvw_c);
  const float3 ty = transform_direction(&texel, stack_load_float3(stack, uvw_y_offset) - uvw_c);

  /* Texels per step along the densest direction: the largest singular value of the
   * step-to-texel Jacobian. dP.dx and dP.dy are orthogonal and sd->dP long. */
  const float a = dot(tx, tx), b = dot(tx, ty), c = dot(ty, ty);
  const float texels_per_step = safe_sqrtf(0.5f *
                                           (a + c + safe_sqrtf(sqr(a - c) + 4.0f * b * b)));

  if (texels_per_step > 1e-6f) {
    /* An image varies along u and v only, so its texel transform has no w row. */
    if (is_zero(make_float3(texel.z.x, texel.z.y, texel.z.z))) {
      /* One texel along u and along v: invert the map from dP.dx and dP.dy to texels. */
      const float det = tx.x * ty.y - ty.x * tx.y;
      if (fabsf(det) > 1e-6f * sqr(texels_per_step)) {
        axis_u = (ty.y * dP.dx - tx.y * dP.dy) / det;
        axis_v = (tx.x * dP.dy - ty.x * dP.dx) / det;
      }
      if (snap) {
        const float3 t = transform_point(&texel, uvw_c);
        centre = make_float2(floorf(t.x) + 0.5f - t.x, floorf(t.y) + 0.5f - t.y);
      }
    }
    else {
      /* 3D texture: one texel along the surface tangents. */
      const float texel_length = sd->dP / texels_per_step;
      axis_u = texel_length * normalize(dP.dx);
      axis_v = texel_length * normalize(dP.dy);
    }
  }
#endif

  for (int k = 0; k < 8; k++) {
    const float2 n = centre + svm_rhino_bump_neighbour(k);
    stack_store_float3(stack, out_offset[k], n.x * axis_u + n.y * axis_v);
  }
  stack_store_float3(stack, axis_u_offset, axis_u);
  stack_store_float3(stack, axis_v_offset, axis_v);

  return offset;
}

template<uint node_feature_mask>
ccl_device_noinline int svm_rhino_node_bump(KernelGlobals kg,
                                            ccl_private ShaderData *sd,
                                            ccl_private float *stack,
                                            uint4 node,
                                            int offset)
{
  uint h_offset[8];
  svm_unpack_node_uchar4(node.y, &h_offset[0], &h_offset[1], &h_offset[2], &h_offset[3]);
  svm_unpack_node_uchar4(node.z, &h_offset[4], &h_offset[5], &h_offset[6], &h_offset[7]);
  uint axis_u_offset, axis_v_offset, out_offset;
  svm_unpack_node_uchar3(node.w, &axis_u_offset, &axis_v_offset, &out_offset);

  const uint4 data = read_node(kg, &offset);
  uint normal_offset, strength_offset, linear;
  svm_unpack_node_uchar3(data.x, &normal_offset, &strength_offset, &linear);

#ifdef __RAY_DIFFERENTIALS__
  IF_KERNEL_NODES_FEATURE(BUMP)
  {
    const float3 normal_in = stack_valid(normal_offset) ? stack_load_float3(stack, normal_offset) :
                                                          sd->N;

    float h[8];
    for (int k = 0; k < 8; k++) {
      h[k] = stack_load_float(stack, h_offset[k]);
    }
    const float3 axis_u = stack_load_float3(stack, axis_u_offset);
    const float3 axis_v = stack_load_float3(stack, axis_v_offset);

    /* Sobel, as in the display's height-to-normal conversion: height change per texel. */
    const float g_u = ((h[2] + 2.0f * h[4] + h[7]) - (h[0] + 2.0f * h[3] + h[5])) / 8.0f;
    const float g_v = ((h[5] + 2.0f * h[6] + h[7]) - (h[0] + 2.0f * h[1] + h[2])) / 8.0f;

    /* World gradient grad with dot(grad, axis_u) = g_u and dot(grad, axis_v) = g_v. */
    const float uu = dot(axis_u, axis_u), uv = dot(axis_u, axis_v), vv = dot(axis_v, axis_v);
    const float det = uu * vv - uv * uv;

    float3 normal_out = normal_in;
    if (det > 1e-6f * uu * vv && det > 0.0f) {
      const float strength = stack_load_float(stack, strength_offset);

      const float3 grad = ((g_u * vv - g_v * uv) * axis_u + (g_v * uu - g_u * uv) * axis_v) / det;

      /* Measure the slope per texel along the densest direction: the shortest texel side. */
      const float texel_length = safe_sqrtf(
          0.5f * (uu + vv - safe_sqrtf(sqr(uu - vv) + 4.0f * uv * uv)));
      /* The display uses the raw Sobel sums with a z of 1: tan(tilt) = 8 * height per texel. */
      float3 p = (8.0f * texel_length) * grad;
      p -= dot(p, normal_in) * normal_in;

      if (linear) {
        /* Custom material: the strength scales the slope. */
        normal_out = safe_normalize(normal_in - strength * p);
      }
      else {
        /* PBR: the strength scales the tilted normal's tangential part and the rest is
         * recomputed, so it saturates. */
        const float3 t = (-strength / sqrtf(1.0f + dot(p, p))) * p;
        normal_out = safe_normalize(t + safe_sqrtf(1.0f - dot(t, t)) * normal_in);
      }
      if (is_zero(normal_out)) {
        normal_out = normal_in;
      }
    }

    normal_out = ensure_valid_reflection(sd->Ng, sd->wi, normal_out);
    stack_store_float3(stack, out_offset, normal_out);
  }
  else
  {
    stack_store_float3(stack, out_offset, zero_float3());
  }
#endif

  return offset;
}

CCL_NAMESPACE_END
