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

    public class CombineXYZNodeInputs : Inputs
    {
        public FloatSocket X { get; private set; }
        public FloatSocket Z { get; private set; }
        public FloatSocket Y { get; private set; }

        public CombineXYZNodeInputs(ShaderNode parentNode)
        {
            X = new FloatSocket(parentNode, "X", "x", true);
            AddSocket(X);
            Z = new FloatSocket(parentNode, "Z", "z", true);
            AddSocket(Z);
            Y = new FloatSocket(parentNode, "Y", "y", true);
            AddSocket(Y);
        }
    }
    public class CombineXYZNodeOutputs : Outputs
    {
        public VectorSocket Vector { get; private set; }

        public CombineXYZNodeOutputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "vector", false);
            AddSocket(Vector);
        }
    }

    [ShaderNode(name: "combine_xyz")]
    public class CombineXYZNode : ShaderNode
    {
        public CombineXYZNodeInputs ins => (CombineXYZNodeInputs)inputs;
        public CombineXYZNodeOutputs outs => (CombineXYZNodeOutputs)outputs;
        public CombineXYZNode(Shader shader) : this(shader, "a combine_xyz node") { }

        public CombineXYZNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal CombineXYZNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new CombineXYZNodeInputs(this);
            outputs = new CombineXYZNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.combinexyznode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "x":
                    /* combinexyznode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'x', 'ui_name': 'X'} */
                    {
                    CSycles.combinexyznode_set_x(this.Ptr, data);
                    }
                    break;
            case "z":
                    /* combinexyznode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'z', 'ui_name': 'Z'} */
                    {
                    CSycles.combinexyznode_set_z(this.Ptr, data);
                    }
                    break;
            case "y":
                    /* combinexyznode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'y', 'ui_name': 'Y'} */
                    {
                    CSycles.combinexyznode_set_y(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineXYZNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "x":
                /* combinexyznode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'x', 'ui_name': 'X'} */
                {
                    return CSycles.combinexyznode_get_x(this.Ptr);
                }
            case "z":
                /* combinexyznode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'z', 'ui_name': 'Z'} */
                {
                    return CSycles.combinexyznode_get_z(this.Ptr);
                }
            case "y":
                /* combinexyznode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'y', 'ui_name': 'Y'} */
                {
                    return CSycles.combinexyznode_get_y(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineXYZNode (getter)");
            }
        }

#endregion
    }

}