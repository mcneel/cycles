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
	public class GridTextureInputs : TwoColorInputs
	{
		public VectorSocket UVW { get; set; }

		public GridTextureInputs(ShaderNode parentNode) : base(parentNode)
		{
			UVW = new VectorSocket(parentNode, "UVW", "uvw");
			AddSocket(UVW);
		}
	}

	public class GridTextureOutputs : TwoColorOutputs
	{

		public GridTextureOutputs(ShaderNode parentNode) : base(parentNode)
		{
		}
	}

	[ShaderNode("rhino_grid_texture")]
	public class GridTextureProceduralNode : ShaderNode
	{
		public GridTextureInputs ins => (GridTextureInputs)inputs;
		public GridTextureOutputs outs => (GridTextureOutputs)outputs;

		public int Cells { get; set; }
		public float FontThickness { get; set; }

		public GridTextureProceduralNode(Shader shader) : this(shader, "a grid texture") { }
		public GridTextureProceduralNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal GridTextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new GridTextureInputs(this);
			outputs = new GridTextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_int(Id, "Cells", Cells);
			CSycles.shadernode_set_member_float(Id, "FontThickness", FontThickness);
		}
	}
}
