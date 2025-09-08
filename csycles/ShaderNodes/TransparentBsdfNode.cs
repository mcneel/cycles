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

    public class TransparentBsdfNodeInputs : Inputs
    {
        public ColorSocket Color { get; private set; }

        public TransparentBsdfNodeInputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    public class TransparentBsdfNodeOutputs : Outputs
    {
        public ClosureSocket BSDF { get; private set; }

        public TransparentBsdfNodeOutputs(ShaderNode parentNode)
        {
            BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF", false);
            AddSocket(BSDF);
        }
    }

    [ShaderNode(name: "transparent_bsdf")]
    public class TransparentBsdfNode : BsdfNode
    {
        public TransparentBsdfNodeInputs ins => (TransparentBsdfNodeInputs)inputs;
        public TransparentBsdfNodeOutputs outs => (TransparentBsdfNodeOutputs)outputs;
        public TransparentBsdfNode(Shader shader) : this(shader, "a transparent_bsdf node") { }

        public TransparentBsdfNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal TransparentBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new TransparentBsdfNodeInputs(this);
            outputs = new TransparentBsdfNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.BSDF;
        }
        public static IntPtr GetNodeType() {
            return CSycles.transparentbsdfnode_get_node_type();
        }
#region Setters

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* transparentbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.bsdfnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type TransparentBsdfNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* transparentbsdfnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.bsdfnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type TransparentBsdfNode (getter)");
            }
        }

#endregion
    }

}