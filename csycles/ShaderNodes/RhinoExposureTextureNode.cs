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
	public class ExposureTextureInputs : Inputs
	{
		public ColorSocket Color { get; set; }

		public ExposureTextureInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	public class ExposureTextureOutputs : TwoColorOutputs
	{

		public ExposureTextureOutputs(ShaderNode parentNode) : base(parentNode)
		{
		}
	}

	[ShaderNode("rhino_exposure_texture")]
	public class ExposureTextureProceduralNode : ShaderNode
	{
		public ExposureTextureInputs ins => (ExposureTextureInputs)inputs;
		public ExposureTextureOutputs outs => (ExposureTextureOutputs)outputs;

		public float Exposure { get; set; }
		public float Multiplier { get; set; }
		public float WorldLuminance { get; set; }
		public float MaxLuminance { get; set; }

		public ExposureTextureProceduralNode(Shader shader) : this(shader, "an exposure texture") { }
		public ExposureTextureProceduralNode(Shader shader, string name) : base(shader, name)
		{
			FinalizeConstructor();
		}

		internal ExposureTextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new ExposureTextureInputs(this);
			outputs = new ExposureTextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_float(Id, "Exposure", Exposure);
			CSycles.shadernode_set_member_float(Id, "Multiplier", Multiplier);
			CSycles.shadernode_set_member_float(Id, "WorldLuminance", WorldLuminance);
			CSycles.shadernode_set_member_float(Id, "MaxLuminance", MaxLuminance);
		}
	}
}
