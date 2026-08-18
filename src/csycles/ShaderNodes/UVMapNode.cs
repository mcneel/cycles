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
	public class UvMapInputs : Inputs
	{
		StringSocket Attribute { get; set; }

		internal UvMapInputs(ShaderNode parent)
		{
			Attribute = new StringSocket(parent, "Attribute", "attribute");
		}
	}

	public class UvMapOutputs : Outputs
	{
		public FloatSocket UV { get; set; }

		internal UvMapOutputs(ShaderNode parentNode)
		{
			UV = new FloatSocket(parentNode, "UV", "UV");
			AddSocket(UV);
		}
	}

	[ShaderNode("uvmap")]
	public class UvMapNode : ShaderNode
	{

		public UvMapInputs ins => (UvMapInputs)inputs;
		public UvMapOutputs outs => (UvMapOutputs)outputs;
		public UvMapNode(Shader shader) : this(shader, "a uvmap node") { }
		public UvMapNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal UvMapNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new UvMapInputs(this);
			outputs = new UvMapOutputs(this);
		}
	}
}
