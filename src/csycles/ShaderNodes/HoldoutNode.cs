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
	public class HoldoutInputs : Inputs
	{
		public HoldoutInputs(ShaderNode parentNode)
		{
		}
	}

	public class HoldoutOutputs : Outputs
	{
		public ClosureSocket Holdout { get; set; }

		public HoldoutOutputs(ShaderNode parentNode)
		{
			Holdout = new ClosureSocket(parentNode, "Holdout", "holdout");
			AddSocket(Holdout);
		}
	}

	[ShaderNode("holdout")]
	public class HoldoutNode : ShaderNode
	{
		public HoldoutInputs ins => (HoldoutInputs)inputs;
		public HoldoutOutputs outs => (HoldoutOutputs)outputs;

		public HoldoutNode(Shader shader) : this(shader, "a holdout node") { }

		public HoldoutNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal HoldoutNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new HoldoutInputs(this);
			outputs = new HoldoutOutputs(this);
		}
	}
}
