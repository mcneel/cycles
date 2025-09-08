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

    public class EnvironmentTextureNodeInputs : Inputs
    {
        public EnumSocket Interpolation { get; private set; }
        public EnumSocket Projection { get; private set; }
        public BoolSocket Animated { get; private set; }
        public PointSocket Vector { get; private set; }
        public StringSocket Filename { get; private set; }
        public StringSocket Colorspace { get; private set; }
        public EnumSocket AlphaType { get; private set; }

        public EnvironmentTextureNodeInputs(ShaderNode parentNode)
        {
            Interpolation = new EnumSocket(parentNode, "Interpolation", "interpolation", true);
            AddSocket(Interpolation);
            Projection = new EnumSocket(parentNode, "Projection", "projection", true);
            AddSocket(Projection);
            Animated = new BoolSocket(parentNode, "Animated", "animated", true);
            AddSocket(Animated);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Filename = new StringSocket(parentNode, "Filename", "filename", true);
            AddSocket(Filename);
            Colorspace = new StringSocket(parentNode, "Colorspace", "colorspace", true);
            AddSocket(Colorspace);
            AlphaType = new EnumSocket(parentNode, "Alpha Type", "alpha_type", true);
            AddSocket(AlphaType);
        }
    }
    public class EnvironmentTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public EnvironmentTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "environment_texture")]
    public class EnvironmentTextureNode : ImageSlotTextureNode
    {
        public enum EnvironmentTextureNodeAlphaType : uint {
            Unassociated = ccl.ImageAlphaType.IMAGE_ALPHA_UNASSOCIATED,
            Associated = ccl.ImageAlphaType.IMAGE_ALPHA_ASSOCIATED,
            ChannelPacked = ccl.ImageAlphaType.IMAGE_ALPHA_CHANNEL_PACKED,
            Ignore = ccl.ImageAlphaType.IMAGE_ALPHA_IGNORE,
            Auto = ccl.ImageAlphaType.IMAGE_ALPHA_AUTO,
        }
        public enum EnvironmentTextureNodeInterpolation : uint {
            Linear = ccl.InterpolationType.INTERPOLATION_LINEAR,
            Closest = ccl.InterpolationType.INTERPOLATION_CLOSEST,
            Cubic = ccl.InterpolationType.INTERPOLATION_CUBIC,
            Smart = ccl.InterpolationType.INTERPOLATION_SMART,
        }
        public enum EnvironmentTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum EnvironmentTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum EnvironmentTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public enum EnvironmentTextureNodeProjection : uint {
            Equirectangular = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_EQUIRECTANGULAR,
            MirrorBall = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_MIRROR_BALL,
            Wallpaper = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_WALLPAPER,
            Emap = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_EMAP,
            Box = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_BOX,
            LightProbe = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_LIGHT_PROBE,
            Cubemap = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_CUBEMAP,
            CubemapHorizontal = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_CUBEMAP_HORIZONTAL,
            CubemapVertical = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_CUBEMAP_VERTICAL,
            Hemispherical = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_HEMISPHERICAL,
            Spherical = ccl.NodeEnvironmentProjection.NODE_ENVIRONMENT_SPHERICAL,
        }
        public EnvironmentTextureNodeInputs ins => (EnvironmentTextureNodeInputs)inputs;
        public EnvironmentTextureNodeOutputs outs => (EnvironmentTextureNodeOutputs)outputs;
        public EnvironmentTextureNode(Shader shader) : this(shader, "a environment_texture node") { }

        public EnvironmentTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal EnvironmentTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new EnvironmentTextureNodeInputs(this);
            outputs = new EnvironmentTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.environmenttexturenode_get_node_type();
        }
#region Setters

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* environmenttexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.environmenttexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type EnvironmentTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "animated":
                    /* environmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'animated', 'ui_name': 'Animated'} */
                    {
                    CSycles.environmenttexturenode_set_animated(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type EnvironmentTextureNode (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "filename":
                    /* environmenttexturenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'filename', 'ui_name': 'Filename'} */
                    {
                    CSycles.environmenttexturenode_set_filename(this.Ptr, data);
                    }
                    break;
            case "colorspace":
                    /* environmenttexturenode . {'datatype': 'STRING', 'default_value': 'u_colorspace_auto', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'colorspace', 'ui_name': 'Colorspace'} */
                    {
                    CSycles.environmenttexturenode_set_colorspace(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type EnvironmentTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "interpolation":
                    /* environmenttexturenode . {'datatype': 'ENUM', 'default_value': 'INTERPOLATION_LINEAR', 'default_value_type': 'InterpolationType', 'is_input': True, 'member_name': 'interpolation', 'ui_name': 'Interpolation'} */
                    {
                    CSycles.environmenttexturenode_set_interpolation(this.Ptr, (ccl.InterpolationType)data);
                    }
                    break;
            case "projection":
                    /* environmenttexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_ENVIRONMENT_EQUIRECTANGULAR', 'default_value_type': 'NodeEnvironmentProjection', 'is_input': True, 'member_name': 'projection', 'ui_name': 'Projection'} */
                    {
                    CSycles.environmenttexturenode_set_projection(this.Ptr, (ccl.NodeEnvironmentProjection)data);
                    }
                    break;
            case "alpha_type":
                    /* environmenttexturenode . {'datatype': 'ENUM', 'default_value': 'IMAGE_ALPHA_AUTO', 'default_value_type': 'ImageAlphaType', 'is_input': True, 'member_name': 'alpha_type', 'ui_name': 'Alpha Type'} */
                    {
                    CSycles.environmenttexturenode_set_alpha_type(this.Ptr, (ccl.ImageAlphaType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type EnvironmentTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* environmenttexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.environmenttexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type EnvironmentTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "animated":
                /* environmenttexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'animated', 'ui_name': 'Animated'} */
                {
                    return CSycles.environmenttexturenode_get_animated(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type EnvironmentTextureNode (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "filename":
                /* environmenttexturenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'filename', 'ui_name': 'Filename'} */
                {
                    return CSycles.environmenttexturenode_get_filename(this.Ptr);
                }
            case "colorspace":
                /* environmenttexturenode . {'datatype': 'STRING', 'default_value': 'u_colorspace_auto', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'colorspace', 'ui_name': 'Colorspace'} */
                {
                    return CSycles.environmenttexturenode_get_colorspace(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type EnvironmentTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "interpolation":
                /* environmenttexturenode . {'datatype': 'ENUM', 'default_value': 'INTERPOLATION_LINEAR', 'default_value_type': 'InterpolationType', 'is_input': True, 'member_name': 'interpolation', 'ui_name': 'Interpolation'} */
                {
                    return (uint)CSycles.environmenttexturenode_get_interpolation(this.Ptr);
                }
            case "projection":
                /* environmenttexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_ENVIRONMENT_EQUIRECTANGULAR', 'default_value_type': 'NodeEnvironmentProjection', 'is_input': True, 'member_name': 'projection', 'ui_name': 'Projection'} */
                {
                    return (uint)CSycles.environmenttexturenode_get_projection(this.Ptr);
                }
            case "alpha_type":
                /* environmenttexturenode . {'datatype': 'ENUM', 'default_value': 'IMAGE_ALPHA_AUTO', 'default_value_type': 'ImageAlphaType', 'is_input': True, 'member_name': 'alpha_type', 'ui_name': 'Alpha Type'} */
                {
                    return (uint)CSycles.environmenttexturenode_get_alpha_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type EnvironmentTextureNode (getter)");
            }
        }

#endregion
    }

}