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
	/// MixClosureNode input sockets
	/// </summary>
	public class MixClosureInputs : Inputs
	{
		/// <summary>
		/// MixClosureNode Fac input socket (0.0 - 1.0f).
		///
		/// 0.0f means Closure1 only, 1.0f means Closure2 only
		/// </summary>
		public FloatSocket Fac { get; set; }
		/// <summary>
		/// MixClosureNode Closure 1 input socket
		/// </summary>
		public ClosureSocket Closure1 { get; set; }
		/// <summary>
		/// MixClosureNode Closure 2 input socket
		/// </summary>
		public ClosureSocket Closure2 { get; set; }

		internal MixClosureInputs(ShaderNode parentNode)
		{
			Closure1 = new ClosureSocket(parentNode, "Closure1", "closure1");
			AddSocket(Closure1);
			Closure2 = new ClosureSocket(parentNode, "Closure2", "closure2");
			AddSocket(Closure2);
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
		}
	}

	/// <summary>
	/// MixClosureNode output sockets
	/// </summary>
	public class MixClosureOutputs : Outputs
	{
		/// <summary>
		/// The resulting shader output based on Closure1,Closure2 and the mix ratio Fac
		/// </summary>
		public ClosureSocket Closure { get; set; }

		internal MixClosureOutputs(ShaderNode parentNode)
		{
			Closure = new ClosureSocket(parentNode, "Closure", "closure");
			AddSocket(Closure);
		}
	}

	/// <summary>
	/// Mix shaders (closures). Mix ration is controlled by a Fac input
	/// </summary>
	[ShaderNode("mix_closure")]
	public class MixClosureNode : ShaderNode
	{
		/// <summary>
		/// MixClosureNode input sockets
		/// </summary>
		public MixClosureInputs ins => (MixClosureInputs)inputs;

		/// <summary>
		/// MixClosureNode output sockets
		/// </summary>
		public MixClosureOutputs outs => (MixClosureOutputs)outputs;

		/// <summary>
		/// Create MixClosureNode. Fac input is by default 0.5f
		/// </summary>
		public MixClosureNode(Shader shader) :
			this(shader, "a mix closure")
		{

		}

		public MixClosureNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal MixClosureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new MixClosureInputs(this);
			outputs = new MixClosureOutputs(this);
			ins.Fac.Value = 0.5f;
		}

		public override ClosureSocket GetClosureSocket()
		{
			return outs.Closure;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.Fac, xmlNode.GetAttribute("fac"));
		}
	}
}
