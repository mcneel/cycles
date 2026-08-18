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

using ccl.Attributes;
using ccl.ShaderNodes.Sockets;
using System;

namespace ccl.ShaderNodes
{
	public class GeometryInfoOutputs : Outputs
	{
		//public VectorSocket NormalIn { get; set; }
		/// <summary>
		/// Position of the shading point
		/// </summary>
		public VectorSocket Position { get; set; }
		/// <summary>
		/// Shading normal at the surface (includes smooth normals and bump mapping)
		/// </summary>

		public VectorSocket Normal { get; set; }
		/// <summary>
		/// Tangent at the surface
		/// </summary>
		public VectorSocket Tangent { get; set; }
		/// <summary>
		/// Normal of the underlaying geometry (flat surface)
		/// </summary>
		public VectorSocket TrueNormal { get; set; }
		/// <summary>
		/// Vector from view towards shading point
		/// </summary>
		public VectorSocket Incoming { get; set; }
		/// <summary>
		/// Parametric coordinates of the shading point on the surface
		/// </summary>
		public VectorSocket Parametric { get; set; }
		/// <summary>
		/// 1.0 if the face is viewed from the backside, 0.0 if from the front
		/// </summary>
		public FloatSocket Backfacing { get; set; }
		/// <summary>
		/// An approximation of the curvature of the mesh per-vertex.
		/// Lighter values indicate convex angles, darker values indicate concave angles.
		/// </summary>
		public FloatSocket Pointiness { get; set; }
		public FloatSocket RandomPerIsland { get; set; }

		internal GeometryInfoOutputs(ShaderNode parentNode)
		{
			//IsCameraRay = new FloatSocket(parentNode, "Is Camera Ray");
			//AddSocket(IsCameraRay);
			Position = new VectorSocket(parentNode, "Position", "position");
			AddSocket(Position);
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
			Tangent = new VectorSocket(parentNode, "Tangent", "tangent");
			AddSocket(Tangent);
			TrueNormal = new VectorSocket(parentNode, "True Normal", "true_normal");
			AddSocket(TrueNormal);
			Incoming = new VectorSocket(parentNode, "Incoming", "incoming");
			AddSocket(Incoming);
			Parametric = new VectorSocket(parentNode, "Parametric", "parametric");
			AddSocket(Parametric);
			Backfacing = new FloatSocket(parentNode, "Backfacing", "backfacing");
			AddSocket(Backfacing);
			Pointiness = new FloatSocket(parentNode, "Pointiness", "pointiness");
			AddSocket(Pointiness);
			RandomPerIsland = new FloatSocket(parentNode, "RandomPerIsland", "random_per_island");
			AddSocket(RandomPerIsland);
		}
	}

	/// <summary>
	/// GeometryInfo input sockets. Not used, here for cast purposes.
	/// </summary>
	public class GeometryInfoInputs : Inputs
	{
	}

	/// <summary>
	/// GeometryInfo node gives information about rays Cycles is handling.
	/// </summary>
	[ShaderNode("geometry")]
	public class GeometryInfoNode : ShaderNode
	{
		/// <summary>
		/// GeometryInfo node input sockets
		/// </summary>
		public GeometryInfoInputs ins => (GeometryInfoInputs)inputs;

		/// <summary>
		/// GeometryInfo node output sockets
		/// </summary>
		public GeometryInfoOutputs outs => (GeometryInfoOutputs)outputs;

		/// <summary>
		/// Create a new GeometryInfoNode
		/// </summary>
		public GeometryInfoNode(Shader shader)
			: this(shader, "a geometry info node")
		{
		}

		public GeometryInfoNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal GeometryInfoNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new GeometryInfoInputs();
			outputs = new GeometryInfoOutputs(this);
		}
	}
}
