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

    public class MagicTextureNodeInputs : Inputs
    {
        public IntSocket Depth { get; private set; }
        public PointSocket Vector { get; private set; }
        public FloatSocket Scale { get; private set; }
        public FloatSocket Distortion { get; private set; }

        public MagicTextureNodeInputs(ShaderNode parentNode)
        {
            Depth = new IntSocket(parentNode, "Depth", "depth", true);
            AddSocket(Depth);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Scale = new FloatSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
            Distortion = new FloatSocket(parentNode, "Distortion", "distortion", true);
            AddSocket(Distortion);
        }
    }
    public class MagicTextureNodeOutputs : Outputs
    {
        public FloatSocket Fac { get; private set; }
        public ColorSocket Color { get; private set; }

        public MagicTextureNodeOutputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", false);
            AddSocket(Fac);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "magic_texture")]
    public class MagicTextureNode : TextureNode
    {
        public enum MagicTextureNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum MagicTextureNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum MagicTextureNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public MagicTextureNodeInputs ins => (MagicTextureNodeInputs)inputs;
        public MagicTextureNodeOutputs outs => (MagicTextureNodeOutputs)outputs;
        public MagicTextureNode(Shader shader) : this(shader, "a magic_texture node") { }

        public MagicTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MagicTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MagicTextureNodeInputs(this);
            outputs = new MagicTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.magictexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "scale":
                    /* magictexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.magictexturenode_set_scale(this.Ptr, data);
                    }
                    break;
            case "distortion":
                    /* magictexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distortion', 'ui_name': 'Distortion'} */
                    {
                    CSycles.magictexturenode_set_distortion(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MagicTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* magictexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.magictexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MagicTextureNode (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "depth":
                    /* magictexturenode . {'datatype': 'INT', 'default_value': '2', 'default_value_type': 'int', 'is_input': True, 'member_name': 'depth', 'ui_name': 'Depth'} */
                    {
                    CSycles.magictexturenode_set_depth(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MagicTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "scale":
                /* magictexturenode . {'datatype': 'FLOAT', 'default_value': '5.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.magictexturenode_get_scale(this.Ptr);
                }
            case "distortion":
                /* magictexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distortion', 'ui_name': 'Distortion'} */
                {
                    return CSycles.magictexturenode_get_distortion(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MagicTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* magictexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.magictexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MagicTextureNode (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "depth":
                /* magictexturenode . {'datatype': 'INT', 'default_value': '2', 'default_value_type': 'int', 'is_input': True, 'member_name': 'depth', 'ui_name': 'Depth'} */
                {
                    return CSycles.magictexturenode_get_depth(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MagicTextureNode (getter)");
            }
        }

#endregion
    }

}