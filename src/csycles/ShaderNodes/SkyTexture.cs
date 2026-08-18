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
	public class SkyInputs : Inputs
	{
		public VectorSocket Vector { get; set; }

		public SkyInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
		}
	}

	public class SkyOutputs : Outputs
	{
		public ColorSocket Color { get; set; }

		public SkyOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	[ShaderNode("sky_texture")]
	public class SkyTexture : TextureNode
	{
		public SkyInputs ins => (SkyInputs)inputs;
		public SkyOutputs outs => (SkyOutputs)outputs;

		public SkyTexture(Shader shader) : this(shader, "a sky texture") { }
		public SkyTexture(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal SkyTexture(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new SkyInputs(this);
			outputs = new SkyOutputs(this);

			SunDirection = new float4(0.0f, 0.0f, 1.0f);
			Turbidity = 2.2f;
			GroundAlbedo = 0.3f;
			SkyType = SkyTypes.Hosek_Wilkie;
		}

		public float4 SunDirection { get; set; }
		public float Turbidity { get; set; }
		public float GroundAlbedo { get; set; }

		public enum SkyTypes
		{
			Preetham,
			Hosek_Wilkie
		}

		/// <summary>
		/// One of:
		/// - Preetham
		/// - Hosek / Wilkie
		/// </summary>
		public SkyTypes SkyType { get; set; }

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "sky", (int)SkyType);
		}

		internal override void SetDirectMembers()
		{
			base.SetDirectMembers();
			CSycles.shadernode_set_member_float(Id, "turbidity", Turbidity);
			CSycles.shadernode_set_member_float(Id, "ground_albedo", GroundAlbedo);
			var sd = SunDirection;
			CSycles.shadernode_set_member_vec(Id, "sun_direction", sd.x, sd.y, sd.z);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));

			var sun_direction = xmlNode.GetAttribute("sun_direction");
			var turbidity = xmlNode.GetAttribute("turbidity");
			var ground_albedo = xmlNode.GetAttribute("ground_albedo");
			var sky_type = xmlNode.GetAttribute("type");

			if (!string.IsNullOrEmpty(sun_direction))
			{
				Utilities.Instance.get_float4(SunDirection, sun_direction);
			}
			if (!string.IsNullOrEmpty(turbidity))
			{
				var turb = 0.0f;
				if (Utilities.Instance.get_float(ref turb, turbidity)) Turbidity = turb;
			}
			if (!string.IsNullOrEmpty(ground_albedo))
			{
				var ground_alb = 0.0f;
				if (Utilities.Instance.get_float(ref ground_alb, ground_albedo)) GroundAlbedo = ground_alb;
			}
			if (!string.IsNullOrEmpty(sky_type))
			{
				if (Enum.TryParse(sky_type, out SkyTypes st))
				{
					SkyType = st;
				}
			}
		}
		public override string CreateXmlAttributes()
		{
			var code = new StringBuilder($" type=\"{SkyType}\" ", 1024);
			code.Append($" ground_albedo=\"{GroundAlbedo}\" ");
			code.Append($" turbidity=\"{Turbidity}\" ");
			code.Append($" sun_direction=\"{SunDirection.x} {SunDirection.y} {SunDirection.z}\" ");

			return code.ToString();
		}
		public override string CreateCodeAttributes()
		{
			var code = new StringBuilder($"{VariableName}.SkyType = {SkyType};", 1024);
			code.Append($"{VariableName}.GroundAlbedo = {GroundAlbedo};");
			code.Append($"{VariableName}.Turbidity = {Turbidity};");
			code.Append($"{VariableName}.SunDirection = new float4({SunDirection.x} , {SunDirection.y}, {SunDirection.z}, 0.0);");

			return code.ToString();
		}
	}
}
