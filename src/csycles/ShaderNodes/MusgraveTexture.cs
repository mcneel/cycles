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
using System.Text;
using System.Xml;

namespace ccl.ShaderNodes
{
	public class MusgraveInputs : Inputs
	{
		public VectorSocket Vector { get; set; }
		public FloatSocket Scale { get; set; }
		public FloatSocket Detail { get; set; }
		public FloatSocket Dimension { get; set; }
		public FloatSocket Lacunarity { get; set; }
		public FloatSocket Offset { get; set; }
		public FloatSocket Gain { get; set; }
		public FloatSocket W { get; set; }

		public MusgraveInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
			Scale = new FloatSocket(parentNode, "Scale", "scale");
			AddSocket(Scale);
			Detail = new FloatSocket(parentNode, "Detail", "detail");
			AddSocket(Detail);
			/* Noise has no fractal dimension; MusgraveTexture.SetSockets turns it into Roughness. */
			Dimension = new FloatSocket(parentNode, "Dimension", "dimension") { Retired = true };
			AddSocket(Dimension);
			Lacunarity = new FloatSocket(parentNode, "Lacunarity", "lacunarity");
			AddSocket(Lacunarity);
			Offset = new FloatSocket(parentNode, "Offset", "offset");
			AddSocket(Offset);
			Gain = new FloatSocket(parentNode, "Gain", "gain");
			AddSocket(Gain);
			W = new FloatSocket(parentNode, "W", "w");
			AddSocket(W);
		}
	}

	public class MusgraveOutputs : Outputs
	{
		public FloatSocket Fac { get; set; }

		public MusgraveOutputs(ShaderNode parentNode)
		{
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
		}
	}

	[ShaderNode("musgrave_texture")]
	public class MusgraveTexture : TextureNode
	{
		public enum Dimensions
		{
			D1 = 1,
			D2,
			D3,
			D4,

		}
		public enum MusgraveTypes
		{
			Multifractal,
			fBM,
			Hybrid_Multifractal,
			Ridged_Multifractal,
			Hetero_Terrain,
		}

		public MusgraveInputs ins => (MusgraveInputs)inputs;
		public MusgraveOutputs outs => (MusgraveOutputs)outputs;

		/* Cycles 4.1 folded musgrave into noise, whose fractal types are musgrave's loops
		 * reparametrised; SetSockets converts (noise runs detail + 1 octaves, roughness is
		 * lacunarity^-dimension, Normalize off). The attribute keeps the old name as the XML
		 * key, see SeparateRgbNode. */
		public override string ShaderNodeTypeName => "noise_texture";

		public MusgraveTexture(Shader shader) : this(shader, "a musgrave texture") { }
		public MusgraveTexture(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal MusgraveTexture(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new MusgraveInputs(this);
			outputs = new MusgraveOutputs(this);

			MusgraveType = MusgraveTypes.fBM;
			Dimension = Dimensions.D3;

			ins.Scale.Value = 1.0f;
			ins.Detail.Value = 2.0f;
			ins.Dimension.Value = 2.0f;
			ins.Lacunarity.Value = 2.0f;
		}

		/// <summary>
		/// musgrave->type
		/// </summary>
		public MusgraveTypes MusgraveType { get; set; }

		public Dimensions Dimension { get; set; }
		public void SetType(string dist)
		{
			dist = dist.Replace(" ", "_");
			MusgraveType = (MusgraveTypes)System.Enum.Parse(typeof(MusgraveTypes), dist, true);
		}

		public void SetDimension(string dim)
		{
			Dimension = (Dimensions)System.Enum.Parse(typeof(Dimensions), dim, true);
		}

		private string MusgraveToString(MusgraveTypes dist)
		{
			var str = dist.ToString();
			str = str.Replace("_", " ");
			return str;
		}

		internal override void SetEnums()
		{
			/* MusgraveTypes has NodeNoiseType's order, and both count dimensions from 1. */
			CSycles.shadernode_set_enum(Id, "type", (int)MusgraveType);
			CSycles.shadernode_set_enum(Id, "dimensions", (int)Dimension);
		}

		internal override void SetSockets()
		{
			base.SetSockets();
			CSycles.shadernode_set_attribute_float(Id, "detail", Math.Max(ins.Detail.Value - 1.0f, 0.0f));
			CSycles.shadernode_set_attribute_float(Id, "roughness",
				(float)Math.Pow(ins.Lacunarity.Value, -ins.Dimension.Value));
			CSycles.shadernode_set_attribute_bool(Id, "use_normalize", false);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
			Utilities.Instance.get_float(ins.Scale, xmlNode.GetAttribute("scale"));
			Utilities.Instance.get_float(ins.Dimension, xmlNode.GetAttribute("dimension"));
			Utilities.Instance.get_float(ins.Detail, xmlNode.GetAttribute("detail"));
			Utilities.Instance.get_float(ins.Lacunarity, xmlNode.GetAttribute("lacunarity"));

			var musgravetype = xmlNode.GetAttribute("musgrave_type");
			if (!string.IsNullOrEmpty(musgravetype))
			{
				SetType(musgravetype);
			}
			var dim = xmlNode.GetAttribute("dimension");
			if (!string.IsNullOrEmpty(dim))
			{
				SetDimension(dim);
			}
		}
		public override string CreateXmlAttributes()
		{
			var code = new StringBuilder($" musgrave_type=\"{MusgraveType}\" ", 1024);
			code.Append($" dimension=\"{Dimension}\" ");

			return code.ToString();
		}
		public override string CreateCodeAttributes()
		{
			var code = new StringBuilder($"{VariableName}.MusgraveType = {MusgraveType};", 1024);
			code.Append($"{VariableName}.Dimension= {Dimension};");

			return code.ToString();
		}
	}
}
