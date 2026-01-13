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
	public partial class CSycles
	{
		#region light

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_create_light(IntPtr sessionId, IntPtr lightShaderId);
		public static IntPtr create_light(IntPtr sessionId, IntPtr lightShaderId)
		{
			return cycles_create_light(sessionId, lightShaderId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_type(IntPtr sessionId, IntPtr lightId, uint type);
		public static void light_set_type(IntPtr sessionId, IntPtr lightId, LightType type)
		{
			cycles_light_set_type(sessionId, lightId, (uint)type);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_samples(IntPtr sessionId, IntPtr lightId, uint samples);
		public static void light_set_samples(IntPtr sessionId, IntPtr lightId, uint samples)
		{
			cycles_light_set_samples(sessionId, lightId, samples);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_max_bounces(IntPtr sessionId, IntPtr lightId, uint maxBounces);
		public static void light_set_max_bounces(IntPtr sessionId, IntPtr lightId, uint maxBounces)
		{
			cycles_light_set_max_bounces(sessionId, lightId, maxBounces);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_map_resolution(IntPtr sessionId, IntPtr lightId, uint mapResolution);
		public static void light_set_map_resolution(IntPtr sessionId, IntPtr lightId, uint mapResolution)
		{
			cycles_light_set_map_resolution(sessionId, lightId, mapResolution);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_spot_angle(IntPtr sessionId, IntPtr lightId, float spotAngle);
		public static void light_set_spot_angle(IntPtr sessionId, IntPtr lightId, float spotAngle)
		{
			cycles_light_set_spot_angle(sessionId, lightId, spotAngle);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_spot_smooth(IntPtr sessionId, IntPtr lightId, float spotSmooth);
		public static void light_set_spot_smooth(IntPtr sessionId, IntPtr lightId, float spotSmooth)
		{
			cycles_light_set_spot_smooth(sessionId, lightId, spotSmooth);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_use_mis(IntPtr sessionId, IntPtr lightId, uint useMis);
		public static void light_set_use_mis(IntPtr sessionId, IntPtr lightId, bool useMis)
		{
			cycles_light_set_use_mis(sessionId, lightId, (uint)(useMis ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_cast_shadow(IntPtr sessionId, IntPtr lightId, uint useMis);
		public static void light_set_cast_shadow(IntPtr sessionId, IntPtr lightId, bool castShadow)
		{
			cycles_light_set_cast_shadow(sessionId, lightId, (uint)(castShadow ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_sizeu(IntPtr sessionId, IntPtr lightId, float sizeu);
		public static void light_set_sizeu(IntPtr sessionId, IntPtr lightId, float sizeu)
		{
			cycles_light_set_sizeu(sessionId, lightId, sizeu);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_sizev(IntPtr sessionId, IntPtr lightId, float sizev);
		public static void light_set_sizev(IntPtr sessionId, IntPtr lightId, float sizev)
		{
			cycles_light_set_sizev(sessionId, lightId, sizev);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_axisu(IntPtr sessionId, IntPtr lightId, float axisux, float axisuy, float axisuz);
		public static void light_set_axisu(IntPtr sessionId, IntPtr lightId, float axisux, float axisuy, float axisuz)
		{
			cycles_light_set_axisu(sessionId, lightId, axisux, axisuy, axisuz);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_axisv(IntPtr sessionId, IntPtr lightId, float axisvx, float axisvy, float axisvz);
		public static void light_set_axisv(IntPtr sessionId, IntPtr lightId, float axisvx, float axisvy, float axisvz)
		{
			cycles_light_set_axisv(sessionId, lightId, axisvx, axisvy, axisvz);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_angle(IntPtr sessionId, IntPtr lightId, float angle);
		public static void light_set_angle(IntPtr sessionId, IntPtr lightId, float angle)
		{
			cycles_light_set_angle(sessionId, lightId, angle);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_size(IntPtr sessionId, IntPtr lightId, float size);
		public static void light_set_size(IntPtr sessionId, IntPtr lightId, float size)
		{
			cycles_light_set_size(sessionId, lightId, size);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_dir(IntPtr sessionId, IntPtr lightId, float dirx, float diry, float dirz);
		public static void light_set_dir(IntPtr sessionId, IntPtr lightId, float dirx, float diry, float dirz)
		{
			cycles_light_set_dir(sessionId, lightId, dirx, diry, dirz);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_set_co(IntPtr sessionId, IntPtr lightId, float cox, float coy, float coz);
		public static void light_set_co(IntPtr sessionId, IntPtr lightId, float cox, float coy, float coz)
		{
			cycles_light_set_co(sessionId, lightId, cox, coy, coz);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_light_tag_update(IntPtr sessionId, IntPtr lightId);
		public static void light_tag_update(IntPtr sessionId, IntPtr lightId)
		{
			cycles_light_tag_update(sessionId, lightId);
		}

		#endregion
	}
}
