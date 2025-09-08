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

    public class WireframeNodeInputs : Inputs
    {
        public FloatSocket Size { get; private set; }
        public BoolSocket UsePixelSize { get; private set; }

        public WireframeNodeInputs(ShaderNode parentNode)
        {
            Size = new FloatSocket(parentNode, "Size", "size", true);
            AddSocket(Size);
            UsePixelSize = new BoolSocket(parentNode, "Use Pixel Size", "use_pixel_size", true);
            AddSocket(UsePixelSize);
        }
    }
    public class WireframeNodeOutputs : Outputs
    {
        public FloatSocket Fac { get; private set; }

        public WireframeNodeOutputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", false);
            AddSocket(Fac);
        }
    }

    [ShaderNode(name: "wireframe")]
    public class WireframeNode : ShaderNode
    {
        public WireframeNodeInputs ins => (WireframeNodeInputs)inputs;
        public WireframeNodeOutputs outs => (WireframeNodeOutputs)outputs;
        public WireframeNode(Shader shader) : this(shader, "a wireframe node") { }

        public WireframeNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal WireframeNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new WireframeNodeInputs(this);
            outputs = new WireframeNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.wireframenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "size":
                    /* wireframenode . {'datatype': 'FLOAT', 'default_value': '0.01f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'size', 'ui_name': 'Size'} */
                    {
                    CSycles.wireframenode_set_size(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WireframeNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_pixel_size":
                    /* wireframenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_pixel_size', 'ui_name': 'Use Pixel Size'} */
                    {
                    CSycles.wireframenode_set_use_pixel_size(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WireframeNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "size":
                /* wireframenode . {'datatype': 'FLOAT', 'default_value': '0.01f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'size', 'ui_name': 'Size'} */
                {
                    return CSycles.wireframenode_get_size(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WireframeNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_pixel_size":
                /* wireframenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_pixel_size', 'ui_name': 'Use Pixel Size'} */
                {
                    return CSycles.wireframenode_get_use_pixel_size(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WireframeNode (getter)");
            }
        }

#endregion
    }

}