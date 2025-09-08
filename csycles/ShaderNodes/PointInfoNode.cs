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
    public class PointInfoNodeOutputs : Outputs
    {
        public FloatSocket Random { get; private set; }
        public FloatSocket Radius { get; private set; }
        public PointSocket Position { get; private set; }

        public PointInfoNodeOutputs(ShaderNode parentNode)
        {
            Random = new FloatSocket(parentNode, "Random", "random", false);
            AddSocket(Random);
            Radius = new FloatSocket(parentNode, "Radius", "radius", false);
            AddSocket(Radius);
            Position = new PointSocket(parentNode, "Position", "position", false);
            AddSocket(Position);
        }
    }

    [ShaderNode(name: "point_info")]
    public class PointInfoNode : ShaderNode
    {
        public PointInfoNodeOutputs outs => (PointInfoNodeOutputs)outputs;
        public PointInfoNode(Shader shader) : this(shader, "a point_info node") { }

        public PointInfoNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal PointInfoNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            outputs = new PointInfoNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.pointinfonode_get_node_type();
        }
    }

}