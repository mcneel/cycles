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

    public class RhinoPerlinMarbleTextureNodeInputs : Inputs
    {
        public FloatSocket Color1Saturation { get; private set; }
        public IntSocket Levels { get; private set; }
        public FloatSocket Color2Saturation { get; private set; }
        public FloatSocket Noise { get; private set; }
        public VectorSocket UVW { get; private set; }
        public FloatSocket Blur { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public FloatSocket Size { get; private set; }
        public ColorSocket Color2 { get; private set; }

        public RhinoPerlinMarbleTextureNodeInputs(ShaderNode parentNode)
        {
            Color1Saturation = new FloatSocket(parentNode, "Color1Saturation", "color1_sat", true);
            AddSocket(Color1Saturation);
            Levels = new IntSocket(parentNode, "Levels", "levels", true);
            AddSocket(Levels);
            Color2Saturation = new FloatSocket(parentNode, "Color2Saturation", "color2_sat", true);
            AddSocket(Color2Saturation);
            Noise = new FloatSocket(parentNode, "Noise", "noise_amount", true);
            AddSocket(Noise);
            UVW = new VectorSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            Blur = new FloatSocket(parentNode, "Blur", "blur", true);
            AddSocket(Blur);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            Size = new FloatSocket(parentNode, "Size", "size", true);
            AddSocket(Size);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
        }
    }
    public class RhinoPerlinMarbleTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoPerlinMarbleTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "out_alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_perlin_marble_texture")]
    public class RhinoPerlinMarbleTextureNode : ShaderNode
    {
        public RhinoPerlinMarbleTextureNodeInputs ins => (RhinoPerlinMarbleTextureNodeInputs)inputs;
        public RhinoPerlinMarbleTextureNodeOutputs outs => (RhinoPerlinMarbleTextureNodeOutputs)outputs;
        public RhinoPerlinMarbleTextureNode(Shader shader) : this(shader, "a rhino_perlin_marble_texture node") { }

        public RhinoPerlinMarbleTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoPerlinMarbleTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoPerlinMarbleTextureNodeInputs(this);
            outputs = new RhinoPerlinMarbleTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinoperlinmarbletexturenode_get_uvw(Ptr); }
            set { CSycles.rhinoperlinmarbletexturenode_set_uvw(Ptr, value); }
        }

        public int Levels {
            get { return CSycles.rhinoperlinmarbletexturenode_get_levels(Ptr); }
            set { CSycles.rhinoperlinmarbletexturenode_set_levels(Ptr, value); }
        }

        public float Size {
            get { return CSycles.rhinoperlinmarbletexturenode_get_size(Ptr); }
            set { CSycles.rhinoperlinmarbletexturenode_set_size(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinoperlinmarbletexturenode_get_color2(Ptr); }
            set { CSycles.rhinoperlinmarbletexturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinoperlinmarbletexturenode_get_node_type();
        }

        public float Blur {
            get { return CSycles.rhinoperlinmarbletexturenode_get_blur(Ptr); }
            set { CSycles.rhinoperlinmarbletexturenode_set_blur(Ptr, value); }
        }

        public float3 Color1 {
            get { return CSycles.rhinoperlinmarbletexturenode_get_color1(Ptr); }
            set { CSycles.rhinoperlinmarbletexturenode_set_color1(Ptr, value); }
        }

        public float Color2Sat {
            get { return CSycles.rhinoperlinmarbletexturenode_get_color2_sat(Ptr); }
            set { CSycles.rhinoperlinmarbletexturenode_set_color2_sat(Ptr, value); }
        }

        public float NoiseAmount {
            get { return CSycles.rhinoperlinmarbletexturenode_get_noise_amount(Ptr); }
            set { CSycles.rhinoperlinmarbletexturenode_set_noise_amount(Ptr, value); }
        }

        public float Color1Sat {
            get { return CSycles.rhinoperlinmarbletexturenode_get_color1_sat(Ptr); }
            set { CSycles.rhinoperlinmarbletexturenode_set_color1_sat(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "color1_sat":
                    /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'color1_sat', 'ui_name': 'Color1Saturation'} */
                    {
                    CSycles.rhinoperlinmarbletexturenode_set_color1_sat(this.Ptr, data);
                    }
                    break;
            case "color2_sat":
                    /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'color2_sat', 'ui_name': 'Color2Saturation'} */
                    {
                    CSycles.rhinoperlinmarbletexturenode_set_color2_sat(this.Ptr, data);
                    }
                    break;
            case "noise_amount":
                    /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'noise_amount', 'ui_name': 'Noise'} */
                    {
                    CSycles.rhinoperlinmarbletexturenode_set_noise_amount(this.Ptr, data);
                    }
                    break;
            case "blur":
                    /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'blur', 'ui_name': 'Blur'} */
                    {
                    CSycles.rhinoperlinmarbletexturenode_set_blur(this.Ptr, data);
                    }
                    break;
            case "size":
                    /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'size', 'ui_name': 'Size'} */
                    {
                    CSycles.rhinoperlinmarbletexturenode_set_size(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerlinMarbleTextureNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinoperlinmarbletexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinoperlinmarbletexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerlinMarbleTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color1":
                    /* rhinoperlinmarbletexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinoperlinmarbletexturenode_set_color1(this.Ptr, data);
                    }
                    break;
            case "color2":
                    /* rhinoperlinmarbletexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinoperlinmarbletexturenode_set_color2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerlinMarbleTextureNode (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "levels":
                    /* rhinoperlinmarbletexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'levels', 'ui_name': 'Levels'} */
                    {
                    CSycles.rhinoperlinmarbletexturenode_set_levels(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerlinMarbleTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "color1_sat":
                /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'color1_sat', 'ui_name': 'Color1Saturation'} */
                {
                    return CSycles.rhinoperlinmarbletexturenode_get_color1_sat(this.Ptr);
                }
            case "color2_sat":
                /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'color2_sat', 'ui_name': 'Color2Saturation'} */
                {
                    return CSycles.rhinoperlinmarbletexturenode_get_color2_sat(this.Ptr);
                }
            case "noise_amount":
                /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'noise_amount', 'ui_name': 'Noise'} */
                {
                    return CSycles.rhinoperlinmarbletexturenode_get_noise_amount(this.Ptr);
                }
            case "blur":
                /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'blur', 'ui_name': 'Blur'} */
                {
                    return CSycles.rhinoperlinmarbletexturenode_get_blur(this.Ptr);
                }
            case "size":
                /* rhinoperlinmarbletexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'size', 'ui_name': 'Size'} */
                {
                    return CSycles.rhinoperlinmarbletexturenode_get_size(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerlinMarbleTextureNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinoperlinmarbletexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinoperlinmarbletexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerlinMarbleTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color1":
                /* rhinoperlinmarbletexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinoperlinmarbletexturenode_get_color1(this.Ptr);
                }
            case "color2":
                /* rhinoperlinmarbletexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinoperlinmarbletexturenode_get_color2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerlinMarbleTextureNode (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "levels":
                /* rhinoperlinmarbletexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'levels', 'ui_name': 'Levels'} */
                {
                    return CSycles.rhinoperlinmarbletexturenode_get_levels(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerlinMarbleTextureNode (getter)");
            }
        }

#endregion
    }

}