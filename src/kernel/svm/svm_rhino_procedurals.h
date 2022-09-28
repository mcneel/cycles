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

ccl_device bool is_odd(int x)
{
  return (x & 1) == 1;
}

/* Checker Texture 2D */

ccl_device float4 checker_texture_2d(float3 uvw,
                                     const Transform *mapping,
                                     float4 color1,
                                     float4 color2)
{
  float4 color_out = make_float4(0.0f, 0.0f, 0.0f, 1.0f);

  uvw = transform_point(mapping, uvw);

  int u_cell = int(floorf(uvw.x));
  int v_cell = int(floorf(uvw.y));

  if (is_odd(u_cell + v_cell)) {
    color_out = color1;
  }
  else {
    color_out = color2;
  }

  return color_out;
}

ccl_device void svm_rhino_node_checker_texture_2d(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, in_color1_offset, in_color2_offset, out_color_offset;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &in_color1_offset, &in_color2_offset, &out_color_offset);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);
  float3 color1 = stack_load_float3(stack, in_color1_offset);
  float3 color2 = stack_load_float3(stack, in_color2_offset);

  Transform uvw_transform;
  uvw_transform.x = read_node_float(kg, offset);
  uvw_transform.y = read_node_float(kg, offset);
  uvw_transform.z = read_node_float(kg, offset);

  float4 out_color = checker_texture_2d(uvw,
                                        &uvw_transform,
                                        make_float4(color1.x, color1.y, color1.z, 1.0f),
                                        make_float4(color2.x, color2.y, color2.z, 1.0f));

  if (stack_valid(out_color_offset))
    stack_store_float3(
        stack, out_color_offset, make_float3(out_color.x, out_color.y, out_color.z));
}

/* Noise */

ccl_device float bias_func(float x, float b)
{
  float smallv = 1e-6;
  if (b < smallv)
    b = smallv;

  if (x < 0.0f)
    x = 0.0f;

  float div = log(b) / log(0.5f);

  if (x == 0.0f && div == 0.0f)
    return 1.0f;

  return pow(x, div);
}

ccl_device float gain_func(float x, float gain)
{
  gain = 1.0f - gain;

  if (x < 0.5f) {
    float bias = bias_func(2.0f * x, gain);

    return bias * 0.5f;
  }

  float bias = bias_func(2.0f - 2.0f * x, gain);

  return 1.0f - (bias * 0.5f);
}

ccl_device int perlin_noise(KernelGlobals *kg, int x)
{
  int perlin_noise_value = (int)kernel_tex_fetch(__lookup_table,
                                                 kernel_data.tables.rhino_perlin_noise_offset + x);

  return perlin_noise_value;
}

ccl_device float gradient(int h, float dx, float dy, float dz)
{
  if (bool(h & 1)) {
    if (bool(h & 2)) {
      float u = h < 8 ? dx : dy;
      float v = h < 4 ? dy : dz;
      return -u - v;
    }
    else {
      float u = h < 8 || h == 13 ? dx : dy;
      float v = h < 4 || h == 13 ? dy : dz;
      return -u + v;
    }
  }
  else {
    if (bool(h & 2)) {
      float u = h < 8 ? dx : dy;
      float v = h < 4 ? dy : dz;
      return u - v;
    }
    else {
      float u = h < 8 || h == 12 ? dx : dy;
      float v = h < 4 || h == 12 ? dy : dz;
      return u + v;
    }
  }
}

ccl_device float noise_weight(float t)
{
  return t * t * t * (10.0f + t * (6.0f * t - 15.0f));
}

ccl_device float noise(KernelGlobals *kg, float x, float y, float z)
{
  float cx = floorf(x);
  float cy = floorf(y);
  float cz = floorf(z);

  float dx = x - cx;
  float dy = y - cy;
  float dz = z - cz;

  int ix = int(cx) & (RHINO_PERLIN_NOISE_PERM_SIZE - 1);
  int iy = int(cy) & (RHINO_PERLIN_NOISE_PERM_SIZE - 1);
  int iz = int(cz) & (RHINO_PERLIN_NOISE_PERM_SIZE - 1);

  int h0 = perlin_noise(kg, ix);
  int h1 = perlin_noise(kg, ix + 1);
  int h00 = perlin_noise(kg, h0 + iy) + iz;
  int h01 = perlin_noise(kg, h1 + iy) + iz;
  int h10 = perlin_noise(kg, h0 + iy + 1) + iz;
  int h11 = perlin_noise(kg, h1 + iy + 1) + iz;

  int h000 = perlin_noise(kg, h00) & 15;
  int h001 = perlin_noise(kg, h01) & 15;
  int h010 = perlin_noise(kg, h10) & 15;
  int h011 = perlin_noise(kg, h11) & 15;
  int h100 = perlin_noise(kg, h00 + 1) & 15;
  int h101 = perlin_noise(kg, h01 + 1) & 15;
  int h110 = perlin_noise(kg, h10 + 1) & 15;
  int h111 = perlin_noise(kg, h11 + 1) & 15;

  float w000 = gradient(h000, dx, dy, dz);
  float w100 = gradient(h001, dx - 1, dy, dz);
  float w010 = gradient(h010, dx, dy - 1, dz);
  float w110 = gradient(h011, dx - 1, dy - 1, dz);
  float w001 = gradient(h100, dx, dy, dz - 1);
  float w101 = gradient(h101, dx - 1, dy, dz - 1);
  float w011 = gradient(h110, dx, dy - 1, dz - 1);
  float w111 = gradient(h111, dx - 1, dy - 1, dz - 1);

  float wx = noise_weight(dx);
  float wy = noise_weight(dy);
  float wz = noise_weight(dz);

  float y0 = w000 + wx * (w100 - w000 + wy * (w110 - w010 + w000 - w100)) + wy * (w010 - w000);
  float y1 = w001 + wx * (w101 - w001 + wy * (w111 - w011 + w001 - w101)) + wy * (w011 - w001);

  return y0 + wz * (y1 - y0);
}

ccl_device float value_noise(KernelGlobals *kg, float x, float y, float z)
{
  float cx = floorf(x);
  float cy = floorf(y);
  float cz = floorf(z);

  float dx = x - cx;
  float dy = y - cy;
  float dz = z - cz;

  int ix = int(cx);
  int iy = int(cy);
  int iz = int(cz);

  /* clang-format off */
  float prn000 = perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, (ix + 0) & 255) + ((iy + 0) & 255)) + ((iz + 0) & 255)));
  float prn001 = perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, (ix + 0) & 255) + ((iy + 0) & 255)) + ((iz + 1) & 255)));
  float prn010 = perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, (ix + 0) & 255) + ((iy + 1) & 255)) + ((iz + 0) & 255)));
  float prn011 = perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, (ix + 0) & 255) + ((iy + 1) & 255)) + ((iz + 1) & 255)));
  float prn100 = perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, (ix + 1) & 255) + ((iy + 0) & 255)) + ((iz + 0) & 255)));
  float prn101 = perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, (ix + 1) & 255) + ((iy + 0) & 255)) + ((iz + 1) & 255)));
  float prn110 = perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, (ix + 1) & 255) + ((iy + 1) & 255)) + ((iz + 0) & 255)));
  float prn111 = perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, perlin_noise(kg, (ix + 1) & 255) + ((iy + 1) & 255)) + ((iz + 1) & 255)));
  /* clang-format on */

  prn000 = -1.0f + 2.0f * (prn000 / 255.0f);
  prn001 = -1.0f + 2.0f * (prn001 / 255.0f);
  prn010 = -1.0f + 2.0f * (prn010 / 255.0f);
  prn011 = -1.0f + 2.0f * (prn011 / 255.0f);
  prn100 = -1.0f + 2.0f * (prn100 / 255.0f);
  prn101 = -1.0f + 2.0f * (prn101 / 255.0f);
  prn110 = -1.0f + 2.0f * (prn110 / 255.0f);
  prn111 = -1.0f + 2.0f * (prn111 / 255.0f);

  float wx = (2.0f * dx - 3.0f) * dx * dx + 1.0f;
  float wy = (2.0f * dy - 3.0f) * dy * dy + 1.0f;
  float wz = (2.0f * dz - 3.0f) * dz * dz + 1.0f;

  float prn00X = prn000 * wz + prn001 * (1.0f - wz);
  float prn01X = prn010 * wz + prn011 * (1.0f - wz);
  float prn10X = prn100 * wz + prn101 * (1.0f - wz);
  float prn11X = prn110 * wz + prn111 * (1.0f - wz);

  float prn0XX = prn00X * wy + prn01X * (1.0f - wy);
  float prn1XX = prn10X * wy + prn11X * (1.0f - wy);

  return prn0XX * wx + prn1XX * (1.0f - wx);
}

ccl_device int B(int n, int b)
{
  return (n >> b) & 1;
}

ccl_device int B(int i, int j, int k, int b)
{
  int SimplexT[8] = {0x15, 0x38, 0x32, 0x2c, 0x0d, 0x13, 0x07, 0x2a};
  return SimplexT[(B(i, b) << 2) | (B(j, b) << 1) | B(k, b)];
}

ccl_device int shuffle(int i, int j, int k)
{
  return B(i, j, k, 0) + B(j, k, i, 1) + B(k, i, j, 2) + B(i, j, k, 3) + B(j, k, i, 4) +
         B(k, i, j, 5) + B(i, j, k, 6) + B(j, k, i, 7);
}

ccl_device float K(int a, float u, float v, float w, int i, int j, int k, int3* a_vec)
{
  float s = float((*a_vec)[0] + (*a_vec)[1] + (*a_vec)[2]) / 6.0f;
  float x = u - float((*a_vec)[0]) + s;
  float y = v - float((*a_vec)[1]) + s;
  float z = w - float((*a_vec)[2]) + s;
  float t = 0.6f - x * x - y * y - z * z;
  int h = shuffle(i + (*a_vec)[0], j + (*a_vec)[1], k + (*a_vec)[2]);
  (*a_vec)[a]++;
  t = max(t, 0.0f);
  int b5 = (h >> 5) & 1;
  int b4 = (h >> 4) & 1;
  int b3 = (h >> 3) & 1;
  int b2 = (h >> 2) & 1;
  int b = (h & 3);
  float p = (b == 1 ? x : (b == 2 ? y : z));
  float q = (b == 1 ? y : (b == 2 ? z : x));
  float r = (b == 1 ? z : (b == 2 ? x : y));
  p = (b5 == b3 ? -p : p);
  q = (b5 == b4 ? -q : q);
  r = (b5 != (b4 ^ b3) ? -r : r);
  t *= t;
  return 8.0f * t * t * (p + (b == 0 ? q + r : (b2 == 0 ? q : r)));
}

ccl_device float simplex_noise(float x, float y, float z)
{
  float s = (x + y + z) / 3.0f;
  int i = int(floorf(x + s));
  int j = int(floorf(y + s));
  int k = int(floorf(z + s));
  s = float(i + j + k) / 6.0f;
  float u = x - float(i) + s;
  float v = y - float(j) + s;
  float w = z - float(k) + s;
  int3 a = make_int3(0);
  int hi = (u >= w ? (u >= v ? 0 : 1) : (v >= w ? 1 : 2));
  int lo = (u < w ? (u < v ? 0 : 1) : (v < w ? 1 : 2));
  float k1 = K(hi, u, v, w, i, j, k, &a);
  float k2 = K(3 - hi - lo, u, v, w, i, j, k, &a);
  float k3 = K(lo, u, v, w, i, j, k, &a);
  float k4 = K(0, u, v, w, i, j, k, &a);
  return k1 + k2 + k3 + k4;
}

#define SCNPERM(kg, x) perlin_noise((kg), (x) & (RHINO_PERLIN_NOISE_PERM_SIZE - 1))
#define SCNINDEX(kg, ix, iy, iz) SCNPERM((kg), (ix) + SCNPERM((kg), (iy) + SCNPERM((kg), iz)))

#define SCNNEXT(h) (((h) + 1) & (RHINO_PERLIN_NOISE_PERM_SIZE - 1))
#define SCNNIMPULSES 3

ccl_device float SCNoise(KernelGlobals *kg, float x, float y, float z)
{
  int ix = int(floorf(x));
  int iy = int(floorf(y));
  int iz = int(floorf(z));

  float fx = x - float(ix);
  float fy = y - float(iy);
  float fz = z - float(iz);

  float sum = 0;

  /* Perform the sparse convolution. */
  for (int i = -2; i <= 2; i++) {
    for (int j = -2; j <= 2; j++) {
      for (int k = -2; k <= 2; k++) {
        /* Compute voxel hash code. */
        int h = SCNINDEX(kg, ix + i, iy + j, iz + k);

        for (int n = SCNNIMPULSES; n > 0; n--, h = SCNNEXT(h)) {
          /* Convolve filter and impulse. */
          //float4 fp = ImpulseValue(h);
          //float dx = fx - (float(i) + fp.x);
          //float dy = fy - (float(j) + fp.y);
          //float dz = fz - (float(k) + fp.z);
          //float distsq = dx * dx + dy * dy + dz * dz;
          //sum += Catrom2(distsq) * fp.w;
        }
      }
    }
  }

  return sum / float(SCNNIMPULSES);
}

ccl_device float4 noise_texture(KernelGlobals *kg,
                                float3 uvw,
                                const Transform* uvw_transform,
                                float4 color1,
                                float4 color2,
                                RhinoProceduralNoiseType noise_type,
                                RhinoProceduralSpecSynthType spec_synth_type,
                                int octave_count,
                                float frequency_multiplier,
                                float amplitude_multiplier,
                                float clamp_min,
                                float clamp_max,
                                bool scale_to_clamp,
                                bool inverse,
                                float gain)
{
  float4 color_out = make_float4(0.0f, 0.0f, 0.0f, 1.0f);

  float total_value = 0.0;
  float freq = 1.0;
  float weight = 1.0;
  float total_weight = 0.0;

  for (int o = 0; o < octave_count; o++) {
    float fo = float(o);
    float value = 0.0;
    float x = uvw.x * freq + fo;
    float y = uvw.y * freq + fo * 2.0f;
    float z = uvw.z * freq - fo;

    switch (noise_type) {
      case RhinoProceduralNoiseType::PERLIN:
        value = noise(kg, x, y, z);
        break;
      case RhinoProceduralNoiseType::VALUE_NOISE:
        value = value_noise(kg, x, y, z);
        break;
      case RhinoProceduralNoiseType::PERLIN_PLUS_VALUE:
        value = 0.5f * value_noise(kg, x, y, z) + 0.5f * noise(kg, x, y, z);
        break;
      case RhinoProceduralNoiseType::SIMPLEX:
        value = simplex_noise(x, y, z);
        break;
      case RhinoProceduralNoiseType::SPARSE_CONVOLUTION:
        value = SCNoise(kg, x, y, z);
        break;
      //case RhinoProceduralNoiseType::LATTICE_CONVOLUTION:
      //  value = VCNoise(x, y, z);
      //  break;
      //case RhinoProceduralNoiseType::WARDS_HERMITE:
      //  value = WardsHermiteNoise(x, y, z);
      //  break;
      //case RhinoProceduralNoiseType::AALTONEN:
      //  value = AaltonenNoise(x, y, z);
      //  break;
      default:
        break;
    }

    if (spec_synth_type == RhinoProceduralSpecSynthType::TURBULENCE && (value < 0.0f))
      value = -value;
    total_value += weight * value;

    total_weight += weight;

    freq *= frequency_multiplier;
    weight *= amplitude_multiplier;
  }

  if (total_weight > 0.0f)
    total_value /= total_weight;

  if (spec_synth_type == RhinoProceduralSpecSynthType::TURBULENCE)
    total_value = 2.0f * total_value - 1.0f;

  if (total_value >= clamp_max)
    total_value = clamp_max;

  if (total_value <= clamp_min)
    total_value = clamp_min;

  if (scale_to_clamp) {
    if (clamp_max - clamp_min != 0.0f) {
      total_value = -1.0f + 2.0f * (total_value - clamp_min) / (clamp_max - clamp_min);
    }
    else {
      total_value = -1.0f;
    }
  }

  if (inverse) {
    total_value = -total_value;
  }

  float color_value_before_gain = 0.5f * (1.0f + total_value);
  float color_value_after_gain = gain_func(color_value_before_gain, gain);

  color_out = mix(color1, color2, color_value_after_gain);
  color_out.w = clamp(color_out.w, 0.0f, 1.0f);

  return color_out;
}

ccl_device void svm_rhino_node_noise_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, in_color1_offset, in_color2_offset, out_color_offset;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &in_color1_offset, &in_color2_offset, &out_color_offset);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);
  float3 color1 = stack_load_float3(stack, in_color1_offset);
  float3 color2 = stack_load_float3(stack, in_color2_offset);

  Transform uvw_transform;
  uvw_transform.x = read_node_float(kg, offset);
  uvw_transform.y = read_node_float(kg, offset);
  uvw_transform.z = read_node_float(kg, offset);

  uint4 data0 = read_node(kg, offset);
  uint4 data1 = read_node(kg, offset);
  uint4 data2 = read_node(kg, offset);

  RhinoProceduralNoiseType noise_type = (RhinoProceduralNoiseType)data0.x;
  RhinoProceduralSpecSynthType spec_synth_type = (RhinoProceduralSpecSynthType)data0.y;
  int octave_count = (int)data0.z;
  float frequency_multiplier = __uint_as_float(data0.w);
  float amplitude_multiplier = __uint_as_float(data1.x);
  float clamp_min = __uint_as_float(data1.y);
  float clamp_max = __uint_as_float(data1.z);
  bool scale_to_clamp = (bool)data1.w;
  bool inverse = (bool)data2.x;
  float gain = __uint_as_float(data2.y);

  float4 out_color = noise_texture(kg,
                                   uvw,
                                   &uvw_transform,
                                   make_float4(color1.x, color1.y, color1.z, 1.0f),
                                   make_float4(color2.x, color2.y, color2.z, 1.0f),
                                   noise_type,
                                   spec_synth_type,
                                   octave_count,
                                   frequency_multiplier,
                                   amplitude_multiplier,
                                   clamp_min,
                                   clamp_max,
                                   scale_to_clamp,
                                   inverse,
                                   gain);

  if (stack_valid(out_color_offset))
    stack_store_float3(
        stack, out_color_offset, make_float3(out_color.x, out_color.y, out_color.z));
}

CCL_NAMESPACE_END
