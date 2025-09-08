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

    public class OutputNodeInputs : Inputs
    {
        public NormalSocket Normal { get; private set; }
        public VectorSocket Displacement { get; private set; }
        public ClosureSocket Surface { get; private set; }

        public OutputNodeInputs(ShaderNode parentNode)
        {
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            Displacement = new VectorSocket(parentNode, "Displacement", "displacement", true);
            AddSocket(Displacement);
            Surface = new ClosureSocket(parentNode, "Surface", "surface", true);
            AddSocket(Surface);
        }
    }
    [ShaderNode(name: "output")]
    public class OutputNode : ShaderNode
    {
        public OutputNodeInputs ins => (OutputNodeInputs)inputs;
        public OutputNode(Shader shader) : this(shader, "a output node") { }

        public OutputNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal OutputNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new OutputNodeInputs(this);

        }
        public void SetVolume(IntPtr value) {
            CSycles.outputnode_set_volume(Ptr, value);
        }
        public IntPtr GetVolume() {
            return CSycles.outputnode_get_volume(Ptr);
        }

        public static IntPtr GetNodeType() {
            return CSycles.outputnode_get_node_type();
        }
#region Setters

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "displacement":
                    /* outputnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'displacement', 'ui_name': 'Displacement'} */
                    {
                    CSycles.outputnode_set_displacement(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type OutputNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* outputnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.outputnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type OutputNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "displacement":
                /* outputnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'displacement', 'ui_name': 'Displacement'} */
                {
                    return CSycles.outputnode_get_displacement(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type OutputNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* outputnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.outputnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type OutputNode (getter)");
            }
        }

#endregion
    }

}