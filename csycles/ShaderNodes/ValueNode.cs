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

    public class ValueNodeInputs : Inputs
    {
        public FloatSocket Value { get; private set; }

        public ValueNodeInputs(ShaderNode parentNode)
        {
            Value = new FloatSocket(parentNode, "Value", "value", true);
            AddSocket(Value);
        }
    }
    public class ValueNodeOutputs : Outputs
    {
        public FloatSocket Value { get; private set; }

        public ValueNodeOutputs(ShaderNode parentNode)
        {
            Value = new FloatSocket(parentNode, "Value", "value", false);
            AddSocket(Value);
        }
    }

    [ShaderNode(name: "value")]
    public class ValueNode : ShaderNode
    {
        public ValueNodeInputs ins => (ValueNodeInputs)inputs;
        public ValueNodeOutputs outs => (ValueNodeOutputs)outputs;
        public ValueNode(Shader shader) : this(shader, "a value node") { }

        public ValueNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal ValueNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new ValueNodeInputs(this);
            outputs = new ValueNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.valuenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "value":
                    /* valuenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                    {
                    CSycles.valuenode_set_value(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ValueNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "value":
                /* valuenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                {
                    return CSycles.valuenode_get_value(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ValueNode (getter)");
            }
        }

#endregion
    }

}