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

    public class PointDensityTextureNodeInputs : Inputs
    {
        public PointSocket Vector { get; private set; }
        public EnumSocket Interpolation { get; private set; }
        public StringSocket Filename { get; private set; }
        public TransformSocket Transform { get; private set; }
        public EnumSocket Space { get; private set; }

        public PointDensityTextureNodeInputs(ShaderNode parentNode)
        {
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Interpolation = new EnumSocket(parentNode, "Interpolation", "interpolation", true);
            AddSocket(Interpolation);
            Filename = new StringSocket(parentNode, "Filename", "filename", true);
            AddSocket(Filename);
            Transform = new TransformSocket(parentNode, "Transform", "tfm", true);
            AddSocket(Transform);
            Space = new EnumSocket(parentNode, "Space", "space", true);
            AddSocket(Space);
        }
    }
    public class PointDensityTextureNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }
        public FloatSocket Density { get; private set; }

        public PointDensityTextureNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
            Density = new FloatSocket(parentNode, "Density", "density", false);
            AddSocket(Density);
        }
    }

    [ShaderNode(name: "point_density_texture")]
    public class PointDensityTextureNode : ShaderNode
    {
        public enum PointDensityTextureNodeInterpolation : uint {
            Linear = ccl.InterpolationType.INTERPOLATION_LINEAR,
            Closest = ccl.InterpolationType.INTERPOLATION_CLOSEST,
            Cubic = ccl.InterpolationType.INTERPOLATION_CUBIC,
            Smart = ccl.InterpolationType.INTERPOLATION_SMART,
        }
        public enum PointDensityTextureNodeSpace : uint {
            Object = ccl.NodeTexVoxelSpace.NODE_TEX_VOXEL_SPACE_OBJECT,
            World = ccl.NodeTexVoxelSpace.NODE_TEX_VOXEL_SPACE_WORLD,
        }
        public PointDensityTextureNodeInputs ins => (PointDensityTextureNodeInputs)inputs;
        public PointDensityTextureNodeOutputs outs => (PointDensityTextureNodeOutputs)outputs;
        public PointDensityTextureNode(Shader shader) : this(shader, "a point_density_texture node") { }

        public PointDensityTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal PointDensityTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new PointDensityTextureNodeInputs(this);
            outputs = new PointDensityTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.pointdensitytexturenode_get_node_type();
        }
#region Setters

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* pointdensitytexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.pointdensitytexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointDensityTextureNode (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "filename":
                    /* pointdensitytexturenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'filename', 'ui_name': 'Filename'} */
                    {
                    CSycles.pointdensitytexturenode_set_filename(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointDensityTextureNode (setter)");
            }
        }

        internal override void SetTransform(string name, Transform data)
        {
            switch(name) {
            case "tfm":
                    /* pointdensitytexturenode . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'tfm', 'ui_name': 'Transform'} */
                    {
                    CSycles.pointdensitytexturenode_set_tfm(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointDensityTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "interpolation":
                    /* pointdensitytexturenode . {'datatype': 'ENUM', 'default_value': 'INTERPOLATION_LINEAR', 'default_value_type': 'InterpolationType', 'is_input': True, 'member_name': 'interpolation', 'ui_name': 'Interpolation'} */
                    {
                    CSycles.pointdensitytexturenode_set_interpolation(this.Ptr, (ccl.InterpolationType)data);
                    }
                    break;
            case "space":
                    /* pointdensitytexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_TEX_VOXEL_SPACE_OBJECT', 'default_value_type': 'NodeTexVoxelSpace', 'is_input': True, 'member_name': 'space', 'ui_name': 'Space'} */
                    {
                    CSycles.pointdensitytexturenode_set_space(this.Ptr, (ccl.NodeTexVoxelSpace)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointDensityTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* pointdensitytexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.pointdensitytexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointDensityTextureNode (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "filename":
                /* pointdensitytexturenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'filename', 'ui_name': 'Filename'} */
                {
                    return CSycles.pointdensitytexturenode_get_filename(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointDensityTextureNode (getter)");
            }
        }

        internal override Transform GetTransform(string name)
        {
            switch(name) {
            case "tfm":
                /* pointdensitytexturenode . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'tfm', 'ui_name': 'Transform'} */
                {
                    return CSycles.pointdensitytexturenode_get_tfm(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointDensityTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "interpolation":
                /* pointdensitytexturenode . {'datatype': 'ENUM', 'default_value': 'INTERPOLATION_LINEAR', 'default_value_type': 'InterpolationType', 'is_input': True, 'member_name': 'interpolation', 'ui_name': 'Interpolation'} */
                {
                    return (uint)CSycles.pointdensitytexturenode_get_interpolation(this.Ptr);
                }
            case "space":
                /* pointdensitytexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_TEX_VOXEL_SPACE_OBJECT', 'default_value_type': 'NodeTexVoxelSpace', 'is_input': True, 'member_name': 'space', 'ui_name': 'Space'} */
                {
                    return (uint)CSycles.pointdensitytexturenode_get_space(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointDensityTextureNode (getter)");
            }
        }

#endregion
    }

}