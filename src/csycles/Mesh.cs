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
		public Mesh(Session client, Shader shader)
		{
			Client = client;
			Shader = shader;
			GeometryPointer = CSycles.scene_add_mesh(Client.Scene.Id, shader.Id);
		}

		/// <summary>
		/// Constructor to use when a mesh that already exists in Cycles needs to be represented.
		/// </summary>
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
		/// Tag for update and rebuild
		/// </summary>
		public void TagRebuild()
		{
			CSycles.geometry_tag_rebuild(Client.Scene.Id, GeometryPointer);
		}

		/// <summary>
		/// Compute tangent space data
		/// </summary>
		public void AttrTangentSpace(string uvmap_name)
		{
			CSycles.mesh_attr_tangentspace(Client.Scene.Id, GeometryPointer, uvmap_name);
		}

		/// <summary>
		/// Resize mesh data to given counts
		/// </summary>
		public void Resize(uint vcount, uint fcount)
		{
			CSycles.mesh_resize(Client.Scene.Id, GeometryPointer, vcount, fcount);
		}

		/// <summary>
		/// Set vertex coordinates
		/// </summary>
		public void SetVerts(ref float[] verts)
		{
			CSycles.mesh_set_verts(Client.Scene.Id, GeometryPointer, ref verts, (uint)(verts.Length / 3));
		}

		/// <summary>
		/// Set trifaces
		/// </summary>
		public void SetVertTris(ref int[] faces, bool smooth)
		{
			CSycles.mesh_set_tris(Client.Scene.Id, GeometryPointer, ref faces, (uint)(faces.Length / 3), Shader.Id, smooth);
		}

		/// <summary>
		/// Set vertex normals
		/// </summary>
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
		public void SetVertexColors(ref float[] vertexcolors)
		{
			CSycles.mesh_set_vertex_colors(Client.Scene.Id, GeometryPointer, ref vertexcolors, (uint)(vertexcolors.Length / 3));
		}
	}
}
