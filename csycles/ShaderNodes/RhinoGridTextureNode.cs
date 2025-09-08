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

    public class RhinoGridTextureNodeInputs : Inputs
    {
        public ColorSocket Color2 { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public FloatSocket FontThickness { get; private set; }
        public FloatSocket Alpha2 { get; private set; }
        public FloatSocket Alpha1 { get; private set; }
        public VectorSocket UVW { get; private set; }
        public IntSocket Cells { get; private set; }

        public RhinoGridTextureNodeInputs(ShaderNode parentNode)
        {
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            FontThickness = new FloatSocket(parentNode, "FontThickness", "font_thickness", true);
            AddSocket(FontThickness);
            Alpha2 = new FloatSocket(parentNode, "Alpha2", "alpha2", true);
            AddSocket(Alpha2);
            Alpha1 = new FloatSocket(parentNode, "Alpha1", "alpha1", true);
            AddSocket(Alpha1);
            UVW = new VectorSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            Cells = new IntSocket(parentNode, "Cells", "cells", true);
            AddSocket(Cells);
        }
    }
    public class RhinoGridTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoGridTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "out_alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_grid_texture")]
    public class RhinoGridTextureNode : ShaderNode
    {
        public RhinoGridTextureNodeInputs ins => (RhinoGridTextureNodeInputs)inputs;
        public RhinoGridTextureNodeOutputs outs => (RhinoGridTextureNodeOutputs)outputs;
        public RhinoGridTextureNode(Shader shader) : this(shader, "a rhino_grid_texture node") { }

        public RhinoGridTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoGridTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoGridTextureNodeInputs(this);
            outputs = new RhinoGridTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinogridtexturenode_get_uvw(Ptr); }
            set { CSycles.rhinogridtexturenode_set_uvw(Ptr, value); }
        }

        public float Alpha1 {
            get { return CSycles.rhinogridtexturenode_get_alpha1(Ptr); }
            set { CSycles.rhinogridtexturenode_set_alpha1(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinogridtexturenode_get_color2(Ptr); }
            set { CSycles.rhinogridtexturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinogridtexturenode_get_node_type();
        }

        public float3 Color1 {
            get { return CSycles.rhinogridtexturenode_get_color1(Ptr); }
            set { CSycles.rhinogridtexturenode_set_color1(Ptr, value); }
        }

        public float Alpha2 {
            get { return CSycles.rhinogridtexturenode_get_alpha2(Ptr); }
            set { CSycles.rhinogridtexturenode_set_alpha2(Ptr, value); }
        }

        public int Cells {
            get { return CSycles.rhinogridtexturenode_get_cells(Ptr); }
            set { CSycles.rhinogridtexturenode_set_cells(Ptr, value); }
        }

        public float FontThickness {
            get { return CSycles.rhinogridtexturenode_get_font_thickness(Ptr); }
            set { CSycles.rhinogridtexturenode_set_font_thickness(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "font_thickness":
                    /* rhinogridtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'font_thickness', 'ui_name': 'FontThickness'} */
                    {
                    CSycles.rhinogridtexturenode_set_font_thickness(this.Ptr, data);
                    }
                    break;
            case "alpha2":
                    /* rhinogridtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                    {
                    CSycles.rhinogridtexturenode_set_alpha2(this.Ptr, data);
                    }
                    break;
            case "alpha1":
                    /* rhinogridtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                    {
                    CSycles.rhinogridtexturenode_set_alpha1(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGridTextureNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinogridtexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinogridtexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGridTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color2":
                    /* rhinogridtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinogridtexturenode_set_color2(this.Ptr, data);
                    }
                    break;
            case "color1":
                    /* rhinogridtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinogridtexturenode_set_color1(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGridTextureNode (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "cells":
                    /* rhinogridtexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'cells', 'ui_name': 'Cells'} */
                    {
                    CSycles.rhinogridtexturenode_set_cells(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGridTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "font_thickness":
                /* rhinogridtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'font_thickness', 'ui_name': 'FontThickness'} */
                {
                    return CSycles.rhinogridtexturenode_get_font_thickness(this.Ptr);
                }
            case "alpha2":
                /* rhinogridtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                {
                    return CSycles.rhinogridtexturenode_get_alpha2(this.Ptr);
                }
            case "alpha1":
                /* rhinogridtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                {
                    return CSycles.rhinogridtexturenode_get_alpha1(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGridTextureNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinogridtexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinogridtexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGridTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color2":
                /* rhinogridtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinogridtexturenode_get_color2(this.Ptr);
                }
            case "color1":
                /* rhinogridtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinogridtexturenode_get_color1(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGridTextureNode (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "cells":
                /* rhinogridtexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'cells', 'ui_name': 'Cells'} */
                {
                    return CSycles.rhinogridtexturenode_get_cells(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGridTextureNode (getter)");
            }
        }

#endregion
    }

}