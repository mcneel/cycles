/**
C
opyright 2014-2024 Robert McNeel and Associates

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

/**
 * \defgroup cclshadernodes CSycles Shader Nodes
 */

/**
 * \ingroup cclshadernodes
 *  This pacakage contains the high-level <c>ccl.ShaderNode</c>s to be used in a <c>ccl.Shader</c>.
 *
 *  The classes give a clear and concise way of setting up <c>ccl.ShaderNode</c>s with a minimum of
 *  fuss.
 */

namespace ccl.ShaderNodes
{
  /// <summary>
  /// AddClosure node input sockets
  /// </summary>
  public class AddClosureInputs : Inputs
	{
		/// <summary>
		/// AddClosure input socket
		/// </summary>
		public ClosureSocket Closure1 { get; set; }
		/// <summary>
		/// AddClosure input socket
		/// </summary>
		public ClosureSocket Closure2 { get; set; }

		internal AddClosureInputs(ShaderNode parentNode)
		{
			Closure1 = new ClosureSocket(parentNode, "Closure1", "closure1");
			AddSocket(Closure1);
			Closure2 = new ClosureSocket(parentNode, "Closure2", "closure2");
			AddSocket(Closure2);
		}
	}

	/// <summary>
	/// AddClosure node output sockets
	/// </summary>
	public class AddClosureOutputs : Outputs
	{
		/// <summary>
		/// AddClosure output socket
		/// </summary>
		public ClosureSocket Closure { get; set; }

		internal AddClosureOutputs(ShaderNode parentNode)
		{
			Closure = new ClosureSocket(parentNode, "Closure", "closure");
			AddSocket(Closure);
		}
	}

	/// <summary>
	/// An Add closure.
	/// This closure takes two inputs, <c>Closure1</c> and <c>Closure2</c>. These
	/// will be simply added together.
	///
	/// There is one output <c>Closure</c>
	/// </summary>
	[ShaderNode("add_closure")]
	public class AddClosureNode : ShaderNode
	{
		/// <summary>
		/// AddClosure input sockets
		/// </summary>
		public AddClosureInputs ins => (AddClosureInputs) inputs;

		/// <summary>
		/// AddClosure output sockets
		/// </summary>
		public AddClosureOutputs outs => (AddClosureOutputs) outputs;

		/// <summary>
		/// Create a new Add closure.
		/// </summary>
		public AddClosureNode(Shader shader) : this(shader, "An add closures node")
		{
		}

		/// <summary>
		/// Create a new Add closure with name
		/// </summary>
		/// <param name="name"></param>
		public AddClosureNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal AddClosureNode(Shader shader, IntPtr shaderNodePtr) : base(shader, shaderNodePtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new AddClosureInputs(this);
			outputs = new AddClosureOutputs(this);
		}

		public override ClosureSocket GetClosureSocket()
		{
			return outs.Closure;
		}
	}
}
