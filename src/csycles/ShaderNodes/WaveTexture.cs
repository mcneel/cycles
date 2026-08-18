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
	public class WaveInputs : Inputs
	{
		public VectorSocket Vector { get; set; }
		public FloatSocket Scale { get; set; }
		public FloatSocket Distortion { get; set; }
		public FloatSocket Detail { get; set; }
		public FloatSocket DetailScale { get; set; }

		public WaveInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
			Scale = new FloatSocket(parentNode, "Scale", "scale");
			AddSocket(Scale);
			Distortion = new FloatSocket(parentNode, "Distortion", "distortion");
			AddSocket(Distortion);
			Detail = new FloatSocket(parentNode, "Detail", "detail");
			AddSocket(Detail);
			DetailScale = new FloatSocket(parentNode, "Detail Scale", "detail_scale");
			AddSocket(DetailScale);
		}
	}

	public class WaveOutputs : Outputs
	{
		public ColorSocket Color { get; set; }
		public FloatSocket Fac { get; set; }

		public WaveOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Fac = new FloatSocket(parentNode, "Fac", "fac");
			AddSocket(Fac);
		}
	}

	[ShaderNode("wave_texture")]
	public class WaveTexture : TextureNode
	{

		public enum WaveTypes
		{
			Bands,
			Rings
		}

		public enum WaveProfiles
		{
			Sine,
			Saw
		}

		public WaveInputs ins => (WaveInputs)inputs;
		public WaveOutputs outs => (WaveOutputs)outputs;

		public WaveTexture(Shader shader) : this(shader, "a wave texture") { }
		public WaveTexture(Shader shader, string name)
			: base(shader, name)
		{
			FinalizeConstructor();
		}

		internal WaveTexture(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new WaveInputs(this);
			outputs = new WaveOutputs(this);

			ins.Scale.Value = 1.0f;
			ins.Distortion.Value = 0.0f;
			ins.Detail.Value = 2.0f;
			ins.DetailScale.Value = 1.0f;

			WaveType = WaveTypes.Bands;
			WaveProfile = WaveProfiles.Sine;
		}

		/// <summary>
		/// wave->type, one of
		/// - Bands
		/// - Rings
		/// </summary>
		public WaveTypes WaveType { get; set; }

		public WaveProfiles WaveProfile { get; set; }

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "wave", (int)WaveType);
			CSycles.shadernode_set_enum(Id, "profile", (int)WaveProfile);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.Vector, xmlNode.GetAttribute("vector"));
			Utilities.Instance.get_float(ins.Scale, xmlNode.GetAttribute("scale"));
			Utilities.Instance.get_float(ins.Distortion, xmlNode.GetAttribute("distortion"));
			Utilities.Instance.get_float(ins.Detail, xmlNode.GetAttribute("detail"));
			Utilities.Instance.get_float(ins.DetailScale, xmlNode.GetAttribute("detail_scale"));

			var wavetype = xmlNode.GetAttribute("wave_type");
			if (!string.IsNullOrEmpty(wavetype))
			{
				WaveTypes wt;
				if (Enum.TryParse(wavetype, out wt))
					WaveType = wt;
			}
			var waveprofile = xmlNode.GetAttribute("wave_profile");
			if (!string.IsNullOrEmpty(waveprofile))
			{
				WaveProfiles wp;
				if (Enum.TryParse(waveprofile, out wp))
					WaveProfile = wp;
			}
		}

		public override string CreateXmlAttributes()
		{
			var code = new StringBuilder($" wave_type=\"{WaveType}\" ", 1024);
			code.Append($" wave_profile=\"{WaveProfile}\" ");

			return code.ToString();
		}
		public override string CreateCodeAttributes()
		{
			var code = new StringBuilder($"{VariableName}.WaveType = {WaveType};", 1024);
			code.Append($"{VariableName}.WaveProfile = {WaveProfile};");

			return code.ToString();
		}
	}
}
