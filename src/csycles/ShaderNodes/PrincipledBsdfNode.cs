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
		//public ColorSocket SpecularColor { get; set; }
		public ColorSocket SubsurfaceColor { get; set; }
		public FloatSocket Metallic { get; set; }
		public FloatSocket Subsurface { get; set; }
		public VectorSocket SubsurfaceRadius { get; set; }
		public FloatSocket Specular { get; set; }
		public FloatSocket Roughness { get; set; }
		public ColorSocket SpecularTint { get; set; }
		public FloatSocket Anisotropic { get; set; }
		public FloatSocket Sheen { get; set; }
		public ColorSocket SheenTint { get; set; }
		public FloatSocket Clearcoat { get; set; }
		/* Was ClearcoatGloss. The 4.x socket is Coat Roughness - the inverse
		 * quantity - so the old name invited exactly the wrong value. */
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

			/* Blender 4.x reworked the principled BSDF and 5.2 carries that rework, so
			 * most of these sockets were renamed and two were dropped. Every name below
			 * was wrong for 5.2 - the values silently went nowhere and the connections
			 * silently found no socket. The C# property names are deliberately left
			 * alone: RhinoCycles assigns all of them, and renaming would churn a lot of
			 * calling code to say the same thing.
			 *
			 * The mapping, for whoever meets 6.x:
			 *   Subsurface            -> Subsurface Weight
			 *   Specular              -> Specular IOR Level
			 *   Sheen                 -> Sheen Weight
			 *   Clearcoat             -> Coat Weight
			 *   Clearcoat Roughness   -> Coat Roughness
			 *   Clearcoat Normal      -> Coat Normal
			 *   Transmission          -> Transmission Weight
			 *   Emission              -> Emission Color
			 *   Specular Tint         -> now a colour, was a scalar
			 *   Sheen Tint            -> now a colour, was a scalar
			 *   Subsurface Color      -> gone; base colour drives subsurface now
			 *   Transmission Roughness-> gone; shares Roughness now
			 *
			 * The two that are gone are marked Retired rather than deleted, so the
			 * RhinoCycles code that still feeds them keeps compiling while the values go
			 * nowhere. Both need a real migration to render faithfully again. */
			BaseColor = new ColorSocket(parentNode, "Base Color", "base_color");
			Subsurface = new FloatSocket(parentNode, "Subsurface Weight", "subsurface_weight");
			SubsurfaceRadius = new VectorSocket(parentNode, "Subsurface Radius", "subsurface_radius");
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
			//AddSocket(SpecularColor);
			AddSocket(Subsurface);
			AddSocket(SubsurfaceRadius);
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
	/// A Principled BSDF closure.
	/// This closure takes two inputs, <c>Color</c> and <c>Roughness</c>. The result
	/// will be a regular diffuse shading.
	///
	/// There is one output <c>Closure</c>
	/// </summary>
	[ShaderNode("principled_bsdf")]
	public class PrincipledBsdfNode : ShaderNode
	{
		/// <summary>
		/// Raw ccl::ClosureType values - these sockets are SOCKET_ENUMs whose entries are
		/// closure ids, so the number sent has to be one Cycles recognises. Check them
		/// against ClosureType in kernel/svm/types.h whenever Cycles moves.
		///
		/// The distribution values look wrong and are not: for the principled BSDF the
		/// kernel tests distribution == MULTI_GGX_GLASS (26) to select multiscatter GGX,
		/// so 26 is a marker rather than a request for glass.
		/// </summary>
		public enum Distributions
		{
			GGX = 26,
			Multiscatter_GGX = 24
		}

		/// <summary>
		/// These were off by one against 5.2's ClosureType: BSSRDF_BURLEY is 31, not 32,
		/// so asking for Burley selected random walk, random walk selected the legacy
		/// variant, and fixed radius selected the skin variant.
		/// </summary>
		public enum ScatterMethod
		{
			Burley = 31,
			RandomWalk = 32,
			RandomWalkFixedRadius = 33,
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
			/* TODO: Add scatter method property */
			inputs = new PrincipledBsdfInputs(this);
			outputs = new PrincipledBsdfOutputs(this);
			ins.BaseColor.Value = new float4(0.7f, 0.6f, 0.5f, 1.0f);
			ins.Metallic.Value = 0.0f;
			ins.Specular.Value = 0.5f;
			/* Specular Tint and Sheen Tint were floats before 4.x, where 0 meant
			 * "untinted". They are colours now and untinted is white - Cycles itself
			 * defaults both to one_float3(). Carrying the old 0 across as black asks
			 * for no specular reflection at all. */
			ins.SpecularTint.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
			ins.Subsurface.Value = 0.0f;
			ins.SubsurfaceColor.Value = new float4(0.7f, 0.1f, 0.1f);
			ins.SubsurfaceRadius.Value = new float4(0.7f, 1.0f, 1.0f, 1.0f);
			ins.Roughness.Value = 0.0f;
			ins.Anisotropic.Value = 0.0f;
			ins.AnisotropicRotation.Value = 0.0f;
			ins.Sheen.Value = 0.0f;
			ins.SheenTint.Value = new float4(1.0f, 1.0f, 1.0f, 1.0f);
			ins.Clearcoat.Value = 0.0f;
			/* 1.0 here used to mean a mirror-smooth coat, as gloss. Against
			 * coat_roughness it asks for the roughest possible coat instead, so any
			 * material with coat weight above zero came out fully rough. Cycles'
			 * own default is 0.03. */
			ins.CoatRoughness.Value = 0.03f;
			ins.IOR.Value = 1.45f;
			ins.Transmission.Value = 0.0f;
			ins.TransmissionRoughness.Value = 0.0f;
			ins.Emission.Value = new float4(0.0f);
			ins.EmissionStrength.Value = 0.0f;
			ins.Alpha.Value = 1.0f;
			Distribution = Distributions.GGX;
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
			//Utilities.Instance.get_float4(ins.SpecularColor, xmlNode);
			Utilities.Instance.get_float(ins.Subsurface, xmlNode);
			Utilities.Instance.get_float4(ins.SubsurfaceRadius, xmlNode);
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
