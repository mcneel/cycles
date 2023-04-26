/* SPDX-License-Identifier: Apache-2.0
 * Copyright 2011-2022 Blender Foundation */

#pragma once

CCL_NAMESPACE_BEGIN

/* wcs_box_coord gives a Rhino-style WCS box texture coordinate mapping. */
ccl_device_inline void wcs_box_coord(KernelGlobals kg, ShaderData *sd, float3 *data)
{
  float3 N = sd->N;
  if (sd->object != OBJECT_NONE && kernel_data_fetch(objects, sd->object).use_ocs_frame) {
    Transform tfm = kernel_data_fetch(objects, sd->object).ocs_frame;
    *data = transform_point(&tfm, *data);
  }

  int side0 = 0;

  float dx = (*data).x;
  float dy = (*data).y;

  // set side0 = side closest to the point
  int side1 = (fabsf(dx) >= fabsf(dy)) ? 0 : 1;
  float rr = side1 ? dy : dx;
  if (fabsf((*data).z) > fabsf(rr))
    side1 = 2;

  float t1 = side1 ? dy : dx;
  if (t1 < 0.0f)
    side0 = 2 * side1 + 1;
  else
    side0 = 2 * side1 + 2;

  side1 = (fabsf(N.x) >= fabsf(N.y)) ? 0 : 1;
  rr = side1 ? N.y : N.x;
  if (fabsf(N.z) > fabsf(rr)) {
    side1 = 2;
  }

  switch (side1) {
    case 0: {
      t1 = N.x;
      break;
    }
    case 1: {
      t1 = N.y;
      break;
    }
    default: {
      t1 = N.z;
      break;
    }
  }
  if (0.0f != t1) {
    if (t1 < 0.0f)
      side0 = 2 * side1 + 1;
    else if (t1 > 0.0f)
      side0 = 2 * side1 + 2;
  }

  // side flag
  //  1 =  left side (x=-1)
  //  2 =  right side (x=+1)
  //  3 =  back side (y=-1)
  //  4 =  front side (y=+1)
  //  5 =  bottom side (z=-1)
  //  6 =  top side (z=+1)
  float3 v = make_float3(0.0f, 0.0f, 0.0f);
  switch (side0) {
    case 1:
      v.x = -(*data).y;
      v.y = (*data).z;
      v.z = (*data).x;
      break;
    case 2:
      v.x = (*data).y;
      v.y = (*data).z;
      v.z = (*data).x;
      break;
    case 3:
      v.x = (*data).x;
      v.y = (*data).z;
      v.z = (*data).y;
      break;
    case 4:
      v.x = -(*data).x;
      v.y = (*data).z;
      v.z = (*data).y;
      break;
    case 5:
      v.x = -(*data).x;
      v.y = (*data).y;
      v.z = (*data).z;
      break;
    case 6:
    default:
      v.x = (*data).x;
      v.y = (*data).y;
      v.z = (*data).z;
      break;
  }

  *data = v;
}

ccl_device_inline float3 get_reflected_incoming_ray(KernelGlobals kg, ShaderData *sd)
{
  float3 n = sd->N;
  float3 i = sd->wi;

  float3 refl = 2 * n * dot(i, n) - i;

  refl = normalize(refl);

  return refl;
}

/* Rhino Decal Data */
typedef struct DecalData {
  float radius;
  float height;
  float3 data;
  float4 sweeps;
  Transform pxyz;
  Transform nxyz;
  Transform uvw;
  bool on_correct_side;
  float3 cur_uv;
} DecalData;

ccl_device_inline float principal_value_angle_rad(float angleRad)
{
  if (angleRad >= M_2PI_F) {
    angleRad = angleRad - floorf(angleRad / M_2PI_F) * M_2PI_F;
  }
  else if (angleRad <= 0.0f) {
    angleRad = angleRad + ceilf(-angleRad / M_2PI_F) * M_2PI_F;
  }
  if (angleRad < 1e-12f)
    angleRad = 0.0f;
  if (angleRad > M_2PI_F)
    angleRad = M_2PI_F;
  return angleRad;
}

ccl_device_inline float3 map_to_uv(const float3 co, const DecalData decal)
{
  float3 ptUVW;
  float3 ptPoint = make_float3(decal.cur_uv.x, decal.cur_uv.y, 0.0f);
  // UV bounds are in sweeps
  if (decal.sweeps.x <= ptPoint.x && decal.sweeps.y >= ptPoint.x && decal.sweeps.z <= ptPoint.y &&
      decal.sweeps.w >= ptPoint.y) {
    ptUVW.x = (ptPoint.x - decal.sweeps.x) / (decal.sweeps.y - decal.sweeps.x);
    ptUVW.y = (ptPoint.y - decal.sweeps.z) / (decal.sweeps.w - decal.sweeps.z);
    ptUVW.z = 1.0f;
  }
  else {
    ptUVW.z = -1.0f;
  }

  return ptUVW;
}

ccl_device_inline float3 map_to_plane(const float3 co, const DecalData decal)
{
  float3 rst = transform_point(&decal.pxyz, co);
  rst.x = 0.5f * rst.x + 0.5f;
  rst.y = 0.5f * rst.y + 0.5f;

  rst = transform_point(&decal.uvw, rst);
  rst.z = 0.0f;

  return rst;
}

// sweeps.x/y = horizontal start/end
ccl_device_inline float3 map_to_cylinder_section(const float3 co, const DecalData decal)
{
  float len, u, v, hit, t, height;
  float4 sweeps;

  sweeps = decal.sweeps;
  height = decal.height;

  float3 rst = transform_point(&decal.pxyz, co);

  hit = u = v = t = 0.0f;
  len = sqrtf(rst.x * rst.x + rst.y * rst.y);
  if (len > 0.0f) {
    t = ((rst.x != 0.0f || rst.y != 0.0f) ? atan2f(rst.y, rst.x) : 0.0f);
    rst.x = 0.5 * M_1_PI_F * t;
    if (rst.x < 0.0f) {
      rst.x += 1.0f;
    }

    rst.x = clamp(rst.x, 0.0f, 1.0f);
    rst.y = 0.5f * /*(1.0f / height) **/ rst.z + 0.5f;
    rst.z = len;

    rst = transform_point(&decal.uvw, rst);

    u = rst.x;
    v = rst.y;

    float pt_longitude = u * M_2PI_F;

    float sta_longitude = sweeps.x, end_longitude = sweeps.y;

    // Longitudes relative to decal start longitudeitude
    float rel_decal_sta_longitude = 0.0;
    float rel_decal_end_longitude = principal_value_angle_rad(end_longitude - sta_longitude);
    float rel_pt_longitude = principal_value_angle_rad(pt_longitude - sta_longitude);

    // If end and start longitudeitudes have same value then decal is supposed to go around the
    // cylinder
    if (rel_decal_end_longitude == 0.0)
      rel_decal_end_longitude = M_2PI_F;

    if (rel_decal_sta_longitude <= rel_pt_longitude &&
        rel_decal_end_longitude >= rel_pt_longitude) {
      // Scale longitudeitude to texture u-coordinate
      u = rel_pt_longitude / rel_decal_end_longitude;
      if (u >= 0.0 && u <= 1.0 && v >= 0.0 && v <= 1.0) {
        hit = 1.0f;
      }
      else {
        u = v = 0.0f;
        hit = -1.0f;
      }
    }
    else {
      u = v = 0.0f;
      hit = -1.0f;
    }
  }
  else {
    u = v = 0.0f;
    hit = -1.0f;
  }
  /*
  if(hit>0.0f) {
    u = 0.0f;
    v = 1.0f;
    hit = 0.0f;
  } else {
    u = 1.0f;
    v = 0.0f;
    hit = 0.0f;
  }*/
  return make_float3(u, v, hit);
}

ccl_device_inline float3 map_to_sphere_section(const float3 co, const DecalData decal)
{
  float r, len, u, v, longitude, latitude, hit;
  float4 sweeps;

  float3 rst = transform_point(&decal.pxyz, co);

  sweeps = decal.sweeps;

  len = u = v = longitude = latitude = hit = 0.0f;

  r = sqrtf(rst.x * rst.x + rst.y * rst.y + rst.z * rst.z);
  len = sqrtf(rst.x * rst.x + rst.y * rst.y);
  if (len > 0.0f) {

    longitude = (rst.y != 0.0f || rst.x != 0.0f) ? atan2f(rst.y, rst.x) : 0.0f;
    latitude = (rst.z != 0.0f) ? atan2f(rst.z, len) : 0.0f;

    if (latitude > M_PI_F) {
      latitude -= M_2PI_F;
    }

    rst.x = 0.5f * longitude * M_1_PI_F;
    if (rst.x < 1e-12f) {
      rst.x += 1.0f;
    }
    if (rst.x < 0.0f) {
      rst.x = 0.0f;
    }
    else if (rst.x > 1.0f) {
      rst.x = 1.0f;
    }

    rst.y = clamp(M_1_PI_F * latitude + 0.5f, 0.0f, 1.0f);

    rst.z = r;

    rst = transform_point(&decal.uvw, rst);

    u = rst.x;
    v = rst.y;

    // Map u rstordinate to longitude
    float point_longitude = u * M_2PI_F;

    float long_start = sweeps.x;
    float long_end = sweeps.y;
    float lat_start = sweeps.z;
    float lat_end = sweeps.w;

    // Longitudes relative to decal start longitude
    float rel_decal_end_longitude = principal_value_angle_rad(long_end - long_start);
    float rel_point_longitude = principal_value_angle_rad(point_longitude - long_start);

    // If end and start longitudes have same value then decal is supposed to go full circle
    // around the vertical axis
    if (rel_decal_end_longitude == 0.0f)
      rel_decal_end_longitude = M_2PI_F;

    float v_start = M_1_PI_F * lat_start + 0.5f;
    float v_end = M_1_PI_F * lat_end + 0.5f;

    if (rel_decal_end_longitude >= rel_point_longitude && v_start <= v && v_end >= v) {
      u = rel_point_longitude / rel_decal_end_longitude;
      v = (v - v_start) / (v_end - v_start);
      hit = 1.0f;
    }
    else {
      u = v = 0.0f;
      hit = -1.0f;
    }
  }
  else {
    u = v = 0.0f;
    hit = -1.0f;
  }

  return make_float3(u, v, hit);
}

ccl_device_inline void decal_data_read(KernelGlobals kg,
                                       ShaderData *sd,
                                       float *stack,
                                       uint4 node,
                                       int *offset,
                                       DecalData *decal,
                                       int derivative)
{
  uint decal_forward_offset, decal_usage_offset;
  uint decal_direction;
  uint decal_map_side;
  uint type = node.y;
  uint out_offset = node.z;

  float3 uv = stack_load_float3(stack, out_offset);

  // float decalforward;
  svm_unpack_node_uchar2(node.w, &decal_forward_offset, &decal_usage_offset);
  float3 data;
  if (derivative == 0) {
    data = sd->P;
  }
  else if (derivative == 1) {
    const differential3 dP = differential_from_compact(sd->Ng, sd->dP);
    data = sd->P + dP.dx;
  }
  else if (derivative == 2) {
    const differential3 dP = differential_from_compact(sd->Ng, sd->dP);
    data = sd->P + dP.dy;
  }
  Transform tfm, itfm, pxyz, nxyz, uvw;
  float3 n = sd->N;
  tfm.x = read_node_float(kg, offset);
  tfm.y = read_node_float(kg, offset);
  tfm.z = read_node_float(kg, offset);
  itfm.x = read_node_float(kg, offset);
  itfm.y = read_node_float(kg, offset);
  itfm.z = read_node_float(kg, offset);
  pxyz.x = read_node_float(kg, offset);
  pxyz.y = read_node_float(kg, offset);
  pxyz.z = read_node_float(kg, offset);
  nxyz.x = read_node_float(kg, offset);
  nxyz.y = read_node_float(kg, offset);
  nxyz.z = read_node_float(kg, offset);
  uvw.x = read_node_float(kg, offset);
  uvw.y = read_node_float(kg, offset);
  uvw.z = read_node_float(kg, offset);

  decal->pxyz = pxyz;
  decal->nxyz = nxyz;
  decal->uvw = uvw;

  uint4 node2 = read_node(kg, offset);
  svm_unpack_node_uchar2(node2.x, &decal_map_side, &decal_direction);

  decal->radius = __int_as_float(node2.y);
  decal->height = __int_as_float(node2.z);  //  *0.5f;
  decal->sweeps = read_node_float(kg, offset);

  float4 origin4 = read_node_float(kg, offset);
  float3 origin = make_float3(origin4.x, origin4.y, origin4.z);

  float4 across4 = read_node_float(kg, offset);
  float3 across = make_float3(across4.x, across4.y, across4.z);

  float4 decal_up4 = read_node_float(kg, offset);
  float3 decal_up = make_float3(decal_up4.x, decal_up4.y, decal_up4.z);

  decal->data = data;

  if (type == NODE_TEXCO_ENV_DECAL_UV) {
    decal->cur_uv.x = uv.x;
    decal->cur_uv.y = uv.y;
    decal->data = map_to_uv(data, *decal);
  }
  else {

    // transform_direction(&itfm, n);
    // n = transform_direction(&decal->nxyz, n);
    float3 crossp = cross(across, decal_up);
    float dotp = dot(crossp, n);
    bool inside = dot(n, decal->data - origin) < 0.0f;

    // decal->data = transform_point(&itfm, data);

    decal->on_correct_side = false;

    if (decal_direction == NODE_IMAGE_DECAL_BOTH || type == NODE_TEXCO_ENV_DECAL_UV) {
      decal->on_correct_side = true;
    }
    else if (decal_direction == NODE_IMAGE_DECAL_FORWARD) {
      switch (type) {
        case NODE_TEXCO_ENV_DECAL_PLANAR: {
          decal->on_correct_side = dotp > 0.0f;
        } break;
        case NODE_TEXCO_ENV_DECAL_SPHERICAL:
        case NODE_TEXCO_ENV_DECAL_CYLINDRICAL: {
          decal->on_correct_side = !inside;
        } break;
      }
    }
    else {  // forward
      switch (type) {
        case NODE_TEXCO_ENV_DECAL_PLANAR: {
          decal->on_correct_side = dotp < 0.0f;
        } break;
        case NODE_TEXCO_ENV_DECAL_SPHERICAL:
        case NODE_TEXCO_ENV_DECAL_CYLINDRICAL: {
          decal->on_correct_side = inside;
        } break;
      }
    }
  }

  // stack_store_float(stack, decal_forward_offset, dotp > 0.0f ? 1.0f : -1.0f);
  stack_store_float(stack, decal_usage_offset, 1.0f);
}

/* Texture Coordinate Node */

ccl_device_noinline int svm_node_tex_coord(KernelGlobals kg,
                                           ccl_private ShaderData *sd,
                                           uint32_t path_flag,
                                           ccl_private float *stack,
                                           uint4 node,
                                           int offset)
{
  float3 data = zero_float3();
  uint type = node.y;
  uint out_offset = node.z;

  switch (type) {
    case NODE_TEXCO_OBJECT: {
      data = sd->P;
      if (node.w == 0) {
        if (sd->object != OBJECT_NONE) {
          object_inverse_position_transform(kg, sd, &data);
        }
      }
      else {
        Transform tfm;
        tfm.x = read_node_float(kg, &offset);
        tfm.y = read_node_float(kg, &offset);
        tfm.z = read_node_float(kg, &offset);
        data = transform_point(&tfm, data);
      }
      break;
    }
    case NODE_TEXCO_NORMAL: {
      data = sd->N;
      object_inverse_normal_transform(kg, sd, &data);
      break;
    }
    case NODE_TEXCO_CAMERA: {
      Transform tfm = kernel_data.cam.worldtocamera;

      if (sd->object != OBJECT_NONE)
        data = transform_point(&tfm, sd->P);
      else
        data = transform_point(&tfm, sd->P + camera_position(kg));
      break;
    }
    case NODE_TEXCO_WINDOW: {
      if ((path_flag & PATH_RAY_CAMERA) && sd->object == OBJECT_NONE &&
          kernel_data.cam.type == CAMERA_ORTHOGRAPHIC)
        data = camera_world_to_ndc(kg, sd, sd->ray_P);
      else
        data = camera_world_to_ndc(kg, sd, sd->P);
      data.z = 0.0f;
      break;
    }
    case NODE_TEXCO_REFLECTION: {
      if (sd->object != OBJECT_NONE)
        data = 2.0f * dot(sd->N, sd->wi) * sd->N - sd->wi;
      else
        data = sd->wi;
      break;
    }
    case NODE_TEXCO_DUPLI_GENERATED: {
      data = object_dupli_generated(kg, sd->object);
      break;
    }
    case NODE_TEXCO_DUPLI_UV: {
      data = object_dupli_uv(kg, sd->object);
      break;
    }
    case NODE_TEXCO_VOLUME_GENERATED: {
      data = sd->P;

#ifdef __VOLUME__
      if (sd->object != OBJECT_NONE)
        data = volume_normalized_position(kg, sd, data);
#endif
      break;
    }
  }

  stack_store_float3(stack, out_offset, data);
  return offset;
}

/* Rhino Texture Coordinate Node */

ccl_device_noinline int svm_rhino_node_tex_coord(KernelGlobals kg,
                                                 ccl_private ShaderData *sd,
                                                 uint32_t path_flag,
                                                 ccl_private float *stack,
                                                 uint4 node,
                                                 int offset)
{
  float3 data = zero_float3();
  uint type = node.y;
  uint out_offset = node.z;

  switch (type) {
    case NODE_TEXCO_OBJECT: {
      data = sd->P;
      if (sd->object != OBJECT_NONE && kernel_data_fetch(objects, sd->object).use_ocs_frame) {
        Transform tfm = kernel_data_fetch(objects, sd->object).ocs_frame;
        data = transform_point(&tfm, data);
      }
      if (node.w == 0) {
        if (sd->object != OBJECT_NONE) {
          object_inverse_position_transform(kg, sd, &data);
        }
      }
      else {
        Transform tfm;
        tfm.x = read_node_float(kg, &offset);
        tfm.y = read_node_float(kg, &offset);
        tfm.z = read_node_float(kg, &offset);
        data = transform_point(&tfm, data);
      }
      break;
    }
    case NODE_TEXCO_NORMAL: {
      data = sd->N;
      object_inverse_normal_transform(kg, sd, &data);
      break;
    }
    case NODE_TEXCO_CAMERA: {
      Transform tfm = kernel_data.cam.worldtocamera;

      if (sd->object != OBJECT_NONE)
        data = transform_direction(&tfm, sd->P);
      else
        data = transform_direction(&tfm, sd->P + camera_position(kg));
      break;
    }
    case NODE_TEXCO_WINDOW: {
      if ((path_flag & PATH_RAY_CAMERA) && sd->object == OBJECT_NONE &&
          kernel_data.cam.type == CAMERA_ORTHOGRAPHIC)
        data = camera_world_to_ndc(kg, sd, sd->ray_P);
      else
        data = camera_world_to_ndc(kg, sd, sd->P);
      data.z = 0.0f;
      break;
    }
    case NODE_TEXCO_REFLECTION: {
      if (sd->object != OBJECT_NONE)
        data = 2.0f * dot(sd->N, sd->wi) * sd->N - sd->wi;
      else
        data = sd->wi;
      break;
    }
    case NODE_TEXCO_DUPLI_GENERATED: {
      data = object_dupli_generated(kg, sd->object);
      break;
    }
    case NODE_TEXCO_DUPLI_UV: {
      data = object_dupli_uv(kg, sd->object);
      break;
    }
    case NODE_TEXCO_VOLUME_GENERATED: {
      data = sd->P;

#ifdef __VOLUME__
      if (sd->object != OBJECT_NONE)
        data = volume_normalized_position(kg, sd, data);
#endif
      break;
    }
    case NODE_TEXCO_WCS_BOX: {
      data = sd->P;
      if (node.w == 0) {
        if (sd->object != OBJECT_NONE) {
          object_inverse_position_transform(kg, sd, &data);
        }
      }
      else {
        Transform tfm;
        tfm.x = read_node_float(kg, &offset);
        tfm.y = read_node_float(kg, &offset);
        tfm.z = read_node_float(kg, &offset);
        data = transform_direction(&tfm, data);
      }
      wcs_box_coord(kg, sd, &data);
      break;
    }
    case NODE_TEXCO_ENV_SPHERICAL: {
      data = get_reflected_incoming_ray(kg, sd);
      data = make_float3(data.y, -data.z, -data.x);
      data = env_spherical(data);
      break;
    }
    case NODE_TEXCO_ENV_EMAP: {
      data = get_reflected_incoming_ray(kg, sd);
      Transform tfm = kernel_data.cam.worldtocamera;
      data = transform_direction(&tfm, data);
      // rhino camera scaled 1, -1, -1, so take that into account
      data = make_float3(data.x, -data.y, -data.z);
      data = env_world_emap(data);
      break;
    }
    case NODE_TEXCO_ENV_BOX: {
      data = get_reflected_incoming_ray(kg, sd);
      data = env_box(data);
      break;
    }
    case NODE_TEXCO_ENV_LIGHTPROBE: {
      data = get_reflected_incoming_ray(kg, sd);
      data = env_light_probe(data);
      break;
    }
    case NODE_TEXCO_ENV_CUBEMAP: {
      data = get_reflected_incoming_ray(kg, sd);
      data = env_cubemap(data);
      break;
    }
    case NODE_TEXCO_ENV_CUBEMAP_VERTICAL_CROSS: {
      data = get_reflected_incoming_ray(kg, sd);
      data = env_cubemap_vertical_cross(data);
      break;
    }
    case NODE_TEXCO_ENV_CUBEMAP_HORIZONTAL_CROSS: {
      data = get_reflected_incoming_ray(kg, sd);
      data = env_cubemap_horizontal_cross(data);
      break;
    }
    case NODE_TEXCO_ENV_HEMI: {
      data = get_reflected_incoming_ray(kg, sd);
      data = make_float3(data.y, -data.z, -data.x);
      data = env_hemispherical(data);
      break;
    }
    case NODE_TEXCO_ENV_DECAL_UV: {
      DecalData decal;
      decal_data_read(kg, sd, stack, node, &offset, &decal, 0);
      data = decal.data;
      // data = map_to_uv(data, decal);
      break;
    }
    case NODE_TEXCO_ENV_DECAL_PLANAR: {
      DecalData decal;
      decal_data_read(kg, sd, stack, node, &offset, &decal, 0);
      data = decal.data;
      data = map_to_plane(data, decal);
      if (!(data.x >= 0.0f && data.x <= 1.0f && data.y >= 0 && data.y <= 1.0f) ||
          !decal.on_correct_side) {
        data.z = -1.0f;
      }
      break;
    }
    case NODE_TEXCO_ENV_DECAL_SPHERICAL: {
      DecalData decal;
      decal_data_read(kg, sd, stack, node, &offset, &decal, 0);
      data = decal.data;
      data = map_to_sphere_section(data, decal);
      if (!decal.on_correct_side) {
        data.z = -1.0f;
      }
      break;
    }
    case NODE_TEXCO_ENV_DECAL_CYLINDRICAL: {
      DecalData decal;
      decal_data_read(kg, sd, stack, node, &offset, &decal, 0);
      data = decal.data;
      data = map_to_cylinder_section(data, decal);
      if (!decal.on_correct_side) {
        data.z = -1.0f;
      }
      break;
    }
  }

  stack_store_float3(stack, out_offset, data);
  return offset;
}

ccl_device_noinline int svm_node_tex_coord_bump_dx(KernelGlobals kg,
                                                   ccl_private ShaderData *sd,
                                                   uint32_t path_flag,
                                                   ccl_private float *stack,
                                                   uint4 node,
                                                   int offset)
{
#ifdef __RAY_DIFFERENTIALS__
  float3 data = zero_float3();
  uint type = node.y;
  uint out_offset = node.z;

  switch (type) {
    case NODE_TEXCO_OBJECT: {
      data = svm_node_bump_P_dx(sd);
      if (node.w == 0) {
        if (sd->object != OBJECT_NONE) {
          object_inverse_position_transform(kg, sd, &data);
        }
      }
      else {
        Transform tfm;
        tfm.x = read_node_float(kg, &offset);
        tfm.y = read_node_float(kg, &offset);
        tfm.z = read_node_float(kg, &offset);
        data = transform_point(&tfm, data);
      }
      break;
    }
    case NODE_TEXCO_NORMAL: {
      data = sd->N;
      object_inverse_normal_transform(kg, sd, &data);
      break;
    }
    case NODE_TEXCO_CAMERA: {
      Transform tfm = kernel_data.cam.worldtocamera;

      if (sd->object != OBJECT_NONE)
        data = transform_point(&tfm, svm_node_bump_P_dx(sd));
      else
        data = transform_point(&tfm, svm_node_bump_P_dx(sd) + camera_position(kg));
      break;
    }
    case NODE_TEXCO_WINDOW: {
      if ((path_flag & PATH_RAY_CAMERA) && sd->object == OBJECT_NONE &&
          kernel_data.cam.type == CAMERA_ORTHOGRAPHIC)
        data = camera_world_to_ndc(kg, sd, sd->ray_P);
      else
        data = camera_world_to_ndc(kg, sd, svm_node_bump_P_dx(sd));
      data.z = 0.0f;
      break;
    }
    case NODE_TEXCO_REFLECTION: {
      if (sd->object != OBJECT_NONE)
        data = 2.0f * dot(sd->N, sd->wi) * sd->N - sd->wi;
      else
        data = sd->wi;
      break;
    }
    case NODE_TEXCO_DUPLI_GENERATED: {
      data = object_dupli_generated(kg, sd->object);
      break;
    }
    case NODE_TEXCO_DUPLI_UV: {
      data = object_dupli_uv(kg, sd->object);
      break;
    }
    case NODE_TEXCO_VOLUME_GENERATED: {
      data = svm_node_bump_P_dx(sd);

#  ifdef __VOLUME__
      if (sd->object != OBJECT_NONE)
        data = volume_normalized_position(kg, sd, data);
#  endif
      break;
    }
  }

  stack_store_float3(stack, out_offset, data);
  return offset;
#else
  return svm_node_tex_coord(kg, sd, path_flag, stack, node, offset);
#endif
}

ccl_device_noinline int svm_node_tex_coord_bump_dy(KernelGlobals kg,
                                                   ccl_private ShaderData *sd,
                                                   uint32_t path_flag,
                                                   ccl_private float *stack,
                                                   uint4 node,
                                                   int offset)
{
#ifdef __RAY_DIFFERENTIALS__
  float3 data = zero_float3();
  uint type = node.y;
  uint out_offset = node.z;

  switch (type) {
    case NODE_TEXCO_OBJECT: {
      data = svm_node_bump_P_dy(sd);
      if (node.w == 0) {
        if (sd->object != OBJECT_NONE) {
          object_inverse_position_transform(kg, sd, &data);
        }
      }
      else {
        Transform tfm;
        tfm.x = read_node_float(kg, &offset);
        tfm.y = read_node_float(kg, &offset);
        tfm.z = read_node_float(kg, &offset);
        data = transform_point(&tfm, data);
      }
      break;
    }
    case NODE_TEXCO_NORMAL: {
      data = sd->N;
      object_inverse_normal_transform(kg, sd, &data);
      break;
    }
    case NODE_TEXCO_CAMERA: {
      Transform tfm = kernel_data.cam.worldtocamera;

      if (sd->object != OBJECT_NONE)
        data = transform_point(&tfm, svm_node_bump_P_dy(sd));
      else
        data = transform_point(&tfm, svm_node_bump_P_dy(sd) + camera_position(kg));
      break;
    }
    case NODE_TEXCO_WINDOW: {
      if ((path_flag & PATH_RAY_CAMERA) && sd->object == OBJECT_NONE &&
          kernel_data.cam.type == CAMERA_ORTHOGRAPHIC)
        data = camera_world_to_ndc(kg, sd, sd->ray_P);
      else
        data = camera_world_to_ndc(kg, sd, svm_node_bump_P_dy(sd));
      data.z = 0.0f;
      break;
    }
    case NODE_TEXCO_REFLECTION: {
      if (sd->object != OBJECT_NONE)
        data = 2.0f * dot(sd->N, sd->wi) * sd->N - sd->wi;
      else
        data = sd->wi;
      break;
    }
    case NODE_TEXCO_DUPLI_GENERATED: {
      data = object_dupli_generated(kg, sd->object);
      break;
    }
    case NODE_TEXCO_DUPLI_UV: {
      data = object_dupli_uv(kg, sd->object);
      break;
    }
    case NODE_TEXCO_VOLUME_GENERATED: {
      data = svm_node_bump_P_dy(sd);

#  ifdef __VOLUME__
      if (sd->object != OBJECT_NONE)
        data = volume_normalized_position(kg, sd, data);
#  endif
      break;
    }
  }

  stack_store_float3(stack, out_offset, data);
  return offset;
#else
  return svm_node_tex_coord(kg, sd, path_flag, stack, node, offset);
#endif
}

ccl_device_noinline void svm_node_normal_map(KernelGlobals kg,
                                             ccl_private ShaderData *sd,
                                             ccl_private float *stack,
                                             uint4 node)
{
  uint color_offset, strength_offset, normal_offset, space;
  svm_unpack_node_uchar4(node.y, &color_offset, &strength_offset, &normal_offset, &space);

  float3 color = stack_load_float3(stack, color_offset);
  color = 2.0f * make_float3(color.x - 0.5f, color.y - 0.5f, color.z - 0.5f);

  bool is_backfacing = (sd->flag & SD_BACKFACING) != 0;
  float3 N;

  if (space == NODE_NORMAL_MAP_TANGENT) {
    /* tangent space */
    if (sd->object == OBJECT_NONE || (sd->type & PRIMITIVE_TRIANGLE) == 0) {
      /* Fallback to unperturbed normal. */
      stack_store_float3(stack, normal_offset, sd->N);
      return;
    }

    /* first try to get tangent attribute */
    const AttributeDescriptor attr = find_attribute(kg, sd, node.z);
    const AttributeDescriptor attr_sign = find_attribute(kg, sd, node.w);

    if (attr.offset == ATTR_STD_NOT_FOUND || attr_sign.offset == ATTR_STD_NOT_FOUND) {
      /* Fallback to unperturbed normal. */
      stack_store_float3(stack, normal_offset, sd->N);
      return;
    }

    /* get _unnormalized_ interpolated normal and tangent */
    float3 tangent = primitive_surface_attribute_float3(kg, sd, attr, NULL, NULL);
    float sign = primitive_surface_attribute_float(kg, sd, attr_sign, NULL, NULL);
    float3 normal;

    if (sd->shader & SHADER_SMOOTH_NORMAL) {
      normal = triangle_smooth_normal_unnormalized(kg, sd, sd->Ng, sd->prim, sd->u, sd->v);
    }
    else {
      normal = sd->Ng;

      /* the normal is already inverted, which is too soon for the math here */
      if (is_backfacing) {
        normal = -normal;
      }

      object_inverse_normal_transform(kg, sd, &normal);
    }

    /* apply normal map */
    float3 B = sign * cross(normal, tangent);
    N = safe_normalize(color.x * tangent + color.y * B + color.z * normal);

    /* transform to world space */
    object_normal_transform(kg, sd, &N);
  }
  else {
    /* strange blender convention */
    if (space == NODE_NORMAL_MAP_BLENDER_OBJECT || space == NODE_NORMAL_MAP_BLENDER_WORLD) {
      color.y = -color.y;
      color.z = -color.z;
    }

    /* object, world space */
    N = color;

    if (space == NODE_NORMAL_MAP_OBJECT || space == NODE_NORMAL_MAP_BLENDER_OBJECT)
      object_normal_transform(kg, sd, &N);
    else
      N = safe_normalize(N);
  }

  /* invert normal for backfacing polygons */
  if (is_backfacing) {
    N = -N;
  }

  float strength = stack_load_float(stack, strength_offset);

  if (strength != 1.0f) {
    strength = max(strength, 0.0f);
    N = safe_normalize(sd->N + (N - sd->N) * strength);
  }

  if (is_zero(N)) {
    N = sd->N;
  }

  stack_store_float3(stack, normal_offset, N);
}

ccl_device_noinline void svm_node_tangent(KernelGlobals kg,
                                          ccl_private ShaderData *sd,
                                          ccl_private float *stack,
                                          uint4 node)
{
  uint tangent_offset, direction_type, axis;
  svm_unpack_node_uchar3(node.y, &tangent_offset, &direction_type, &axis);

  float3 tangent;
  float3 attribute_value;
  const AttributeDescriptor desc = find_attribute(kg, sd, node.z);
  if (desc.offset != ATTR_STD_NOT_FOUND) {
    if (desc.type == NODE_ATTR_FLOAT2) {
      float2 value = primitive_surface_attribute_float2(kg, sd, desc, NULL, NULL);
      attribute_value.x = value.x;
      attribute_value.y = value.y;
      attribute_value.z = 0.0f;
    }
    else {
      attribute_value = primitive_surface_attribute_float3(kg, sd, desc, NULL, NULL);
    }
  }

  if (direction_type == NODE_TANGENT_UVMAP) {
    /* UV map */
    if (desc.offset == ATTR_STD_NOT_FOUND) {
      stack_store_float3(stack, tangent_offset, zero_float3());
      return;
    }
    else {
      tangent = attribute_value;
    }
  }
  else {
    /* radial */
    float3 generated;

    if (desc.offset == ATTR_STD_NOT_FOUND)
      generated = sd->P;
    else
      generated = attribute_value;

    if (axis == NODE_TANGENT_AXIS_X)
      tangent = make_float3(0.0f, -(generated.z - 0.5f), (generated.y - 0.5f));
    else if (axis == NODE_TANGENT_AXIS_Y)
      tangent = make_float3(-(generated.z - 0.5f), 0.0f, (generated.x - 0.5f));
    else
      tangent = make_float3(-(generated.y - 0.5f), (generated.x - 0.5f), 0.0f);
  }

  object_normal_transform(kg, sd, &tangent);
  tangent = cross(sd->N, normalize(cross(tangent, sd->N)));
  stack_store_float3(stack, tangent_offset, tangent);
}

CCL_NAMESPACE_END
