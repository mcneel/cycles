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

    public class VectorDisplacementNodeInputs : Inputs
    {
        public FloatSocket Scale { get; private set; }
        public ColorSocket Vector { get; private set; }
        public EnumSocket Space { get; private set; }
        public FloatSocket Midlevel { get; private set; }

        public VectorDisplacementNodeInputs(ShaderNode parentNode)
        {
            Scale = new FloatSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
            Vector = new ColorSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Space = new EnumSocket(parentNode, "Space", "space", true);
            AddSocket(Space);
            Midlevel = new FloatSocket(parentNode, "Midlevel", "midlevel", true);
            AddSocket(Midlevel);
        }
    }
    public class VectorDisplacementNodeOutputs : Outputs
    {
        public VectorSocket Displacement { get; private set; }

        public VectorDisplacementNodeOutputs(ShaderNode parentNode)
        {
            Displacement = new VectorSocket(parentNode, "Displacement", "displacement", false);
            AddSocket(Displacement);
        }
    }

    [ShaderNode(name: "vector_displacement")]
    public class VectorDisplacementNode : ShaderNode
    {
        public enum VectorDisplacementNodeSpace : uint {
            Tangent = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_TANGENT,
            Object = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_OBJECT,
            World = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_WORLD,
        }
        public VectorDisplacementNodeInputs ins => (VectorDisplacementNodeInputs)inputs;
        public VectorDisplacementNodeOutputs outs => (VectorDisplacementNodeOutputs)outputs;
        public VectorDisplacementNode(Shader shader) : this(shader, "a vector_displacement node") { }

        public VectorDisplacementNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal VectorDisplacementNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new VectorDisplacementNodeInputs(this);
            outputs = new VectorDisplacementNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.vectordisplacementnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "scale":
                    /* vectordisplacementnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.vectordisplacementnode_set_scale(this.Ptr, data);
                    }
                    break;
            case "midlevel":
                    /* vectordisplacementnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'midlevel', 'ui_name': 'Midlevel'} */
                    {
                    CSycles.vectordisplacementnode_set_midlevel(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorDisplacementNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* vectordisplacementnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.vectordisplacementnode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorDisplacementNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "space":
                    /* vectordisplacementnode . {'datatype': 'ENUM', 'default_value': 'NODE_NORMAL_MAP_TANGENT', 'default_value_type': 'NodeNormalMapSpace', 'is_input': True, 'member_name': 'space', 'ui_name': 'Space'} */
                    {
                    CSycles.vectordisplacementnode_set_space(this.Ptr, (ccl.NodeNormalMapSpace)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorDisplacementNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "scale":
                /* vectordisplacementnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.vectordisplacementnode_get_scale(this.Ptr);
                }
            case "midlevel":
                /* vectordisplacementnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'midlevel', 'ui_name': 'Midlevel'} */
                {
                    return CSycles.vectordisplacementnode_get_midlevel(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorDisplacementNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "vector":
                /* vectordisplacementnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.vectordisplacementnode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorDisplacementNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "space":
                /* vectordisplacementnode . {'datatype': 'ENUM', 'default_value': 'NODE_NORMAL_MAP_TANGENT', 'default_value_type': 'NodeNormalMapSpace', 'is_input': True, 'member_name': 'space', 'ui_name': 'Space'} */
                {
                    return (uint)CSycles.vectordisplacementnode_get_space(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorDisplacementNode (getter)");
            }
        }

#endregion
    }

}