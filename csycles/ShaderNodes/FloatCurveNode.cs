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

    public class FloatCurveNodeInputs : Inputs
    {
        public FloatSocket Value { get; private set; }
        public BoolSocket Extrapolate { get; private set; }
        public FloatSocket MinX { get; private set; }
        public FloatSocket Factor { get; private set; }
        public FloatSocket MaxX { get; private set; }
        public FloatArraySocket Curve { get; private set; }

        public FloatCurveNodeInputs(ShaderNode parentNode)
        {
            Value = new FloatSocket(parentNode, "Value", "value", true);
            AddSocket(Value);
            Extrapolate = new BoolSocket(parentNode, "Extrapolate", "extrapolate", true);
            AddSocket(Extrapolate);
            MinX = new FloatSocket(parentNode, "Min X", "min_x", true);
            AddSocket(MinX);
            Factor = new FloatSocket(parentNode, "Factor", "fac", true);
            AddSocket(Factor);
            MaxX = new FloatSocket(parentNode, "Max X", "max_x", true);
            AddSocket(MaxX);
            Curve = new FloatArraySocket(parentNode, "Curve", "curve", true);
            AddSocket(Curve);
        }
    }
    public class FloatCurveNodeOutputs : Outputs
    {
        public FloatSocket Value { get; private set; }

        public FloatCurveNodeOutputs(ShaderNode parentNode)
        {
            Value = new FloatSocket(parentNode, "Value", "value", false);
            AddSocket(Value);
        }
    }

    [ShaderNode(name: "float_curve")]
    public class FloatCurveNode : ShaderNode
    {
        public FloatCurveNodeInputs ins => (FloatCurveNodeInputs)inputs;
        public FloatCurveNodeOutputs outs => (FloatCurveNodeOutputs)outputs;
        public FloatCurveNode(Shader shader) : this(shader, "a float_curve node") { }

        public FloatCurveNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal FloatCurveNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new FloatCurveNodeInputs(this);
            outputs = new FloatCurveNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.floatcurvenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "value":
                    /* floatcurvenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                    {
                    CSycles.floatcurvenode_set_value(this.Ptr, data);
                    }
                    break;
            case "min_x":
                    /* floatcurvenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'min_x', 'ui_name': 'Min X'} */
                    {
                    CSycles.floatcurvenode_set_min_x(this.Ptr, data);
                    }
                    break;
            case "fac":
                    /* floatcurvenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                    {
                    CSycles.floatcurvenode_set_fac(this.Ptr, data);
                    }
                    break;
            case "max_x":
                    /* floatcurvenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max_x', 'ui_name': 'Max X'} */
                    {
                    CSycles.floatcurvenode_set_max_x(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FloatCurveNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "extrapolate":
                    /* floatcurvenode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'extrapolate', 'ui_name': 'Extrapolate'} */
                    {
                    CSycles.floatcurvenode_set_extrapolate(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FloatCurveNode (setter)");
            }
        }

        internal override void SetFloatArray(string name, List<float> data)
        {
            switch(name) {
            case "curve":
                    /* floatcurvenode . {'datatype': 'FLOAT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'curve', 'ui_name': 'Curve'} */
                    {
                    CSycles.floatcurvenode_set_curve(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FloatCurveNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "value":
                /* floatcurvenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                {
                    return CSycles.floatcurvenode_get_value(this.Ptr);
                }
            case "min_x":
                /* floatcurvenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'min_x', 'ui_name': 'Min X'} */
                {
                    return CSycles.floatcurvenode_get_min_x(this.Ptr);
                }
            case "fac":
                /* floatcurvenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                {
                    return CSycles.floatcurvenode_get_fac(this.Ptr);
                }
            case "max_x":
                /* floatcurvenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max_x', 'ui_name': 'Max X'} */
                {
                    return CSycles.floatcurvenode_get_max_x(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FloatCurveNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "extrapolate":
                /* floatcurvenode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'extrapolate', 'ui_name': 'Extrapolate'} */
                {
                    return CSycles.floatcurvenode_get_extrapolate(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FloatCurveNode (getter)");
            }
        }

        internal override List<float> GetFloatArray(string name)
        {
            switch(name) {
            case "curve":
                /* floatcurvenode . {'datatype': 'FLOAT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'curve', 'ui_name': 'Curve'} */
                {
                    return CSycles.floatcurvenode_get_curve(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type FloatCurveNode (getter)");
            }
        }

#endregion
    }

}