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

    public class VertexColorNodeInputs : Inputs
    {
        public StringSocket LayerName { get; private set; }

        public VertexColorNodeInputs(ShaderNode parentNode)
        {
            LayerName = new StringSocket(parentNode, "Layer Name", "layer_name", true);
            AddSocket(LayerName);
        }
    }
    public class VertexColorNodeOutputs : Outputs
    {
        public FloatSocket Alpha { get; private set; }
        public ColorSocket Color { get; private set; }

        public VertexColorNodeOutputs(ShaderNode parentNode)
        {
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", false);
            AddSocket(Alpha);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "vertex_color")]
    public class VertexColorNode : ShaderNode
    {
        public VertexColorNodeInputs ins => (VertexColorNodeInputs)inputs;
        public VertexColorNodeOutputs outs => (VertexColorNodeOutputs)outputs;
        public VertexColorNode(Shader shader) : this(shader, "a vertex_color node") { }

        public VertexColorNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal VertexColorNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new VertexColorNodeInputs(this);
            outputs = new VertexColorNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.vertexcolornode_get_node_type();
        }
#region Setters

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "layer_name":
                    /* vertexcolornode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'layer_name', 'ui_name': 'Layer Name'} */
                    {
                    CSycles.vertexcolornode_set_layer_name(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VertexColorNode (setter)");
            }
        }

#endregion
#region Getters

        internal override string GetString(string name)
        {
            switch(name) {
            case "layer_name":
                /* vertexcolornode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'layer_name', 'ui_name': 'Layer Name'} */
                {
                    return CSycles.vertexcolornode_get_layer_name(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type VertexColorNode (getter)");
            }
        }

#endregion
    }

}