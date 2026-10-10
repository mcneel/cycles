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
	public class LightFalloffOutputs : Outputs
	{
		public FloatSocket Quadratic { get; set; }
		public FloatSocket Linear { get; set; }
		public FloatSocket Constant { get; set; }

		public LightFalloffOutputs(ShaderNode parentNode)
		{
			Quadratic = new FloatSocket(parentNode, "Quadratic", "quadratic");
			AddSocket(Quadratic);
			Linear = new FloatSocket(parentNode, "Linear", "linear");
			AddSocket(Linear);
			Constant = new FloatSocket(parentNode, "Constant", "constant");
			AddSocket(Constant);
		}
	}

	public class LightFalloffInputs : Inputs
	{
		public FloatSocket Strength { get; set; }
		public FloatSocket Smooth { get; set; }
		public LightFalloffInputs(ShaderNode parentNode)
		{
			Strength = new FloatSocket(parentNode, "Strength", "strength");
			AddSocket(Strength);
			Smooth = new FloatSocket(parentNode, "Smooth", "smooth");
			AddSocket(Smooth);
		}
	}

	[ShaderNode("light_falloff")]
	public class LightFalloffNode : ShaderNode
	{
		public LightFalloffInputs ins => (LightFalloffInputs)inputs;
		public LightFalloffOutputs outs => (LightFalloffOutputs)outputs;

		public LightFalloffNode(Shader shader) : this(shader, "a light fall-off node") { }
		public LightFalloffNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal LightFalloffNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new LightFalloffInputs(this);
			outputs = new LightFalloffOutputs(this);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.Smooth, xmlNode.GetAttribute("smooth"));
			Utilities.Instance.get_float(ins.Strength, xmlNode.GetAttribute("strength"));
		}
	}
}
