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

    public class AttributeNodeInputs : Inputs
    {
        public StringSocket Attribute { get; private set; }

        public AttributeNodeInputs(ShaderNode parentNode)
        {
            Attribute = new StringSocket(parentNode, "Attribute", "attribute", true);
            AddSocket(Attribute);
        }
    }
    public class AttributeNodeOutputs : Outputs
    {
        public FloatSocket Fac { get; private set; }
        public VectorSocket Vector { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket Alpha { get; private set; }

        public AttributeNodeOutputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", false);
            AddSocket(Fac);
            Vector = new VectorSocket(parentNode, "Vector", "vector", false);
            AddSocket(Vector);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", false);
            AddSocket(Alpha);
        }
    }

    [ShaderNode(name: "attribute")]
    public class AttributeNode : ShaderNode
    {
        public AttributeNodeInputs ins => (AttributeNodeInputs)inputs;
        public AttributeNodeOutputs outs => (AttributeNodeOutputs)outputs;
        public AttributeNode(Shader shader) : this(shader, "a attribute node") { }

        public AttributeNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal AttributeNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new AttributeNodeInputs(this);
            outputs = new AttributeNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.attributenode_get_node_type();
        }
#region Setters

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "attribute":
                    /* attributenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'attribute', 'ui_name': 'Attribute'} */
                    {
                    CSycles.attributenode_set_attribute(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AttributeNode (setter)");
            }
        }

#endregion
#region Getters

        internal override string GetString(string name)
        {
            switch(name) {
            case "attribute":
                /* attributenode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'attribute', 'ui_name': 'Attribute'} */
                {
                    return CSycles.attributenode_get_attribute(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AttributeNode (getter)");
            }
        }

#endregion
    }

}