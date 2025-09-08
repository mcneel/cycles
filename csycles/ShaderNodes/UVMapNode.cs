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

    public class UVMapNodeInputs : Inputs
    {
        public BoolSocket fromdupli { get; private set; }
        public StringSocket attribute { get; private set; }

        public UVMapNodeInputs(ShaderNode parentNode)
        {
            fromdupli = new BoolSocket(parentNode, "from dupli", "from_dupli", true);
            AddSocket(fromdupli);
            attribute = new StringSocket(parentNode, "attribute", "attribute", true);
            AddSocket(attribute);
        }
    }
    public class UVMapNodeOutputs : Outputs
    {
        public PointSocket UV { get; private set; }

        public UVMapNodeOutputs(ShaderNode parentNode)
        {
            UV = new PointSocket(parentNode, "UV", "UV", false);
            AddSocket(UV);
        }
    }

    [ShaderNode(name: "uvmap")]
    public class UVMapNode : ShaderNode
    {
        public UVMapNodeInputs ins => (UVMapNodeInputs)inputs;
        public UVMapNodeOutputs outs => (UVMapNodeOutputs)outputs;
        public UVMapNode(Shader shader) : this(shader, "a uvmap node") { }

        public UVMapNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal UVMapNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new UVMapNodeInputs(this);
            outputs = new UVMapNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.uvmapnode_get_node_type();
        }
#region Setters

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "from_dupli":
                    /* uvmapnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'from_dupli', 'ui_name': 'from dupli'} */
                    {
                    CSycles.uvmapnode_set_from_dupli(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type UVMapNode (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "attribute":
                    /* uvmapnode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'attribute', 'ui_name': 'attribute'} */
                    {
                    CSycles.uvmapnode_set_attribute(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type UVMapNode (setter)");
            }
        }

#endregion
#region Getters

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "from_dupli":
                /* uvmapnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'from_dupli', 'ui_name': 'from dupli'} */
                {
                    return CSycles.uvmapnode_get_from_dupli(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type UVMapNode (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "attribute":
                /* uvmapnode . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'attribute', 'ui_name': 'attribute'} */
                {
                    return CSycles.uvmapnode_get_attribute(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type UVMapNode (getter)");
            }
        }

#endregion
    }

}