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
	public class CombineRgbInputs : Inputs
	{
		public FloatSocket R { get; set; }
		public FloatSocket G { get; set; }
		public FloatSocket B { get; set; }

		public CombineRgbInputs(ShaderNode parentNode)
		{
			R = new FloatSocket(parentNode, "R", "r");
			AddSocket(R);
			G = new FloatSocket(parentNode, "G", "g");
			AddSocket(G);
			B = new FloatSocket(parentNode, "B", "b");
			AddSocket(B);
		}
	}

	public class CombineRgbOutputs : Outputs
	{
		public ColorSocket Image { get; set; }

		public CombineRgbOutputs(ShaderNode parentNode)
		{
			Image = new ColorSocket(parentNode, "Image", "image");
			AddSocket(Image);
		}
	}

	/// <summary>
	/// Add a Combine RGB node, converting single R G B scalars to a vector output
	/// </summary>
	[ShaderNode("combine_rgb")]
	public class CombineRgbNode : ShaderNode
	{
		public CombineRgbInputs ins => (CombineRgbInputs)inputs;
		public CombineRgbOutputs outs => (CombineRgbOutputs)outputs;

		public CombineRgbNode(Shader shader) : this(shader, "a combine rgb node") { }
		public CombineRgbNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal CombineRgbNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new CombineRgbInputs(this);
			outputs = new CombineRgbOutputs(this);

			ins.R.Value = 0.0f;
			ins.G.Value = 0.0f;
			ins.B.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.R, xmlNode.GetAttribute("r"));
			Utilities.Instance.get_float(ins.G, xmlNode.GetAttribute("g"));
			Utilities.Instance.get_float(ins.B, xmlNode.GetAttribute("b"));
		}
	}
}
