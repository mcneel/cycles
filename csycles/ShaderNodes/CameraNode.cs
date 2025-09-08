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
    public class CameraNodeOutputs : Outputs
    {
        public FloatSocket ViewDistance { get; private set; }
        public FloatSocket ViewZDepth { get; private set; }
        public VectorSocket ViewVector { get; private set; }

        public CameraNodeOutputs(ShaderNode parentNode)
        {
            ViewDistance = new FloatSocket(parentNode, "View Distance", "view_distance", false);
            AddSocket(ViewDistance);
            ViewZDepth = new FloatSocket(parentNode, "View Z Depth", "view_z_depth", false);
            AddSocket(ViewZDepth);
            ViewVector = new VectorSocket(parentNode, "View Vector", "view_vector", false);
            AddSocket(ViewVector);
        }
    }

    [ShaderNode(name: "camera_info")]
    public class CameraNode : ShaderNode
    {
        public CameraNodeOutputs outs => (CameraNodeOutputs)outputs;
        public CameraNode(Shader shader) : this(shader, "a camera_info node") { }

        public CameraNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal CameraNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            outputs = new CameraNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.cameranode_get_node_type();
        }
    }

}