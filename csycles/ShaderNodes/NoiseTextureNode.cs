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

    public class NoiseTextureNodeInputs : Inputs
    {
        public FloatSocket Scale { get; private set; }
        public FloatSocket Detail { get; private set; }
        public FloatSocket Roughness { get; private set; }
        public FloatSocket Lacunarity { get; private set; }
        public EnumSocket Dimensions { get; private set; }
        public FloatSocket Offset { get; private set; }
        public EnumSocket Type { get; private set; }
        public FloatSocket Gain { get; private set; }
        public BoolSocket Normalize { get; private set; }
        public FloatSocket Distortion { get; private set; }
        public PointSocket Vector { get; private set; }
        public FloatSocket W { get; private set; }

        public NoiseTextureNodeInputs(ShaderNode parentNode)
        {
            Scale = new FloatSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
            Detail = new FloatSocket(parentNode, "Detail", "detail", true);
            AddSocket(Detail);
            Roughness = new FloatSocket(parentNode, "Roughness", "roughness", true);
            AddSocket(Roughness);
            Lacunarity = new FloatSocket(parentNode, "Lacunarity", "lacunarity", true);
            AddSocket(Lacunarity);
            Dimensions = new EnumSocket(parentNode, "Dimensions", "dimensions", true);
            AddSocket(Dimensions);
            Offset = new FloatSocket(parentNode, "Offset", "offset", true);
            AddSocket(Offset);
            Type = new EnumSocket(parentNode, "Type", "type", true);
            AddSocket(Type);
            Gain = new FloatSocket(parentNode, "Gain", "gain", true);
            AddSocket(Gain);
            Normalize = new BoolSocket(parentNode, "Normalize", "use_normalize", true);
            AddSocket(Normalize);
            Distortion = new FloatSocket(parentNode, "Distortion", "distortion", true);
            AddSocket(Distortion);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            W = new FloatSocket(parentNode, "W", "w", true);
            AddSocket(W);
        }
    }
    public class NoiseTextureNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }
        public FloatSocket Fac { get; private set; }

        public NoiseTextureNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
            Fac = new FloatSocket(parentNode, "Fac", "fac", false);
            AddSocket(Fac);
        }
    }

    [ShaderNode(name: "noise_texture")]
    public class NoiseTextureNode : TextureNode
    {
        public enum NoiseTextureNodeDimensions : uint {
            Dim1d = ccl.Dimensions.DIM1D,
            Dim2d = ccl.Dimensions.DIM2D,
            Dim3d = ccl.Dimensions.DIM3D,
            Dim4d = ccl.Dimensions.DIM4D,
        }
        public enum NoiseTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum NoiseTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum NoiseTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public enum NoiseTextureNodeType : uint {
            Multifractal = ccl.NodeNoiseType.NODE_NOISE_MULTIFRACTAL,
            Fbm = ccl.NodeNoiseType.NODE_NOISE_FBM,
            HybridMultifractal = ccl.NodeNoiseType.NODE_NOISE_HYBRID_MULTIFRACTAL,
            RidgedMultifractal = ccl.NodeNoiseType.NODE_NOISE_RIDGED_MULTIFRACTAL,
            HeteroTerrain = ccl.NodeNoiseType.NODE_NOISE_HETERO_TERRAIN,
        }
        public NoiseTextureNodeInputs ins => (NoiseTextureNodeInputs)inputs;
        public NoiseTextureNodeOutputs outs => (NoiseTextureNodeOutputs)outputs;
        public NoiseTextureNode(Shader shader) : this(shader, "a noise_texture node") { }

        public NoiseTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal NoiseTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new NoiseTextureNodeInputs(this);
            outputs = new NoiseTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.noisetexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "scale":
                    /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.noisetexturenode_set_scale(this.Ptr, data);
                    }
                    break;
            case "detail":
                    /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail', 'ui_name': 'Detail'} */
                    {
                    CSycles.noisetexturenode_set_detail(this.Ptr, data);
                    }
                    break;
            case "roughness":
                    /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                    {
                    CSycles.noisetexturenode_set_roughness(this.Ptr, data);
                    }
                    break;
            case "lacunarity":
                    /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'lacunarity', 'ui_name': 'Lacunarity'} */
                    {
                    CSycles.noisetexturenode_set_lacunarity(this.Ptr, data);
                    }
                    break;
            case "offset":
                    /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'offset', 'ui_name': 'Offset'} */
                    {
                    CSycles.noisetexturenode_set_offset(this.Ptr, data);
                    }
                    break;
            case "gain":
                    /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gain', 'ui_name': 'Gain'} */
                    {
                    CSycles.noisetexturenode_set_gain(this.Ptr, data);
                    }
                    break;
            case "distortion":
                    /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distortion', 'ui_name': 'Distortion'} */
                    {
                    CSycles.noisetexturenode_set_distortion(this.Ptr, data);
                    }
                    break;
            case "w":
                    /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'w', 'ui_name': 'W'} */
                    {
                    CSycles.noisetexturenode_set_w(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NoiseTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* noisetexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.noisetexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NoiseTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_normalize":
                    /* noisetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_normalize', 'ui_name': 'Normalize'} */
                    {
                    CSycles.noisetexturenode_set_use_normalize(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NoiseTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "dimensions":
                    /* noisetexturenode . {'datatype': 'ENUM', 'default_value': 'DIM3D', 'default_value_type': 'Dimensions', 'is_input': True, 'member_name': 'dimensions', 'ui_name': 'Dimensions'} */
                    {
                    CSycles.noisetexturenode_set_dimensions(this.Ptr, (ccl.Dimensions)data);
                    }
                    break;
            case "type":
                    /* noisetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_NOISE_FBM', 'default_value_type': 'NodeNoiseType', 'is_input': True, 'member_name': 'type', 'ui_name': 'Type'} */
                    {
                    CSycles.noisetexturenode_set_type(this.Ptr, (ccl.NodeNoiseType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NoiseTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "scale":
                /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.noisetexturenode_get_scale(this.Ptr);
                }
            case "detail":
                /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail', 'ui_name': 'Detail'} */
                {
                    return CSycles.noisetexturenode_get_detail(this.Ptr);
                }
            case "roughness":
                /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                {
                    return CSycles.noisetexturenode_get_roughness(this.Ptr);
                }
            case "lacunarity":
                /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'lacunarity', 'ui_name': 'Lacunarity'} */
                {
                    return CSycles.noisetexturenode_get_lacunarity(this.Ptr);
                }
            case "offset":
                /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'offset', 'ui_name': 'Offset'} */
                {
                    return CSycles.noisetexturenode_get_offset(this.Ptr);
                }
            case "gain":
                /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gain', 'ui_name': 'Gain'} */
                {
                    return CSycles.noisetexturenode_get_gain(this.Ptr);
                }
            case "distortion":
                /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distortion', 'ui_name': 'Distortion'} */
                {
                    return CSycles.noisetexturenode_get_distortion(this.Ptr);
                }
            case "w":
                /* noisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'w', 'ui_name': 'W'} */
                {
                    return CSycles.noisetexturenode_get_w(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NoiseTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* noisetexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.noisetexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NoiseTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_normalize":
                /* noisetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_normalize', 'ui_name': 'Normalize'} */
                {
                    return CSycles.noisetexturenode_get_use_normalize(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NoiseTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "dimensions":
                /* noisetexturenode . {'datatype': 'ENUM', 'default_value': 'DIM3D', 'default_value_type': 'Dimensions', 'is_input': True, 'member_name': 'dimensions', 'ui_name': 'Dimensions'} */
                {
                    return (uint)CSycles.noisetexturenode_get_dimensions(this.Ptr);
                }
            case "type":
                /* noisetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_NOISE_FBM', 'default_value_type': 'NodeNoiseType', 'is_input': True, 'member_name': 'type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.noisetexturenode_get_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type NoiseTextureNode (getter)");
            }
        }

#endregion
    }

}