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
		#region mesh
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_verts(IntPtr sessionId, IntPtr meshId, float* verts, uint vcount);
		public static void mesh_set_verts(IntPtr sessionId, IntPtr meshId, ref float[] verts, uint vcount)
		{
			unsafe
			{
				fixed (float* pverts = verts)
				{
					cycles_mesh_set_verts(sessionId, meshId, pverts, vcount);
				}
			}
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_uvs(IntPtr sessionId, IntPtr meshId, float* uvs, uint uvcount, [MarshalAs(UnmanagedType.LPStr)] string uvmap_name);
		public static void mesh_set_uvs(IntPtr sessionId, IntPtr meshId, ref float[] uvs, uint uvcount, string uvmap_name)
		{
			unsafe
			{
				fixed (float* puvs = uvs)
				{
					cycles_mesh_set_uvs(sessionId, meshId, puvs, uvcount, uvmap_name);
				}
			}
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_vertex_normals(IntPtr sessionId, IntPtr meshId, float* vertex_normals, uint vncount);
		public static void mesh_set_vertex_normals(IntPtr sessionId, IntPtr meshId, ref float[] vertex_normals, uint vncount)
		{
			unsafe
			{
				fixed (float* pvertex_normals = vertex_normals)
				{
					cycles_mesh_set_vertex_normals(sessionId, meshId, pvertex_normals, vncount);
				}
			}
		}
		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_vertex_colors(IntPtr sessionId, IntPtr meshId, float* vertex_colors, uint vccount);
		public static void mesh_set_vertex_colors(IntPtr sessionId, IntPtr meshId, ref float[] vertex_colors, uint vccount)
		{
			unsafe
			{
				fixed (float* pvertex_colors = vertex_colors)
				{
					cycles_mesh_set_vertex_colors(sessionId, meshId, pvertex_colors, vccount);
				}
			}
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private unsafe static extern void cycles_mesh_set_tris(IntPtr sessionId, IntPtr meshId, int* faces, uint fcount, IntPtr shaderId, uint smooth);
		public static void mesh_set_tris(IntPtr sessionId, IntPtr meshId, ref int[] tris, uint fcount, IntPtr shaderId, bool smooth)
		{
			unsafe
			{
				fixed (int* ptris = tris)
				{
					cycles_mesh_set_tris(sessionId, meshId, ptris, fcount, shaderId, (uint)(smooth ? 1 : 0));
				}
			}
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_set_triangle(IntPtr sessionId, IntPtr meshId, uint tri_idx, uint v0, uint v1, uint v2, IntPtr shaderId, uint smooth);

		public static void mesh_set_triangle(IntPtr sessionId, IntPtr meshId, uint tri_idx, uint v0, uint v1, uint v2,
			IntPtr shaderId, bool smooth)
		{
			cycles_mesh_set_triangle(sessionId, meshId, tri_idx, v0, v1, v2, shaderId, (uint)(smooth ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_add_triangle(IntPtr sessionId, IntPtr meshId, uint v0, uint v1, uint v2, IntPtr shaderId, uint smooth);

		public static void mesh_add_triangle(IntPtr sessionId, IntPtr meshId, uint v0, uint v1, uint v2,
			IntPtr shaderId, bool smooth)
		{
			cycles_mesh_add_triangle(sessionId, meshId, v0, v1, v2, shaderId, (uint)(smooth ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_set_smooth(IntPtr sessionId, IntPtr meshId, uint smooth);

		public static void mesh_set_smooth(IntPtr sessionId, IntPtr meshId, bool smooth)
		{
			cycles_mesh_set_smooth(sessionId, meshId, (uint)(smooth ? 1 : 0));
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_geometry_set_shader(IntPtr sessionId, IntPtr geometry, IntPtr shader);

		public static void geometry_set_shader(IntPtr sessionId, IntPtr geometry, IntPtr shaderId)
		{
			cycles_geometry_set_shader(sessionId, geometry, shaderId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_geometry_clear(IntPtr sessionId, IntPtr meshId);

		public static void geometry_clear(IntPtr sessionId, IntPtr meshId)
		{
			cycles_geometry_clear(sessionId, meshId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_resize(IntPtr sessionId, IntPtr meshId, uint vcount, uint fcount);

		public static void mesh_resize(IntPtr sessionId, IntPtr meshId, uint vcount, uint fcount)
		{
			cycles_mesh_resize(sessionId, meshId, vcount, fcount);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_reserve(IntPtr sessionId, IntPtr meshId, uint vcount, uint fcount);

		public static void mesh_reserve(IntPtr sessionId, IntPtr meshId, uint vcount, uint fcount)
		{
			cycles_mesh_reserve(sessionId, meshId, vcount, fcount);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_geometry_tag_rebuild(IntPtr sessionId, IntPtr meshId);

		public static void geometry_tag_rebuild(IntPtr sessionId, IntPtr meshId)
		{
			cycles_geometry_tag_rebuild(sessionId, meshId);
		}

		[DllImport(Constants.ccycles, SetLastError = false, CallingConvention = CallingConvention.Cdecl)]
		private static extern void cycles_mesh_attr_tangentspace(IntPtr sessionId, IntPtr meshId, [MarshalAs(UnmanagedType.LPStr)] string uvmap_name);

		public static void mesh_attr_tangentspace(IntPtr sessionId, IntPtr meshId, string uvmap_name)
		{
			cycles_mesh_attr_tangentspace(sessionId, meshId, uvmap_name);
		}

		#endregion

	}
}
