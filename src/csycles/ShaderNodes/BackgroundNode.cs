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
	/// Input sockets for BackgroundNode
	/// </summary>
	public class BackgroundInputs : Inputs
	{
		/// <summary>
		/// BackgroundNode Color input socket
		/// </summary>
		public ColorSocket Color { get; set; }
		/// <summary>
		/// BackgroundNode Strength input socket. Default value 1.0f
		/// </summary>
		public FloatSocket Strength { get; set; }

		internal BackgroundInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Strength = new FloatSocket(parentNode, "Strength", "strength");
			AddSocket(Strength);
		}
	}

	/// <summary>
	/// Output socket for BackgroundNode
	/// </summary>
	public class BackgroundOutputs : Outputs
	{
		/// <summary>
		/// BackgroundNode output socket, can go on closure sockets.
		/// </summary>
		public ClosureSocket Background { get; set; }

		internal BackgroundOutputs(ShaderNode parentNode)
		{
			Background = new ClosureSocket(parentNode, "Background", "background");
			AddSocket(Background);
		}
	}

	/// <summary>
	/// Background shader node. Used in world/background shaders
	/// </summary>
	[ShaderNode("background_shader")]
	public class BackgroundNode : ShaderNode
	{
		/// <summary>
		/// Input sockets for background node
		/// </summary>
		public BackgroundInputs ins => (BackgroundInputs)inputs;

		/// <summary>
		/// Output sockets for background node
		/// </summary>
		public BackgroundOutputs outs => (BackgroundOutputs)outputs;

		/// <summary>
		/// Create a new background/world shader node
		/// </summary>
		public BackgroundNode(Shader shader) :
			this(shader, "a background")
		{
		}

		/// <summary>
		/// Create a new background/world shader node with given name
		/// </summary>
		/// <param name="name">UiName for shader node</param>
		public BackgroundNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal BackgroundNode(Shader shader, IntPtr shaderNodePtr) : base(shader, shaderNodePtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new BackgroundInputs(this);
			outputs = new BackgroundOutputs(this);
			ins.Color.Value = new float4();
			ins.Strength.Value = 1.0f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Color, xmlNode.GetAttribute("color"));
			Utilities.Instance.get_float(ins.Strength, xmlNode.GetAttribute("strength"));
		}
	}
}
