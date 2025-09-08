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

    public class CombineColorNodeInputs : Inputs
    {
        public FloatSocket Red { get; private set; }
        public EnumSocket Type { get; private set; }
        public FloatSocket Blue { get; private set; }
        public FloatSocket Green { get; private set; }

        public CombineColorNodeInputs(ShaderNode parentNode)
        {
            Red = new FloatSocket(parentNode, "Red", "r", true);
            AddSocket(Red);
            Type = new EnumSocket(parentNode, "Type", "color_type", true);
            AddSocket(Type);
            Blue = new FloatSocket(parentNode, "Blue", "b", true);
            AddSocket(Blue);
            Green = new FloatSocket(parentNode, "Green", "g", true);
            AddSocket(Green);
        }
    }
    public class CombineColorNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public CombineColorNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "combine_color")]
    public class CombineColorNode : ShaderNode
    {
        public enum CombineColorNodeType : uint {
            Rgb = ccl.NodeCombSepColorType.NODE_COMBSEP_COLOR_RGB,
            Hsv = ccl.NodeCombSepColorType.NODE_COMBSEP_COLOR_HSV,
            Hsl = ccl.NodeCombSepColorType.NODE_COMBSEP_COLOR_HSL,
        }
        public CombineColorNodeInputs ins => (CombineColorNodeInputs)inputs;
        public CombineColorNodeOutputs outs => (CombineColorNodeOutputs)outputs;
        public CombineColorNode(Shader shader) : this(shader, "a combine_color node") { }

        public CombineColorNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal CombineColorNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new CombineColorNodeInputs(this);
            outputs = new CombineColorNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.combinecolornode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "r":
                    /* combinecolornode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'r', 'ui_name': 'Red'} */
                    {
                    CSycles.combinecolornode_set_r(this.Ptr, data);
                    }
                    break;
            case "b":
                    /* combinecolornode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'b', 'ui_name': 'Blue'} */
                    {
                    CSycles.combinecolornode_set_b(this.Ptr, data);
                    }
                    break;
            case "g":
                    /* combinecolornode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'g', 'ui_name': 'Green'} */
                    {
                    CSycles.combinecolornode_set_g(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineColorNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "color_type":
                    /* combinecolornode . {'datatype': 'ENUM', 'default_value': 'NODE_COMBSEP_COLOR_RGB', 'default_value_type': 'NodeCombSepColorType', 'is_input': True, 'member_name': 'color_type', 'ui_name': 'Type'} */
                    {
                    CSycles.combinecolornode_set_color_type(this.Ptr, (ccl.NodeCombSepColorType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineColorNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "r":
                /* combinecolornode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'r', 'ui_name': 'Red'} */
                {
                    return CSycles.combinecolornode_get_r(this.Ptr);
                }
            case "b":
                /* combinecolornode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'b', 'ui_name': 'Blue'} */
                {
                    return CSycles.combinecolornode_get_b(this.Ptr);
                }
            case "g":
                /* combinecolornode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'g', 'ui_name': 'Green'} */
                {
                    return CSycles.combinecolornode_get_g(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineColorNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "color_type":
                /* combinecolornode . {'datatype': 'ENUM', 'default_value': 'NODE_COMBSEP_COLOR_RGB', 'default_value_type': 'NodeCombSepColorType', 'is_input': True, 'member_name': 'color_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.combinecolornode_get_color_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type CombineColorNode (getter)");
            }
        }

#endregion
    }

}