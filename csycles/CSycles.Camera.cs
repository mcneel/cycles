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
		#region camera
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_size(IntPtr sessionId, uint width, uint height);
		public static void camera_set_size(IntPtr sessionId, uint width, uint height)
		{
			cycles_camera_set_size(sessionId, width, height);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_camera_get_width(IntPtr sessionId);
		public static uint camera_get_width(IntPtr sessionId)
		{
			return cycles_camera_get_width(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_camera_get_height(IntPtr sessionId);
		public static uint camera_get_height(IntPtr sessionId)
		{
			return cycles_camera_get_height(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_matrix(IntPtr sessionId,
			float a, float b, float c, float d,
			float e, float f, float g, float h,
			float i, float j, float k, float l);
		public static void camera_set_matrix(IntPtr sessionId, Transform t)
		{
			cycles_camera_set_matrix(sessionId,
				t.x.x, t.x.y, t.x.z, t.x.w,
				t.y.x, t.y.y, t.y.z, t.y.w,
				t.z.x, t.z.y, t.z.z, t.z.w);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_type(IntPtr sessionId, uint type);
		public static void camera_set_type(IntPtr sessionId, CameraType type)
		{
			cycles_camera_set_type(sessionId, (uint)type);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_panorama_type(IntPtr sessionId, uint type);
		public static void camera_set_panorama_type(IntPtr sessionId, PanoramaType type)
		{
			cycles_camera_set_panorama_type(sessionId, (uint)type);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_compute_auto_viewplane(IntPtr sessionId);
		public static void camera_compute_auto_viewplane(IntPtr sessionId)
		{
			cycles_camera_compute_auto_viewplane(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_viewplane(IntPtr sessionId, float left, float right, float top, float bottom);
		public static void camera_set_viewplane(IntPtr sessionId, float left, float right, float top, float bottom)
		{
			cycles_camera_set_viewplane(sessionId, left, right, top, bottom);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_update(IntPtr sessionId);
		public static void camera_update(IntPtr sessionId)
		{
			cycles_camera_update(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_fov(IntPtr sessionId, float fov);
		public static void camera_set_fov(IntPtr sessionId, float fov)
		{
			cycles_camera_set_fov(sessionId, fov);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_sensor_width(IntPtr sessionId, float sensorWidth);
		public static void camera_set_sensor_width(IntPtr sessionId, float sensorWidth)
		{
			cycles_camera_set_sensor_width(sessionId, sensorWidth);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_sensor_height(IntPtr sessionId, float sensorHeight);
		public static void camera_set_sensor_height(IntPtr sessionId, float sensorHeight)
		{
			cycles_camera_set_sensor_height(sessionId, sensorHeight);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_nearclip(IntPtr sessionId, float nearclip);
		public static void camera_set_nearclip(IntPtr sessionId, float nearclip)
		{
			cycles_camera_set_nearclip(sessionId, nearclip);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_farclip(IntPtr sessionId, float farclip);
		public static void camera_set_farclip(IntPtr sessionId, float farclip)
		{
			cycles_camera_set_farclip(sessionId, farclip);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_aperturesize(IntPtr sessionId, float aperturesize);
		public static void camera_set_aperturesize(IntPtr sessionId, float aperturesize)
		{
			cycles_camera_set_aperturesize(sessionId, aperturesize);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_aperture_ratio(IntPtr sessionId, float apertureRatio);
		public static void camera_set_aperture_ratio(IntPtr sessionId, float apertureRatio)
		{
			cycles_camera_set_aperture_ratio(sessionId, apertureRatio);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_blades(IntPtr sessionId, uint blades);
		public static void camera_set_blades(IntPtr sessionId, uint blades)
		{
			cycles_camera_set_blades(sessionId, blades);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_bladesrotation(IntPtr sessionId, float bladesrotation);
		public static void camera_set_bladesrotation(IntPtr sessionId, float bladesrotation)
		{
			cycles_camera_set_bladesrotation(sessionId, bladesrotation);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_shuttertime(IntPtr sessionId, float shuttertime);
		public static void camera_set_shuttertime(IntPtr sessionId, float shuttertime)
		{
			cycles_camera_set_shuttertime(sessionId, shuttertime);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_focaldistance(IntPtr sessionId, float focaldistance);
		public static void camera_set_focaldistance(IntPtr sessionId, float focaldistance)
		{
			cycles_camera_set_focaldistance(sessionId, focaldistance);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_fisheye_fov(IntPtr sessionId, float fisheyeFov);
		public static void camera_set_fisheye_fov(IntPtr sessionId, float fisheyeFov)
		{
			cycles_camera_set_fisheye_fov(sessionId, fisheyeFov);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_camera_set_fisheye_lens(IntPtr sessionId, float fisheyeLens);
		public static void camera_set_fisheye_lens(IntPtr sessionId, float fisheyeLens)
		{
			cycles_camera_set_fisheye_lens(sessionId, fisheyeLens);
		}
		#endregion
	}
}
