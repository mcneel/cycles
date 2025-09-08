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

    public class VectorRotateNodeInputs : Inputs
    {
        public EnumSocket Type { get; private set; }
        public VectorSocket Axis { get; private set; }
        public PointSocket Rotation { get; private set; }
        public BoolSocket Invert { get; private set; }
        public FloatSocket Angle { get; private set; }
        public PointSocket Center { get; private set; }
        public VectorSocket Vector { get; private set; }

        public VectorRotateNodeInputs(ShaderNode parentNode)
        {
            Type = new EnumSocket(parentNode, "Type", "rotate_type", true);
            AddSocket(Type);
            Axis = new VectorSocket(parentNode, "Axis", "axis", true);
            AddSocket(Axis);
            Rotation = new PointSocket(parentNode, "Rotation", "rotation", true);
            AddSocket(Rotation);
            Invert = new BoolSocket(parentNode, "Invert", "invert", true);
            AddSocket(Invert);
            Angle = new FloatSocket(parentNode, "Angle", "angle", true);
            AddSocket(Angle);
            Center = new PointSocket(parentNode, "Center", "center", true);
            AddSocket(Center);
            Vector = new VectorSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
        }
    }
    public class VectorRotateNodeOutputs : Outputs
    {
        public VectorSocket Vector { get; private set; }

        public VectorRotateNodeOutputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "vector", false);
            AddSocket(Vector);
        }
    }

    [ShaderNode(name: "vector_rotate")]
    public class VectorRotateNode : ShaderNode
    {
        public enum VectorRotateNodeType : uint {
            Axis = ccl.NodeVectorRotateType.NODE_VECTOR_ROTATE_TYPE_AXIS,
            XAxis = ccl.NodeVectorRotateType.NODE_VECTOR_ROTATE_TYPE_AXIS_X,
            YAxis = ccl.NodeVectorRotateType.NODE_VECTOR_ROTATE_TYPE_AXIS_Y,
            ZAxis = ccl.NodeVectorRotateType.NODE_VECTOR_ROTATE_TYPE_AXIS_Z,
            EulerXyz = ccl.NodeVectorRotateType.NODE_VECTOR_ROTATE_TYPE_EULER_XYZ,
        }
        public VectorRotateNodeInputs ins => (VectorRotateNodeInputs)inputs;
        public VectorRotateNodeOutputs outs => (VectorRotateNodeOutputs)outputs;
        public VectorRotateNode(Shader shader) : this(shader, "a vector_rotate node") { }

        public VectorRotateNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal VectorRotateNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new VectorRotateNodeInputs(this);
            outputs = new VectorRotateNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.vectorrotatenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "angle":
                    /* vectorrotatenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'angle', 'ui_name': 'Angle'} */
                    {
                    CSycles.vectorrotatenode_set_angle(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "axis":
                    /* vectorrotatenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,1.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'axis', 'ui_name': 'Axis'} */
                    {
                    CSycles.vectorrotatenode_set_axis(this.Ptr, data);
                    }
                    break;
            case "vector":
                    /* vectorrotatenode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.vectorrotatenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "rotation":
                    /* vectorrotatenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'rotation', 'ui_name': 'Rotation'} */
                    {
                    CSycles.vectorrotatenode_set_rotation(this.Ptr, data);
                    }
                    break;
            case "center":
                    /* vectorrotatenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'center', 'ui_name': 'Center'} */
                    {
                    CSycles.vectorrotatenode_set_center(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "invert":
                    /* vectorrotatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'invert', 'ui_name': 'Invert'} */
                    {
                    CSycles.vectorrotatenode_set_invert(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "rotate_type":
                    /* vectorrotatenode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_ROTATE_TYPE_AXIS', 'default_value_type': 'NodeVectorRotateType', 'is_input': True, 'member_name': 'rotate_type', 'ui_name': 'Type'} */
                    {
                    CSycles.vectorrotatenode_set_rotate_type(this.Ptr, (ccl.NodeVectorRotateType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "angle":
                /* vectorrotatenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'angle', 'ui_name': 'Angle'} */
                {
                    return CSycles.vectorrotatenode_get_angle(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "axis":
                /* vectorrotatenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,1.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'axis', 'ui_name': 'Axis'} */
                {
                    return CSycles.vectorrotatenode_get_axis(this.Ptr);
                }
            case "vector":
                /* vectorrotatenode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.vectorrotatenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "rotation":
                /* vectorrotatenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'rotation', 'ui_name': 'Rotation'} */
                {
                    return CSycles.vectorrotatenode_get_rotation(this.Ptr);
                }
            case "center":
                /* vectorrotatenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'center', 'ui_name': 'Center'} */
                {
                    return CSycles.vectorrotatenode_get_center(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "invert":
                /* vectorrotatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'invert', 'ui_name': 'Invert'} */
                {
                    return CSycles.vectorrotatenode_get_invert(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "rotate_type":
                /* vectorrotatenode . {'datatype': 'ENUM', 'default_value': 'NODE_VECTOR_ROTATE_TYPE_AXIS', 'default_value_type': 'NodeVectorRotateType', 'is_input': True, 'member_name': 'rotate_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.vectorrotatenode_get_rotate_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorRotateNode (getter)");
            }
        }

#endregion
    }

}