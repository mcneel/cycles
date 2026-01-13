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
		#region object
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_matrix(IntPtr sessionId, IntPtr objectId,
			float a, float b, float c, float d,
			float e, float f, float g, float h,
			float i, float j, float k, float l);
		public static void object_set_matrix(IntPtr sessionId, IntPtr objectId, Transform t)
		{
			cycles_scene_object_set_matrix(sessionId, objectId,
				t.x.x, t.x.y, t.x.z, t.x.w,
				t.y.x, t.y.y, t.y.z, t.y.w,
				t.z.x, t.z.y, t.z.z, t.z.w);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_ocs_frame(IntPtr sessionId, IntPtr objectId,
			float a, float b, float c, float d,
			float e, float f, float g, float h,
			float i, float j, float k, float l);
		public static void object_set_ocs_frame(IntPtr sessionId, IntPtr objectId, Transform t)
		{
			cycles_scene_object_set_ocs_frame(sessionId, objectId,
				t.x.x, t.x.y, t.x.z, t.x.w,
				t.y.x, t.y.y, t.y.z, t.y.w,
				t.z.x, t.z.y, t.z.z, t.z.w);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_geometry(IntPtr sessionId, IntPtr obj, IntPtr geometry);
		public static void object_set_geometry(IntPtr sessionId, IntPtr obj, IntPtr geometry)
		{
			cycles_scene_object_set_geometry(sessionId, obj, geometry);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_visibility(IntPtr sessionId, IntPtr objectId, uint visibility);
		public static void object_set_visibility(IntPtr sessionId, IntPtr objectId, PathRay visibility)
		{
			cycles_scene_object_set_visibility(sessionId, objectId, (uint)visibility);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_shader(IntPtr sessionId, IntPtr objectId, IntPtr shader);
		public static void object_set_shader(IntPtr sessionId, IntPtr objectId, IntPtr shader)
		{
			cycles_scene_object_set_shader(sessionId, objectId, shader);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_is_shadowcatcher(IntPtr sessionId, IntPtr objectId, bool is_shadowcatcher);
		public static void object_set_is_shadowcatcher(IntPtr sessionId, IntPtr objectId, bool is_shadowcatcher)
		{
			cycles_scene_object_set_is_shadowcatcher(sessionId, objectId, is_shadowcatcher);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_object_set_mesh_light_no_cast_shadow(IntPtr sessionId, IntPtr objectId, bool mesh_light_no_cast_shadow);
		public static void object_set_mesh_light_no_cast_shadow(IntPtr sessionId, IntPtr objectId, bool mesh_light_no_cast_shadow)
		{
			cycles_scene_object_set_mesh_light_no_cast_shadow(sessionId, objectId, mesh_light_no_cast_shadow);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_object_tag_update(IntPtr sessionId, IntPtr objectId);
		public static void object_tag_update(IntPtr sessionId, IntPtr objectId)
		{
			cycles_object_tag_update(sessionId, objectId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_object_set_pass_id(IntPtr sessionId, IntPtr objectId, int pass_id);
		public static void object_set_pass_id(IntPtr sessionId, IntPtr objectId, int pass_id)
		{
			cycles_object_set_pass_id(sessionId, objectId, pass_id);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_object_set_random_id(IntPtr sessionId, IntPtr objectId, uint random_id);
		public static void object_set_random_id(IntPtr sessionId, IntPtr objectId, uint random_id)
		{
			cycles_object_set_random_id(sessionId, objectId, random_id);
		}

		#endregion

		#region clipping planes

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_add_clipping_plane(IntPtr sessionId, float a, float b, float c, float d);

		public static uint scene_add_clipping_plane(IntPtr sessionId, float4 equation)
		{
			return scene_add_clipping_plane(sessionId, equation.x, equation.y, equation.z, equation.w);
		}
		public static uint scene_add_clipping_plane(IntPtr sessionId, float a, float b, float c, float d)
		{
			return cycles_scene_add_clipping_plane(sessionId, a, b, c, d);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_clipping_plane(IntPtr sessionId, uint cpId, float a, float b, float c, float d);

		public static void scene_set_clipping_plane(IntPtr sessionId, uint cpId, float a, float b, float c, float d)
		{
			cycles_scene_set_clipping_plane(sessionId, cpId, a, b, c, d);
		}
		public static void scene_set_clipping_plane(IntPtr sessionId, uint cpId, float4 equation)
		{
			scene_set_clipping_plane(sessionId, cpId, equation.x, equation.y, equation.z, equation.w);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_clear_clipping_planes(IntPtr sessionId);

		public static void scene_clear_clipping_planes(IntPtr sessionId)
		{
			cycles_scene_clear_clipping_planes(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_discard_clipping_plane(IntPtr sessionId, uint cpId);

		public static void scene_discard_clipping_plane(IntPtr sessionId, uint cpId)
		{
			cycles_scene_discard_clipping_plane(sessionId, cpId);
		}

		#endregion
	}
}
