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

    public class BackgroundNodeInputs : Inputs
    {
        public FloatSocket Strength { get; private set; }
        public ColorSocket Color { get; private set; }

        public BackgroundNodeInputs(ShaderNode parentNode)
        {
            Strength = new FloatSocket(parentNode, "Strength", "strength", true);
            AddSocket(Strength);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    public class BackgroundNodeOutputs : Outputs
    {
        public ClosureSocket Background { get; private set; }

        public BackgroundNodeOutputs(ShaderNode parentNode)
        {
            Background = new ClosureSocket(parentNode, "Background", "background", false);
            AddSocket(Background);
        }
    }

    [ShaderNode(name: "background_shader")]
    public class BackgroundNode : ShaderNode
    {
        public BackgroundNodeInputs ins => (BackgroundNodeInputs)inputs;
        public BackgroundNodeOutputs outs => (BackgroundNodeOutputs)outputs;
        public BackgroundNode(Shader shader) : this(shader, "a background_shader node") { }

        public BackgroundNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal BackgroundNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new BackgroundNodeInputs(this);
            outputs = new BackgroundNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.Background;
        }
        public static IntPtr GetNodeType() {
            return CSycles.backgroundnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "strength":
                    /* backgroundnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                    {
                    CSycles.backgroundnode_set_strength(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BackgroundNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* backgroundnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.backgroundnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BackgroundNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "strength":
                /* backgroundnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                {
                    return CSycles.backgroundnode_get_strength(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BackgroundNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* backgroundnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.backgroundnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BackgroundNode (getter)");
            }
        }

#endregion
    }

}