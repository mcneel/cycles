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

    public class IESLightNodeInputs : Inputs
    {
        public PointSocket Vector { get; private set; }
        public StringSocket IES { get; private set; }
        public StringSocket FileName { get; private set; }
        public FloatSocket Strength { get; private set; }

        public IESLightNodeInputs(ShaderNode parentNode)
        {
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            IES = new StringSocket(parentNode, "IES", "ies", true);
            AddSocket(IES);
            FileName = new StringSocket(parentNode, "File Name", "filename", true);
            AddSocket(FileName);
            Strength = new FloatSocket(parentNode, "Strength", "strength", true);
            AddSocket(Strength);
        }
    }
    public class IESLightNodeOutputs : Outputs
    {
        public FloatSocket Fac { get; private set; }

        public IESLightNodeOutputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", false);
            AddSocket(Fac);
        }
    }

    [ShaderNode(name: "ies_light")]
    public class IESLightNode : TextureNode
    {
        public enum IESLightNodeMappingAxis : uint {
            None = ccl.TextureMapping_Mapping.NONE,
            X = ccl.TextureMapping_Mapping.X,
            Y = ccl.TextureMapping_Mapping.Y,
            Z = ccl.TextureMapping_Mapping.Z,
        }
        public enum IESLightNodeMappingProjection : uint {
            Flat = ccl.TextureMapping_Projection.FLAT,
            Cube = ccl.TextureMapping_Projection.CUBE,
            Tube = ccl.TextureMapping_Projection.TUBE,
            Sphere = ccl.TextureMapping_Projection.SPHERE,
        }
        public enum IESLightNodeMappingType : uint {
            Point = ccl.TextureMapping_Type.POINT,
            Texture = ccl.TextureMapping_Type.TEXTURE,
            Vector = ccl.TextureMapping_Type.VECTOR,
            Normal = ccl.TextureMapping_Type.NORMAL,
        }
        public IESLightNodeInputs ins => (IESLightNodeInputs)inputs;
        public IESLightNodeOutputs outs => (IESLightNodeOutputs)outputs;
        public IESLightNode(Shader shader) : this(shader, "a ies_light node") { }

        public IESLightNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal IESLightNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new IESLightNodeInputs(this);
            outputs = new IESLightNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.ieslightnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "strength":
                    /* ieslightnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                    {
                    CSycles.ieslightnode_set_strength(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type IESLightNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* ieslightnode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.ieslightnode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type IESLightNode (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "ies":
                    /* ieslightnode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'ies', 'ui_name': 'IES'} */
                    {
                    CSycles.ieslightnode_set_ies(this.Ptr, data);
                    }
                    break;
            case "filename":
                    /* ieslightnode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'filename', 'ui_name': 'File Name'} */
                    {
                    CSycles.ieslightnode_set_filename(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type IESLightNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "strength":
                /* ieslightnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                {
                    return CSycles.ieslightnode_get_strength(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type IESLightNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* ieslightnode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.ieslightnode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type IESLightNode (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "ies":
                /* ieslightnode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'ies', 'ui_name': 'IES'} */
                {
                    return CSycles.ieslightnode_get_ies(this.Ptr);
                }
            case "filename":
                /* ieslightnode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'filename', 'ui_name': 'File Name'} */
                {
                    return CSycles.ieslightnode_get_filename(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type IESLightNode (getter)");
            }
        }

#endregion
    }

}