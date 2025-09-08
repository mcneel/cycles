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

    public class RhinoNormalPart1TextureNodeInputs : Inputs
    {
        public VectorSocket UVW { get; private set; }

        public RhinoNormalPart1TextureNodeInputs(ShaderNode parentNode)
        {
            UVW = new VectorSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
        }
    }
    public class RhinoNormalPart1TextureNodeOutputs : Outputs
    {
        public VectorSocket UVW6 { get; private set; }
        public VectorSocket UVW2 { get; private set; }
        public VectorSocket UVW7 { get; private set; }
        public VectorSocket UVW3 { get; private set; }
        public VectorSocket UVW8 { get; private set; }
        public VectorSocket UVW4 { get; private set; }
        public VectorSocket UVW5 { get; private set; }
        public VectorSocket UVW1 { get; private set; }

        public RhinoNormalPart1TextureNodeOutputs(ShaderNode parentNode)
        {
            UVW6 = new VectorSocket(parentNode, "UVW6", "uvw6_out", false);
            AddSocket(UVW6);
            UVW2 = new VectorSocket(parentNode, "UVW2", "uvw2_out", false);
            AddSocket(UVW2);
            UVW7 = new VectorSocket(parentNode, "UVW7", "uvw7_out", false);
            AddSocket(UVW7);
            UVW3 = new VectorSocket(parentNode, "UVW3", "uvw3_out", false);
            AddSocket(UVW3);
            UVW8 = new VectorSocket(parentNode, "UVW8", "uvw8_out", false);
            AddSocket(UVW8);
            UVW4 = new VectorSocket(parentNode, "UVW4", "uvw4_out", false);
            AddSocket(UVW4);
            UVW5 = new VectorSocket(parentNode, "UVW5", "uvw5_out", false);
            AddSocket(UVW5);
            UVW1 = new VectorSocket(parentNode, "UVW1", "uvw1_out", false);
            AddSocket(UVW1);
        }
    }

    [ShaderNode(name: "rhino_normal_part1_texture")]
    public class RhinoNormalPart1TextureNode : ShaderNode
    {
        public RhinoNormalPart1TextureNodeInputs ins => (RhinoNormalPart1TextureNodeInputs)inputs;
        public RhinoNormalPart1TextureNodeOutputs outs => (RhinoNormalPart1TextureNodeOutputs)outputs;
        public RhinoNormalPart1TextureNode(Shader shader) : this(shader, "a rhino_normal_part1_texture node") { }

        public RhinoNormalPart1TextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoNormalPart1TextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoNormalPart1TextureNodeInputs(this);
            outputs = new RhinoNormalPart1TextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinonormalpart1texturenode_get_uvw(Ptr); }
            set { CSycles.rhinonormalpart1texturenode_set_uvw(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinonormalpart1texturenode_get_node_type();
        }
#region Setters

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinonormalpart1texturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinonormalpart1texturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNormalPart1TextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinonormalpart1texturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinonormalpart1texturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoNormalPart1TextureNode (getter)");
            }
        }

#endregion
    }

}