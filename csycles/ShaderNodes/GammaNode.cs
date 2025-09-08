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

    public class GammaNodeInputs : Inputs
    {
        public ColorSocket Color { get; private set; }
        public FloatSocket Gamma { get; private set; }

        public GammaNodeInputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Gamma = new FloatSocket(parentNode, "Gamma", "gamma", true);
            AddSocket(Gamma);
        }
    }
    public class GammaNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public GammaNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "gamma")]
    public class GammaNode : ShaderNode
    {
        public GammaNodeInputs ins => (GammaNodeInputs)inputs;
        public GammaNodeOutputs outs => (GammaNodeOutputs)outputs;
        public GammaNode(Shader shader) : this(shader, "a gamma node") { }

        public GammaNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal GammaNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new GammaNodeInputs(this);
            outputs = new GammaNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.gammanode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "gamma":
                    /* gammanode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gamma', 'ui_name': 'Gamma'} */
                    {
                    CSycles.gammanode_set_gamma(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GammaNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* gammanode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.gammanode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GammaNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "gamma":
                /* gammanode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gamma', 'ui_name': 'Gamma'} */
                {
                    return CSycles.gammanode_get_gamma(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GammaNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* gammanode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.gammanode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GammaNode (getter)");
            }
        }

#endregion
    }

}