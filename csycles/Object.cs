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

namespace ccl
{

	public class ClippingPlane
	{
		/// <summary>
		/// Id of the Cycles object.
		/// </summary>
		public uint Id { get; }
		/// <summary>
		/// Reference to the client.
		/// </summary>
		private Session Client { get; }

		/// <summary>
		/// Add a new clipping plane using given equation.
		/// </summary>
		/// <param name="client"></param>
		/// <param name="equation"></param>
		public ClippingPlane(Session client, float4 equation)
		{
			Client = client;
			Id = CSycles.scene_add_clipping_plane(client.Scene.Id, equation);
		}

		/// <summary>
		/// Set new equation for the clipping plane.
		/// </summary>
		/// <param name="equation"></param>
		public void SetEquation(float4 equation)
		{
			CSycles.scene_set_clipping_plane(Client.Scene.Id, Id, equation);
		}

		/// <summary>
		/// Mark the clipping plane as not used.
		/// </summary>
		public void Discard()
		{
			CSycles.scene_discard_clipping_plane(Client.Scene.Id, Id);
		}

	}
	/// <summary>
	/// Representation of a Cycles object.
	/// </summary>
	public class Object
	{
		/// <summary>
		/// Id of the Cycles object.
		/// </summary>
		public System.IntPtr ObjectPtr { get; }
		/// <summary>
		/// Reference to the client.
		/// </summary>
		private Session Client { get; }

		/// <summary>
		/// Create a new mesh for the given client using shader as the default shader
		/// </summary>
		/// <param name="client"></param>
		/// <param name="shader"></param>
		public Object(Session client)
		{
			Client = client;

			ObjectPtr = CSycles.scene_add_object(Client.Scene.Id);
		}

		private Mesh m_mesh = null;
		/// <summary>
		/// Get or set the mesh
		/// </summary>
		public Mesh Mesh
		{
			get
			{
				return m_mesh;
			}
			set
			{
				m_mesh = value;
				CSycles.object_set_geometry(Client.Scene.Id, ObjectPtr, value.GeometryPointer);
			}
		}

		/// <summary>
		/// Tag object for update.
		/// </summary>
		public void TagUpdate()
		{
			CSycles.object_tag_update(Client.Scene.Id, ObjectPtr);
		}

		/// <summary>
		/// Set the object transformation
		/// </summary>
		public Transform Transform
		{
			set
			{
				CSycles.object_set_matrix(Client.Scene.Id, ObjectPtr, value);
			}
		}

		/// <summary>
		/// Set the object OCS frame
		/// </summary>
		public Transform OcsFrame
		{
			set
			{
				CSycles.object_set_ocs_frame(Client.Scene.Id, ObjectPtr, value);
			}
		}

		/// <summary>
		/// Set the visibility of this object to specific rays.
		/// </summary>
		public PathRay Visibility
		{
			set
			{
				CSycles.object_set_visibility(Client.Scene.Id, ObjectPtr, value);
			}
		}

		public IntPtr Shader
		{
			set
			{
				CSycles.object_set_shader(Client.Scene.Id, ObjectPtr, value);
			}
		}

		/// <summary>
		/// Set to true if this object should act as shadow catcher.
		/// </summary>
		public bool IsShadowCatcher
		{
			set
			{
				CSycles.object_set_is_shadowcatcher(Client.Scene.Id, ObjectPtr, value);
			}
		}

		/// <summary>
		/// Set to true to force mesh light to not cast shadows.
		/// </summary>
		public bool MeshLightNoCastShadow
		{
			set
			{
				CSycles.object_set_mesh_light_no_cast_shadow(Client.Scene.Id, ObjectPtr, value);
			}
		}

		/// <summary>
		/// Set object pass id.
		/// </summary>
		public int PassId
		{
			set
			{
				CSycles.object_set_pass_id(Client.Scene.Id, ObjectPtr, value);
			}
		}

		/// <summary>
		/// Set object random id.
		/// </summary>
		public uint RandomId
		{
			set
			{
				CSycles.object_set_random_id(Client.Scene.Id, ObjectPtr, value);
			}
		}
	}
}
