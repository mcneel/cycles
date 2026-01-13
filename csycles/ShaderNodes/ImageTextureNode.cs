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
	/// ImageTexture input sockets
	/// </summary>
	public class ImageTextureInputs : Inputs
	{
		/// <summary>
		/// ImageTexture space coordinate to sample texture at
		/// </summary>
		public VectorSocket Vector { get; set; }

		/// <summary>
		/// DecalForward as calculated by the TextureCoordinateNode. Needs to be
		/// connected to work.
		/// </summary>
		public FloatSocket DecalForward { get; set; }
		/// <summary>
		/// DecalInside as calculated by the TextureCoordinateNode. Needs to be
		/// connected to work.
		/// </summary>
		public FloatSocket DecalUsage { get; set; }
		public StringSocket Filename { get; set; }

		internal ImageTextureInputs(ShaderNode parentNode)
		{
			Vector = new VectorSocket(parentNode, "Vector", "vector");
			AddSocket(Vector);
			DecalForward = new FloatSocket(parentNode, "DecalForward", "decalforward");
			AddSocket(DecalForward);
			DecalUsage = new FloatSocket(parentNode, "DecalUsage", "decalusage");
			AddSocket(DecalUsage);
			Filename = new StringSocket(parentNode, "Filename", "filename");
			AddSocket(Filename);
		}
	}

	/// <summary>
	/// ImageTexture output sockets
	/// </summary>
	public class ImageTextureOutputs : Outputs
	{
		/// <summary>
		/// ImageTexture Color output
		/// </summary>
		public ColorSocket Color { get; set; }
		/// <summary>
		/// ImageTexture alpha output
		/// </summary>
		public FloatSocket Alpha { get; set; }

		internal ImageTextureOutputs(ShaderNode parentNode)
		{
			Color = new ColorSocket(parentNode, "Color", "color");
			AddSocket(Color);
			Alpha = new FloatSocket(parentNode, "Alpha", "alpha");
			AddSocket(Alpha);
		}
	}

	[ShaderNode("image_texture")]
	public class ImageTextureNode : TextureNode
	{
		/// <summary>
		/// Image texture input sockets
		/// </summary>
		public ImageTextureInputs ins => (ImageTextureInputs)inputs;

		/// <summary>
		/// Image texture output sockets
		/// </summary>
		public ImageTextureOutputs outs => (ImageTextureOutputs)outputs;

		public ImageTextureNode(Shader shader) : this(shader, "an image texture node")
		{
		}

		public ImageTextureNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal ImageTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new ImageTextureInputs(this);
			outputs = new ImageTextureOutputs(this);

			// By default we don't do decal sampling, instead
			// this value gets set by TextureCoordinateNode output DecalUsage
			// when connected up
			ins.DecalUsage.Value = 0.0f;

			UseAlpha = true;
			AlternateTiles = false;
			ProjectionBlend = 0.0f;
			Interpolation = InterpolationType.Linear;
			ColorSpace = TextureColorSpace.None;
			Projection = TextureProjection.Flat;
			Extension = TextureExtension.Repeat;
		}

		/// <summary>
		/// ImageTexture texture projection
		/// </summary>
		public TextureProjection Projection { get; set; }
		/// <summary>
		/// ImageTexture texture projection blend
		/// </summary>
		public float ProjectionBlend { get; set; }
		/// <summary>
		/// ImageTexture float image
		/// </summary>
		public bool IsFloat { get; set; }
		/// <summary>
		/// ImageTexture use alpha channel if true
		///
		/// TODO [NATHANLOOK] hook up different alpha usage types. For now auto (true), ignore (false)
		/// </summary>
		public bool UseAlpha { get; set; }
		/// <summary>
		/// Set to true to alternate UV grid tiling. (Rhino specific)
		/// </summary>
		public bool AlternateTiles { get; set; }


		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "projection", (int)Projection);
			CSycles.shadernode_set_enum(Id, "interpolation", (int)Interpolation);
		}

		internal override void SetDirectMembers()
		{
			base.SetDirectMembers();
			CSycles.shadernode_set_member_float(Id, "projection_blend", ProjectionBlend);
			CSycles.shadernode_set_member_int(Id, "extension", (int)Extension);
			CSycles.shadernode_set_member_bool(Id, "use_alpha", UseAlpha);
			//CSycles.shadernode_set_member_bool(Id, "is_linear", IsLinear);
			CSycles.shadernode_set_member_bool(Id, "alternate_tiles", AlternateTiles);
		}
		private void SetProjection(string projection)
		{
			projection = projection.Replace(" ", "_");
			Projection = (TextureProjection)Enum.Parse(typeof(TextureProjection), projection, true);
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
			var extension = xmlNode.GetAttribute("extension");
			if (!string.IsNullOrEmpty(extension))
			{
				SetExtension(extension);
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
			code.Append($" use_alpha=\"{UseAlpha}\"");
			code.Append($" is_linear=\"{IsLinear}\"");
			code.Append($" alternate_tiles=\"{AlternateTiles}\"");
			if (Filename != null)
			{
				code.Append($" src=\"{Filename.Replace("\\", "\\\\")}\"");
			}

			return code.ToString();
		}

		public override string CreateCodeAttributes()
		{
			var code = new StringBuilder($"{VariableName}.Projection = {Projection};", 1024);
			code.Append($"{VariableName}.ColorSpace = {ColorSpace};");
			code.Append($"{VariableName}.Extension = {Extension};");
			code.Append($"{VariableName}.Interpolation = {Interpolation};");
			code.Append($"{VariableName}.UseAlpha = {UseAlpha};");
			code.Append($"{VariableName}.IsLinear = {IsLinear};");
			code.Append($"{VariableName}.AlternateTiles = {AlternateTiles};");

			return code.ToString();
		}
	}
}
