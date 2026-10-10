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

namespace ccl.ShaderNodes
{
	/// <summary>
	/// OutputNode input sockets
	/// </summary>
	public class OutputInputs : Inputs
	{
		/// <summary>
		/// Surface shading socket. Plug Background here for world shaders
		/// </summary>
		public ClosureSocket Surface { get; set; }
		public ClosureSocket Volume { get; set; }
		/// <summary>
		/// Only useful for material output nodes
		/// </summary>
		public Float4Socket Displacement { get; set; }
		public Float4Socket Normal { get; set; }

		internal OutputInputs(ShaderNode parentNode)
		{
			Surface = new ClosureSocket(parentNode, "Surface", "surface");
			AddSocket(Surface);
			Volume = new ClosureSocket(parentNode, "Volume", "volume");
			AddSocket(Volume);
			Displacement = new Float4Socket(parentNode, "Displacement", "displacement");
			AddSocket(Displacement);
			Normal = new Float4Socket(parentNode, "Normal", "normal");
			AddSocket(Normal);
		}
	}

	public class OutputOutputs : Outputs
	{
		internal OutputOutputs(ShaderNode parentNode)
		{

		}
	}

	/// <summary>
	/// The final output shader node for shaders.
	/// </summary>
	[ShaderNode("output")]
	public class OutputNode : ShaderNode
	{
		public override string VariableName => "output";

		public OutputInputs ins => (OutputInputs)inputs;
		public OutputOutputs outs => (OutputOutputs)outputs;

		public OutputNode(Shader shader) : this(shader, "output") { }
		public OutputNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal OutputNode(Shader shader, IntPtr shaderNodePtr) : base(shader, shaderNodePtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new OutputInputs(this);
			outputs = new OutputOutputs(this);
		}
	}

}
