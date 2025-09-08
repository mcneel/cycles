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

    public class WaveTextureNodeInputs : Inputs
    {
        public EnumSocket BandsDirection { get; private set; }
        public EnumSocket RingsDirection { get; private set; }
        public EnumSocket Profile { get; private set; }
        public PointSocket Vector { get; private set; }
        public FloatSocket Scale { get; private set; }
        public FloatSocket Distortion { get; private set; }
        public FloatSocket Detail { get; private set; }
        public FloatSocket DetailScale { get; private set; }
        public FloatSocket DetailRoughness { get; private set; }
        public EnumSocket Type { get; private set; }
        public FloatSocket PhaseOffset { get; private set; }

        public WaveTextureNodeInputs(ShaderNode parentNode)
        {
            BandsDirection = new EnumSocket(parentNode, "Bands Direction", "bands_direction", true);
            AddSocket(BandsDirection);
            RingsDirection = new EnumSocket(parentNode, "Rings Direction", "rings_direction", true);
            AddSocket(RingsDirection);
            Profile = new EnumSocket(parentNode, "Profile", "profile", true);
            AddSocket(Profile);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Scale = new FloatSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
            Distortion = new FloatSocket(parentNode, "Distortion", "distortion", true);
            AddSocket(Distortion);
            Detail = new FloatSocket(parentNode, "Detail", "detail", true);
            AddSocket(Detail);
            DetailScale = new FloatSocket(parentNode, "Detail Scale", "detail_scale", true);
            AddSocket(DetailScale);
            DetailRoughness = new FloatSocket(parentNode, "Detail Roughness", "detail_roughness", true);
            AddSocket(DetailRoughness);
            Type = new EnumSocket(parentNode, "Type", "wave_type", true);
            AddSocket(Type);
            PhaseOffset = new FloatSocket(parentNode, "Phase Offset", "phase", true);
            AddSocket(PhaseOffset);
        }
    }
    public class WaveTextureNodeOutputs : Outputs
    {
        public FloatSocket Fac { get; private set; }
        public ColorSocket Color { get; private set; }

        public WaveTextureNodeOutputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", false);
            AddSocket(Fac);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "wave_texture")]
    public class WaveTextureNode : TextureNode
    {
        public enum WaveTextureNodeBandsDirection : uint {
            X = ccl.NodeWaveBandsDirection.NODE_WAVE_BANDS_DIRECTION_X,
            Y = ccl.NodeWaveBandsDirection.NODE_WAVE_BANDS_DIRECTION_Y,
            Z = ccl.NodeWaveBandsDirection.NODE_WAVE_BANDS_DIRECTION_Z,
            Diagonal = ccl.NodeWaveBandsDirection.NODE_WAVE_BANDS_DIRECTION_DIAGONAL,
        }
        public enum WaveTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum WaveTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum WaveTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public enum WaveTextureNodeProfile : uint {
            Sine = ccl.NodeWaveProfile.NODE_WAVE_PROFILE_SIN,
            Saw = ccl.NodeWaveProfile.NODE_WAVE_PROFILE_SAW,
            Tri = ccl.NodeWaveProfile.NODE_WAVE_PROFILE_TRI,
        }
        public enum WaveTextureNodeRingsDirection : uint {
            X = ccl.NodeWaveRingsDirection.NODE_WAVE_RINGS_DIRECTION_X,
            Y = ccl.NodeWaveRingsDirection.NODE_WAVE_RINGS_DIRECTION_Y,
            Z = ccl.NodeWaveRingsDirection.NODE_WAVE_RINGS_DIRECTION_Z,
            Spherical = ccl.NodeWaveRingsDirection.NODE_WAVE_RINGS_DIRECTION_SPHERICAL,
        }
        public enum WaveTextureNodeType : uint {
            Bands = ccl.NodeWaveType.NODE_WAVE_BANDS,
            Rings = ccl.NodeWaveType.NODE_WAVE_RINGS,
        }
        public WaveTextureNodeInputs ins => (WaveTextureNodeInputs)inputs;
        public WaveTextureNodeOutputs outs => (WaveTextureNodeOutputs)outputs;
        public WaveTextureNode(Shader shader) : this(shader, "a wave_texture node") { }

        public WaveTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal WaveTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new WaveTextureNodeInputs(this);
            outputs = new WaveTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.wavetexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "scale":
                    /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.wavetexturenode_set_scale(this.Ptr, data);
                    }
                    break;
            case "distortion":
                    /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distortion', 'ui_name': 'Distortion'} */
                    {
                    CSycles.wavetexturenode_set_distortion(this.Ptr, data);
                    }
                    break;
            case "detail":
                    /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail', 'ui_name': 'Detail'} */
                    {
                    CSycles.wavetexturenode_set_detail(this.Ptr, data);
                    }
                    break;
            case "detail_scale":
                    /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail_scale', 'ui_name': 'Detail Scale'} */
                    {
                    CSycles.wavetexturenode_set_detail_scale(this.Ptr, data);
                    }
                    break;
            case "detail_roughness":
                    /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail_roughness', 'ui_name': 'Detail Roughness'} */
                    {
                    CSycles.wavetexturenode_set_detail_roughness(this.Ptr, data);
                    }
                    break;
            case "phase":
                    /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'phase', 'ui_name': 'Phase Offset'} */
                    {
                    CSycles.wavetexturenode_set_phase(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WaveTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* wavetexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.wavetexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WaveTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "bands_direction":
                    /* wavetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_WAVE_BANDS_DIRECTION_X', 'default_value_type': 'NodeWaveBandsDirection', 'is_input': True, 'member_name': 'bands_direction', 'ui_name': 'Bands Direction'} */
                    {
                    CSycles.wavetexturenode_set_bands_direction(this.Ptr, (ccl.NodeWaveBandsDirection)data);
                    }
                    break;
            case "rings_direction":
                    /* wavetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_WAVE_RINGS_DIRECTION_X', 'default_value_type': 'NodeWaveRingsDirection', 'is_input': True, 'member_name': 'rings_direction', 'ui_name': 'Rings Direction'} */
                    {
                    CSycles.wavetexturenode_set_rings_direction(this.Ptr, (ccl.NodeWaveRingsDirection)data);
                    }
                    break;
            case "profile":
                    /* wavetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_WAVE_PROFILE_SIN', 'default_value_type': 'NodeWaveProfile', 'is_input': True, 'member_name': 'profile', 'ui_name': 'Profile'} */
                    {
                    CSycles.wavetexturenode_set_profile(this.Ptr, (ccl.NodeWaveProfile)data);
                    }
                    break;
            case "wave_type":
                    /* wavetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_WAVE_BANDS', 'default_value_type': 'NodeWaveType', 'is_input': True, 'member_name': 'wave_type', 'ui_name': 'Type'} */
                    {
                    CSycles.wavetexturenode_set_wave_type(this.Ptr, (ccl.NodeWaveType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WaveTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "scale":
                /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.wavetexturenode_get_scale(this.Ptr);
                }
            case "distortion":
                /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distortion', 'ui_name': 'Distortion'} */
                {
                    return CSycles.wavetexturenode_get_distortion(this.Ptr);
                }
            case "detail":
                /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail', 'ui_name': 'Detail'} */
                {
                    return CSycles.wavetexturenode_get_detail(this.Ptr);
                }
            case "detail_scale":
                /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail_scale', 'ui_name': 'Detail Scale'} */
                {
                    return CSycles.wavetexturenode_get_detail_scale(this.Ptr);
                }
            case "detail_roughness":
                /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail_roughness', 'ui_name': 'Detail Roughness'} */
                {
                    return CSycles.wavetexturenode_get_detail_roughness(this.Ptr);
                }
            case "phase":
                /* wavetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'phase', 'ui_name': 'Phase Offset'} */
                {
                    return CSycles.wavetexturenode_get_phase(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WaveTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* wavetexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.wavetexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WaveTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "bands_direction":
                /* wavetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_WAVE_BANDS_DIRECTION_X', 'default_value_type': 'NodeWaveBandsDirection', 'is_input': True, 'member_name': 'bands_direction', 'ui_name': 'Bands Direction'} */
                {
                    return (uint)CSycles.wavetexturenode_get_bands_direction(this.Ptr);
                }
            case "rings_direction":
                /* wavetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_WAVE_RINGS_DIRECTION_X', 'default_value_type': 'NodeWaveRingsDirection', 'is_input': True, 'member_name': 'rings_direction', 'ui_name': 'Rings Direction'} */
                {
                    return (uint)CSycles.wavetexturenode_get_rings_direction(this.Ptr);
                }
            case "profile":
                /* wavetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_WAVE_PROFILE_SIN', 'default_value_type': 'NodeWaveProfile', 'is_input': True, 'member_name': 'profile', 'ui_name': 'Profile'} */
                {
                    return (uint)CSycles.wavetexturenode_get_profile(this.Ptr);
                }
            case "wave_type":
                /* wavetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_WAVE_BANDS', 'default_value_type': 'NodeWaveType', 'is_input': True, 'member_name': 'wave_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.wavetexturenode_get_wave_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WaveTextureNode (getter)");
            }
        }

#endregion
    }

}