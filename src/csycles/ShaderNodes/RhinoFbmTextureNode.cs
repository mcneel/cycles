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
	public class FbmTextureInputs : TwoColorInputs
	{
		public VectorSocket UVW { get; set; }

		public FbmTextureInputs(ShaderNode parentNode) : base(parentNode)
		{
			UVW = new VectorSocket(parentNode, "UVW", "uvw");
			AddSocket(UVW);
		}
	}

	public class FbmTextureOutputs : TwoColorOutputs
	{
		public FbmTextureOutputs(ShaderNode parentNode) : base(parentNode)
		{
		}
	}

	[ShaderNode("rhino_fbm_texture")]
	public class FbmTextureProceduralNode : ShaderNode
	{
		public FbmTextureInputs ins => (FbmTextureInputs)inputs;
		public FbmTextureOutputs outs => (FbmTextureOutputs)outputs;

		public bool IsTurbulent { get; set; }
		public int MaxOctaves { get; set; }
		public float Gain { get; set; }
		public float Roughness { get; set; }

		public FbmTextureProceduralNode(Shader shader) : this(shader, "an fbm texture") { }
		public FbmTextureProceduralNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal FbmTextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new FbmTextureInputs(this);
			outputs = new FbmTextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_bool(Id, "IsTurbulent", IsTurbulent);
			CSycles.shadernode_set_member_int(Id, "MaxOctaves", MaxOctaves);
			CSycles.shadernode_set_member_float(Id, "Gain", Gain);
			CSycles.shadernode_set_member_float(Id, "Roughness", Roughness);
		}
	}
}
