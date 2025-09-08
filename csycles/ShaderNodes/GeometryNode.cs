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
    public class GeometryNodeOutputs : Outputs
    {
        public NormalSocket TrueNormal { get; private set; }
        public FloatSocket RandomPerIsland { get; private set; }
        public VectorSocket Incoming { get; private set; }
        public PointSocket Position { get; private set; }
        public PointSocket Parametric { get; private set; }
        public NormalSocket Normal { get; private set; }
        public FloatSocket Backfacing { get; private set; }
        public NormalSocket Tangent { get; private set; }
        public FloatSocket Pointiness { get; private set; }

        public GeometryNodeOutputs(ShaderNode parentNode)
        {
            TrueNormal = new NormalSocket(parentNode, "True Normal", "true_normal", false);
            AddSocket(TrueNormal);
            RandomPerIsland = new FloatSocket(parentNode, "Random Per Island", "random_per_island", false);
            AddSocket(RandomPerIsland);
            Incoming = new VectorSocket(parentNode, "Incoming", "incoming", false);
            AddSocket(Incoming);
            Position = new PointSocket(parentNode, "Position", "position", false);
            AddSocket(Position);
            Parametric = new PointSocket(parentNode, "Parametric", "parametric", false);
            AddSocket(Parametric);
            Normal = new NormalSocket(parentNode, "Normal", "normal", false);
            AddSocket(Normal);
            Backfacing = new FloatSocket(parentNode, "Backfacing", "backfacing", false);
            AddSocket(Backfacing);
            Tangent = new NormalSocket(parentNode, "Tangent", "tangent", false);
            AddSocket(Tangent);
            Pointiness = new FloatSocket(parentNode, "Pointiness", "pointiness", false);
            AddSocket(Pointiness);
        }
    }

    [ShaderNode(name: "geometry")]
    public class GeometryNode : ShaderNode
    {
        public GeometryNodeOutputs outs => (GeometryNodeOutputs)outputs;
        public GeometryNode(Shader shader) : this(shader, "a geometry node") { }

        public GeometryNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal GeometryNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            outputs = new GeometryNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.geometrynode_get_node_type();
        }
    }

}