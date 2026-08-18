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
	/// <summary>
	/// RgbToBw input sockets
	/// </summary>
	public class ConvertRgbInputs : Inputs
	{
		/// <summary>
		/// RgbToBw input color
		/// </summary>
		public ColorSocket Color { get; set; }

		internal ConvertRgbInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	/// <summary>
	/// RgbToBw output sockets
	/// </summary>
	public class ConvertValOutputs : Outputs
	{
		/// <summary>
		/// RgbToBw value calculated from input color
		/// </summary>
		public FloatSocket Val { get; set; }

		internal ConvertValOutputs(ShaderNode parentNode)
		{
			Val = new FloatSocket(parentNode, "Val", "val");
			AddSocket(Val);
		}
	}

	/// <summary>
	/// RgbToBw node to convert a color to a value
	/// </summary>
	[ShaderNode("rgb_to_bw")]
	public class RgbToBwNode : ShaderNode
	{
		/// <summary>
		/// RgbToBw input sockets
		/// </summary>
		public ConvertRgbInputs ins => (ConvertRgbInputs)inputs;

		/// <summary>
		/// RgbToBw output sockets
		/// </summary>
		public ConvertValOutputs outs => (ConvertValOutputs)outputs;

		/// <summary>
		/// Create new RgbToBw node
		/// </summary>
		public RgbToBwNode(Shader shader) : this(shader, "An RgbToBw node") { }

		/// <summary>
		/// Create new RgbToBw node with given name
		/// </summary>
		public RgbToBwNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal RgbToBwNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new ConvertRgbInputs(this);
			outputs = new ConvertValOutputs(this);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
		}
	}
}
