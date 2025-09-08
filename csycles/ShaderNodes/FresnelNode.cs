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

    public class FresnelNodeInputs : Inputs
    {
        public FloatSocket IOR { get; private set; }
        public NormalSocket Normal { get; private set; }

        public FresnelNodeInputs(ShaderNode parentNode)
        {
            IOR = new FloatSocket(parentNode, "IOR", "IOR", true);
            AddSocket(IOR);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
        }
    }
    public class FresnelNodeOutputs : Outputs
    {
        public FloatSocket Fac { get; private set; }

        public FresnelNodeOutputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", false);
            AddSocket(Fac);
        }
    }

    [ShaderNode(name: "fresnel")]
    public class FresnelNode : ShaderNode
    {
        public FresnelNodeInputs ins => (FresnelNodeInputs)inputs;
        public FresnelNodeOutputs outs => (FresnelNodeOutputs)outputs;
        public FresnelNode(Shader shader) : this(shader, "a fresnel node") { }

        public FresnelNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal FresnelNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new FresnelNodeInputs(this);
            outputs = new FresnelNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.fresnelnode_get_node_type();
        }
        public float GetIor() {
            return CSycles.fresnelnode_get_ior(Ptr);
        }

        public void SetIor(float value) {
            CSycles.fresnelnode_set_ior(Ptr, value);
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "IOR":
                    /* fresnelnode . {'datatype': 'FLOAT', 'default_value': '1.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'IOR', 'ui_name': 'IOR'} */
                    {
                    CSycles.fresnelnode_set_ior(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FresnelNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* fresnelnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.fresnelnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FresnelNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "IOR":
                /* fresnelnode . {'datatype': 'FLOAT', 'default_value': '1.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'IOR', 'ui_name': 'IOR'} */
                {
                    return CSycles.fresnelnode_get_ior(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FresnelNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* fresnelnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.fresnelnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FresnelNode (getter)");
            }
        }

#endregion
    }

}