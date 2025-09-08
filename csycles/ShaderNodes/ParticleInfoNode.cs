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
    public class ParticleInfoNodeOutputs : Outputs
    {
        public FloatSocket Lifetime { get; private set; }
        public PointSocket Location { get; private set; }
        public FloatSocket Index { get; private set; }
        public FloatSocket Size { get; private set; }
        public FloatSocket Random { get; private set; }
        public VectorSocket Velocity { get; private set; }
        public FloatSocket Age { get; private set; }
        public VectorSocket AngularVelocity { get; private set; }

        public ParticleInfoNodeOutputs(ShaderNode parentNode)
        {
            Lifetime = new FloatSocket(parentNode, "Lifetime", "lifetime", false);
            AddSocket(Lifetime);
            Location = new PointSocket(parentNode, "Location", "location", false);
            AddSocket(Location);
            Index = new FloatSocket(parentNode, "Index", "index", false);
            AddSocket(Index);
            Size = new FloatSocket(parentNode, "Size", "size", false);
            AddSocket(Size);
            Random = new FloatSocket(parentNode, "Random", "random", false);
            AddSocket(Random);
            Velocity = new VectorSocket(parentNode, "Velocity", "velocity", false);
            AddSocket(Velocity);
            Age = new FloatSocket(parentNode, "Age", "age", false);
            AddSocket(Age);
            AngularVelocity = new VectorSocket(parentNode, "Angular Velocity", "angular_velocity", false);
            AddSocket(AngularVelocity);
        }
    }

    [ShaderNode(name: "particle_info")]
    public class ParticleInfoNode : ShaderNode
    {
        public ParticleInfoNodeOutputs outs => (ParticleInfoNodeOutputs)outputs;
        public ParticleInfoNode(Shader shader) : this(shader, "a particle_info node") { }

        public ParticleInfoNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal ParticleInfoNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            outputs = new ParticleInfoNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.particleinfonode_get_node_type();
        }
    }

}