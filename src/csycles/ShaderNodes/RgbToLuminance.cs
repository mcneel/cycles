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
using System;

namespace ccl.ShaderNodes
{

	/// <summary>
	/// RgbToBw node to convert a color to a value
	/// </summary>
	[ShaderNode("rgb_to_luminance")]
	public class RgbToLuminanceNode : ShaderNode
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
		public RgbToLuminanceNode(Shader shader) : this(shader, "An RgbToLuminance node") { }

		/// <summary>
		/// Create new RgbToLuminance node with given name
		/// </summary>
		public RgbToLuminanceNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal RgbToLuminanceNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new ConvertRgbInputs(this);
			outputs = new ConvertValOutputs(this);
		}

		internal override void ParseXml(System.Xml.XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
		}
	}
}
