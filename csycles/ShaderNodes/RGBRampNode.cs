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

    public class RGBRampNodeInputs : Inputs
    {
        public ColorArraySocket Ramp { get; private set; }
        public FloatSocket Fac { get; private set; }
        public BoolSocket Interpolate { get; private set; }
        public FloatArraySocket RampAlpha { get; private set; }

        public RGBRampNodeInputs(ShaderNode parentNode)
        {
            Ramp = new ColorArraySocket(parentNode, "Ramp", "ramp", true);
            AddSocket(Ramp);
            Fac = new FloatSocket(parentNode, "Fac", "fac", true);
            AddSocket(Fac);
            Interpolate = new BoolSocket(parentNode, "Interpolate", "interpolate", true);
            AddSocket(Interpolate);
            RampAlpha = new FloatArraySocket(parentNode, "Ramp Alpha", "ramp_alpha", true);
            AddSocket(RampAlpha);
        }
    }
    public class RGBRampNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RGBRampNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rgb_ramp")]
    public class RGBRampNode : ShaderNode
    {
        public RGBRampNodeInputs ins => (RGBRampNodeInputs)inputs;
        public RGBRampNodeOutputs outs => (RGBRampNodeOutputs)outputs;
        public RGBRampNode(Shader shader) : this(shader, "a rgb_ramp node") { }

        public RGBRampNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RGBRampNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RGBRampNodeInputs(this);
            outputs = new RGBRampNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.rgbrampnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "fac":
                    /* rgbrampnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                    {
                    CSycles.rgbrampnode_set_fac(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBRampNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "interpolate":
                    /* rgbrampnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'interpolate', 'ui_name': 'Interpolate'} */
                    {
                    CSycles.rgbrampnode_set_interpolate(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBRampNode (setter)");
            }
        }

        internal override void SetFloatArray(string name, List<float> data)
        {
            switch(name) {
            case "ramp_alpha":
                    /* rgbrampnode . {'datatype': 'FLOAT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'ramp_alpha', 'ui_name': 'Ramp Alpha'} */
                    {
                    CSycles.rgbrampnode_set_ramp_alpha(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBRampNode (setter)");
            }
        }

        internal override void SetColorArray(string name, List<float3> data)
        {
            switch(name) {
            case "ramp":
                    /* rgbrampnode . {'datatype': 'COLOR_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'ramp', 'ui_name': 'Ramp'} */
                    {
                    CSycles.rgbrampnode_set_ramp(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBRampNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "fac":
                /* rgbrampnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                {
                    return CSycles.rgbrampnode_get_fac(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBRampNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "interpolate":
                /* rgbrampnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'interpolate', 'ui_name': 'Interpolate'} */
                {
                    return CSycles.rgbrampnode_get_interpolate(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBRampNode (getter)");
            }
        }

        internal override List<float> GetFloatArray(string name)
        {
            switch(name) {
            case "ramp_alpha":
                /* rgbrampnode . {'datatype': 'FLOAT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'ramp_alpha', 'ui_name': 'Ramp Alpha'} */
                {
                    return CSycles.rgbrampnode_get_ramp_alpha(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBRampNode (getter)");
            }
        }

        internal override List<float3> GetColorArray(string name)
        {
            switch(name) {
            case "ramp":
                /* rgbrampnode . {'datatype': 'COLOR_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'ramp', 'ui_name': 'Ramp'} */
                {
                    return CSycles.rgbrampnode_get_ramp(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBRampNode (getter)");
            }
        }

#endregion
    }

}