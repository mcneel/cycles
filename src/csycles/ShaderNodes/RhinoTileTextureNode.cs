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
	public class TileTextureInputs : TwoColorInputs
	{
		public VectorSocket UVW { get; set; }

		public TileTextureInputs(ShaderNode parentNode) : base(parentNode)
		{
			UVW = new VectorSocket(parentNode, "UVW", "uvw");
			AddSocket(UVW);
		}
	}

	public class TileTextureOutputs : TwoColorOutputs
	{
		public TileTextureOutputs(ShaderNode parentNode) : base(parentNode)
		{
		}
	}

	[ShaderNode("rhino_tile_texture")]
	public class TileTextureProceduralNode : ShaderNode
	{
		public enum TileTypes
		{
			RECTANGULAR_3D,
			RECTANGULAR_2D,
			HEXAGONAL_2D,
			TRIANGULAR_2D,
			OCTAGONAL_2D,
		};

		public TileTextureInputs ins => (TileTextureInputs)inputs;
		public TileTextureOutputs outs => (TileTextureOutputs)outputs;

		public TileTypes TileType { get; set; }
		public float PhaseX { get; set; }
		public float PhaseY { get; set; }
		public float PhaseZ { get; set; }
		public float JoinWidthX { get; set; }
		public float JoinWidthY { get; set; }
		public float JoinWidthZ { get; set; }

		public TileTextureProceduralNode(Shader shader) : this(shader, "a tile texture") { }
		public TileTextureProceduralNode(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal TileTextureProceduralNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new TileTextureInputs(this);
			outputs = new TileTextureOutputs(this);
		}

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_int(Id, "Type", (int)TileType);
			CSycles.shadernode_set_member_vec(Id, "Phase", PhaseX, PhaseY, PhaseZ);
			CSycles.shadernode_set_member_vec(Id, "JoinWidth", JoinWidthX, JoinWidthY, JoinWidthZ);
		}
	}
}
