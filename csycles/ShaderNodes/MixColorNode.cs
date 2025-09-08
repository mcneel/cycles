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

    public class MixColorNodeInputs : Inputs
    {
        public BoolSocket UseClamp { get; private set; }
        public ColorSocket B { get; private set; }
        public FloatSocket Factor { get; private set; }
        public BoolSocket UseClampResult { get; private set; }
        public ColorSocket A { get; private set; }
        public EnumSocket Type { get; private set; }

        public MixColorNodeInputs(ShaderNode parentNode)
        {
            UseClamp = new BoolSocket(parentNode, "Use Clamp", "use_clamp", true);
            AddSocket(UseClamp);
            B = new ColorSocket(parentNode, "B", "b", true);
            AddSocket(B);
            Factor = new FloatSocket(parentNode, "Factor", "fac", true);
            AddSocket(Factor);
            UseClampResult = new BoolSocket(parentNode, "Use Clamp Result", "use_clamp_result", true);
            AddSocket(UseClampResult);
            A = new ColorSocket(parentNode, "A", "a", true);
            AddSocket(A);
            Type = new EnumSocket(parentNode, "Type", "blend_type", true);
            AddSocket(Type);
        }
    }
    public class MixColorNodeOutputs : Outputs
    {
        public ColorSocket Result { get; private set; }

        public MixColorNodeOutputs(ShaderNode parentNode)
        {
            Result = new ColorSocket(parentNode, "Result", "result", false);
            AddSocket(Result);
        }
    }

    [ShaderNode(name: "mix_color")]
    public class MixColorNode : ShaderNode
    {
        public enum MixColorNodeType : uint {
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
        public MixColorNodeInputs ins => (MixColorNodeInputs)inputs;
        public MixColorNodeOutputs outs => (MixColorNodeOutputs)outputs;
        public MixColorNode(Shader shader) : this(shader, "a mix_color node") { }

        public MixColorNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MixColorNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MixColorNodeInputs(this);
            outputs = new MixColorNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.mixcolornode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "fac":
                    /* mixcolornode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                    {
                    CSycles.mixcolornode_set_fac(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixColorNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "b":
                    /* mixcolornode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                    {
                    CSycles.mixcolornode_set_b(this.Ptr, data);
                    }
                    break;
            case "a":
                    /* mixcolornode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'a', 'ui_name': 'A'} */
                    {
                    CSycles.mixcolornode_set_a(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixColorNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_clamp":
                    /* mixcolornode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                    {
                    CSycles.mixcolornode_set_use_clamp(this.Ptr, data);
                    }
                    break;
            case "use_clamp_result":
                    /* mixcolornode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp_result', 'ui_name': 'Use Clamp Result'} */
                    {
                    CSycles.mixcolornode_set_use_clamp_result(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixColorNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "blend_type":
                    /* mixcolornode . {'datatype': 'ENUM', 'default_value': 'NODE_MIX_BLEND', 'default_value_type': 'NodeMix', 'is_input': True, 'member_name': 'blend_type', 'ui_name': 'Type'} */
                    {
                    CSycles.mixcolornode_set_blend_type(this.Ptr, (ccl.NodeMix)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixColorNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "fac":
                /* mixcolornode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Factor'} */
                {
                    return CSycles.mixcolornode_get_fac(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixColorNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "b":
                /* mixcolornode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'b', 'ui_name': 'B'} */
                {
                    return CSycles.mixcolornode_get_b(this.Ptr);
                }
            case "a":
                /* mixcolornode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'a', 'ui_name': 'A'} */
                {
                    return CSycles.mixcolornode_get_a(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixColorNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_clamp":
                /* mixcolornode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp', 'ui_name': 'Use Clamp'} */
                {
                    return CSycles.mixcolornode_get_use_clamp(this.Ptr);
                }
            case "use_clamp_result":
                /* mixcolornode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_clamp_result', 'ui_name': 'Use Clamp Result'} */
                {
                    return CSycles.mixcolornode_get_use_clamp_result(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixColorNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "blend_type":
                /* mixcolornode . {'datatype': 'ENUM', 'default_value': 'NODE_MIX_BLEND', 'default_value_type': 'NodeMix', 'is_input': True, 'member_name': 'blend_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.mixcolornode_get_blend_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixColorNode (getter)");
            }
        }

#endregion
    }

}