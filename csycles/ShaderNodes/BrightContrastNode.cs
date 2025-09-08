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

    public class BrightContrastNodeInputs : Inputs
    {
        public FloatSocket Contrast { get; private set; }
        public FloatSocket Bright { get; private set; }
        public ColorSocket Color { get; private set; }

        public BrightContrastNodeInputs(ShaderNode parentNode)
        {
            Contrast = new FloatSocket(parentNode, "Contrast", "contrast", true);
            AddSocket(Contrast);
            Bright = new FloatSocket(parentNode, "Bright", "bright", true);
            AddSocket(Bright);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    public class BrightContrastNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public BrightContrastNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "brightness_contrast")]
    public class BrightContrastNode : ShaderNode
    {
        public BrightContrastNodeInputs ins => (BrightContrastNodeInputs)inputs;
        public BrightContrastNodeOutputs outs => (BrightContrastNodeOutputs)outputs;
        public BrightContrastNode(Shader shader) : this(shader, "a brightness_contrast node") { }

        public BrightContrastNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal BrightContrastNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new BrightContrastNodeInputs(this);
            outputs = new BrightContrastNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.brightcontrastnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "contrast":
                    /* brightcontrastnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'contrast', 'ui_name': 'Contrast'} */
                    {
                    CSycles.brightcontrastnode_set_contrast(this.Ptr, data);
                    }
                    break;
            case "bright":
                    /* brightcontrastnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'bright', 'ui_name': 'Bright'} */
                    {
                    CSycles.brightcontrastnode_set_bright(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrightContrastNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* brightcontrastnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.brightcontrastnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrightContrastNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "contrast":
                /* brightcontrastnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'contrast', 'ui_name': 'Contrast'} */
                {
                    return CSycles.brightcontrastnode_get_contrast(this.Ptr);
                }
            case "bright":
                /* brightcontrastnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'bright', 'ui_name': 'Bright'} */
                {
                    return CSycles.brightcontrastnode_get_bright(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrightContrastNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* brightcontrastnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.brightcontrastnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BrightContrastNode (getter)");
            }
        }

#endregion
    }

}