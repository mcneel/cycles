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

    public class RayPortalBsdfNodeInputs : Inputs
    {
        public ColorSocket Color { get; private set; }
        public VectorSocket Direction { get; private set; }
        public VectorSocket Position { get; private set; }

        public RayPortalBsdfNodeInputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Direction = new VectorSocket(parentNode, "Direction", "direction", true);
            AddSocket(Direction);
            Position = new VectorSocket(parentNode, "Position", "position", true);
            AddSocket(Position);
        }
    }
    public class RayPortalBsdfNodeOutputs : Outputs
    {
        public ClosureSocket BSDF { get; private set; }

        public RayPortalBsdfNodeOutputs(ShaderNode parentNode)
        {
            BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF", false);
            AddSocket(BSDF);
        }
    }

    [ShaderNode(name: "ray_portal_bsdf")]
    public class RayPortalBsdfNode : BsdfNode
    {
        public RayPortalBsdfNodeInputs ins => (RayPortalBsdfNodeInputs)inputs;
        public RayPortalBsdfNodeOutputs outs => (RayPortalBsdfNodeOutputs)outputs;
        public RayPortalBsdfNode(Shader shader) : this(shader, "a ray_portal_bsdf node") { }

        public RayPortalBsdfNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RayPortalBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RayPortalBsdfNodeInputs(this);
            outputs = new RayPortalBsdfNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.BSDF;
        }
        public static IntPtr GetNodeType() {
            return CSycles.rayportalbsdfnode_get_node_type();
        }
#region Setters

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "direction":
                    /* rayportalbsdfnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'direction', 'ui_name': 'Direction'} */
                    {
                    CSycles.rayportalbsdfnode_set_direction(this.Ptr, data);
                    }
                    break;
            case "position":
                    /* rayportalbsdfnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'position', 'ui_name': 'Position'} */
                    {
                    CSycles.rayportalbsdfnode_set_position(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RayPortalBsdfNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* rayportalbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.bsdfnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RayPortalBsdfNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "direction":
                /* rayportalbsdfnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'direction', 'ui_name': 'Direction'} */
                {
                    return CSycles.rayportalbsdfnode_get_direction(this.Ptr);
                }
            case "position":
                /* rayportalbsdfnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'position', 'ui_name': 'Position'} */
                {
                    return CSycles.rayportalbsdfnode_get_position(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RayPortalBsdfNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* rayportalbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.bsdfnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RayPortalBsdfNode (getter)");
            }
        }

#endregion
    }

}