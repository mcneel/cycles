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
using System.Text;
using System.Xml;

namespace ccl.ShaderNodes
{
	public class MixInputs : Inputs
	{
		public FloatSocket Fac { get; set; }
		public ColorSocket Color1 { get; set; }
		public ColorSocket Color2 { get; set; }

		internal MixInputs(ShaderNode parentNode)
		{
			Color1 = new ColorSocket(parentNode, "Color1", "color1");
			AddSocket(Color1);
			Color2 = new ColorSocket(parentNode, "Color2", "color2");
			AddSocket(Color2);
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
		}
	}

	public class MixOutputs : Outputs
	{
		public ColorSocket Color { get; set; }

		internal MixOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	/// <summary>
	/// Blending node for two colors. Default BlendType is BlendTypes.Mix
	/// </summary>
	[ShaderNode("mix")]
	public class MixNode : ShaderNode
	{
		public enum BlendTypes
		{
			Blend,
			Add,
			Multiply,
			Subtract,
			Screen,
			Divide,
			Difference,
			Darken,
			Lighten,
			Overlay,
			Dodge,
			Burn,
			Hue,
			Saturation,
			Value,
			Color,
			Soft_Light,
			Linear_Light,
		}


		public MixInputs ins => (MixInputs)inputs;
		public MixOutputs outs => (MixOutputs)outputs;

		/// <summary>
		/// Create new MixNode with blend type Mix. By default Color inputs are black.
		/// </summary>
		public MixNode(Shader shader) : this(shader, "a mix color node")
		{
		}

		/// <summary>
		/// Create new MixNode with blend type Mix and name.
		/// </summary>
		/// <param name="name"></param>
		public MixNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal MixNode(Shader shader, IntPtr intptr) : base(shader, intptr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new MixInputs(this);
			outputs = new MixOutputs(this);

			BlendType = BlendTypes.Blend;
			UseClamp = false;

			ins.Fac.Value = 0.5f;
			ins.Color1.Value = new float4();
			ins.Color2.Value = new float4();
		}

		public BlendTypes BlendType { get; set; }
		public bool UseClamp { get; set; }

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "type", (int)BlendType);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_bool(Id, "use_clamp", UseClamp);
		}

		private void SetBlendType(string op)
		{
			op = op.Replace(" ", "_");
			BlendType = (BlendTypes)Enum.Parse(typeof(BlendTypes), op, true);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color1, xmlNode.GetAttribute("color1"));
			Utilities.Instance.get_float4(ins.Color2, xmlNode.GetAttribute("color2"));
			Utilities.Instance.get_float(ins.Fac, xmlNode.GetAttribute("fac"));
			bool useclamp = false;
			Utilities.Instance.get_bool(ref useclamp, xmlNode.GetAttribute("use_clamp"));
			UseClamp = useclamp;

			var blendtype = xmlNode.GetAttribute("type");
			if (!string.IsNullOrEmpty(blendtype))
			{
				SetBlendType(blendtype);
			}
		}

		public override string CreateXmlAttributes()
		{
			var code = new StringBuilder($" type=\"{BlendType}\" ", 1024);
			code.Append($" use_clamp=\"{UseClamp}\"");

			return code.ToString();
		}

		public override string CreateCodeAttributes()
		{
			var code = new StringBuilder($"{VariableName}.BlendType = ccl.ShaderNodes.MixNode.BlendTypes.{BlendType};", 1024);
			code.Append($"{VariableName}.UseClamp = {UseClamp.ToString().ToLowerInvariant()};");

			return code.ToString();
		}
	}
}
