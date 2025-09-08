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

    public class SeparateXYZNodeInputs : Inputs
    {
        public ColorSocket Vector { get; private set; }

        public SeparateXYZNodeInputs(ShaderNode parentNode)
        {
            Vector = new ColorSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
        }
    }
    public class SeparateXYZNodeOutputs : Outputs
    {
        public FloatSocket Z { get; private set; }
        public FloatSocket Y { get; private set; }
        public FloatSocket X { get; private set; }

        public SeparateXYZNodeOutputs(ShaderNode parentNode)
        {
            Z = new FloatSocket(parentNode, "Z", "z", false);
            AddSocket(Z);
            Y = new FloatSocket(parentNode, "Y", "y", false);
            AddSocket(Y);
            X = new FloatSocket(parentNode, "X", "x", false);
            AddSocket(X);
        }
    }

    [ShaderNode(name: "separate_xyz")]
    public class SeparateXYZNode : ShaderNode
    {
        public SeparateXYZNodeInputs ins => (SeparateXYZNodeInputs)inputs;
        public SeparateXYZNodeOutputs outs => (SeparateXYZNodeOutputs)outputs;
        public SeparateXYZNode(Shader shader) : this(shader, "a separate_xyz node") { }

        public SeparateXYZNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal SeparateXYZNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new SeparateXYZNodeInputs(this);
            outputs = new SeparateXYZNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.separatexyznode_get_node_type();
        }
#region Setters

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* separatexyznode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.separatexyznode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateXYZNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "vector":
                /* separatexyznode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.separatexyznode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateXYZNode (getter)");
            }
        }

#endregion
    }

}