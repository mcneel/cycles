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

    public class PrincipledVolumeNodeInputs : Inputs
    {
        public FloatSocket EmissionStrength { get; private set; }
        public ColorSocket Color { get; private set; }
        public ColorSocket EmissionColor { get; private set; }
        public FloatSocket Density { get; private set; }
        public FloatSocket BlackbodyIntensity { get; private set; }
        public StringSocket DensityAttribute { get; private set; }
        public FloatSocket Anisotropy { get; private set; }
        public ColorSocket BlackbodyTint { get; private set; }
        public StringSocket ColorAttribute { get; private set; }
        public ColorSocket AbsorptionColor { get; private set; }
        public FloatSocket Temperature { get; private set; }
        public StringSocket TemperatureAttribute { get; private set; }

        public PrincipledVolumeNodeInputs(ShaderNode parentNode)
        {
            EmissionStrength = new FloatSocket(parentNode, "Emission Strength", "emission_strength", true);
            AddSocket(EmissionStrength);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            EmissionColor = new ColorSocket(parentNode, "Emission Color", "emission_color", true);
            AddSocket(EmissionColor);
            Density = new FloatSocket(parentNode, "Density", "density", true);
            AddSocket(Density);
            BlackbodyIntensity = new FloatSocket(parentNode, "Blackbody Intensity", "blackbody_intensity", true);
            AddSocket(BlackbodyIntensity);
            DensityAttribute = new StringSocket(parentNode, "Density Attribute", "density_attribute", true);
            AddSocket(DensityAttribute);
            Anisotropy = new FloatSocket(parentNode, "Anisotropy", "anisotropy", true);
            AddSocket(Anisotropy);
            BlackbodyTint = new ColorSocket(parentNode, "Blackbody Tint", "blackbody_tint", true);
            AddSocket(BlackbodyTint);
            ColorAttribute = new StringSocket(parentNode, "Color Attribute", "color_attribute", true);
            AddSocket(ColorAttribute);
            AbsorptionColor = new ColorSocket(parentNode, "Absorption Color", "absorption_color", true);
            AddSocket(AbsorptionColor);
            Temperature = new FloatSocket(parentNode, "Temperature", "temperature", true);
            AddSocket(Temperature);
            TemperatureAttribute = new StringSocket(parentNode, "Temperature Attribute", "temperature_attribute", true);
            AddSocket(TemperatureAttribute);
        }
    }
    public class PrincipledVolumeNodeOutputs : Outputs
    {
        public ClosureSocket Volume { get; private set; }

        public PrincipledVolumeNodeOutputs(ShaderNode parentNode)
        {
            Volume = new ClosureSocket(parentNode, "Volume", "volume", false);
            AddSocket(Volume);
        }
    }

    [ShaderNode(name: "principled_volume")]
    public class PrincipledVolumeNode : VolumeNode
    {
        public PrincipledVolumeNodeInputs ins => (PrincipledVolumeNodeInputs)inputs;
        public PrincipledVolumeNodeOutputs outs => (PrincipledVolumeNodeOutputs)outputs;
        public PrincipledVolumeNode(Shader shader) : this(shader, "a principled_volume node") { }

        public PrincipledVolumeNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal PrincipledVolumeNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new PrincipledVolumeNodeInputs(this);
            outputs = new PrincipledVolumeNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.Volume;
        }
        public static IntPtr GetNodeType() {
            return CSycles.principledvolumenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "emission_strength":
                    /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'emission_strength', 'ui_name': 'Emission Strength'} */
                    {
                    CSycles.principledvolumenode_set_emission_strength(this.Ptr, data);
                    }
                    break;
            case "density":
                    /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'density', 'ui_name': 'Density'} */
                    {
                    CSycles.volumenode_set_density(this.Ptr, data);
                    }
                    break;
            case "blackbody_intensity":
                    /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'blackbody_intensity', 'ui_name': 'Blackbody Intensity'} */
                    {
                    CSycles.principledvolumenode_set_blackbody_intensity(this.Ptr, data);
                    }
                    break;
            case "anisotropy":
                    /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                    {
                    CSycles.principledvolumenode_set_anisotropy(this.Ptr, data);
                    }
                    break;
            case "temperature":
                    /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '1000.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'temperature', 'ui_name': 'Temperature'} */
                    {
                    CSycles.principledvolumenode_set_temperature(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledVolumeNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* principledvolumenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.5f,0.5f,0.5f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.volumenode_set_color(this.Ptr, data);
                    }
                    break;
            case "emission_color":
                    /* principledvolumenode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'emission_color', 'ui_name': 'Emission Color'} */
                    {
                    CSycles.principledvolumenode_set_emission_color(this.Ptr, data);
                    }
                    break;
            case "blackbody_tint":
                    /* principledvolumenode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'blackbody_tint', 'ui_name': 'Blackbody Tint'} */
                    {
                    CSycles.principledvolumenode_set_blackbody_tint(this.Ptr, data);
                    }
                    break;
            case "absorption_color":
                    /* principledvolumenode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'absorption_color', 'ui_name': 'Absorption Color'} */
                    {
                    CSycles.principledvolumenode_set_absorption_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledVolumeNode (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "density_attribute":
                    /* principledvolumenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'density_attribute', 'ui_name': 'Density Attribute'} */
                    {
                    CSycles.principledvolumenode_set_density_attribute(this.Ptr, data);
                    }
                    break;
            case "color_attribute":
                    /* principledvolumenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'color_attribute', 'ui_name': 'Color Attribute'} */
                    {
                    CSycles.principledvolumenode_set_color_attribute(this.Ptr, data);
                    }
                    break;
            case "temperature_attribute":
                    /* principledvolumenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'temperature_attribute', 'ui_name': 'Temperature Attribute'} */
                    {
                    CSycles.principledvolumenode_set_temperature_attribute(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledVolumeNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "emission_strength":
                /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'emission_strength', 'ui_name': 'Emission Strength'} */
                {
                    return CSycles.principledvolumenode_get_emission_strength(this.Ptr);
                }
            case "density":
                /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'density', 'ui_name': 'Density'} */
                {
                    return CSycles.volumenode_get_density(this.Ptr);
                }
            case "blackbody_intensity":
                /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'blackbody_intensity', 'ui_name': 'Blackbody Intensity'} */
                {
                    return CSycles.principledvolumenode_get_blackbody_intensity(this.Ptr);
                }
            case "anisotropy":
                /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                {
                    return CSycles.principledvolumenode_get_anisotropy(this.Ptr);
                }
            case "temperature":
                /* principledvolumenode . {'datatype': 'FLOAT', 'default_value': '1000.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'temperature', 'ui_name': 'Temperature'} */
                {
                    return CSycles.principledvolumenode_get_temperature(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledVolumeNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* principledvolumenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.5f,0.5f,0.5f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.volumenode_get_color(this.Ptr);
                }
            case "emission_color":
                /* principledvolumenode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'emission_color', 'ui_name': 'Emission Color'} */
                {
                    return CSycles.principledvolumenode_get_emission_color(this.Ptr);
                }
            case "blackbody_tint":
                /* principledvolumenode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'blackbody_tint', 'ui_name': 'Blackbody Tint'} */
                {
                    return CSycles.principledvolumenode_get_blackbody_tint(this.Ptr);
                }
            case "absorption_color":
                /* principledvolumenode . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'absorption_color', 'ui_name': 'Absorption Color'} */
                {
                    return CSycles.principledvolumenode_get_absorption_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledVolumeNode (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "density_attribute":
                /* principledvolumenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'density_attribute', 'ui_name': 'Density Attribute'} */
                {
                    return CSycles.principledvolumenode_get_density_attribute(this.Ptr);
                }
            case "color_attribute":
                /* principledvolumenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'color_attribute', 'ui_name': 'Color Attribute'} */
                {
                    return CSycles.principledvolumenode_get_color_attribute(this.Ptr);
                }
            case "temperature_attribute":
                /* principledvolumenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'temperature_attribute', 'ui_name': 'Temperature Attribute'} */
                {
                    return CSycles.principledvolumenode_get_temperature_attribute(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PrincipledVolumeNode (getter)");
            }
        }

#endregion
    }

}