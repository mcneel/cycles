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

    public class OutputAOVNodeInputs : Inputs
    {
        public FloatSocket Value { get; private set; }
        public ColorSocket Color { get; private set; }

        public OutputAOVNodeInputs(ShaderNode parentNode)
        {
            Value = new FloatSocket(parentNode, "Value", "value", true);
            AddSocket(Value);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    [ShaderNode(name: "aov_output")]
    public class OutputAOVNode : ShaderNode
    {
        public OutputAOVNodeInputs ins => (OutputAOVNodeInputs)inputs;
        public OutputAOVNode(Shader shader) : this(shader, "a aov_output node") { }

        public OutputAOVNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal OutputAOVNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new OutputAOVNodeInputs(this);

        }
        public bool IsColor {
            get { return CSycles.outputaovnode_get_is_color(Ptr); }
            set { CSycles.outputaovnode_set_is_color(Ptr, value); }
        }
        public int Offset {
            get { return CSycles.outputaovnode_get_offset(Ptr); }
            set { CSycles.outputaovnode_set_offset(Ptr, value); }
        }
        public static IntPtr GetNodeType() {
            return CSycles.outputaovnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "value":
                    /* outputaovnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                    {
                    CSycles.outputaovnode_set_value(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type OutputAOVNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* outputaovnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.outputaovnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type OutputAOVNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "value":
                /* outputaovnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                {
                    return CSycles.outputaovnode_get_value(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type OutputAOVNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* outputaovnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.outputaovnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type OutputAOVNode (getter)");
            }
        }

#endregion
    }

}