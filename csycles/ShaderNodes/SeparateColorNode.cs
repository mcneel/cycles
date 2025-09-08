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

    public class SeparateColorNodeInputs : Inputs
    {
        public ColorSocket Color { get; private set; }
        public EnumSocket Type { get; private set; }

        public SeparateColorNodeInputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Type = new EnumSocket(parentNode, "Type", "color_type", true);
            AddSocket(Type);
        }
    }
    public class SeparateColorNodeOutputs : Outputs
    {
        public FloatSocket Blue { get; private set; }
        public FloatSocket Green { get; private set; }
        public FloatSocket Red { get; private set; }

        public SeparateColorNodeOutputs(ShaderNode parentNode)
        {
            Blue = new FloatSocket(parentNode, "Blue", "b", false);
            AddSocket(Blue);
            Green = new FloatSocket(parentNode, "Green", "g", false);
            AddSocket(Green);
            Red = new FloatSocket(parentNode, "Red", "r", false);
            AddSocket(Red);
        }
    }

    [ShaderNode(name: "separate_color")]
    public class SeparateColorNode : ShaderNode
    {
        public enum SeparateColorNodeType : uint {
            Rgb = ccl.NodeCombSepColorType.NODE_COMBSEP_COLOR_RGB,
            Hsv = ccl.NodeCombSepColorType.NODE_COMBSEP_COLOR_HSV,
            Hsl = ccl.NodeCombSepColorType.NODE_COMBSEP_COLOR_HSL,
        }
        public SeparateColorNodeInputs ins => (SeparateColorNodeInputs)inputs;
        public SeparateColorNodeOutputs outs => (SeparateColorNodeOutputs)outputs;
        public SeparateColorNode(Shader shader) : this(shader, "a separate_color node") { }

        public SeparateColorNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal SeparateColorNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new SeparateColorNodeInputs(this);
            outputs = new SeparateColorNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.separatecolornode_get_node_type();
        }
#region Setters

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* separatecolornode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.separatecolornode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateColorNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "color_type":
                    /* separatecolornode . {'datatype': 'ENUM', 'default_value': 'NODE_COMBSEP_COLOR_RGB', 'default_value_type': 'NodeCombSepColorType', 'is_input': True, 'member_name': 'color_type', 'ui_name': 'Type'} */
                    {
                    CSycles.separatecolornode_set_color_type(this.Ptr, (ccl.NodeCombSepColorType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateColorNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* separatecolornode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.separatecolornode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateColorNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "color_type":
                /* separatecolornode . {'datatype': 'ENUM', 'default_value': 'NODE_COMBSEP_COLOR_RGB', 'default_value_type': 'NodeCombSepColorType', 'is_input': True, 'member_name': 'color_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.separatecolornode_get_color_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SeparateColorNode (getter)");
            }
        }

#endregion
    }

}