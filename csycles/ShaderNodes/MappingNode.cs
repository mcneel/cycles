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

    public class MappingNodeInputs : Inputs
    {
        public PointSocket Scale { get; private set; }
        public PointSocket Location { get; private set; }
        public EnumSocket Type { get; private set; }
        public PointSocket Rotation { get; private set; }
        public PointSocket Vector { get; private set; }

        public MappingNodeInputs(ShaderNode parentNode)
        {
            Scale = new PointSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
            Location = new PointSocket(parentNode, "Location", "location", true);
            AddSocket(Location);
            Type = new EnumSocket(parentNode, "Type", "mapping_type", true);
            AddSocket(Type);
            Rotation = new PointSocket(parentNode, "Rotation", "rotation", true);
            AddSocket(Rotation);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
        }
    }
    public class MappingNodeOutputs : Outputs
    {
        public PointSocket Vector { get; private set; }

        public MappingNodeOutputs(ShaderNode parentNode)
        {
            Vector = new PointSocket(parentNode, "Vector", "vector", false);
            AddSocket(Vector);
        }
    }

    [ShaderNode(name: "mapping")]
    public class MappingNode : ShaderNode
    {
        public enum MappingNodeType : uint {
            Point = ccl.NodeMappingType.NODE_MAPPING_TYPE_POINT,
            Texture = ccl.NodeMappingType.NODE_MAPPING_TYPE_TEXTURE,
            Vector = ccl.NodeMappingType.NODE_MAPPING_TYPE_VECTOR,
            Normal = ccl.NodeMappingType.NODE_MAPPING_TYPE_NORMAL,
        }
        public MappingNodeInputs ins => (MappingNodeInputs)inputs;
        public MappingNodeOutputs outs => (MappingNodeOutputs)outputs;
        public MappingNode(Shader shader) : this(shader, "a mapping node") { }

        public MappingNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MappingNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MappingNodeInputs(this);
            outputs = new MappingNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.mappingnode_get_node_type();
        }
#region Setters

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "scale":
                    /* mappingnode . {'datatype': 'POINT', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.mappingnode_set_scale(this.Ptr, data);
                    }
                    break;
            case "location":
                    /* mappingnode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'location', 'ui_name': 'Location'} */
                    {
                    CSycles.mappingnode_set_location(this.Ptr, data);
                    }
                    break;
            case "rotation":
                    /* mappingnode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'rotation', 'ui_name': 'Rotation'} */
                    {
                    CSycles.mappingnode_set_rotation(this.Ptr, data);
                    }
                    break;
            case "vector":
                    /* mappingnode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.mappingnode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MappingNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "mapping_type":
                    /* mappingnode . {'datatype': 'ENUM', 'default_value': 'NODE_MAPPING_TYPE_POINT', 'default_value_type': 'NodeMappingType', 'is_input': True, 'member_name': 'mapping_type', 'ui_name': 'Type'} */
                    {
                    CSycles.mappingnode_set_mapping_type(this.Ptr, (ccl.NodeMappingType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MappingNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "scale":
                /* mappingnode . {'datatype': 'POINT', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.mappingnode_get_scale(this.Ptr);
                }
            case "location":
                /* mappingnode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'location', 'ui_name': 'Location'} */
                {
                    return CSycles.mappingnode_get_location(this.Ptr);
                }
            case "rotation":
                /* mappingnode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'rotation', 'ui_name': 'Rotation'} */
                {
                    return CSycles.mappingnode_get_rotation(this.Ptr);
                }
            case "vector":
                /* mappingnode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.mappingnode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MappingNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "mapping_type":
                /* mappingnode . {'datatype': 'ENUM', 'default_value': 'NODE_MAPPING_TYPE_POINT', 'default_value_type': 'NodeMappingType', 'is_input': True, 'member_name': 'mapping_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.mappingnode_get_mapping_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MappingNode (getter)");
            }
        }

#endregion
    }

}