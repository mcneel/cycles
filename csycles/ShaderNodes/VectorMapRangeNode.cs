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

    public class VectorMapRangeNodeInputs : Inputs
    {
        public VectorSocket From_Min_FLOAT3 { get; private set; }
        public BoolSocket UseClamp { get; private set; }
        public VectorSocket From_Max_FLOAT3 { get; private set; }
        public VectorSocket To_Min_FLOAT3 { get; private set; }
        public EnumSocket Type { get; private set; }
        public VectorSocket To_Max_FLOAT3 { get; private set; }
        public VectorSocket Vector { get; private set; }
        public VectorSocket Steps_FLOAT3 { get; private set; }

        public VectorMapRangeNodeInputs(ShaderNode parentNode)
        {
            From_Min_FLOAT3 = new VectorSocket(parentNode, "From_Min_FLOAT3", "from_min", true);
            AddSocket(From_Min_FLOAT3);
            UseClamp = new BoolSocket(parentNode, "Use Clamp", "use_clamp", true);
            AddSocket(UseClamp);
            From_Max_FLOAT3 = new VectorSocket(parentNode, "From_Max_FLOAT3", "from_max", true);
            AddSocket(From_Max_FLOAT3);
            To_Min_FLOAT3 = new VectorSocket(parentNode, "To_Min_FLOAT3", "to_min", true);
            AddSocket(To_Min_FLOAT3);
            Type = new EnumSocket(parentNode, "Type", "range_type", true);
            AddSocket(Type);
            To_Max_FLOAT3 = new VectorSocket(parentNode, "To_Max_FLOAT3", "to_max", true);
            AddSocket(To_Max_FLOAT3);
            Vector = new VectorSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Steps_FLOAT3 = new VectorSocket(parentNode, "Steps_FLOAT3", "steps", true);
            AddSocket(Steps_FLOAT3);
        }
    }
    public class VectorMapRangeNodeOutputs : Outputs
    {
        public VectorSocket Vector { get; private set; }

        public VectorMapRangeNodeOutputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "vector", false);
            AddSocket(Vector);
        }
    }

    [ShaderNode(name: "vector_map_range")]
    public class VectorMapRangeNode : ShaderNode
    {
        public enum VectorMapRangeNodeType : uint {
            Linear = ccl.NodeMapRangeType.NODE_MAP_RANGE_LINEAR,
            Stepped = ccl.NodeMapRangeType.NODE_MAP_RANGE_STEPPED,
            Smoothstep = ccl.NodeMapRangeType.NODE_MAP_RANGE_SMOOTHSTEP,
            Smootherstep = ccl.NodeMapRangeType.NODE_MAP_RANGE_SMOOTHERSTEP,
        }
        public VectorMapRangeNodeInputs ins => (VectorMapRangeNodeInputs)inputs;
        public VectorMapRangeNodeOutputs outs => (VectorMapRangeNodeOutputs)outputs;
        public VectorMapRangeNode(Shader shader) : this(shader, "a vector_map_range node") { }

        public VectorMapRangeNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal VectorMapRangeNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new VectorMapRangeNodeInputs(this);
            outputs = new VectorMapRangeNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.vectormaprangenode_get_node_type();
        }
#region Setters

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "from_min":
                    /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'from_min', 'ui_name': 'From_Min_FLOAT3'} */
                    {
                    CSycles.vectormaprangenode_set_from_min(this.Ptr, data);
                    }
                    break;
            case "from_max":
                    /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'from_max', 'ui_name': 'From_Max_FLOAT3'} */
                    {
                    CSycles.vectormaprangenode_set_from_max(this.Ptr, data);
                    }
                    break;
            case "to_min":
                    /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'to_min', 'ui_name': 'To_Min_FLOAT3'} */
                    {
                    CSycles.vectormaprangenode_set_to_min(this.Ptr, data);
                    }
                    break;
            case "to_max":
                    /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'to_max', 'ui_name': 'To_Max_FLOAT3'} */
                    {
                    CSycles.vectormaprangenode_set_to_max(this.Ptr, data);
                    }
                    break;
            case "vector":
                    /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.vectormaprangenode_set_vector(this.Ptr, data);
                    }
                    break;
            case "steps":
                    /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(4.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'steps', 'ui_name': 'Steps_FLOAT3'} */
                    {
                    CSycles.vectormaprangenode_set_steps(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMapRangeNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_clamp":
                    /* vectormaprangenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                    {
                    CSycles.vectormaprangenode_set_use_clamp(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMapRangeNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "range_type":
                    /* vectormaprangenode . {'datatype': 'ENUM', 'default_value': 'NODE_MAP_RANGE_LINEAR', 'default_value_type': 'NodeMapRangeType', 'is_input': True, 'member_name': 'range_type', 'ui_name': 'Type'} */
                    {
                    CSycles.vectormaprangenode_set_range_type(this.Ptr, (ccl.NodeMapRangeType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMapRangeNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "from_min":
                /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'from_min', 'ui_name': 'From_Min_FLOAT3'} */
                {
                    return CSycles.vectormaprangenode_get_from_min(this.Ptr);
                }
            case "from_max":
                /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'from_max', 'ui_name': 'From_Max_FLOAT3'} */
                {
                    return CSycles.vectormaprangenode_get_from_max(this.Ptr);
                }
            case "to_min":
                /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'to_min', 'ui_name': 'To_Min_FLOAT3'} */
                {
                    return CSycles.vectormaprangenode_get_to_min(this.Ptr);
                }
            case "to_max":
                /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'to_max', 'ui_name': 'To_Max_FLOAT3'} */
                {
                    return CSycles.vectormaprangenode_get_to_max(this.Ptr);
                }
            case "vector":
                /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.vectormaprangenode_get_vector(this.Ptr);
                }
            case "steps":
                /* vectormaprangenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(4.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'steps', 'ui_name': 'Steps_FLOAT3'} */
                {
                    return CSycles.vectormaprangenode_get_steps(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMapRangeNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_clamp":
                /* vectormaprangenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                {
                    return CSycles.vectormaprangenode_get_use_clamp(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMapRangeNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "range_type":
                /* vectormaprangenode . {'datatype': 'ENUM', 'default_value': 'NODE_MAP_RANGE_LINEAR', 'default_value_type': 'NodeMapRangeType', 'is_input': True, 'member_name': 'range_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.vectormaprangenode_get_range_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VectorMapRangeNode (getter)");
            }
        }

#endregion
    }

}