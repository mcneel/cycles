/**
Copyright 2014-2024 Robert McNeel and Associates

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

using System;
using System.Runtime.InteropServices;

namespace ccl
{

	internal struct _tfmApi
	{
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		static public extern void cycles_tfm_inverse([In, MarshalAs(UnmanagedType.Struct)] _Transform t, [In, Out, MarshalAs(UnmanagedType.Struct)] ref _Transform res);

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		static public extern void cycles_tfm_lookat([In, MarshalAs(UnmanagedType.Struct)] _float4 position, [In, MarshalAs(UnmanagedType.Struct)] _float4 look, [In, MarshalAs(UnmanagedType.Struct)] _float4 up, [In, Out, MarshalAs(UnmanagedType.Struct)] ref _Transform res);

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		static public extern void cycles_tfm_rotate_around_axis(float angle, [In, MarshalAs(UnmanagedType.Struct)] _float4 axis, [In, Out, MarshalAs(UnmanagedType.Struct)] ref _Transform res);

	}

	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct _Transform
	{
		public _float4 x;
		public _float4 y;
		public _float4 z;
		public _Transform(
			float a, float b, float c, float d,
			float e, float f, float g, float h,
			float i, float j, float k, float l
		)
		{
			x = new _float4(a, b, c, d);
			y = new _float4(e, f, g, h);
			z = new _float4(i, j, k, l);
		}
		public _float4 this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					default:
						throw new IndexOutOfRangeException("Only indices [0..2] are acceptable");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					case 2:
						z = value;
						break;
					default:
						throw new IndexOutOfRangeException("Only indices [0..2] are acceptable");
				}

			}
		}

		public void CopyTo(Transform target)
		{
			target.x.x = x.x;
			target.x.y = x.y;
			target.x.z = x.z;
			target.x.w = x.w;

			target.y.x = y.x;
			target.y.y = y.y;
			target.y.z = y.z;
			target.y.w = y.w;

			target.z.x = z.x;
			target.z.y = z.y;
			target.z.z = z.z;
			target.z.w = z.w;
		}

		public void CopyFrom(Transform source)
		{
			x.x = source.x.x;
			x.y = source.x.y;
			x.z = source.x.z;
			x.w = source.x.w;

			y.x = source.y.x;
			y.y = source.y.y;
			y.z = source.y.z;
			y.w = source.y.w;

			z.x = source.z.x;
			z.y = source.z.y;
			z.z = source.z.z;
			z.w = source.z.w;
		}

		public static explicit operator Transform(_Transform t)
		{
			Transform conv = new Transform
			{
				x = (float4)t.x,
				y = (float4)t.y,
				z = (float4)t.z,
			};


			return conv;
		}

		public static explicit operator _Transform(Transform t)
		{
			_Transform conv = new _Transform
			{
				x = (_float4)t.x,
				y = (_float4)t.y,
				z = (_float4)t.z,
			};

			return conv;
		}

		public static _Transform Inverse(_Transform t)
		{
			_Transform res = new _Transform();
			_tfmApi.cycles_tfm_inverse(t, ref res);

			return res;
		}

		public static _Transform LookAt(_float4 position, _float4 look, _float4 up)
		{
			_Transform res = new _Transform();
			_tfmApi.cycles_tfm_lookat(position, look, up, ref res);

			return res;
		}

		public static _Transform RotateAroundAxis(float angle, _float4 axis)
		{
			_Transform res = new _Transform();
			_tfmApi.cycles_tfm_rotate_around_axis(angle, axis, ref res);

			return res;
		}
	}
	/// <summary>
	/// Transformation matrix.
	/// </summary>
	public class Transform
	{
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

		/// <summary>
		/// X row, elements M00-M03
		/// </summary>
		public float4 x;
		/// <summary>
		/// Y row, elements M10-M13
		/// </summary>
		public float4 y;
		/// <summary>
		/// Z row, elements M20-M23
		/// </summary>
		public float4 z;

		/// <summary>
		/// Create a new transform using the given
		/// floats
		/// </summary>
		/// <param name="a">M00</param>
		/// <param name="b">M01</param>
		/// <param name="c">M02</param>
		/// <param name="d">M03</param>
		/// <param name="e">M10</param>
		/// <param name="f">M11</param>
		/// <param name="g">M12</param>
		/// <param name="h">M13</param>
		/// <param name="i">M20</param>
		/// <param name="j">M21</param>
		/// <param name="k">M22</param>
		/// <param name="l">M23</param>
		public Transform(
			float a, float b, float c, float d,
			float e, float f, float g, float h,
			float i, float j, float k, float l
		)
		{
			x = new float4(a, b, c, d);
			y = new float4(e, f, g, h);
			z = new float4(i, j, k, l);

		}
		/// <summary>
		/// Create new transform with all cells set to 0.0f
		/// </summary>
		public Transform()
			: this(0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f, 0.0f)
		{
		}

		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="old"></param>
		public Transform(Transform old)
			: this(
				old.x.x, old.x.y, old.x.z, old.x.w,
				old.y.x, old.y.y, old.y.z, old.y.w,
				old.z.x, old.z.y, old.z.z, old.z.w
			)
		{
		}

		/// <summary>
		/// Construct a Transform using a float array
		/// </summary>
		/// <param name="m">Array of 12 floats</param>
		public Transform(float[] m)
			: this(m[0], m[1], m[2], m[3], m[4], m[5], m[6], m[7], m[8], m[9], m[10], m[11])
		{
		}

		/// <summary>
		/// Set all matrix cells with elements from the given float array.
		/// </summary>
		/// <param name="m">float array of at least 12 elements. If the array is larger only the first 12 elements will be used.</param>
		public void SetMatrix(float[] m)
		{
			if (m.Length < 12) throw new ArgumentException("float array too short, must contain at least 12 float elements.");
			x.x = m[0];
			x.y = m[1];
			x.z = m[2];
			x.w = m[3];
			y.x = m[4];
			y.y = m[5];
			y.z = m[6];
			y.w = m[7];
			z.x = m[8];
			z.y = m[9];
			z.z = m[10];
			z.w = m[11];
		}

		/// <summary>
		/// Index the transformation, 0=x, 1=y, 2=z
		/// </summary>
		/// <param name="index">0<=index<=2</param>
		/// <returns>float4 designated by the index</returns>
		public float4 this[int index]
		{
			get
			{
				switch (index)
				{
					case 0:
						return x;
					case 1:
						return y;
					case 2:
						return z;
					default:
						throw new IndexOutOfRangeException("Only indices [0..2] are acceptable");
				}
			}
			set
			{
				switch (index)
				{
					case 0:
						x = value;
						break;
					case 1:
						y = value;
						break;
					case 2:
						z = value;
						break;
					default:
						throw new IndexOutOfRangeException("Only indices [0..2] are acceptable");
				}

			}
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

		/// <summary>
		/// Rotation matrix for given angle (radians) around the given rotation axis
		/// </summary>
		/// <param name="angle">Angle in radians</param>
		/// <param name="rotation_axis">axis around which the rotation is. w of float4 is unused</param>
		/// <returns></returns>
		static public Transform Rotate(float angle, float4 rotation_axis)
		{
			var axis = new float4(rotation_axis) { w = 0.0f };
			var s = (float)Math.Sin(angle);
			var c = (float)Math.Cos(angle);
			var t = 1.0f - c;

			axis = float4.Normalize(axis);

			return new Transform(
				axis.x * axis.x * t + c,
				axis.x * axis.y * t - s * axis.z,
				axis.x * axis.z * t + s * axis.y,
				0.0f,

				axis.y * axis.x * t + s * axis.z,
				axis.y * axis.y * t + c,
				axis.y * axis.z * t - s * axis.x,
				0.0f,

				axis.z * axis.x * t - s * axis.y,
				axis.z * axis.y * t + s * axis.x,
				axis.z * axis.z * t + c,
				0.0f

				);

		}


		public void SetTranslate(float4 t)
		{
			x.w = t.x;
			y.w = t.y;
			z.w = t.z;
		}

		public void SetTranslate(float x, float y, float z)
		{
			SetTranslate(new float4(x, y, z));
		}

		public void SetTranslate(double x, double y, double z)
		{
			SetTranslate(new float4((float)x, (float)y, (float)z));
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

		/// <summary>
		/// Compute a transform given the position, point to look at and
		/// an up vector.
		/// </summary>
		/// <param name="pos"></param>
		/// <param name="look"></param>
		/// <param name="up"></param>
		/// <returns></returns>
		static public Transform LookAt(float4 pos, float4 look, float4 up)
		{
			return (Transform)(_Transform.LookAt((_float4)pos, (_float4)look, (_float4)up));
		}

		/// <summary>
		/// Textual representation of the matrix
		/// </summary>
		/// <returns></returns>
		override public string ToString()
		{
			return
				$"[{x.x}, {x.y}, {x.z}, {x.w}, {y.x}, {y.y}, {y.z}, {y.w}, {z.x}, {z.y}, {z.z}, {z.w}]";
		}
	}

}
