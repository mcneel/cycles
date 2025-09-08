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
    public class ObjectInfoNodeOutputs : Outputs
    {
        public VectorSocket Location { get; private set; }
        public FloatSocket Random { get; private set; }
        public FloatSocket ObjectIndex { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket MaterialIndex { get; private set; }
        public FloatSocket Alpha { get; private set; }

        public ObjectInfoNodeOutputs(ShaderNode parentNode)
        {
            Location = new VectorSocket(parentNode, "Location", "location", false);
            AddSocket(Location);
            Random = new FloatSocket(parentNode, "Random", "random", false);
            AddSocket(Random);
            ObjectIndex = new FloatSocket(parentNode, "Object Index", "object_index", false);
            AddSocket(ObjectIndex);
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
            MaterialIndex = new FloatSocket(parentNode, "Material Index", "material_index", false);
            AddSocket(MaterialIndex);
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", false);
            AddSocket(Alpha);
        }
    }

    [ShaderNode(name: "object_info")]
    public class ObjectInfoNode : ShaderNode
    {
        public ObjectInfoNodeOutputs outs => (ObjectInfoNodeOutputs)outputs;
        public ObjectInfoNode(Shader shader) : this(shader, "a object_info node") { }

        public ObjectInfoNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal ObjectInfoNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            outputs = new ObjectInfoNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.objectinfonode_get_node_type();
        }
    }

}