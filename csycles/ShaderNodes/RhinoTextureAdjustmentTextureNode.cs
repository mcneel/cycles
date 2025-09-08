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

    public class RhinoTextureAdjustmentTextureNodeInputs : Inputs
    {
        public FloatSocket ClampMax { get; private set; }
        public BoolSocket IsHdr { get; private set; }
        public BoolSocket Clamp { get; private set; }
        public FloatSocket Gain { get; private set; }
        public BoolSocket ScaleToClamp { get; private set; }
        public FloatSocket Gamma { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket Multiplier { get; private set; }
        public FloatSocket Saturation { get; private set; }
        public BoolSocket Grayscale { get; private set; }
        public FloatSocket ClampMin { get; private set; }
        public FloatSocket HueShift { get; private set; }
        public BoolSocket Invert { get; private set; }

        public RhinoTextureAdjustmentTextureNodeInputs(ShaderNode parentNode)
        {
            ClampMax = new FloatSocket(parentNode, "ClampMax", "clamp_max", true);
            AddSocket(ClampMax);
            IsHdr = new BoolSocket(parentNode, "IsHdr", "is_hdr", true);
            AddSocket(IsHdr);
            Clamp = new BoolSocket(parentNode, "Clamp", "clamp", true);
            AddSocket(Clamp);
            Gain = new FloatSocket(parentNode, "Gain", "gain", true);
            AddSocket(Gain);
            ScaleToClamp = new BoolSocket(parentNode, "ScaleToClamp", "scale_to_clamp", true);
            AddSocket(ScaleToClamp);
            Gamma = new FloatSocket(parentNode, "Gamma", "gamma", true);
            AddSocket(Gamma);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Multiplier = new FloatSocket(parentNode, "Multiplier", "multiplier", true);
            AddSocket(Multiplier);
            Saturation = new FloatSocket(parentNode, "Saturation", "saturation", true);
            AddSocket(Saturation);
            Grayscale = new BoolSocket(parentNode, "Grayscale", "grayscale", true);
            AddSocket(Grayscale);
            ClampMin = new FloatSocket(parentNode, "ClampMin", "clamp_min", true);
            AddSocket(ClampMin);
            HueShift = new FloatSocket(parentNode, "HueShift", "hue_shift", true);
            AddSocket(HueShift);
            Invert = new BoolSocket(parentNode, "Invert", "invert", true);
            AddSocket(Invert);
        }
    }
    public class RhinoTextureAdjustmentTextureNodeOutputs : Outputs
    {
        public VectorSocket Color { get; private set; }

        public RhinoTextureAdjustmentTextureNodeOutputs(ShaderNode parentNode)
        {
            Color = new VectorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_texture_adjustment_texture")]
    public class RhinoTextureAdjustmentTextureNode : ShaderNode
    {
        public RhinoTextureAdjustmentTextureNodeInputs ins => (RhinoTextureAdjustmentTextureNodeInputs)inputs;
        public RhinoTextureAdjustmentTextureNodeOutputs outs => (RhinoTextureAdjustmentTextureNodeOutputs)outputs;
        public RhinoTextureAdjustmentTextureNode(Shader shader) : this(shader, "a rhino_texture_adjustment_texture node") { }

        public RhinoTextureAdjustmentTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoTextureAdjustmentTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoTextureAdjustmentTextureNodeInputs(this);
            outputs = new RhinoTextureAdjustmentTextureNodeOutputs(this);
        }
        public float3 Color {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_color(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_color(Ptr, value); }
        }

        public bool Clamp {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_clamp(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_clamp(Ptr, value); }
        }

        public float Multiplier {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_multiplier(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_multiplier(Ptr, value); }
        }

        public float ClampMax {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_clamp_max(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_clamp_max(Ptr, value); }
        }

        public float ClampMin {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_clamp_min(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_clamp_min(Ptr, value); }
        }

        public float Gain {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_gain(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_gain(Ptr, value); }
        }

        public bool ScaleToClamp {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_scale_to_clamp(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_scale_to_clamp(Ptr, value); }
        }

        public float HueShift {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_hue_shift(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_hue_shift(Ptr, value); }
        }

        public float Saturation {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_saturation(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_saturation(Ptr, value); }
        }

        public bool IsHdr {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_is_hdr(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_is_hdr(Ptr, value); }
        }

        public bool Invert {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_invert(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_invert(Ptr, value); }
        }

        public bool Grayscale {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_grayscale(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_grayscale(Ptr, value); }
        }

        public float Gamma {
            get { return CSycles.rhinotextureadjustmenttexturenode_get_gamma(Ptr); }
            set { CSycles.rhinotextureadjustmenttexturenode_set_gamma(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinotextureadjustmenttexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "clamp_max":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'clamp_max', 'ui_name': 'ClampMax'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_clamp_max(this.Ptr, data);
                    }
                    break;
            case "gain":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gain', 'ui_name': 'Gain'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_gain(this.Ptr, data);
                    }
                    break;
            case "gamma":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gamma', 'ui_name': 'Gamma'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_gamma(this.Ptr, data);
                    }
                    break;
            case "multiplier":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'multiplier', 'ui_name': 'Multiplier'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_multiplier(this.Ptr, data);
                    }
                    break;
            case "saturation":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'saturation', 'ui_name': 'Saturation'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_saturation(this.Ptr, data);
                    }
                    break;
            case "clamp_min":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'clamp_min', 'ui_name': 'ClampMin'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_clamp_min(this.Ptr, data);
                    }
                    break;
            case "hue_shift":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'hue_shift', 'ui_name': 'HueShift'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_hue_shift(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureAdjustmentTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureAdjustmentTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "is_hdr":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_hdr', 'ui_name': 'IsHdr'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_is_hdr(this.Ptr, data);
                    }
                    break;
            case "clamp":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'clamp', 'ui_name': 'Clamp'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_clamp(this.Ptr, data);
                    }
                    break;
            case "scale_to_clamp":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'scale_to_clamp', 'ui_name': 'ScaleToClamp'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_scale_to_clamp(this.Ptr, data);
                    }
                    break;
            case "grayscale":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'grayscale', 'ui_name': 'Grayscale'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_grayscale(this.Ptr, data);
                    }
                    break;
            case "invert":
                    /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'invert', 'ui_name': 'Invert'} */
                    {
                    CSycles.rhinotextureadjustmenttexturenode_set_invert(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureAdjustmentTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "clamp_max":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'clamp_max', 'ui_name': 'ClampMax'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_clamp_max(this.Ptr);
                }
            case "gain":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gain', 'ui_name': 'Gain'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_gain(this.Ptr);
                }
            case "gamma":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gamma', 'ui_name': 'Gamma'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_gamma(this.Ptr);
                }
            case "multiplier":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'multiplier', 'ui_name': 'Multiplier'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_multiplier(this.Ptr);
                }
            case "saturation":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'saturation', 'ui_name': 'Saturation'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_saturation(this.Ptr);
                }
            case "clamp_min":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'clamp_min', 'ui_name': 'ClampMin'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_clamp_min(this.Ptr);
                }
            case "hue_shift":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'hue_shift', 'ui_name': 'HueShift'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_hue_shift(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureAdjustmentTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureAdjustmentTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "is_hdr":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_hdr', 'ui_name': 'IsHdr'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_is_hdr(this.Ptr);
                }
            case "clamp":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'clamp', 'ui_name': 'Clamp'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_clamp(this.Ptr);
                }
            case "scale_to_clamp":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'scale_to_clamp', 'ui_name': 'ScaleToClamp'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_scale_to_clamp(this.Ptr);
                }
            case "grayscale":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'grayscale', 'ui_name': 'Grayscale'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_grayscale(this.Ptr);
                }
            case "invert":
                /* rhinotextureadjustmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'invert', 'ui_name': 'Invert'} */
                {
                    return CSycles.rhinotextureadjustmenttexturenode_get_invert(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureAdjustmentTextureNode (getter)");
            }
        }

#endregion
    }

}