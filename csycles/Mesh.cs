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

namespace ccl
{
	/// <summary>
	/// Representation of a Cycles mesh.
	/// </summary>
	public class Mesh
	{
		/// <summary>
		/// Id of mesh in scene.
		/// </summary>
		public System.IntPtr GeometryPointer { get; }
		/// <summary>
		/// Reference to client.
		/// </summary>
		private Session Client { get; }
		/// <summary>
		/// Shader used for this mesh.
		/// </summary>
		private Shader Shader { get; set; }

		/// <summary>
		/// Create a new mesh for the given client using shader as the default shader
		/// </summary>
		/// <param name="client"></param>
		/// <param name="shader"></param>
		public Mesh(Session client, Shader shader)
		{
			Client = client;
			Shader = shader;
			GeometryPointer = CSycles.scene_add_mesh(Client.Scene.Id, shader.Id);
			//System.Diagnostics.Trace.WriteLine($"Created mesh {GeometryPointer} with shader {shader.Id}\n");
		}

		/// <summary>
		/// Constructor to use when a mesh that already exists in Cycles needs to be represented.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="id"></param>
		/// <param name="shader"></param>
		internal Mesh(Session client, System.IntPtr geometry_ptr, Shader shader)
		{
			Client = client;
			Shader = shader;

			GeometryPointer = geometry_ptr;
		}

		/// <summary>
		/// Clears out any pushed data
		/// </summary>
		public void ClearData()
		{
			CSycles.geometry_clear(Client.Scene.Id, GeometryPointer);
		}

		/// <summary>
		/// Use given <c>shader</c> as new shader.
		/// </summary>
		/// <param name="shader"></param>
		public void ReplaceShader(Shader shader)
		{
			System.Diagnostics.Trace.WriteLine($"on mesh {GeometryPointer} replacing {Shader.Id} with {shader.Id}\n");
			Shader = shader;
			CSycles.geometry_set_shader(Client.Scene.Id, GeometryPointer, Shader.Id);
			TagRebuild();
		}

		/// <summary>
		/// Tag for update and rebuild
		/// </summary>
		public void TagRebuild()
		{
			CSycles.geometry_tag_rebuild(Client.Scene.Id, GeometryPointer);
		}

		/// <summary>
		/// Compute tangent space data
		/// </summary>
		/// <param name="uvmap_name"></param>
		public void AttrTangentSpace(string uvmap_name)
		{
			CSycles.mesh_attr_tangentspace(Client.Scene.Id, GeometryPointer, uvmap_name);
		}

		/// <summary>
		/// Reserve memory for mesh data
		/// </summary>
		/// <param name="vcount"></param>
		/// <param name="fcount"></param>
		public void Reserve(uint vcount, uint fcount)
		{
			CSycles.mesh_reserve(Client.Scene.Id, GeometryPointer, vcount, fcount);
		}

		/// <summary>
		/// Resize mesh data to given counts
		/// </summary>
		/// <param name="vcount"></param>
		/// <param name="fcount"></param>
		public void Resize(uint vcount, uint fcount)
		{
			CSycles.mesh_resize(Client.Scene.Id, GeometryPointer, vcount, fcount);
		}

		/// <summary>
		/// Set vertex coordinates
		/// </summary>
		/// <param name="verts"></param>
		public void SetVerts(ref float[] verts)
		{
			CSycles.mesh_set_verts(Client.Scene.Id, GeometryPointer, ref verts, (uint)(verts.Length / 3));
		}

		/// <summary>
		/// Set trifaces
		/// </summary>
		/// <param name="faces"></param>
		/// <param name="smooth"></param>
		public void SetVertTris(ref int[] faces, bool smooth)
		{
			CSycles.mesh_set_tris(Client.Scene.Id, GeometryPointer, ref faces, (uint)(faces.Length / 3), Shader.Id, smooth);
		}

		/// <summary>
		/// Set vertex normals
		/// </summary>
		/// <param name="vertex_normals"></param>
		public void SetVertNormals(ref float[] vertex_normals)
		{
			CSycles.mesh_set_vertex_normals(Client.Scene.Id, GeometryPointer, ref vertex_normals, (uint)(vertex_normals.Length / 3));
		}

		/// <summary>
		/// Set UVs
		/// </summary>
		/// <param name="uvs">UV coordinates. Stride 2.</param>
		/// <param name="uvmap_name">UiName for the UV map attribute set</param>
		public void SetUvs(ref float[] uvs, string uvmap_name)
		{
			CSycles.mesh_set_uvs(Client.Scene.Id, GeometryPointer, ref uvs, (uint)(uvs.Length / 2), uvmap_name);
		}

		/// <summary>
		/// Set vertex colors
		/// </summary>
		/// <param name="uvs"></param>
		public void SetVertexColors(ref float[] vertexcolors)
		{
			CSycles.mesh_set_vertex_colors(Client.Scene.Id, GeometryPointer, ref vertexcolors, (uint)(vertexcolors.Length / 3));
		}

		/// <summary>
		/// Add a triangle to the mesh using the given vertex coordinates, shader and smooth flag
		/// </summary>
		/// <param name="v0"></param>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="shader"></param>
		/// <param name="smooth"></param>
		public void AddTri(uint v0, uint v1, uint v2, Shader shader, bool smooth)
		{
			CSycles.mesh_add_triangle(Client.Scene.Id, GeometryPointer, v0, v1, v2, shader.Id, smooth);
		}

		/// <summary>
		/// Set a triangle to the mesh at given triangle idx using the given vertex coordinates, shader and smooth flag
		/// </summary>
		/// <param name="idx"></param>
		/// <param name="v0"></param>
		/// <param name="v1"></param>
		/// <param name="v2"></param>
		/// <param name="shader"></param>
		/// <param name="smooth"></param>
		public void SetTri(uint idx, uint v0, uint v1, uint v2, Shader shader, bool smooth)
		{
			CSycles.mesh_set_triangle(Client.Scene.Id, GeometryPointer, idx, v0, v1, v2, shader.Id, smooth);
		}
	}
}
