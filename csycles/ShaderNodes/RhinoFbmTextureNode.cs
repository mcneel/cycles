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

    public class RhinoFbmTextureNodeInputs : Inputs
    {
        public BoolSocket IsTurbulent { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public IntSocket MaxOctaves { get; private set; }
        public FloatSocket Alpha1 { get; private set; }
        public FloatSocket Gain { get; private set; }
        public ColorSocket Color2 { get; private set; }
        public FloatSocket Roughness { get; private set; }
        public FloatSocket Alpha2 { get; private set; }
        public VectorSocket UVW { get; private set; }

        public RhinoFbmTextureNodeInputs(ShaderNode parentNode)
        {
            IsTurbulent = new BoolSocket(parentNode, "IsTurbulent", "is_turbulent", true);
            AddSocket(IsTurbulent);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            MaxOctaves = new IntSocket(parentNode, "MaxOctaves", "max_octaves", true);
            AddSocket(MaxOctaves);
            Alpha1 = new FloatSocket(parentNode, "Alpha1", "alpha1", true);
            AddSocket(Alpha1);
            Gain = new FloatSocket(parentNode, "Gain", "gain", true);
            AddSocket(Gain);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            Roughness = new FloatSocket(parentNode, "Roughness", "roughness", true);
            AddSocket(Roughness);
            Alpha2 = new FloatSocket(parentNode, "Alpha2", "alpha2", true);
            AddSocket(Alpha2);
            UVW = new VectorSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
        }
    }
    public class RhinoFbmTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoFbmTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "out_alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_fbm_texture")]
    public class RhinoFbmTextureNode : ShaderNode
    {
        public RhinoFbmTextureNodeInputs ins => (RhinoFbmTextureNodeInputs)inputs;
        public RhinoFbmTextureNodeOutputs outs => (RhinoFbmTextureNodeOutputs)outputs;
        public RhinoFbmTextureNode(Shader shader) : this(shader, "a rhino_fbm_texture node") { }

        public RhinoFbmTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoFbmTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoFbmTextureNodeInputs(this);
            outputs = new RhinoFbmTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinofbmtexturenode_get_uvw(Ptr); }
            set { CSycles.rhinofbmtexturenode_set_uvw(Ptr, value); }
        }

        public float Alpha1 {
            get { return CSycles.rhinofbmtexturenode_get_alpha1(Ptr); }
            set { CSycles.rhinofbmtexturenode_set_alpha1(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinofbmtexturenode_get_color2(Ptr); }
            set { CSycles.rhinofbmtexturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinofbmtexturenode_get_node_type();
        }

        public float Gain {
            get { return CSycles.rhinofbmtexturenode_get_gain(Ptr); }
            set { CSycles.rhinofbmtexturenode_set_gain(Ptr, value); }
        }

        public float3 Color1 {
            get { return CSycles.rhinofbmtexturenode_get_color1(Ptr); }
            set { CSycles.rhinofbmtexturenode_set_color1(Ptr, value); }
        }

        public float Alpha2 {
            get { return CSycles.rhinofbmtexturenode_get_alpha2(Ptr); }
            set { CSycles.rhinofbmtexturenode_set_alpha2(Ptr, value); }
        }

        public float Roughness {
            get { return CSycles.rhinofbmtexturenode_get_roughness(Ptr); }
            set { CSycles.rhinofbmtexturenode_set_roughness(Ptr, value); }
        }

        public bool IsTurbulent {
            get { return CSycles.rhinofbmtexturenode_get_is_turbulent(Ptr); }
            set { CSycles.rhinofbmtexturenode_set_is_turbulent(Ptr, value); }
        }

        public int MaxOctaves {
            get { return CSycles.rhinofbmtexturenode_get_max_octaves(Ptr); }
            set { CSycles.rhinofbmtexturenode_set_max_octaves(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "alpha1":
                    /* rhinofbmtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                    {
                    CSycles.rhinofbmtexturenode_set_alpha1(this.Ptr, data);
                    }
                    break;
            case "gain":
                    /* rhinofbmtexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gain', 'ui_name': 'Gain'} */
                    {
                    CSycles.rhinofbmtexturenode_set_gain(this.Ptr, data);
                    }
                    break;
            case "roughness":
                    /* rhinofbmtexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                    {
                    CSycles.rhinofbmtexturenode_set_roughness(this.Ptr, data);
                    }
                    break;
            case "alpha2":
                    /* rhinofbmtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                    {
                    CSycles.rhinofbmtexturenode_set_alpha2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinofbmtexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinofbmtexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color1":
                    /* rhinofbmtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinofbmtexturenode_set_color1(this.Ptr, data);
                    }
                    break;
            case "color2":
                    /* rhinofbmtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinofbmtexturenode_set_color2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "is_turbulent":
                    /* rhinofbmtexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_turbulent', 'ui_name': 'IsTurbulent'} */
                    {
                    CSycles.rhinofbmtexturenode_set_is_turbulent(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "max_octaves":
                    /* rhinofbmtexturenode . {'datatype': 'INT', 'default_value': '3', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_octaves', 'ui_name': 'MaxOctaves'} */
                    {
                    CSycles.rhinofbmtexturenode_set_max_octaves(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "alpha1":
                /* rhinofbmtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                {
                    return CSycles.rhinofbmtexturenode_get_alpha1(this.Ptr);
                }
            case "gain":
                /* rhinofbmtexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'gain', 'ui_name': 'Gain'} */
                {
                    return CSycles.rhinofbmtexturenode_get_gain(this.Ptr);
                }
            case "roughness":
                /* rhinofbmtexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                {
                    return CSycles.rhinofbmtexturenode_get_roughness(this.Ptr);
                }
            case "alpha2":
                /* rhinofbmtexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                {
                    return CSycles.rhinofbmtexturenode_get_alpha2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinofbmtexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinofbmtexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color1":
                /* rhinofbmtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinofbmtexturenode_get_color1(this.Ptr);
                }
            case "color2":
                /* rhinofbmtexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinofbmtexturenode_get_color2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "is_turbulent":
                /* rhinofbmtexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_turbulent', 'ui_name': 'IsTurbulent'} */
                {
                    return CSycles.rhinofbmtexturenode_get_is_turbulent(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "max_octaves":
                /* rhinofbmtexturenode . {'datatype': 'INT', 'default_value': '3', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_octaves', 'ui_name': 'MaxOctaves'} */
                {
                    return CSycles.rhinofbmtexturenode_get_max_octaves(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoFbmTextureNode (getter)");
            }
        }

#endregion
    }

}