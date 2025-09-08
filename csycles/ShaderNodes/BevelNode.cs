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

    public class BevelNodeInputs : Inputs
    {
        public NormalSocket Normal { get; private set; }
        public FloatSocket Radius { get; private set; }
        public IntSocket Samples { get; private set; }

        public BevelNodeInputs(ShaderNode parentNode)
        {
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            Radius = new FloatSocket(parentNode, "Radius", "radius", true);
            AddSocket(Radius);
            Samples = new IntSocket(parentNode, "Samples", "samples", true);
            AddSocket(Samples);
        }
    }
    public class BevelNodeOutputs : Outputs
    {
        public NormalSocket Normal { get; private set; }

        public BevelNodeOutputs(ShaderNode parentNode)
        {
            Normal = new NormalSocket(parentNode, "Normal", "bevel", false);
            AddSocket(Normal);
        }
    }

    [ShaderNode(name: "bevel")]
    public class BevelNode : ShaderNode
    {
        public BevelNodeInputs ins => (BevelNodeInputs)inputs;
        public BevelNodeOutputs outs => (BevelNodeOutputs)outputs;
        public BevelNode(Shader shader) : this(shader, "a bevel node") { }

        public BevelNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal BevelNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new BevelNodeInputs(this);
            outputs = new BevelNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.bevelnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "radius":
                    /* bevelnode . {'datatype': 'FLOAT', 'default_value': '0.05f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'radius', 'ui_name': 'Radius'} */
                    {
                    CSycles.bevelnode_set_radius(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BevelNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* bevelnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.bevelnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BevelNode (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "samples":
                    /* bevelnode . {'datatype': 'INT', 'default_value': '4', 'default_value_type': 'int', 'is_input': True, 'member_name': 'samples', 'ui_name': 'Samples'} */
                    {
                    CSycles.bevelnode_set_samples(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BevelNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "radius":
                /* bevelnode . {'datatype': 'FLOAT', 'default_value': '0.05f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'radius', 'ui_name': 'Radius'} */
                {
                    return CSycles.bevelnode_get_radius(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BevelNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* bevelnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.bevelnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BevelNode (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "samples":
                /* bevelnode . {'datatype': 'INT', 'default_value': '4', 'default_value_type': 'int', 'is_input': True, 'member_name': 'samples', 'ui_name': 'Samples'} */
                {
                    return CSycles.bevelnode_get_samples(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BevelNode (getter)");
            }
        }

#endregion
    }

}