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

    public class SetNormalNodeInputs : Inputs
    {
        public VectorSocket Direction { get; private set; }

        public SetNormalNodeInputs(ShaderNode parentNode)
        {
            Direction = new VectorSocket(parentNode, "Direction", "direction", true);
            AddSocket(Direction);
        }
    }
    public class SetNormalNodeOutputs : Outputs
    {
        public NormalSocket Normal { get; private set; }

        public SetNormalNodeOutputs(ShaderNode parentNode)
        {
            Normal = new NormalSocket(parentNode, "Normal", "normal", false);
            AddSocket(Normal);
        }
    }

    [ShaderNode(name: "set_normal")]
    public class SetNormalNode : ShaderNode
    {
        public SetNormalNodeInputs ins => (SetNormalNodeInputs)inputs;
        public SetNormalNodeOutputs outs => (SetNormalNodeOutputs)outputs;
        public SetNormalNode(Shader shader) : this(shader, "a set_normal node") { }

        public SetNormalNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal SetNormalNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new SetNormalNodeInputs(this);
            outputs = new SetNormalNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.setnormalnode_get_node_type();
        }
#region Setters

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "direction":
                    /* setnormalnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'direction', 'ui_name': 'Direction'} */
                    {
                    CSycles.setnormalnode_set_direction(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SetNormalNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "direction":
                /* setnormalnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'direction', 'ui_name': 'Direction'} */
                {
                    return CSycles.setnormalnode_get_direction(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SetNormalNode (getter)");
            }
        }

#endregion
    }

}