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

    public class CombineHSVNodeInputs : Inputs
    {
        public FloatSocket V { get; private set; }
        public FloatSocket S { get; private set; }
        public FloatSocket H { get; private set; }

        public CombineHSVNodeInputs(ShaderNode parentNode)
        {
            V = new FloatSocket(parentNode, "V", "v", true);
            AddSocket(V);
            S = new FloatSocket(parentNode, "S", "s", true);
            AddSocket(S);
            H = new FloatSocket(parentNode, "H", "h", true);
            AddSocket(H);
        }
    }
    public class CombineHSVNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public CombineHSVNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "combine_hsv")]
    public class CombineHSVNode : ShaderNode
    {
        public CombineHSVNodeInputs ins => (CombineHSVNodeInputs)inputs;
        public CombineHSVNodeOutputs outs => (CombineHSVNodeOutputs)outputs;
        public CombineHSVNode(Shader shader) : this(shader, "a combine_hsv node") { }

        public CombineHSVNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal CombineHSVNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new CombineHSVNodeInputs(this);
            outputs = new CombineHSVNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.combinehsvnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "v":
                    /* combinehsvnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'v', 'ui_name': 'V'} */
                    {
                    CSycles.combinehsvnode_set_v(this.Ptr, data);
                    }
                    break;
            case "s":
                    /* combinehsvnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 's', 'ui_name': 'S'} */
                    {
                    CSycles.combinehsvnode_set_s(this.Ptr, data);
                    }
                    break;
            case "h":
                    /* combinehsvnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'h', 'ui_name': 'H'} */
                    {
                    CSycles.combinehsvnode_set_h(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineHSVNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "v":
                /* combinehsvnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'v', 'ui_name': 'V'} */
                {
                    return CSycles.combinehsvnode_get_v(this.Ptr);
                }
            case "s":
                /* combinehsvnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 's', 'ui_name': 'S'} */
                {
                    return CSycles.combinehsvnode_get_s(this.Ptr);
                }
            case "h":
                /* combinehsvnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'h', 'ui_name': 'H'} */
                {
                    return CSycles.combinehsvnode_get_h(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineHSVNode (getter)");
            }
        }

#endregion
    }

}