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
	public class NoiseTextureInputs : TwoColorInputs
	{
		public VectorSocket UVW { get; set; }

		public NoiseTextureInputs(ShaderNode parentNode) : base(parentNode)
		{
			UVW = new VectorSocket(parentNode, "UVW", "uvw");
			AddSocket(UVW);
		}
	}

	public class NoiseTextureOutputs : TwoColorOutputs
	{
		public NoiseTextureOutputs(ShaderNode parentNode) : base(parentNode)
		{ }
	}

	[ShaderNode("rhino_noise_texture")]
	public class NoiseTextureProceduralNode : ShaderNode
	{
		public enum NoiseTypes
		{
			PERLIN,
			VALUE_NOISE,
			PERLIN_PLUS_VALUE,
			SIMPLEX,
			SPARSE_CONVOLUTION,
			LATTICE_CONVOLUTION,
			WARDS_HERMITE,
			AALTONEN,
		};

		public enum SpecSynthTypes
		{
			FRACTAL_SUM,
			TURBULENCE,
		}

		public NoiseTextureInputs ins => (NoiseTextureInputs)inputs;
		public NoiseTextureOutputs outs => (NoiseTextureOutputs)outputs;

		public NoiseTypes NoiseType { get; set; } = NoiseTypes.PERLIN;
		public SpecSynthTypes SpecSynthType { get; set; } = SpecSynthTypes.FRACTAL_SUM;
		public int OctaveCount { get; set; } = 3;
		public float FrequencyMultiplier { get; set; } = 2.0f;
		public float AmplitudeMultiplier { get; set; } = 0.5f;
		public float ClampMin { get; set; } = -1.0f;
		public float ClampMax { get; set; } = 1.0f;
		public bool ScaleToClamp { get; set; } = false;
		public bool Inverse { get; set; } = false;
		public float Gain { get; set; } = 0.5f;

		public NoiseTextureProceduralNode(Shader shader) : this(shader, "a noise texture") { }
		public NoiseTextureProceduralNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal NoiseTextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new NoiseTextureInputs(this);
			outputs = new NoiseTextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_int(Id, "NoiseType", (int)NoiseType);
			CSycles.shadernode_set_member_int(Id, "SpecSynthType", (int)SpecSynthType);
			CSycles.shadernode_set_member_int(Id, "OctaveCount", OctaveCount);
			CSycles.shadernode_set_member_float(Id, "FrequencyMultiplier", FrequencyMultiplier);
			CSycles.shadernode_set_member_float(Id, "AmplitudeMultiplier", AmplitudeMultiplier);
			CSycles.shadernode_set_member_float(Id, "ClampMin", ClampMin);
			CSycles.shadernode_set_member_float(Id, "ClampMax", ClampMax);
			CSycles.shadernode_set_member_bool(Id, "ScaleToClamp", ScaleToClamp);
			CSycles.shadernode_set_member_bool(Id, "Inverse", Inverse);
			CSycles.shadernode_set_member_float(Id, "Gain", Gain);
		}
	}
}
