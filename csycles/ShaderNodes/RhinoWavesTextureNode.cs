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

    public class RhinoWavesTextureNodeInputs : Inputs
    {
        public ColorSocket Color3 { get; private set; }
        public FloatSocket Contrast2 { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public EnumSocket WaveType { get; private set; }
        public FloatSocket Alpha1 { get; private set; }
        public FloatSocket WaveWidth { get; private set; }
        public ColorSocket Color2 { get; private set; }
        public BoolSocket WaveWidthTextureOn { get; private set; }
        public FloatSocket Alpha2 { get; private set; }
        public FloatSocket Contrast1 { get; private set; }
        public PointSocket UVW { get; private set; }

        public RhinoWavesTextureNodeInputs(ShaderNode parentNode)
        {
            Color3 = new ColorSocket(parentNode, "Color3", "color3", true);
            AddSocket(Color3);
            Contrast2 = new FloatSocket(parentNode, "Contrast2", "contrast2", true);
            AddSocket(Contrast2);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            WaveType = new EnumSocket(parentNode, "WaveType", "wave_type", true);
            AddSocket(WaveType);
            Alpha1 = new FloatSocket(parentNode, "Alpha1", "alpha1", true);
            AddSocket(Alpha1);
            WaveWidth = new FloatSocket(parentNode, "WaveWidth", "wave_width", true);
            AddSocket(WaveWidth);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            WaveWidthTextureOn = new BoolSocket(parentNode, "WaveWidthTextureOn", "wave_width_texture_on", true);
            AddSocket(WaveWidthTextureOn);
            Alpha2 = new FloatSocket(parentNode, "Alpha2", "alpha2", true);
            AddSocket(Alpha2);
            Contrast1 = new FloatSocket(parentNode, "Contrast1", "contrast1", true);
            AddSocket(Contrast1);
            UVW = new PointSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
        }
    }
    public class RhinoWavesTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoWavesTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_waves_texture")]
    public class RhinoWavesTextureNode : ShaderNode
    {
        public enum RhinoWavesTextureNodeWaveType : uint {
            Linear = ccl.RhinoProceduralWavesType.RHINO_WAVES_LINEAR,
            Radial = ccl.RhinoProceduralWavesType.RHINO_WAVES_RADIAL,
        }
        public RhinoWavesTextureNodeInputs ins => (RhinoWavesTextureNodeInputs)inputs;
        public RhinoWavesTextureNodeOutputs outs => (RhinoWavesTextureNodeOutputs)outputs;
        public RhinoWavesTextureNode(Shader shader) : this(shader, "a rhino_waves_texture node") { }

        public RhinoWavesTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoWavesTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoWavesTextureNodeInputs(this);
            outputs = new RhinoWavesTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinowavestexturenode_get_uvw(Ptr); }
            set { CSycles.rhinowavestexturenode_set_uvw(Ptr, value); }
        }

        public float3 Color3 {
            get { return CSycles.rhinowavestexturenode_get_color3(Ptr); }
            set { CSycles.rhinowavestexturenode_set_color3(Ptr, value); }
        }

        public bool WaveWidthTextureOn {
            get { return CSycles.rhinowavestexturenode_get_wave_width_texture_on(Ptr); }
            set { CSycles.rhinowavestexturenode_set_wave_width_texture_on(Ptr, value); }
        }

        public float Alpha1 {
            get { return CSycles.rhinowavestexturenode_get_alpha1(Ptr); }
            set { CSycles.rhinowavestexturenode_set_alpha1(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinowavestexturenode_get_color2(Ptr); }
            set { CSycles.rhinowavestexturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinowavestexturenode_get_node_type();
        }

        public float WaveWidth {
            get { return CSycles.rhinowavestexturenode_get_wave_width(Ptr); }
            set { CSycles.rhinowavestexturenode_set_wave_width(Ptr, value); }
        }

        public float3 Color1 {
            get { return CSycles.rhinowavestexturenode_get_color1(Ptr); }
            set { CSycles.rhinowavestexturenode_set_color1(Ptr, value); }
        }

        public float Alpha2 {
            get { return CSycles.rhinowavestexturenode_get_alpha2(Ptr); }
            set { CSycles.rhinowavestexturenode_set_alpha2(Ptr, value); }
        }

        public float Contrast2 {
            get { return CSycles.rhinowavestexturenode_get_contrast2(Ptr); }
            set { CSycles.rhinowavestexturenode_set_contrast2(Ptr, value); }
        }

        public RhinoProceduralWavesType WaveType {
            get { return CSycles.rhinowavestexturenode_get_wave_type(Ptr); }
            set { CSycles.rhinowavestexturenode_set_wave_type(Ptr, value); }
        }

        public float Contrast1 {
            get { return CSycles.rhinowavestexturenode_get_contrast1(Ptr); }
            set { CSycles.rhinowavestexturenode_set_contrast1(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "contrast2":
                    /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'contrast2', 'ui_name': 'Contrast2'} */
                    {
                    CSycles.rhinowavestexturenode_set_contrast2(this.Ptr, data);
                    }
                    break;
            case "alpha1":
                    /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                    {
                    CSycles.rhinowavestexturenode_set_alpha1(this.Ptr, data);
                    }
                    break;
            case "wave_width":
                    /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '0', 'default_value_type': '0', 'is_input': True, 'member_name': 'wave_width', 'ui_name': 'WaveWidth'} */
                    {
                    CSycles.rhinowavestexturenode_set_wave_width(this.Ptr, data);
                    }
                    break;
            case "alpha2":
                    /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                    {
                    CSycles.rhinowavestexturenode_set_alpha2(this.Ptr, data);
                    }
                    break;
            case "contrast1":
                    /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'contrast1', 'ui_name': 'Contrast1'} */
                    {
                    CSycles.rhinowavestexturenode_set_contrast1(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinowavestexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinowavestexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color3":
                    /* rhinowavestexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color3', 'ui_name': 'Color3'} */
                    {
                    CSycles.rhinowavestexturenode_set_color3(this.Ptr, data);
                    }
                    break;
            case "color1":
                    /* rhinowavestexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinowavestexturenode_set_color1(this.Ptr, data);
                    }
                    break;
            case "color2":
                    /* rhinowavestexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinowavestexturenode_set_color2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "wave_width_texture_on":
                    /* rhinowavestexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'wave_width_texture_on', 'ui_name': 'WaveWidthTextureOn'} */
                    {
                    CSycles.rhinowavestexturenode_set_wave_width_texture_on(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "wave_type":
                    /* rhinowavestexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_WAVES_LINEAR', 'default_value_type': 'RhinoProceduralWavesType', 'is_input': True, 'member_name': 'wave_type', 'ui_name': 'WaveType'} */
                    {
                    CSycles.rhinowavestexturenode_set_wave_type(this.Ptr, (ccl.RhinoProceduralWavesType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "contrast2":
                /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'contrast2', 'ui_name': 'Contrast2'} */
                {
                    return CSycles.rhinowavestexturenode_get_contrast2(this.Ptr);
                }
            case "alpha1":
                /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                {
                    return CSycles.rhinowavestexturenode_get_alpha1(this.Ptr);
                }
            case "wave_width":
                /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '0', 'default_value_type': '0', 'is_input': True, 'member_name': 'wave_width', 'ui_name': 'WaveWidth'} */
                {
                    return CSycles.rhinowavestexturenode_get_wave_width(this.Ptr);
                }
            case "alpha2":
                /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                {
                    return CSycles.rhinowavestexturenode_get_alpha2(this.Ptr);
                }
            case "contrast1":
                /* rhinowavestexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'contrast1', 'ui_name': 'Contrast1'} */
                {
                    return CSycles.rhinowavestexturenode_get_contrast1(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinowavestexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinowavestexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color3":
                /* rhinowavestexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color3', 'ui_name': 'Color3'} */
                {
                    return CSycles.rhinowavestexturenode_get_color3(this.Ptr);
                }
            case "color1":
                /* rhinowavestexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinowavestexturenode_get_color1(this.Ptr);
                }
            case "color2":
                /* rhinowavestexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinowavestexturenode_get_color2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "wave_width_texture_on":
                /* rhinowavestexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'wave_width_texture_on', 'ui_name': 'WaveWidthTextureOn'} */
                {
                    return CSycles.rhinowavestexturenode_get_wave_width_texture_on(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "wave_type":
                /* rhinowavestexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_WAVES_LINEAR', 'default_value_type': 'RhinoProceduralWavesType', 'is_input': True, 'member_name': 'wave_type', 'ui_name': 'WaveType'} */
                {
                    return (uint)CSycles.rhinowavestexturenode_get_wave_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesTextureNode (getter)");
            }
        }

#endregion
    }

}