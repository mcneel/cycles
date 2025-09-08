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

    public class MixNodeInputs : Inputs
    {
        public ColorSocket Color2 { get; private set; }
        public FloatSocket Fac { get; private set; }
        public EnumSocket Type { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public BoolSocket UseClamp { get; private set; }

        public MixNodeInputs(ShaderNode parentNode)
        {
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            Fac = new FloatSocket(parentNode, "Fac", "fac", true);
            AddSocket(Fac);
            Type = new EnumSocket(parentNode, "Type", "mix_type", true);
            AddSocket(Type);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            UseClamp = new BoolSocket(parentNode, "Use Clamp", "use_clamp", true);
            AddSocket(UseClamp);
        }
    }
    public class MixNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public MixNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "mix")]
    public class MixNode : ShaderNode
    {
        public enum MixNodeType : uint {
            Mix = ccl.NodeMix.NODE_MIX_BLEND,
            Add = ccl.NodeMix.NODE_MIX_ADD,
            Multiply = ccl.NodeMix.NODE_MIX_MUL,
            Subtract = ccl.NodeMix.NODE_MIX_SUB,
            Screen = ccl.NodeMix.NODE_MIX_SCREEN,
            Divide = ccl.NodeMix.NODE_MIX_DIV,
            Difference = ccl.NodeMix.NODE_MIX_DIFF,
            Darken = ccl.NodeMix.NODE_MIX_DARK,
            Lighten = ccl.NodeMix.NODE_MIX_LIGHT,
            Overlay = ccl.NodeMix.NODE_MIX_OVERLAY,
            Dodge = ccl.NodeMix.NODE_MIX_DODGE,
            Burn = ccl.NodeMix.NODE_MIX_BURN,
            Hue = ccl.NodeMix.NODE_MIX_HUE,
            Saturation = ccl.NodeMix.NODE_MIX_SAT,
            Value = ccl.NodeMix.NODE_MIX_VAL,
            Color = ccl.NodeMix.NODE_MIX_COL,
            SoftLight = ccl.NodeMix.NODE_MIX_SOFT,
            LinearLight = ccl.NodeMix.NODE_MIX_LINEAR,
            Exclusion = ccl.NodeMix.NODE_MIX_EXCLUSION,
        }
        public MixNodeInputs ins => (MixNodeInputs)inputs;
        public MixNodeOutputs outs => (MixNodeOutputs)outputs;
        public MixNode(Shader shader) : this(shader, "a mix node") { }

        public MixNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MixNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MixNodeInputs(this);
            outputs = new MixNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.mixnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "fac":
                    /* mixnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                    {
                    CSycles.mixnode_set_fac(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color2":
                    /* mixnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.mixnode_set_color2(this.Ptr, data);
                    }
                    break;
            case "color1":
                    /* mixnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.mixnode_set_color1(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_clamp":
                    /* mixnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                    {
                    CSycles.mixnode_set_use_clamp(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "mix_type":
                    /* mixnode . {'datatype': 'ENUM', 'default_value': 'NODE_MIX_BLEND', 'default_value_type': 'NodeMix', 'is_input': True, 'member_name': 'mix_type', 'ui_name': 'Type'} */
                    {
                    CSycles.mixnode_set_mix_type(this.Ptr, (ccl.NodeMix)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "fac":
                /* mixnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                {
                    return CSycles.mixnode_get_fac(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color2":
                /* mixnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.mixnode_get_color2(this.Ptr);
                }
            case "color1":
                /* mixnode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.mixnode_get_color1(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_clamp":
                /* mixnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                {
                    return CSycles.mixnode_get_use_clamp(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "mix_type":
                /* mixnode . {'datatype': 'ENUM', 'default_value': 'NODE_MIX_BLEND', 'default_value_type': 'NodeMix', 'is_input': True, 'member_name': 'mix_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.mixnode_get_mix_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixNode (getter)");
            }
        }

#endregion
    }

}