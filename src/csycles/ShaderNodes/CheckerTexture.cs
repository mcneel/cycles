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
	public class CheckerInputs : Inputs
	{
		public VectorSocket Vector { get; set; }
		public ColorSocket Color1 { get; set; }
		public ColorSocket Color2 { get; set; }
		public FloatSocket Scale { get; set; }

		public CheckerInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
			Color1 = new ColorSocket(parentNode, "Color1", "color1");
			AddSocket(Color1);
			Color2 = new ColorSocket(parentNode, "Color2", "color2");
			AddSocket(Color2);
			Scale = new FloatSocket(parentNode, "Scale", "scale");
			AddSocket(Scale);
		}
	}

	public class CheckerOutputs : Outputs
	{
		public VectorSocket Color { get; set; }
		public FloatSocket Fac { get; set; }

		public CheckerOutputs(ShaderNode parentNode)
		{
			Color = new VectorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
		}
	}

	[ShaderNode("checker_texture")]
	public class CheckerTexture : TextureNode
	{
		public CheckerInputs ins => (CheckerInputs)inputs;
		public CheckerOutputs outs => (CheckerOutputs)outputs;
		public CheckerTexture(Shader shader) : this(shader, "a checker texture") { }
		public CheckerTexture(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal CheckerTexture(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new CheckerInputs(this);
			outputs = new CheckerOutputs(this);
			ins.Color1.Value = new float4(1.0f, 0.5f, 0.25f);
			ins.Color2.Value = new float4(0.25f, 0.5f, 1.0f);
			ins.Scale.Value = 5.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color1, xmlNode.GetAttribute("Color1"));
			Utilities.Instance.get_float4(ins.Color2, xmlNode.GetAttribute("Color2"));
		}
	}
}
