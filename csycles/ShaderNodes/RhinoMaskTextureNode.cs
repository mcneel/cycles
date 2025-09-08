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

    public class RhinoMaskTextureNodeInputs : Inputs
    {
        public EnumSocket MaskType { get; private set; }
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoMaskTextureNodeInputs(ShaderNode parentNode)
        {
            MaskType = new EnumSocket(parentNode, "MaskType", "mask_type", true);
            AddSocket(MaskType);
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", true);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    public class RhinoMaskTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public VectorSocket Color { get; private set; }

        public RhinoMaskTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "out_alpha", false);
            AddSocket(Alpha);
            Color = new VectorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_mask_texture")]
    public class RhinoMaskTextureNode : ShaderNode
    {
        public enum RhinoMaskTextureNodeMaskType : uint {
            Luminance = ccl.RhinoProceduralMaskType.RHINO_MASK_LUMINANCE,
            Red = ccl.RhinoProceduralMaskType.RHINO_MASK_RED,
            Green = ccl.RhinoProceduralMaskType.RHINO_MASK_GREEN,
            Blue = ccl.RhinoProceduralMaskType.RHINO_MASK_BLUE,
            Alpha = ccl.RhinoProceduralMaskType.RHINO_MASK_ALPHA,
        }
        public RhinoMaskTextureNodeInputs ins => (RhinoMaskTextureNodeInputs)inputs;
        public RhinoMaskTextureNodeOutputs outs => (RhinoMaskTextureNodeOutputs)outputs;
        public RhinoMaskTextureNode(Shader shader) : this(shader, "a rhino_mask_texture node") { }

        public RhinoMaskTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoMaskTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoMaskTextureNodeInputs(this);
            outputs = new RhinoMaskTextureNodeOutputs(this);
        }
        public float3 Color {
            get { return CSycles.rhinomasktexturenode_get_color(Ptr); }
            set { CSycles.rhinomasktexturenode_set_color(Ptr, value); }
        }

        public RhinoProceduralMaskType MaskType {
            get { return CSycles.rhinomasktexturenode_get_mask_type(Ptr); }
            set { CSycles.rhinomasktexturenode_set_mask_type(Ptr, value); }
        }

        public float Alpha {
            get { return CSycles.rhinomasktexturenode_get_alpha(Ptr); }
            set { CSycles.rhinomasktexturenode_set_alpha(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinomasktexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "alpha":
                    /* rhinomasktexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha', 'ui_name': 'Alpha'} */
                    {
                    CSycles.rhinomasktexturenode_set_alpha(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoMaskTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* rhinomasktexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.rhinomasktexturenode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoMaskTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "mask_type":
                    /* rhinomasktexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_MASK_LUMINANCE', 'default_value_type': 'RhinoProceduralMaskType', 'is_input': True, 'member_name': 'mask_type', 'ui_name': 'MaskType'} */
                    {
                    CSycles.rhinomasktexturenode_set_mask_type(this.Ptr, (ccl.RhinoProceduralMaskType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoMaskTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "alpha":
                /* rhinomasktexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha', 'ui_name': 'Alpha'} */
                {
                    return CSycles.rhinomasktexturenode_get_alpha(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoMaskTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* rhinomasktexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.rhinomasktexturenode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoMaskTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "mask_type":
                /* rhinomasktexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_MASK_LUMINANCE', 'default_value_type': 'RhinoProceduralMaskType', 'is_input': True, 'member_name': 'mask_type', 'ui_name': 'MaskType'} */
                {
                    return (uint)CSycles.rhinomasktexturenode_get_mask_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoMaskTextureNode (getter)");
            }
        }

#endregion
    }

}