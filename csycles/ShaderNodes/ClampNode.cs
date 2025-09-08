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

    public class ClampNodeInputs : Inputs
    {
        public FloatSocket Value { get; private set; }
        public EnumSocket Type { get; private set; }
        public FloatSocket Max { get; private set; }
        public FloatSocket Min { get; private set; }

        public ClampNodeInputs(ShaderNode parentNode)
        {
            Value = new FloatSocket(parentNode, "Value", "value", true);
            AddSocket(Value);
            Type = new EnumSocket(parentNode, "Type", "clamp_type", true);
            AddSocket(Type);
            Max = new FloatSocket(parentNode, "Max", "max", true);
            AddSocket(Max);
            Min = new FloatSocket(parentNode, "Min", "min", true);
            AddSocket(Min);
        }
    }
    public class ClampNodeOutputs : Outputs
    {
        public FloatSocket Result { get; private set; }

        public ClampNodeOutputs(ShaderNode parentNode)
        {
            Result = new FloatSocket(parentNode, "Result", "result", false);
            AddSocket(Result);
        }
    }

    [ShaderNode(name: "clamp")]
    public class ClampNode : ShaderNode
    {
        public enum ClampNodeType : uint {
            Minmax = ccl.NodeClampType.NODE_CLAMP_MINMAX,
            Range = ccl.NodeClampType.NODE_CLAMP_RANGE,
        }
        public ClampNodeInputs ins => (ClampNodeInputs)inputs;
        public ClampNodeOutputs outs => (ClampNodeOutputs)outputs;
        public ClampNode(Shader shader) : this(shader, "a clamp node") { }

        public ClampNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal ClampNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new ClampNodeInputs(this);
            outputs = new ClampNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.clampnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "value":
                    /* clampnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                    {
                    CSycles.clampnode_set_value(this.Ptr, data);
                    }
                    break;
            case "max":
                    /* clampnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max', 'ui_name': 'Max'} */
                    {
                    CSycles.clampnode_set_max(this.Ptr, data);
                    }
                    break;
            case "min":
                    /* clampnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'min', 'ui_name': 'Min'} */
                    {
                    CSycles.clampnode_set_min(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ClampNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "clamp_type":
                    /* clampnode . {'datatype': 'ENUM', 'default_value': 'NODE_CLAMP_MINMAX', 'default_value_type': 'NodeClampType', 'is_input': True, 'member_name': 'clamp_type', 'ui_name': 'Type'} */
                    {
                    CSycles.clampnode_set_clamp_type(this.Ptr, (ccl.NodeClampType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ClampNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "value":
                /* clampnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'value', 'ui_name': 'Value'} */
                {
                    return CSycles.clampnode_get_value(this.Ptr);
                }
            case "max":
                /* clampnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max', 'ui_name': 'Max'} */
                {
                    return CSycles.clampnode_get_max(this.Ptr);
                }
            case "min":
                /* clampnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'min', 'ui_name': 'Min'} */
                {
                    return CSycles.clampnode_get_min(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ClampNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "clamp_type":
                /* clampnode . {'datatype': 'ENUM', 'default_value': 'NODE_CLAMP_MINMAX', 'default_value_type': 'NodeClampType', 'is_input': True, 'member_name': 'clamp_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.clampnode_get_clamp_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ClampNode (getter)");
            }
        }

#endregion
    }

}