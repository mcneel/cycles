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

    public class MatrixMathNodeInputs : Inputs
    {
        public VectorSocket Vector { get; private set; }
        public TransformSocket Transform { get; private set; }
        public EnumSocket Type { get; private set; }

        public MatrixMathNodeInputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Transform = new TransformSocket(parentNode, "Transform", "tfm", true);
            AddSocket(Transform);
            Type = new EnumSocket(parentNode, "Type", "type", true);
            AddSocket(Type);
        }
    }
    public class MatrixMathNodeOutputs : Outputs
    {
        public VectorSocket Vector { get; private set; }

        public MatrixMathNodeOutputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "vector", false);
            AddSocket(Vector);
        }
    }

    [ShaderNode(name: "matrix_math")]
    public class MatrixMathNode : ShaderNode
    {
        public enum MatrixMathNodeType : uint {
            Point = ccl.NodeMatrixMath.NODE_MATRIX_MATH_POINT,
            Direction = ccl.NodeMatrixMath.NODE_MATRIX_MATH_DIRECTION,
            Perspective = ccl.NodeMatrixMath.NODE_MATRIX_MATH_PERSPECTIVE,
            DirectionTransposed = ccl.NodeMatrixMath.NODE_MATRIX_MATH_DIR_TRANSPOSED,
        }
        public MatrixMathNodeInputs ins => (MatrixMathNodeInputs)inputs;
        public MatrixMathNodeOutputs outs => (MatrixMathNodeOutputs)outputs;
        public MatrixMathNode(Shader shader) : this(shader, "a matrix_math node") { }

        public MatrixMathNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MatrixMathNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MatrixMathNodeInputs(this);
            outputs = new MatrixMathNodeOutputs(this);
        }
        public Transform Tfm {
            get { return CSycles.matrixmathnode_get_tfm(Ptr); }
            set { CSycles.matrixmathnode_set_tfm(Ptr, value); }
        }

        public float3 Vector {
            get { return CSycles.matrixmathnode_get_vector(Ptr); }
            set { CSycles.matrixmathnode_set_vector(Ptr, value); }
        }

        public NodeMatrixMath Type {
            get { return CSycles.matrixmathnode_get_type(Ptr); }
            set { CSycles.matrixmathnode_set_type(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.matrixmathnode_get_node_type();
        }
#region Setters

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* matrixmathnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.matrixmathnode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MatrixMathNode (setter)");
            }
        }

        internal override void SetTransform(string name, Transform data)
        {
            switch(name) {
            case "tfm":
                    /* matrixmathnode . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'tfm', 'ui_name': 'Transform'} */
                    {
                    CSycles.matrixmathnode_set_tfm(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MatrixMathNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "type":
                    /* matrixmathnode . {'datatype': 'ENUM', 'default_value': 'NODE_MATRIX_MATH_POINT', 'default_value_type': 'NodeMatrixMath', 'is_input': True, 'member_name': 'type', 'ui_name': 'Type'} */
                    {
                    CSycles.matrixmathnode_set_type(this.Ptr, (ccl.NodeMatrixMath)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MatrixMathNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "vector":
                /* matrixmathnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.matrixmathnode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MatrixMathNode (getter)");
            }
        }

        internal override Transform GetTransform(string name)
        {
            switch(name) {
            case "tfm":
                /* matrixmathnode . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'tfm', 'ui_name': 'Transform'} */
                {
                    return CSycles.matrixmathnode_get_tfm(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MatrixMathNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "type":
                /* matrixmathnode . {'datatype': 'ENUM', 'default_value': 'NODE_MATRIX_MATH_POINT', 'default_value_type': 'NodeMatrixMath', 'is_input': True, 'member_name': 'type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.matrixmathnode_get_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MatrixMathNode (getter)");
            }
        }

#endregion
    }

}