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

    public class RhinoCheckerTextureNodeInputs : Inputs
    {
        public FloatSocket Alpha1 { get; private set; }
        public PointSocket UVW { get; private set; }
        public ColorSocket Color2 { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public FloatSocket Alpha2 { get; private set; }

        public RhinoCheckerTextureNodeInputs(ShaderNode parentNode)
        {
            Alpha1 = new FloatSocket(parentNode, "Alpha1", "alpha1", true);
            AddSocket(Alpha1);
            UVW = new PointSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            Alpha2 = new FloatSocket(parentNode, "Alpha2", "alpha2", true);
            AddSocket(Alpha2);
        }
    }
    public class RhinoCheckerTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoCheckerTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_checker_texture")]
    public class RhinoCheckerTextureNode : ShaderNode
    {
        public RhinoCheckerTextureNodeInputs ins => (RhinoCheckerTextureNodeInputs)inputs;
        public RhinoCheckerTextureNodeOutputs outs => (RhinoCheckerTextureNodeOutputs)outputs;
        public RhinoCheckerTextureNode(Shader shader) : this(shader, "a rhino_checker_texture node") { }

        public RhinoCheckerTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoCheckerTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoCheckerTextureNodeInputs(this);
            outputs = new RhinoCheckerTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinocheckertexturenode_get_uvw(Ptr); }
            set { CSycles.rhinocheckertexturenode_set_uvw(Ptr, value); }
        }

        public float Alpha1 {
            get { return CSycles.rhinocheckertexturenode_get_alpha1(Ptr); }
            set { CSycles.rhinocheckertexturenode_set_alpha1(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinocheckertexturenode_get_color2(Ptr); }
            set { CSycles.rhinocheckertexturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinocheckertexturenode_get_node_type();
        }

        public float3 Color1 {
            get { return CSycles.rhinocheckertexturenode_get_color1(Ptr); }
            set { CSycles.rhinocheckertexturenode_set_color1(Ptr, value); }
        }

        public float Alpha2 {
            get { return CSycles.rhinocheckertexturenode_get_alpha2(Ptr); }
            set { CSycles.rhinocheckertexturenode_set_alpha2(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "alpha1":
                    /* rhinocheckertexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                    {
                    CSycles.rhinocheckertexturenode_set_alpha1(this.Ptr, data);
                    }
                    break;
            case "alpha2":
                    /* rhinocheckertexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                    {
                    CSycles.rhinocheckertexturenode_set_alpha2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoCheckerTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinocheckertexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinocheckertexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoCheckerTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color2":
                    /* rhinocheckertexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinocheckertexturenode_set_color2(this.Ptr, data);
                    }
                    break;
            case "color1":
                    /* rhinocheckertexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinocheckertexturenode_set_color1(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoCheckerTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "alpha1":
                /* rhinocheckertexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                {
                    return CSycles.rhinocheckertexturenode_get_alpha1(this.Ptr);
                }
            case "alpha2":
                /* rhinocheckertexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                {
                    return CSycles.rhinocheckertexturenode_get_alpha2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoCheckerTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinocheckertexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinocheckertexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoCheckerTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color2":
                /* rhinocheckertexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinocheckertexturenode_get_color2(this.Ptr);
                }
            case "color1":
                /* rhinocheckertexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinocheckertexturenode_get_color1(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoCheckerTextureNode (getter)");
            }
        }

#endregion
    }

}