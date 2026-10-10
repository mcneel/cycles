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
	public class SeparateXyzInputs : Inputs
	{
		public VectorSocket Vector { get; set; }

		public SeparateXyzInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
		}
	}

	public class SeparateXyzOutputs : Outputs
	{
		public FloatSocket X { get; set; }
		public FloatSocket Y { get; set; }
		public FloatSocket Z { get; set; }

		public SeparateXyzOutputs(ShaderNode parentNode)
		{
			X = new FloatSocket(parentNode, "X", "x");
			AddSocket(X);
			Y = new FloatSocket(parentNode, "Y", "y");
			AddSocket(Y);
			Z = new FloatSocket(parentNode, "Z", "z");
			AddSocket(Z);
		}
	}

	/// <summary>
	/// Add a Separate XYZ node, converting a vector input to single X Y Z scalar nodes
	/// </summary>
	[ShaderNode("separate_xyz")]
	public class SeparateXyzNode : ShaderNode
	{
		public SeparateXyzInputs ins => (SeparateXyzInputs)inputs;
		public SeparateXyzOutputs outs => (SeparateXyzOutputs)outputs;

		public SeparateXyzNode(Shader shader) : this(shader, "a separate XYZ node") { }
		public SeparateXyzNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal SeparateXyzNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new SeparateXyzInputs(this);
			outputs = new SeparateXyzOutputs(this);

			ins.Vector.Value = new float4(0.0f);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
		}
	}
}
