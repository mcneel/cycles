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

    public class MixFloatNodeInputs : Inputs
    {
        public FloatSocket Factor { get; private set; }
        public BoolSocket UseClamp { get; private set; }
        public FloatSocket B { get; private set; }
        public FloatSocket A { get; private set; }

        public MixFloatNodeInputs(ShaderNode parentNode)
        {
            Factor = new FloatSocket(parentNode, "Factor", "fac", true);
            AddSocket(Factor);
            UseClamp = new BoolSocket(parentNode, "Use Clamp", "use_clamp", true);
            AddSocket(UseClamp);
            B = new FloatSocket(parentNode, "B", "b", true);
            AddSocket(B);
            A = new FloatSocket(parentNode, "A", "a", true);
            AddSocket(A);
        }
    }
    public class MixFloatNodeOutputs : Outputs
    {
        public FloatSocket Result { get; private set; }

        public MixFloatNodeOutputs(ShaderNode parentNode)
        {
            Result = new FloatSocket(parentNode, "Result", "result", false);
            AddSocket(Result);
        }
    }

    [ShaderNode(name: "mix_float")]
    public class MixFloatNode : ShaderNode
    {
        public MixFloatNodeInputs ins => (MixFloatNodeInputs)inputs;
        public MixFloatNodeOutputs outs => (MixFloatNodeOutputs)outputs;
        public MixFloatNode(Shader shader) : this(shader, "a mix_float node") { }

        public MixFloatNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MixFloatNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MixFloatNodeInputs(this);
            outputs = new MixFloatNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.mixfloatnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "fac":
                    /* mixfloatnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                    {
                    CSycles.mixfloatnode_set_fac(this.Ptr, data);
                    }
                    break;
            case "b":
                    /* mixfloatnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                    {
                    CSycles.mixfloatnode_set_b(this.Ptr, data);
                    }
                    break;
            case "a":
                    /* mixfloatnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'a', 'ui_name': 'A'} */
                    {
                    CSycles.mixfloatnode_set_a(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixFloatNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_clamp":
                    /* mixfloatnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                    {
                    CSycles.mixfloatnode_set_use_clamp(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixFloatNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "fac":
                /* mixfloatnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                {
                    return CSycles.mixfloatnode_get_fac(this.Ptr);
                }
            case "b":
                /* mixfloatnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                {
                    return CSycles.mixfloatnode_get_b(this.Ptr);
                }
            case "a":
                /* mixfloatnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'a', 'ui_name': 'A'} */
                {
                    return CSycles.mixfloatnode_get_a(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixFloatNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_clamp":
                /* mixfloatnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                {
                    return CSycles.mixfloatnode_get_use_clamp(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixFloatNode (getter)");
            }
        }

#endregion
    }

}