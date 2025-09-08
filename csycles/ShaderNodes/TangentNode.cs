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

    public class TangentNodeInputs : Inputs
    {
        public EnumSocket Axis { get; private set; }
        public EnumSocket DirectionType { get; private set; }

        public TangentNodeInputs(ShaderNode parentNode)
        {
            Axis = new EnumSocket(parentNode, "Axis", "axis", true);
            AddSocket(Axis);
            DirectionType = new EnumSocket(parentNode, "Direction Type", "direction_type", true);
            AddSocket(DirectionType);
        }
    }
    public class TangentNodeOutputs : Outputs
    {
        public NormalSocket Tangent { get; private set; }

        public TangentNodeOutputs(ShaderNode parentNode)
        {
            Tangent = new NormalSocket(parentNode, "Tangent", "tangent", false);
            AddSocket(Tangent);
        }
    }

    [ShaderNode(name: "tangent")]
    public class TangentNode : ShaderNode
    {
        public enum TangentNodeAxis : uint {
            X = ccl.NodeTangentAxis.NODE_TANGENT_AXIS_X,
            Y = ccl.NodeTangentAxis.NODE_TANGENT_AXIS_Y,
            Z = ccl.NodeTangentAxis.NODE_TANGENT_AXIS_Z,
        }
        public enum TangentNodeDirectionType : uint {
            Radial = ccl.NodeTangentDirectionType.NODE_TANGENT_RADIAL,
            UvMap = ccl.NodeTangentDirectionType.NODE_TANGENT_UVMAP,
        }
        public TangentNodeInputs ins => (TangentNodeInputs)inputs;
        public TangentNodeOutputs outs => (TangentNodeOutputs)outputs;
        public TangentNode(Shader shader) : this(shader, "a tangent node") { }

        public TangentNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal TangentNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new TangentNodeInputs(this);
            outputs = new TangentNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.tangentnode_get_node_type();
        }
#region Setters

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "axis":
                    /* tangentnode . {'datatype': 'ENUM', 'default_value': 'NODE_TANGENT_AXIS_X', 'default_value_type': 'NodeTangentAxis', 'is_input': True, 'member_name': 'axis', 'ui_name': 'Axis'} */
                    {
                    CSycles.tangentnode_set_axis(this.Ptr, (ccl.NodeTangentAxis)data);
                    }
                    break;
            case "direction_type":
                    /* tangentnode . {'datatype': 'ENUM', 'default_value': 'NODE_TANGENT_RADIAL', 'default_value_type': 'NodeTangentDirectionType', 'is_input': True, 'member_name': 'direction_type', 'ui_name': 'Direction Type'} */
                    {
                    CSycles.tangentnode_set_direction_type(this.Ptr, (ccl.NodeTangentDirectionType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type TangentNode (setter)");
            }
        }

#endregion
#region Getters

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "axis":
                /* tangentnode . {'datatype': 'ENUM', 'default_value': 'NODE_TANGENT_AXIS_X', 'default_value_type': 'NodeTangentAxis', 'is_input': True, 'member_name': 'axis', 'ui_name': 'Axis'} */
                {
                    return (uint)CSycles.tangentnode_get_axis(this.Ptr);
                }
            case "direction_type":
                /* tangentnode . {'datatype': 'ENUM', 'default_value': 'NODE_TANGENT_RADIAL', 'default_value_type': 'NodeTangentDirectionType', 'is_input': True, 'member_name': 'direction_type', 'ui_name': 'Direction Type'} */
                {
                    return (uint)CSycles.tangentnode_get_direction_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type TangentNode (getter)");
            }
        }

#endregion
    }

}