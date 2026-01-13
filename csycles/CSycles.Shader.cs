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
		#region shaders
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_create_shader(IntPtr sessionId);
		public static IntPtr create_shader(IntPtr sessionId)
		{
			return cycles_create_shader(sessionId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_scene_tag_shader(IntPtr sessionId, IntPtr shaderId, bool use);
		public static void scene_tag_shader(IntPtr sessionId, IntPtr shaderId, bool use)
		{
			cycles_scene_tag_shader(sessionId, shaderId, use);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern uint cycles_scene_shader_id(IntPtr sessionId, IntPtr shaderId);
		public static uint scene_shader_id(IntPtr sessionId, IntPtr shaderId)
		{
			return cycles_scene_shader_id(sessionId, shaderId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_add_shader_node(IntPtr shaderId, [MarshalAs(UnmanagedType.LPStr)] string shnType, [MarshalAs(UnmanagedType.LPStr)] string shnName);
		public static IntPtr add_shader_node(IntPtr shaderId, string node_type, string node_name)
		{
			return cycles_add_shader_node(shaderId, node_type, node_name);
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_texmapping_set_transformation(IntPtr shadernodeId, uint transformType, float x, float y, float z);
		public static void shadernode_texmapping_set_transformation(IntPtr shadernodeId, uint transformType, float x, float y, float z)
		{
			cycles_shadernode_texmapping_set_transformation(shadernodeId, transformType, x, y, z);
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_texmapping_set_mapping(IntPtr shadernodeId, uint mappingx, uint mappingy, uint mappingz);
		public static void shadernode_texmapping_set_mapping(IntPtr shadernodeId, uint mappingx, uint mappingy, uint mappingz)
		{
			cycles_shadernode_texmapping_set_mapping(shadernodeId, mappingx, mappingy, mappingz);
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_texmapping_set_projection(IntPtr shadernodeId, uint projection);
		[Obsolete("No longer existing")]
		public static void shadernode_texmapping_set_projection(IntPtr shadernodeId, uint projection)
		{
			cycles_shadernode_texmapping_set_projection(shadernodeId, projection);
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_texmapping_set_type(IntPtr shadernodeId, uint type);
		public static void shadernode_texmapping_set_type(IntPtr shadernodeId, uint type)
		{
			cycles_shadernode_texmapping_set_type(shadernodeId, type);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_enum(IntPtr shadernodeId, [MarshalAs(UnmanagedType.LPStr)] string enum_name, int value);
		/// <summary>
		/// Set enumeration value for a shader node
		/// </summary>
		/// <param name="clientId">Session</param>
		/// <param name="sceneId">Session</param>
		/// <param name="shaderId">Shader ID</param>
		/// <param name="shadernodeId">Node ID in shader</param>
		/// <param name="shnType">Type of shader node</param>
		/// <param name="enum_name">UiName of enumeration. Used mostly for nodes that have multiple</param>
		/// <param name="value">Int value of the enumeration</param>
		public static void shadernode_set_enum(IntPtr shadernodeId, string enum_name, int value)
		{
			cycles_shadernode_set_enum(shadernodeId, enum_name, value);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_attribute_int(IntPtr shadernodeId, string name, int val);
		public static void shadernode_set_attribute_int(IntPtr shadernodeId, [MarshalAs(UnmanagedType.LPStr)] string name, int val)
		{
			cycles_shadernode_set_attribute_int(shadernodeId, name, val);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_attribute_bool(IntPtr shadernodeId, string name, bool val);
		public static void shadernode_set_attribute_bool(IntPtr shadernodeId, [MarshalAs(UnmanagedType.LPStr)] string name, bool val)
		{
			cycles_shadernode_set_attribute_bool(shadernodeId, name, val);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Auto,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_attribute_string(IntPtr shadernodeId, string name, string val);
		public static void shadernode_set_attribute_string(IntPtr shadernodeId, string name, string val)
		{
			cycles_shadernode_set_attribute_string(shadernodeId, name, val);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_attribute_float(IntPtr shadernodeId, string name, float val);
		public static void shadernode_set_attribute_float(IntPtr shadernodeId, [MarshalAs(UnmanagedType.LPStr)] string name, float val)
		{
			cycles_shadernode_set_attribute_float(shadernodeId, name, val);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_attribute_vec(IntPtr shadernodeId, string name, float x, float y, float z);
		public static void shadernode_set_attribute_vec(IntPtr shadernodeId, [MarshalAs(UnmanagedType.LPStr)] string name, float4 val)
		{
			cycles_shadernode_set_attribute_vec(shadernodeId, name, val.x, val.y, val.z);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_float(IntPtr shadernodeId, string name, float val);
		public static void shadernode_set_member_float(IntPtr shadernodeId,
			[MarshalAs(UnmanagedType.LPStr)] string name, float val)
		{
			cycles_shadernode_set_member_float(shadernodeId, name, val);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_int(IntPtr shadernodeId, string name, int val);
		public static void shadernode_set_member_int(IntPtr shadernodeId,
			[MarshalAs(UnmanagedType.LPStr)] string name, int val)
		{
			cycles_shadernode_set_member_int(shadernodeId, name, val);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_bool(IntPtr shadernodeId, string name, bool val);
		public static void shadernode_set_member_bool(IntPtr shadernodeId, [MarshalAs(UnmanagedType.LPStr)] string name, bool val)
		{
			cycles_shadernode_set_member_bool(shadernodeId, name, val);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_vec(IntPtr shadernodeId, string name, float x, float y, float z);
		public static void shadernode_set_member_vec(IntPtr shadernodeId, [MarshalAs(UnmanagedType.LPStr)] string name, float x, float y, float z)
		{
			cycles_shadernode_set_member_vec(shadernodeId, name, x, y, z);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_string(IntPtr shadernodeId, string name, string value);
		public static void shadernode_set_member_string(IntPtr shadernodeId, [MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string value)
		{
			cycles_shadernode_set_member_string(shadernodeId, name, value);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shadernode_set_member_vec4_at_index(IntPtr shadernodeId, string name, float x, float y, float z, float w, int index);
		public static void shadernode_set_member_vec4_at_index(IntPtr shadernodeId,
			[MarshalAs(UnmanagedType.LPStr)] string name, float x, float y, float z, float w, int index)
		{
			cycles_shadernode_set_member_vec4_at_index(shadernodeId, name, x, y, z, w, index);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_apply_gamma_to_byte_buffer(IntPtr rgba_buffer, int size_in_bytes, float gamma);
		public static void apply_gamma_to_byte_buffer(IntPtr rgba_buffer, int size_in_bytes, float gamma)
		{
			cycles_apply_gamma_to_byte_buffer(rgba_buffer, size_in_bytes, gamma);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_apply_gamma_to_float_buffer(IntPtr rgba_buffer, int size_in_bytes, float gamma);
		public static void apply_gamma_to_float_buffer(IntPtr rgba_buffer, int size_in_bytes, float gamma)
		{
			cycles_apply_gamma_to_float_buffer(rgba_buffer, size_in_bytes, gamma);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern bool cycles_shader_connect_nodes(IntPtr shaderId, IntPtr fromId, string from, IntPtr toId,
			string to);
		public static bool shader_connect_nodes(IntPtr shaderId, IntPtr fromId, string from, IntPtr toId, string to)
		{
			return cycles_shader_connect_nodes(shaderId, fromId, from, toId, to);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_disconnect_node(IntPtr shaderId, IntPtr nodeId, string from);
		public static void shader_disconnect_node(IntPtr shaderId, IntPtr nodeId, string from)
		{
			cycles_shader_disconnect_node(shaderId, nodeId, from);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_dump_graph(IntPtr shaderId, [MarshalAs(UnmanagedType.LPStr)] string filename);
		public static void shader_dump_graph(IntPtr shaderId, string filename)
		{
			cycles_shader_dump_graph(shaderId, filename);
		}


		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern int cycles_shader_get_pass_id(IntPtr shaderId);
		public static int shader_get_pass_id(IntPtr shaderId)
		{
			return cycles_shader_get_pass_id(shaderId);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_set_pass_id(IntPtr shaderId, int passId);
		public static void shader_set_pass_id(IntPtr shaderId, int passId)
		{
			cycles_shader_set_pass_id(shaderId, passId);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_set_name(IntPtr shaderId, [MarshalAs(UnmanagedType.LPStr)] string name);
		public static void shader_set_name(IntPtr shaderId, string name)
		{
			cycles_shader_set_name(shaderId, name);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern bool cycles_shader_get_name(IntPtr shaderId, IntPtr stringholder);
		public static string shader_get_name(IntPtr shaderId)
		{
			using (CSStringHolder stringHolder = new CSStringHolder())
			{
				bool success = cycles_shader_get_name(shaderId, stringHolder.Ptr);
				if (success)
				{
					string name = stringHolder.Value;
					return name;
				}
			}
			return $"UiName failed for {shaderId}";
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern bool cycles_nodetype_get_name(IntPtr nodeTypePtr, IntPtr stringholder);
		public static string nodetype_get_name(IntPtr nodeTypePtr)
		{
			using (CSStringHolder stringHolder = new CSStringHolder())
			{
				bool success = cycles_nodetype_get_name(nodeTypePtr, stringHolder.Ptr);
				if (success)
				{
					string name = stringHolder.Value;
					return name;
				}
			}
			return $"UiName failed for {nodeTypePtr}";
		}
		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern int cycles_get_shadernodetype_count();
		public static int get_shadernodetype_count()
		{
			return cycles_get_shadernodetype_count();
		}
		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_get_shadernodetype(int idx);
		public static IntPtr get_shadernodetype(int idx)
		{
			return cycles_get_shadernodetype(idx);
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern int cycles_shadernode_get_socketcount(IntPtr shader, int input_output);
		public static int shadernode_get_socketcount(IntPtr shader, int input_output)
		{
			return cycles_shadernode_get_socketcount(shader, input_output);
		}
		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_shadernode_get_sockettype(IntPtr shader, int idx, int input_output);
		public static IntPtr shadernode_get_sockettype(IntPtr shader, int idx, int input_output)
		{
			return cycles_shadernode_get_sockettype(shader, idx, input_output);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern int cycles_sockettype_get_type(IntPtr sockettypeId);
		public static int sockettype_get_type(IntPtr sockettypeId)
		{
			return cycles_sockettype_get_type(sockettypeId);
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern bool cycles_sockettype_get_ui_name(IntPtr sockettypeId, IntPtr stringholder);
		public static string sockettype_get_ui_name(IntPtr sockettypeId)
		{
			using (CSStringHolder stringHolder = new CSStringHolder())
			{
				bool success = cycles_sockettype_get_ui_name(sockettypeId, stringHolder.Ptr);
				if (success)
				{
					string name = stringHolder.Value;
					return name;
				}
			}
			return $"UiName failed for {sockettypeId}";
		}
		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern bool cycles_sockettype_get_internal_name(IntPtr sockettypeId, IntPtr stringholder);
		public static string sockettype_get_internal_name(IntPtr sockettypeId)
		{
			using (CSStringHolder stringHolder = new CSStringHolder())
			{
				bool success = cycles_sockettype_get_internal_name(sockettypeId, stringHolder.Ptr);
				if (success)
				{
					string name = stringHolder.Value;
					return name;
				}
			}
			return $"UiName failed for {sockettypeId}";
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_set_use_mis(IntPtr sessionId, IntPtr shaderId, uint useMis);
		public static void shader_set_use_mis(IntPtr sessionId, IntPtr shaderId, bool useMis)
		{
			cycles_shader_set_use_mis(sessionId, shaderId, (uint)(useMis ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_set_use_transparent_shadow(IntPtr sessionId, IntPtr shaderId, uint useTransparentShadow);
		public static void shader_set_use_transparent_shadow(IntPtr sessionId, IntPtr shaderId, bool useTransparentShadow)
		{
			cycles_shader_set_use_transparent_shadow(sessionId, shaderId, (uint)(useTransparentShadow ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_set_heterogeneous_volume(IntPtr sessionId, IntPtr shaderId, uint heterogeneousVolume);
		public static void shader_set_heterogeneous_volume(IntPtr sessionId, IntPtr shaderId, bool heterogeneousVolume)
		{
			cycles_shader_set_heterogeneous_volume(sessionId, shaderId, (uint)(heterogeneousVolume ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_shader_new_graph(IntPtr shaderId);
		public static void shader_new_graph(IntPtr shaderId)
		{
			cycles_shader_new_graph(shaderId);
		}
		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern int cycles_shader_node_count(IntPtr shader);
		public static int shader_node_count(IntPtr shader)
		{
			return cycles_shader_node_count(shader);
		}
		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_shader_node_get(IntPtr shader, int idx);
		public static IntPtr shader_node_get(IntPtr shader, int idx)
		{
			return cycles_shader_node_get(shader, idx);
		}
		[DllImport(Constants.ccycles, SetLastError = false,
			CallingConvention = CallingConvention.Cdecl)]
		private static extern bool cycles_shadernode_get_name(IntPtr shn, IntPtr strholder);
		public static string shadernode_get_name(IntPtr shn)
		{
			using (CSStringHolder stringHolder = new CSStringHolder())
			{
				bool success = cycles_shadernode_get_name(shn, stringHolder.Ptr);
				if (success)
				{
					string name = stringHolder.Value;
					return name;
				}
			}

			return "";
		}


		#endregion
	}
}
