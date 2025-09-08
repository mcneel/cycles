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

    public class ToonBsdfNodeInputs : Inputs
    {
        public ColorSocket Color { get; private set; }
        public FloatSocket Smooth { get; private set; }
        public EnumSocket Component { get; private set; }
        public NormalSocket Normal { get; private set; }
        public FloatSocket Size { get; private set; }

        public ToonBsdfNodeInputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Smooth = new FloatSocket(parentNode, "Smooth", "smooth", true);
            AddSocket(Smooth);
            Component = new EnumSocket(parentNode, "Component", "component", true);
            AddSocket(Component);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            Size = new FloatSocket(parentNode, "Size", "size", true);
            AddSocket(Size);
        }
    }
    public class ToonBsdfNodeOutputs : Outputs
    {
        public ClosureSocket BSDF { get; private set; }

        public ToonBsdfNodeOutputs(ShaderNode parentNode)
        {
            BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF", false);
            AddSocket(BSDF);
        }
    }

    [ShaderNode(name: "toon_bsdf")]
    public class ToonBsdfNode : BsdfNode
    {
        public enum ToonBsdfNodeComponent : uint {
            Diffuse = ccl.ClosureType.CLOSURE_BSDF_DIFFUSE_TOON_ID,
            Glossy = ccl.ClosureType.CLOSURE_BSDF_GLOSSY_TOON_ID,
        }
        public ToonBsdfNodeInputs ins => (ToonBsdfNodeInputs)inputs;
        public ToonBsdfNodeOutputs outs => (ToonBsdfNodeOutputs)outputs;
        public ToonBsdfNode(Shader shader) : this(shader, "a toon_bsdf node") { }

        public ToonBsdfNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal ToonBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new ToonBsdfNodeInputs(this);
            outputs = new ToonBsdfNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.BSDF;
        }
        public static IntPtr GetNodeType() {
            return CSycles.toonbsdfnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "smooth":
                    /* toonbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'smooth', 'ui_name': 'Smooth'} */
                    {
                    CSycles.toonbsdfnode_set_smooth(this.Ptr, data);
                    }
                    break;
            case "size":
                    /* toonbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'size', 'ui_name': 'Size'} */
                    {
                    CSycles.toonbsdfnode_set_size(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ToonBsdfNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* toonbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.bsdfnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ToonBsdfNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* toonbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.bsdfnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ToonBsdfNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "component":
                    /* toonbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_DIFFUSE_TOON_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'component', 'ui_name': 'Component'} */
                    {
                    CSycles.toonbsdfnode_set_component(this.Ptr, (ccl.ClosureType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ToonBsdfNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "smooth":
                /* toonbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'smooth', 'ui_name': 'Smooth'} */
                {
                    return CSycles.toonbsdfnode_get_smooth(this.Ptr);
                }
            case "size":
                /* toonbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'size', 'ui_name': 'Size'} */
                {
                    return CSycles.toonbsdfnode_get_size(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ToonBsdfNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* toonbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.bsdfnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ToonBsdfNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* toonbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.bsdfnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ToonBsdfNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "component":
                /* toonbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_DIFFUSE_TOON_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'component', 'ui_name': 'Component'} */
                {
                    return (uint)CSycles.toonbsdfnode_get_component(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ToonBsdfNode (getter)");
            }
        }

#endregion
    }

}