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

    public class BrickTextureNodeInputs : Inputs
    {
        public ColorSocket Color2 { get; private set; }
        public ColorSocket Mortar { get; private set; }
        public FloatSocket Scale { get; private set; }
        public FloatSocket MortarSize { get; private set; }
        public FloatSocket Offset { get; private set; }
        public FloatSocket MortarSmooth { get; private set; }
        public IntSocket OffsetFrequency { get; private set; }
        public FloatSocket Bias { get; private set; }
        public FloatSocket Squash { get; private set; }
        public FloatSocket BrickWidth { get; private set; }
        public IntSocket SquashFrequency { get; private set; }
        public FloatSocket RowHeight { get; private set; }
        public PointSocket Vector { get; private set; }
        public ColorSocket Color1 { get; private set; }

        public BrickTextureNodeInputs(ShaderNode parentNode)
        {
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            Mortar = new ColorSocket(parentNode, "Mortar", "mortar", true);
            AddSocket(Mortar);
            Scale = new FloatSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
            MortarSize = new FloatSocket(parentNode, "Mortar Size", "mortar_size", true);
            AddSocket(MortarSize);
            Offset = new FloatSocket(parentNode, "Offset", "offset", true);
            AddSocket(Offset);
            MortarSmooth = new FloatSocket(parentNode, "Mortar Smooth", "mortar_smooth", true);
            AddSocket(MortarSmooth);
            OffsetFrequency = new IntSocket(parentNode, "Offset Frequency", "offset_frequency", true);
            AddSocket(OffsetFrequency);
            Bias = new FloatSocket(parentNode, "Bias", "bias", true);
            AddSocket(Bias);
            Squash = new FloatSocket(parentNode, "Squash", "squash", true);
            AddSocket(Squash);
            BrickWidth = new FloatSocket(parentNode, "Brick Width", "brick_width", true);
            AddSocket(BrickWidth);
            SquashFrequency = new IntSocket(parentNode, "Squash Frequency", "squash_frequency", true);
            AddSocket(SquashFrequency);
            RowHeight = new FloatSocket(parentNode, "Row Height", "row_height", true);
            AddSocket(RowHeight);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
        }
    }
    public class BrickTextureNodeOutputs : Outputs
    {
        public FloatSocket Fac { get; private set; }
        public ColorSocket Color { get; private set; }

        public BrickTextureNodeOutputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", false);
            AddSocket(Fac);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "brick_texture")]
    public class BrickTextureNode : TextureNode
    {
        public enum BrickTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum BrickTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum BrickTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public BrickTextureNodeInputs ins => (BrickTextureNodeInputs)inputs;
        public BrickTextureNodeOutputs outs => (BrickTextureNodeOutputs)outputs;
        public BrickTextureNode(Shader shader) : this(shader, "a brick_texture node") { }

        public BrickTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal BrickTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new BrickTextureNodeInputs(this);
            outputs = new BrickTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.bricktexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "scale":
                    /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.bricktexturenode_set_scale(this.Ptr, data);
                    }
                    break;
            case "mortar_size":
                    /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.02f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mortar_size', 'ui_name': 'Mortar Size'} */
                    {
                    CSycles.bricktexturenode_set_mortar_size(this.Ptr, data);
                    }
                    break;
            case "offset":
                    /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'offset', 'ui_name': 'Offset'} */
                    {
                    CSycles.bricktexturenode_set_offset(this.Ptr, data);
                    }
                    break;
            case "mortar_smooth":
                    /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mortar_smooth', 'ui_name': 'Mortar Smooth'} */
                    {
                    CSycles.bricktexturenode_set_mortar_smooth(this.Ptr, data);
                    }
                    break;
            case "bias":
                    /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'bias', 'ui_name': 'Bias'} */
                    {
                    CSycles.bricktexturenode_set_bias(this.Ptr, data);
                    }
                    break;
            case "squash":
                    /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'squash', 'ui_name': 'Squash'} */
                    {
                    CSycles.bricktexturenode_set_squash(this.Ptr, data);
                    }
                    break;
            case "brick_width":
                    /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'brick_width', 'ui_name': 'Brick Width'} */
                    {
                    CSycles.bricktexturenode_set_brick_width(this.Ptr, data);
                    }
                    break;
            case "row_height":
                    /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.25f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'row_height', 'ui_name': 'Row Height'} */
                    {
                    CSycles.bricktexturenode_set_row_height(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrickTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* bricktexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.bricktexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrickTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color2":
                    /* bricktexturenode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.bricktexturenode_set_color2(this.Ptr, data);
                    }
                    break;
            case "mortar":
                    /* bricktexturenode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'mortar', 'ui_name': 'Mortar'} */
                    {
                    CSycles.bricktexturenode_set_mortar(this.Ptr, data);
                    }
                    break;
            case "color1":
                    /* bricktexturenode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.bricktexturenode_set_color1(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrickTextureNode (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "offset_frequency":
                    /* bricktexturenode . {'datatype': 'INT', 'default_value': '2', 'default_value_type': 'int', 'is_input': True, 'member_name': 'offset_frequency', 'ui_name': 'Offset Frequency'} */
                    {
                    CSycles.bricktexturenode_set_offset_frequency(this.Ptr, data);
                    }
                    break;
            case "squash_frequency":
                    /* bricktexturenode . {'datatype': 'INT', 'default_value': '2', 'default_value_type': 'int', 'is_input': True, 'member_name': 'squash_frequency', 'ui_name': 'Squash Frequency'} */
                    {
                    CSycles.bricktexturenode_set_squash_frequency(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrickTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "scale":
                /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.bricktexturenode_get_scale(this.Ptr);
                }
            case "mortar_size":
                /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.02f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mortar_size', 'ui_name': 'Mortar Size'} */
                {
                    return CSycles.bricktexturenode_get_mortar_size(this.Ptr);
                }
            case "offset":
                /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'offset', 'ui_name': 'Offset'} */
                {
                    return CSycles.bricktexturenode_get_offset(this.Ptr);
                }
            case "mortar_smooth":
                /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mortar_smooth', 'ui_name': 'Mortar Smooth'} */
                {
                    return CSycles.bricktexturenode_get_mortar_smooth(this.Ptr);
                }
            case "bias":
                /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'bias', 'ui_name': 'Bias'} */
                {
                    return CSycles.bricktexturenode_get_bias(this.Ptr);
                }
            case "squash":
                /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'squash', 'ui_name': 'Squash'} */
                {
                    return CSycles.bricktexturenode_get_squash(this.Ptr);
                }
            case "brick_width":
                /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'brick_width', 'ui_name': 'Brick Width'} */
                {
                    return CSycles.bricktexturenode_get_brick_width(this.Ptr);
                }
            case "row_height":
                /* bricktexturenode . {'datatype': 'FLOAT', 'default_value': '0.25f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'row_height', 'ui_name': 'Row Height'} */
                {
                    return CSycles.bricktexturenode_get_row_height(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrickTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* bricktexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.bricktexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrickTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color2":
                /* bricktexturenode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.bricktexturenode_get_color2(this.Ptr);
                }
            case "mortar":
                /* bricktexturenode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'mortar', 'ui_name': 'Mortar'} */
                {
                    return CSycles.bricktexturenode_get_mortar(this.Ptr);
                }
            case "color1":
                /* bricktexturenode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.bricktexturenode_get_color1(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrickTextureNode (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "offset_frequency":
                /* bricktexturenode . {'datatype': 'INT', 'default_value': '2', 'default_value_type': 'int', 'is_input': True, 'member_name': 'offset_frequency', 'ui_name': 'Offset Frequency'} */
                {
                    return CSycles.bricktexturenode_get_offset_frequency(this.Ptr);
                }
            case "squash_frequency":
                /* bricktexturenode . {'datatype': 'INT', 'default_value': '2', 'default_value_type': 'int', 'is_input': True, 'member_name': 'squash_frequency', 'ui_name': 'Squash Frequency'} */
                {
                    return CSycles.bricktexturenode_get_squash_frequency(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrickTextureNode (getter)");
            }
        }

#endregion
    }

}