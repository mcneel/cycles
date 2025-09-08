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

    public class RhinoPerturbingPart1TextureNodeInputs : Inputs
    {
        public PointSocket UVW { get; private set; }

        public RhinoPerturbingPart1TextureNodeInputs(ShaderNode parentNode)
        {
            UVW = new PointSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
        }
    }
    public class RhinoPerturbingPart1TextureNodeOutputs : Outputs
    {
        public PointSocket UVW3 { get; private set; }
        public PointSocket UVW2 { get; private set; }
        public PointSocket UVW1 { get; private set; }

        public RhinoPerturbingPart1TextureNodeOutputs(ShaderNode parentNode)
        {
            UVW3 = new PointSocket(parentNode, "UVW3", "out_uvw3", false);
            AddSocket(UVW3);
            UVW2 = new PointSocket(parentNode, "UVW2", "out_uvw2", false);
            AddSocket(UVW2);
            UVW1 = new PointSocket(parentNode, "UVW1", "out_uvw1", false);
            AddSocket(UVW1);
        }
    }

    [ShaderNode(name: "rhino_perturbing_part1_texture")]
    public class RhinoPerturbingPart1TextureNode : ShaderNode
    {
        public RhinoPerturbingPart1TextureNodeInputs ins => (RhinoPerturbingPart1TextureNodeInputs)inputs;
        public RhinoPerturbingPart1TextureNodeOutputs outs => (RhinoPerturbingPart1TextureNodeOutputs)outputs;
        public RhinoPerturbingPart1TextureNode(Shader shader) : this(shader, "a rhino_perturbing_part1_texture node") { }

        public RhinoPerturbingPart1TextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoPerturbingPart1TextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoPerturbingPart1TextureNodeInputs(this);
            outputs = new RhinoPerturbingPart1TextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinoperturbingpart1texturenode_get_uvw(Ptr); }
            set { CSycles.rhinoperturbingpart1texturenode_set_uvw(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinoperturbingpart1texturenode_get_node_type();
        }
#region Setters

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinoperturbingpart1texturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinoperturbingpart1texturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerturbingPart1TextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinoperturbingpart1texturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinoperturbingpart1texturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoPerturbingPart1TextureNode (getter)");
            }
        }

#endregion
    }

}