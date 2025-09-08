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
    public class VolumeInfoNodeOutputs : Outputs
    {
        public FloatSocket Flame { get; private set; }
        public FloatSocket Density { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket Temperature { get; private set; }

        public VolumeInfoNodeOutputs(ShaderNode parentNode)
        {
            Flame = new FloatSocket(parentNode, "Flame", "flame", false);
            AddSocket(Flame);
            Density = new FloatSocket(parentNode, "Density", "density", false);
            AddSocket(Density);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
            Temperature = new FloatSocket(parentNode, "Temperature", "temperature", false);
            AddSocket(Temperature);
        }
    }

    [ShaderNode(name: "volume_info")]
    public class VolumeInfoNode : ShaderNode
    {
        public VolumeInfoNodeOutputs outs => (VolumeInfoNodeOutputs)outputs;
        public VolumeInfoNode(Shader shader) : this(shader, "a volume_info node") { }

        public VolumeInfoNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal VolumeInfoNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            outputs = new VolumeInfoNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.volumeinfonode_get_node_type();
        }
    }

}