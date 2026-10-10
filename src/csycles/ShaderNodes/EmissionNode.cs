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
	public class EmissionInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Strength { get; set; }

		internal EmissionInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Strength = new FloatSocket(parentNode, "Strength", "strength");
			AddSocket(Strength);
		}
	}

	public class EmissionOutputs : Outputs
	{
		public ClosureSocket Emission { get; set; }

		internal EmissionOutputs(ShaderNode parentNode)
		{
			Emission = new ClosureSocket(parentNode, "Emission", "emission");
			AddSocket(Emission);
		}
	}

	[ShaderNode("emission")]
	public class EmissionNode : ShaderNode
	{
		public EmissionInputs ins => (EmissionInputs)inputs;
		public EmissionOutputs outs => (EmissionOutputs)outputs;

		public EmissionNode(Shader shader) : this(shader, "an emission node") { }
		public EmissionNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal EmissionNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new EmissionInputs(this);
			outputs = new EmissionOutputs(this);

			ins.Color.Value = new float4(0.8f);
			ins.Strength.Value = 1.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Strength, xmlNode.GetAttribute("strength"));
		}
	}
}
