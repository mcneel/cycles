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

    public class AddClosureNodeInputs : Inputs
    {
        public ClosureSocket Closure1 { get; private set; }
        public ClosureSocket Closure2 { get; private set; }

        public AddClosureNodeInputs(ShaderNode parentNode)
        {
            Closure1 = new ClosureSocket(parentNode, "Closure1", "closure1", true);
            AddSocket(Closure1);
            Closure2 = new ClosureSocket(parentNode, "Closure2", "closure2", true);
            AddSocket(Closure2);
        }
    }
    public class AddClosureNodeOutputs : Outputs
    {
        public ClosureSocket Closure { get; private set; }

        public AddClosureNodeOutputs(ShaderNode parentNode)
        {
            Closure = new ClosureSocket(parentNode, "Closure", "closure", false);
            AddSocket(Closure);
        }
    }

    [ShaderNode(name: "add_closure")]
    public class AddClosureNode : ShaderNode
    {
        public AddClosureNodeInputs ins => (AddClosureNodeInputs)inputs;
        public AddClosureNodeOutputs outs => (AddClosureNodeOutputs)outputs;
        public AddClosureNode(Shader shader) : this(shader, "a add_closure node") { }

        public AddClosureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal AddClosureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new AddClosureNodeInputs(this);
            outputs = new AddClosureNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.Closure;
        }
        public static IntPtr GetNodeType() {
            return CSycles.addclosurenode_get_node_type();
        }
    }

}