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
	public class CombineXyzInputs : Inputs
	{
		public FloatSocket X { get; set; }
		public FloatSocket Y { get; set; }
		public FloatSocket Z { get; set; }

		public CombineXyzInputs(ShaderNode parentNode)
		{
			X = new FloatSocket(parentNode, "X", "x");
			AddSocket(X);
			Y = new FloatSocket(parentNode, "Y", "y");
			AddSocket(Y);
			Z = new FloatSocket(parentNode, "Z", "z");
			AddSocket(Z);
		}
	}

	public class CombineXyzOutputs : Outputs
	{
		public VectorSocket Vector { get; set; }

		public CombineXyzOutputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
		}
	}

	/// <summary>
	/// Add a Combine XYZ node, converting single X Y Z scalars to a vector output
	/// </summary>
	[ShaderNode("combine_xyz")]
	public class CombineXyzNode : ShaderNode
	{
		public CombineXyzInputs ins => (CombineXyzInputs)inputs;
		public CombineXyzOutputs outs => (CombineXyzOutputs)outputs;

		public CombineXyzNode(Shader shader) : this(shader, "a combine XYZ node") { }
		public CombineXyzNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal CombineXyzNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new CombineXyzInputs(this);
			outputs = new CombineXyzOutputs(this);

			ins.X.Value = 0.0f;
			ins.Y.Value = 0.0f;
			ins.Z.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.X, xmlNode.GetAttribute("x"));
			Utilities.Instance.get_float(ins.Y, xmlNode.GetAttribute("y"));
			Utilities.Instance.get_float(ins.Z, xmlNode.GetAttribute("z"));
		}
	}
}
