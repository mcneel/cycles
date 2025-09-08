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

    public class MixVectorNodeInputs : Inputs
    {
        public BoolSocket UseClamp { get; private set; }
        public VectorSocket B { get; private set; }
        public VectorSocket A { get; private set; }
        public FloatSocket Factor { get; private set; }

        public MixVectorNodeInputs(ShaderNode parentNode)
        {
            UseClamp = new BoolSocket(parentNode, "Use Clamp", "use_clamp", true);
            AddSocket(UseClamp);
            B = new VectorSocket(parentNode, "B", "b", true);
            AddSocket(B);
            A = new VectorSocket(parentNode, "A", "a", true);
            AddSocket(A);
            Factor = new FloatSocket(parentNode, "Factor", "fac", true);
            AddSocket(Factor);
        }
    }
    public class MixVectorNodeOutputs : Outputs
    {
        public VectorSocket Result { get; private set; }

        public MixVectorNodeOutputs(ShaderNode parentNode)
        {
            Result = new VectorSocket(parentNode, "Result", "result", false);
            AddSocket(Result);
        }
    }

    [ShaderNode(name: "mix_vector")]
    public class MixVectorNode : ShaderNode
    {
        public MixVectorNodeInputs ins => (MixVectorNodeInputs)inputs;
        public MixVectorNodeOutputs outs => (MixVectorNodeOutputs)outputs;
        public MixVectorNode(Shader shader) : this(shader, "a mix_vector node") { }

        public MixVectorNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MixVectorNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MixVectorNodeInputs(this);
            outputs = new MixVectorNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.mixvectornode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "fac":
                    /* mixvectornode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                    {
                    CSycles.mixvectornode_set_fac(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "b":
                    /* mixvectornode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                    {
                    CSycles.mixvectornode_set_b(this.Ptr, data);
                    }
                    break;
            case "a":
                    /* mixvectornode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'a', 'ui_name': 'A'} */
                    {
                    CSycles.mixvectornode_set_a(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_clamp":
                    /* mixvectornode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                    {
                    CSycles.mixvectornode_set_use_clamp(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "fac":
                /* mixvectornode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                {
                    return CSycles.mixvectornode_get_fac(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "b":
                /* mixvectornode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                {
                    return CSycles.mixvectornode_get_b(this.Ptr);
                }
            case "a":
                /* mixvectornode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'a', 'ui_name': 'A'} */
                {
                    return CSycles.mixvectornode_get_a(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_clamp":
                /* mixvectornode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                {
                    return CSycles.mixvectornode_get_use_clamp(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNode (getter)");
            }
        }

#endregion
    }

}