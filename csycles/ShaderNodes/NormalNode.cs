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

    public class NormalNodeInputs : Inputs
    {
        public NormalSocket Normal { get; private set; }
        public VectorSocket direction { get; private set; }

        public NormalNodeInputs(ShaderNode parentNode)
        {
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            direction = new VectorSocket(parentNode, "direction", "direction", true);
            AddSocket(direction);
        }
    }
    public class NormalNodeOutputs : Outputs
    {
        public FloatSocket Dot { get; private set; }
        public NormalSocket Normal { get; private set; }

        public NormalNodeOutputs(ShaderNode parentNode)
        {
            Dot = new FloatSocket(parentNode, "Dot", "dot", false);
            AddSocket(Dot);
            Normal = new NormalSocket(parentNode, "Normal", "normal", false);
            AddSocket(Normal);
        }
    }

    [ShaderNode(name: "normal")]
    public class NormalNode : ShaderNode
    {
        public NormalNodeInputs ins => (NormalNodeInputs)inputs;
        public NormalNodeOutputs outs => (NormalNodeOutputs)outputs;
        public NormalNode(Shader shader) : this(shader, "a normal node") { }

        public NormalNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal NormalNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new NormalNodeInputs(this);
            outputs = new NormalNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.normalnode_get_node_type();
        }
#region Setters

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "direction":
                    /* normalnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'direction', 'ui_name': 'direction'} */
                    {
                    CSycles.normalnode_set_direction(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* normalnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.normalnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "direction":
                /* normalnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'direction', 'ui_name': 'direction'} */
                {
                    return CSycles.normalnode_get_direction(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* normalnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.normalnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalNode (getter)");
            }
        }

#endregion
    }

}