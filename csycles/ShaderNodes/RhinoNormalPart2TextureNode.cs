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

    public class RhinoNormalPart2TextureNodeInputs : Inputs
    {
        public ColorSocket Color7 { get; private set; }
        public ColorSocket Color3 { get; private set; }
        public ColorSocket Color8 { get; private set; }
        public ColorSocket Color4 { get; private set; }
        public ColorSocket Color5 { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public ColorSocket Color6 { get; private set; }
        public ColorSocket Color2 { get; private set; }

        public RhinoNormalPart2TextureNodeInputs(ShaderNode parentNode)
        {
            Color7 = new ColorSocket(parentNode, "Color7", "color7", true);
            AddSocket(Color7);
            Color3 = new ColorSocket(parentNode, "Color3", "color3", true);
            AddSocket(Color3);
            Color8 = new ColorSocket(parentNode, "Color8", "color8", true);
            AddSocket(Color8);
            Color4 = new ColorSocket(parentNode, "Color4", "color4", true);
            AddSocket(Color4);
            Color5 = new ColorSocket(parentNode, "Color5", "color5", true);
            AddSocket(Color5);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            Color6 = new ColorSocket(parentNode, "Color6", "color6", true);
            AddSocket(Color6);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
        }
    }
    public class RhinoNormalPart2TextureNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public RhinoNormalPart2TextureNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color_out", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_normal_part2_texture")]
    public class RhinoNormalPart2TextureNode : ShaderNode
    {
        public RhinoNormalPart2TextureNodeInputs ins => (RhinoNormalPart2TextureNodeInputs)inputs;
        public RhinoNormalPart2TextureNodeOutputs outs => (RhinoNormalPart2TextureNodeOutputs)outputs;
        public RhinoNormalPart2TextureNode(Shader shader) : this(shader, "a rhino_normal_part2_texture node") { }

        public RhinoNormalPart2TextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoNormalPart2TextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoNormalPart2TextureNodeInputs(this);
            outputs = new RhinoNormalPart2TextureNodeOutputs(this);
        }
        public float3 Color3 {
            get { return CSycles.rhinonormalpart2texturenode_get_color3(Ptr); }
            set { CSycles.rhinonormalpart2texturenode_set_color3(Ptr, value); }
        }

        public float3 Color8 {
            get { return CSycles.rhinonormalpart2texturenode_get_color8(Ptr); }
            set { CSycles.rhinonormalpart2texturenode_set_color8(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinonormalpart2texturenode_get_color2(Ptr); }
            set { CSycles.rhinonormalpart2texturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinonormalpart2texturenode_get_node_type();
        }

        public float3 Color1 {
            get { return CSycles.rhinonormalpart2texturenode_get_color1(Ptr); }
            set { CSycles.rhinonormalpart2texturenode_set_color1(Ptr, value); }
        }

        public float3 Color4 {
            get { return CSycles.rhinonormalpart2texturenode_get_color4(Ptr); }
            set { CSycles.rhinonormalpart2texturenode_set_color4(Ptr, value); }
        }

        public float3 Color6 {
            get { return CSycles.rhinonormalpart2texturenode_get_color6(Ptr); }
            set { CSycles.rhinonormalpart2texturenode_set_color6(Ptr, value); }
        }

        public float3 Color7 {
            get { return CSycles.rhinonormalpart2texturenode_get_color7(Ptr); }
            set { CSycles.rhinonormalpart2texturenode_set_color7(Ptr, value); }
        }

        public float3 Color5 {
            get { return CSycles.rhinonormalpart2texturenode_get_color5(Ptr); }
            set { CSycles.rhinonormalpart2texturenode_set_color5(Ptr, value); }
        }
#region Setters

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color7":
                    /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color7', 'ui_name': 'Color7'} */
                    {
                    CSycles.rhinonormalpart2texturenode_set_color7(this.Ptr, data);
                    }
                    break;
            case "color3":
                    /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color3', 'ui_name': 'Color3'} */
                    {
                    CSycles.rhinonormalpart2texturenode_set_color3(this.Ptr, data);
                    }
                    break;
            case "color8":
                    /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color8', 'ui_name': 'Color8'} */
                    {
                    CSycles.rhinonormalpart2texturenode_set_color8(this.Ptr, data);
                    }
                    break;
            case "color4":
                    /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color4', 'ui_name': 'Color4'} */
                    {
                    CSycles.rhinonormalpart2texturenode_set_color4(this.Ptr, data);
                    }
                    break;
            case "color5":
                    /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color5', 'ui_name': 'Color5'} */
                    {
                    CSycles.rhinonormalpart2texturenode_set_color5(this.Ptr, data);
                    }
                    break;
            case "color1":
                    /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinonormalpart2texturenode_set_color1(this.Ptr, data);
                    }
                    break;
            case "color6":
                    /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color6', 'ui_name': 'Color6'} */
                    {
                    CSycles.rhinonormalpart2texturenode_set_color6(this.Ptr, data);
                    }
                    break;
            case "color2":
                    /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinonormalpart2texturenode_set_color2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNormalPart2TextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color7":
                /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color7', 'ui_name': 'Color7'} */
                {
                    return CSycles.rhinonormalpart2texturenode_get_color7(this.Ptr);
                }
            case "color3":
                /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color3', 'ui_name': 'Color3'} */
                {
                    return CSycles.rhinonormalpart2texturenode_get_color3(this.Ptr);
                }
            case "color8":
                /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color8', 'ui_name': 'Color8'} */
                {
                    return CSycles.rhinonormalpart2texturenode_get_color8(this.Ptr);
                }
            case "color4":
                /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color4', 'ui_name': 'Color4'} */
                {
                    return CSycles.rhinonormalpart2texturenode_get_color4(this.Ptr);
                }
            case "color5":
                /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color5', 'ui_name': 'Color5'} */
                {
                    return CSycles.rhinonormalpart2texturenode_get_color5(this.Ptr);
                }
            case "color1":
                /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinonormalpart2texturenode_get_color1(this.Ptr);
                }
            case "color6":
                /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color6', 'ui_name': 'Color6'} */
                {
                    return CSycles.rhinonormalpart2texturenode_get_color6(this.Ptr);
                }
            case "color2":
                /* rhinonormalpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinonormalpart2texturenode_get_color2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNormalPart2TextureNode (getter)");
            }
        }

#endregion
    }

}