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
	public class HueSaturationInputs : Inputs
	{
		public FloatSocket Hue { get; set; }
		public FloatSocket Saturation { get; set; }
		public FloatSocket Value { get; set; }
		public FloatSocket Fac { get; set; }
		public ColorSocket Color { get; set; }

		public HueSaturationInputs(ShaderNode parentNode)
		{
			Hue = new FloatSocket(parentNode, "Hue", "hue");
			AddSocket(Hue);
			Saturation = new FloatSocket(parentNode, "Saturation", "saturation");
			AddSocket(Saturation);
			Value = new FloatSocket(parentNode, "Value", "value");
			AddSocket(Value);
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	public class HueSaturationOutputs : Outputs
	{
		public ClosureSocket Color { get; set; }

		public HueSaturationOutputs(ShaderNode parentNode)
		{
			Color = new ClosureSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	[ShaderNode("hsv")]
	public class HueSaturationNode : ShaderNode
	{
		public HueSaturationInputs ins => (HueSaturationInputs)inputs;
		public HueSaturationOutputs outs => (HueSaturationOutputs)outputs;

		public HueSaturationNode(Shader shader) : this(shader, "a HSV node") { }
		public HueSaturationNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal HueSaturationNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new HueSaturationInputs(this);
			outputs = new HueSaturationOutputs(this);

			ins.Hue.Value = 0.5f;
			ins.Saturation.Value = 1.0f;
			ins.Value.Value = 1.0f;
			ins.Fac.Value = 1.0f;
			ins.Color.Value = new float4(0.8f);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.Hue, xmlNode.GetAttribute("hue"));
			Utilities.Instance.get_float(ins.Saturation, xmlNode.GetAttribute("saturation"));
			Utilities.Instance.get_float(ins.Value, xmlNode.GetAttribute("value"));
			Utilities.Instance.get_float(ins.Fac, xmlNode.GetAttribute("fac"));
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
		}
	}
}
