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
	/// <summary>
	/// BumpNode input sockets
	/// </summary>
	public class BumpInputs : Inputs
	{
		/// <summary>
		/// BumpNode height of the bump
		/// </summary>
		public FloatSocket Height { get; set; }
		/// <summary>
		/// BumpNode input normal. If not connected will use default shading normal
		/// </summary>
		public VectorSocket Normal { get; set; }
		/// <summary>
		/// BumpNode strength of the bump effect
		/// </summary>
		public FloatSocket Strength { get; set; }
		/// <summary>
		/// BumpNode distance
		/// </summary>
		public FloatSocket Distance { get; set; }
		public BoolSocket Invert { get; set; }
		public BoolSocket UseObjectSpace { get; set; }
		public FloatSocket SampleX { get; set; }
		public FloatSocket SampleY { get; set; }
		public FloatSocket SampleCenter { get; set; }

		internal BumpInputs(ShaderNode parentNode)
		{
			Invert = new BoolSocket(parentNode, "Invert", "invert");
			AddSocket(Invert);
			UseObjectSpace = new BoolSocket(parentNode, "UseObjectSpace", "use_object_space");
			AddSocket(UseObjectSpace);
			Height = new FloatSocket(parentNode, "Height", "height");
			AddSocket(Height);
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
			Strength = new FloatSocket(parentNode, "Strength", "strength");
			AddSocket(Strength);
			Distance = new FloatSocket(parentNode, "Distance", "distance");
			AddSocket(Distance);
			SampleCenter = new FloatSocket(parentNode, "SampleCenter", "sample_center");
			AddSocket(SampleCenter);
			SampleX = new FloatSocket(parentNode, "SampleX", "sample_x");
			AddSocket(SampleX);
			SampleY = new FloatSocket(parentNode, "SampleY", "sample_y");
			AddSocket(SampleY);
		}
	}

	/// <summary>
	/// BumpNode output sockets
	/// </summary>
	public class BumpOutputs : Outputs
	{
		/// <summary>
		/// BumpNode new Normal
		/// </summary>
		public VectorSocket Normal { get; set; }

		internal BumpOutputs(ShaderNode parentNode)
		{
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			AddSocket(Normal);
		}
	}

	/// <summary>
	/// BumpNode
	/// </summary>
	[ShaderNode("bump")]
	public class BumpNode : ShaderNode
	{
		/// <summary>
		/// BumpNode input sockets
		/// </summary>
		public BumpInputs ins => (BumpInputs)inputs;

		/// <summary>
		/// BumpNode output sockets
		/// </summary>
		public BumpOutputs outs => (BumpOutputs)outputs;

		/// <summary>
		/// Create new BumpNode with blend type Bump.
		/// </summary>
		public BumpNode(Shader shader) : this(shader, "a bump node") { }
		public BumpNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal BumpNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new BumpInputs(this);
			outputs = new BumpOutputs(this);

			Invert = false;
			ins.Strength.Value = 1.0f;
			ins.Distance.Value = 0.15f;
		}

		/// <summary>
		/// BumpNode set to true to invert result
		/// </summary>
		public bool Invert { get; set; }

		internal override void SetDirectMembers()
		{
			CSycles.shadernode_set_member_bool(Id, "invert", Invert);
		}

		internal override void ParseXml(System.Xml.XmlReader xmlNode)
		{
			bool invert = false;
			Utilities.Instance.get_bool(ref invert, xmlNode.GetAttribute("invert"));
			Invert = invert;

			Utilities.Instance.get_float(ins.Strength, xmlNode.GetAttribute("strength"));
			Utilities.Instance.get_float(ins.Distance, xmlNode.GetAttribute("distance"));

		}
	}
}
