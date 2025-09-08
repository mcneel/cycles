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

    public class HSVNodeInputs : Inputs
    {
        public FloatSocket Hue { get; private set; }
        public FloatSocket Fac { get; private set; }
        public FloatSocket Saturation { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket Value { get; private set; }

        public HSVNodeInputs(ShaderNode parentNode)
        {
            Hue = new FloatSocket(parentNode, "Hue", "hue", true);
            AddSocket(Hue);
            Fac = new FloatSocket(parentNode, "Fac", "fac", true);
            AddSocket(Fac);
            Saturation = new FloatSocket(parentNode, "Saturation", "saturation", true);
            AddSocket(Saturation);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Value = new FloatSocket(parentNode, "Value", "value", true);
            AddSocket(Value);
        }
    }
    public class HSVNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public HSVNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "hsv")]
    public class HSVNode : ShaderNode
    {
        public HSVNodeInputs ins => (HSVNodeInputs)inputs;
        public HSVNodeOutputs outs => (HSVNodeOutputs)outputs;
        public HSVNode(Shader shader) : this(shader, "a hsv node") { }

        public HSVNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal HSVNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new HSVNodeInputs(this);
            outputs = new HSVNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.hsvnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "hue":
                    /* hsvnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'hue', 'ui_name': 'Hue'} */
                    {
                    CSycles.hsvnode_set_hue(this.Ptr, data);
                    }
                    break;
            case "fac":
                    /* hsvnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                    {
                    CSycles.hsvnode_set_fac(this.Ptr, data);
                    }
                    break;
            case "saturation":
                    /* hsvnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'saturation', 'ui_name': 'Saturation'} */
                    {
                    CSycles.hsvnode_set_saturation(this.Ptr, data);
                    }
                    break;
            case "value":
                    /* hsvnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                    {
                    CSycles.hsvnode_set_value(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type HSVNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* hsvnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.hsvnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type HSVNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "hue":
                /* hsvnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'hue', 'ui_name': 'Hue'} */
                {
                    return CSycles.hsvnode_get_hue(this.Ptr);
                }
            case "fac":
                /* hsvnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                {
                    return CSycles.hsvnode_get_fac(this.Ptr);
                }
            case "saturation":
                /* hsvnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'saturation', 'ui_name': 'Saturation'} */
                {
                    return CSycles.hsvnode_get_saturation(this.Ptr);
                }
            case "value":
                /* hsvnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                {
                    return CSycles.hsvnode_get_value(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type HSVNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* hsvnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.hsvnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type HSVNode (getter)");
            }
        }

#endregion
    }

}