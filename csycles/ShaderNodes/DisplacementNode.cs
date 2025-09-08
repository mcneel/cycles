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

    public class DisplacementNodeInputs : Inputs
    {
        public FloatSocket Scale { get; private set; }
        public FloatSocket Height { get; private set; }
        public NormalSocket Normal { get; private set; }
        public FloatSocket Midlevel { get; private set; }
        public EnumSocket Space { get; private set; }

        public DisplacementNodeInputs(ShaderNode parentNode)
        {
            Scale = new FloatSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
            Height = new FloatSocket(parentNode, "Height", "height", true);
            AddSocket(Height);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            Midlevel = new FloatSocket(parentNode, "Midlevel", "midlevel", true);
            AddSocket(Midlevel);
            Space = new EnumSocket(parentNode, "Space", "space", true);
            AddSocket(Space);
        }
    }
    public class DisplacementNodeOutputs : Outputs
    {
        public VectorSocket Displacement { get; private set; }

        public DisplacementNodeOutputs(ShaderNode parentNode)
        {
            Displacement = new VectorSocket(parentNode, "Displacement", "displacement", false);
            AddSocket(Displacement);
        }
    }

    [ShaderNode(name: "displacement")]
    public class DisplacementNode : ShaderNode
    {
        public enum DisplacementNodeSpace : uint {
            Object = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_OBJECT,
            World = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_WORLD,
        }
        public DisplacementNodeInputs ins => (DisplacementNodeInputs)inputs;
        public DisplacementNodeOutputs outs => (DisplacementNodeOutputs)outputs;
        public DisplacementNode(Shader shader) : this(shader, "a displacement node") { }

        public DisplacementNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal DisplacementNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new DisplacementNodeInputs(this);
            outputs = new DisplacementNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.displacementnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "scale":
                    /* displacementnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.displacementnode_set_scale(this.Ptr, data);
                    }
                    break;
            case "height":
                    /* displacementnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'height', 'ui_name': 'Height'} */
                    {
                    CSycles.displacementnode_set_height(this.Ptr, data);
                    }
                    break;
            case "midlevel":
                    /* displacementnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'midlevel', 'ui_name': 'Midlevel'} */
                    {
                    CSycles.displacementnode_set_midlevel(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type DisplacementNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* displacementnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.displacementnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type DisplacementNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "space":
                    /* displacementnode . {'datatype': 'ENUM', 'default_value': 'NODE_NORMAL_MAP_OBJECT', 'default_value_type': 'NodeNormalMapSpace', 'is_input': True, 'member_name': 'space', 'ui_name': 'Space'} */
                    {
                    CSycles.displacementnode_set_space(this.Ptr, (ccl.NodeNormalMapSpace)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type DisplacementNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "scale":
                /* displacementnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.displacementnode_get_scale(this.Ptr);
                }
            case "height":
                /* displacementnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'height', 'ui_name': 'Height'} */
                {
                    return CSycles.displacementnode_get_height(this.Ptr);
                }
            case "midlevel":
                /* displacementnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'midlevel', 'ui_name': 'Midlevel'} */
                {
                    return CSycles.displacementnode_get_midlevel(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type DisplacementNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* displacementnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.displacementnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type DisplacementNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "space":
                /* displacementnode . {'datatype': 'ENUM', 'default_value': 'NODE_NORMAL_MAP_OBJECT', 'default_value_type': 'NodeNormalMapSpace', 'is_input': True, 'member_name': 'space', 'ui_name': 'Space'} */
                {
                    return (uint)CSycles.displacementnode_get_space(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type DisplacementNode (getter)");
            }
        }

#endregion
    }

}