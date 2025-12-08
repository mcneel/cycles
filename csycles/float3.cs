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

    [StructLayout(LayoutKind.Sequential)]
    public struct float3
    {
        public float x;
        public float y;
        public float z;
        private float _pad = 0;

        public float3()
        {
            x = 0;
            y = 0;
            z = 0;
        }

        public float3(float3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;
        }

        public float3(float v)
        {
            x = v;
            y = v;
            z = v;
        }

        public float3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static implicit operator float3((float x, float y, float z, float w) v)
        {
            return new float3(v.x, v.y, v.z);
        }

        public static implicit operator float3(float4 v)
        {
            return new float3(v.x, v.y, v.z);
        }

        public static implicit operator float3(float x)
        {
            return new float3(x);
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public float3 Normalize(float epsilon = 1e-6f)
        {
            var length = Length();
            if (length < epsilon)
            {
                return new float3(0.0f);
            }
            float inv = 1.0f / length;
            return new float3(x * inv, y * inv, z * inv);
        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
    }
}