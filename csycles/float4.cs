/**
Copyright 2014-2025 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

----------------------------------------------------------------------
NOTE: Do NOT modify this file directly, it is automatically generated.

Code generated at: 2025-10-13 06:03:25 UTC
----------------------------------------------------------------------

**/

using ccl;
using ccl.Attributes;
using ccl.ShaderNodes.Sockets;
using System;
using System.Xml;
using System.Reflection;
using System.Runtime.InteropServices;

namespace ccl
{
    using cclext;
    using System.Runtime.CompilerServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct float4
    {
        public float x;
        public float y;
        public float z;
        public float w;

        public float4(float x, float y, float z, float w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }

        public float4(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = 0f;
        }

        public float4(float v)
        {
            x = v;
            y = v;
            z = v;
            w = v;
        }
        public float4(float4 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
            w = v.w;
        }

        public float4(float[] floats)
        {
            if(floats.Length < 4)
                throw new ArgumentException("Array must contain at least 4 elements");
            x = floats[0];
            y = floats[1];
            z = floats[2];
            w = floats[3];
        }

        public float this[int i]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                switch(i)
                {
                    case 0: return x;
                    case 1: return y;
                    case 2: return z;
                    case 3: return w;
                    default:
                        throw new IndexOutOfRangeException("Invalid float4 index!");
                }
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                switch(i)                 {
                    case 0: x = value; break;
                    case 1: y = value; break;
                    case 2: z = value; break;
                    case 3: w = value; break;
                    default:
                        throw new IndexOutOfRangeException("Invalid float4 index!");
                }
            }
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z}, {w})";
        }

        /// <summary>
        /// Assume this float4 is a color representation and
        /// apply gamma to the x, y and z channels if
        /// gamma != 1.0f;
        ///
        /// pow(channel, gamma)
        /// </summary>
        /// <param name="gamma"></param>
        public static float4 operator ^(float4 a, float gamma)
        {
            if (Math.Abs(1.0f - gamma) > float.Epsilon)
            {
                return new float4((float)Math.Pow(a.x, gamma), (float)Math.Pow(a.y, gamma), (float)Math.Pow(a.z, gamma), a.w);
            }
            return a;
        }

        public static float4 operator /(float4 a, float4 b)
        {
            return new float4(a.x / b.x, a.y / b.y, a.z / b.z, a.w / b.w);
        }

        public static float4 operator /(float4 a, float b)
        {
            var inv = 1.0f / b;
            return new float4(a.x * inv, a.y * inv, a.z * inv, a.w * inv);
        }

        public static float4 operator /(float a, float4 b)
        {
            return new float4(a / b.x, a / b.y, a / b.z, a / b.w);
        }

        public static float4 operator *(float4 a, float4 b)
        {
            return new float4(a.x * b.x, a.y * b.y, a.z * b.z, a.w * b.w);
        }

        public static float4 operator *(float4 a, float b)
        {
            return new float4(a.x * b, a.y * b, a.z * b, a.w * b);
        }

        public static float4 operator *(float a, float4 b)
        {
            return new float4(b.x * a, b.y * a, b.z * a, b.w * a);
        }

        public static float4 operator +(float4 a, float4 b)
        {
            return new float4(a.x + b.x, a.y + b.y, a.z + b.z, a.w + b.w);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if(obj.GetType() != typeof(float4)) return false;
            return this == (float4)obj;
        }

        public override int GetHashCode()
        {
            return 43 * x.GetHashCode() +
                   47 * y.GetHashCode() +
                   53 * z.GetHashCode() +
                   59 * w.GetHashCode();
        }

        public static bool operator==(float4 lhs, float4 rhs)
        {
            if(lhs==null || rhs==null) return false;
            return lhs.x==rhs.x && lhs.y==rhs.y && lhs.z==rhs.z && lhs.w==rhs.w;
        }

        public static bool operator!=(float4 lhs, float4 rhs)
        {
            return !(lhs==rhs);
        }

        /// <summary>
        /// Transform point a with Transform t
        /// </summary>
        /// <param name="t"></param>
        /// <param name="a"></param>
        /// <returns></returns>
        public static float4 operator *(Transform t, float4 a)
        {

            float4 c = new float4(
                a.x * t.x.x + a.y * t.x.y + a.z * t.x.z + t.x.w,
                a.x * t.y.x + a.y * t.y.y + a.z * t.y.z + t.y.w,
                a.x * t.z.x + a.y * t.z.y + a.z * t.z.z + t.z.w);

            return c;
        }

        public float Length()
        {
            return (float)Math.Sqrt(Dot(this, this));
        }

        public static float Dot(float4 a, float4 b)
        {
            return (a.x * b.x + a.y * b.y) + (a.z * b.z + a.w * b.w);
        }

        public static float4 Normalize(float4 a)
        {
            return a / a.Length();
        }

        public bool IsZero(bool checkW)
        {
            if (checkW)
                return Math.Abs(x) < 0.00001f && Math.Abs(y) < 0.00001f
                    && Math.Abs(z) < 0.00001f && Math.Abs(w) < 0.00001f;

            return Math.Abs(x) < 0.00001f && Math.Abs(y) < 0.00001f
                && Math.Abs(z) < 0.00001f;
        }

        private static float srgb_to_linear(float c)
        {
            if (c < 0.04045f)
                return (c < 0.0f) ? 0.0f : c * (1.0f / 12.92f);
            else
                return (float)Math.Pow((c + 0.055f) * (1.0f / 1.055f), 2.4f);

        }

        /// <summary>
        /// Apply sRGB to linear conversion on RGB. The A is kept as is.
        /// </summary>
        /// <param name="f4"></param>
        /// <returns></returns>
        public static float4 SrgbToLinear(float4 f4)
        {
            return new float4(
                srgb_to_linear(f4.x),
                srgb_to_linear(f4.y),
                srgb_to_linear(f4.z),
                f4.w
            )
            ;
        }
    }
}
