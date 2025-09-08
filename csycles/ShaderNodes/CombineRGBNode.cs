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

    public class CombineRGBNodeInputs : Inputs
    {
        public FloatSocket B { get; private set; }
        public FloatSocket G { get; private set; }
        public FloatSocket R { get; private set; }

        public CombineRGBNodeInputs(ShaderNode parentNode)
        {
            B = new FloatSocket(parentNode, "B", "b", true);
            AddSocket(B);
            G = new FloatSocket(parentNode, "G", "g", true);
            AddSocket(G);
            R = new FloatSocket(parentNode, "R", "r", true);
            AddSocket(R);
        }
    }
    public class CombineRGBNodeOutputs : Outputs
    {
        public ColorSocket Image { get; private set; }

        public CombineRGBNodeOutputs(ShaderNode parentNode)
        {
            Image = new ColorSocket(parentNode, "Image", "image", false);
            AddSocket(Image);
        }
    }

    [ShaderNode(name: "combine_rgb")]
    public class CombineRGBNode : ShaderNode
    {
        public CombineRGBNodeInputs ins => (CombineRGBNodeInputs)inputs;
        public CombineRGBNodeOutputs outs => (CombineRGBNodeOutputs)outputs;
        public CombineRGBNode(Shader shader) : this(shader, "a combine_rgb node") { }

        public CombineRGBNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal CombineRGBNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new CombineRGBNodeInputs(this);
            outputs = new CombineRGBNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.combinergbnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "b":
                    /* combinergbnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                    {
                    CSycles.combinergbnode_set_b(this.Ptr, data);
                    }
                    break;
            case "g":
                    /* combinergbnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'g', 'ui_name': 'G'} */
                    {
                    CSycles.combinergbnode_set_g(this.Ptr, data);
                    }
                    break;
            case "r":
                    /* combinergbnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'r', 'ui_name': 'R'} */
                    {
                    CSycles.combinergbnode_set_r(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineRGBNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "b":
                /* combinergbnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                {
                    return CSycles.combinergbnode_get_b(this.Ptr);
                }
            case "g":
                /* combinergbnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'g', 'ui_name': 'G'} */
                {
                    return CSycles.combinergbnode_get_g(this.Ptr);
                }
            case "r":
                /* combinergbnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'r', 'ui_name': 'R'} */
                {
                    return CSycles.combinergbnode_get_r(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineRGBNode (getter)");
            }
        }

#endregion
    }

}