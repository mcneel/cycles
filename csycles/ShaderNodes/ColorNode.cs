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

    public class ColorNodeInputs : Inputs
    {
        public ColorSocket Value { get; private set; }

        public ColorNodeInputs(ShaderNode parentNode)
        {
            Value = new ColorSocket(parentNode, "Value", "value", true);
            AddSocket(Value);
        }
    }
    public class ColorNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public ColorNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "color")]
    public class ColorNode : ShaderNode
    {
        public ColorNodeInputs ins => (ColorNodeInputs)inputs;
        public ColorNodeOutputs outs => (ColorNodeOutputs)outputs;
        public ColorNode(Shader shader) : this(shader, "a color node") { }

        public ColorNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal ColorNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new ColorNodeInputs(this);
            outputs = new ColorNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.colornode_get_node_type();
        }
#region Setters

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "value":
                    /* colornode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                    {
                    CSycles.colornode_set_value(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ColorNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "value":
                /* colornode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                {
                    return CSycles.colornode_get_value(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ColorNode (getter)");
            }
        }

#endregion
    }

}