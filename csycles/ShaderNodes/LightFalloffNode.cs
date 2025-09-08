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

    public class LightFalloffNodeInputs : Inputs
    {
        public FloatSocket Smooth { get; private set; }
        public FloatSocket Strength { get; private set; }

        public LightFalloffNodeInputs(ShaderNode parentNode)
        {
            Smooth = new FloatSocket(parentNode, "Smooth", "smooth", true);
            AddSocket(Smooth);
            Strength = new FloatSocket(parentNode, "Strength", "strength", true);
            AddSocket(Strength);
        }
    }
    public class LightFalloffNodeOutputs : Outputs
    {
        public FloatSocket Constant { get; private set; }
        public FloatSocket Linear { get; private set; }
        public FloatSocket Quadratic { get; private set; }

        public LightFalloffNodeOutputs(ShaderNode parentNode)
        {
            Constant = new FloatSocket(parentNode, "Constant", "constant", false);
            AddSocket(Constant);
            Linear = new FloatSocket(parentNode, "Linear", "linear", false);
            AddSocket(Linear);
            Quadratic = new FloatSocket(parentNode, "Quadratic", "quadratic", false);
            AddSocket(Quadratic);
        }
    }

    [ShaderNode(name: "light_falloff")]
    public class LightFalloffNode : ShaderNode
    {
        public LightFalloffNodeInputs ins => (LightFalloffNodeInputs)inputs;
        public LightFalloffNodeOutputs outs => (LightFalloffNodeOutputs)outputs;
        public LightFalloffNode(Shader shader) : this(shader, "a light_falloff node") { }

        public LightFalloffNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal LightFalloffNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new LightFalloffNodeInputs(this);
            outputs = new LightFalloffNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.lightfalloffnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "smooth":
                    /* lightfalloffnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'smooth', 'ui_name': 'Smooth'} */
                    {
                    CSycles.lightfalloffnode_set_smooth(this.Ptr, data);
                    }
                    break;
            case "strength":
                    /* lightfalloffnode . {'datatype': 'FLOAT', 'default_value': '100.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                    {
                    CSycles.lightfalloffnode_set_strength(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type LightFalloffNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "smooth":
                /* lightfalloffnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'smooth', 'ui_name': 'Smooth'} */
                {
                    return CSycles.lightfalloffnode_get_smooth(this.Ptr);
                }
            case "strength":
                /* lightfalloffnode . {'datatype': 'FLOAT', 'default_value': '100.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                {
                    return CSycles.lightfalloffnode_get_strength(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type LightFalloffNode (getter)");
            }
        }

#endregion
    }

}