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
	public class GammaInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Gamma { get; set; }

		public GammaInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Gamma = new FloatSocket(parentNode, "Gamma", "gamma");
			AddSocket(Gamma);
		}
	}

	public class GammaOutputs : Outputs
	{
		public ClosureSocket Color { get; set; }

		public GammaOutputs(ShaderNode parentNode)
		{
			Color = new ClosureSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	[ShaderNode("gamma")]
	public class GammaNode : ShaderNode
	{
		public GammaInputs ins => (GammaInputs)inputs;
		public GammaOutputs outs => (GammaOutputs)outputs;

		public GammaNode(Shader shader) : this(shader, "a gamma node") { }
		public GammaNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal GammaNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new GammaInputs(this);
			outputs = new GammaOutputs(this);

			ins.Color.Value = new float4(0.8f);
			ins.Gamma.Value = 1.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Gamma, xmlNode.GetAttribute("gamma"));
		}
	}
}
