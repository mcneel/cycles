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
	public class PerturbingPart1TextureInputs : Inputs
	{
		public VectorSocket UVW { get; set; }

		public PerturbingPart1TextureInputs(ShaderNode parentNode)
		{
			UVW = new VectorSocket(parentNode, "UVW", "uvw");
			AddSocket(UVW);
		}
	}

	public class PerturbingPart1TextureOutputs : Outputs
	{
		public VectorSocket UVW1 { get; set; }
		public VectorSocket UVW2 { get; set; }
		public VectorSocket UVW3 { get; set; }

		public PerturbingPart1TextureOutputs(ShaderNode parentNode)
		{
			UVW1 = new VectorSocket(parentNode, "UVW1", "out_uvw1");
			AddSocket(UVW1);
			UVW2 = new VectorSocket(parentNode, "UVW2", "out_uvw2");
			AddSocket(UVW2);
			UVW3 = new VectorSocket(parentNode, "UVW3", "out_uvw3");
			AddSocket(UVW3);
		}
	}

	[ShaderNode("rhino_perturbing_part1_texture")]
	public class PerturbingPart1TextureProceduralNode : ShaderNode
	{
		public PerturbingPart1TextureInputs ins => (PerturbingPart1TextureInputs)inputs;
		public PerturbingPart1TextureOutputs outs => (PerturbingPart1TextureOutputs)outputs;

		public PerturbingPart1TextureProceduralNode(Shader shader) : this(shader, "a pertrubing part1 texture") { }
		public PerturbingPart1TextureProceduralNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal PerturbingPart1TextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new PerturbingPart1TextureInputs(this);
			outputs = new PerturbingPart1TextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
		}
	}

	public class PerturbingPart2TextureInputs : Inputs
	{
		public VectorSocket UVW { get; set; }
		public ColorSocket Color1 { get; set; }
		public ColorSocket Color2 { get; set; }
		public ColorSocket Color3 { get; set; }

		public PerturbingPart2TextureInputs(ShaderNode parentNode)
		{
			UVW = new VectorSocket(parentNode, "UVW", "uvw");
			AddSocket(UVW);
			Color1 = new ColorSocket(parentNode, "Color1", "color1");
			AddSocket(Color1);
			Color2 = new ColorSocket(parentNode, "Color2", "color2");
			AddSocket(Color2);
			Color3 = new ColorSocket(parentNode, "Color3", "color3");
			AddSocket(Color3);
		}
	}

	public class PerturbingPart2TextureOutputs : Outputs
	{
		public VectorSocket PerturbedUVW { get; set; }

		public PerturbingPart2TextureOutputs(ShaderNode parentNode)
		{
			PerturbedUVW = new VectorSocket(parentNode, "Perturbed UVW", "out_uvw");
			AddSocket(PerturbedUVW);
		}
	}

	[ShaderNode("rhino_perturbing_part2_texture")]
	public class PerturbingPart2TextureProceduralNode : ShaderNode
	{
		public PerturbingPart2TextureInputs ins => (PerturbingPart2TextureInputs)inputs;
		public PerturbingPart2TextureOutputs outs => (PerturbingPart2TextureOutputs)outputs;

		public float Amount { get; set; } = 0.1f;

		public PerturbingPart2TextureProceduralNode(Shader shader) : this(shader, "a pertrubing part2 texture") { }
		public PerturbingPart2TextureProceduralNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal PerturbingPart2TextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new PerturbingPart2TextureInputs(this);
			outputs = new PerturbingPart2TextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_float(Id, "Amount", Amount);
		}
	}
}
