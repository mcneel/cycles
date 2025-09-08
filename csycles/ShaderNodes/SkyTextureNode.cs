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

    public class SkyTextureNodeInputs : Inputs
    {
        public VectorSocket SunDirection { get; private set; }
        public FloatSocket Dust { get; private set; }
        public FloatSocket Turbidity { get; private set; }
        public FloatSocket Ozone { get; private set; }
        public FloatSocket GroundAlbedo { get; private set; }
        public PointSocket Vector { get; private set; }
        public BoolSocket SunDisc { get; private set; }
        public FloatSocket SunSize { get; private set; }
        public FloatSocket SunIntensity { get; private set; }
        public FloatSocket SunElevation { get; private set; }
        public FloatSocket SunRotation { get; private set; }
        public FloatSocket Altitude { get; private set; }
        public EnumSocket Type { get; private set; }
        public FloatSocket Air { get; private set; }

        public SkyTextureNodeInputs(ShaderNode parentNode)
        {
            SunDirection = new VectorSocket(parentNode, "Sun Direction", "sun_direction", true);
            AddSocket(SunDirection);
            Dust = new FloatSocket(parentNode, "Dust", "dust_density", true);
            AddSocket(Dust);
            Turbidity = new FloatSocket(parentNode, "Turbidity", "turbidity", true);
            AddSocket(Turbidity);
            Ozone = new FloatSocket(parentNode, "Ozone", "ozone_density", true);
            AddSocket(Ozone);
            GroundAlbedo = new FloatSocket(parentNode, "Ground Albedo", "ground_albedo", true);
            AddSocket(GroundAlbedo);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            SunDisc = new BoolSocket(parentNode, "Sun Disc", "sun_disc", true);
            AddSocket(SunDisc);
            SunSize = new FloatSocket(parentNode, "Sun Size", "sun_size", true);
            AddSocket(SunSize);
            SunIntensity = new FloatSocket(parentNode, "Sun Intensity", "sun_intensity", true);
            AddSocket(SunIntensity);
            SunElevation = new FloatSocket(parentNode, "Sun Elevation", "sun_elevation", true);
            AddSocket(SunElevation);
            SunRotation = new FloatSocket(parentNode, "Sun Rotation", "sun_rotation", true);
            AddSocket(SunRotation);
            Altitude = new FloatSocket(parentNode, "Altitude", "altitude", true);
            AddSocket(Altitude);
            Type = new EnumSocket(parentNode, "Type", "sky_type", true);
            AddSocket(Type);
            Air = new FloatSocket(parentNode, "Air", "air_density", true);
            AddSocket(Air);
        }
    }
    public class SkyTextureNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public SkyTextureNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "sky_texture")]
    public class SkyTextureNode : TextureNode
    {
        public enum SkyTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum SkyTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum SkyTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public enum SkyTextureNodeType : uint {
            Preetham = ccl.NodeSkyType.NODE_SKY_PREETHAM,
            HosekWilkie = ccl.NodeSkyType.NODE_SKY_HOSEK,
            NishitaImproved = ccl.NodeSkyType.NODE_SKY_NISHITA,
        }
        public SkyTextureNodeInputs ins => (SkyTextureNodeInputs)inputs;
        public SkyTextureNodeOutputs outs => (SkyTextureNodeOutputs)outputs;
        public SkyTextureNode(Shader shader) : this(shader, "a sky_texture node") { }

        public SkyTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal SkyTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new SkyTextureNodeInputs(this);
            outputs = new SkyTextureNodeOutputs(this);
        }
        public float GetSunAverageRadiance() {
            return CSycles.skytexturenode_get_sun_average_radiance(Ptr);
        }
        public static IntPtr GetNodeType() {
            return CSycles.skytexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "dust_density":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'dust_density', 'ui_name': 'Dust'} */
                    {
                    CSycles.skytexturenode_set_dust_density(this.Ptr, data);
                    }
                    break;
            case "turbidity":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '2.2f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'turbidity', 'ui_name': 'Turbidity'} */
                    {
                    CSycles.skytexturenode_set_turbidity(this.Ptr, data);
                    }
                    break;
            case "ozone_density":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ozone_density', 'ui_name': 'Ozone'} */
                    {
                    CSycles.skytexturenode_set_ozone_density(this.Ptr, data);
                    }
                    break;
            case "ground_albedo":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '0.3f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ground_albedo', 'ui_name': 'Ground Albedo'} */
                    {
                    CSycles.skytexturenode_set_ground_albedo(this.Ptr, data);
                    }
                    break;
            case "sun_size":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '0.009512f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_size', 'ui_name': 'Sun Size'} */
                    {
                    CSycles.skytexturenode_set_sun_size(this.Ptr, data);
                    }
                    break;
            case "sun_intensity":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_intensity', 'ui_name': 'Sun Intensity'} */
                    {
                    CSycles.skytexturenode_set_sun_intensity(this.Ptr, data);
                    }
                    break;
            case "sun_elevation":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '15.0f*M_PI_F/180.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_elevation', 'ui_name': 'Sun Elevation'} */
                    {
                    CSycles.skytexturenode_set_sun_elevation(this.Ptr, data);
                    }
                    break;
            case "sun_rotation":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_rotation', 'ui_name': 'Sun Rotation'} */
                    {
                    CSycles.skytexturenode_set_sun_rotation(this.Ptr, data);
                    }
                    break;
            case "altitude":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'altitude', 'ui_name': 'Altitude'} */
                    {
                    CSycles.skytexturenode_set_altitude(this.Ptr, data);
                    }
                    break;
            case "air_density":
                    /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'air_density', 'ui_name': 'Air'} */
                    {
                    CSycles.skytexturenode_set_air_density(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "sun_direction":
                    /* skytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,1.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'sun_direction', 'ui_name': 'Sun Direction'} */
                    {
                    CSycles.skytexturenode_set_sun_direction(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* skytexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.skytexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "sun_disc":
                    /* skytexturenode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'sun_disc', 'ui_name': 'Sun Disc'} */
                    {
                    CSycles.skytexturenode_set_sun_disc(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "sky_type":
                    /* skytexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_SKY_NISHITA', 'default_value_type': 'NodeSkyType', 'is_input': True, 'member_name': 'sky_type', 'ui_name': 'Type'} */
                    {
                    CSycles.skytexturenode_set_sky_type(this.Ptr, (ccl.NodeSkyType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "dust_density":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'dust_density', 'ui_name': 'Dust'} */
                {
                    return CSycles.skytexturenode_get_dust_density(this.Ptr);
                }
            case "turbidity":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '2.2f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'turbidity', 'ui_name': 'Turbidity'} */
                {
                    return CSycles.skytexturenode_get_turbidity(this.Ptr);
                }
            case "ozone_density":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ozone_density', 'ui_name': 'Ozone'} */
                {
                    return CSycles.skytexturenode_get_ozone_density(this.Ptr);
                }
            case "ground_albedo":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '0.3f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ground_albedo', 'ui_name': 'Ground Albedo'} */
                {
                    return CSycles.skytexturenode_get_ground_albedo(this.Ptr);
                }
            case "sun_size":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '0.009512f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_size', 'ui_name': 'Sun Size'} */
                {
                    return CSycles.skytexturenode_get_sun_size(this.Ptr);
                }
            case "sun_intensity":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_intensity', 'ui_name': 'Sun Intensity'} */
                {
                    return CSycles.skytexturenode_get_sun_intensity(this.Ptr);
                }
            case "sun_elevation":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '15.0f*M_PI_F/180.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_elevation', 'ui_name': 'Sun Elevation'} */
                {
                    return CSycles.skytexturenode_get_sun_elevation(this.Ptr);
                }
            case "sun_rotation":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sun_rotation', 'ui_name': 'Sun Rotation'} */
                {
                    return CSycles.skytexturenode_get_sun_rotation(this.Ptr);
                }
            case "altitude":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'altitude', 'ui_name': 'Altitude'} */
                {
                    return CSycles.skytexturenode_get_altitude(this.Ptr);
                }
            case "air_density":
                /* skytexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'air_density', 'ui_name': 'Air'} */
                {
                    return CSycles.skytexturenode_get_air_density(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "sun_direction":
                /* skytexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,1.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'sun_direction', 'ui_name': 'Sun Direction'} */
                {
                    return CSycles.skytexturenode_get_sun_direction(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* skytexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.skytexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "sun_disc":
                /* skytexturenode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'sun_disc', 'ui_name': 'Sun Disc'} */
                {
                    return CSycles.skytexturenode_get_sun_disc(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "sky_type":
                /* skytexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_SKY_NISHITA', 'default_value_type': 'NodeSkyType', 'is_input': True, 'member_name': 'sky_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.skytexturenode_get_sky_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SkyTextureNode (getter)");
            }
        }

#endregion
    }

}