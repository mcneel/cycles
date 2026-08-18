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
	public class SeparateRgbInputs : Inputs
	{
		public ColorSocket Image { get; set; }

		public SeparateRgbInputs(ShaderNode parentNode)
		{
			Image = new ColorSocket(parentNode, "Image", "color");
			AddSocket(Image);
		}
	}

	public class SeparateRgbOutputs : Outputs
	{
		public FloatSocket R { get; set; }
		public FloatSocket G { get; set; }
		public FloatSocket B { get; set; }

		public SeparateRgbOutputs(ShaderNode parentNode)
		{
			R = new FloatSocket(parentNode, "R", "r");
			AddSocket(R);
			G = new FloatSocket(parentNode, "G", "g");
			AddSocket(G);
			B = new FloatSocket(parentNode, "B", "b");
			AddSocket(B);
		}
	}

	[ShaderNode("separate_rgb")]
	public class SeparateRgbNode : ShaderNode
	{
		public SeparateRgbInputs ins => (SeparateRgbInputs)inputs;
		public SeparateRgbOutputs outs { get { return (SeparateRgbOutputs)outputs; } }

		/// <summary>
		/// Create new Separate RGB node.
		/// </summary>
		public SeparateRgbNode(Shader shader) : this(shader, "a separate rgb node") { }
		public SeparateRgbNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal SeparateRgbNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new SeparateRgbInputs(this);
			outputs = new SeparateRgbOutputs(this);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Image, xmlNode.GetAttribute("image"));
		}
	}
}
