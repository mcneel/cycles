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
	public class AnisotropicBsdfInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public VectorSocket Tangent { get; set; }
		public VectorSocket Normal { get; set; }
		public FloatSocket Roughness { get; set; }
		public FloatSocket Anisotropy { get; set; }
		public FloatSocket Rotation { get; set; }

		public AnisotropicBsdfInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Tangent = new VectorSocket(parentNode, "Tangent", "tangent");
			AddSocket(Tangent);
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
			Roughness = new FloatSocket(parentNode, "Roughness", "roughness");
			AddSocket(Roughness);
			Anisotropy = new FloatSocket(parentNode, "Anisotropy", "anisotropy");
			AddSocket(Anisotropy);
			Rotation = new FloatSocket(parentNode, "Rotation", "rotation");
			AddSocket(Rotation);
		}
	}

	public class AnisotropicBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public AnisotropicBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF");
			AddSocket(BSDF);
		}
	}

	/// <summary>
	/// A Anisotropic BSDF closure.
	///
	/// There is one output <c>Closure</c>
	/// </summary>
	[ShaderNode("anisotropic_bsdf")]
	public class AnisotropicBsdfNode : ShaderNode
	{
		public enum AnisotropicDistribution
		{
			Beckmann = 20,
			GGX = 16,
			Multiscatter_GGX = 18,
			Asihkmin_Shirley = 21,
		}
		public AnisotropicBsdfInputs ins => (AnisotropicBsdfInputs)inputs;
		public AnisotropicBsdfOutputs outs => (AnisotropicBsdfOutputs)outputs;

		/// <summary>
		/// Create a new Anisotropic BSDF closure.
		/// </summary>
		public AnisotropicBsdfNode(Shader shader) : this(shader, "a anisotropic bsdf node") { }
		public AnisotropicBsdfNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal AnisotropicBsdfNode(Shader shader, IntPtr shaderNodePtr) : base(shader, shaderNodePtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new AnisotropicBsdfInputs(this);
			outputs = new AnisotropicBsdfOutputs(this);
			ins.Color.Value = new float4(1.0f);
			ins.Tangent.Value = new float4(0.0f, 0.0f, 0.0f, 1.0f);
			ins.Roughness.Value = 0.2f;
			ins.Anisotropy.Value = 0.5f;
			ins.Rotation.Value = 0.0f;

			Distribution = AnisotropicDistribution.GGX;
		}

		public void SetDistribution(string dist)
		{
			dist = dist.Replace("-", "_");
			Distribution = (AnisotropicDistribution)System.Enum.Parse(typeof(AnisotropicDistribution), dist, true);
		}

		private string AnisotropicToString(AnisotropicDistribution dist)
		{
			var str = dist.ToString();
			str = str.Replace("_", "-");
			return str;
		}

		AnisotropicDistribution Distribution { get; set; }
		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "distribution", (int)Distribution);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float4(ins.Normal, xmlNode.GetAttribute("normal"));
			Utilities.Instance.get_float4(ins.Tangent, xmlNode.GetAttribute("tangent"));
			Utilities.Instance.get_float(ins.Roughness, xmlNode.GetAttribute("roughness"));
			Utilities.Instance.get_float(ins.Anisotropy, xmlNode.GetAttribute("anisotropy"));
			Utilities.Instance.get_float(ins.Rotation, xmlNode.GetAttribute("rotation"));
			var anidistribution = xmlNode.GetAttribute("distribution");
			if (!string.IsNullOrEmpty(anidistribution))
			{
				SetDistribution(anidistribution);
			}

		}
	}
}
