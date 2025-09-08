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

    public class NormalMapNodeInputs : Inputs
    {
        public EnumSocket Space { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket Strength { get; private set; }
        public StringSocket Attribute { get; private set; }

        public NormalMapNodeInputs(ShaderNode parentNode)
        {
            Space = new EnumSocket(parentNode, "Space", "space", true);
            AddSocket(Space);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Strength = new FloatSocket(parentNode, "Strength", "strength", true);
            AddSocket(Strength);
            Attribute = new StringSocket(parentNode, "Attribute", "attribute", true);
            AddSocket(Attribute);
        }
    }
    public class NormalMapNodeOutputs : Outputs
    {
        public NormalSocket Normal { get; private set; }

        public NormalMapNodeOutputs(ShaderNode parentNode)
        {
            Normal = new NormalSocket(parentNode, "Normal", "normal", false);
            AddSocket(Normal);
        }
    }

    [ShaderNode(name: "normal_map")]
    public class NormalMapNode : ShaderNode
    {
        public enum NormalMapNodeSpace : uint {
            Tangent = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_TANGENT,
            Object = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_OBJECT,
            World = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_WORLD,
            BlenderObject = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_BLENDER_OBJECT,
            BlenderWorld = ccl.NodeNormalMapSpace.NODE_NORMAL_MAP_BLENDER_WORLD,
        }
        public NormalMapNodeInputs ins => (NormalMapNodeInputs)inputs;
        public NormalMapNodeOutputs outs => (NormalMapNodeOutputs)outputs;
        public NormalMapNode(Shader shader) : this(shader, "a normal_map node") { }

        public NormalMapNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal NormalMapNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new NormalMapNodeInputs(this);
            outputs = new NormalMapNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.normalmapnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "strength":
                    /* normalmapnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                    {
                    CSycles.normalmapnode_set_strength(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalMapNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* normalmapnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.5f,0.5f,1.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.normalmapnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalMapNode (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "attribute":
                    /* normalmapnode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'attribute', 'ui_name': 'Attribute'} */
                    {
                    CSycles.normalmapnode_set_attribute(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalMapNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "space":
                    /* normalmapnode . {'datatype': 'ENUM', 'default_value': 'NODE_NORMAL_MAP_TANGENT', 'default_value_type': 'NodeNormalMapSpace', 'is_input': True, 'member_name': 'space', 'ui_name': 'Space'} */
                    {
                    CSycles.normalmapnode_set_space(this.Ptr, (ccl.NodeNormalMapSpace)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalMapNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "strength":
                /* normalmapnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                {
                    return CSycles.normalmapnode_get_strength(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalMapNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* normalmapnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.5f,0.5f,1.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.normalmapnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalMapNode (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "attribute":
                /* normalmapnode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'attribute', 'ui_name': 'Attribute'} */
                {
                    return CSycles.normalmapnode_get_attribute(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalMapNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "space":
                /* normalmapnode . {'datatype': 'ENUM', 'default_value': 'NODE_NORMAL_MAP_TANGENT', 'default_value_type': 'NodeNormalMapSpace', 'is_input': True, 'member_name': 'space', 'ui_name': 'Space'} */
                {
                    return (uint)CSycles.normalmapnode_get_space(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NormalMapNode (getter)");
            }
        }

#endregion
    }

}