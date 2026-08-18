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
	public class AbsorptionVolumeInputs : Inputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Density { get; set; }

		public AbsorptionVolumeInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Density = new FloatSocket(parentNode, "Density", "density");
			AddSocket(Density);
		}
	}

	public class AbsorptionVolumeOutputs : Outputs
	{
		public ClosureSocket Volume { get; set; }

		public AbsorptionVolumeOutputs(ShaderNode parentNode)
		{
			Volume = new ClosureSocket(parentNode, "Volume", "volume");
			AddSocket(Volume);
		}
	}

	/// <summary>
	/// A absorption volume node.
	/// </summary>
	[ShaderNode("absorption_volume")]
	public class AbsorptionVolumeNode : ShaderNode
	{
		public AbsorptionVolumeInputs ins => (AbsorptionVolumeInputs)inputs;
		public AbsorptionVolumeOutputs outs => (AbsorptionVolumeOutputs)outputs;

		/// <summary>
		/// Create a new Absorption volume node
		/// </summary>
		public AbsorptionVolumeNode(Shader shader) : this(shader, "a absorption volume node") { }

		public AbsorptionVolumeNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal AbsorptionVolumeNode(Shader shader, IntPtr shaderNodePtr) : base(shader, shaderNodePtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new AbsorptionVolumeInputs(this);
			outputs = new AbsorptionVolumeOutputs(this);
			ins.Color.Value = new float4(1.0f);
			ins.Density.Value = 1.0f;
		}


		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Density, xmlNode.GetAttribute("density"));
		}
	}
}
