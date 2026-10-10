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
	public class FresnelInputs : Inputs
	{
		public FloatSocket IOR { get; set; }

		internal FresnelInputs(ShaderNode parentNode)
		{
			IOR = new FloatSocket(parentNode, "IOR", "IOR");
			AddSocket(IOR);
		}
	}

	public class FresnelOutputs : Outputs
	{
		public FloatSocket Fac { get; set; }

		internal FresnelOutputs(ShaderNode parentNode)
		{
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
		}
	}

	[ShaderNode("fresnel")]
	public class FresnelNode : ShaderNode
	{
		public FresnelInputs ins => (FresnelInputs)inputs;
		public FresnelOutputs outs => (FresnelOutputs)outputs;
		public FresnelNode(Shader shader) : this(shader, "a fresnel input node") { }
		public FresnelNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal FresnelNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new FresnelInputs(this);
			outputs = new FresnelOutputs(this);
			ins.IOR.Value = 1.45f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.IOR, xmlNode.GetAttribute("ior"));
		}
	}
}
