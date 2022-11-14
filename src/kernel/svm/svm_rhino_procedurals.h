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

#define PI 3.14159265358979f
#define TWO_PI 6.28318530717959f
#define INV_PI 0.31830988618379f
#define INV_TWO_PI 0.15915494309190f
#define SQRT_PI 1.77245385090552f

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


#define VCNPERM(kg, x) perlin_noise((kg), (x) & (RHINO_PERLIN_NOISE_PERM_SIZE - 1))
#define VCNINDEX(kg, ix, iy, iz) VCNPERM((kg), (ix) + VCNPERM((kg), (iy) + VCNPERM((kg), iz)))

ccl_device float4 impulse_noise(KernelGlobals *kg, int x)
{
  float4 noise = make_float4(0.0f);
  noise.x = kernel_tex_fetch(__lookup_table, kernel_data.tables.rhino_impulse_noise_offset + x++);
  noise.y = kernel_tex_fetch(__lookup_table, kernel_data.tables.rhino_impulse_noise_offset + x++);
  noise.z = kernel_tex_fetch(__lookup_table, kernel_data.tables.rhino_impulse_noise_offset + x++);
  noise.w = kernel_tex_fetch(__lookup_table, kernel_data.tables.rhino_impulse_noise_offset + x++);

  return noise;
}

ccl_device float catrom2(float d)
{
#define SAMPRATE 100 /* table entries per unit distance */

  float factor = (d < 4.0f ? 0.0f : 1.0f);
  d = (1.0f - factor) * d + factor;

  d = d * float(SAMPRATE) + 0.5f;
  int i = int(floorf(d));

  float x = sqrt(i / float(SAMPRATE));

  factor = (x < 1.0f ? 0.0f : 1.0f);
  x = (1.0f - factor) * (0.5f * (2.0f + x * x * (-5.0f + x * 3.0f))) +
      factor * (0.5f * (4.0f + x * (-8.0f + x * (5.0f - x))));

  return x;
}

ccl_device float sparse_convolution_noise(KernelGlobals *kg, float x, float y, float z)
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
          float4 fp = impulse_noise(kg, h * 4);
          float dx = fx - (float(i) + fp.x);
          float dy = fy - (float(j) + fp.y);
          float dz = fz - (float(k) + fp.z);
          float distsq = dx * dx + dy * dy + dz * dz;
          sum += catrom2(distsq) * fp.w;
        }
      }
    }
  }

  return sum / float(SCNNIMPULSES);
}

ccl_device float vc_noise(KernelGlobals *kg, int x)
{
  float noise = kernel_tex_fetch(__lookup_table, kernel_data.tables.rhino_vc_noise_offset + x);

  return noise;
}

ccl_device float lattice_convolution_noise(KernelGlobals *kg, float x, float y, float z)
{
  int ix = int(floorf(x));
  int iy = int(floorf(y));
  int iz = int(floorf(z));

  float fx = x - float(ix);
  float fy = y - float(iy);
  float fz = z - float(iz);

  float sum = 0.0f;

  for (int k = -1; k <= 2; k++) {
    float dz = float(k) - fz;
    dz = dz * dz;

    for (int j = -1; j <= 2; j++) {
      float dy = float(j) - fy;
      dy = dy * dy;

      for (int i = -1; i <= 2; i++) {
        float dx = float(i) - fx;
        dx = dx * dx;

        sum += vc_noise(kg, VCNINDEX(kg, ix + i, iy + j, iz + k)) * catrom2(dx + dy + dz);
      }
    }
  }

  return sum;
}

ccl_device float WHN_frand(int seed)
{
  seed = seed << (13 ^ seed);
  return (1.0f - float(int(seed * (seed * seed * 15731 + 789221) + 1376312589) & 0x7fffffff) /
                     1073741824.0f);
}

ccl_device float WHN_rand3a(int x, int y, int z)
{
  return WHN_frand(67 * x + 59 * y + 71 * z);
}
ccl_device float WHN_rand3b(int x, int y, int z)
{
  return WHN_frand(73 * x + 79 * y + 83 * z);
}
ccl_device float WHN_rand3c(int x, int y, int z)
{
  return WHN_frand(89 * x + 97 * y + 101 * z);
}
ccl_device float WHN_rand3d(int x, int y, int z)
{
  return WHN_frand(103 * x + 107 * y + 109 * z);
}

ccl_device float WHN_hpoly1(float t)
{
  return ((2.0 * t - 3.0) * t * t + 1.0);
}
ccl_device float WHN_hpoly2(float t)
{
  return (-2.0 * t + 3.0) * t * t;
}
ccl_device float WHN_hpoly3(float t)
{
  return ((t - 2.0) * t + 1.0) * t;
}
ccl_device float WHN_hpoly4(float t)
{
  return (t - 1.0) * t * t;
}

ccl_device float4 WHN_rand(int i, int3 xlim[2])
{
  float4 f;

  f[0] = WHN_rand3a(xlim[i & 1][0], xlim[(i >> 1) & 1][1], xlim[(i >> 2) & 1][2]);
  f[1] = WHN_rand3b(xlim[i & 1][0], xlim[(i >> 1) & 1][1], xlim[(i >> 2) & 1][2]);
  f[2] = WHN_rand3c(xlim[i & 1][0], xlim[(i >> 1) & 1][1], xlim[(i >> 2) & 1][2]);
  f[3] = WHN_rand3d(xlim[i & 1][0], xlim[(i >> 1) & 1][1], xlim[(i >> 2) & 1][2]);

  return f;
}

ccl_device float4 WHN_hpoly(int n, float3 xarg, float4 f0, float4 f1)
{
  float4 f;

  float x = xarg[n];
  float hp1 = WHN_hpoly1(x);
  float hp2 = WHN_hpoly2(x);
  f[0] = f0[0] * hp1 + f1[0] * hp2;
  f[1] = f0[1] * hp1 + f1[1] * hp2;
  f[2] = f0[2] * hp1 + f1[2] * hp2;
  f[3] = f0[3] * hp1 + f1[3] * hp2 + f0[n] * WHN_hpoly3(x) + f1[n] * WHN_hpoly4(x);

  return f;
}

ccl_device float4 WHN_interpolate_nonrecursive(int3 xlim[2], float3 pXarg)
{
  float4 f0 = WHN_rand(0, xlim);
  float4 f1 = WHN_rand(1, xlim);

  float4 f2 = WHN_rand(2, xlim);
  float4 f3 = WHN_rand(3, xlim);

  float4 f4 = WHN_rand(4, xlim);
  float4 f5 = WHN_rand(5, xlim);

  float4 f6 = WHN_rand(10, xlim);
  float4 f7 = WHN_rand(11, xlim);

  float4 ff0 = WHN_hpoly(0, pXarg, f0, f1);
  float4 ff1 = WHN_hpoly(0, pXarg, f2, f3);
  float4 ff2 = WHN_hpoly(0, pXarg, f4, f5);
  float4 ff3 = WHN_hpoly(0, pXarg, f6, f7);

  float4 fff0 = WHN_hpoly(1, pXarg, ff0, ff1);
  float4 fff1 = WHN_hpoly(1, pXarg, ff2, ff3);

  return WHN_hpoly(2, pXarg, fff0, fff1);
}

ccl_device float wards_hermite_noise(float x, float y, float z)
{
  float3 uvw = make_float3(x, y, z);

  int3 xlim[2];
  xlim[0] = make_int3(int(floorf(uvw.x)), int(floorf(uvw.y)), int(floorf(uvw.z)));
  xlim[1] = xlim[0] + make_int3(1);

  float3 xarg = uvw - make_float3(xlim[0].x, xlim[0].y, xlim[0].z);

  float4 f = WHN_interpolate_nonrecursive(xlim, xarg);

  return f[3];
}

ccl_device int aaltonen_value(KernelGlobals *kg, int x)
{
  float value = kernel_tex_fetch(__lookup_table, kernel_data.tables.rhino_aaltonen_noise_offset + x);

  return int(value);
}

ccl_device float aaltonen_noise(KernelGlobals *kg, float x, float y, float z)
{
  // clang-format off
  float ANsquaredRadius[16] = {
	  1.5000000f, 1.8262500f, 2.1524999f, 2.4787498f,
      2.8049998f, 3.1312499f, 1.7175000f, 2.0437498f,
      2.3699999f, 2.6962500f, 3.0224998f, 1.6087500f,
      1.9349999f, 2.2612500f, 2.5874999f, 2.9137497f
  };

  float ANinverseOfSquaredRadius[16] = {
	  0.66666669f, 0.54757017f, 0.46457610f, 0.40342918f,
      0.35650626f, 0.31936130f, 0.58224165f, 0.48929667f,
      0.42194095f, 0.37088549f, 0.33085197f, 0.62160063f,
      0.51679587f, 0.44223326f, 0.38647345f, 0.34320039f
  };
  // clang-format on

#define maxRadius 1.8f
#define maxRadiusInt 1

  int sx = int(ceilf(x - maxRadius));
  int ex = int(floorf(x + maxRadius));
  if (ex >= sx + 4 || sx > ex)
    return 0.0f;

  int sy = int(ceilf(y - maxRadius));
  int ey = int(floorf(y + maxRadius));
  if (ey >= sy + 4 || sy > ey)
    return 0.0f;

  int sz = int(ceilf(z - maxRadius));
  int ez = int(floorf(z + maxRadius));
  if (ez >= sz + 4 || sz > ez)
    return 0.0f;

  float result = 0.0;
  for (int iz = sz; iz <= ez; iz++) {
    float dz = iz - z;
    float sqdz = dz * dz;
    int permZ = aaltonen_value(kg, iz & 255);
    for (int iy = sy; iy <= ey; iy++) {
      float dy = iy - y;
      float sqdy = dy * dy;
      float sqdydz = sqdy + sqdz;
      int permY = aaltonen_value(kg, permZ + (iy & 255));
      for (int ix = sx; ix <= ex; ix++) {
        float dx = ix - x;
        float sqdx = dx * dx;
        float dsq = sqdx + sqdydz;
        int prn = aaltonen_value(kg, permY + (ix & 255));
        if (ANsquaredRadius[prn & 15] >= dsq) {
          float t = 1.0f - dsq * ANinverseOfSquaredRadius[prn & 15];
          if ((prn & 128) == 128)
            result -= t;
          else
            result += t;
        }
      }
    }
  }

  return result / 10.333334f;
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
  uvw = transform_point(uvw_transform, uvw);

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
      case RHINO_NOISE_PERLIN:
        value = noise(kg, x, y, z);
        break;
      case RHINO_NOISE_VALUE_NOISE:
        value = value_noise(kg, x, y, z);
        break;
      case RHINO_NOISE_PERLIN_PLUS_VALUE:
        value = 0.5f * value_noise(kg, x, y, z) + 0.5f * noise(kg, x, y, z);
        break;
      case RHINO_NOISE_SIMPLEX:
        value = simplex_noise(x, y, z);
        break;
      case RHINO_NOISE_SPARSE_CONVOLUTION:
        value = sparse_convolution_noise(kg, x, y, z);
        break;
      case RHINO_NOISE_LATTICE_CONVOLUTION:
        value = lattice_convolution_noise(kg, x, y, z);
        break;
      case RHINO_NOISE_WARDS_HERMITE:
        value = wards_hermite_noise(x, y, z);
        break;
      case RHINO_NOISE_AALTONEN:
        value = aaltonen_noise(kg, x, y, z);
        break;
      default:
        break;
    }

    if (spec_synth_type == RHINO_SPEC_SYNTH_TURBULENCE && (value < 0.0f))
      value = -value;
    total_value += weight * value;

    total_weight += weight;

    freq *= frequency_multiplier;
    weight *= amplitude_multiplier;
  }

  if (total_weight > 0.0f)
    total_value /= total_weight;

  if (spec_synth_type == RHINO_SPEC_SYNTH_TURBULENCE)
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

ccl_device float luminance(float3 color)
{
  return 0.299f * color.x + 0.587f * color.y + 0.114f * color.z;
}

ccl_device float4 waves_width_texture(float3 uvw,
                                      const Transform *uvw_transform,
                                      RhinoProceduralWavesType wave_type)
{
  uvw = transform_point(uvw_transform, uvw);

  float3 uvw_perturbed = make_float3(uvw.x, 0.5 + floorf(uvw.y), 0.0);

  if (wave_type == RHINO_WAVES_RADIAL) {
    float uvw_length = len(uvw);
    if (uvw_length == 0.0)
      uvw_perturbed = make_float3(0.0f);
    else
      uvw_perturbed = (uvw / uvw_length) * (0.5 + floorf(uvw_length));
  }

  return make_float4(uvw_perturbed.x, uvw_perturbed.y, uvw_perturbed.z, 1.0);
}

ccl_device void svm_rhino_node_waves_width_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, out_uvw_offset;
  uint dummy;

  svm_unpack_node_uchar4(node.y, &in_uvw_offset, &out_uvw_offset, &dummy, &dummy);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);

  Transform uvw_transform;
  uvw_transform.x = read_node_float(kg, offset);
  uvw_transform.y = read_node_float(kg, offset);
  uvw_transform.z = read_node_float(kg, offset);

  uint4 data0 = read_node(kg, offset);

  RhinoProceduralWavesType wave_type = (RhinoProceduralWavesType)data0.x;

  float4 out_uvw = waves_width_texture(uvw,
                                   &uvw_transform,
                                   wave_type);

  if (stack_valid(out_uvw_offset))
    stack_store_float3(stack, out_uvw_offset, make_float3(out_uvw.x, out_uvw.y, out_uvw.z));
}

ccl_device float4 waves_texture(float3 uvw,
                                const Transform *uvw_transform,
                                float4 color1,
                                float4 color2,
                                float4 color3,
                                RhinoProceduralWavesType wave_type,
                                float wave_width,
                                bool wave_width_texture_on,
                                float contrast1,
                                float contrast2)
{
  uvw = transform_point(uvw_transform, uvw);

  float parameter = ((wave_type == RHINO_WAVES_LINEAR) ?
                         uvw.y :
                         sqrt(uvw.x * uvw.x + uvw.y * uvw.y));

  float phase = parameter - floorf(parameter);

  float hillParameter = min(phase, 1.0f - phase);

  float waveWidth = wave_width_texture_on ? luminance(make_float3(color3.x, color3.y, color3.z)) :
                                            wave_width;

  float hillLength = min(1.0f - waveWidth, waveWidth);
  float hillMiddle = 0.5f * (1.0f - waveWidth);

  float value = 0.0f;
  if (hillParameter > hillMiddle - 0.5f * hillLength) {
    if (hillParameter < hillMiddle + 0.5f * hillLength) {
      value = 0.5f + 0.5f * sin((hillParameter - hillMiddle) / (hillLength)*PI);
    }
    else
      value = 1.0f;
  }

  float contrast = contrast1;
  if (phase > 0.5f)
    contrast = contrast2;

  if (contrast < 1.0f) {
    value = 0.5f + 0.5f * (2.0f * value - 1.0f) / (1.0f - contrast);
    if (value < 0.0f)
      value = 0.0f;
    if (value > 1.0f)
      value = 1.0f;
  }
  else {
    if (value >= 0.5f)
      value = 1.0f;
    if (value < 0.5f)
      value = 0.0f;
  }

  if (value <= 0.0f)
    return color1;

  if (value >= 1.0f)
    return color2;

  float4 output_color = mix(color1, color2, value);
  output_color.w = clamp(output_color.w, 0.0f, 1.0f);

  return output_color;
}

ccl_device void svm_rhino_node_waves_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, in_color1_offset, in_color2_offset, in_color3_offset, out_color_offset;
  uint dummy;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &in_color1_offset, &in_color2_offset, &in_color3_offset);
  svm_unpack_node_uchar4(
      node.z, &out_color_offset, &dummy, &dummy, &dummy);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);
  float3 color1 = stack_load_float3(stack, in_color1_offset);
  float3 color2 = stack_load_float3(stack, in_color2_offset);
  float3 color3 = stack_load_float3(stack, in_color3_offset);

  Transform uvw_transform;
  uvw_transform.x = read_node_float(kg, offset);
  uvw_transform.y = read_node_float(kg, offset);
  uvw_transform.z = read_node_float(kg, offset);

  uint4 data0 = read_node(kg, offset);
  uint4 data1 = read_node(kg, offset);

  RhinoProceduralWavesType wave_type = (RhinoProceduralWavesType)data0.x;
  float wave_width = __uint_as_float(data0.y);
  bool wave_width_texture_on = (bool)data0.z;
  float contrast1 = __uint_as_float(data0.w);
  float contrast2 = __uint_as_float(data1.x);

  float4 out_color = waves_texture(uvw,
                                   &uvw_transform,
                                   make_float4(color1.x, color1.y, color1.z, 1.0f),
                                   make_float4(color2.x, color2.y, color2.z, 1.0f),
                                   make_float4(color3.x, color3.y, color3.z, 1.0f),
                                   wave_type,
                                   wave_width,
                                   wave_width_texture_on,
                                   contrast1,
                                   contrast2);

  if (stack_valid(out_color_offset))
    stack_store_float3(
        stack, out_color_offset, make_float3(out_color.x, out_color.y, out_color.z));
}

ccl_device void perturbing_part1_texture(float3 uvw, const Transform *uvw_transform, float3* out_uvw0, float3* out_uvw1, float3* out_uvw2)
{
  uvw = transform_point(uvw_transform, uvw);

  *out_uvw0 = uvw;
  *out_uvw1 = make_float3(1.0f, 0.0f, 0.0f) - uvw;
  *out_uvw2 = make_float3(0.0f, 1.0f, 0.0f) - uvw;
}

ccl_device void svm_rhino_node_perturbing_part1_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, out_uvw0_offset, out_uvw1_offset, out_uvw2_offset;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &out_uvw0_offset, &out_uvw1_offset, &out_uvw2_offset);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);

  Transform uvw_transform;
  uvw_transform.x = read_node_float(kg, offset);
  uvw_transform.y = read_node_float(kg, offset);
  uvw_transform.z = read_node_float(kg, offset);

  float3 out_uvw0, out_uvw1, out_uvw2;
  perturbing_part1_texture(uvw, &uvw_transform, &out_uvw0, &out_uvw1, &out_uvw2);

  if (stack_valid(out_uvw0_offset))
    stack_store_float3(stack, out_uvw0_offset, out_uvw0);

  if (stack_valid(out_uvw1_offset))
    stack_store_float3(stack, out_uvw1_offset, out_uvw1);

  if (stack_valid(out_uvw2_offset))
    stack_store_float3(stack, out_uvw2_offset, out_uvw2);
}

ccl_device float3 perturbing_part2_texture(
    float3 uvw, float3 perturb_color1, float3 perturb_color2, float3 perturb_color3, float amount)
{
  float x = reduce_add(perturb_color1) / 1.5f * amount - amount;
  float y = reduce_add(perturb_color2) / 1.5f * amount - amount;
  float z = reduce_add(perturb_color3) / 1.5f * amount - amount;

  float3 perturbed_uvw = uvw + make_float3(x, y, z);

  return perturbed_uvw;
}

ccl_device void svm_rhino_node_perturbing_part2_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, in_color0_offset, in_color1_offset, in_color2_offset, out_uvw_offset;
  uint dummy;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &in_color0_offset, &in_color1_offset, &in_color2_offset);
  svm_unpack_node_uchar4(node.z, &out_uvw_offset, &dummy, &dummy, &dummy);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);
  float3 color0 = stack_load_float3(stack, in_color0_offset);
  float3 color1 = stack_load_float3(stack, in_color1_offset);
  float3 color2 = stack_load_float3(stack, in_color2_offset);
  float amount = __uint_as_float(node.w);

  float3 out_uvw = perturbing_part2_texture(uvw, color0, color1, color2, amount);

  if (stack_valid(out_uvw_offset))
    stack_store_float3(stack, out_uvw_offset, out_uvw);
}

ccl_device float4 gradient_texture(float3 uvw,
                                   const Transform *uvw_transform,
                                   float4 color1,
                                   float4 color2,
                                   RhinoProceduralGradientType type,
                                   bool flip_alternate,
                                   bool use_custom_curve,
                                   int point_width,
                                   int point_height)
{
  uvw = transform_point(uvw_transform, uvw);

  float4 color_out = make_float4(0.0f, 0.0f, 0.0f, 1.0f);

  int u_cell = int(floorf(uvw.x));
  int v_cell = int(floorf(uvw.y));

  float u = fractf(uvw.x);
  float v = fractf(uvw.y);

  int cell_parity = 0;
  float t = 0;

  switch (type) {
    case RHINO_GRADIENT_LINEAR:
      t = v;
      cell_parity = v_cell;
      break;

    case RHINO_GRADIENT_BOX:
      t = 2.0f * min(min(v, 1.0f - v), min(u, 1.0f - u));
      cell_parity = v_cell + u_cell;
      break;

    case RHINO_GRADIENT_RADIAL: {
      float dist = sqrt(uvw.x * uvw.x + uvw.y * uvw.y);
      t = fractf(dist);
      cell_parity = int(floorf(dist));
    } break;

    case RHINO_GRADIENT_TARTAN:
      t = 1.0f - min(fabsf(0.5f - u), fabsf(0.5f - v)) * 2.0f;
      cell_parity = v_cell + u_cell;
      break;

    case RHINO_GRADIENT_SWEEP:
      t = atan2f(uvw.x, uvw.y) / (2.0f * PI / 4.0f);
      cell_parity = int(floorf(t));
      t = fmodf(t + 40.0f, 1.0f);
      break;

    case RHINO_GRADIENT_PONG:
      t = atan2f(uvw.x, uvw.y) / (2.0f * PI / 8.0f);
      cell_parity = int(floorf(0.5 * t));
      t = fmodf(t + 80.0f, 2.0f);
      if (t > 1.0f)
        t = 2.0f - t;
      break;

    case RHINO_GRADIENT_SPIRAL:
      t = atan2f(0.5f - uvw.x, uvw.y - 0.5f) / (2.0f * PI / 1.0f);
      t = fmodf(t + 10.0f, 1.0f);
      cell_parity = 0;
      break;
  }

  if (flip_alternate && is_odd(cell_parity)) {
    t = 1.0 - t;
  }

  if (use_custom_curve) {
    // for (int i = 0; i < point_height; ++i) {
    //   float newV = GetCurveY(i, t, point_width, point_height, sampler_index);
    //   color1[i] *= newV;
    //   color2[i] *= newV;
    // }

    color_out = clamp(color1 + color2, make_float4(0.0f), make_float4(1.0f));
  }
  else {
    color_out = mix(color2, color1, t);
  }

  return color_out;
}

ccl_device void svm_rhino_node_gradient_texture(
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

  RhinoProceduralGradientType gradient_type = (RhinoProceduralGradientType)data0.x;
  bool flip_alternate = (bool)data0.y;
  bool use_custom_curve = (bool)data0.z;
  int point_width = (int)data0.w;
  int point_height = (int)data1.x;

  float4 out_color = gradient_texture(uvw,
                                      &uvw_transform,
                                      make_float4(color1.x, color1.y, color1.z, 1.0f),
                                      make_float4(color2.x, color2.y, color2.z, 1.0f),
                                      gradient_type,
                                      flip_alternate,
                                      use_custom_curve,
                                      point_width,
                                      point_height);

  if (stack_valid(out_color_offset))
    stack_store_float3(stack, out_color_offset, make_float3(out_color.x, out_color.y, out_color.z));
}

ccl_device float Luminance(float3 color)
{
  return 0.299f * color.x + 0.587f * color.y + 0.114f * color.z;
}

ccl_device float4 blend_texture(float3 uvw,
                                float4 color1,
                                float4 color2,
                                float4 blend_color,
                                bool use_blend_color,
                                float blend_factor)
{
  if (use_blend_color) {
    blend_factor = Luminance(make_float3(blend_color.x, blend_color.y, blend_color.z));
  }

  float4 color_out = mix(color1, color2, blend_factor);
  color_out.w = clamp(color_out.w, 0.0f, 1.0f);

  return color_out;
}

ccl_device void svm_rhino_node_blend_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, in_color1_offset, in_color2_offset, in_blend_color_offset, out_color_offset;
  uint dummy;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &in_color1_offset, &in_color2_offset, &in_blend_color_offset);
  svm_unpack_node_uchar4(node.z, &out_color_offset, &dummy, &dummy, &dummy);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);
  float3 color1 = stack_load_float3(stack, in_color1_offset);
  float3 color2 = stack_load_float3(stack, in_color2_offset);
  float3 blend_color = stack_load_float3(stack, in_blend_color_offset);

  uint4 data = read_node(kg, offset);

  bool use_blend_color = (bool)data.x;
  float blend_factor = __uint_as_float(data.y);

  float4 out_color = blend_texture(uvw,
                                   make_float4(color1.x, color1.y, color1.z, 1.0f),
                                   make_float4(color2.x, color2.y, color2.z, 1.0f),
                                   make_float4(blend_color.x, blend_color.y, blend_color.z, 1.0f),
                                   use_blend_color,
                                   blend_factor);

  if (stack_valid(out_color_offset))
    stack_store_float3(
        stack, out_color_offset, make_float3(out_color.x, out_color.y, out_color.z));
}

ccl_device float3 RgbToYxy(float3 rgb)
{
  float Y = 0.0f;
  float x = 0.0f;
  float y = 0.0f;

  float r = rgb.x;
  float g = rgb.y;
  float b = rgb.z;

  float res0 = (0.5141364f * r) + (0.32387860f * g) + (0.16036376f * b);
  float res1 = (0.2650680f * r) + (0.67023428f * g) + (0.06409157f * b);
  float res2 = (0.0241188f * r) + (0.12281780f * g) + (0.84442666f * b);

  float fW = res0 + res1 + res2;
  if (fW > 0.0f) {
    Y = res1;
    x = res0 / fW;
    y = res1 / fW;
  }

  return make_float3(Y, x, y);
}

ccl_device float3 LogMapYxy(float3 Yxy, float exposure, float world_luminance, float max_luminance)
{
  float Y = Yxy[0];
  float x = Yxy[1];
  float y = Yxy[2];

  float fExposure = pow(2.0f, exposure);  // default exposure is 1, 2^0

  // Arbitrary Bias Parameter
  float fBias = 0.85f;
  float fCont = 0.0f;

  float exp_adapt = 1.0f;

  float fAvLum = exp(world_luminance) / exp_adapt;
  float fBiasP = (log(fBias) / log(0.5));
  float fContP = (0.0f != fCont) ? 1.0f / fCont : 1e22f;
  float fLmax = max_luminance / fAvLum;
  float fDivider = log10(fLmax + 1.0f);

  float fValue = Y;

  // Inverse gamma function to enhance contrast. Not in paper.
  if (0.0f != fCont) {
    fValue = pow(fValue, fContP);
  }

  fValue = (fValue / fAvLum) * fExposure;

  float fLog;

  if (fValue < 1.0f) {
    // Approximation of log(x+1) = x(6 + x) / (6 + 4x) when x < 1.
    fLog = fValue * (6.0f + fValue) / (6.0f + (4.0f * fValue));
  }
  else if (fValue < 2.0f) {
    // Approximation of log(x+1) = x(6 + 0.7662x) / (5.9897 + 3.7658x) when 1 < x < 2.
    fLog = fValue * (6.0f + 0.7662f * fValue) / (5.9897f + 3.7658f * fValue);
  }
  else {
    // Normal log(x+1).
    fLog = log(fValue + 1.0f);
  }

  float fInterpol = log(2.0f + pow(fValue / fLmax, fBiasP) * 8.0f);

  Y = (fLog / fInterpol) / fDivider;

  return make_float3(Y, x, y);
}

ccl_device float3 YxyToRgb(float3 Yxy)
{
  float Y = Yxy[0];
  float x = Yxy[1];
  float y = Yxy[2];

  float X = 0.0;
  float Z = 0.0;

  if ((x > 0.0) && (y > 0.0)) {
    X = (x * Y) / y;
    Z = (X / x) - X - Y;
  }

  float r = (2.5651f * X) + (-1.1665f * Y) + (-0.3986f * Z);
  float g = (-1.0217f * X) + (1.9777f * Y) + (0.0439f * Z);
  float b = (0.0753f * X) + (-0.2543f * Y) + (1.1892f * Z);

  return make_float3(r, g, b);
}

ccl_device float4 exposure_texture(
    float4 color, float exposure, float multiplier, float world_luminance, float max_luminance)
{
  float3 Yxy = RgbToYxy(make_float3(color.x, color.y, color.z));

  Yxy = LogMapYxy(Yxy, exposure, world_luminance, max_luminance);

  float3 rgb = YxyToRgb(Yxy);
  rgb *= multiplier;

  return make_float4(rgb.x, rgb.y, rgb.z, 1.0);
}

ccl_device void svm_rhino_node_exposure_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_color_offset, out_color_offset;
  uint dummy;

  svm_unpack_node_uchar4(node.y, &in_color_offset, &out_color_offset, &dummy, &dummy);

  float3 color = stack_load_float3(stack, in_color_offset);

  uint4 data = read_node(kg, offset);

  float exposure = __uint_as_float(data.x);
  float multiplier = __uint_as_float(data.y);
  float world_luminance = __uint_as_float(data.z);
  float max_luminance = __uint_as_float(data.w);

  float4 out_color = exposure_texture(make_float4(color.x, color.y, color.z, 1.0f),
                                      exposure,
                                      multiplier,
                                      world_luminance,
                                      max_luminance);

  if (stack_valid(out_color_offset))
    stack_store_float3(
        stack, out_color_offset, make_float3(out_color.x, out_color.y, out_color.z));
}

ccl_device float fbm(KernelGlobals *kg, float3 P, bool is_turbulent, float omega, int maxOctaves)
{
  float sum = 0.0, lambda = 1.0, o = 1.0;
  for (int i = 0; i < maxOctaves; ++i) {
    float3 lambda_p = lambda * P;
    float noise_value = noise(kg, lambda_p.x, lambda_p.y, lambda_p.z);

    if (is_turbulent)
      noise_value = fabsf(noise_value);

    sum += o * noise_value;
    lambda *= 1.99;
    o *= omega;
  }

  return sum;
}

ccl_device float4 fbm_texture(KernelGlobals *kg,
                              float3 uvw,
                              float4 color1,
                              float4 color2,
                              bool is_turbulent,
                              int max_octaves,
                              float gain,
                              float roughness)
{
  float t = fabsf(fbm(kg, uvw, is_turbulent, roughness, max_octaves) * gain);

  float4 color_out = mix(color1, color2, t);
  color_out = clamp(color_out, make_float4(0.0f), make_float4(1.0f));

  return color_out;
}

ccl_device void svm_rhino_node_fbm_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, in_color1_offset, in_color2_offset, out_color_offset;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &in_color1_offset, &in_color2_offset, &out_color_offset);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);
  float3 color1 = stack_load_float3(stack, in_color1_offset);
  float3 color2 = stack_load_float3(stack, in_color2_offset);

  uint4 data = read_node(kg, offset);

  bool is_turbulent = (bool)data.x;
  int max_octaves = data.y;
  float gain = __uint_as_float(data.z);
  float roughness = __uint_as_float(data.w);

  float4 out_color = fbm_texture(kg,
                                 uvw,
                                 make_float4(color1.x, color1.y, color1.z, 1.0),
                                 make_float4(color2.x, color2.y, color2.z, 1.0),
                                 is_turbulent,
                                 max_octaves,
                                 gain,
                                 roughness);

  if (stack_valid(out_color_offset))
    stack_store_float3(
        stack, out_color_offset, make_float3(out_color.x, out_color.y, out_color.z));
}

ccl_device int get_segment(float u, float v, float thickness, float margin)
{
  if (u < 2.0 * margin || 1.0 - 2.0 * margin < u || v < margin) {
    return 0;
  }

  if (u > 2.0 * (margin + thickness) && u < 1.0 - 2.0 * (margin + thickness) &&
      v > margin + thickness && v < 0.5 - 0.5 * thickness) {
    return 0;
  }

  if (u - 2.0 * v > 0.0 && u + 2.0 * v < 1.0) {
    return 1;
  }

  if (0.25 * u + v > 0.5 * margin + 0.5 && 0.25 * u - v < -0.5 * margin - 0.25) {
    return 4;
  }

  if (u < 0.5) {
    return 2;
  }

  return 3;
}

ccl_device int test_digit(int n, float u, float v, float thickness, float margin)
{
  /* clang-format off */

  int segLU[80] = {
    0, 2, 2, 2, 1, 2, 2, 2 , // 0
    0, 1, 1, 2, 1, 2, 1, 1 , // 1
    0, 2, 1, 2, 2, 1, 2, 2 , // 2
    0, 2, 1, 2, 2, 2, 1, 2 , // 3
    0, 1, 2, 2, 2, 2, 1, 1 , // 4
    0, 2, 2, 1, 2, 2, 1, 2 , // 5
    0, 2, 2, 1, 2, 2, 2, 2 , // 6
    0, 2, 1, 2, 1, 2, 1, 1 , // 7
    0, 2, 2, 2, 2, 2, 2, 2 , // 8
    0, 2, 2, 2, 2, 2, 1, 2   // 9
  };

  /* clang-format on */

  if (n < 0 || 9 < n)
    return 0;

  bool mirror = v > 0.5;
  if (mirror) {
    v = 1.0 - v;
  }

  int s = get_segment(u, v, thickness, margin);

  if (s < 0)
    return 0;

  if (mirror) {
    s = (8 - s) % 8;
  }

  return segLU[8 * n + s];
}

ccl_device int TestNumber(int digits, int number, float thickness, float margin, float u, float v)
{
  float digitIndexDbl = 0.0;
  float digitU = modff(u * digits, &digitIndexDbl);
  float digitV = v;
  int digitIndex = int(digitIndexDbl);

  if (0 <= digitIndex && digitIndex < digits) {
    int nU = number;
    int digit = 0;
    for (int i = 0; i < digits - digitIndex; i++) {
      digit = nU % 10;
      nU -= digit;
      nU /= 10;
    }
    return test_digit(digit, digitU, digitV, thickness, margin);
  }

  return 0;
}

ccl_device float4
grid_texture(float3 uvw, float4 color1, float4 color2, int cells, float font_thickness)
{
  float u = fractf(uvw.x);
  float v = fractf(uvw.y);

  float gridU = 0.0;
  float gridV = 0.0;
  float cellU = 0.0;
  float cellV = 0.0;

  cellU = modff(cells * u, &gridU);
  cellV = modff(cells * v, &gridV);

  int cellParity = (int(gridU) + int(gridV)) % 2;

  float3 white = make_float3(1.0f, 1.0f, 1.0f);
  float3 black = make_float3(0.0f, 0.0f, 0.0f);
  float4 colBackground = (cellParity == 0) ? color1 : color2;
  float3 colNumber = Luminance(make_float3(colBackground.x, colBackground.y, colBackground.z)) <
                             0.5f ?
                         white :
                         black;

  int cellNumber = int(gridU) + cells * (cells - 1 - int(gridV));

  int maxCellNumber = cells * cells - 1;

  int digitCount = maxCellNumber < 10   ? 1 :
                   maxCellNumber < 100  ? 2 :
                   maxCellNumber < 1000 ? 3 :
                   maxCellNumber < 1000 ? 4 :
                                          5;
  float numberStartU = (5 - digitCount) * 0.1;
  float numberEndU = 1.0 - numberStartU;
  float numberStartV = 0.3;
  float numberEndV = 0.7;
  float numberU = (cellU - numberStartU) / (numberEndU - numberStartU);
  float numberV = (cellV - numberStartV) / (numberEndV - numberStartV);

  int hitResult = TestNumber(
      digitCount, cellNumber, 0.12 * font_thickness, 0.1, numberU, 1.0 - numberV);

  if (2 == hitResult)
    return make_float4(colNumber.x, colNumber.y, colNumber.z, 1);

  return colBackground;
}

ccl_device void svm_rhino_node_grid_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, in_color1_offset, in_color2_offset, out_color_offset;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &in_color1_offset, &in_color2_offset, &out_color_offset);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);
  float3 color1 = stack_load_float3(stack, in_color1_offset);
  float3 color2 = stack_load_float3(stack, in_color2_offset);

  uint4 data = read_node(kg, offset);

  int cells = data.x;
  float font_thickness = __uint_as_float(data.y);

  float4 out_color = grid_texture(uvw,
                                  make_float4(color1.x, color1.y, color1.z, 1.0),
                                  make_float4(color2.x, color2.y, color2.z, 1.0),
                                  cells,
                                  font_thickness);

  if (stack_valid(out_color_offset))
    stack_store_float3(
        stack, out_color_offset, make_float3(out_color.x, out_color.y, out_color.z));
}

ccl_device float3 light_probe_to_world(float2 uv)
{
  float u = uv.x;
  float v = uv.y;

  float d = sqrt((0.5f - u) * (0.5f - u) + (0.5f - v) * (0.5f - v)) * 2.0f;
  float cosine = cos(PI * d);
  float factor = 2.0f * sqrt(1.0f - cosine * cosine);

  float3 vec;
  vec.x = (u - 0.5f) * factor / d;
  vec.y = (0.5f - v) * factor / d;
  vec.z = cosine;

  return vec;
}

ccl_device float2 world_to_lightprobe(float3 n)
{
  float x = n.x;
  float y = n.y;
  float z = n.z;

  float fDivisor = sqrt(x * x + y * y);

  float f = (acos(z) / PI) / fDivisor;

  float px = x * f;
  float py = y * f;

  float u = (px + 1.0f) * 0.5f;
  float v = (-py + 1.0f) * 0.5f;

  return make_float2(u, v);
}

ccl_device float3 equirect_to_world(float2 uv)
{
  float theta = (uv.x - 0.5f) * TWO_PI;
  float phi = -(uv.y - 0.5f) * PI;

  float cosphi = cos(phi);

  float3 vec;
  vec.x = sin(theta) * cosphi;
  vec.y = sin(phi);
  vec.z = cos(theta) * cosphi;

  return vec;
}

ccl_device float2 world_to_equirect(float3 n)
{
  float x = -n.z;
  float y = -n.x;
  float z = n.y;

  float theta, phi;

  if (x == 0.0 && y == 0.0) {
    theta = 0.0;
    phi = (z >= 0.0 ? 0.5 * PI : -0.5 * PI);
  }
  else {
    theta = atan2(y, x);
    if (theta < 0.0)
      theta += 2.0 * PI;

    float r;
    if (fabsf(x) >= fabsf(y)) {
      r = y / x;
      r = fabsf(x) * sqrt(1.0 + r * r);
    }
    else {
      r = x / y;
      r = fabsf(y) * sqrt(1.0 + r * r);
    }

    phi = atan(z / r);
  }

  float u = theta / (2.0 * PI);
  float v = (-phi + 0.5 * PI) / PI;

  return make_float2(u, v);
}

ccl_device float3 cubemap_to_world(float2 uv)
{
  float u = uv.x;
  float v = uv.y;

  float uCube = u - floorf(u);
  float vCube = v - floorf(v);
  int subTextureIndex = int(floorf(uCube * 6.0f));
  float subTextureOffset = float(subTextureIndex / 6.0f);
  float uSubTex = (uCube - subTextureOffset) * 6.0f;
  float vSubTex = vCube;
  float sc = 2.0f * uSubTex - 1.0f;
  float tc = 2.0f * vSubTex - 1.0f;

  float3 vec = make_float3(0.0f, 0.0f, 0.0f);

  switch (subTextureIndex) {
    case 0:
      vec.z = -sc;
      vec.y = -tc;
      vec.x = 1.0f;
      break;
    case 1:
      vec.z = sc;
      vec.y = -tc;
      vec.x = -1.0f;
      break;
    case 2:
      vec.x = sc;
      vec.z = -tc;
      vec.y = -1.0f;
      break;
    case 3:
      vec.x = sc;
      vec.z = tc;
      vec.y = 1.0f;
      break;
    case 4:
      vec.x = sc;
      vec.y = -tc;
      vec.z = 1.0f;
      break;
    case 5:
      vec.x = -sc;
      vec.y = -tc;
      vec.z = -1.0f;
      break;
  }

  return normalize(vec);
}

ccl_device int get_main_axis_index(float3 v)
{
  v.x = fabsf(v.x);
  v.y = fabsf(v.y);
  v.z = fabsf(v.z);

  if (v.x > v.y && v.x > v.z) {
    return 0;
  }
  else if (v.y > v.z) {
    return 1;
  }
  else {
    return 2;
  }
}

ccl_device float2 world_to_cubemap(float3 n)
{
  int mainAxis = get_main_axis_index(n);
  float mainAxisDir = n[mainAxis];

  int subTextureIndex;

  switch (mainAxis) {
    case 0:
      subTextureIndex = (mainAxisDir >= 0.0f ? 0 : 1);
      break;
    case 1:
      subTextureIndex = (mainAxisDir >= 0.0f ? 3 : 2);
      break;
    case 2:
      subTextureIndex = (mainAxisDir >= 0.0f ? 4 : 5);
      break;
  }

  float subTextureOffset = float(subTextureIndex) / 6.0f;
  float ma = fabsf(mainAxisDir);

  float sc = 0.0f;
  float tc = 0.0f;

  switch (subTextureIndex) {
    case 0:
      sc = -n.z;
      tc = -n.y;
      break;
    case 1:
      sc = n.z;
      tc = -n.y;
      break;
    case 2:
      sc = n.x;
      tc = -n.z;
      break;
    case 3:
      sc = n.x;
      tc = n.z;
      break;
    case 4:
      sc = n.x;
      tc = -n.y;
      break;
    case 5:
      sc = -n.x;
      tc = -n.y;
      break;
  }

  float u = (sc / ma + 1.0f) / 12.0f + subTextureOffset;
  float v = (tc / ma + 1.0f) / 2.0f;

  return make_float2(u, v);
}

ccl_device float3 vertical_cross_cubemap_to_world(float2 uv)
{
  float u = uv.x;
  float v = uv.y;

  float uCube = u - floorf(u);
  float vCube = v - floorf(v);

  const float minU = 1.0f / 3.0f;
  const float maxU = 2.0f / 3.0f;
  const float minV = 2.0f / 4.0f;
  const float maxV = 3.0f / 4.0f;
  const float borderU = 1.0f / 360.0f;
  const float borderV = 1.0f / 360.0f;

  float uLtd = (uCube < minU ? minU : (maxU < uCube ? maxU : uCube));
  float vLtd = (vCube < minV ? minV : (maxV < vCube ? maxV : vCube));
  float uAdj = fabsf(uLtd - uCube);
  float vAdj = fabsf(vLtd - vCube);

  if (uAdj != 0.0 && vAdj != 0.0) {
    if (uAdj <= vAdj && uAdj < borderU) {
      uCube = uLtd;
    }
    else if (vAdj <= uAdj && vAdj < borderV) {
      vCube = vLtd;
    }
  }

  int subTexU = int(floorf(3.0f * uCube));
  int subTexV = int(floorf(4.0f * vCube));
  float subTextureStartU = float(subTexU / 3.0f);
  float subTextureStartV = float(subTexV / 4.0f);
  float uSubTex = 3.0f * (uCube - subTextureStartU);
  float vSubTex = 4.0f * (vCube - subTextureStartV);
  float sc = 2.0f * uSubTex - 1.0f;
  float tc = 2.0f * vSubTex - 1.0f;

  int subTextureIndex = -1;
  if (subTexU == 0) {
    if (subTexV == 2)
      subTextureIndex = 1;
  }
  else {
    if (subTexU == 1) {
      if (subTexV == 0)
        subTextureIndex = 5;
      else if (subTexV == 1)
        subTextureIndex = 2;
      else if (subTexV == 2)
        subTextureIndex = 4;
      else if (subTexV == 3)
        subTextureIndex = 3;
    }
    else {
      if (subTexU == 2) {
        if (subTexV == 2)
          subTextureIndex = 0;
      }
    }
  }

  if (subTextureIndex == -1)
    return make_float3(0.0f);

  float3 vec = make_float3(0.0f);

  switch (subTextureIndex) {
    case 0:
      vec.z = -sc;
      vec.y = -tc;
      vec.x = 1.0f;
      break;
    case 1:
      vec.z = sc;
      vec.y = -tc;
      vec.x = -1.0f;
      break;
    case 2:
      vec.x = sc;
      vec.z = tc;
      vec.y = 1.0f;
      break;
    case 3:
      vec.x = sc;
      vec.z = -tc;
      vec.y = -1.0f;
      break;
    case 4:
      vec.x = sc;
      vec.y = -tc;
      vec.z = 1.0f;
      break;
    case 5:
      vec.x = sc;
      vec.y = tc;
      vec.z = -1.0f;
      break;
  }

  return normalize(vec);
}

ccl_device float2 world_to_vertical_cross_cubemap(float3 n)
{
  float2 st = make_float2(0.0f, 0.0f);

  const float uI[6] = {2.0 / 3.0, 0.0, 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0};
  const float vI[6] = {2.0 / 4.0, 2.0 / 4.0, 1.0 / 4.0, 3.0 / 4.0, 2.0 / 4.0, 0.0};

  float2 uva[6] = {make_float2(-n.z, -n.y),
                   make_float2(n.z, -n.y),
                   make_float2(n.x, n.z),
                   make_float2(n.x, -n.z),
                   make_float2(n.x, -n.y),
                   make_float2(n.x, n.y)};

  int mainAxis = get_main_axis_index(n);
  float mainAxisDir = n[mainAxis];
  int subTextureIndex = (2 * mainAxis) + (mainAxisDir >= 0.0f ? 0 : 1);

  float2 SubTexStart = make_float2(uI[subTextureIndex], vI[subTextureIndex]);
  st = (uva[subTextureIndex] / fabsf(mainAxisDir) + 1.0) / make_float2(6.0, 8.0) + SubTexStart;

  return st;
}

ccl_device float3 horizontal_cross_cubemap_to_world(float2 uv)
{
  float u = uv.x;
  float v = uv.y;

  float uCube = u - floorf(u);
  float vCube = v - floorf(v);

  const float minU = 1.0f / 4.0f;
  const float maxU = 2.0f / 4.0f;
  const float minV = 1.0f / 3.0f;
  const float maxV = 2.0f / 3.0f;
  const float borderU = 1.0f / 360.0f;
  const float borderV = 1.0f / 360.0f;

  float uLtd = (uCube < minU ? minU : (maxU < uCube ? maxU : uCube));
  float vLtd = (vCube < minV ? minV : (maxV < vCube ? maxV : vCube));
  float uAdj = fabsf(uLtd - uCube);
  float vAdj = fabsf(vLtd - vCube);

  if (uAdj != 0.0 && vAdj != 0.0) {
    if (uAdj <= vAdj && uAdj < borderU) {
      uCube = uLtd;
    }
    else if (vAdj <= uAdj && vAdj < borderV) {
      vCube = vLtd;
    }
  }

  int subTexU = int(floorf(4.0f * uCube));
  int subTexV = int(floorf(3.0f * vCube));
  float subTextureStartU = float(subTexU / 4.0f);
  float subTextureStartV = float(subTexV / 3.0f);
  float uSubTex = 4.0f * (uCube - subTextureStartU);
  float vSubTex = 3.0f * (vCube - subTextureStartV);
  float sc = 2.0f * uSubTex - 1.0f;
  float tc = 2.0f * vSubTex - 1.0f;

  int subTextureIndex = -1;
  if (subTexV == 0) {
    if (subTexU == 1)
      subTextureIndex = 2;
  }
  else {
    if (subTexV == 1) {
      if (subTexU == 0)
        subTextureIndex = 1;
      else if (subTexU == 1)
        subTextureIndex = 4;
      else if (subTexU == 2)
        subTextureIndex = 0;
      else if (subTexU == 3)
        subTextureIndex = 5;
    }
    else {
      if (subTexV == 2) {
        if (subTexU == 1)
          subTextureIndex = 3;
      }
    }
  }

  if (subTextureIndex == -1)
    return make_float3(0.0f);

  float3 vec = make_float3(0.0f);

  switch (subTextureIndex) {
    case 0:
      vec.z = -sc;
      vec.y = -tc;
      vec.x = 1.0f;
      break;
    case 1:
      vec.z = sc;
      vec.y = -tc;
      vec.x = -1.0f;
      break;
    case 2:
      vec.x = sc;
      vec.z = tc;
      vec.y = 1.0f;
      break;
    case 3:
      vec.x = sc;
      vec.z = -tc;
      vec.y = -1.0f;
      break;
    case 4:
      vec.x = sc;
      vec.y = -tc;
      vec.z = 1.0f;
      break;
    case 5:
      vec.x = -sc;
      vec.y = -tc;
      vec.z = -1.0f;
      break;
  }

  return normalize(vec);
}

ccl_device float2 world_to_horizontal_cross_cubemap(float3 n)
{
  float2 st = make_float2(0.0f, 0.0f);

  const float uI[6] = {2.0 / 4.0, 0.0, 1.0 / 4.0, 1.0 / 4.0, 1.0 / 4.0, 3.0 / 4.0};
  const float vI[6] = {1.0 / 3.0, 1.0 / 3.0, 0.0, 2.0 / 3.0, 1.0 / 3.0, 1.0 / 3.0};

  float2 uva[6] = {make_float2(-n.z, -n.y),
                   make_float2(n.z, -n.y),
                   make_float2(n.x, n.z),
                   make_float2(n.x, -n.z),
                   make_float2(n.x, -n.y),
                   make_float2(-n.x, -n.y)};

  int mainAxis = get_main_axis_index(n);
  float mainAxisDir = n[mainAxis];
  int subTextureIndex = (2 * mainAxis) + (mainAxisDir >= 0.0f ? 0 : 1);

  float2 SubTexStart = make_float2(uI[subTextureIndex], vI[subTextureIndex]);
  st = (uva[subTextureIndex] / fabsf(mainAxisDir) + 1.0) / make_float2(8.0, 6.0) + SubTexStart;

  return st;
}

ccl_device float3 emap_to_world(float2 uv)
{
  float x = 2.0f * uv.x - 1.0f;
  float y = -(2.0f * uv.y - 1.0f);

  float r = sqrt(x * x + y * y);
  r = min(r, 1.0f);

  float t = 2.0f * asin(r);
  float s = sin(t) / r;

  float3 vec;
  vec.x = -s * x;
  vec.y = s * y;
  vec.z = cos(t);

  return vec;
}

ccl_device float2 world_to_emap(float3 n)
{
  float x = n.x;
  float y = n.y;
  float z = n.z;

  float m = 2.0 * sqrt((x * x) + (y * y) + (z + 1.0) * (z + 1.0));

  float u = x / m + 0.5;
  float v = y / m + 0.5;

  return make_float2(u, v);
}

ccl_device float3 hemispherical_to_world(float2 uv)
{
  return equirect_to_world(make_float2(uv.x, 0.5f + 0.5f * uv.y));
}

ccl_device float2 world_to_hemispherical(float3 n)
{
  float3 hemi = make_float3(n.x, min(n.y, 0.0f), n.z);

  float2 uv = world_to_equirect(normalize(hemi));

  uv.y = 2.0f * uv.y - 1.0f;

  return uv;
}

ccl_device Transform rotation_matrix(float3 axis, float angle)
{
  float s = -sin(angle);
  float c = cos(angle);
  float oc = 1.0 - c;

  Transform tf;
  tf.x.x = oc * axis.x * axis.x + c;
  tf.x.y = oc * axis.x * axis.y - axis.z * s;
  tf.x.z = oc * axis.z * axis.x + axis.y * s;
  tf.x.w = 0.0f;

  tf.y.x = oc * axis.x * axis.y + axis.z * s;
  tf.y.y = oc * axis.y * axis.y + c;
  tf.y.z = oc * axis.y * axis.z - axis.x * s;
  tf.y.w = 0.0f;

  tf.z.x = oc * axis.z * axis.x - axis.y * s;
  tf.z.y = oc * axis.y * axis.z + axis.x * s;
  tf.z.z = oc * axis.z * axis.z + c;
  tf.z.w = 0.0f;

  return tf;
}

ccl_device bool is_parallel_to(float3 a, float3 b)
{
  return len(cross(a, b)) < 1e-6;
}

ccl_device float4 projection_changer_texture(float3 uvw,
                                             RhinoProceduralProjectionType input_projection_type,
                                             RhinoProceduralProjectionType output_projection_type,
                                             float azimuth,
                                             float altitude)
{
  output_projection_type = (output_projection_type == RHINO_PROJECTION_SAME_AS_INPUT) ?
                               input_projection_type :
                               output_projection_type;

  uvw = make_float3(fractf(uvw.x), fractf(uvw.y), fractf(uvw.z));

  bool bQuick = false;

  if (input_projection_type == RHINO_PROJECTION_PLANAR)
    bQuick = true;
  else if ((output_projection_type == input_projection_type) && (azimuth == 0.0f) &&
           (altitude == 0.0f))
    bQuick = true;

  if (!bQuick) {
    float3 vec = make_float3(0);

    switch (output_projection_type) {
      case RHINO_PROJECTION_LIGHTPROBE:
        vec = light_probe_to_world(make_float2(uvw.x, uvw.y));
        break;
      case RHINO_PROJECTION_EQUIRECT:
        vec = equirect_to_world(make_float2(uvw.x, uvw.y));
        break;
      case RHINO_PROJECTION_CUBEMAP:
        vec = cubemap_to_world(make_float2(uvw.x, uvw.y));
        break;
      case RHINO_PROJECTION_VERTICAL_CROSS_CUBEMAP:
        vec = vertical_cross_cubemap_to_world(make_float2(uvw.x, uvw.y));
        break;
      case RHINO_PROJECTION_HORIZONTAL_CROSS_CUBEMAP:
        vec = horizontal_cross_cubemap_to_world(make_float2(uvw.x, uvw.y));
        break;
      case RHINO_PROJECTION_EMAP:
        vec = emap_to_world(make_float2(uvw.x, uvw.y));
        break;
      case RHINO_PROJECTION_HEMISPHERICAL:
        vec = hemispherical_to_world(make_float2(uvw.x, uvw.y));
        break;
      default:
        vec = uvw;
        break;
    }

    float3 rotate_axis = make_float3(0, 1, 0);

    Transform transform = rotation_matrix(rotate_axis, azimuth);
    vec = transform_direction(&transform, vec);

    if (!is_parallel_to(vec, rotate_axis)) {
      float3 cross_axis = normalize(cross(vec, rotate_axis));
      transform = rotation_matrix(cross_axis, altitude);
      vec = transform_direction(&transform, vec);
    }

    float2 uv = make_float2(0.0f, 0.0f);

    switch (input_projection_type) {
      case RHINO_PROJECTION_LIGHTPROBE:
        uv = world_to_lightprobe(vec);
        break;
      case RHINO_PROJECTION_EQUIRECT:
        uv = world_to_equirect(vec);
        break;
      case RHINO_PROJECTION_CUBEMAP:
        uv = world_to_cubemap(vec);
        break;
      case RHINO_PROJECTION_VERTICAL_CROSS_CUBEMAP:
        uv = world_to_vertical_cross_cubemap(vec);
        break;
      case RHINO_PROJECTION_HORIZONTAL_CROSS_CUBEMAP:
        uv = world_to_horizontal_cross_cubemap(vec);
        break;
      case RHINO_PROJECTION_EMAP:
        uv = world_to_emap(vec);
        break;
      case RHINO_PROJECTION_HEMISPHERICAL:
        uv = world_to_hemispherical(vec);
        break;
      default:
        uv = make_float2(vec.x, vec.y);
        break;
    }

    uvw.x = uv.x;
    uvw.y = uv.y;
  }

  return make_float4(uvw.x, uvw.y, uvw.z, 1.0);
}

ccl_device void svm_rhino_node_projection_changer_texture(
    KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node, int *offset)
{
  uint in_uvw_offset, out_uvw_offset;
  uint dummy;

  svm_unpack_node_uchar4(
      node.y, &in_uvw_offset, &out_uvw_offset, &dummy, &dummy);

  float3 uvw = stack_load_float3(stack, in_uvw_offset);

  uint4 data = read_node(kg, offset);

  RhinoProceduralProjectionType input_projection_type = (RhinoProceduralProjectionType)data.x;
  RhinoProceduralProjectionType output_projection_type = (RhinoProceduralProjectionType)data.y;
  float azimuth = __uint_as_float(data.z);
  float altitude = __uint_as_float(data.w);

  //float4 out_uvw = make_float4(uvw.x, uvw.y, uvw.z, 1.0);
  float4 out_uvw = projection_changer_texture(uvw, input_projection_type, output_projection_type, azimuth, altitude);

  if (stack_valid(out_uvw_offset))
    stack_store_float3(stack, out_uvw_offset, make_float3(out_uvw.x, out_uvw.y, out_uvw.z));
}

CCL_NAMESPACE_END
