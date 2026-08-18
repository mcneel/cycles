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
	public class ToonBsdfInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public VectorSocket Normal { get; set; }

		public FloatSocket Size { get; set; }
		public FloatSocket Smooth { get; set; }

		internal ToonBsdfInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
			Size = new FloatSocket(parentNode, "Size", "size");
			AddSocket(Size);
			Smooth = new FloatSocket(parentNode, "Smooth", "smooth");
			AddSocket(Smooth);

		}
	}

	public class ToonBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		internal ToonBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF");
			AddSocket(BSDF);
		}
	}

	/// <summary>
	/// A Toon BSDF closure.

	/// There is one output <c>BSDF</c>
	/// </summary>
	[ShaderNode("toon_bsdf")]
	public class ToonBsdfNode : ShaderNode
	{
		public ToonBsdfInputs ins => (ToonBsdfInputs)inputs;
		public ToonBsdfOutputs outs => (ToonBsdfOutputs)outputs;

		public enum Components
		{
			Diffuse = 7,
			Glossy = 24,
		}

		/// <summary>
		/// Create a new Toon BSDF closure. Default Color is white
		/// </summary>
		public ToonBsdfNode(Shader shader) : this(shader, "a toon bsdf node") { }
		/// <summary>
		/// Create a new Toon BSDF closure. Default Color is white
		/// </summary>
		public ToonBsdfNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal ToonBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new ToonBsdfInputs(this);
			outputs = new ToonBsdfOutputs(this);
			ins.Color.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
			ins.Size.Value = 0.5f;
			ins.Smooth.Value = 0.0f;
			Component = Components.Diffuse;
		}

		public Components Component { get; set; }

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "component", (int)Component);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
		}
	}
}
