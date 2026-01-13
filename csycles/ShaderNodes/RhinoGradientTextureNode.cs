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
	public class GradientTextureInputs : TwoColorInputs
	{
		public VectorSocket UVW { get; set; }

		public GradientTextureInputs(ShaderNode parentNode) : base(parentNode)
		{
			UVW = new VectorSocket(parentNode, "UVW", "uvw");
			AddSocket(UVW);
		}
	}

	public class GradientTextureOutputs : TwoColorOutputs
	{
		public GradientTextureOutputs(ShaderNode parentNode) : base(parentNode)
		{
		}
	}

	[ShaderNode("rhino_gradient_texture")]
	public class GradientTextureProceduralNode : ShaderNode
	{
		public enum GradientTypes
		{
			LINEAR,
			BOX,
			RADIAL,
			TARTAN,
			SWEEP,
			PONG,
			SPIRAL,
		};

		public GradientTextureInputs ins => (GradientTextureInputs)inputs;
		public GradientTextureOutputs outs => (GradientTextureOutputs)outputs;

		public GradientTypes GradientType { get; set; }
		public bool FlipAlternate { get; set; }
		public bool UseCustomCurve { get; set; }
		public int PointWidth { get; set; }
		public int PointHeight { get; set; }

		public GradientTextureProceduralNode(Shader shader) : this(shader, "a gradient texture") { }
		public GradientTextureProceduralNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal GradientTextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new GradientTextureInputs(this);
			outputs = new GradientTextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_int(Id, "GradientType", (int)GradientType);
			CSycles.shadernode_set_member_bool(Id, "FlipAlternate", FlipAlternate);
			CSycles.shadernode_set_member_bool(Id, "UseCustomCurve", UseCustomCurve);
			//CSycles.shadernode_set_member_int(sessionId, shaderId, Id, Type, "PointWidth", PointWidth);
			//CSycles.shadernode_set_member_int(sessionId, shaderId, Id, Type, "PointHeight", PointHeight);
		}
	}
}
