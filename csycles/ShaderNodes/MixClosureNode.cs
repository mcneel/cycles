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

    public class MixClosureNodeInputs : Inputs
    {
        public FloatSocket Fac { get; private set; }
        public ClosureSocket Closure1 { get; private set; }
        public ClosureSocket Closure2 { get; private set; }

        public MixClosureNodeInputs(ShaderNode parentNode)
        {
            Fac = new FloatSocket(parentNode, "Fac", "fac", true);
            AddSocket(Fac);
            Closure1 = new ClosureSocket(parentNode, "Closure1", "closure1", true);
            AddSocket(Closure1);
            Closure2 = new ClosureSocket(parentNode, "Closure2", "closure2", true);
            AddSocket(Closure2);
        }
    }
    public class MixClosureNodeOutputs : Outputs
    {
        public ClosureSocket Closure { get; private set; }

        public MixClosureNodeOutputs(ShaderNode parentNode)
        {
            Closure = new ClosureSocket(parentNode, "Closure", "closure", false);
            AddSocket(Closure);
        }
    }

    [ShaderNode(name: "mix_closure")]
    public class MixClosureNode : ShaderNode
    {
        public MixClosureNodeInputs ins => (MixClosureNodeInputs)inputs;
        public MixClosureNodeOutputs outs => (MixClosureNodeOutputs)outputs;
        public MixClosureNode(Shader shader) : this(shader, "a mix_closure node") { }

        public MixClosureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MixClosureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MixClosureNodeInputs(this);
            outputs = new MixClosureNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.Closure;
        }
        public static IntPtr GetNodeType() {
            return CSycles.mixclosurenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "fac":
                    /* mixclosurenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                    {
                    CSycles.mixclosurenode_set_fac(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixClosureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "fac":
                /* mixclosurenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fac', 'ui_name': 'Fac'} */
                {
                    return CSycles.mixclosurenode_get_fac(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MixClosureNode (getter)");
            }
        }

#endregion
    }

}