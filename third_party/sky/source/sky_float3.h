/* SPDX-License-Identifier: Apache-2.0
 * Copyright 2026 Robert McNeel & Associates */

/* ALB 2026.11.28 RH-100740 Replaces Blender's GPL header with the float3 math sky_nishita.cpp uses. */

#ifndef __SKY_FLOAT3_H__
#define __SKY_FLOAT3_H__

#include <math.h>

#ifndef M_PI_F
#  define M_PI_F (3.1415926535897932f)
#endif
#ifndef M_PI_2_F
#  define M_PI_2_F (1.5707963267948966f)
#endif
#ifndef M_2PI_F
#  define M_2PI_F (6.2831853071795864f)
#endif

struct float3 {
  float x, y, z;
};

inline float3 make_float3(float x, float y, float z)
{
  return {x, y, z};
}

inline float3 operator+(const float3 &a, const float3 &b)
{
  return {a.x + b.x, a.y + b.y, a.z + b.z};
}

inline float3 operator-(const float3 &a, const float3 &b)
{
  return {a.x - b.x, a.y - b.y, a.z - b.z};
}

inline float3 operator-(const float3 &a)
{
  return {-a.x, -a.y, -a.z};
}

inline float3 operator*(const float3 &a, const float3 &b)
{
  return {a.x * b.x, a.y * b.y, a.z * b.z};
}

inline float3 operator*(const float3 &a, float s)
{
  return {a.x * s, a.y * s, a.z * s};
}

inline float3 operator*(float s, const float3 &a)
{
  return a * s;
}

inline float3 &operator+=(float3 &a, const float3 &b)
{
  a = a + b;
  return a;
}

inline float sqr(float a)
{
  return a * a;
}

inline float dot(const float3 &a, const float3 &b)
{
  return a.x * b.x + a.y * b.y + a.z * b.z;
}

inline float len_squared(const float3 &a)
{
  return dot(a, a);
}

inline float len(const float3 &a)
{
  return sqrtf(len_squared(a));
}

inline float distance(const float3 &a, const float3 &b)
{
  return len(a - b);
}

inline float reduce_add(const float3 &a)
{
  return a.x + a.y + a.z;
}

#endif /* __SKY_FLOAT3_H__ */
