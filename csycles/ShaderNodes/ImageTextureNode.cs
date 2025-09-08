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

    public class ImageTextureNodeInputs : Inputs
    {
        public EnumSocket Interpolation { get; private set; }
        public EnumSocket Extension { get; private set; }
        public EnumSocket Projection { get; private set; }
        public FloatSocket ProjectionBlend { get; private set; }
        public IntArraySocket Tiles { get; private set; }
        public BoolSocket Animated { get; private set; }
        public StringSocket Filename { get; private set; }
        public PointSocket Vector { get; private set; }
        public StringSocket Colorspace { get; private set; }
        public FloatSocket DecalForward { get; private set; }
        public BoolSocket AlternateTiles { get; private set; }
        public FloatSocket DecalUsage { get; private set; }
        public EnumSocket AlphaType { get; private set; }

        public ImageTextureNodeInputs(ShaderNode parentNode)
        {
            Interpolation = new EnumSocket(parentNode, "Interpolation", "interpolation", true);
            AddSocket(Interpolation);
            Extension = new EnumSocket(parentNode, "Extension", "extension", true);
            AddSocket(Extension);
            Projection = new EnumSocket(parentNode, "Projection", "projection", true);
            AddSocket(Projection);
            ProjectionBlend = new FloatSocket(parentNode, "Projection Blend", "projection_blend", true);
            AddSocket(ProjectionBlend);
            Tiles = new IntArraySocket(parentNode, "Tiles", "tiles", true);
            AddSocket(Tiles);
            Animated = new BoolSocket(parentNode, "Animated", "animated", true);
            AddSocket(Animated);
            Filename = new StringSocket(parentNode, "Filename", "filename", true);
            AddSocket(Filename);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Colorspace = new StringSocket(parentNode, "Colorspace", "colorspace", true);
            AddSocket(Colorspace);
            DecalForward = new FloatSocket(parentNode, "DecalForward", "decalforward", true);
            AddSocket(DecalForward);
            AlternateTiles = new BoolSocket(parentNode, "Alternate Tiles", "alternate_tiles", true);
            AddSocket(AlternateTiles);
            DecalUsage = new FloatSocket(parentNode, "DecalUsage", "decalusage", true);
            AddSocket(DecalUsage);
            AlphaType = new EnumSocket(parentNode, "Alpha Type", "alpha_type", true);
            AddSocket(AlphaType);
        }
    }
    public class ImageTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public ImageTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "image_texture")]
    public class ImageTextureNode : ImageSlotTextureNode
    {
        public enum ImageTextureNodeAlphaType : uint {
            Unassociated = ccl.ImageAlphaType.IMAGE_ALPHA_UNASSOCIATED,
            Associated = ccl.ImageAlphaType.IMAGE_ALPHA_ASSOCIATED,
            ChannelPacked = ccl.ImageAlphaType.IMAGE_ALPHA_CHANNEL_PACKED,
            Ignore = ccl.ImageAlphaType.IMAGE_ALPHA_IGNORE,
            Auto = ccl.ImageAlphaType.IMAGE_ALPHA_AUTO,
        }
        public enum ImageTextureNodeExtension : uint {
            Periodic = ccl.ExtensionType.EXTENSION_REPEAT,
            Clamp = ccl.ExtensionType.EXTENSION_EXTEND,
            Black = ccl.ExtensionType.EXTENSION_CLIP,
            Mirror = ccl.ExtensionType.EXTENSION_MIRROR,
        }
        public enum ImageTextureNodeInterpolation : uint {
            Linear = ccl.InterpolationType.INTERPOLATION_LINEAR,
            Closest = ccl.InterpolationType.INTERPOLATION_CLOSEST,
            Cubic = ccl.InterpolationType.INTERPOLATION_CUBIC,
            Smart = ccl.InterpolationType.INTERPOLATION_SMART,
        }
        public enum ImageTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum ImageTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum ImageTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public enum ImageTextureNodeProjection : uint {
            Flat = ccl.NodeImageProjection.NODE_IMAGE_PROJ_FLAT,
            Box = ccl.NodeImageProjection.NODE_IMAGE_PROJ_BOX,
            Sphere = ccl.NodeImageProjection.NODE_IMAGE_PROJ_SPHERE,
            Tube = ccl.NodeImageProjection.NODE_IMAGE_PROJ_TUBE,
        }
        public ImageTextureNodeInputs ins => (ImageTextureNodeInputs)inputs;
        public ImageTextureNodeOutputs outs => (ImageTextureNodeOutputs)outputs;
        public ImageTextureNode(Shader shader) : this(shader, "a image_texture node") { }

        public ImageTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal ImageTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new ImageTextureNodeInputs(this);
            outputs = new ImageTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.imagetexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "projection_blend":
                    /* imagetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'projection_blend', 'ui_name': 'Projection Blend'} */
                    {
                    CSycles.imagetexturenode_set_projection_blend(this.Ptr, data);
                    }
                    break;
            case "decalforward":
                    /* imagetexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'decalforward', 'ui_name': 'DecalForward'} */
                    {
                    CSycles.imagetexturenode_set_decalforward(this.Ptr, data);
                    }
                    break;
            case "decalusage":
                    /* imagetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'decalusage', 'ui_name': 'DecalUsage'} */
                    {
                    CSycles.imagetexturenode_set_decalusage(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* imagetexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.imagetexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "animated":
                    /* imagetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'animated', 'ui_name': 'Animated'} */
                    {
                    CSycles.imagetexturenode_set_animated(this.Ptr, data);
                    }
                    break;
            case "alternate_tiles":
                    /* imagetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'alternate_tiles', 'ui_name': 'Alternate Tiles'} */
                    {
                    CSycles.imagetexturenode_set_alternate_tiles(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "filename":
                    /* imagetexturenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'filename', 'ui_name': 'Filename'} */
                    {
                    CSycles.imagetexturenode_set_filename(this.Ptr, data);
                    }
                    break;
            case "colorspace":
                    /* imagetexturenode . {'datatype': 'STRING', 'default_value': 'u_colorspace_auto', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'colorspace', 'ui_name': 'Colorspace'} */
                    {
                    CSycles.imagetexturenode_set_colorspace(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "interpolation":
                    /* imagetexturenode . {'datatype': 'ENUM', 'default_value': 'INTERPOLATION_LINEAR', 'default_value_type': 'InterpolationType', 'is_input': True, 'member_name': 'interpolation', 'ui_name': 'Interpolation'} */
                    {
                    CSycles.imagetexturenode_set_interpolation(this.Ptr, (ccl.InterpolationType)data);
                    }
                    break;
            case "extension":
                    /* imagetexturenode . {'datatype': 'ENUM', 'default_value': 'EXTENSION_REPEAT', 'default_value_type': 'ExtensionType', 'is_input': True, 'member_name': 'extension', 'ui_name': 'Extension'} */
                    {
                    CSycles.imagetexturenode_set_extension(this.Ptr, (ccl.ExtensionType)data);
                    }
                    break;
            case "projection":
                    /* imagetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_IMAGE_PROJ_FLAT', 'default_value_type': 'NodeImageProjection', 'is_input': True, 'member_name': 'projection', 'ui_name': 'Projection'} */
                    {
                    CSycles.imagetexturenode_set_projection(this.Ptr, (ccl.NodeImageProjection)data);
                    }
                    break;
            case "alpha_type":
                    /* imagetexturenode . {'datatype': 'ENUM', 'default_value': 'IMAGE_ALPHA_AUTO', 'default_value_type': 'ImageAlphaType', 'is_input': True, 'member_name': 'alpha_type', 'ui_name': 'Alpha Type'} */
                    {
                    CSycles.imagetexturenode_set_alpha_type(this.Ptr, (ccl.ImageAlphaType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (setter)");
            }
        }

        internal override void SetIntArray(string name, List<int> data)
        {
            switch(name) {
            case "tiles":
                    /* imagetexturenode . {'datatype': 'INT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'tiles', 'ui_name': 'Tiles'} */
                    {
                    CSycles.imagetexturenode_set_tiles(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "projection_blend":
                /* imagetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'projection_blend', 'ui_name': 'Projection Blend'} */
                {
                    return CSycles.imagetexturenode_get_projection_blend(this.Ptr);
                }
            case "decalforward":
                /* imagetexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'decalforward', 'ui_name': 'DecalForward'} */
                {
                    return CSycles.imagetexturenode_get_decalforward(this.Ptr);
                }
            case "decalusage":
                /* imagetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'decalusage', 'ui_name': 'DecalUsage'} */
                {
                    return CSycles.imagetexturenode_get_decalusage(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* imagetexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.imagetexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "animated":
                /* imagetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'animated', 'ui_name': 'Animated'} */
                {
                    return CSycles.imagetexturenode_get_animated(this.Ptr);
                }
            case "alternate_tiles":
                /* imagetexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'alternate_tiles', 'ui_name': 'Alternate Tiles'} */
                {
                    return CSycles.imagetexturenode_get_alternate_tiles(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "filename":
                /* imagetexturenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'filename', 'ui_name': 'Filename'} */
                {
                    return CSycles.imagetexturenode_get_filename(this.Ptr);
                }
            case "colorspace":
                /* imagetexturenode . {'datatype': 'STRING', 'default_value': 'u_colorspace_auto', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'colorspace', 'ui_name': 'Colorspace'} */
                {
                    return CSycles.imagetexturenode_get_colorspace(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "interpolation":
                /* imagetexturenode . {'datatype': 'ENUM', 'default_value': 'INTERPOLATION_LINEAR', 'default_value_type': 'InterpolationType', 'is_input': True, 'member_name': 'interpolation', 'ui_name': 'Interpolation'} */
                {
                    return (uint)CSycles.imagetexturenode_get_interpolation(this.Ptr);
                }
            case "extension":
                /* imagetexturenode . {'datatype': 'ENUM', 'default_value': 'EXTENSION_REPEAT', 'default_value_type': 'ExtensionType', 'is_input': True, 'member_name': 'extension', 'ui_name': 'Extension'} */
                {
                    return (uint)CSycles.imagetexturenode_get_extension(this.Ptr);
                }
            case "projection":
                /* imagetexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_IMAGE_PROJ_FLAT', 'default_value_type': 'NodeImageProjection', 'is_input': True, 'member_name': 'projection', 'ui_name': 'Projection'} */
                {
                    return (uint)CSycles.imagetexturenode_get_projection(this.Ptr);
                }
            case "alpha_type":
                /* imagetexturenode . {'datatype': 'ENUM', 'default_value': 'IMAGE_ALPHA_AUTO', 'default_value_type': 'ImageAlphaType', 'is_input': True, 'member_name': 'alpha_type', 'ui_name': 'Alpha Type'} */
                {
                    return (uint)CSycles.imagetexturenode_get_alpha_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (getter)");
            }
        }

        internal override List<int> GetIntArray(string name)
        {
            switch(name) {
            case "tiles":
                /* imagetexturenode . {'datatype': 'INT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'tiles', 'ui_name': 'Tiles'} */
                {
                    return CSycles.imagetexturenode_get_tiles(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ImageTextureNode (getter)");
            }
        }

#endregion
    }

}