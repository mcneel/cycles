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

    public class RhinoDotsTextureNodeInputs : Inputs
    {
        public IntSocket DataCount { get; private set; }
        public EnumSocket FalloffType { get; private set; }
        public IntSocket TreeNodeCount { get; private set; }
        public EnumSocket CompositionType { get; private set; }
        public VectorSocket UVW { get; private set; }
        public FloatSocket SampleAreaSize { get; private set; }
        public ColorSocket Color1 { get; private set; }
        public BoolSocket Rings { get; private set; }
        public ColorSocket Color2 { get; private set; }
        public FloatSocket RingRadius { get; private set; }

        public RhinoDotsTextureNodeInputs(ShaderNode parentNode)
        {
            DataCount = new IntSocket(parentNode, "DataCount", "dots_data_count", true);
            AddSocket(DataCount);
            FalloffType = new EnumSocket(parentNode, "FalloffType", "falloff_type", true);
            AddSocket(FalloffType);
            TreeNodeCount = new IntSocket(parentNode, "TreeNodeCount", "dots_tree_node_count", true);
            AddSocket(TreeNodeCount);
            CompositionType = new EnumSocket(parentNode, "CompositionType", "composition_type", true);
            AddSocket(CompositionType);
            UVW = new VectorSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            SampleAreaSize = new FloatSocket(parentNode, "SampleAreaSize", "sample_area_size", true);
            AddSocket(SampleAreaSize);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
            Rings = new BoolSocket(parentNode, "Rings", "rings", true);
            AddSocket(Rings);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            RingRadius = new FloatSocket(parentNode, "RingRadius", "ring_radius", true);
            AddSocket(RingRadius);
        }
    }
    public class RhinoDotsTextureNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public RhinoDotsTextureNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_dots_texture")]
    public class RhinoDotsTextureNode : ShaderNode
    {
        public enum RhinoDotsTextureNodeComposition : uint {
            Maximum = ccl.RhinoProceduralDotsCompositionType.RHINO_DOTS_COMPOSITION_MAXIMUM,
            Addition = ccl.RhinoProceduralDotsCompositionType.RHINO_DOTS_COMPOSITION_ADDITION,
            Subtraction = ccl.RhinoProceduralDotsCompositionType.RHINO_DOTS_COMPOSITION_SUBTRACTION,
            Multiplication = ccl.RhinoProceduralDotsCompositionType.RHINO_DOTS_COMPOSITION_MULTIPLICATION,
            Average = ccl.RhinoProceduralDotsCompositionType.RHINO_DOTS_COMPOSITION_AVERAGE,
            Standard = ccl.RhinoProceduralDotsCompositionType.RHINO_DOTS_COMPOSITION_STANDARD,
        }
        public enum RhinoDotsTextureNodeFalloff : uint {
            Flat = ccl.RhinoProceduralDotsFalloffType.RHINO_DOTS_FALLOFF_FLAT,
            Linear = ccl.RhinoProceduralDotsFalloffType.RHINO_DOTS_FALLOFF_LINEAR,
            Cubic = ccl.RhinoProceduralDotsFalloffType.RHINO_DOTS_FALLOFF_CUBIC,
            Elliptic = ccl.RhinoProceduralDotsFalloffType.RHINO_DOTS_FALLOFF_ELLIPTIC,
        }
        public RhinoDotsTextureNodeInputs ins => (RhinoDotsTextureNodeInputs)inputs;
        public RhinoDotsTextureNodeOutputs outs => (RhinoDotsTextureNodeOutputs)outputs;
        public RhinoDotsTextureNode(Shader shader) : this(shader, "a rhino_dots_texture node") { }

        public RhinoDotsTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoDotsTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoDotsTextureNodeInputs(this);
            outputs = new RhinoDotsTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinodotstexturenode_get_uvw(Ptr); }
            set { CSycles.rhinodotstexturenode_set_uvw(Ptr, value); }
        }

        public bool Rings {
            get { return CSycles.rhinodotstexturenode_get_rings(Ptr); }
            set { CSycles.rhinodotstexturenode_set_rings(Ptr, value); }
        }

        public float RingRadius {
            get { return CSycles.rhinodotstexturenode_get_ring_radius(Ptr); }
            set { CSycles.rhinodotstexturenode_set_ring_radius(Ptr, value); }
        }

        public RhinoProceduralDotsCompositionType CompositionType {
            get { return CSycles.rhinodotstexturenode_get_composition_type(Ptr); }
            set { CSycles.rhinodotstexturenode_set_composition_type(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinodotstexturenode_get_color2(Ptr); }
            set { CSycles.rhinodotstexturenode_set_color2(Ptr, value); }
        }

        public float SampleAreaSize {
            get { return CSycles.rhinodotstexturenode_get_sample_area_size(Ptr); }
            set { CSycles.rhinodotstexturenode_set_sample_area_size(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinodotstexturenode_get_node_type();
        }

        public int DotsDataCount {
            get { return CSycles.rhinodotstexturenode_get_dots_data_count(Ptr); }
            set { CSycles.rhinodotstexturenode_set_dots_data_count(Ptr, value); }
        }

        public float3 Color1 {
            get { return CSycles.rhinodotstexturenode_get_color1(Ptr); }
            set { CSycles.rhinodotstexturenode_set_color1(Ptr, value); }
        }

        public RhinoProceduralDotsFalloffType FalloffType {
            get { return CSycles.rhinodotstexturenode_get_falloff_type(Ptr); }
            set { CSycles.rhinodotstexturenode_set_falloff_type(Ptr, value); }
        }

        public int DotsTreeNodeCount {
            get { return CSycles.rhinodotstexturenode_get_dots_tree_node_count(Ptr); }
            set { CSycles.rhinodotstexturenode_set_dots_tree_node_count(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "sample_area_size":
                    /* rhinodotstexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_area_size', 'ui_name': 'SampleAreaSize'} */
                    {
                    CSycles.rhinodotstexturenode_set_sample_area_size(this.Ptr, data);
                    }
                    break;
            case "ring_radius":
                    /* rhinodotstexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ring_radius', 'ui_name': 'RingRadius'} */
                    {
                    CSycles.rhinodotstexturenode_set_ring_radius(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinodotstexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinodotstexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color1":
                    /* rhinodotstexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinodotstexturenode_set_color1(this.Ptr, data);
                    }
                    break;
            case "color2":
                    /* rhinodotstexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinodotstexturenode_set_color2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "rings":
                    /* rhinodotstexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'rings', 'ui_name': 'Rings'} */
                    {
                    CSycles.rhinodotstexturenode_set_rings(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "dots_data_count":
                    /* rhinodotstexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'dots_data_count', 'ui_name': 'DataCount'} */
                    {
                    CSycles.rhinodotstexturenode_set_dots_data_count(this.Ptr, data);
                    }
                    break;
            case "dots_tree_node_count":
                    /* rhinodotstexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'dots_tree_node_count', 'ui_name': 'TreeNodeCount'} */
                    {
                    CSycles.rhinodotstexturenode_set_dots_tree_node_count(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "falloff_type":
                    /* rhinodotstexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_DOTS_FALLOFF_FLAT', 'default_value_type': 'RhinoProceduralDotsFalloffType', 'is_input': True, 'member_name': 'falloff_type', 'ui_name': 'FalloffType'} */
                    {
                    CSycles.rhinodotstexturenode_set_falloff_type(this.Ptr, (ccl.RhinoProceduralDotsFalloffType)data);
                    }
                    break;
            case "composition_type":
                    /* rhinodotstexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_DOTS_COMPOSITION_MAXIMUM', 'default_value_type': 'RhinoProceduralDotsCompositionType', 'is_input': True, 'member_name': 'composition_type', 'ui_name': 'CompositionType'} */
                    {
                    CSycles.rhinodotstexturenode_set_composition_type(this.Ptr, (ccl.RhinoProceduralDotsCompositionType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "sample_area_size":
                /* rhinodotstexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_area_size', 'ui_name': 'SampleAreaSize'} */
                {
                    return CSycles.rhinodotstexturenode_get_sample_area_size(this.Ptr);
                }
            case "ring_radius":
                /* rhinodotstexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ring_radius', 'ui_name': 'RingRadius'} */
                {
                    return CSycles.rhinodotstexturenode_get_ring_radius(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinodotstexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinodotstexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color1":
                /* rhinodotstexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinodotstexturenode_get_color1(this.Ptr);
                }
            case "color2":
                /* rhinodotstexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinodotstexturenode_get_color2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "rings":
                /* rhinodotstexturenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'rings', 'ui_name': 'Rings'} */
                {
                    return CSycles.rhinodotstexturenode_get_rings(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "dots_data_count":
                /* rhinodotstexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'dots_data_count', 'ui_name': 'DataCount'} */
                {
                    return CSycles.rhinodotstexturenode_get_dots_data_count(this.Ptr);
                }
            case "dots_tree_node_count":
                /* rhinodotstexturenode . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'dots_tree_node_count', 'ui_name': 'TreeNodeCount'} */
                {
                    return CSycles.rhinodotstexturenode_get_dots_tree_node_count(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "falloff_type":
                /* rhinodotstexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_DOTS_FALLOFF_FLAT', 'default_value_type': 'RhinoProceduralDotsFalloffType', 'is_input': True, 'member_name': 'falloff_type', 'ui_name': 'FalloffType'} */
                {
                    return (uint)CSycles.rhinodotstexturenode_get_falloff_type(this.Ptr);
                }
            case "composition_type":
                /* rhinodotstexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_DOTS_COMPOSITION_MAXIMUM', 'default_value_type': 'RhinoProceduralDotsCompositionType', 'is_input': True, 'member_name': 'composition_type', 'ui_name': 'CompositionType'} */
                {
                    return (uint)CSycles.rhinodotstexturenode_get_composition_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoDotsTextureNode (getter)");
            }
        }

#endregion
    }

}