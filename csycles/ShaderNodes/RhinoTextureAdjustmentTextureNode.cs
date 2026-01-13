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
	public class TextureAdjustmentTextureInputs : Inputs
	{
		public ColorSocket Color { get; set; }

		public TextureAdjustmentTextureInputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
		}
	}

	public class TextureAdjustmentTextureOutputs : Outputs
	{
		public ColorSocket Color { get; set; }

		public TextureAdjustmentTextureOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "out_color");
			AddSocket(Color);
		}
	}

	[ShaderNode("rhino_texture_adjustment_texture")]
	public class TextureAdjustmentTextureProceduralNode : ShaderNode
	{
		public TextureAdjustmentTextureInputs ins => (TextureAdjustmentTextureInputs)inputs;
		public TextureAdjustmentTextureOutputs outs => (TextureAdjustmentTextureOutputs)outputs;

		public bool Grayscale { get; set; }
		public bool Invert { get; set; }
		public bool Clamp { get; set; }
		public bool ScaleToClamp { get; set; }
		public float Multiplier { get; set; }
		public float ClampMin { get; set; }
		public float ClampMax { get; set; }
		public float Gain { get; set; }
		public float Gamma { get; set; }
		public float Saturation { get; set; }
		public float HueShift { get; set; }
		public bool IsHdr { get; set; }

		public TextureAdjustmentTextureProceduralNode(Shader shader) : this(shader, "a texture adjustment texture") { }
		public TextureAdjustmentTextureProceduralNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal TextureAdjustmentTextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new TextureAdjustmentTextureInputs(this);
			outputs = new TextureAdjustmentTextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_bool(Id, "Grayscale", Grayscale);
			CSycles.shadernode_set_member_bool(Id, "Invert", Invert);
			CSycles.shadernode_set_member_bool(Id, "Clamp", Clamp);
			CSycles.shadernode_set_member_bool(Id, "ScaleToClamp", ScaleToClamp);
			CSycles.shadernode_set_member_float(Id, "Multiplier", Multiplier);
			CSycles.shadernode_set_member_float(Id, "ClampMin", ClampMin);
			CSycles.shadernode_set_member_float(Id, "ClampMax", ClampMax);
			CSycles.shadernode_set_member_float(Id, "Gain", Gain);
			CSycles.shadernode_set_member_float(Id, "Gamma", Gamma);
			CSycles.shadernode_set_member_float(Id, "Saturation", Saturation);
			CSycles.shadernode_set_member_float(Id, "HueShift", HueShift);
			CSycles.shadernode_set_member_bool(Id, "IsHdr", IsHdr);
		}
	}
}
