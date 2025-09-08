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

    public class WavelengthNodeInputs : Inputs
    {
        public FloatSocket Wavelength { get; private set; }

        public WavelengthNodeInputs(ShaderNode parentNode)
        {
            Wavelength = new FloatSocket(parentNode, "Wavelength", "wavelength", true);
            AddSocket(Wavelength);
        }
    }
    public class WavelengthNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }

        public WavelengthNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
        }
    }

    [ShaderNode(name: "wavelength")]
    public class WavelengthNode : ShaderNode
    {
        public WavelengthNodeInputs ins => (WavelengthNodeInputs)inputs;
        public WavelengthNodeOutputs outs => (WavelengthNodeOutputs)outputs;
        public WavelengthNode(Shader shader) : this(shader, "a wavelength node") { }

        public WavelengthNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal WavelengthNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new WavelengthNodeInputs(this);
            outputs = new WavelengthNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.wavelengthnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "wavelength":
                    /* wavelengthnode . {'datatype': 'FLOAT', 'default_value': '500.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'wavelength', 'ui_name': 'Wavelength'} */
                    {
                    CSycles.wavelengthnode_set_wavelength(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WavelengthNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "wavelength":
                /* wavelengthnode . {'datatype': 'FLOAT', 'default_value': '500.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'wavelength', 'ui_name': 'Wavelength'} */
                {
                    return CSycles.wavelengthnode_get_wavelength(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WavelengthNode (getter)");
            }
        }

#endregion
    }

}