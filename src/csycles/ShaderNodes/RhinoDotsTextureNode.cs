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
	public class DotsTextureInputs : Inputs
	{
		public VectorSocket UVW { get; set; }
		public ColorSocket Color1 { get; set; }
		public ColorSocket Color2 { get; set; }

		public DotsTextureInputs(ShaderNode parentNode)
		{
			UVW = new VectorSocket(parentNode, "UVW", "uvw");
			AddSocket(UVW);
			Color1 = new ColorSocket(parentNode, "Color1", "color1");
			AddSocket(Color1);
			Color2 = new ColorSocket(parentNode, "Color2", "color2");
			AddSocket(Color2);
		}
	}

	public class DotsTextureOutputs : Outputs
	{
		public ColorSocket Color { get; set; }

		public DotsTextureOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "out_color");
			AddSocket(Color);
		}
	}

	[ShaderNode("rhino_dots_texture")]
	public class DotsTextureProceduralNode : ShaderNode
	{
		public enum FalloffTypes
		{
			FLAT,
			LINEAR,
			CUBIC,
			ELLIPTIC,
		};

		public enum CompositionTypes
		{
			MAXIMUM,
			ADDITION,
			SUBTRACTION,
			MULTIPLICATION,
			AVERAGE,
			STANDARD,
		};

		public DotsTextureInputs ins => (DotsTextureInputs)inputs;
		public DotsTextureOutputs outs => (DotsTextureOutputs)outputs;

		public int DataCount { get; set; }
		public int TreeNodeCount { get; set; }
		public float SampleAreaSize { get; set; }
		public bool Rings { get; set; }
		public float RingRadius { get; set; }
		public FalloffTypes FalloffType { get; set; }
		public CompositionTypes CompositionType { get; set; }

		public DotsTextureProceduralNode(Shader shader) : this(shader, "a dots texture") { }
		public DotsTextureProceduralNode(Shader shader, string name) : base(shader, name)
		{
			FinalzeConstructor();
		}

		internal DotsTextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalzeConstructor();
		}

		private void FinalzeConstructor()
		{
			inputs = new DotsTextureInputs(this);
			outputs = new DotsTextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_int(Id, "DataCount", DataCount);
			CSycles.shadernode_set_member_int(Id, "TreeNodeCount", TreeNodeCount);
			CSycles.shadernode_set_member_float(Id, "SampleAreaSize", SampleAreaSize);
			CSycles.shadernode_set_member_bool(Id, "Rings", Rings);
			CSycles.shadernode_set_member_float(Id, "RingRadius", RingRadius);
			CSycles.shadernode_set_member_int(Id, "FalloffType", (int)FalloffType);
			CSycles.shadernode_set_member_int(Id, "CompositionType", (int)CompositionType);
		}
	}
}
