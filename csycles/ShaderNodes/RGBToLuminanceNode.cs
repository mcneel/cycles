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

    public class RGBToLuminanceNodeInputs : Inputs
    {
        public ColorSocket Color { get; private set; }

        public RGBToLuminanceNodeInputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    public class RGBToLuminanceNodeOutputs : Outputs
    {
        public FloatSocket Val { get; private set; }

        public RGBToLuminanceNodeOutputs(ShaderNode parentNode)
        {
            Val = new FloatSocket(parentNode, "Val", "val", false);
            AddSocket(Val);
        }
    }

    [ShaderNode(name: "rgb_to_luminance")]
    public class RGBToLuminanceNode : ShaderNode
    {
        public RGBToLuminanceNodeInputs ins => (RGBToLuminanceNodeInputs)inputs;
        public RGBToLuminanceNodeOutputs outs => (RGBToLuminanceNodeOutputs)outputs;
        public RGBToLuminanceNode(Shader shader) : this(shader, "a rgb_to_luminance node") { }

        public RGBToLuminanceNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RGBToLuminanceNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RGBToLuminanceNodeInputs(this);
            outputs = new RGBToLuminanceNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.rgbtoluminancenode_get_node_type();
        }
#region Setters

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* rgbtoluminancenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.rgbtoluminancenode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBToLuminanceNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* rgbtoluminancenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.rgbtoluminancenode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBToLuminanceNode (getter)");
            }
        }

#endregion
    }

}