/**
Copyright 2026 Robert McNeel and Associates

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
		#region shader node types
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern int cycles_shader_node_type_count();
		/// <summary>
		/// Get the number of shader node types Cycles can create. Needs no session.
		/// </summary>
		public static int shader_node_type_count()
		{
			return cycles_shader_node_type_count();
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr cycles_shader_node_type_name(int idx);
		/// <summary>
		/// Get the name of shader node type idx, e.g. "principled_bsdf". Types are sorted by name.
		/// </summary>
		public static string shader_node_type_name(int idx)
		{
			return Marshal.PtrToStringAnsi(cycles_shader_node_type_name(idx)) ?? string.Empty;
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern int cycles_shader_node_type_socket_count([MarshalAs(UnmanagedType.LPStr)] string nodeType, [MarshalAs(UnmanagedType.U1)] bool output);
		/// <summary>
		/// Get the number of input or output sockets of a shader node type; -1 for an unknown type.
		/// </summary>
		public static int shader_node_type_socket_count(string nodeType, bool output)
		{
			return cycles_shader_node_type_socket_count(nodeType, output);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool cycles_shader_node_type_socket([MarshalAs(UnmanagedType.LPStr)] string nodeType, [MarshalAs(UnmanagedType.U1)] bool output, int idx,
			out IntPtr name, out IntPtr uiName, out IntPtr socketType, out int flags);
		/// <summary>
		/// Describe input or output socket idx of a shader node type.
		/// </summary>
		/// <param name="socketType">Cycles socket type name, e.g. "float", "color" or "closure"</param>
		/// <param name="flags">Cycles SocketType flags</param>
		public static bool shader_node_type_socket(string nodeType, bool output, int idx, out string name, out string uiName, out string socketType, out int flags)
		{
			var found = cycles_shader_node_type_socket(nodeType, output, idx, out var namePtr, out var uiNamePtr, out var typePtr, out flags);
			name = found ? Marshal.PtrToStringAnsi(namePtr) : string.Empty;
			uiName = found ? Marshal.PtrToStringAnsi(uiNamePtr) : string.Empty;
			socketType = found ? Marshal.PtrToStringAnsi(typePtr) : string.Empty;
			return found;
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool cycles_shader_node_type_socket_default([MarshalAs(UnmanagedType.LPStr)] string nodeType, int idx, IntPtr strHolder);
		/// <summary>
		/// Get the default of input socket idx in XML attribute form, e.g. "0.8 0.8 0.8".
		/// Empty for closures, arrays and transforms.
		/// </summary>
		public static string shader_node_type_socket_default(string nodeType, int idx)
		{
			using (var stringHolder = new CSStringHolder())
			{
				return cycles_shader_node_type_socket_default(nodeType, idx, stringHolder.Ptr) ? stringHolder.Value : string.Empty;
			}
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		private static extern int cycles_shader_node_type_socket_enum_count([MarshalAs(UnmanagedType.LPStr)] string nodeType, int idx);
		/// <summary>
		/// Get the number of values of enum input socket idx; 0 if it is not an enum.
		/// </summary>
		public static int shader_node_type_socket_enum_count(string nodeType, int idx)
		{
			return cycles_shader_node_type_socket_enum_count(nodeType, idx);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool cycles_shader_node_type_socket_enum([MarshalAs(UnmanagedType.LPStr)] string nodeType, int idx, int enumIdx, out IntPtr name, out int value);
		/// <summary>
		/// Get value enumIdx of enum input socket idx. Values are sorted by number.
		/// </summary>
		public static bool shader_node_type_socket_enum(string nodeType, int idx, int enumIdx, out string name, out int value)
		{
			var found = cycles_shader_node_type_socket_enum(nodeType, idx, enumIdx, out var namePtr, out value);
			name = found ? Marshal.PtrToStringAnsi(namePtr) : string.Empty;
			return found;
		}
		#endregion
	}
}
