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

    public class RhinoNoiseTextureNodeInputs : Inputs
    {
        public EnumSocket NoiseType { get; private set; }
        public BoolSocket Inverse { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public FloatSocket AmplitudeMultiplier { get; private set; }
        public EnumSocket SpecSynthType { get; private set; }
        public FloatSocket Gain { get; private set; }
        public FloatSocket Alpha1 { get; private set; }
        public FloatSocket ClampMin { get; private set; }
        public IntSocket OctaveCount { get; private set; }
        public ColorSocket Color2 { get; private set; }
        public FloatSocket ClampMax { get; private set; }
        public FloatSocket FrequencyMultiplier { get; private set; }
        public FloatSocket Alpha2 { get; private set; }
        public BoolSocket ScaleToClamp { get; private set; }
        public PointSocket UVW { get; private set; }

        public RhinoNoiseTextureNodeInputs(ShaderNode parentNode)
        {
            NoiseType = new EnumSocket(parentNode, "NoiseType", "noise_type", true);
            AddSocket(NoiseType);
            Inverse = new BoolSocket(parentNode, "Inverse", "inverse", true);
            AddSocket(Inverse);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            AmplitudeMultiplier = new FloatSocket(parentNode, "AmplitudeMultiplier", "amplitude_multiplier", true);
            AddSocket(AmplitudeMultiplier);
            SpecSynthType = new EnumSocket(parentNode, "SpecSynthType", "spec_synth_type", true);
            AddSocket(SpecSynthType);
            Gain = new FloatSocket(parentNode, "Gain", "gain", true);
            AddSocket(Gain);
            Alpha1 = new FloatSocket(parentNode, "Alpha1", "alpha1", true);
            AddSocket(Alpha1);
            ClampMin = new FloatSocket(parentNode, "ClampMin", "clamp_min", true);
            AddSocket(ClampMin);
            OctaveCount = new IntSocket(parentNode, "OctaveCount", "octave_count", true);
            AddSocket(OctaveCount);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            ClampMax = new FloatSocket(parentNode, "ClampMax", "clamp_max", true);
            AddSocket(ClampMax);
            FrequencyMultiplier = new FloatSocket(parentNode, "FrequencyMultiplier", "frequency_multiplier", true);
            AddSocket(FrequencyMultiplier);
            Alpha2 = new FloatSocket(parentNode, "Alpha2", "alpha2", true);
            AddSocket(Alpha2);
            ScaleToClamp = new BoolSocket(parentNode, "ScaleToClamp", "scale_to_clamp", true);
            AddSocket(ScaleToClamp);
            UVW = new PointSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
        }
    }
    public class RhinoNoiseTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoNoiseTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_noise_texture")]
    public class RhinoNoiseTextureNode : ShaderNode
    {
        public enum RhinoNoiseTextureNodeNoiseType : uint {
            Perlin = ccl.RhinoProceduralNoiseType.RHINO_NOISE_PERLIN,
            ValueNoise = ccl.RhinoProceduralNoiseType.RHINO_NOISE_VALUE_NOISE,
            PerlinPlusValue = ccl.RhinoProceduralNoiseType.RHINO_NOISE_PERLIN_PLUS_VALUE,
            Simplex = ccl.RhinoProceduralNoiseType.RHINO_NOISE_SIMPLEX,
            SparseConvolution = ccl.RhinoProceduralNoiseType.RHINO_NOISE_SPARSE_CONVOLUTION,
            LatticeConvolution = ccl.RhinoProceduralNoiseType.RHINO_NOISE_LATTICE_CONVOLUTION,
            WardsHermite = ccl.RhinoProceduralNoiseType.RHINO_NOISE_WARDS_HERMITE,
            Aaltonen = ccl.RhinoProceduralNoiseType.RHINO_NOISE_AALTONEN,
        }
        public enum RhinoNoiseTextureNodeSpecSynthType : uint {
            FractalSum = ccl.RhinoProceduralSpecSynthType.RHINO_SPEC_SYNTH_FRACTAL_SUM,
            Turbulence = ccl.RhinoProceduralSpecSynthType.RHINO_SPEC_SYNTH_TURBULENCE,
        }
        public RhinoNoiseTextureNodeInputs ins => (RhinoNoiseTextureNodeInputs)inputs;
        public RhinoNoiseTextureNodeOutputs outs => (RhinoNoiseTextureNodeOutputs)outputs;
        public RhinoNoiseTextureNode(Shader shader) : this(shader, "a rhino_noise_texture node") { }

        public RhinoNoiseTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoNoiseTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoNoiseTextureNodeInputs(this);
            outputs = new RhinoNoiseTextureNodeOutputs(this);
        }
        public float ClampMax {
            get { return CSycles.rhinonoisetexturenode_get_clamp_max(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_clamp_max(Ptr, value); }
        }

        public float ClampMin {
            get { return CSycles.rhinonoisetexturenode_get_clamp_min(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_clamp_min(Ptr, value); }
        }

        public float Gain {
            get { return CSycles.rhinonoisetexturenode_get_gain(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_gain(Ptr, value); }
        }

        public bool ScaleToClamp {
            get { return CSycles.rhinonoisetexturenode_get_scale_to_clamp(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_scale_to_clamp(Ptr, value); }
        }

        public bool Inverse {
            get { return CSycles.rhinonoisetexturenode_get_inverse(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_inverse(Ptr, value); }
        }

        public RhinoProceduralNoiseType NoiseType {
            get { return CSycles.rhinonoisetexturenode_get_noise_type(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_noise_type(Ptr, value); }
        }

        public float3 Uvw {
            get { return CSycles.rhinonoisetexturenode_get_uvw(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_uvw(Ptr, value); }
        }

        public float AmplitudeMultiplier {
            get { return CSycles.rhinonoisetexturenode_get_amplitude_multiplier(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_amplitude_multiplier(Ptr, value); }
        }

        public float Alpha1 {
            get { return CSycles.rhinonoisetexturenode_get_alpha1(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_alpha1(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinonoisetexturenode_get_color2(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinonoisetexturenode_get_node_type();
        }

        public float3 Color1 {
            get { return CSycles.rhinonoisetexturenode_get_color1(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_color1(Ptr, value); }
        }

        public float Alpha2 {
            get { return CSycles.rhinonoisetexturenode_get_alpha2(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_alpha2(Ptr, value); }
        }

        public float FrequencyMultiplier {
            get { return CSycles.rhinonoisetexturenode_get_frequency_multiplier(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_frequency_multiplier(Ptr, value); }
        }

        public RhinoProceduralSpecSynthType SpecSynthType {
            get { return CSycles.rhinonoisetexturenode_get_spec_synth_type(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_spec_synth_type(Ptr, value); }
        }

        public int OctaveCount {
            get { return CSycles.rhinonoisetexturenode_get_octave_count(Ptr); }
            set { CSycles.rhinonoisetexturenode_set_octave_count(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "amplitude_multiplier":
                    /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'amplitude_multiplier', 'ui_name': 'AmplitudeMultiplier'} */
                    {
                    CSycles.rhinonoisetexturenode_set_amplitude_multiplier(this.Ptr, data);
                    }
                    break;
            case "gain":
                    /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gain', 'ui_name': 'Gain'} */
                    {
                    CSycles.rhinonoisetexturenode_set_gain(this.Ptr, data);
                    }
                    break;
            case "alpha1":
                    /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                    {
                    CSycles.rhinonoisetexturenode_set_alpha1(this.Ptr, data);
                    }
                    break;
            case "clamp_min":
                    /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'clamp_min', 'ui_name': 'ClampMin'} */
                    {
                    CSycles.rhinonoisetexturenode_set_clamp_min(this.Ptr, data);
                    }
                    break;
            case "clamp_max":
                    /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'clamp_max', 'ui_name': 'ClampMax'} */
                    {
                    CSycles.rhinonoisetexturenode_set_clamp_max(this.Ptr, data);
                    }
                    break;
            case "frequency_multiplier":
                    /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'frequency_multiplier', 'ui_name': 'FrequencyMultiplier'} */
                    {
                    CSycles.rhinonoisetexturenode_set_frequency_multiplier(this.Ptr, data);
                    }
                    break;
            case "alpha2":
                    /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                    {
                    CSycles.rhinonoisetexturenode_set_alpha2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinonoisetexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinonoisetexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color1":
                    /* rhinonoisetexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinonoisetexturenode_set_color1(this.Ptr, data);
                    }
                    break;
            case "color2":
                    /* rhinonoisetexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinonoisetexturenode_set_color2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "inverse":
                    /* rhinonoisetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'inverse', 'ui_name': 'Inverse'} */
                    {
                    CSycles.rhinonoisetexturenode_set_inverse(this.Ptr, data);
                    }
                    break;
            case "scale_to_clamp":
                    /* rhinonoisetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'scale_to_clamp', 'ui_name': 'ScaleToClamp'} */
                    {
                    CSycles.rhinonoisetexturenode_set_scale_to_clamp(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "octave_count":
                    /* rhinonoisetexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'octave_count', 'ui_name': 'OctaveCount'} */
                    {
                    CSycles.rhinonoisetexturenode_set_octave_count(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "noise_type":
                    /* rhinonoisetexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_NOISE_PERLIN', 'default_value_type': 'RhinoProceduralNoiseType', 'is_input': True, 'member_name': 'noise_type', 'ui_name': 'NoiseType'} */
                    {
                    CSycles.rhinonoisetexturenode_set_noise_type(this.Ptr, (ccl.RhinoProceduralNoiseType)data);
                    }
                    break;
            case "spec_synth_type":
                    /* rhinonoisetexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_SPEC_SYNTH_FRACTAL_SUM', 'default_value_type': 'RhinoProceduralSpecSynthType', 'is_input': True, 'member_name': 'spec_synth_type', 'ui_name': 'SpecSynthType'} */
                    {
                    CSycles.rhinonoisetexturenode_set_spec_synth_type(this.Ptr, (ccl.RhinoProceduralSpecSynthType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "amplitude_multiplier":
                /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'amplitude_multiplier', 'ui_name': 'AmplitudeMultiplier'} */
                {
                    return CSycles.rhinonoisetexturenode_get_amplitude_multiplier(this.Ptr);
                }
            case "gain":
                /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gain', 'ui_name': 'Gain'} */
                {
                    return CSycles.rhinonoisetexturenode_get_gain(this.Ptr);
                }
            case "alpha1":
                /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                {
                    return CSycles.rhinonoisetexturenode_get_alpha1(this.Ptr);
                }
            case "clamp_min":
                /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'clamp_min', 'ui_name': 'ClampMin'} */
                {
                    return CSycles.rhinonoisetexturenode_get_clamp_min(this.Ptr);
                }
            case "clamp_max":
                /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'clamp_max', 'ui_name': 'ClampMax'} */
                {
                    return CSycles.rhinonoisetexturenode_get_clamp_max(this.Ptr);
                }
            case "frequency_multiplier":
                /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'frequency_multiplier', 'ui_name': 'FrequencyMultiplier'} */
                {
                    return CSycles.rhinonoisetexturenode_get_frequency_multiplier(this.Ptr);
                }
            case "alpha2":
                /* rhinonoisetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                {
                    return CSycles.rhinonoisetexturenode_get_alpha2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinonoisetexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinonoisetexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color1":
                /* rhinonoisetexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinonoisetexturenode_get_color1(this.Ptr);
                }
            case "color2":
                /* rhinonoisetexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinonoisetexturenode_get_color2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "inverse":
                /* rhinonoisetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'inverse', 'ui_name': 'Inverse'} */
                {
                    return CSycles.rhinonoisetexturenode_get_inverse(this.Ptr);
                }
            case "scale_to_clamp":
                /* rhinonoisetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'scale_to_clamp', 'ui_name': 'ScaleToClamp'} */
                {
                    return CSycles.rhinonoisetexturenode_get_scale_to_clamp(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "octave_count":
                /* rhinonoisetexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'octave_count', 'ui_name': 'OctaveCount'} */
                {
                    return CSycles.rhinonoisetexturenode_get_octave_count(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "noise_type":
                /* rhinonoisetexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_NOISE_PERLIN', 'default_value_type': 'RhinoProceduralNoiseType', 'is_input': True, 'member_name': 'noise_type', 'ui_name': 'NoiseType'} */
                {
                    return (uint)CSycles.rhinonoisetexturenode_get_noise_type(this.Ptr);
                }
            case "spec_synth_type":
                /* rhinonoisetexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_SPEC_SYNTH_FRACTAL_SUM', 'default_value_type': 'RhinoProceduralSpecSynthType', 'is_input': True, 'member_name': 'spec_synth_type', 'ui_name': 'SpecSynthType'} */
                {
                    return (uint)CSycles.rhinonoisetexturenode_get_spec_synth_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNoiseTextureNode (getter)");
            }
        }

#endregion
    }

}