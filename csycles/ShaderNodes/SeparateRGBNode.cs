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

    public class SeparateRGBNodeInputs : Inputs
    {
        public ColorSocket Image { get; private set; }

        public SeparateRGBNodeInputs(ShaderNode parentNode)
        {
            Image = new ColorSocket(parentNode, "Image", "color", true);
            AddSocket(Image);
        }
    }
    public class SeparateRGBNodeOutputs : Outputs
    {
        public FloatSocket B { get; private set; }
        public FloatSocket G { get; private set; }
        public FloatSocket R { get; private set; }

        public SeparateRGBNodeOutputs(ShaderNode parentNode)
        {
            B = new FloatSocket(parentNode, "B", "b", false);
            AddSocket(B);
            G = new FloatSocket(parentNode, "G", "g", false);
            AddSocket(G);
            R = new FloatSocket(parentNode, "R", "r", false);
            AddSocket(R);
        }
    }

    [ShaderNode(name: "separate_rgb")]
    public class SeparateRGBNode : ShaderNode
    {
        public SeparateRGBNodeInputs ins => (SeparateRGBNodeInputs)inputs;
        public SeparateRGBNodeOutputs outs => (SeparateRGBNodeOutputs)outputs;
        public SeparateRGBNode(Shader shader) : this(shader, "a separate_rgb node") { }

        public SeparateRGBNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal SeparateRGBNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new SeparateRGBNodeInputs(this);
            outputs = new SeparateRGBNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.separatergbnode_get_node_type();
        }
#region Setters

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* separatergbnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Image'} */
                    {
                    CSycles.separatergbnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateRGBNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* separatergbnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Image'} */
                {
                    return CSycles.separatergbnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateRGBNode (getter)");
            }
        }

#endregion
    }

}