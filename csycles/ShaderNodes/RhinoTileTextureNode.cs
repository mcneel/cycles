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

    public class RhinoTileTextureNodeInputs : Inputs
    {
        public FloatSocket Alpha1 { get; private set; }
        public VectorSocket Phase { get; private set; }
        public ColorSocket Color2 { get; private set; }
        public VectorSocket JoinWidth { get; private set; }
        public FloatSocket Alpha2 { get; private set; }
        public VectorSocket UVW { get; private set; }
        public EnumSocket Type { get; private set; }
        public ColorSocket Color1 { get; private set; }

        public RhinoTileTextureNodeInputs(ShaderNode parentNode)
        {
            Alpha1 = new FloatSocket(parentNode, "Alpha1", "alpha1", true);
            AddSocket(Alpha1);
            Phase = new VectorSocket(parentNode, "Phase", "phase", true);
            AddSocket(Phase);
            Color2 = new ColorSocket(parentNode, "Color2", "color2", true);
            AddSocket(Color2);
            JoinWidth = new VectorSocket(parentNode, "JoinWidth", "join_width", true);
            AddSocket(JoinWidth);
            Alpha2 = new FloatSocket(parentNode, "Alpha2", "alpha2", true);
            AddSocket(Alpha2);
            UVW = new VectorSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            Type = new EnumSocket(parentNode, "Type", "tile_type", true);
            AddSocket(Type);
            Color1 = new ColorSocket(parentNode, "Color1", "color1", true);
            AddSocket(Color1);
        }
    }
    public class RhinoTileTextureNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoTileTextureNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "out_alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "rhino_tile_texture")]
    public class RhinoTileTextureNode : ShaderNode
    {
        public enum RhinoTileTextureNodeType : uint {
            Rhi3dRectangular = ccl.RhinoProceduralTileType.RHINO_TILE_3D_RECTANGULAR,
            Rhi2dRectangular = ccl.RhinoProceduralTileType.RHINO_TILE_2D_RECTANGULAR,
            Rhi2dHexagonal = ccl.RhinoProceduralTileType.RHINO_TILE_2D_HEXAGONAL,
            Rhi2dTriangular = ccl.RhinoProceduralTileType.RHINO_TILE_2D_TRIANGULAR,
            Rhi2dOctagonal = ccl.RhinoProceduralTileType.RHINO_TILE_2D_OCTAGONAL,
        }
        public RhinoTileTextureNodeInputs ins => (RhinoTileTextureNodeInputs)inputs;
        public RhinoTileTextureNodeOutputs outs => (RhinoTileTextureNodeOutputs)outputs;
        public RhinoTileTextureNode(Shader shader) : this(shader, "a rhino_tile_texture node") { }

        public RhinoTileTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoTileTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoTileTextureNodeInputs(this);
            outputs = new RhinoTileTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinotiletexturenode_get_uvw(Ptr); }
            set { CSycles.rhinotiletexturenode_set_uvw(Ptr, value); }
        }

        public float3 JoinWidth {
            get { return CSycles.rhinotiletexturenode_get_join_width(Ptr); }
            set { CSycles.rhinotiletexturenode_set_join_width(Ptr, value); }
        }

        public float3 Phase {
            get { return CSycles.rhinotiletexturenode_get_phase(Ptr); }
            set { CSycles.rhinotiletexturenode_set_phase(Ptr, value); }
        }

        public float Alpha1 {
            get { return CSycles.rhinotiletexturenode_get_alpha1(Ptr); }
            set { CSycles.rhinotiletexturenode_set_alpha1(Ptr, value); }
        }

        public float3 Color2 {
            get { return CSycles.rhinotiletexturenode_get_color2(Ptr); }
            set { CSycles.rhinotiletexturenode_set_color2(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinotiletexturenode_get_node_type();
        }

        public RhinoProceduralTileType TileType {
            get { return CSycles.rhinotiletexturenode_get_tile_type(Ptr); }
            set { CSycles.rhinotiletexturenode_set_tile_type(Ptr, value); }
        }

        public float3 Color1 {
            get { return CSycles.rhinotiletexturenode_get_color1(Ptr); }
            set { CSycles.rhinotiletexturenode_set_color1(Ptr, value); }
        }

        public float Alpha2 {
            get { return CSycles.rhinotiletexturenode_get_alpha2(Ptr); }
            set { CSycles.rhinotiletexturenode_set_alpha2(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "alpha1":
                    /* rhinotiletexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                    {
                    CSycles.rhinotiletexturenode_set_alpha1(this.Ptr, data);
                    }
                    break;
            case "alpha2":
                    /* rhinotiletexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                    {
                    CSycles.rhinotiletexturenode_set_alpha2(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTileTextureNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "phase":
                    /* rhinotiletexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'phase', 'ui_name': 'Phase'} */
                    {
                    CSycles.rhinotiletexturenode_set_phase(this.Ptr, data);
                    }
                    break;
            case "join_width":
                    /* rhinotiletexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'join_width', 'ui_name': 'JoinWidth'} */
                    {
                    CSycles.rhinotiletexturenode_set_join_width(this.Ptr, data);
                    }
                    break;
            case "uvw":
                    /* rhinotiletexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinotiletexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTileTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color2":
                    /* rhinotiletexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                    {
                    CSycles.rhinotiletexturenode_set_color2(this.Ptr, data);
                    }
                    break;
            case "color1":
                    /* rhinotiletexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                    {
                    CSycles.rhinotiletexturenode_set_color1(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTileTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "tile_type":
                    /* rhinotiletexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_TILE_3D_RECTANGULAR', 'default_value_type': 'RhinoProceduralTileType', 'is_input': True, 'member_name': 'tile_type', 'ui_name': 'Type'} */
                    {
                    CSycles.rhinotiletexturenode_set_tile_type(this.Ptr, (ccl.RhinoProceduralTileType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTileTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "alpha1":
                /* rhinotiletexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha1', 'ui_name': 'Alpha1'} */
                {
                    return CSycles.rhinotiletexturenode_get_alpha1(this.Ptr);
                }
            case "alpha2":
                /* rhinotiletexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha2', 'ui_name': 'Alpha2'} */
                {
                    return CSycles.rhinotiletexturenode_get_alpha2(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTileTextureNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "phase":
                /* rhinotiletexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'phase', 'ui_name': 'Phase'} */
                {
                    return CSycles.rhinotiletexturenode_get_phase(this.Ptr);
                }
            case "join_width":
                /* rhinotiletexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'join_width', 'ui_name': 'JoinWidth'} */
                {
                    return CSycles.rhinotiletexturenode_get_join_width(this.Ptr);
                }
            case "uvw":
                /* rhinotiletexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinotiletexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTileTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color2":
                /* rhinotiletexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color2', 'ui_name': 'Color2'} */
                {
                    return CSycles.rhinotiletexturenode_get_color2(this.Ptr);
                }
            case "color1":
                /* rhinotiletexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color1', 'ui_name': 'Color1'} */
                {
                    return CSycles.rhinotiletexturenode_get_color1(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTileTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "tile_type":
                /* rhinotiletexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_TILE_3D_RECTANGULAR', 'default_value_type': 'RhinoProceduralTileType', 'is_input': True, 'member_name': 'tile_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.rhinotiletexturenode_get_tile_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTileTextureNode (getter)");
            }
        }

#endregion
    }

}