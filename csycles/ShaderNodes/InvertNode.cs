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
	public class InvertInputs : Inputs
	{
		public FloatSocket Fac { get; set; }
		public ColorSocket Color { get; set; }

		internal InvertInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
		}
	}

	public class InvertOutputs : Outputs
	{
		public ColorSocket Color { get; set; }

		internal InvertOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	/// <summary>
	/// Invert color node
	/// </summary>
	[ShaderNode("invert")]
	public class InvertNode : ShaderNode
	{
		public InvertInputs ins => (InvertInputs)inputs;
		public InvertOutputs outs => (InvertOutputs)outputs;

		/// <summary>
		/// Create new InvertNode. By default Color input is black.
		/// </summary>
		public InvertNode(Shader shader) : this(shader, "an invert color node")
		{
		}

		/// <summary>
		/// Create new InvertNode with blend type Invert and name.
		/// </summary>
		/// <param name="name"></param>
		public InvertNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal InvertNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new InvertInputs(this);
			outputs = new InvertOutputs(this);

			ins.Fac.Value = 0.5f;
			ins.Color.Value = new float4();
		}

		public bool UseClamp { get; set; }

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Fac, xmlNode.GetAttribute("fac"));
		}

	}
}
