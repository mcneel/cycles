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
	/// <summary>
	/// EnvironmentTextureNode input sockets
	/// </summary>
	public class EnvironmentTextureInputs : Inputs
	{
		/// <summary>
		/// EnvironmentTextureNode vector input
		/// </summary>
		public VectorSocket Vector { get; set; }
		public StringSocket Filename { get; set; }

		internal EnvironmentTextureInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
			Filename = new StringSocket(parentNode, "Filename", "filename");
			AddSocket(Filename);
		}
	}

	/// <summary>
	/// EnvironmentTextureNode output sockets
	/// </summary>
	public class EnvironmentTextureOutputs : Outputs
	{
		/// <summary>
		/// EnvironmentTextureNode color output
		/// </summary>
		public ColorSocket Color { get; set; }
		/// <summary>
		/// EnvironmentTextureNode alpha output
		/// </summary>
		public FloatSocket Alpha { get; set; }

		internal EnvironmentTextureOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Alpha = new FloatSocket(parentNode, "Alpha", "alpha");
			AddSocket(Alpha);
		}
	}

	/// <summary>
	/// EnvironmentTextureNode
	/// </summary>
	[ShaderNode("environment_texture")]
	public class EnvironmentTextureNode : TextureNode
	{
		/// <summary>
		/// EnvironmentTextureNode input sockets
		/// </summary>
		public EnvironmentTextureInputs ins => (EnvironmentTextureInputs)inputs;

		/// <summary>
		/// EnvironmentTextureNode output sockets
		/// </summary>
		public EnvironmentTextureOutputs outs => (EnvironmentTextureOutputs)outputs;

		public EnvironmentTextureNode(Shader shader) : this(shader, "an env texture node") { }

		/// <summary>
		/// Create an EnvironmentTextureNode
		/// </summary>
		public EnvironmentTextureNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal EnvironmentTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new EnvironmentTextureInputs(this);
			outputs = new EnvironmentTextureOutputs(this);

			Interpolation = InterpolationType.Linear;
			ColorSpace = TextureColorSpace.None;
			Projection = EnvironmentProjection.Equirectangular;
		}

		/// <summary>
		/// Get or set environment projection
		/// </summary>
		public EnvironmentProjection Projection { get; set; }
		private void SetProjection(string projection)
		{
			projection = projection.Replace(" ", "_");
			Projection = (EnvironmentProjection)Enum.Parse(typeof(EnvironmentProjection), projection, true);
		}

		internal override void SetEnums()
		{
			Projection = Projection switch
			{
				EnvironmentProjection.Wallpaper => EnvironmentProjection.Wallpaper,
				_ => EnvironmentProjection.Equirectangular
			};
			CSycles.shadernode_set_enum(Id, "projection", (int)Projection);
			CSycles.shadernode_set_enum(Id, "color_space", (int)ColorSpace);
			CSycles.shadernode_set_enum(Id, "interpolation", (int)Interpolation);
		}

		internal override void SetDirectMembers()
		{
			base.SetDirectMembers();

			CSycles.shadernode_set_member_bool(Id, "is_linear", IsLinear);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			var cs = xmlNode.GetAttribute("color_space");
			if (!string.IsNullOrEmpty(cs))
			{
				SetColorSpace(cs);
			}
			var projection = xmlNode.GetAttribute("projection");
			if (!string.IsNullOrEmpty(projection))
			{
				SetProjection(projection);
			}
			var interpolation = xmlNode.GetAttribute("interpolation");
			if (!string.IsNullOrEmpty(interpolation))
			{
				SetInterpolation(interpolation);
			}
			ImageParseXml(xmlNode);
		}


		public override string CreateXmlAttributes()
		{
			var code = new StringBuilder($" projection=\"{Projection}\" ", 1024);
			code.Append($" color_space=\"{ColorSpace}\"");
			code.Append($" extension=\"{Extension}\"");
			code.Append($" interpolation=\"{Interpolation}\"");
			code.Append($" is_linear=\"{IsLinear}\"");
			if (Filename != null)
			{
				code.Append($" src=\"{Filename.Replace("\\", "\\\\")}\"");
			}

			return code.ToString();
		}
		public override string CreateCodeAttributes()
		{
			var code = $"{VariableName}.Projection = {Projection};";
			code += $"{VariableName}.ColorSpace = {ColorSpace};";
			code += $"{VariableName}.Extension = {Extension};";
			code += $"{VariableName}.Interpolation = {Interpolation};";
			code += $"{VariableName}.IsLinear = {IsLinear};";

			return code;
		}
	}
}
