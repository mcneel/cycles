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

    public class MapRangeNodeInputs : Inputs
    {
        public FloatSocket FromMin { get; private set; }
        public BoolSocket Clamp { get; private set; }
        public FloatSocket FromMax { get; private set; }
        public FloatSocket ToMin { get; private set; }
        public EnumSocket Type { get; private set; }
        public FloatSocket ToMax { get; private set; }
        public FloatSocket Value { get; private set; }
        public FloatSocket Steps { get; private set; }

        public MapRangeNodeInputs(ShaderNode parentNode)
        {
            FromMin = new FloatSocket(parentNode, "From Min", "from_min", true);
            AddSocket(FromMin);
            Clamp = new BoolSocket(parentNode, "Clamp", "clamp", true);
            AddSocket(Clamp);
            FromMax = new FloatSocket(parentNode, "From Max", "from_max", true);
            AddSocket(FromMax);
            ToMin = new FloatSocket(parentNode, "To Min", "to_min", true);
            AddSocket(ToMin);
            Type = new EnumSocket(parentNode, "Type", "range_type", true);
            AddSocket(Type);
            ToMax = new FloatSocket(parentNode, "To Max", "to_max", true);
            AddSocket(ToMax);
            Value = new FloatSocket(parentNode, "Value", "value", true);
            AddSocket(Value);
            Steps = new FloatSocket(parentNode, "Steps", "steps", true);
            AddSocket(Steps);
        }
    }
    public class MapRangeNodeOutputs : Outputs
    {
        public FloatSocket Result { get; private set; }

        public MapRangeNodeOutputs(ShaderNode parentNode)
        {
            Result = new FloatSocket(parentNode, "Result", "result", false);
            AddSocket(Result);
        }
    }

    [ShaderNode(name: "map_range")]
    public class MapRangeNode : ShaderNode
    {
        public enum MapRangeNodeType : uint {
            Linear = ccl.NodeMapRangeType.NODE_MAP_RANGE_LINEAR,
            Stepped = ccl.NodeMapRangeType.NODE_MAP_RANGE_STEPPED,
            Smoothstep = ccl.NodeMapRangeType.NODE_MAP_RANGE_SMOOTHSTEP,
            Smootherstep = ccl.NodeMapRangeType.NODE_MAP_RANGE_SMOOTHERSTEP,
        }
        public MapRangeNodeInputs ins => (MapRangeNodeInputs)inputs;
        public MapRangeNodeOutputs outs => (MapRangeNodeOutputs)outputs;
        public MapRangeNode(Shader shader) : this(shader, "a map_range node") { }

        public MapRangeNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MapRangeNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MapRangeNodeInputs(this);
            outputs = new MapRangeNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.maprangenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "from_min":
                    /* maprangenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'from_min', 'ui_name': 'From Min'} */
                    {
                    CSycles.maprangenode_set_from_min(this.Ptr, data);
                    }
                    break;
            case "from_max":
                    /* maprangenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'from_max', 'ui_name': 'From Max'} */
                    {
                    CSycles.maprangenode_set_from_max(this.Ptr, data);
                    }
                    break;
            case "to_min":
                    /* maprangenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'to_min', 'ui_name': 'To Min'} */
                    {
                    CSycles.maprangenode_set_to_min(this.Ptr, data);
                    }
                    break;
            case "to_max":
                    /* maprangenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'to_max', 'ui_name': 'To Max'} */
                    {
                    CSycles.maprangenode_set_to_max(this.Ptr, data);
                    }
                    break;
            case "value":
                    /* maprangenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                    {
                    CSycles.maprangenode_set_value(this.Ptr, data);
                    }
                    break;
            case "steps":
                    /* maprangenode . {'datatype': 'FLOAT', 'default_value': '4.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'steps', 'ui_name': 'Steps'} */
                    {
                    CSycles.maprangenode_set_steps(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MapRangeNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "clamp":
                    /* maprangenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'clamp', 'ui_name': 'Clamp'} */
                    {
                    CSycles.maprangenode_set_clamp(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MapRangeNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "range_type":
                    /* maprangenode . {'datatype': 'ENUM', 'default_value': 'NODE_MAP_RANGE_LINEAR', 'default_value_type': 'NodeMapRangeType', 'is_input': True, 'member_name': 'range_type', 'ui_name': 'Type'} */
                    {
                    CSycles.maprangenode_set_range_type(this.Ptr, (ccl.NodeMapRangeType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MapRangeNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "from_min":
                /* maprangenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'from_min', 'ui_name': 'From Min'} */
                {
                    return CSycles.maprangenode_get_from_min(this.Ptr);
                }
            case "from_max":
                /* maprangenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'from_max', 'ui_name': 'From Max'} */
                {
                    return CSycles.maprangenode_get_from_max(this.Ptr);
                }
            case "to_min":
                /* maprangenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'to_min', 'ui_name': 'To Min'} */
                {
                    return CSycles.maprangenode_get_to_min(this.Ptr);
                }
            case "to_max":
                /* maprangenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'to_max', 'ui_name': 'To Max'} */
                {
                    return CSycles.maprangenode_get_to_max(this.Ptr);
                }
            case "value":
                /* maprangenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                {
                    return CSycles.maprangenode_get_value(this.Ptr);
                }
            case "steps":
                /* maprangenode . {'datatype': 'FLOAT', 'default_value': '4.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'steps', 'ui_name': 'Steps'} */
                {
                    return CSycles.maprangenode_get_steps(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MapRangeNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "clamp":
                /* maprangenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'clamp', 'ui_name': 'Clamp'} */
                {
                    return CSycles.maprangenode_get_clamp(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MapRangeNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "range_type":
                /* maprangenode . {'datatype': 'ENUM', 'default_value': 'NODE_MAP_RANGE_LINEAR', 'default_value_type': 'NodeMapRangeType', 'is_input': True, 'member_name': 'range_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.maprangenode_get_range_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MapRangeNode (getter)");
            }
        }

#endregion
    }

}