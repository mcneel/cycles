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

    public class VectorCurvesNodeInputs : Inputs
    {
        public VectorSocket Vector { get; private set; }
        public BoolSocket Extrapolate { get; private set; }
        public FloatSocket MinX { get; private set; }
        public FloatSocket Fac { get; private set; }
        public FloatSocket MaxX { get; private set; }
        public VectorArraySocket Curves { get; private set; }

        public VectorCurvesNodeInputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "value", true);
            AddSocket(Vector);
            Extrapolate = new BoolSocket(parentNode, "Extrapolate", "extrapolate", true);
            AddSocket(Extrapolate);
            MinX = new FloatSocket(parentNode, "Min X", "min_x", true);
            AddSocket(MinX);
            Fac = new FloatSocket(parentNode, "Fac", "fac", true);
            AddSocket(Fac);
            MaxX = new FloatSocket(parentNode, "Max X", "max_x", true);
            AddSocket(MaxX);
            Curves = new VectorArraySocket(parentNode, "Curves", "curves", true);
            AddSocket(Curves);
        }
    }
    public class VectorCurvesNodeOutputs : Outputs
    {
        public VectorSocket Vector { get; private set; }

        public VectorCurvesNodeOutputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "value", false);
            AddSocket(Vector);
        }
    }

    [ShaderNode(name: "vector_curves")]
    public class VectorCurvesNode : CurvesNode
    {
        public VectorCurvesNodeInputs ins => (VectorCurvesNodeInputs)inputs;
        public VectorCurvesNodeOutputs outs => (VectorCurvesNodeOutputs)outputs;
        public VectorCurvesNode(Shader shader) : this(shader, "a vector_curves node") { }

        public VectorCurvesNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal VectorCurvesNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new VectorCurvesNodeInputs(this);
            outputs = new VectorCurvesNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.vectorcurvesnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "min_x":
                    /* vectorcurvesnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'min_x', 'ui_name': 'Min X'} */
                    {
                    CSycles.curvesnode_set_min_x(this.Ptr, data);
                    }
                    break;
            case "fac":
                    /* vectorcurvesnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                    {
                    CSycles.curvesnode_set_fac(this.Ptr, data);
                    }
                    break;
            case "max_x":
                    /* vectorcurvesnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max_x', 'ui_name': 'Max X'} */
                    {
                    CSycles.curvesnode_set_max_x(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorCurvesNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "value":
                    /* vectorcurvesnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'value', 'ui_name': 'Vector'} */
                    {
                    CSycles.curvesnode_set_value(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorCurvesNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "extrapolate":
                    /* vectorcurvesnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'extrapolate', 'ui_name': 'Extrapolate'} */
                    {
                    CSycles.curvesnode_set_extrapolate(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorCurvesNode (setter)");
            }
        }

        internal override void SetVectorArray(string name, List<float3> data)
        {
            switch(name) {
            case "curves":
                    /* vectorcurvesnode . {'datatype': 'VECTOR_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'curves', 'ui_name': 'Curves'} */
                    {
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorCurvesNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "min_x":
                /* vectorcurvesnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'min_x', 'ui_name': 'Min X'} */
                {
                    return CSycles.curvesnode_get_min_x(this.Ptr);
                }
            case "fac":
                /* vectorcurvesnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                {
                    return CSycles.curvesnode_get_fac(this.Ptr);
                }
            case "max_x":
                /* vectorcurvesnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max_x', 'ui_name': 'Max X'} */
                {
                    return CSycles.curvesnode_get_max_x(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorCurvesNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "value":
                /* vectorcurvesnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'value', 'ui_name': 'Vector'} */
                {
                    return CSycles.curvesnode_get_value(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorCurvesNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "extrapolate":
                /* vectorcurvesnode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'extrapolate', 'ui_name': 'Extrapolate'} */
                {
                    return CSycles.curvesnode_get_extrapolate(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorCurvesNode (getter)");
            }
        }

        internal override List<float3> GetVectorArray(string name)
        {
            switch(name) {
            case "curves":
                /* vectorcurvesnode . {'datatype': 'VECTOR_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'curves', 'ui_name': 'Curves'} */
                {
                    return []; // NOTYET TODO
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorCurvesNode (getter)");
            }
        }

#endregion
    }

}