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
			Image = new ColorSocket(parentNode, "Color", "color");
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
			R = new FloatSocket(parentNode, "Red", "r");
			AddSocket(R);
			G = new FloatSocket(parentNode, "Green", "g");
			AddSocket(G);
			B = new FloatSocket(parentNode, "Blue", "b");
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

		/* Cycles 5.2 removed separate_rgb, separate_hsv, combine_rgb and
		 * combine_hsv, folding them into separate_color and combine_color with a
		 * color_type enum. NodeType::find returned null for the old name and
		 * ccycles dereferenced it, which took Rhino down as soon as anything built
		 * a background shader.
		 *
		 * The attribute keeps the old name because it is also the XML key, and two
		 * classes claiming "separate_color" would collide in
		 * g_registered_shadernodes - it keeps the first one it enumerates and
		 * silently drops the other. Overriding the Cycles type name separately is
		 * the same shape the MathNode subclasses already use.
		 *
		 * Both socket names matter, for different things. The internal name is what
		 * sets a value; the ui name is what Connect matches on, because
		 * ShaderInput::name() returns socket_type.ui_name and csycles passes UiName.
		 * separate_color keeps the internal names separate_rgb used - color, r, g, b
		 * - but renamed the ui names to Color, Red, Green and Blue, so those had to
		 * change here or every connection silently found no socket. The C#
		 * properties stay Image, R, G and B. */
		public override string ShaderNodeTypeName => "separate_color";

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "color_type", 0); /* NODE_COMBSEP_COLOR_RGB */
		}
	}
}
