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

    public class SeparateHSVNodeInputs : Inputs
    {
        public ColorSocket Color { get; private set; }

        public SeparateHSVNodeInputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    public class SeparateHSVNodeOutputs : Outputs
    {
        public FloatSocket V { get; private set; }
        public FloatSocket S { get; private set; }
        public FloatSocket H { get; private set; }

        public SeparateHSVNodeOutputs(ShaderNode parentNode)
        {
            V = new FloatSocket(parentNode, "V", "v", false);
            AddSocket(V);
            S = new FloatSocket(parentNode, "S", "s", false);
            AddSocket(S);
            H = new FloatSocket(parentNode, "H", "h", false);
            AddSocket(H);
        }
    }

    [ShaderNode(name: "separate_hsv")]
    public class SeparateHSVNode : ShaderNode
    {
        public SeparateHSVNodeInputs ins => (SeparateHSVNodeInputs)inputs;
        public SeparateHSVNodeOutputs outs => (SeparateHSVNodeOutputs)outputs;
        public SeparateHSVNode(Shader shader) : this(shader, "a separate_hsv node") { }

        public SeparateHSVNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal SeparateHSVNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new SeparateHSVNodeInputs(this);
            outputs = new SeparateHSVNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.separatehsvnode_get_node_type();
        }
#region Setters

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* separatehsvnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.separatehsvnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateHSVNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* separatehsvnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.separatehsvnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateHSVNode (getter)");
            }
        }

#endregion
    }

}