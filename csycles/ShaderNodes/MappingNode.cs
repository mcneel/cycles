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
using System.Xml;

namespace ccl.ShaderNodes
{
	/// <summary>
	/// MappingNode input sockets
	/// </summary>
	public class MappingInputs : Inputs
	{
		/// <summary>
		/// MappingNode input vector that should be transformed
		/// </summary>
		public VectorSocket Vector { get; set; }
		public VectorSocket Location { get; set; }
		public VectorSocket Rotation { get; set; }
		public VectorSocket Scale { get; set; }

		internal MappingInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
			Location = new VectorSocket(parentNode, "Location", "location");
			AddSocket(Location);
			Rotation = new VectorSocket(parentNode, "Rotation", "rotation");
			AddSocket(Rotation);
			Scale = new VectorSocket(parentNode, "Scale", "scale");
			AddSocket(Scale);
		}
	}

	/// <summary>
	/// MappingNode output sockets
	/// </summary>
	public class MappingOutputs : Outputs
	{
		/// <summary>
		/// MappingNode output vector
		/// </summary>
		public VectorSocket Vector { get; set; }

		internal MappingOutputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
		}
	}

	/// <summary>
	/// Mapping node to transform an input vector utilising one of the four
	/// types
	/// - Texture: Transform a texture by inverse mapping the texture coordinate
	/// - Point: Transform a point
	/// - Vector: Transform a direction vector
	/// - Normal: Transform a normal vector with unit length
	/// </summary>
	[ShaderNode("mapping")]
	public class MappingNode : TextureNode
	{
		/// <summary>
		/// MappingNode input sockets
		/// </summary>
		public MappingInputs ins => (MappingInputs)inputs;

		/// <summary>
		/// MappingNode output sockets
		/// </summary>
		public MappingOutputs outs => (MappingOutputs)outputs;


		/// <summary>
		/// Create new MappingNode
		/// </summary>
		public MappingNode(Shader shader) : this(shader, "a mapping node")
		{
		}

		public MappingNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal MappingNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new MappingInputs(this);
			outputs = new MappingOutputs(this);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
			var mapping_type = xmlNode.GetAttribute("mapping_type");
			if (!string.IsNullOrEmpty(mapping_type))
			{
				try
				{
					var mt = (MappingType)Enum.Parse(typeof(MappingType), mapping_type, true);
					Mapping = mt;
				}
				catch (ArgumentException)
				{
					Mapping = MappingType.Texture;
				}
			}
			var f4 = new float4(0.0f);
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("rotation"));
			Rotation = f4;
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("translation"));
			Translation = f4;
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("scale"));
			Scale = f4;
			bool b = false;
			Utilities.Instance.get_bool(ref b, xmlNode.GetAttribute("useminmax"));
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("min"));
			if (b && !f4.IsZero(false))
			{
				Min = f4;
			}
			Utilities.Instance.get_float4(f4, xmlNode.GetAttribute("max"));
			if (b && !f4.IsZero(false))
			{
				Max = f4;
			}

		}
	}
}
