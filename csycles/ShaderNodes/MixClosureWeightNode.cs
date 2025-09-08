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

    public class MixClosureWeightNodeInputs : Inputs
    {
        public FloatSocket Fac { get; private set; }
        public FloatSocket Weight { get; private set; }

        public MixClosureWeightNodeInputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", true);
            AddSocket(Fac);
            Weight = new FloatSocket(parentNode, "Weight", "weight", true);
            AddSocket(Weight);
        }
    }
    public class MixClosureWeightNodeOutputs : Outputs
    {
        public FloatSocket Weight2 { get; private set; }
        public FloatSocket Weight1 { get; private set; }

        public MixClosureWeightNodeOutputs(ShaderNode parentNode)
        {
            Weight2 = new FloatSocket(parentNode, "Weight2", "weight2", false);
            AddSocket(Weight2);
            Weight1 = new FloatSocket(parentNode, "Weight1", "weight1", false);
            AddSocket(Weight1);
        }
    }

    [ShaderNode(name: "mix_closure_weight")]
    public class MixClosureWeightNode : ShaderNode
    {
        public MixClosureWeightNodeInputs ins => (MixClosureWeightNodeInputs)inputs;
        public MixClosureWeightNodeOutputs outs => (MixClosureWeightNodeOutputs)outputs;
        public MixClosureWeightNode(Shader shader) : this(shader, "a mix_closure_weight node") { }

        public MixClosureWeightNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MixClosureWeightNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MixClosureWeightNodeInputs(this);
            outputs = new MixClosureWeightNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.mixclosureweightnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "fac":
                    /* mixclosureweightnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                    {
                    CSycles.mixclosureweightnode_set_fac(this.Ptr, data);
                    }
                    break;
            case "weight":
                    /* mixclosureweightnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'weight', 'ui_name': 'Weight'} */
                    {
                    CSycles.mixclosureweightnode_set_weight(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixClosureWeightNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "fac":
                /* mixclosureweightnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                {
                    return CSycles.mixclosureweightnode_get_fac(this.Ptr);
                }
            case "weight":
                /* mixclosureweightnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'weight', 'ui_name': 'Weight'} */
                {
                    return CSycles.mixclosureweightnode_get_weight(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixClosureWeightNode (getter)");
            }
        }

#endregion
    }

}