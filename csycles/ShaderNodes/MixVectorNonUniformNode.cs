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

    public class MixVectorNonUniformNodeInputs : Inputs
    {
        public BoolSocket UseClamp { get; private set; }
        public VectorSocket B { get; private set; }
        public VectorSocket A { get; private set; }
        public VectorSocket Factor { get; private set; }

        public MixVectorNonUniformNodeInputs(ShaderNode parentNode)
        {
            UseClamp = new BoolSocket(parentNode, "Use Clamp", "use_clamp", true);
            AddSocket(UseClamp);
            B = new VectorSocket(parentNode, "B", "b", true);
            AddSocket(B);
            A = new VectorSocket(parentNode, "A", "a", true);
            AddSocket(A);
            Factor = new VectorSocket(parentNode, "Factor", "fac", true);
            AddSocket(Factor);
        }
    }
    public class MixVectorNonUniformNodeOutputs : Outputs
    {
        public VectorSocket Result { get; private set; }

        public MixVectorNonUniformNodeOutputs(ShaderNode parentNode)
        {
            Result = new VectorSocket(parentNode, "Result", "result", false);
            AddSocket(Result);
        }
    }

    [ShaderNode(name: "mix_vector_non_uniform")]
    public class MixVectorNonUniformNode : ShaderNode
    {
        public MixVectorNonUniformNodeInputs ins => (MixVectorNonUniformNodeInputs)inputs;
        public MixVectorNonUniformNodeOutputs outs => (MixVectorNonUniformNodeOutputs)outputs;
        public MixVectorNonUniformNode(Shader shader) : this(shader, "a mix_vector_non_uniform node") { }

        public MixVectorNonUniformNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MixVectorNonUniformNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MixVectorNonUniformNodeInputs(this);
            outputs = new MixVectorNonUniformNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.mixvectornonuniformnode_get_node_type();
        }
#region Setters

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "b":
                    /* mixvectornonuniformnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                    {
                    CSycles.mixvectornonuniformnode_set_b(this.Ptr, data);
                    }
                    break;
            case "a":
                    /* mixvectornonuniformnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'a', 'ui_name': 'A'} */
                    {
                    CSycles.mixvectornonuniformnode_set_a(this.Ptr, data);
                    }
                    break;
            case "fac":
                    /* mixvectornonuniformnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.5f,0.5f,0.5f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                    {
                    CSycles.mixvectornonuniformnode_set_fac(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNonUniformNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_clamp":
                    /* mixvectornonuniformnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                    {
                    CSycles.mixvectornonuniformnode_set_use_clamp(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNonUniformNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "b":
                /* mixvectornonuniformnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                {
                    return CSycles.mixvectornonuniformnode_get_b(this.Ptr);
                }
            case "a":
                /* mixvectornonuniformnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'a', 'ui_name': 'A'} */
                {
                    return CSycles.mixvectornonuniformnode_get_a(this.Ptr);
                }
            case "fac":
                /* mixvectornonuniformnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.5f,0.5f,0.5f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                {
                    return CSycles.mixvectornonuniformnode_get_fac(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNonUniformNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_clamp":
                /* mixvectornonuniformnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                {
                    return CSycles.mixvectornonuniformnode_get_use_clamp(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixVectorNonUniformNode (getter)");
            }
        }

#endregion
    }

}