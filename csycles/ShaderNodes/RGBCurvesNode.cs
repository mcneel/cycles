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

    public class RGBCurvesNodeInputs : Inputs
    {
        public ColorSocket Color { get; private set; }
        public BoolSocket Extrapolate { get; private set; }
        public FloatSocket MinX { get; private set; }
        public FloatSocket Fac { get; private set; }
        public FloatSocket MaxX { get; private set; }
        public ColorArraySocket Curves { get; private set; }

        public RGBCurvesNodeInputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "value", true);
            AddSocket(Color);
            Extrapolate = new BoolSocket(parentNode, "Extrapolate", "extrapolate", true);
            AddSocket(Extrapolate);
            MinX = new FloatSocket(parentNode, "Min X", "min_x", true);
            AddSocket(MinX);
            Fac = new FloatSocket(parentNode, "Fac", "fac", true);
            AddSocket(Fac);
            MaxX = new FloatSocket(parentNode, "Max X", "max_x", true);
            AddSocket(MaxX);
            Curves = new ColorArraySocket(parentNode, "Curves", "curves", true);
            AddSocket(Curves);
        }
    }
    public class RGBCurvesNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public RGBCurvesNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "value", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rgb_curves")]
    public class RGBCurvesNode : CurvesNode
    {
        public RGBCurvesNodeInputs ins => (RGBCurvesNodeInputs)inputs;
        public RGBCurvesNodeOutputs outs => (RGBCurvesNodeOutputs)outputs;
        public RGBCurvesNode(Shader shader) : this(shader, "a rgb_curves node") { }

        public RGBCurvesNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RGBCurvesNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RGBCurvesNodeInputs(this);
            outputs = new RGBCurvesNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.rgbcurvesnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "min_x":
                    /* rgbcurvesnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'min_x', 'ui_name': 'Min X'} */
                    {
                    CSycles.curvesnode_set_min_x(this.Ptr, data);
                    }
                    break;
            case "fac":
                    /* rgbcurvesnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                    {
                    CSycles.curvesnode_set_fac(this.Ptr, data);
                    }
                    break;
            case "max_x":
                    /* rgbcurvesnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max_x', 'ui_name': 'Max X'} */
                    {
                    CSycles.curvesnode_set_max_x(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBCurvesNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "value":
                    /* rgbcurvesnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'value', 'ui_name': 'Color'} */
                    {
                    CSycles.curvesnode_set_value(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBCurvesNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "extrapolate":
                    /* rgbcurvesnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'extrapolate', 'ui_name': 'Extrapolate'} */
                    {
                    CSycles.curvesnode_set_extrapolate(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBCurvesNode (setter)");
            }
        }

        internal override void SetColorArray(string name, List<float3> data)
        {
            switch(name) {
            case "curves":
                    /* rgbcurvesnode . {'datatype': 'COLOR_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'curves', 'ui_name': 'Curves'} */
                    {
                    // NOTYET CSycles.curvesnode_set_curves(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBCurvesNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "min_x":
                /* rgbcurvesnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'min_x', 'ui_name': 'Min X'} */
                {
                    return CSycles.curvesnode_get_min_x(this.Ptr);
                }
            case "fac":
                /* rgbcurvesnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                {
                    return CSycles.curvesnode_get_fac(this.Ptr);
                }
            case "max_x":
                /* rgbcurvesnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max_x', 'ui_name': 'Max X'} */
                {
                    return CSycles.curvesnode_get_max_x(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBCurvesNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "value":
                /* rgbcurvesnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'value', 'ui_name': 'Color'} */
                {
                    return CSycles.curvesnode_get_value(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBCurvesNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "extrapolate":
                /* rgbcurvesnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'extrapolate', 'ui_name': 'Extrapolate'} */
                {
                    return CSycles.curvesnode_get_extrapolate(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBCurvesNode (getter)");
            }
        }

        internal override List<float3> GetColorArray(string name)
        {
            switch(name) {
            case "curves":
                /* rgbcurvesnode . {'datatype': 'COLOR_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'curves', 'ui_name': 'Curves'} */
                {
                    // NOTYET return CSycles.curvesnode_get_curves(this.Ptr);
                    return [];
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RGBCurvesNode (getter)");
            }
        }

#endregion
    }

}