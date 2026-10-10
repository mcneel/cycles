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
	public class LayerWeightInputs : Inputs
	{
		public FloatSocket Blend { get; set; }

		public LayerWeightInputs(ShaderNode parentNode)
		{
			Blend = new FloatSocket(parentNode, "Blend", "blend");
			AddSocket(Blend);
		}
	}

	public class LayerWeightOutputs : Outputs
	{
		public FloatSocket Fresnel { get; set; }
		public FloatSocket Facing { get; set; }

		public LayerWeightOutputs(ShaderNode parentNode)
		{
			Fresnel = new FloatSocket(parentNode, "Fresnel", "fresnel");
			AddSocket(Fresnel);
			Facing = new FloatSocket(parentNode, "Facing", "facing");
			AddSocket(Facing);
		}
	}

	[ShaderNode("layer_weight")]
	public class LayerWeightNode : ShaderNode
	{
		public LayerWeightInputs ins => (LayerWeightInputs)inputs;
		public LayerWeightOutputs outs => (LayerWeightOutputs)outputs;

		public LayerWeightNode(Shader shader) : this(shader, "a layerweight node") { }
		public LayerWeightNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal LayerWeightNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new LayerWeightInputs(this);
			outputs = new LayerWeightOutputs(this);

			ins.Blend.Value = 0.5f;
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float(ins.Blend, xmlNode.GetAttribute("blend"));
		}
	}
}
