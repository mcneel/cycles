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

    public class VoronoiTextureNodeInputs : Inputs
    {
        public FloatSocket W { get; private set; }
        public FloatSocket Scale { get; private set; }
        public FloatSocket Detail { get; private set; }
        public FloatSocket Roughness { get; private set; }
        public FloatSocket Lacunarity { get; private set; }
        public EnumSocket Dimensions { get; private set; }
        public FloatSocket Smoothness { get; private set; }
        public EnumSocket DistanceMetric { get; private set; }
        public FloatSocket Exponent { get; private set; }
        public EnumSocket Feature { get; private set; }
        public FloatSocket Randomness { get; private set; }
        public BoolSocket Normalize { get; private set; }
        public PointSocket Vector { get; private set; }

        public VoronoiTextureNodeInputs(ShaderNode parentNode)
        {
            W = new FloatSocket(parentNode, "W", "w", true);
            AddSocket(W);
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
            Smoothness = new FloatSocket(parentNode, "Smoothness", "smoothness", true);
            AddSocket(Smoothness);
            DistanceMetric = new EnumSocket(parentNode, "Distance Metric", "metric", true);
            AddSocket(DistanceMetric);
            Exponent = new FloatSocket(parentNode, "Exponent", "exponent", true);
            AddSocket(Exponent);
            Feature = new EnumSocket(parentNode, "Feature", "feature", true);
            AddSocket(Feature);
            Randomness = new FloatSocket(parentNode, "Randomness", "randomness", true);
            AddSocket(Randomness);
            Normalize = new BoolSocket(parentNode, "Normalize", "use_normalize", true);
            AddSocket(Normalize);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
        }
    }
    public class VoronoiTextureNodeOutputs : Outputs
    {
        public FloatSocket W { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket Radius { get; private set; }
        public PointSocket Position { get; private set; }
        public FloatSocket Distance { get; private set; }

        public VoronoiTextureNodeOutputs(ShaderNode parentNode)
        {
            W = new FloatSocket(parentNode, "W", "w", false);
            AddSocket(W);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
            Radius = new FloatSocket(parentNode, "Radius", "radius", false);
            AddSocket(Radius);
            Position = new PointSocket(parentNode, "Position", "position", false);
            AddSocket(Position);
            Distance = new FloatSocket(parentNode, "Distance", "distance", false);
            AddSocket(Distance);
        }
    }

    [ShaderNode(name: "voronoi_texture")]
    public class VoronoiTextureNode : TextureNode
    {
        public enum VoronoiTextureNodeDimensions : uint {
            Dim1d = ccl.Dimensions.DIM1D,
            Dim2d = ccl.Dimensions.DIM2D,
            Dim3d = ccl.Dimensions.DIM3D,
            Dim4d = ccl.Dimensions.DIM4D,
        }
        public enum VoronoiTextureNodeFeature : uint {
            F1 = ccl.NodeVoronoiFeature.NODE_VORONOI_F1,
            F2 = ccl.NodeVoronoiFeature.NODE_VORONOI_F2,
            SmoothF1 = ccl.NodeVoronoiFeature.NODE_VORONOI_SMOOTH_F1,
            DistanceToEdge = ccl.NodeVoronoiFeature.NODE_VORONOI_DISTANCE_TO_EDGE,
            NSphereRadius = ccl.NodeVoronoiFeature.NODE_VORONOI_N_SPHERE_RADIUS,
        }
        public enum VoronoiTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum VoronoiTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum VoronoiTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public enum VoronoiTextureNodeMetric : uint {
            Euclidean = ccl.NodeVoronoiDistanceMetric.NODE_VORONOI_EUCLIDEAN,
            Manhattan = ccl.NodeVoronoiDistanceMetric.NODE_VORONOI_MANHATTAN,
            Chebychev = ccl.NodeVoronoiDistanceMetric.NODE_VORONOI_CHEBYCHEV,
            Minkowski = ccl.NodeVoronoiDistanceMetric.NODE_VORONOI_MINKOWSKI,
        }
        public VoronoiTextureNodeInputs ins => (VoronoiTextureNodeInputs)inputs;
        public VoronoiTextureNodeOutputs outs => (VoronoiTextureNodeOutputs)outputs;
        public VoronoiTextureNode(Shader shader) : this(shader, "a voronoi_texture node") { }

        public VoronoiTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal VoronoiTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new VoronoiTextureNodeInputs(this);
            outputs = new VoronoiTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.voronoitexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "w":
                    /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'w', 'ui_name': 'W'} */
                    {
                    CSycles.voronoitexturenode_set_w(this.Ptr, data);
                    }
                    break;
            case "scale":
                    /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.voronoitexturenode_set_scale(this.Ptr, data);
                    }
                    break;
            case "detail":
                    /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail', 'ui_name': 'Detail'} */
                    {
                    CSycles.voronoitexturenode_set_detail(this.Ptr, data);
                    }
                    break;
            case "roughness":
                    /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                    {
                    CSycles.voronoitexturenode_set_roughness(this.Ptr, data);
                    }
                    break;
            case "lacunarity":
                    /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'lacunarity', 'ui_name': 'Lacunarity'} */
                    {
                    CSycles.voronoitexturenode_set_lacunarity(this.Ptr, data);
                    }
                    break;
            case "smoothness":
                    /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'smoothness', 'ui_name': 'Smoothness'} */
                    {
                    CSycles.voronoitexturenode_set_smoothness(this.Ptr, data);
                    }
                    break;
            case "exponent":
                    /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exponent', 'ui_name': 'Exponent'} */
                    {
                    CSycles.voronoitexturenode_set_exponent(this.Ptr, data);
                    }
                    break;
            case "randomness":
                    /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'randomness', 'ui_name': 'Randomness'} */
                    {
                    CSycles.voronoitexturenode_set_randomness(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VoronoiTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* voronoitexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.voronoitexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VoronoiTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_normalize":
                    /* voronoitexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_normalize', 'ui_name': 'Normalize'} */
                    {
                    CSycles.voronoitexturenode_set_use_normalize(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VoronoiTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "dimensions":
                    /* voronoitexturenode . {'datatype': 'ENUM', 'default_value': 'DIM3D', 'default_value_type': 'Dimensions', 'is_input': True, 'member_name': 'dimensions', 'ui_name': 'Dimensions'} */
                    {
                    CSycles.voronoitexturenode_set_dimensions(this.Ptr, (ccl.Dimensions)data);
                    }
                    break;
            case "metric":
                    /* voronoitexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_VORONOI_EUCLIDEAN', 'default_value_type': 'NodeVoronoiDistanceMetric', 'is_input': True, 'member_name': 'metric', 'ui_name': 'Distance Metric'} */
                    {
                    CSycles.voronoitexturenode_set_metric(this.Ptr, (ccl.NodeVoronoiDistanceMetric)data);
                    }
                    break;
            case "feature":
                    /* voronoitexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_VORONOI_F1', 'default_value_type': 'NodeVoronoiFeature', 'is_input': True, 'member_name': 'feature', 'ui_name': 'Feature'} */
                    {
                    CSycles.voronoitexturenode_set_feature(this.Ptr, (ccl.NodeVoronoiFeature)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VoronoiTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "w":
                /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'w', 'ui_name': 'W'} */
                {
                    return CSycles.voronoitexturenode_get_w(this.Ptr);
                }
            case "scale":
                /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.voronoitexturenode_get_scale(this.Ptr);
                }
            case "detail":
                /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'detail', 'ui_name': 'Detail'} */
                {
                    return CSycles.voronoitexturenode_get_detail(this.Ptr);
                }
            case "roughness":
                /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                {
                    return CSycles.voronoitexturenode_get_roughness(this.Ptr);
                }
            case "lacunarity":
                /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'lacunarity', 'ui_name': 'Lacunarity'} */
                {
                    return CSycles.voronoitexturenode_get_lacunarity(this.Ptr);
                }
            case "smoothness":
                /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'smoothness', 'ui_name': 'Smoothness'} */
                {
                    return CSycles.voronoitexturenode_get_smoothness(this.Ptr);
                }
            case "exponent":
                /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exponent', 'ui_name': 'Exponent'} */
                {
                    return CSycles.voronoitexturenode_get_exponent(this.Ptr);
                }
            case "randomness":
                /* voronoitexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'randomness', 'ui_name': 'Randomness'} */
                {
                    return CSycles.voronoitexturenode_get_randomness(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VoronoiTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* voronoitexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.voronoitexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VoronoiTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_normalize":
                /* voronoitexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_normalize', 'ui_name': 'Normalize'} */
                {
                    return CSycles.voronoitexturenode_get_use_normalize(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VoronoiTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "dimensions":
                /* voronoitexturenode . {'datatype': 'ENUM', 'default_value': 'DIM3D', 'default_value_type': 'Dimensions', 'is_input': True, 'member_name': 'dimensions', 'ui_name': 'Dimensions'} */
                {
                    return (uint)CSycles.voronoitexturenode_get_dimensions(this.Ptr);
                }
            case "metric":
                /* voronoitexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_VORONOI_EUCLIDEAN', 'default_value_type': 'NodeVoronoiDistanceMetric', 'is_input': True, 'member_name': 'metric', 'ui_name': 'Distance Metric'} */
                {
                    return (uint)CSycles.voronoitexturenode_get_metric(this.Ptr);
                }
            case "feature":
                /* voronoitexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_VORONOI_F1', 'default_value_type': 'NodeVoronoiFeature', 'is_input': True, 'member_name': 'feature', 'ui_name': 'Feature'} */
                {
                    // return (uint)CSycles.shadernode_get_feature(this.Ptr);
                    return uint.MaxValue;
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VoronoiTextureNode (getter)");
            }
        }

#endregion
    }

}