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
		private static extern uint cycles_scene_create(uint sceneParamsId, IntPtr sessionId);
		public static uint scene_create(uint sceneParamsId, IntPtr sessionId)
		{
			return cycles_scene_create(sceneParamsId, sessionId);
		}

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
		private static extern void cycles_scene_lock(IntPtr sessionId);
		public static void scene_lock(IntPtr sessionId)
		{
			cycles_scene_lock(sessionId);
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

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_set_default_surface_shader(IntPtr sessionId, IntPtr shaderId);
		/// <summary>
		/// Set the default surface shader for sessionId to shaderId.
		/// 
		/// Note that this shaderId has to be the scene-specific shader id.
		/// </summary>
		/// <param name="clientId">ID of client</param>
		/// <param name="sessionId">Session for which the default shader is set</param>
		/// <param name="shaderId">The shader to which the default shader is set</param>
		public static void scene_set_default_surface_shader(IntPtr sessionId, IntPtr shaderId)
		{
			cycles_scene_set_default_surface_shader(sessionId, shaderId);
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_scene_get_default_surface_shader(IntPtr sessionId);
		/// <summary>
		/// Get the default surface shader for sessionId
		/// </summary>
		/// <param name="sessionId"></param>
		/// <returns></returns>
		public static IntPtr scene_get_default_surface_shader(IntPtr sessionId)
		{
			return cycles_scene_get_default_surface_shader(sessionId);
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

		#region scene parameters
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_params_create(uint shadingsystem, uint bvhtype, uint bvhspatialsplit, int bvhlayout, uint persistentdata);
		public static uint scene_params_create(ShadingSystem shadingSystem, BvhType bvhType, bool bvhSpatialSplit, BvhLayout bvhLayout, bool persistentData)
		{
			return cycles_scene_params_create((uint)shadingSystem, (uint)bvhType, (uint)(bvhSpatialSplit ? 1 : 0), (int)(bvhLayout), (uint)(persistentData ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_bvh_type(uint sceneParamsId, uint type);
		public static void scene_params_set_bvh_type(uint sceneParamsId, BvhType type)
		{
			cycles_scene_params_set_bvh_type(sceneParamsId, (uint)type);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_bvh_spatial_split(uint sceneParamsId, uint split);
		public static void scene_params_set_bvh_spatial_split(uint sceneParamsId, bool split)
		{
			cycles_scene_params_set_bvh_spatial_split(sceneParamsId, (uint)(split ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_qbvh(uint sceneParamsId, uint qbvh);
		public static void scene_params_set_qbvh(uint sceneParamsId, bool qbvh)
		{
			cycles_scene_params_set_qbvh(sceneParamsId, (uint)(qbvh ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_shadingsystem(uint sceneParamsId, uint shadingsystem);
		public static void scene_params_set_shadingsystem(uint sceneParamsId, ShadingSystem system)
		{
			cycles_scene_params_set_shadingsystem(sceneParamsId, (uint)system);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_params_set_persistent_data(uint sceneParamsId, uint persistentData);
		public static void scene_params_set_persistent_data(uint sceneParamsId, bool persistent)
		{
			cycles_scene_params_set_persistent_data(sceneParamsId, (uint)(persistent ? 1 : 0));
		}

		#endregion


	}
}
