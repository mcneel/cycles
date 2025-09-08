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

    public class RhinoPerturbingPart2TextureNodeInputs : Inputs
    {
        public ColorSocket Color1 { get; private set; }
        public FloatSocket Amount { get; private set; }
        public ColorSocket Color2 { get; private set; }
        public PointSocket UVW { get; private set; }
        public ColorSocket Color3 { get; private set; }

        public RhinoPerturbingPart2TextureNodeInputs(ShaderNode parentNode)
        {
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            Amount = new FloatSocket(parentNode, "Amount", "amount", true);
            AddSocket(Amount);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            UVW = new PointSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            Color3 = new ColorSocket(parentNode, "Color3", "color3", true);
            AddSocket(Color3);
        }
    }
    public class RhinoPerturbingPart2TextureNodeOutputs : Outputs
    {
        public PointSocket PerturbedUVW { get; private set; }

        public RhinoPerturbingPart2TextureNodeOutputs(ShaderNode parentNode)
        {
            PerturbedUVW = new PointSocket(parentNode, "Perturbed UVW", "out_uvw", false);
            AddSocket(PerturbedUVW);
        }
    }

    [ShaderNode(name: "rhino_perturbing_part2_texture")]
    public class RhinoPerturbingPart2TextureNode : ShaderNode
    {
        public RhinoPerturbingPart2TextureNodeInputs ins => (RhinoPerturbingPart2TextureNodeInputs)inputs;
        public RhinoPerturbingPart2TextureNodeOutputs outs => (RhinoPerturbingPart2TextureNodeOutputs)outputs;
        public RhinoPerturbingPart2TextureNode(Shader shader) : this(shader, "a rhino_perturbing_part2_texture node") { }

        public RhinoPerturbingPart2TextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoPerturbingPart2TextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoPerturbingPart2TextureNodeInputs(this);
            outputs = new RhinoPerturbingPart2TextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinoperturbingpart2texturenode_get_uvw(Ptr); }
            set { CSycles.rhinoperturbingpart2texturenode_set_uvw(Ptr, value); }
        }

        public float3 Color3 {
            get { return CSycles.rhinoperturbingpart2texturenode_get_color3(Ptr); }
            set { CSycles.rhinoperturbingpart2texturenode_set_color3(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinoperturbingpart2texturenode_get_color2(Ptr); }
            set { CSycles.rhinoperturbingpart2texturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinoperturbingpart2texturenode_get_node_type();
        }

        public float Amount {
            get { return CSycles.rhinoperturbingpart2texturenode_get_amount(Ptr); }
            set { CSycles.rhinoperturbingpart2texturenode_set_amount(Ptr, value); }
        }

        public float3 Color1 {
            get { return CSycles.rhinoperturbingpart2texturenode_get_color1(Ptr); }
            set { CSycles.rhinoperturbingpart2texturenode_set_color1(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "amount":
                    /* rhinoperturbingpart2texturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'amount', 'ui_name': 'Amount'} */
                    {
                    CSycles.rhinoperturbingpart2texturenode_set_amount(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerturbingPart2TextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinoperturbingpart2texturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinoperturbingpart2texturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerturbingPart2TextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color1":
                    /* rhinoperturbingpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinoperturbingpart2texturenode_set_color1(this.Ptr, data);
                    }
                    break;
            case "color2":
                    /* rhinoperturbingpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinoperturbingpart2texturenode_set_color2(this.Ptr, data);
                    }
                    break;
            case "color3":
                    /* rhinoperturbingpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color3', 'ui_name': 'Color3'} */
                    {
                    CSycles.rhinoperturbingpart2texturenode_set_color3(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerturbingPart2TextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "amount":
                /* rhinoperturbingpart2texturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'amount', 'ui_name': 'Amount'} */
                {
                    return CSycles.rhinoperturbingpart2texturenode_get_amount(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerturbingPart2TextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinoperturbingpart2texturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinoperturbingpart2texturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerturbingPart2TextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color1":
                /* rhinoperturbingpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinoperturbingpart2texturenode_get_color1(this.Ptr);
                }
            case "color2":
                /* rhinoperturbingpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinoperturbingpart2texturenode_get_color2(this.Ptr);
                }
            case "color3":
                /* rhinoperturbingpart2texturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color3', 'ui_name': 'Color3'} */
                {
                    return CSycles.rhinoperturbingpart2texturenode_get_color3(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerturbingPart2TextureNode (getter)");
            }
        }

#endregion
    }

}