/**
Copyright 2014-2025 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.

----------------------------------------------------------------------
NOTE: Do NOT modify this file directly, it is automatically generated.

Code generated at: 2025-12-02 03:24:08 UTC
----------------------------------------------------------------------

**/

using ccl;
using ccl.Attributes;
using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;
using ccl.NodeSockets;
using System;
using System.Collections.Generic;
namespace ccl.ShaderNodes
{
    using cclext;

    public class PrincipledBsdfNodeInputs : Inputs
    {
        public VectorSocket SubsurfaceRadius { get; private set; }
        public ColorSocket EmissionColor { get; private set; }
        public FloatSocket SubsurfaceIOR { get; private set; }
        public FloatSocket EmissionStrength { get; private set; }
        public FloatSocket SubsurfaceAnisotropy { get; private set; }
        public FloatSocket ThinFilmThickness { get; private set; }
        public FloatSocket SpecularIORLevel { get; private set; }
        public FloatSocket ThinFilmIOR { get; private set; }
        public ColorSocket SpecularTint { get; private set; }
        public FloatSocket Anisotropic { get; private set; }
        public FloatSocket AnisotropicRotation { get; private set; }
        public NormalSocket Tangent { get; private set; }
        public EnumSocket Distribution { get; private set; }
        public FloatSocket TransmissionWeight { get; private set; }
        public EnumSocket SubsurfaceMethod { get; private set; }
        public FloatSocket SheenWeight { get; private set; }
        public ColorSocket BaseColor { get; private set; }
        public FloatSocket SheenRoughness { get; private set; }
        public FloatSocket Metallic { get; private set; }
        public ColorSocket SheenTint { get; private set; }
        public FloatSocket Roughness { get; private set; }
        public FloatSocket CoatWeight { get; private set; }
        public FloatSocket IOR { get; private set; }
        public FloatSocket CoatRoughness { get; private set; }
        public FloatSocket Alpha { get; private set; }
        public FloatSocket CoatIOR { get; private set; }
        public NormalSocket Normal { get; private set; }
        public ColorSocket CoatTint { get; private set; }
        public FloatSocket DiffuseRoughness { get; private set; }
        public NormalSocket CoatNormal { get; private set; }
        public FloatSocket SubsurfaceWeight { get; private set; }
        public FloatSocket SubsurfaceScale { get; private set; }

        public PrincipledBsdfNodeInputs(ShaderNode parentNode)
        {
            SubsurfaceRadius = new VectorSocket(parentNode, "Subsurface Radius", "subsurface_radius", true);
            AddSocket(SubsurfaceRadius);
            EmissionColor = new ColorSocket(parentNode, "Emission Color", "emission_color", true);
            AddSocket(EmissionColor);
            SubsurfaceIOR = new FloatSocket(parentNode, "Subsurface IOR", "subsurface_ior", true);
            AddSocket(SubsurfaceIOR);
            EmissionStrength = new FloatSocket(parentNode, "Emission Strength", "emission_strength", true);
            AddSocket(EmissionStrength);
            SubsurfaceAnisotropy = new FloatSocket(parentNode, "Subsurface Anisotropy", "subsurface_anisotropy", true);
            AddSocket(SubsurfaceAnisotropy);
            ThinFilmThickness = new FloatSocket(parentNode, "Thin Film Thickness", "thin_film_thickness", true);
            AddSocket(ThinFilmThickness);
            SpecularIORLevel = new FloatSocket(parentNode, "Specular IOR Level", "specular_ior_level", true);
            AddSocket(SpecularIORLevel);
            ThinFilmIOR = new FloatSocket(parentNode, "Thin Film IOR", "thin_film_ior", true);
            AddSocket(ThinFilmIOR);
            SpecularTint = new ColorSocket(parentNode, "Specular Tint", "specular_tint", true);
            AddSocket(SpecularTint);
            Anisotropic = new FloatSocket(parentNode, "Anisotropic", "anisotropic", true);
            AddSocket(Anisotropic);
            AnisotropicRotation = new FloatSocket(parentNode, "Anisotropic Rotation", "anisotropic_rotation", true);
            AddSocket(AnisotropicRotation);
            Tangent = new NormalSocket(parentNode, "Tangent", "tangent", true);
            AddSocket(Tangent);
            Distribution = new EnumSocket(parentNode, "Distribution", "distribution", true);
            AddSocket(Distribution);
            TransmissionWeight = new FloatSocket(parentNode, "Transmission Weight", "transmission_weight", true);
            AddSocket(TransmissionWeight);
            SubsurfaceMethod = new EnumSocket(parentNode, "Subsurface Method", "subsurface_method", true);
            AddSocket(SubsurfaceMethod);
            SheenWeight = new FloatSocket(parentNode, "Sheen Weight", "sheen_weight", true);
            AddSocket(SheenWeight);
            BaseColor = new ColorSocket(parentNode, "Base Color", "base_color", true);
            AddSocket(BaseColor);
            SheenRoughness = new FloatSocket(parentNode, "Sheen Roughness", "sheen_roughness", true);
            AddSocket(SheenRoughness);
            Metallic = new FloatSocket(parentNode, "Metallic", "metallic", true);
            AddSocket(Metallic);
            SheenTint = new ColorSocket(parentNode, "Sheen Tint", "sheen_tint", true);
            AddSocket(SheenTint);
            Roughness = new FloatSocket(parentNode, "Roughness", "roughness", true);
            AddSocket(Roughness);
            CoatWeight = new FloatSocket(parentNode, "Coat Weight", "coat_weight", true);
            AddSocket(CoatWeight);
            IOR = new FloatSocket(parentNode, "IOR", "ior", true);
            AddSocket(IOR);
            CoatRoughness = new FloatSocket(parentNode, "Coat Roughness", "coat_roughness", true);
            AddSocket(CoatRoughness);
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", true);
            AddSocket(Alpha);
            CoatIOR = new FloatSocket(parentNode, "Coat IOR", "coat_ior", true);
            AddSocket(CoatIOR);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            CoatTint = new ColorSocket(parentNode, "Coat Tint", "coat_tint", true);
            AddSocket(CoatTint);
            DiffuseRoughness = new FloatSocket(parentNode, "Diffuse Roughness", "diffuse_roughness", true);
            AddSocket(DiffuseRoughness);
            CoatNormal = new NormalSocket(parentNode, "Coat Normal", "coat_normal", true);
            AddSocket(CoatNormal);
            SubsurfaceWeight = new FloatSocket(parentNode, "Subsurface Weight", "subsurface_weight", true);
            AddSocket(SubsurfaceWeight);
            SubsurfaceScale = new FloatSocket(parentNode, "Subsurface Scale", "subsurface_scale", true);
            AddSocket(SubsurfaceScale);
        }
    }
    public class PrincipledBsdfNodeOutputs : Outputs
    {
        public ClosureSocket BSDF { get; private set; }

        public PrincipledBsdfNodeOutputs(ShaderNode parentNode)
        {
            BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF", false);
            AddSocket(BSDF);
        }
    }

    [ShaderNode(name: "principled_bsdf")]
    public class PrincipledBsdfNode : BsdfBaseNode
    {
        public enum PrincipledBsdfNodeDistribution : uint {
            Ggx = ccl.ClosureType.CLOSURE_BSDF_MICROFACET_GGX_GLASS_ID,
            MultiGgx = ccl.ClosureType.CLOSURE_BSDF_MICROFACET_MULTI_GGX_GLASS_ID,
        }
        public enum PrincipledBsdfNodeSubsurfaceMethod : uint {
            Burley = ccl.ClosureType.CLOSURE_BSSRDF_BURLEY_ID,
            RandomWalk = ccl.ClosureType.CLOSURE_BSSRDF_RANDOM_WALK_ID,
            RandomWalkSkin = ccl.ClosureType.CLOSURE_BSSRDF_RANDOM_WALK_SKIN_ID,
        }
        public PrincipledBsdfNodeInputs ins => (PrincipledBsdfNodeInputs)inputs;
        public PrincipledBsdfNodeOutputs outs => (PrincipledBsdfNodeOutputs)outputs;
        public PrincipledBsdfNode(Shader shader) : this(shader, "a principled_bsdf node") { }

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
            inputs = new PrincipledBsdfNodeInputs(this);
            outputs = new PrincipledBsdfNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.BSDF;
        }
        public static IntPtr GetNodeType() {
            return CSycles.principledbsdfnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "subsurface_ior":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.4f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_ior', 'ui_name': 'Subsurface IOR'} */
                    {
                    CSycles.principledbsdfnode_set_subsurface_ior(this.Ptr, data);
                    }
                    break;
            case "emission_strength":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'emission_strength', 'ui_name': 'Emission Strength'} */
                    {
                    CSycles.principledbsdfnode_set_emission_strength(this.Ptr, data);
                    }
                    break;
            case "subsurface_anisotropy":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_anisotropy', 'ui_name': 'Subsurface Anisotropy'} */
                    {
                    CSycles.principledbsdfnode_set_subsurface_anisotropy(this.Ptr, data);
                    }
                    break;
            case "thin_film_thickness":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'thin_film_thickness', 'ui_name': 'Thin Film Thickness'} */
                    {
                    CSycles.principledbsdfnode_set_thin_film_thickness(this.Ptr, data);
                    }
                    break;
            case "specular_ior_level":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'specular_ior_level', 'ui_name': 'Specular IOR Level'} */
                    {
                    CSycles.principledbsdfnode_set_specular_ior_level(this.Ptr, data);
                    }
                    break;
            case "thin_film_ior":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.3f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'thin_film_ior', 'ui_name': 'Thin Film IOR'} */
                    {
                    CSycles.principledbsdfnode_set_thin_film_ior(this.Ptr, data);
                    }
                    break;
            case "anisotropic":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropic', 'ui_name': 'Anisotropic'} */
                    {
                    CSycles.principledbsdfnode_set_anisotropic(this.Ptr, data);
                    }
                    break;
            case "anisotropic_rotation":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropic_rotation', 'ui_name': 'Anisotropic Rotation'} */
                    {
                    CSycles.principledbsdfnode_set_anisotropic_rotation(this.Ptr, data);
                    }
                    break;
            case "transmission_weight":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'transmission_weight', 'ui_name': 'Transmission Weight'} */
                    {
                    CSycles.principledbsdfnode_set_transmission_weight(this.Ptr, data);
                    }
                    break;
            case "sheen_weight":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sheen_weight', 'ui_name': 'Sheen Weight'} */
                    {
                    CSycles.principledbsdfnode_set_sheen_weight(this.Ptr, data);
                    }
                    break;
            case "sheen_roughness":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sheen_roughness', 'ui_name': 'Sheen Roughness'} */
                    {
                    CSycles.principledbsdfnode_set_sheen_roughness(this.Ptr, data);
                    }
                    break;
            case "metallic":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'metallic', 'ui_name': 'Metallic'} */
                    {
                    CSycles.principledbsdfnode_set_metallic(this.Ptr, data);
                    }
                    break;
            case "roughness":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                    {
                    CSycles.principledbsdfnode_set_roughness(this.Ptr, data);
                    }
                    break;
            case "coat_weight":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'coat_weight', 'ui_name': 'Coat Weight'} */
                    {
                    CSycles.principledbsdfnode_set_coat_weight(this.Ptr, data);
                    }
                    break;
            case "ior":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ior', 'ui_name': 'IOR'} */
                    {
                    CSycles.principledbsdfnode_set_ior(this.Ptr, data);
                    }
                    break;
            case "coat_roughness":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.03f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'coat_roughness', 'ui_name': 'Coat Roughness'} */
                    {
                    CSycles.principledbsdfnode_set_coat_roughness(this.Ptr, data);
                    }
                    break;
            case "alpha":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha', 'ui_name': 'Alpha'} */
                    {
                    CSycles.principledbsdfnode_set_alpha(this.Ptr, data);
                    }
                    break;
            case "coat_ior":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'coat_ior', 'ui_name': 'Coat IOR'} */
                    {
                    CSycles.principledbsdfnode_set_coat_ior(this.Ptr, data);
                    }
                    break;
            case "diffuse_roughness":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'diffuse_roughness', 'ui_name': 'Diffuse Roughness'} */
                    {
                    CSycles.principledbsdfnode_set_diffuse_roughness(this.Ptr, data);
                    }
                    break;
            case "subsurface_weight":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_weight', 'ui_name': 'Subsurface Weight'} */
                    {
                    CSycles.principledbsdfnode_set_subsurface_weight(this.Ptr, data);
                    }
                    break;
            case "subsurface_scale":
                    /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_scale', 'ui_name': 'Subsurface Scale'} */
                    {
                    CSycles.principledbsdfnode_set_subsurface_scale(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "subsurface_radius":
                    /* principledbsdfnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.1f,0.1f,0.1f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'subsurface_radius', 'ui_name': 'Subsurface Radius'} */
                    {
                    CSycles.principledbsdfnode_set_subsurface_radius(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "tangent":
                    /* principledbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'tangent', 'ui_name': 'Tangent'} */
                    {
                    CSycles.principledbsdfnode_set_tangent(this.Ptr, data);
                    }
                    break;
            case "normal":
                    /* principledbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.principledbsdfnode_set_normal(this.Ptr, data);
                    }
                    break;
            case "coat_normal":
                    /* principledbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'coat_normal', 'ui_name': 'Coat Normal'} */
                    {
                    CSycles.principledbsdfnode_set_coat_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "emission_color":
                    /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'emission_color', 'ui_name': 'Emission Color'} */
                    {
                    CSycles.principledbsdfnode_set_emission_color(this.Ptr, data);
                    }
                    break;
            case "specular_tint":
                    /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'specular_tint', 'ui_name': 'Specular Tint'} */
                    {
                    CSycles.principledbsdfnode_set_specular_tint(this.Ptr, data);
                    }
                    break;
            case "base_color":
                    /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'base_color', 'ui_name': 'Base Color'} */
                    {
                    CSycles.principledbsdfnode_set_base_color(this.Ptr, data);
                    }
                    break;
            case "sheen_tint":
                    /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'sheen_tint', 'ui_name': 'Sheen Tint'} */
                    {
                    CSycles.principledbsdfnode_set_sheen_tint(this.Ptr, data);
                    }
                    break;
            case "coat_tint":
                    /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'coat_tint', 'ui_name': 'Coat Tint'} */
                    {
                    CSycles.principledbsdfnode_set_coat_tint(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "distribution":
                    /* principledbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_MICROFACET_MULTI_GGX_GLASS_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'distribution', 'ui_name': 'Distribution'} */
                    {
                    CSycles.principledbsdfnode_set_distribution(this.Ptr, (ccl.ClosureType)data);
                    }
                    break;
            case "subsurface_method":
                    /* principledbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSSRDF_RANDOM_WALK_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'subsurface_method', 'ui_name': 'Subsurface Method'} */
                    {
                    CSycles.principledbsdfnode_set_subsurface_method(this.Ptr, (ccl.ClosureType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "subsurface_ior":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.4f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_ior', 'ui_name': 'Subsurface IOR'} */
                {
                    return CSycles.principledbsdfnode_get_subsurface_ior(this.Ptr);
                }
            case "emission_strength":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'emission_strength', 'ui_name': 'Emission Strength'} */
                {
                    return CSycles.principledbsdfnode_get_emission_strength(this.Ptr);
                }
            case "subsurface_anisotropy":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_anisotropy', 'ui_name': 'Subsurface Anisotropy'} */
                {
                    return CSycles.principledbsdfnode_get_subsurface_anisotropy(this.Ptr);
                }
            case "thin_film_thickness":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'thin_film_thickness', 'ui_name': 'Thin Film Thickness'} */
                {
                    return CSycles.principledbsdfnode_get_thin_film_thickness(this.Ptr);
                }
            case "specular_ior_level":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'specular_ior_level', 'ui_name': 'Specular IOR Level'} */
                {
                    return CSycles.principledbsdfnode_get_specular_ior_level(this.Ptr);
                }
            case "thin_film_ior":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.3f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'thin_film_ior', 'ui_name': 'Thin Film IOR'} */
                {
                    return CSycles.principledbsdfnode_get_thin_film_ior(this.Ptr);
                }
            case "anisotropic":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropic', 'ui_name': 'Anisotropic'} */
                {
                    return CSycles.principledbsdfnode_get_anisotropic(this.Ptr);
                }
            case "anisotropic_rotation":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropic_rotation', 'ui_name': 'Anisotropic Rotation'} */
                {
                    return CSycles.principledbsdfnode_get_anisotropic_rotation(this.Ptr);
                }
            case "transmission_weight":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'transmission_weight', 'ui_name': 'Transmission Weight'} */
                {
                    return CSycles.principledbsdfnode_get_transmission_weight(this.Ptr);
                }
            case "sheen_weight":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sheen_weight', 'ui_name': 'Sheen Weight'} */
                {
                    return CSycles.principledbsdfnode_get_sheen_weight(this.Ptr);
                }
            case "sheen_roughness":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sheen_roughness', 'ui_name': 'Sheen Roughness'} */
                {
                    return CSycles.principledbsdfnode_get_sheen_roughness(this.Ptr);
                }
            case "metallic":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'metallic', 'ui_name': 'Metallic'} */
                {
                    return CSycles.principledbsdfnode_get_metallic(this.Ptr);
                }
            case "roughness":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                {
                    return CSycles.principledbsdfnode_get_roughness(this.Ptr);
                }
            case "coat_weight":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'coat_weight', 'ui_name': 'Coat Weight'} */
                {
                    return CSycles.principledbsdfnode_get_coat_weight(this.Ptr);
                }
            case "ior":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ior', 'ui_name': 'IOR'} */
                {
                    return CSycles.principledbsdfnode_get_ior(this.Ptr);
                }
            case "coat_roughness":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.03f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'coat_roughness', 'ui_name': 'Coat Roughness'} */
                {
                    return CSycles.principledbsdfnode_get_coat_roughness(this.Ptr);
                }
            case "alpha":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha', 'ui_name': 'Alpha'} */
                {
                    return CSycles.principledbsdfnode_get_alpha(this.Ptr);
                }
            case "coat_ior":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'coat_ior', 'ui_name': 'Coat IOR'} */
                {
                    return CSycles.principledbsdfnode_get_coat_ior(this.Ptr);
                }
            case "diffuse_roughness":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'diffuse_roughness', 'ui_name': 'Diffuse Roughness'} */
                {
                    return CSycles.principledbsdfnode_get_diffuse_roughness(this.Ptr);
                }
            case "subsurface_weight":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_weight', 'ui_name': 'Subsurface Weight'} */
                {
                    return CSycles.principledbsdfnode_get_subsurface_weight(this.Ptr);
                }
            case "subsurface_scale":
                /* principledbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_scale', 'ui_name': 'Subsurface Scale'} */
                {
                    return CSycles.principledbsdfnode_get_subsurface_scale(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "subsurface_radius":
                /* principledbsdfnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.1f,0.1f,0.1f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'subsurface_radius', 'ui_name': 'Subsurface Radius'} */
                {
                    return CSycles.principledbsdfnode_get_subsurface_radius(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "tangent":
                /* principledbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'tangent', 'ui_name': 'Tangent'} */
                {
                    return CSycles.principledbsdfnode_get_tangent(this.Ptr);
                }
            case "normal":
                /* principledbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.principledbsdfnode_get_normal(this.Ptr);
                }
            case "coat_normal":
                /* principledbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'coat_normal', 'ui_name': 'Coat Normal'} */
                {
                    return CSycles.principledbsdfnode_get_coat_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "emission_color":
                /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'emission_color', 'ui_name': 'Emission Color'} */
                {
                    return CSycles.principledbsdfnode_get_emission_color(this.Ptr);
                }
            case "specular_tint":
                /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'specular_tint', 'ui_name': 'Specular Tint'} */
                {
                    return CSycles.principledbsdfnode_get_specular_tint(this.Ptr);
                }
            case "base_color":
                /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'base_color', 'ui_name': 'Base Color'} */
                {
                    return CSycles.principledbsdfnode_get_base_color(this.Ptr);
                }
            case "sheen_tint":
                /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'sheen_tint', 'ui_name': 'Sheen Tint'} */
                {
                    return CSycles.principledbsdfnode_get_sheen_tint(this.Ptr);
                }
            case "coat_tint":
                /* principledbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'coat_tint', 'ui_name': 'Coat Tint'} */
                {
                    return CSycles.principledbsdfnode_get_coat_tint(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "distribution":
                /* principledbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_MICROFACET_MULTI_GGX_GLASS_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'distribution', 'ui_name': 'Distribution'} */
                {
                    return (uint)CSycles.principledbsdfnode_get_distribution(this.Ptr);
                }
            case "subsurface_method":
                /* principledbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSSRDF_RANDOM_WALK_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'subsurface_method', 'ui_name': 'Subsurface Method'} */
                {
                    return (uint)CSycles.principledbsdfnode_get_subsurface_method(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledBsdfNode (getter)");
            }
        }

#endregion
    }

}