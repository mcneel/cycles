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
	public class DiffuseBsdfInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Roughness { get; set; }
		public VectorSocket Normal { get; set; }

		public DiffuseBsdfInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Roughness = new FloatSocket(parentNode, "Roughness", "roughness");
			AddSocket(Roughness);
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
		}
	}

	public class DiffuseBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public DiffuseBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF");
			AddSocket(BSDF);
		}
	}

	/// <summary>
	/// A Diffuse BSDF closure.
	/// This closure takes two inputs, <c>Color</c> and <c>Roughness</c>. The result
	/// will be a regular diffuse shading.
	///
	/// There is one output <c>Closure</c>
	/// </summary>
	[ShaderNode("diffuse_bsdf")]
	public class DiffuseBsdfNode : ShaderNode
	{
		public DiffuseBsdfInputs ins => (DiffuseBsdfInputs)inputs;
		public DiffuseBsdfOutputs outs => (DiffuseBsdfOutputs)outputs;

		/// <summary>
		/// Create a new Diffuse BSDF closure.
		/// </summary>
		public DiffuseBsdfNode(Shader shader) : this(shader, "a diffuse bsdf node") { }
		public DiffuseBsdfNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal DiffuseBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new DiffuseBsdfInputs(this);
			outputs = new DiffuseBsdfOutputs(this);
			ins.Color.Value = new float4(0.0f, 0.0f, 0.0f, 1.0f);
			ins.Roughness.Value = 0.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Roughness, xmlNode.GetAttribute("roughness"));
		}

		public override ClosureSocket GetClosureSocket()
		{
			return outs.BSDF;
		}

	}
}
