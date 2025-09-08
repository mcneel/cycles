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
    public struct Transform
    {
        public float4 x;
        public float4 y;
        public float4 z;

        public Transform(float4 x, float4 y, float4 z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Transform(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23)
        {
            x = new float4(m00, m01, m02, m03);
            y = new float4(m10, m11, m12, m13);
            z = new float4(m20, m21, m22, m23);
        }

        public Transform(Transform t)
        {
            x = t.x;
            y = t.y;
            z = t.z;
        }

        public Transform(float[] data) : this(
        data[0], data[1], data[2], data[3],
        data[4], data[5], data[6], data[7],
        data[8], data[9], data[10], data[11]
        )
        {

        }

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }

        public void SetMatrix(
            float m00, float m01, float m02, float m03,
            float m10, float m11, float m12, float m13,
            float m20, float m21, float m22, float m23)
        {
            x.x = m00; x.y = m01; x.z = m02; x.w = m03;
            y.x = m10; y.y = m11; y.z = m12; y.w = m13;
            z.x = m20; z.y = m21; z.z = m22; z.w = m23;
        }

        public void SetMatrix(float[] matrix)
        {
            if (matrix.Length < 12)
                throw new ArgumentException("Matrix array must have at least 12 elements");

            x.x = matrix[0]; x.y = matrix[1]; x.z = matrix[2]; x.w = matrix[3];
            y.x = matrix[4]; y.y = matrix[5]; y.z = matrix[6]; y.w = matrix[7];
            z.x = matrix[8]; z.y = matrix[9]; z.z = matrix[10]; z.w = matrix[11];
        }

        /// <summary>
        /// The identity matrix
        /// </summary>
        /// <returns></returns>
        static public Transform Identity()
        {
            return Scale(1.0f, 1.0f, 1.0f);
        }

        /// <summary>
        /// Give scale matrix for scale vector (x,y,z)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        static public Transform Scale(float x, float y, float z)
        {
            return new Transform(
                x, 0.0f, 0.0f, 0.0f,
                0.0f, y, 0.0f, 0.0f,
                0.0f, 0.0f, z, 0.0f
                );
        }

        public float this[int i, int j]
        {
            get {
                switch (i)
                {
                    case 0:
                        return x[j];
                    case 1:
                        return y[j];
                    case 2:
                        return z[j];
                    default:
                        throw new IndexOutOfRangeException("Invalid Transform i-index!");
                }
            }
            set {
                switch (i)
                {
                    case 0:
                        x[j] = value;
                        break;
                    case 1:
                        y[j] = value;
                        break;
                    case 2:
                        z[j] = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException("Invalid Transform i-index!");
                }
            }
        }

        /// <summary>
		/// Give translation matrix for vector t
		/// </summary>
		/// <param name="t"></param>
		/// <returns></returns>
		static public Transform Translate(float4 t)
        {
            return new Transform(
                1, 0, 0, t.x,
                0, 1, 0, t.y,
                0, 0, 1, t.z
                );
        }

        /// <summary>
        /// Give translation matrix for translation vector (x,y,z)
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        static public Transform Translate(float x, float y, float z)
        {
            return Translate(new float4(x, y, z));
        }

        /// <summary>
		/// Dot product of two matrices
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		static public Transform operator *(Transform a, Transform b)
        {
            float4 c_x = new float4(b.x.x, b.y.x, b.z.x, 0.0f);
            float4 c_y = new float4(b.x.y, b.y.y, b.z.y, 0.0f);
            float4 c_z = new float4(b.x.z, b.y.z, b.z.z, 0.0f);
            float4 c_w = new float4(b.x.w, b.y.w, b.z.w, 1.0f);
            return new Transform(
                float4.Dot(a.x, c_x), float4.Dot(a.x, c_y), float4.Dot(a.x, c_z), float4.Dot(a.x, c_w),
                float4.Dot(a.y, c_x), float4.Dot(a.y, c_y), float4.Dot(a.y, c_z), float4.Dot(a.y, c_w),
                float4.Dot(a.z, c_x), float4.Dot(a.z, c_y), float4.Dot(a.z, c_z), float4.Dot(a.z, c_w)
            );
        }

        static public bool operator ==(Transform a, Transform b)
        {
            return (a.x == b.x) && (a.y == b.y) && (a.z == b.z);
        }
        static public bool operator !=(Transform a, Transform b)
        {
            return !(a == b);
        }

        public override bool Equals(object obj)
        {
            return obj is Transform transform &&
                   x.Equals(transform.x) &&
                   y.Equals(transform.y) &&
                   z.Equals(transform.z);
        }

        public override int GetHashCode()
        {
            //return HashCode.Combine(x, y, z);
            return x.GetHashCode() * 313 + y.GetHashCode() * 317 + z.GetHashCode() * 137;
        }

        /// <summary>
		/// Conversion matrix for rhino-cycles camera
		/// </summary>
		static public Transform RhinoToCyclesCam { get; } = new Transform(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, -1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, -1.0f, 0.0f
        );
        static public Transform RhinoToCyclesCamNoFlip { get; } = new Transform(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, 1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, -1.0f, 0.0f
        );
        static public Transform RhinoToCyclesCamReflected { get; } = new Transform(
            1.0f, 0.0f, 0.0f, 0.0f,
            0.0f, -1.0f, 0.0f, 0.0f,
            0.0f, 0.0f, -1.0f, 0.0f
        );

    }
}
