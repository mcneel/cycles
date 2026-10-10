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
	public class MagicInputs : Inputs
	{
		public VectorSocket Vector { get; set; }
		public FloatSocket Scale { get; set; }
		public FloatSocket Distortion { get; set; }

		public MagicInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
			Scale = new FloatSocket(parentNode, "Scale", "scale");
			AddSocket(Scale);
			Distortion = new FloatSocket(parentNode, "Distortion", "distortion");
			AddSocket(Distortion);
		}
	}

	public class MagicOutputs : Outputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Fac { get; set; }

		public MagicOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
		}
	}

	[ShaderNode("magic_texture")]
	public class MagicTexture : TextureNode
	{
		public MagicInputs ins => (MagicInputs)inputs;
		public MagicOutputs outs => (MagicOutputs)outputs;

		public MagicTexture(Shader shader) : this(shader, "a magic texture") { }
		public MagicTexture(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal MagicTexture(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new MagicInputs(this);
			outputs = new MagicOutputs(this);

			ins.Scale.Value = 5.0f;
			ins.Distortion.Value = 1.0f;
			Depth = 2;
		}

		public int Depth { get; set; }

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_int(Id, "depth", Depth);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
			Utilities.Instance.get_float(ins.Scale, xmlNode.GetAttribute("scale"));
			Utilities.Instance.get_float(ins.Distortion, xmlNode.GetAttribute("distortion"));
			int depth = 2;
			if (Utilities.Instance.get_int(ref depth, xmlNode.GetAttribute("depth")))
			{
				Depth = depth;
			}
		}
	}
}
