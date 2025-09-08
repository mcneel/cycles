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

    public class RhinoGradientTextureNodeInputs : Inputs
    {
        public FloatSocket Alpha1 { get; private set; }
        public BoolSocket UseCustomCurve { get; private set; }
        public ColorSocket Color2 { get; private set; }
        public FloatSocket PointWidth { get; private set; }
        public FloatSocket Alpha2 { get; private set; }
        public FloatSocket PointHeight { get; private set; }
        public PointSocket UVW { get; private set; }
        public EnumSocket GradientType { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public BoolSocket FlipAlternate { get; private set; }

        public RhinoGradientTextureNodeInputs(ShaderNode parentNode)
        {
            Alpha1 = new FloatSocket(parentNode, "Alpha1", "alpha1", true);
            AddSocket(Alpha1);
            UseCustomCurve = new BoolSocket(parentNode, "UseCustomCurve", "use_custom_curve", true);
            AddSocket(UseCustomCurve);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            PointWidth = new FloatSocket(parentNode, "PointWidth", "point_width", true);
            AddSocket(PointWidth);
            Alpha2 = new FloatSocket(parentNode, "Alpha2", "alpha2", true);
            AddSocket(Alpha2);
            PointHeight = new FloatSocket(parentNode, "PointHeight", "point_height", true);
            AddSocket(PointHeight);
            UVW = new PointSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            GradientType = new EnumSocket(parentNode, "GradientType", "gradient_type", true);
            AddSocket(GradientType);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            FlipAlternate = new BoolSocket(parentNode, "FlipAlternate", "flip_alternate", true);
            AddSocket(FlipAlternate);
        }
    }
    public class RhinoGradientTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoGradientTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "out_alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_gradient_texture")]
    public class RhinoGradientTextureNode : ShaderNode
    {
        public enum RhinoGradientTextureNodeGradientType : uint {
            Linear = ccl.RhinoProceduralGradientType.RHINO_GRADIENT_LINEAR,
            Box = ccl.RhinoProceduralGradientType.RHINO_GRADIENT_BOX,
            Radial = ccl.RhinoProceduralGradientType.RHINO_GRADIENT_RADIAL,
            Tartan = ccl.RhinoProceduralGradientType.RHINO_GRADIENT_TARTAN,
            Sweep = ccl.RhinoProceduralGradientType.RHINO_GRADIENT_SWEEP,
            Pong = ccl.RhinoProceduralGradientType.RHINO_GRADIENT_PONG,
            Spiral = ccl.RhinoProceduralGradientType.RHINO_GRADIENT_SPIRAL,
        }
        public RhinoGradientTextureNodeInputs ins => (RhinoGradientTextureNodeInputs)inputs;
        public RhinoGradientTextureNodeOutputs outs => (RhinoGradientTextureNodeOutputs)outputs;
        public RhinoGradientTextureNode(Shader shader) : this(shader, "a rhino_gradient_texture node") { }

        public RhinoGradientTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoGradientTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoGradientTextureNodeInputs(this);
            outputs = new RhinoGradientTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinogradienttexturenode_get_uvw(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_uvw(Ptr, value); }
        }

        public bool FlipAlternate {
            get { return CSycles.rhinogradienttexturenode_get_flip_alternate(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_flip_alternate(Ptr, value); }
        }

        public float PointWidth {
            get { return CSycles.rhinogradienttexturenode_get_point_width(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_point_width(Ptr, value); }
        }

        public float Alpha1 {
            get { return CSycles.rhinogradienttexturenode_get_alpha1(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_alpha1(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinogradienttexturenode_get_color2(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinogradienttexturenode_get_node_type();
        }

        public float3 Color1 {
            get { return CSycles.rhinogradienttexturenode_get_color1(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_color1(Ptr, value); }
        }

        public RhinoProceduralGradientType GradientType {
            get { return CSycles.rhinogradienttexturenode_get_gradient_type(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_gradient_type(Ptr, value); }
        }

        public float Alpha2 {
            get { return CSycles.rhinogradienttexturenode_get_alpha2(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_alpha2(Ptr, value); }
        }

        public float PointHeight {
            get { return CSycles.rhinogradienttexturenode_get_point_height(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_point_height(Ptr, value); }
        }

        public bool UseCustomCurve {
            get { return CSycles.rhinogradienttexturenode_get_use_custom_curve(Ptr); }
            set { CSycles.rhinogradienttexturenode_set_use_custom_curve(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "alpha1":
                    /* rhinogradienttexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                    {
                    CSycles.rhinogradienttexturenode_set_alpha1(this.Ptr, data);
                    }
                    break;
            case "point_width":
                    /* rhinogradienttexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'point_width', 'ui_name': 'PointWidth'} */
                    {
                    CSycles.rhinogradienttexturenode_set_point_width(this.Ptr, data);
                    }
                    break;
            case "alpha2":
                    /* rhinogradienttexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                    {
                    CSycles.rhinogradienttexturenode_set_alpha2(this.Ptr, data);
                    }
                    break;
            case "point_height":
                    /* rhinogradienttexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'point_height', 'ui_name': 'PointHeight'} */
                    {
                    CSycles.rhinogradienttexturenode_set_point_height(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinogradienttexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinogradienttexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color2":
                    /* rhinogradienttexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinogradienttexturenode_set_color2(this.Ptr, data);
                    }
                    break;
            case "color1":
                    /* rhinogradienttexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinogradienttexturenode_set_color1(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_custom_curve":
                    /* rhinogradienttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_custom_curve', 'ui_name': 'UseCustomCurve'} */
                    {
                    CSycles.rhinogradienttexturenode_set_use_custom_curve(this.Ptr, data);
                    }
                    break;
            case "flip_alternate":
                    /* rhinogradienttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'flip_alternate', 'ui_name': 'FlipAlternate'} */
                    {
                    CSycles.rhinogradienttexturenode_set_flip_alternate(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "gradient_type":
                    /* rhinogradienttexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_GRADIENT_LINEAR', 'default_value_type': 'RhinoProceduralGradientType', 'is_input': True, 'member_name': 'gradient_type', 'ui_name': 'GradientType'} */
                    {
                    CSycles.rhinogradienttexturenode_set_gradient_type(this.Ptr, (ccl.RhinoProceduralGradientType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "alpha1":
                /* rhinogradienttexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                {
                    return CSycles.rhinogradienttexturenode_get_alpha1(this.Ptr);
                }
            case "point_width":
                /* rhinogradienttexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'point_width', 'ui_name': 'PointWidth'} */
                {
                    return CSycles.rhinogradienttexturenode_get_point_width(this.Ptr);
                }
            case "alpha2":
                /* rhinogradienttexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                {
                    return CSycles.rhinogradienttexturenode_get_alpha2(this.Ptr);
                }
            case "point_height":
                /* rhinogradienttexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'point_height', 'ui_name': 'PointHeight'} */
                {
                    return CSycles.rhinogradienttexturenode_get_point_height(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinogradienttexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinogradienttexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color2":
                /* rhinogradienttexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinogradienttexturenode_get_color2(this.Ptr);
                }
            case "color1":
                /* rhinogradienttexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinogradienttexturenode_get_color1(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_custom_curve":
                /* rhinogradienttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_custom_curve', 'ui_name': 'UseCustomCurve'} */
                {
                    return CSycles.rhinogradienttexturenode_get_use_custom_curve(this.Ptr);
                }
            case "flip_alternate":
                /* rhinogradienttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'flip_alternate', 'ui_name': 'FlipAlternate'} */
                {
                    return CSycles.rhinogradienttexturenode_get_flip_alternate(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "gradient_type":
                /* rhinogradienttexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_GRADIENT_LINEAR', 'default_value_type': 'RhinoProceduralGradientType', 'is_input': True, 'member_name': 'gradient_type', 'ui_name': 'GradientType'} */
                {
                    return (uint)CSycles.rhinogradienttexturenode_get_gradient_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoGradientTextureNode (getter)");
            }
        }

#endregion
    }

}