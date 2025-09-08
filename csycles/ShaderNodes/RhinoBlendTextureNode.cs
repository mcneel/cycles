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

    public class RhinoBlendTextureNodeInputs : Inputs
    {
        public FloatSocket Alpha2 { get; private set; }
        public PointSocket UVW { get; private set; }
        public ColorSocket BlendColor { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public BoolSocket UseBlendColor { get; private set; }
        public FloatSocket Alpha1 { get; private set; }
        public FloatSocket BlendFactor { get; private set; }
        public ColorSocket Color2 { get; private set; }

        public RhinoBlendTextureNodeInputs(ShaderNode parentNode)
        {
            Alpha2 = new FloatSocket(parentNode, "Alpha2", "alpha2", true);
            AddSocket(Alpha2);
            UVW = new PointSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            BlendColor = new ColorSocket(parentNode, "BlendColor", "blend_color", true);
            AddSocket(BlendColor);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            UseBlendColor = new BoolSocket(parentNode, "UseBlendColor", "use_blend_color", true);
            AddSocket(UseBlendColor);
            Alpha1 = new FloatSocket(parentNode, "Alpha1", "alpha1", true);
            AddSocket(Alpha1);
            BlendFactor = new FloatSocket(parentNode, "BlendFactor", "blend_factor", true);
            AddSocket(BlendFactor);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
        }
    }
    public class RhinoBlendTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoBlendTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "out_alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_blend_texture")]
    public class RhinoBlendTextureNode : ShaderNode
    {
        public RhinoBlendTextureNodeInputs ins => (RhinoBlendTextureNodeInputs)inputs;
        public RhinoBlendTextureNodeOutputs outs => (RhinoBlendTextureNodeOutputs)outputs;
        public RhinoBlendTextureNode(Shader shader) : this(shader, "a rhino_blend_texture node") { }

        public RhinoBlendTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoBlendTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoBlendTextureNodeInputs(this);
            outputs = new RhinoBlendTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinoblendtexturenode_get_uvw(Ptr); }
            set { CSycles.rhinoblendtexturenode_set_uvw(Ptr, value); }
        }

        public float Alpha1 {
            get { return CSycles.rhinoblendtexturenode_get_alpha1(Ptr); }
            set { CSycles.rhinoblendtexturenode_set_alpha1(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinoblendtexturenode_get_color2(Ptr); }
            set { CSycles.rhinoblendtexturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinoblendtexturenode_get_node_type();
        }

        public float3 Color1 {
            get { return CSycles.rhinoblendtexturenode_get_color1(Ptr); }
            set { CSycles.rhinoblendtexturenode_set_color1(Ptr, value); }
        }

        public bool UseBlendColor {
            get { return CSycles.rhinoblendtexturenode_get_use_blend_color(Ptr); }
            set { CSycles.rhinoblendtexturenode_set_use_blend_color(Ptr, value); }
        }

        public float Alpha2 {
            get { return CSycles.rhinoblendtexturenode_get_alpha2(Ptr); }
            set { CSycles.rhinoblendtexturenode_set_alpha2(Ptr, value); }
        }

        public float BlendFactor {
            get { return CSycles.rhinoblendtexturenode_get_blend_factor(Ptr); }
            set { CSycles.rhinoblendtexturenode_set_blend_factor(Ptr, value); }
        }

        public float3 BlendColor {
            get { return CSycles.rhinoblendtexturenode_get_blend_color(Ptr); }
            set { CSycles.rhinoblendtexturenode_set_blend_color(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "alpha2":
                    /* rhinoblendtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                    {
                    CSycles.rhinoblendtexturenode_set_alpha2(this.Ptr, data);
                    }
                    break;
            case "alpha1":
                    /* rhinoblendtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                    {
                    CSycles.rhinoblendtexturenode_set_alpha1(this.Ptr, data);
                    }
                    break;
            case "blend_factor":
                    /* rhinoblendtexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'blend_factor', 'ui_name': 'BlendFactor'} */
                    {
                    CSycles.rhinoblendtexturenode_set_blend_factor(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoBlendTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinoblendtexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinoblendtexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoBlendTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "blend_color":
                    /* rhinoblendtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'blend_color', 'ui_name': 'BlendColor'} */
                    {
                    CSycles.rhinoblendtexturenode_set_blend_color(this.Ptr, data);
                    }
                    break;
            case "color1":
                    /* rhinoblendtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinoblendtexturenode_set_color1(this.Ptr, data);
                    }
                    break;
            case "color2":
                    /* rhinoblendtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinoblendtexturenode_set_color2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoBlendTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_blend_color":
                    /* rhinoblendtexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_blend_color', 'ui_name': 'UseBlendColor'} */
                    {
                    CSycles.rhinoblendtexturenode_set_use_blend_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoBlendTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "alpha2":
                /* rhinoblendtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                {
                    return CSycles.rhinoblendtexturenode_get_alpha2(this.Ptr);
                }
            case "alpha1":
                /* rhinoblendtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                {
                    return CSycles.rhinoblendtexturenode_get_alpha1(this.Ptr);
                }
            case "blend_factor":
                /* rhinoblendtexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'blend_factor', 'ui_name': 'BlendFactor'} */
                {
                    return CSycles.rhinoblendtexturenode_get_blend_factor(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoBlendTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinoblendtexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinoblendtexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoBlendTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "blend_color":
                /* rhinoblendtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'blend_color', 'ui_name': 'BlendColor'} */
                {
                    return CSycles.rhinoblendtexturenode_get_blend_color(this.Ptr);
                }
            case "color1":
                /* rhinoblendtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinoblendtexturenode_get_color1(this.Ptr);
                }
            case "color2":
                /* rhinoblendtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinoblendtexturenode_get_color2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoBlendTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_blend_color":
                /* rhinoblendtexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_blend_color', 'ui_name': 'UseBlendColor'} */
                {
                    return CSycles.rhinoblendtexturenode_get_use_blend_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoBlendTextureNode (getter)");
            }
        }

#endregion
    }

}