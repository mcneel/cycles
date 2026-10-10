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
	public class VoronoiInputs : Inputs
	{
		public VectorSocket Vector { get; set; }
		public FloatSocket W { get; set; }
		public FloatSocket Scale { get; set; }
		public FloatSocket Smoothness { get; set; }
		public FloatSocket Exponent { get; set; }
		public FloatSocket Randomness { get; set; }

		public VoronoiInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
			W = new FloatSocket(parentNode, "W", "w");
			AddSocket(W);
			Scale = new FloatSocket(parentNode, "Scale", "scale");
			AddSocket(Scale);
			Smoothness = new FloatSocket(parentNode, "Smoothness", "smoothness");
			AddSocket(Smoothness);
			Exponent = new FloatSocket(parentNode, "Exponent", "exponent");
			AddSocket(Exponent);
			Randomness = new FloatSocket(parentNode, "Randomness", "randomness");
			AddSocket(Randomness);
		}
	}

	public class VoronoiOutputs : Outputs
	{
		public FloatSocket Distance { get; set; }
		public ColorSocket Color { get; set; }
		public VectorSocket Position { get; set; }
		public FloatSocket W { get; set; }
		public FloatSocket Radius { get; set; }

		public VoronoiOutputs(ShaderNode parentNode)
		{
			Distance = new FloatSocket(parentNode, "Distance", "distance");
			AddSocket(Distance);
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Position = new VectorSocket(parentNode, "Position", "position");
			AddSocket(Position);
			W = new FloatSocket(parentNode, "W", "w");
			AddSocket(W);
			Radius = new FloatSocket(parentNode, "Radius", "radius");
			AddSocket(Radius);
		}
	}

	[ShaderNode("voronoi_texture")]
	public class VoronoiTexture : TextureNode
	{

		public enum Dimensions
		{
			D1 = 1,
			D2,
			D3,
			D4,
		}

		public enum Metrics
		{
			Euclidian,
			Manhattan,
			Chebychev,
			Minkowski,
		}

		public enum Features
		{
			F1,
			F2,
			SMOOTH_F1,
			DISTANCE_TO_EDGE,
			N_SPHERE_RADIUS,
		}
		public VoronoiInputs ins => (VoronoiInputs)inputs;
		public VoronoiOutputs outs => (VoronoiOutputs)outputs;

		public VoronoiTexture(Shader shader) : this(shader, "a voronoi texture") { }
		public VoronoiTexture(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal VoronoiTexture(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new VoronoiInputs(this);
			outputs = new VoronoiOutputs(this);

			ins.W.Value = 0.0f;
			ins.Scale.Value = 5.0f;
			ins.Smoothness.Value = 5.0f;
			ins.Exponent.Value = 0.5f;
			ins.Randomness.Value = 0.5f;
		}

		/// <summary>
		/// One of:
		/// - Cells
		/// - Intensity
		/// </summary>
		public Dimensions Dimension { get; set; } = Dimensions.D3;
		public Metrics Metric { get; set; } = Metrics.Euclidian;
		public Features Feature { get; set; } = Features.F1;

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "dimension", (int)Dimension);
			CSycles.shadernode_set_enum(Id, "metric", (int)Metric);
			CSycles.shadernode_set_enum(Id, "feature", (int)Feature);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
			Utilities.Instance.get_float(ins.Scale, xmlNode.GetAttribute("scale"));
			var dimension = xmlNode.GetAttribute("dimension");
			if (!string.IsNullOrEmpty(dimension))
			{
				if (Enum.TryParse(dimension, out Dimensions ct))
					Dimension = ct;
			}
			var metric = xmlNode.GetAttribute("metric");
			if (!string.IsNullOrEmpty(metric))
			{
				if (Enum.TryParse(metric, out Metrics ct))
					Metric = ct;
			}
			var feature = xmlNode.GetAttribute("feature");
			if (!string.IsNullOrEmpty(feature))
			{
				if (Enum.TryParse(feature, out Features ct))
					Feature = ct;
			}
		}
		public override string CreateXmlAttributes()
		{
			var code = new StringBuilder($" dimension=\"{Dimension}\" ", 1024);
			code.Append($" metric=\"{Metric}\" ");
			code.Append($" feature=\"{Feature}\" ");

			return code.ToString();
		}
		public override string CreateCodeAttributes()
		{
			var code = new StringBuilder($"{VariableName}.Dimension = {Dimension};", 1024);
			code.Append($"{VariableName}.Metric = {Metric};");
			code.Append($"{VariableName}.Feature = {Feature};");

			return code.ToString();
		}
	}
}
