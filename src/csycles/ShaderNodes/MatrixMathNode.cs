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
using System.Xml;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// MatrixMathNode input sockets
	/// </summary>
	public class MatrixMathInputs : Inputs
	{
		/// <summary>
		/// MatrixMathNode Vector input socket
		/// </summary>
		public VectorSocket Vector { get; set; }

		/// <summary>
		/// Create MatrixMathNode input sockets
		/// </summary>
		/// <param name="parentNode"></param>
		internal MatrixMathInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
		}
	}

	/// <summary>
	/// MatrixMathNode output sockets
	/// </summary>
	public class MatrixMathOutputs : Outputs
	{
		/// <summary>
		/// The resulting value of the MatrixMathNode operation
		/// </summary>
		public VectorSocket Vector { get; set; }

		/// <summary>
		/// Create MatrixMathNode output sockets
		/// </summary>
		/// <param name="parentNode"></param>
		internal MatrixMathOutputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
		}
	}

	/// <summary>
	/// Add a MatrixMath node, setting output Value with any of the following <c>Operation</c>s using Value1 and Value2
	///
	/// Note that some operations use only Value1
	/// </summary>
	[ShaderNode("matrix_math")]
	public class MatrixMathNode : ShaderNode
	{
		public enum Operations
		{
			Point,
			Direction,
			Perspective,
			Direction_Transposed,
		}

		/// <summary>
		/// MatrixMathNode input sockets
		/// </summary>
		public MatrixMathInputs ins => (MatrixMathInputs)inputs;

		/// <summary>
		/// MatrixMathNode output sockets
		/// </summary>
		public MatrixMathOutputs outs => (MatrixMathOutputs)outputs;

		/// <summary>
		/// MatrixMath node operates on float inputs (note, some operations use only Value1)
		/// </summary>
		public MatrixMathNode(Shader shader) : this(shader, "a matrix mathnode")
		{

		}

		public MatrixMathNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal MatrixMathNode(Shader shader, IntPtr ptr) : base(shader, ptr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new MatrixMathInputs(this);
			outputs = new MatrixMathOutputs(this);

			Operation = Operations.Point;

			ins.Vector.Value = new float4(0.0f);
		}

		public Transform Transform { get; set; }

		public Operations Operation { get; set; }

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "operation", (int)Operation);
		}

		internal override void SetDirectMembers()
		{
			var tfm = Transform;
			CSycles.shadernode_set_member_vec4_at_index(Id, "tfm_x", tfm.x.x, tfm.x.y, tfm.x.z, tfm.x.w, 0);
			CSycles.shadernode_set_member_vec4_at_index(Id, "tfm_y", tfm.y.x, tfm.y.y, tfm.y.z, tfm.y.w, 1);
			CSycles.shadernode_set_member_vec4_at_index(Id, "tfm_z", tfm.z.x, tfm.z.y, tfm.z.z, tfm.z.w, 2);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));

			var mat = xmlNode.GetAttribute("matrix");
			var t = new Transform();

			if (Utilities.Instance.get_transform(t, mat)) Transform = t;
		}
	}
}
