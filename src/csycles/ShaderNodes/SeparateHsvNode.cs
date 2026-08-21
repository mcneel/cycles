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
	public class SeparateHsvInputs : Inputs
	{
		public ColorSocket Color { get; set; }

		public SeparateHsvInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	public class SeparateHsvOutputs : Outputs
	{
		public FloatSocket H { get; set; }
		public FloatSocket S { get; set; }
		public FloatSocket V { get; set; }

		public SeparateHsvOutputs(ShaderNode parentNode)
		{
			/* separate_color names its outputs r/g/b - ui names Red/Green/Blue -
			 * whatever the colour type is, so the hsv components map onto those. Both
			 * names have to match: the internal name is what sets a value, and the ui
			 * name is what Connect matches on, because ShaderInput::name() returns
			 * socket_type.ui_name and csycles passes UiName. The C# properties stay
			 * H, S and V, so callers read the same as before. */
			H = new FloatSocket(parentNode, "Red", "r");
			AddSocket(H);
			S = new FloatSocket(parentNode, "Green", "g");
			AddSocket(S);
			V = new FloatSocket(parentNode, "Blue", "b");
			AddSocket(V);
		}
	}

	[ShaderNode("separate_hsv")]
	public class SeparateHsvNode : ShaderNode
	{
		public SeparateHsvInputs ins => (SeparateHsvInputs)inputs;
		public SeparateHsvOutputs outs => (SeparateHsvOutputs)outputs;

		/// <summary>
		/// Create new Separate HSV node.
		/// </summary>
		public SeparateHsvNode(Shader shader) : this(shader, "a separate HSV node") { }
		public SeparateHsvNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal SeparateHsvNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new SeparateHsvInputs(this);
			outputs = new SeparateHsvOutputs(this);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
		}

		/* separate_hsv is gone in 5.2; separate_color with color_type hsv does the
		 * same work. See SeparateRgbNode for why the attribute name has to stay as
		 * it is rather than becoming separate_color too. */
		public override string ShaderNodeTypeName => "separate_color";

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "color_type", 1); /* NODE_COMBSEP_COLOR_HSV */
		}
	}
}
