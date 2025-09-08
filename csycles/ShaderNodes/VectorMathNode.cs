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

    public class VectorMathNodeInputs : Inputs
    {
        public VectorSocket Vector2 { get; private set; }
        public EnumSocket Type { get; private set; }
        public VectorSocket Vector3 { get; private set; }
        public VectorSocket Vector1 { get; private set; }
        public FloatSocket Scale { get; private set; }

        public VectorMathNodeInputs(ShaderNode parentNode)
        {
            Vector2 = new VectorSocket(parentNode, "Vector2", "vector2", true);
            AddSocket(Vector2);
            Type = new EnumSocket(parentNode, "Type", "math_type", true);
            AddSocket(Type);
            Vector3 = new VectorSocket(parentNode, "Vector3", "vector3", true);
            AddSocket(Vector3);
            Vector1 = new VectorSocket(parentNode, "Vector1", "vector1", true);
            AddSocket(Vector1);
            Scale = new FloatSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
        }
    }
    public class VectorMathNodeOutputs : Outputs
    {
        public VectorSocket Vector { get; private set; }
        public FloatSocket Value { get; private set; }

        public VectorMathNodeOutputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "vector", false);
            AddSocket(Vector);
            Value = new FloatSocket(parentNode, "Value", "value", false);
            AddSocket(Value);
        }
    }

    [ShaderNode(name: "vector_math")]
    public class VectorMathNode : ShaderNode
    {
        public enum VectorMathNodeType : uint {
            Add = ccl.NodeVectorMathType.NODE_VECTOR_MATH_ADD,
            Subtract = ccl.NodeVectorMathType.NODE_VECTOR_MATH_SUBTRACT,
            Multiply = ccl.NodeVectorMathType.NODE_VECTOR_MATH_MULTIPLY,
            Divide = ccl.NodeVectorMathType.NODE_VECTOR_MATH_DIVIDE,
            CrossProduct = ccl.NodeVectorMathType.NODE_VECTOR_MATH_CROSS_PRODUCT,
            Project = ccl.NodeVectorMathType.NODE_VECTOR_MATH_PROJECT,
            Reflect = ccl.NodeVectorMathType.NODE_VECTOR_MATH_REFLECT,
            DotProduct = ccl.NodeVectorMathType.NODE_VECTOR_MATH_DOT_PRODUCT,
            Distance = ccl.NodeVectorMathType.NODE_VECTOR_MATH_DISTANCE,
            Length = ccl.NodeVectorMathType.NODE_VECTOR_MATH_LENGTH,
            Scale = ccl.NodeVectorMathType.NODE_VECTOR_MATH_SCALE,
            Normalize = ccl.NodeVectorMathType.NODE_VECTOR_MATH_NORMALIZE,
            Snap = ccl.NodeVectorMathType.NODE_VECTOR_MATH_SNAP,
            Floor = ccl.NodeVectorMathType.NODE_VECTOR_MATH_FLOOR,
            Ceil = ccl.NodeVectorMathType.NODE_VECTOR_MATH_CEIL,
            Modulo = ccl.NodeVectorMathType.NODE_VECTOR_MATH_MODULO,
            Fraction = ccl.NodeVectorMathType.NODE_VECTOR_MATH_FRACTION,
            Absolute = ccl.NodeVectorMathType.NODE_VECTOR_MATH_ABSOLUTE,
            Minimum = ccl.NodeVectorMathType.NODE_VECTOR_MATH_MINIMUM,
            Maximum = ccl.NodeVectorMathType.NODE_VECTOR_MATH_MAXIMUM,
            Wrap = ccl.NodeVectorMathType.NODE_VECTOR_MATH_WRAP,
            Sine = ccl.NodeVectorMathType.NODE_VECTOR_MATH_SINE,
            Cosine = ccl.NodeVectorMathType.NODE_VECTOR_MATH_COSINE,
            Tangent = ccl.NodeVectorMathType.NODE_VECTOR_MATH_TANGENT,
            Refract = ccl.NodeVectorMathType.NODE_VECTOR_MATH_REFRACT,
            Faceforward = ccl.NodeVectorMathType.NODE_VECTOR_MATH_FACEFORWARD,
            MultiplyAdd = ccl.NodeVectorMathType.NODE_VECTOR_MATH_MULTIPLY_ADD,
        }
        public VectorMathNodeInputs ins => (VectorMathNodeInputs)inputs;
        public VectorMathNodeOutputs outs => (VectorMathNodeOutputs)outputs;
        public VectorMathNode(Shader shader) : this(shader, "a vector_math node") { }

        public VectorMathNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal VectorMathNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new VectorMathNodeInputs(this);
            outputs = new VectorMathNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.vectormathnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "scale":
                    /* vectormathnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.vectormathnode_set_scale(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMathNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "vector2":
                    /* vectormathnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector2', 'ui_name': 'Vector2'} */
                    {
                    CSycles.vectormathnode_set_vector2(this.Ptr, data);
                    }
                    break;
            case "vector3":
                    /* vectormathnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector3', 'ui_name': 'Vector3'} */
                    {
                    CSycles.vectormathnode_set_vector3(this.Ptr, data);
                    }
                    break;
            case "vector1":
                    /* vectormathnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector1', 'ui_name': 'Vector1'} */
                    {
                    CSycles.vectormathnode_set_vector1(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMathNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "math_type":
                    /* vectormathnode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_MATH_ADD', 'default_value_type': 'NodeVectorMathType', 'is_input': True, 'member_name': 'math_type', 'ui_name': 'Type'} */
                    {
                    CSycles.vectormathnode_set_math_type(this.Ptr, (ccl.NodeVectorMathType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMathNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "scale":
                /* vectormathnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.vectormathnode_get_scale(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMathNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "vector2":
                /* vectormathnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector2', 'ui_name': 'Vector2'} */
                {
                    return CSycles.vectormathnode_get_vector2(this.Ptr);
                }
            case "vector3":
                /* vectormathnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector3', 'ui_name': 'Vector3'} */
                {
                    return CSycles.vectormathnode_get_vector3(this.Ptr);
                }
            case "vector1":
                /* vectormathnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector1', 'ui_name': 'Vector1'} */
                {
                    return CSycles.vectormathnode_get_vector1(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMathNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "math_type":
                /* vectormathnode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_MATH_ADD', 'default_value_type': 'NodeVectorMathType', 'is_input': True, 'member_name': 'math_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.vectormathnode_get_math_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMathNode (getter)");
            }
        }

#endregion
    }

}