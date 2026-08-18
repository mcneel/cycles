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
	public class GlossyInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Roughness { get; set; }
		public VectorSocket Normal { get; set; }

		internal GlossyInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Roughness = new FloatSocket(parentNode, "Roughness", "roughness");
			AddSocket(Roughness);
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
		}
	}

	public class GlossyOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		internal GlossyOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF");
			AddSocket(BSDF);
		}
	}

	[ShaderNode("glossy_bsdf")]
	public class GlossyBsdfNode : ShaderNode
	{

		public enum GlossyDistribution
		{
			Sharp = 8,
			Beckmann = 12,
			GGX = 9,
			Asihkmin_Shirley = 15,
			Multiscatter_GGX = 13
		}

		public GlossyInputs ins => (GlossyInputs)inputs;
		public GlossyOutputs outs => (GlossyOutputs)outputs;

		public GlossyBsdfNode(Shader shader) : this(shader, "a glossy bsdf node") { }
		public GlossyBsdfNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal GlossyBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new GlossyInputs(this);
			outputs = new GlossyOutputs(this);
			Distribution = GlossyDistribution.Multiscatter_GGX;
			ins.Color.Value = new float4();
			ins.Roughness.Value = 0.0f;
		}

		public GlossyDistribution Distribution { get; set; }

		public void SetDistribution(string dist)
		{
			dist = dist.Replace("-", "_");
			Distribution = (GlossyDistribution)Enum.Parse(typeof(GlossyDistribution), dist, true);
		}

		private string GlossyToString(GlossyDistribution dist)
		{
			var str = dist.ToString();
			str = str.Replace("_", "-");
			return str;
		}

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "distribution", (int)Distribution);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float4(ins.Normal, xmlNode.GetAttribute("normal"));
			Utilities.Instance.get_float(ins.Roughness, xmlNode.GetAttribute("roughness"));
			var glossydistribution = xmlNode.GetAttribute("distribution");
			if (!string.IsNullOrEmpty(glossydistribution))
			{
				SetDistribution(glossydistribution);
			}
		}
	}
}
