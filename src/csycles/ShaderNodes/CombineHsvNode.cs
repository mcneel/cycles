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
	public class CombineHsvInputs : Inputs
	{
		public FloatSocket H { get; set; }
		public FloatSocket S { get; set; }
		public FloatSocket V { get; set; }

		public CombineHsvInputs(ShaderNode parentNode)
		{
			H = new FloatSocket(parentNode, "H", "h");
			AddSocket(H);
			S = new FloatSocket(parentNode, "S", "s");
			AddSocket(S);
			V = new FloatSocket(parentNode, "V", "v");
			AddSocket(V);
		}
	}

	public class CombineHsvOutputs : Outputs
	{
		public ColorSocket Color { get; set; }

		public CombineHsvOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	/// <summary>
	/// Add a Combine HSV node, converting single H S V scalars to a vector output
	/// </summary>
	[ShaderNode("combine_hsv")]
	public class CombineHsvNode : ShaderNode
	{
		public CombineHsvInputs ins => (CombineHsvInputs)inputs;
		public CombineHsvOutputs outs => (CombineHsvOutputs)outputs;

		public CombineHsvNode(Shader shader) : this(shader, "A combine HSV node") { }
		public CombineHsvNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal CombineHsvNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new CombineHsvInputs(this);
			outputs = new CombineHsvOutputs(this);

			ins.H.Value = 0.0f;
			ins.S.Value = 0.0f;
			ins.V.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.H, xmlNode.GetAttribute("h"));
			Utilities.Instance.get_float(ins.S, xmlNode.GetAttribute("s"));
			Utilities.Instance.get_float(ins.V, xmlNode.GetAttribute("v"));
		}
	}
}
