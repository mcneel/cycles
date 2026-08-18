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
	public class NormalMapInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Strength { get; set; }

		internal NormalMapInputs(ShaderNode parentNode)
		{
			Strength = new FloatSocket(parentNode, "Strength", "strength");
			AddSocket(Strength);
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	public class NormalMapOutputs : Outputs
	{
		public VectorSocket Normal { get; set; }

		internal NormalMapOutputs(ShaderNode parentNode)
		{
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
		}
	}

	[ShaderNode("normal_map")]
	public class NormalMapNode : ShaderNode
	{
		public enum Space
		{
			Tangent,
			Object,
			World,
			/*BlenderObject,
			BlenderWorld,*/
		}
		public NormalMapInputs ins => (NormalMapInputs)inputs;
		public NormalMapOutputs outs => (NormalMapOutputs)outputs;

		public NormalMapNode(Shader shader) : this(shader, "a normalmap node") { }
		public NormalMapNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal NormalMapNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new NormalMapInputs(this);
			outputs = new NormalMapOutputs(this);

			ins.Color.Value = new float4(0.8f);
			ins.Strength.Value = 1.0f;

			/* TODO: Add Attribute property for custom maps*/
		}

		public Space SpaceType { get; set; } = Space.Tangent;

		public string Attribute { get; set; } = "uvmap1";

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "type", (int)SpaceType);
		}

		internal override void SetDirectMembers()
		{
			base.SetDirectMembers();
			CSycles.shadernode_set_member_string(Id, "attribute", Attribute);
		}



		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Strength, xmlNode.GetAttribute("strength"));
		}
		public override string CreateXmlAttributes()
		{
			var code = new StringBuilder($" type=\"{SpaceType}\" ", 1024);

			return code.ToString();
		}

		public override string CreateCodeAttributes()
		{
			var code = new StringBuilder($"{VariableName}.SpaceType = ccl.ShaderNodes.NormalMapNode.Space.{SpaceType};", 1024);

			return code.ToString();
		}
	}
}
