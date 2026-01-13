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
	public class GlassInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Roughness { get; set; }
		public FloatSocket IOR { get; set; }
		public VectorSocket Normal { get; set; }

		public GlassInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Roughness = new FloatSocket(parentNode, "Roughness", "roughness");
			AddSocket(Roughness);
			IOR = new FloatSocket(parentNode, "IOR", "IOR");
			AddSocket(IOR);
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
		}
	}

	public class GlassOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public GlassOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF");
			AddSocket(BSDF);
		}
	}

	[ShaderNode("glass_bsdf")]
	public class GlassBsdfNode : ShaderNode
	{

		public enum GlassDistribution
		{
			Sharp = 34,
			Beckmann = 31,
			GGX = 32,
			Multiscatter_GGX = 30
		}

		public GlassInputs ins => (GlassInputs)inputs;
		public GlassOutputs outs => (GlassOutputs)outputs;
		public GlassBsdfNode(Shader shader) : this(shader, "a glass bsdf") { }
		public GlassBsdfNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal GlassBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new GlassInputs(this);
			outputs = new GlassOutputs(this);

			Distribution = GlassDistribution.Multiscatter_GGX;
		}

		public GlassDistribution Distribution { get; set; }

		public void SetDistribution(string dist)
		{
			Distribution = (GlassDistribution)Enum.Parse(typeof(GlassDistribution), dist, true);
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
			Utilities.Instance.get_float(ins.IOR, xmlNode.GetAttribute("ior"));
			var glassdistribution = xmlNode.GetAttribute("distribution");
			if (!string.IsNullOrEmpty(glassdistribution))
			{
				SetDistribution(glassdistribution);
			}
		}
	}
}
