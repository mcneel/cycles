/* SPDX-FileCopyrightText: 2011-2022 Blender Foundation
 *
 * SPDX-License-Identifier: Apache-2.0 */

#pragma once

#include "kernel/globals.h"
#include "kernel/image.h"

#include "kernel/camera/projection.h"

#include "kernel/geom/object.h"

#include "kernel/svm/node_types.h"
#include "kernel/svm/util.h"

#include "util/color.h"
#include "util/types_image.h"

CCL_NAMESPACE_BEGIN

/* ---- Rhino image and environment mapping ----------------------------- */
/* Upstream 5.2 templated this file on Float3Type for dual-number derivatives.
 * The Rhino helpers below are float3-only; the environment dispatch that uses
 * them is reconciled at the call site. */

ccl_device float alternate_tile(float p)
{
    int mod = (int)p % 2;
    if (p > 0.0f) {
        if (mod == 0) return p;
        return (float)(2 * (int)p) - p + 1;
    }

    if (mod != 0) return p;
    return (float)(2 * (int)p) - p - 1;
}


ccl_device_inline float3 env_spherical(float3 R)
{
  // float3 Rc = make_float3(R.y, -R.z, -R.x);

  float x = -R.z;
  float y = -R.x;
  float z = R.y;

  float theta, phi;

  if (x == 0.0f && y == 0.0f) {
    theta = 0.0f;
    phi = (z >= 0.0f ? 0.5f * M_PI_F : -0.5f * M_PI_F);
  }
  else {
    theta = atan2f(y, x);
    if (theta < 0.0f)
      theta += 2.0f * M_PI_F;

    float r;
    if (fabsf(x) >= fabsf(y)) {
      r = y / x;
      r = fabsf(x) * sqrt(1.0f + r * r);
    }
    else {
      r = x / y;
      r = fabsf(y) * sqrt(1.0f + r * r);
    }

    phi = atan(z / r);
  }

  float u = theta / (2.0f * M_PI_F);
  float v = (-phi + 0.5f * M_PI_F) / M_PI_F;

  return make_float3(u, v, 0.0f);
}

ccl_device_inline float3 env_world_emap(float3 R)
{
  float x = R.x;
  float y = R.y;
  float z = R.z;

  float m = 2.0f * sqrt((x * x) + (y * y) + (z + 1.0f) * (z + 1.0f));

  float u = x / m + 0.5f;
  float v = y / m + 0.5f;

  return make_float3(u, v, 0.0f);
}

ccl_device_inline float3 env_emap_act(float3 R)
{
  float x = R.x;
  float y = R.y;
  float z = R.z;

  float fDivisor = sqrt((x * x) + (y * y));

  if (fDivisor < FLT_MIN)
    fDivisor = FLT_MIN;

  float f = sinf(0.5f * acosf(z)) / fDivisor;

  float px = -x * f;
  float py = y * f;

  float u = (1.0f + px) * 0.5f;
  float v = (1.0f - py) * 0.5f;

  return make_float3(u, v, 0.0f);
}

ccl_device_inline float3 env_emap(float3 R)
{
  R = make_float3(R.y, -R.z, -R.x);

  return env_emap_act(R);
}

ccl_device_inline float3 env_light_probe(float3 R)
{
  R = make_float3(R.y, -R.z, -R.x);

  float x = R.x;
  float y = R.y;
  float z = R.z;

  float fDivisor = sqrt((x * x) + (y * y));

  if (fDivisor < FLT_MIN)
    fDivisor = FLT_MIN;

  float f = (acosf(z) / M_PI_F) / fDivisor;

  float px = x * f;
  float py = y * f;

  float u = (1.0f + px) * 0.5f;
  float v = (1.0f - py) * 0.5f;

  return make_float3(u, v, 0.0f);
}

ccl_device_inline float3 env_box(float3 R)
{
  float x_abs = fabsf(R.x);
  float y_abs = fabsf(R.y);
  float z_abs = fabsf(R.z);

  float3 face_o, face_x, face_y;

  if (x_abs > y_abs && x_abs > z_abs) {
    if (R.x > 0.0f) {
      face_o = make_float3(+1.f, +1.f, -1.f);
      face_x = make_float3(0.f, -1.f, 0.f);
      face_y = make_float3(0.f, 0.f, +1.f);
    }
    else {
      face_o = make_float3(-1.f, -1.f, -1.f);
      face_x = make_float3(0.f, +1.f, 0.f);
      face_y = make_float3(0.f, 0.f, +1.f);
    }
  }
  else if (y_abs > z_abs) {
    if (R.y > 0.0f) {
      face_o = make_float3(-1.f, +1.f, -1.f);
      face_x = make_float3(+1.f, 0.f, 0.f);
      face_y = make_float3(0.f, 0.f, +1.f);
    }
    else {
      face_o = make_float3(+1.f, -1.f, -1.f);
      face_x = make_float3(-1.f, 0.f, 0.f);
      face_y = make_float3(0.f, 0.f, +1.f);
    }
  }
  else {
    if (R.z > 0.0f) {
      face_o = make_float3(+1.f, +1.f, +1.f);
      face_x = make_float3(0.f, -1.f, 0.f);
      face_y = make_float3(-1.f, 0.f, 0.f);
    }
    else {
      face_o = make_float3(-1.f, +1.f, -1.f);
      face_x = make_float3(0.f, -1.f, 0.f);
      face_y = make_float3(+1.f, 0.f, 0.f);
    }
  }

  float3 plane_normal = cross(face_x, face_y);

  float rp_dot = dot(R, plane_normal);
  kernel_assert(rp_dot != 0.0f);

  float t = dot(face_o, plane_normal) / rp_dot;

  float3 isect = t * R;
  float3 local_isect = isect - face_o;

  float u = dot(local_isect, face_x) / 2.0f;
  float v = dot(local_isect, face_y) / 2.0f;

  return make_float3(u, v, 0.0f);
}

ccl_device_inline int env_get_main_axis_index(float3 v)
{
  float x_abs = fabsf(v.x);
  float y_abs = fabsf(v.y);
  float z_abs = fabsf(v.z);

  if (x_abs > y_abs && x_abs > z_abs) {
    return 0;
  }
  else if (y_abs > z_abs) {
    return 1;
  }
  else {
    return 2;
  }
}

ccl_device_inline float3 env_cubemap(float3 R)
{
  R = make_float3(R.y, -R.z, -R.x);

  int main_axis = env_get_main_axis_index(R);
  float main_axis_dir = (main_axis == 0 ? R.x : (main_axis == 1 ? R.y : R.z));

  int sub_texture_index;

  if (main_axis == 0) {
    sub_texture_index = (main_axis_dir >= 0.0f ? 0 : 1);
  }
  else if (main_axis == 1) {
    sub_texture_index = (main_axis_dir >= 0.0f ? 3 : 2);
  }
  else {
    sub_texture_index = (main_axis_dir >= 0.0f ? 4 : 5);
  }

  float sub_texture_offset = (float)(sub_texture_index) / 6.0f;
  float ma = fabsf(main_axis_dir);

  float sc = 0.0f;
  float tc = 0.0f;

  if (sub_texture_index == 0) {
    sc = -R.z;
    tc = -R.y;
  }
  else if (sub_texture_index == 1) {
    sc = R.z;
    tc = -R.y;
  }
  else if (sub_texture_index == 2) {
    sc = R.x;
    tc = -R.z;
  }
  else if (sub_texture_index == 3) {
    sc = R.x;
    tc = R.z;
  }
  else if (sub_texture_index == 4) {
    sc = R.x;
    tc = -R.y;
  }
  else if (sub_texture_index == 5) {
    sc = -R.x;
    tc = -R.y;
  }

  float u = (sc / ma + 1.0f) / 12.0f + sub_texture_offset;
  float v = (tc / ma + 1.0f) / 2.0f;

  return make_float3(u, v, 0.0f);
}

ccl_device_inline float3 env_cubemap_vertical_cross(float3 R)
{
  R = make_float3(R.y, -R.z, -R.x);

  int main_axis = env_get_main_axis_index(R);
  float main_axis_dir = (main_axis == 0 ? R.x : (main_axis == 1 ? R.y : R.z));

  int sub_texture_index = (2 * main_axis) + (main_axis_dir >= 0.0f ? 0 : 1);

  float u_sub_tex_start = (sub_texture_index == 1 ?
                               0.0f :
                               (sub_texture_index == 0 ? (2.0f / 3.0f) : (1.0f / 3.0f)));
  float v_sub_tex_start = (sub_texture_index == 5 ?
                               0.0f :
                               (sub_texture_index == 2 ?
                                    (1.0f / 4.0f) :
                                    (sub_texture_index == 3 ? (3.0f / 4.0f) : (2.0f / 4.0f))));

  float ma = fabsf(main_axis_dir);

  float sc = 0.0f;
  float tc = 0.0f;

  if (sub_texture_index == 0) {
    sc = -R.z;
    tc = -R.y;
  }
  else if (sub_texture_index == 1) {
    sc = R.z;
    tc = -R.y;
  }
  else if (sub_texture_index == 2) {
    sc = R.x;
    tc = R.z;
  }
  else if (sub_texture_index == 3) {
    sc = R.x;
    tc = -R.z;
  }
  else if (sub_texture_index == 4) {
    sc = R.x;
    tc = -R.y;
  }
  else if (sub_texture_index == 5) {
    sc = R.x;
    tc = R.y;
  }

  float u = (sc / ma + 1.0f) / 6.0f + u_sub_tex_start;
  float v = (tc / ma + 1.0f) / 8.0f + v_sub_tex_start;

  return make_float3(u, v, 0.0f);
}

ccl_device_inline float3 env_cubemap_horizontal_cross(float3 R)
{
  R = make_float3(R.y, -R.z, -R.x);

  int main_axis = env_get_main_axis_index(R);
  float main_axis_dir = (main_axis == 0 ? R.x : (main_axis == 1 ? R.y : R.z));

  int sub_texture_index = (2 * main_axis) + (main_axis_dir >= 0.0f ? 0 : 1);

  float u_sub_tex_start = sub_texture_index == 1 ?
                              0.0f :
                              (sub_texture_index == 0 ?
                                   (2.0f / 4.0f) :
                                   (sub_texture_index == 5 ? (3.0f / 4.0f) : (1.0f / 4.0f)));
  float v_sub_tex_start = sub_texture_index == 2 ?
                              0.0f :
                              (sub_texture_index == 3 ? (2.0f / 3.0f) : (1.0f / 3.0f));

  float ma = fabsf(main_axis_dir);

  float sc = 0.0f;
  float tc = 0.0f;

  if (sub_texture_index == 0) {
    sc = -R.z;
    tc = -R.y;
  }
  else if (sub_texture_index == 1) {
    sc = R.z;
    tc = -R.y;
  }
  else if (sub_texture_index == 2) {
    sc = R.x;
    tc = R.z;
  }
  else if (sub_texture_index == 3) {
    sc = R.x;
    tc = -R.z;
  }
  else if (sub_texture_index == 4) {
    sc = R.x;
    tc = -R.y;
  }
  else if (sub_texture_index == 5) {
    sc = -R.x;
    tc = -R.y;
  }

  float u = (sc / ma + 1.0f) / 8.0f + u_sub_tex_start;
  float v = (tc / ma + 1.0f) / 6.0f + v_sub_tex_start;

  return make_float3(u, v, 0.0f);
}

ccl_device_inline float3 env_hemispherical(float3 R)
{
  R = make_float3(R.y, -R.z, -R.x);
  float3 hemi = normalize(make_float3(R.x, min(R.y, 0.0f), R.z));
  float3 uv = env_spherical(hemi);
  uv.y = 2.0f * uv.y - 1.0f;

  return uv;
}



ccl_device float4 svm_image_texture(
    KernelGlobals kg, ccl_private ShaderData *sd, const int id, const dual2 uv, const uint flags)
{
  float4 r = kernel_image_interp_with_udim(kg, sd, id, uv);
  const float alpha = r.w;

  if ((flags & NODE_IMAGE_ALPHA_UNASSOCIATE) && alpha != 1.0f && alpha != 0.0f) {
    r /= alpha;
    r.w = alpha;
  }

  if (flags & NODE_IMAGE_COMPRESS_AS_SRGB) {
    r = color_srgb_to_linear_v4(r);
  }

  return r;
}

/* Remap coordinate from 0..1 box to -1..-1 */
template<class Float3Type> ccl_device_inline Float3Type texco_remap_square(const Float3Type co)
{
  return (co - make_float3(0.5f, 0.5f, 0.5f)) * 2.0f;
}

template<class Float3Type>
ccl_device_inline auto svm_node_tex_image_mapping(const Float3Type co, const uint proj)
{
  if (proj == NODE_IMAGE_PROJ_SPHERE) {
    return map_to_sphere(texco_remap_square(co));
  }
  if (proj == NODE_IMAGE_PROJ_TUBE) {
    return map_to_tube(texco_remap_square(co));
  }

  return make_float2(co);
}

/* Rhino: the plain value of a coordinate that may or may not carry
 * derivatives. Upstream templated the image node on Float3Type; Rhino's decal
 * test only wants the value. */
ccl_device_inline float3 svm_texco_value(const float3 co)
{
  return co;
}
ccl_device_inline float3 svm_texco_value(const dual3 co)
{
  return co.val;
}

/* Rhino: fold x and y back on alternate tiles - see alternate_tile above. */
ccl_device_inline float3 svm_alternate_tiles(const float3 co)
{
  return make_float3(alternate_tile(co.x), alternate_tile(co.y), co.z);
}
ccl_device_inline dual3 svm_alternate_tiles(const dual3 co)
{
  /* A folded tile runs backwards, so flip the differentials on the folded axes.
   * Only the filter footprint depends on them, but getting the sign wrong there
   * blurs across the fold. */
  const float sx = (alternate_tile(co.val.x + 1e-4f) >= alternate_tile(co.val.x)) ? 1.0f : -1.0f;
  const float sy = (alternate_tile(co.val.y + 1e-4f) >= alternate_tile(co.val.y)) ? 1.0f : -1.0f;
  return dual3(svm_alternate_tiles(co.val),
               make_float3(co.dx.x * sx, co.dx.y * sy, co.dx.z),
               make_float3(co.dy.x * sx, co.dy.y * sy, co.dy.z));
}

template<class Float3Type>
ccl_device_noinline void svm_node_tex_image(KernelGlobals kg,
                                            ccl_private ShaderData *sd,
                                            ccl_private float *ccl_restrict stack,
                                            const ccl_global SVMNodeTexImage &ccl_restrict node)
{
  Float3Type co = stack_load<Float3Type>(stack, node.co);

  /* Rhino "Mirrored" repeat. Every other tile is folded back rather than
   * wrapped, which upstream has no notion of. Fold the value; on a folded tile
   * the coordinate runs backwards, so the derivatives change sign. */
  if (node.alternate_tiles != 0) {
    co = svm_alternate_tiles(co);
  }

  const dual2 tex_co(svm_node_tex_image_mapping(co, node.projection));

  float4 f = svm_image_texture(kg, sd, node.id, tex_co, node.flags);

  /* Rhino decals. rhino_texture_coordinate reports in the third component
   * whether the shading point falls inside the decal footprint - negative means
   * outside - and DecalUsage says this image is a decal at all. Without this the
   * decal image is sampled over the whole surface and its alpha never reaches
   * zero, so the decal art tiles across the material instead of sitting in its
   * own patch. */
  if (stack_valid(node.decal_usage_offset)) {
    const float decalusage = stack_load_float(stack, node.decal_usage_offset);
    if (decalusage > 0.0f && svm_texco_value(co).z < 0.0f) {
      f.w = 0.0f;
    }
  }

  if (stack_valid(node.out_offset)) {
    stack_store_float3(stack, node.out_offset, make_float3(f));
  }
  if (stack_valid(node.alpha_offset)) {
    stack_store_float(stack, node.alpha_offset, f.w);
  }
}

template<class Float3Type>
ccl_device_noinline void svm_node_tex_image_box(KernelGlobals kg,
                                                ccl_private ShaderData *sd,
                                                ccl_private float *ccl_restrict stack,
                                                const ccl_global SVMNodeTexImageBox &ccl_restrict
                                                    node)
{
  /* get object space normal */
  float3 N = sd->N;

  object_inverse_normal_transform(kg, sd, &N);

  /* project from direction vector to barycentric coordinates in triangles */
  const float3 signed_N = N;

  N = fabs(N);

  N /= (N.x + N.y + N.z);

  /* basic idea is to think of this as a triangle, each corner representing
   * one of the 3 faces of the cube. in the corners we have single textures,
   * in between we blend between two textures, and in the middle we a blend
   * between three textures.
   *
   * The `Nxyz` values are the barycentric coordinates in an equilateral
   * triangle, which in case of blending, in the middle has a smaller
   * equilateral triangle where 3 textures blend. this divides things into
   * 7 zones, with an `if()` test for each zone. */

  float3 weight = make_float3(0.0f, 0.0f, 0.0f);
  const float blend = node.blend;
  const float limit = 0.5f * (1.0f + blend);

  /* first test for corners with single texture */
  if (N.x > limit * (N.x + N.y) && N.x > limit * (N.x + N.z)) {
    weight.x = 1.0f;
  }
  else if (N.y > limit * (N.x + N.y) && N.y > limit * (N.y + N.z)) {
    weight.y = 1.0f;
  }
  else if (N.z > limit * (N.x + N.z) && N.z > limit * (N.y + N.z)) {
    weight.z = 1.0f;
  }
  else if (blend > 0.0f) {
    /* in case of blending, test for mixes between two textures */
    if (N.z < (1.0f - limit) * (N.y + N.x)) {
      weight.x = N.x / (N.x + N.y);
      weight.x = saturatef((weight.x - 0.5f * (1.0f - blend)) / blend);
      weight.y = 1.0f - weight.x;
    }
    else if (N.x < (1.0f - limit) * (N.y + N.z)) {
      weight.y = N.y / (N.y + N.z);
      weight.y = saturatef((weight.y - 0.5f * (1.0f - blend)) / blend);
      weight.z = 1.0f - weight.y;
    }
    else if (N.y < (1.0f - limit) * (N.x + N.z)) {
      weight.x = N.x / (N.x + N.z);
      weight.x = saturatef((weight.x - 0.5f * (1.0f - blend)) / blend);
      weight.z = 1.0f - weight.x;
    }
    else {
      /* last case, we have a mix between three */
      weight.x = ((2.0f - limit) * N.x + (limit - 1.0f)) / (2.0f * limit - 1.0f);
      weight.y = ((2.0f - limit) * N.y + (limit - 1.0f)) / (2.0f * limit - 1.0f);
      weight.z = ((2.0f - limit) * N.z + (limit - 1.0f)) / (2.0f * limit - 1.0f);
    }
  }
  else {
    /* Desperate mode, no valid choice anyway, fall back to one side. */
    weight.x = 1.0f;
  }

  /* now fetch textures */
  float4 f = zero_float4();

  const dual3 co = dual3(stack_load<Float3Type>(stack, node.co));

  /* Map so that no textures are flipped, rotation is somewhat arbitrary. */
  if (weight.x > 0.0f) {
    const dual2 uv = make_float2((signed_N.x < 0.0f) ? 1.0f - co.y() : co.y(), co.z());
    f += weight.x * svm_image_texture(kg, sd, node.id, uv, node.flags);
  }
  if (weight.y > 0.0f) {
    const dual2 uv = make_float2((signed_N.y > 0.0f) ? 1.0f - co.x() : co.x(), co.z());
    f += weight.y * svm_image_texture(kg, sd, node.id, uv, node.flags);
  }
  if (weight.z > 0.0f) {
    const dual2 uv = make_float2((signed_N.z > 0.0f) ? 1.0f - co.y() : co.y(), co.x());
    f += weight.z * svm_image_texture(kg, sd, node.id, uv, node.flags);
  }

  if (stack_valid(node.out_offset)) {
    stack_store_float3(stack, node.out_offset, make_float3(f.x, f.y, f.z));
  }
  if (stack_valid(node.alpha_offset)) {
    stack_store_float(stack, node.alpha_offset, f.w);
  }
}

template<class Float3Type>
ccl_device_inline auto svm_node_tex_environment_projection(Float3Type co, const uint proj)
{
  co = safe_normalize(co);
  if (proj == 0) {
    return direction_to_equirectangular(co);
  }
  return direction_to_mirrorball(co);
}

template<class Float3Type>
ccl_device_noinline void svm_node_tex_environment(
    KernelGlobals kg,
    ccl_private ShaderData *sd,
    ccl_private float *ccl_restrict stack,
    const ccl_global SVMNodeTexEnvironment &ccl_restrict node)
{
  const Float3Type co = stack_load<Float3Type>(stack, node.co);
  const dual2 uv(svm_node_tex_environment_projection(co, node.projection));

  const float4 f = svm_image_texture(kg, sd, node.id, uv, node.flags);

  if (stack_valid(node.out_offset)) {
    stack_store_float3(stack, node.out_offset, make_float3(f.x, f.y, f.z));
  }
  if (stack_valid(node.alpha_offset)) {
    stack_store_float(stack, node.alpha_offset, f.w);
  }
}

CCL_NAMESPACE_END
