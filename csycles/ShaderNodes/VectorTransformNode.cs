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

    public class VectorTransformNodeInputs : Inputs
    {
        public VectorSocket Vector { get; private set; }
        public EnumSocket ConvertTo { get; private set; }
        public EnumSocket ConvertFrom { get; private set; }
        public EnumSocket Type { get; private set; }

        public VectorTransformNodeInputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            ConvertTo = new EnumSocket(parentNode, "Convert To", "convert_to", true);
            AddSocket(ConvertTo);
            ConvertFrom = new EnumSocket(parentNode, "Convert From", "convert_from", true);
            AddSocket(ConvertFrom);
            Type = new EnumSocket(parentNode, "Type", "transform_type", true);
            AddSocket(Type);
        }
    }
    public class VectorTransformNodeOutputs : Outputs
    {
        public VectorSocket Vector { get; private set; }

        public VectorTransformNodeOutputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "vector", false);
            AddSocket(Vector);
        }
    }

    [ShaderNode(name: "vector_transform")]
    public class VectorTransformNode : ShaderNode
    {
        public enum VectorTransformNodeSpace : uint {
            World = ccl.NodeVectorTransformConvertSpace.NODE_VECTOR_TRANSFORM_CONVERT_SPACE_WORLD,
            Object = ccl.NodeVectorTransformConvertSpace.NODE_VECTOR_TRANSFORM_CONVERT_SPACE_OBJECT,
            Camera = ccl.NodeVectorTransformConvertSpace.NODE_VECTOR_TRANSFORM_CONVERT_SPACE_CAMERA,
        }
        public enum VectorTransformNodeType : uint {
            Vector = ccl.NodeVectorTransformType.NODE_VECTOR_TRANSFORM_TYPE_VECTOR,
            Point = ccl.NodeVectorTransformType.NODE_VECTOR_TRANSFORM_TYPE_POINT,
            Normal = ccl.NodeVectorTransformType.NODE_VECTOR_TRANSFORM_TYPE_NORMAL,
        }
        public VectorTransformNodeInputs ins => (VectorTransformNodeInputs)inputs;
        public VectorTransformNodeOutputs outs => (VectorTransformNodeOutputs)outputs;
        public VectorTransformNode(Shader shader) : this(shader, "a vector_transform node") { }

        public VectorTransformNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal VectorTransformNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new VectorTransformNodeInputs(this);
            outputs = new VectorTransformNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.vectortransformnode_get_node_type();
        }
#region Setters

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* vectortransformnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.vectortransformnode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorTransformNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "convert_to":
                    /* vectortransformnode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_TRANSFORM_CONVERT_SPACE_OBJECT', 'default_value_type': 'NodeVectorTransformConvertSpace', 'is_input': True, 'member_name': 'convert_to', 'ui_name': 'Convert To'} */
                    {
                    CSycles.vectortransformnode_set_convert_to(this.Ptr, (ccl.NodeVectorTransformConvertSpace)data);
                    }
                    break;
            case "convert_from":
                    /* vectortransformnode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_TRANSFORM_CONVERT_SPACE_WORLD', 'default_value_type': 'NodeVectorTransformConvertSpace', 'is_input': True, 'member_name': 'convert_from', 'ui_name': 'Convert From'} */
                    {
                    CSycles.vectortransformnode_set_convert_from(this.Ptr, (ccl.NodeVectorTransformConvertSpace)data);
                    }
                    break;
            case "transform_type":
                    /* vectortransformnode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_TRANSFORM_TYPE_VECTOR', 'default_value_type': 'NodeVectorTransformType', 'is_input': True, 'member_name': 'transform_type', 'ui_name': 'Type'} */
                    {
                    CSycles.vectortransformnode_set_transform_type(this.Ptr, (ccl.NodeVectorTransformType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorTransformNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "vector":
                /* vectortransformnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.vectortransformnode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorTransformNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "convert_to":
                /* vectortransformnode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_TRANSFORM_CONVERT_SPACE_OBJECT', 'default_value_type': 'NodeVectorTransformConvertSpace', 'is_input': True, 'member_name': 'convert_to', 'ui_name': 'Convert To'} */
                {
                    return (uint)CSycles.vectortransformnode_get_convert_to(this.Ptr);
                }
            case "convert_from":
                /* vectortransformnode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_TRANSFORM_CONVERT_SPACE_WORLD', 'default_value_type': 'NodeVectorTransformConvertSpace', 'is_input': True, 'member_name': 'convert_from', 'ui_name': 'Convert From'} */
                {
                    return (uint)CSycles.vectortransformnode_get_convert_from(this.Ptr);
                }
            case "transform_type":
                /* vectortransformnode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_TRANSFORM_TYPE_VECTOR', 'default_value_type': 'NodeVectorTransformType', 'is_input': True, 'member_name': 'transform_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.vectortransformnode_get_transform_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorTransformNode (getter)");
            }
        }

#endregion
    }

}