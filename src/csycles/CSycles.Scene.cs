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
		#region scene
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_reset(IntPtr sessionId);
		public static void scene_reset(IntPtr sessionId)
		{
			cycles_scene_reset(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool cycles_scene_try_lock(IntPtr sessionId);
		public static bool scene_try_lock(IntPtr sessionId)
		{
			return cycles_scene_try_lock(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_unlock(IntPtr sessionId);
		public static void scene_unlock(IntPtr sessionId)
		{
			cycles_scene_unlock(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_scene_add_object(IntPtr sessionId);
		public static IntPtr scene_add_object(IntPtr sessionId)
		{
			return cycles_scene_add_object(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_scene_add_mesh(IntPtr sessionId, IntPtr shaderId);
		public static IntPtr scene_add_mesh(IntPtr sessionId, IntPtr shaderId)
		{
			return cycles_scene_add_mesh(sessionId, shaderId);
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_shader(IntPtr sessionId, IntPtr shaderId);
		public static void scene_set_background_shader(IntPtr sessionId, IntPtr shaderId)
		{
			cycles_scene_set_background_shader(sessionId, shaderId);
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_scene_get_background_shader(IntPtr sessionId);
		public static IntPtr scene_get_background_shader(IntPtr sessionId)
		{
			return cycles_scene_get_background_shader(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_transparent(IntPtr sessionId, bool transparent);

		public static void scene_set_background_transparent(IntPtr sessionId, bool transparent)
		{
			cycles_scene_set_background_transparent(sessionId, transparent);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_background_visibility(IntPtr sessionId, uint raypathFlag);

		public static void scene_set_background_visibility(IntPtr sessionId, PathRay raypathFlag)
		{
			cycles_scene_set_background_visibility(sessionId, (uint)raypathFlag);
		}
		#endregion
	}
}
