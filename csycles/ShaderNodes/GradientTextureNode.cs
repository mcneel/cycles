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

    public class GradientTextureNodeInputs : Inputs
    {
        public EnumSocket Type { get; private set; }
        public PointSocket Vector { get; private set; }

        public GradientTextureNodeInputs(ShaderNode parentNode)
        {
            Type = new EnumSocket(parentNode, "Type", "gradient_type", true);
            AddSocket(Type);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
        }
    }
    public class GradientTextureNodeOutputs : Outputs
    {
        public FloatSocket Fac { get; private set; }
        public ColorSocket Color { get; private set; }

        public GradientTextureNodeOutputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", false);
            AddSocket(Fac);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "gradient_texture")]
    public class GradientTextureNode : TextureNode
    {
        public enum GradientTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum GradientTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum GradientTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public enum GradientTextureNodeType : uint {
            Linear = ccl.NodeGradientType.NODE_BLEND_LINEAR,
            Quadratic = ccl.NodeGradientType.NODE_BLEND_QUADRATIC,
            Easing = ccl.NodeGradientType.NODE_BLEND_EASING,
            Diagonal = ccl.NodeGradientType.NODE_BLEND_DIAGONAL,
            Radial = ccl.NodeGradientType.NODE_BLEND_RADIAL,
            QuadraticSphere = ccl.NodeGradientType.NODE_BLEND_QUADRATIC_SPHERE,
            Spherical = ccl.NodeGradientType.NODE_BLEND_SPHERICAL,
        }
        public GradientTextureNodeInputs ins => (GradientTextureNodeInputs)inputs;
        public GradientTextureNodeOutputs outs => (GradientTextureNodeOutputs)outputs;
        public GradientTextureNode(Shader shader) : this(shader, "a gradient_texture node") { }

        public GradientTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal GradientTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new GradientTextureNodeInputs(this);
            outputs = new GradientTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.gradienttexturenode_get_node_type();
        }
#region Setters

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* gradienttexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.gradienttexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GradientTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "gradient_type":
                    /* gradienttexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_BLEND_LINEAR', 'default_value_type': 'NodeGradientType', 'is_input': True, 'member_name': 'gradient_type', 'ui_name': 'Type'} */
                    {
                    CSycles.gradienttexturenode_set_gradient_type(this.Ptr, (ccl.NodeGradientType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GradientTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* gradienttexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.gradienttexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GradientTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "gradient_type":
                /* gradienttexturenode . {'datatype': 'ENUM', 'default_value': 'NODE_BLEND_LINEAR', 'default_value_type': 'NodeGradientType', 'is_input': True, 'member_name': 'gradient_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.gradienttexturenode_get_gradient_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GradientTextureNode (getter)");
            }
        }

#endregion
    }

}