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

    public class BlackbodyNodeInputs : Inputs
    {
        public FloatSocket Temperature { get; private set; }

        public BlackbodyNodeInputs(ShaderNode parentNode)
        {
            Temperature = new FloatSocket(parentNode, "Temperature", "temperature", true);
            AddSocket(Temperature);
        }
    }
    public class BlackbodyNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public BlackbodyNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "blackbody")]
    public class BlackbodyNode : ShaderNode
    {
        public BlackbodyNodeInputs ins => (BlackbodyNodeInputs)inputs;
        public BlackbodyNodeOutputs outs => (BlackbodyNodeOutputs)outputs;
        public BlackbodyNode(Shader shader) : this(shader, "a blackbody node") { }

        public BlackbodyNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal BlackbodyNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new BlackbodyNodeInputs(this);
            outputs = new BlackbodyNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.blackbodynode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "temperature":
                    /* blackbodynode . {'datatype': 'FLOAT', 'default_value': '1200.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'temperature', 'ui_name': 'Temperature'} */
                    {
                    CSycles.blackbodynode_set_temperature(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BlackbodyNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "temperature":
                /* blackbodynode . {'datatype': 'FLOAT', 'default_value': '1200.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'temperature', 'ui_name': 'Temperature'} */
                {
                    return CSycles.blackbodynode_get_temperature(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BlackbodyNode (getter)");
            }
        }

#endregion
    }

}