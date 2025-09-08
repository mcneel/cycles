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

    public class GaborTextureNodeInputs : Inputs
    {
        public EnumSocket Type { get; private set; }
        public PointSocket Vector { get; private set; }
        public FloatSocket Scale { get; private set; }
        public FloatSocket Frequency { get; private set; }
        public FloatSocket Anisotropy { get; private set; }
        public FloatSocket Orientation2D { get; private set; }
        public VectorSocket Orientation3D { get; private set; }

        public GaborTextureNodeInputs(ShaderNode parentNode)
        {
            Type = new EnumSocket(parentNode, "Type", "type", true);
            AddSocket(Type);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Scale = new FloatSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
            Frequency = new FloatSocket(parentNode, "Frequency", "frequency", true);
            AddSocket(Frequency);
            Anisotropy = new FloatSocket(parentNode, "Anisotropy", "anisotropy", true);
            AddSocket(Anisotropy);
            Orientation2D = new FloatSocket(parentNode, "Orientation 2D", "orientation_2d", true);
            AddSocket(Orientation2D);
            Orientation3D = new VectorSocket(parentNode, "Orientation 3D", "orientation_3d", true);
            AddSocket(Orientation3D);
        }
    }
    public class GaborTextureNodeOutputs : Outputs
    {
        public FloatSocket Phase { get; private set; }
        public FloatSocket Value { get; private set; }
        public FloatSocket Intensity { get; private set; }

        public GaborTextureNodeOutputs(ShaderNode parentNode)
        {
            Phase = new FloatSocket(parentNode, "Phase", "phase", false);
            AddSocket(Phase);
            Value = new FloatSocket(parentNode, "Value", "value", false);
            AddSocket(Value);
            Intensity = new FloatSocket(parentNode, "Intensity", "intensity", false);
            AddSocket(Intensity);
        }
    }

    [ShaderNode(name: "gabor_texture")]
    public class GaborTextureNode : TextureNode
    {
        public enum GaborTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum GaborTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum GaborTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public enum GaborTextureNodeType : uint {
            Gab2D = ccl.NodeGaborType.NODE_GABOR_TYPE_2D,
            Gab3D = ccl.NodeGaborType.NODE_GABOR_TYPE_3D,
        }
        public GaborTextureNodeInputs ins => (GaborTextureNodeInputs)inputs;
        public GaborTextureNodeOutputs outs => (GaborTextureNodeOutputs)outputs;
        public GaborTextureNode(Shader shader) : this(shader, "a gabor_texture node") { }

        public GaborTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal GaborTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new GaborTextureNodeInputs(this);
            outputs = new GaborTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.gabortexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "scale":
                    /* gabortexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.gabortexturenode_set_scale(this.Ptr, data);
                    }
                    break;
            case "frequency":
                    /* gabortexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'frequency', 'ui_name': 'Frequency'} */
                    {
                    CSycles.gabortexturenode_set_frequency(this.Ptr, data);
                    }
                    break;
            case "anisotropy":
                    /* gabortexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                    {
                    CSycles.gabortexturenode_set_anisotropy(this.Ptr, data);
                    }
                    break;
            case "orientation_2d":
                    /* gabortexturenode . {'datatype': 'FLOAT', 'default_value': '', 'default_value_type': 'float', 'is_input': True, 'member_name': 'orientation_2d', 'ui_name': 'Orientation 2D'} */
                    {
                    CSycles.gabortexturenode_set_orientation_2d(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GaborTextureNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "orientation_3d":
                    /* gabortexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(1.4142135623730950f,1.4142135623730950f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'orientation_3d', 'ui_name': 'Orientation 3D'} */
                    {
                    CSycles.gabortexturenode_set_orientation_3d(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GaborTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* gabortexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.gabortexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GaborTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "type":
                    /* gabortexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_GABOR_TYPE_2D', 'default_value_type': 'NodeGaborType', 'is_input': True, 'member_name': 'type', 'ui_name': 'Type'} */
                    {
                    CSycles.gabortexturenode_set_type(this.Ptr, (ccl.NodeGaborType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GaborTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "scale":
                /* gabortexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.gabortexturenode_get_scale(this.Ptr);
                }
            case "frequency":
                /* gabortexturenode . {'datatype': 'FLOAT', 'default_value': '2.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'frequency', 'ui_name': 'Frequency'} */
                {
                    return CSycles.gabortexturenode_get_frequency(this.Ptr);
                }
            case "anisotropy":
                /* gabortexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                {
                    return CSycles.gabortexturenode_get_anisotropy(this.Ptr);
                }
            case "orientation_2d":
                /* gabortexturenode . {'datatype': 'FLOAT', 'default_value': '', 'default_value_type': 'float', 'is_input': True, 'member_name': 'orientation_2d', 'ui_name': 'Orientation 2D'} */
                {
                    return CSycles.gabortexturenode_get_orientation_2d(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GaborTextureNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "orientation_3d":
                /* gabortexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(1.4142135623730950f,1.4142135623730950f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'orientation_3d', 'ui_name': 'Orientation 3D'} */
                {
                    return CSycles.gabortexturenode_get_orientation_3d(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GaborTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* gabortexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.gabortexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GaborTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "type":
                /* gabortexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_GABOR_TYPE_2D', 'default_value_type': 'NodeGaborType', 'is_input': True, 'member_name': 'type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.gabortexturenode_get_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GaborTextureNode (getter)");
            }
        }

#endregion
    }

}