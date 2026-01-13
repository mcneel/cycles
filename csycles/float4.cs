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
	internal struct _f4Api
	{
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.LPStruct)]
		static public extern void cycles_f4_add([MarshalAs(UnmanagedType.Struct)] _float4 a, [MarshalAs(UnmanagedType.Struct)] _float4 b, [In, Out, MarshalAs(UnmanagedType.Struct)] ref _float4 res);

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.LPStruct)]
		static public extern void cycles_f4_sub([MarshalAs(UnmanagedType.Struct)] _float4 a, [MarshalAs(UnmanagedType.Struct)] _float4 b, [In, Out, MarshalAs(UnmanagedType.Struct)] ref _float4 res);
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.LPStruct)]
		static public extern void cycles_f4_mul([MarshalAs(UnmanagedType.Struct)] _float4 a, [MarshalAs(UnmanagedType.Struct)] _float4 b, [In, Out, MarshalAs(UnmanagedType.Struct)] ref _float4 res);
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.LPStruct)]
		static public extern void cycles_f4_div([MarshalAs(UnmanagedType.Struct)] _float4 a, [MarshalAs(UnmanagedType.Struct)] _float4 b, [In, Out, MarshalAs(UnmanagedType.Struct)] ref _float4 res);

	}
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	public struct _float4
	{
		public float x;
		public float y;
		public float z;
		public float w;

		public _float4(float a, float b, float c, float d)
		{
			x = a;
			y = b;
			z = c;
			w = d;
		}

		public static explicit operator float4(_float4 _f4)
		{
			float4 f4 = new float4(_f4.x, _f4.y, _f4.z, _f4.w);

			return f4;
		}

		public static explicit operator _float4(float4 f4)
		{
			_float4 _f4 = new _float4(f4.x, f4.y, f4.z, f4.w);

			return _f4;
		}

		public static _float4 operator +(_float4 a, _float4 b)
		{
			_float4 res = new _float4();
			_f4Api.cycles_f4_add(a, b, ref res);
			return res;
		}

		public static _float4 operator -(_float4 a, _float4 b)
		{
			_float4 res = new _float4();
			_f4Api.cycles_f4_sub(a, b, ref res);
			return res;
		}
		public static _float4 operator *(_float4 a, _float4 b)
		{
			_float4 res = new _float4();
			_f4Api.cycles_f4_mul(a, b, ref res);
			return res;
		}
		public static _float4 operator /(_float4 a, _float4 b)
		{
			_float4 res = new _float4();
			_f4Api.cycles_f4_div(a, b, ref res);
			return res;
		}
		public float this[int index]
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
					case 3:
						return w;
					default:
						throw new IndexOutOfRangeException("Only 0-3 are acceptable");
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
					case 3:
						w = value;
						break;
					default:
						throw new IndexOutOfRangeException("Only 0-3 are acceptable");
				}

			}
		}


	}
	public class float4
	{
		public float x;
		public float y;
		public float z;
		public float w;
		public float4() : this(0.0f, 0.0f, 0.0f, 0.0f) { }
		/// <summary>
		/// Create float4 with all members set to x_
		/// </summary>
		/// <param name="x_"></param>
		public float4(float x_) : this(x_, x_, x_, x_) { }
		public float4(float x_, float y_, float z_) : this(x_, y_, z_, 0.0f) { }
		public float4(float x_, float y_, float z_, float w_)
		{
			x = x_;
			y = y_;
			z = z_;
			w = w_;
		}
		public float4(double x_, double y_, double z_, double w_) :
			this((float)x_, (float)y_, (float)z_, (float)w_)
		{ }
		public float4(double[] arr) : this(arr[0], arr[1], arr[2], arr[3]) { }
		/// <summary>
		/// Copy constructor
		/// </summary>
		/// <param name="old">float4 to copy</param>
		public float4(float4 old) : this(old.x, old.y, old.z, old.w) { }
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

		public float this[int index]
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
					case 3:
						return w;
					default:
						throw new IndexOutOfRangeException("Only 0-3 are acceptable");
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
					case 3:
						w = value;
						break;
					default:
						throw new IndexOutOfRangeException("Only 0-3 are acceptable");
				}

			}
		}

		/// <summary>
		/// Copy values from given source float4
		/// </summary>
		/// <param name="source">float4 to copy from</param>
		public void Copy(float4 source)
		{
			x = source.x;
			y = source.y;
			z = source.z;
			w = source.w;
		}

		public override string ToString()
		{
			var nfi = Utilities.Instance.NumberFormatInfo;
			return string.Format(nfi, "ccl.float4({0}f, {1}f, {2}f, {3}f)", x, y, z, w);
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
	}

}
