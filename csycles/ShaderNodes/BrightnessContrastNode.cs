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
	public class BrightnessContrastInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Bright { get; set; }
		public FloatSocket Contrast { get; set; }

		public BrightnessContrastInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Bright = new FloatSocket(parentNode, "Bright", "bright");
			AddSocket(Bright);
			Contrast = new FloatSocket(parentNode, "Contrast", "contrast");
			AddSocket(Contrast);
		}
	}

	public class BrightnessContrastOutputs : Outputs
	{
		public ClosureSocket Color { get; set; }

		public BrightnessContrastOutputs(ShaderNode parentNode)
		{
			Color = new ClosureSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	[ShaderNode("brightness_contrast")]
	public class BrightnessContrastNode : ShaderNode
	{
		public BrightnessContrastInputs ins => (BrightnessContrastInputs)inputs;
		public BrightnessContrastOutputs outs => (BrightnessContrastOutputs)outputs;

		public BrightnessContrastNode(Shader shader) : this(shader, "a brightness contrast node") { }
		public BrightnessContrastNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal BrightnessContrastNode(Shader shader, IntPtr shaderNodePtr) : base(shader, shaderNodePtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new BrightnessContrastInputs(this);
			outputs = new BrightnessContrastOutputs(this);

			ins.Color.Value = new float4(0.8f);
			ins.Bright.Value = 0.0f;
			ins.Contrast.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Bright, xmlNode.GetAttribute("bright"));
			Utilities.Instance.get_float(ins.Contrast, xmlNode.GetAttribute("contrast"));
		}
	}
}
