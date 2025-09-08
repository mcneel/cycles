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

    public class RhinoPhysicalSkyTextureNodeInputs : Inputs
    {
        public FloatSocket Exposure { get; private set; }
        public VectorSocket SunDirection { get; private set; }
        public FloatSocket SunBrightness { get; private set; }
        public FloatSocket AtmosphericDensity { get; private set; }
        public FloatSocket SunSize { get; private set; }
        public FloatSocket RayleighScattering { get; private set; }
        public VectorSocket SunColor { get; private set; }
        public FloatSocket MieScattering { get; private set; }
        public VectorSocket InverseWavelengths { get; private set; }
        public VectorSocket UVW { get; private set; }
        public BoolSocket ShowSun { get; private set; }

        public RhinoPhysicalSkyTextureNodeInputs(ShaderNode parentNode)
        {
            Exposure = new FloatSocket(parentNode, "Exposure", "exposure", true);
            AddSocket(Exposure);
            SunDirection = new VectorSocket(parentNode, "SunDirection", "sun_dir", true);
            AddSocket(SunDirection);
            SunBrightness = new FloatSocket(parentNode, "SunBrightness", "sun_brightness", true);
            AddSocket(SunBrightness);
            AtmosphericDensity = new FloatSocket(parentNode, "AtmosphericDensity", "atmospheric_density", true);
            AddSocket(AtmosphericDensity);
            SunSize = new FloatSocket(parentNode, "SunSize", "sun_size", true);
            AddSocket(SunSize);
            RayleighScattering = new FloatSocket(parentNode, "RayleighScattering", "rayleigh_scattering", true);
            AddSocket(RayleighScattering);
            SunColor = new VectorSocket(parentNode, "SunColor", "sun_color", true);
            AddSocket(SunColor);
            MieScattering = new FloatSocket(parentNode, "MieScattering", "mie_scattering", true);
            AddSocket(MieScattering);
            InverseWavelengths = new VectorSocket(parentNode, "InverseWavelengths", "inv_wavelengths", true);
            AddSocket(InverseWavelengths);
            UVW = new VectorSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            ShowSun = new BoolSocket(parentNode, "ShowSun", "show_sun", true);
            AddSocket(ShowSun);
        }
    }
    public class RhinoPhysicalSkyTextureNodeOutputs : Outputs
    {
        public VectorSocket Color { get; private set; }

        public RhinoPhysicalSkyTextureNodeOutputs(ShaderNode parentNode)
        {
            Color = new VectorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_physical_sky_texture")]
    public class RhinoPhysicalSkyTextureNode : ShaderNode
    {
        public RhinoPhysicalSkyTextureNodeInputs ins => (RhinoPhysicalSkyTextureNodeInputs)inputs;
        public RhinoPhysicalSkyTextureNodeOutputs outs => (RhinoPhysicalSkyTextureNodeOutputs)outputs;
        public RhinoPhysicalSkyTextureNode(Shader shader) : this(shader, "a rhino_physical_sky_texture node") { }

        public RhinoPhysicalSkyTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoPhysicalSkyTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoPhysicalSkyTextureNodeInputs(this);
            outputs = new RhinoPhysicalSkyTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinophysicalskytexturenode_get_uvw(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_uvw(Ptr, value); }
        }

        public float Exposure {
            get { return CSycles.rhinophysicalskytexturenode_get_exposure(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_exposure(Ptr, value); }
        }

        public float RayleighScattering {
            get { return CSycles.rhinophysicalskytexturenode_get_rayleigh_scattering(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_rayleigh_scattering(Ptr, value); }
        }

        public float3 SunColor {
            get { return CSycles.rhinophysicalskytexturenode_get_sun_color(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_sun_color(Ptr, value); }
        }

        public float MieScattering {
            get { return CSycles.rhinophysicalskytexturenode_get_mie_scattering(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_mie_scattering(Ptr, value); }
        }

        public float3 InvWavelengths {
            get { return CSycles.rhinophysicalskytexturenode_get_inv_wavelengths(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_inv_wavelengths(Ptr, value); }
        }

        public bool ShowSun {
            get { return CSycles.rhinophysicalskytexturenode_get_show_sun(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_show_sun(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinophysicalskytexturenode_get_node_type();
        }

        public float SunBrightness {
            get { return CSycles.rhinophysicalskytexturenode_get_sun_brightness(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_sun_brightness(Ptr, value); }
        }

        public float3 SunDir {
            get { return CSycles.rhinophysicalskytexturenode_get_sun_dir(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_sun_dir(Ptr, value); }
        }

        public float AtmosphericDensity {
            get { return CSycles.rhinophysicalskytexturenode_get_atmospheric_density(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_atmospheric_density(Ptr, value); }
        }

        public float SunSize {
            get { return CSycles.rhinophysicalskytexturenode_get_sun_size(Ptr); }
            set { CSycles.rhinophysicalskytexturenode_set_sun_size(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "exposure":
                    /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exposure', 'ui_name': 'Exposure'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_exposure(this.Ptr, data);
                    }
                    break;
            case "sun_brightness":
                    /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_brightness', 'ui_name': 'SunBrightness'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_sun_brightness(this.Ptr, data);
                    }
                    break;
            case "atmospheric_density":
                    /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'atmospheric_density', 'ui_name': 'AtmosphericDensity'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_atmospheric_density(this.Ptr, data);
                    }
                    break;
            case "sun_size":
                    /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_size', 'ui_name': 'SunSize'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_sun_size(this.Ptr, data);
                    }
                    break;
            case "rayleigh_scattering":
                    /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'rayleigh_scattering', 'ui_name': 'RayleighScattering'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_rayleigh_scattering(this.Ptr, data);
                    }
                    break;
            case "mie_scattering":
                    /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mie_scattering', 'ui_name': 'MieScattering'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_mie_scattering(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPhysicalSkyTextureNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "sun_dir":
                    /* rhinophysicalskytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'sun_dir', 'ui_name': 'SunDirection'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_sun_dir(this.Ptr, data);
                    }
                    break;
            case "sun_color":
                    /* rhinophysicalskytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'sun_color', 'ui_name': 'SunColor'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_sun_color(this.Ptr, data);
                    }
                    break;
            case "inv_wavelengths":
                    /* rhinophysicalskytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'inv_wavelengths', 'ui_name': 'InverseWavelengths'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_inv_wavelengths(this.Ptr, data);
                    }
                    break;
            case "uvw":
                    /* rhinophysicalskytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPhysicalSkyTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "show_sun":
                    /* rhinophysicalskytexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'show_sun', 'ui_name': 'ShowSun'} */
                    {
                    CSycles.rhinophysicalskytexturenode_set_show_sun(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPhysicalSkyTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "exposure":
                /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exposure', 'ui_name': 'Exposure'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_exposure(this.Ptr);
                }
            case "sun_brightness":
                /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_brightness', 'ui_name': 'SunBrightness'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_sun_brightness(this.Ptr);
                }
            case "atmospheric_density":
                /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'atmospheric_density', 'ui_name': 'AtmosphericDensity'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_atmospheric_density(this.Ptr);
                }
            case "sun_size":
                /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_size', 'ui_name': 'SunSize'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_sun_size(this.Ptr);
                }
            case "rayleigh_scattering":
                /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'rayleigh_scattering', 'ui_name': 'RayleighScattering'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_rayleigh_scattering(this.Ptr);
                }
            case "mie_scattering":
                /* rhinophysicalskytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'mie_scattering', 'ui_name': 'MieScattering'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_mie_scattering(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPhysicalSkyTextureNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "sun_dir":
                /* rhinophysicalskytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'sun_dir', 'ui_name': 'SunDirection'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_sun_dir(this.Ptr);
                }
            case "sun_color":
                /* rhinophysicalskytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'sun_color', 'ui_name': 'SunColor'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_sun_color(this.Ptr);
                }
            case "inv_wavelengths":
                /* rhinophysicalskytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'inv_wavelengths', 'ui_name': 'InverseWavelengths'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_inv_wavelengths(this.Ptr);
                }
            case "uvw":
                /* rhinophysicalskytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPhysicalSkyTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "show_sun":
                /* rhinophysicalskytexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'show_sun', 'ui_name': 'ShowSun'} */
                {
                    return CSycles.rhinophysicalskytexturenode_get_show_sun(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPhysicalSkyTextureNode (getter)");
            }
        }

#endregion
    }

}