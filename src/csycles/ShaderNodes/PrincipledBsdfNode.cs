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
	public class PrincipledBsdfInputs : Inputs
	{
		public ColorSocket BaseColor { get; set; }
		public ColorSocket SubsurfaceColor { get; set; }
		public FloatSocket Metallic { get; set; }
		public FloatSocket Subsurface { get; set; }
		public VectorSocket SubsurfaceRadius { get; set; }
		public FloatSocket SubsurfaceScale { get; set; }
		public FloatSocket Specular { get; set; }
		public FloatSocket Roughness { get; set; }
		public ColorSocket SpecularTint { get; set; }
		public FloatSocket Anisotropic { get; set; }
		public FloatSocket Sheen { get; set; }
		public ColorSocket SheenTint { get; set; }
		public FloatSocket Clearcoat { get; set; }
		/* Was ClearcoatGloss; 4.x's Coat Roughness is the inverse quantity. */
		public FloatSocket CoatRoughness { get; set; }
		public FloatSocket IOR { get; set; }
		public FloatSocket Transmission { get; set; }
		public FloatSocket TransmissionRoughness { get; set; }
		public FloatSocket AnisotropicRotation { get; set; }
		public ColorSocket Emission { get; set; }
		public FloatSocket EmissionStrength { get; set; }
		public FloatSocket Alpha { get; set; }
		public VectorSocket Normal { get; set; }
		public VectorSocket ClearcoatNormal { get; set; }
		public VectorSocket Tangent { get; set; }

		public PrincipledBsdfInputs(ShaderNode parentNode)
		{

			/* Socket names are Blender 4.x's principled rework (kept in 5.2); the C#
			 * properties keep the old names because RhinoCycles assigns all of them.
			 * Subsurface Color and Transmission Roughness are gone and stay as Retired;
			 * RhinoFullNxt migrates Subsurface Color, Transmission Roughness not yet. */
			BaseColor = new ColorSocket(parentNode, "Base Color", "base_color");
			Subsurface = new FloatSocket(parentNode, "Subsurface Weight", "subsurface_weight");
			SubsurfaceRadius = new VectorSocket(parentNode, "Subsurface Radius", "subsurface_radius");
			SubsurfaceScale = new FloatSocket(parentNode, "Subsurface Scale", "subsurface_scale");
			SubsurfaceColor = new ColorSocket(parentNode, "Subsurface Color", "subsurface_color") { Retired = true };
			Metallic = new FloatSocket(parentNode, "Metallic", "metallic");
			Specular = new FloatSocket(parentNode, "Specular IOR Level", "specular_ior_level");
			SpecularTint = new ColorSocket(parentNode, "Specular Tint", "specular_tint");
			Roughness = new FloatSocket(parentNode, "Roughness", "roughness");
			Anisotropic = new FloatSocket(parentNode, "Anisotropic", "anisotropic");
			Sheen = new FloatSocket(parentNode, "Sheen Weight", "sheen_weight");
			SheenTint = new ColorSocket(parentNode, "Sheen Tint", "sheen_tint");
			Clearcoat = new FloatSocket(parentNode, "Coat Weight", "coat_weight");
			CoatRoughness = new FloatSocket(parentNode, "Coat Roughness", "coat_roughness");
			IOR = new FloatSocket(parentNode, "IOR", "ior");
			Transmission = new FloatSocket(parentNode, "Transmission Weight", "transmission_weight");
			TransmissionRoughness = new FloatSocket(parentNode, "Transmission Roughness", "transmission_roughness") { Retired = true };
			AnisotropicRotation = new FloatSocket(parentNode, "Anisotropic Rotation", "anisotropic_rotation");
			Emission = new ColorSocket(parentNode, "Emission Color", "emission_color");
			EmissionStrength = new FloatSocket(parentNode, "Emission Strength", "emission_strength");
			Alpha = new FloatSocket(parentNode, "Alpha", "alpha");
			Normal = new VectorSocket(parentNode, "Normal", "normal");
			ClearcoatNormal = new VectorSocket(parentNode, "Coat Normal", "coat_normal");
			Tangent = new VectorSocket(parentNode, "Tangent", "tangent");

			AddSocket(BaseColor);
			AddSocket(Subsurface);
			AddSocket(SubsurfaceRadius);
			AddSocket(SubsurfaceScale);
			AddSocket(SubsurfaceColor);
			AddSocket(Metallic);
			AddSocket(Specular);
			AddSocket(SpecularTint);
			AddSocket(Roughness);
			AddSocket(Anisotropic);
			AddSocket(AnisotropicRotation);
			AddSocket(Sheen);
			AddSocket(SheenTint);
			AddSocket(Clearcoat);
			AddSocket(CoatRoughness);
			AddSocket(IOR);
			AddSocket(Transmission);
			AddSocket(TransmissionRoughness);
			AddSocket(Emission);
			AddSocket(EmissionStrength);
			AddSocket(Alpha);
			AddSocket(Normal);
			AddSocket(ClearcoatNormal);
			AddSocket(Tangent);
		}
	}

	public class PrincipledBsdfOutputs : Outputs
	{
		public ClosureSocket BSDF { get; set; }

		public PrincipledBsdfOutputs(ShaderNode parentNode)
		{
			BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF");
			AddSocket(BSDF);
		}
	}

	/// <summary>
	/// A Principled BSDF closure with one output <c>BSDF</c>.
	/// </summary>
	[ShaderNode("principled_bsdf")]
	public class PrincipledBsdfNode : ShaderNode
	{
		/// <summary>
		/// Raw ccl::ClosureType ids, cast with no name lookup: check them against kernel/svm/types.h
		/// whenever Cycles moves (3.5 had GGX_GLASS 26, MULTI_GGX_GLASS 24). The glass ids are
		/// deliberate: the principled kernel tests for MULTI_GGX_GLASS to select multiscatter GGX.
		/// Multiscatter is Blender's default; that it renders brighter than shipping Rhino as roughness
		/// rises is Cycles 5's behaviour, not a fault to correct.
		/// </summary>
		public enum Distributions
		{
			GGX = 25,               // CLOSURE_BSDF_MICROFACET_GGX_GLASS_ID
			Multiscatter_GGX = 26   // CLOSURE_BSDF_MICROFACET_MULTI_GGX_GLASS_ID
		}

		/// <summary>
		/// Raw ccl::ClosureType ids (BSSRDF_BURLEY is 31). Names follow Cycles 5, whose RandomWalk
		/// is a new model: 3.5's random walk is RandomWalkSkin, and 3.5's fixed radius is
		/// RandomWalkFixedRadius (random_walk_legacy).
		/// </summary>
		public enum ScatterMethod
		{
			Burley = 31,                // CLOSURE_BSSRDF_BURLEY_ID
			RandomWalk = 32,            // CLOSURE_BSSRDF_RANDOM_WALK_ID
			RandomWalkFixedRadius = 33, // CLOSURE_BSSRDF_RANDOM_WALK_LEGACY_ID
			RandomWalkSkin = 34,        // CLOSURE_BSSRDF_RANDOM_WALK_SKIN_ID
		}

		public PrincipledBsdfInputs ins => (PrincipledBsdfInputs)inputs;
		public PrincipledBsdfOutputs outs => (PrincipledBsdfOutputs)outputs;

		/// <summary>
		/// Create a new Principled BSDF closure.
		/// </summary>
		public PrincipledBsdfNode(Shader shader) : this(shader, "a principled bsdf node") { }
		public PrincipledBsdfNode(Shader shader, string name) :
			base(shader, name)
		{
			FinalizeConstructor();
		}

		internal PrincipledBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
		{
			FinalizeConstructor();
		}

		private void FinalizeConstructor()
		{
			inputs = new PrincipledBsdfInputs(this);
			outputs = new PrincipledBsdfOutputs(this);
			ins.BaseColor.Value = new float4(0.7f, 0.6f, 0.5f, 1.0f);
			ins.Metallic.Value = 0.0f;
			ins.Specular.Value = 0.5f;
			/* The tints are colours since 4.x and untinted is white; the old float 0, as
			 * black, would remove the specular entirely. */
			ins.SpecularTint.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
			ins.Subsurface.Value = 0.0f;
			ins.SubsurfaceColor.Value = new float4(0.7f, 0.1f, 0.1f);
			ins.SubsurfaceRadius.Value = new float4(0.7f, 1.0f, 1.0f, 1.0f);
			/* Cycles' default; every value is pushed, and at scale 0 Cycles skips subsurface. */
			ins.SubsurfaceScale.Value = 0.005f;
			ins.Roughness.Value = 0.0f;
			ins.Anisotropic.Value = 0.0f;
			ins.AnisotropicRotation.Value = 0.0f;
			ins.Sheen.Value = 0.0f;
			ins.SheenTint.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
			ins.Clearcoat.Value = 0.0f;
			/* Cycles' default. The old gloss 1.0 (mirror-smooth) would be the roughest coat. */
			ins.CoatRoughness.Value = 0.03f;
			ins.IOR.Value = 1.45f;
			ins.Transmission.Value = 0.0f;
			ins.TransmissionRoughness.Value = 0.0f;
			ins.Emission.Value = new float4(0.0f);
			ins.EmissionStrength.Value = 0.0f;
			ins.Alpha.Value = 1.0f;
			Distribution = Distributions.Multiscatter_GGX;
			Sss = ScatterMethod.RandomWalk;
		}

		public Distributions Distribution { get; set; }
		public ScatterMethod Sss { get; set; }

		internal override void SetEnums()
		{
			CSycles.shadernode_set_enum(Id, "distribution", (int)Distribution);
			CSycles.shadernode_set_enum(Id, "sss", (int)Sss);
		}

		internal override void ParseXml(XmlReader xmlNode)
		{
			Utilities.Instance.get_float4(ins.BaseColor, xmlNode);
			Utilities.Instance.get_float(ins.Subsurface, xmlNode);
			Utilities.Instance.get_float4(ins.SubsurfaceRadius, xmlNode);
			Utilities.Instance.get_float(ins.SubsurfaceScale, xmlNode);
			Utilities.Instance.get_float4(ins.SubsurfaceColor, xmlNode);
			Utilities.Instance.get_float(ins.Metallic, xmlNode);
			Utilities.Instance.get_float(ins.Specular, xmlNode);
			Utilities.Instance.get_float4(ins.SpecularTint, xmlNode);
			Utilities.Instance.get_float(ins.Roughness, xmlNode);
			Utilities.Instance.get_float(ins.Anisotropic, xmlNode);
			Utilities.Instance.get_float(ins.AnisotropicRotation, xmlNode);
			Utilities.Instance.get_float(ins.Sheen, xmlNode);
			Utilities.Instance.get_float4(ins.SheenTint, xmlNode);
			Utilities.Instance.get_float(ins.Clearcoat, xmlNode);
			Utilities.Instance.get_float(ins.CoatRoughness, xmlNode);
			Utilities.Instance.get_float(ins.IOR, xmlNode);
			Utilities.Instance.get_float(ins.Transmission, xmlNode);
			Utilities.Instance.get_float(ins.TransmissionRoughness, xmlNode);
			Utilities.Instance.get_float4(ins.Normal, xmlNode);
			Utilities.Instance.get_float4(ins.ClearcoatNormal, xmlNode);
			Utilities.Instance.get_float4(ins.Tangent, xmlNode);
			var str = "";
			Utilities.Instance.read_string(ref str, xmlNode.GetAttribute("distribution"));
			if (!string.IsNullOrEmpty(str))
			{
				Distributions d;
				if (Enum.TryParse(str, true, out d)) Distribution = d;
			}
			str = "";
			Utilities.Instance.read_string(ref str, xmlNode.GetAttribute("sss"));
			if (!string.IsNullOrEmpty(str))
			{
				ScatterMethod sss;
				if (Enum.TryParse(str, true, out sss)) Sss = sss;
			}
		}

		public override ClosureSocket GetClosureSocket()
		{
			return outs.BSDF;
		}
	}
}
