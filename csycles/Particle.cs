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

Code generated at: 2025-11-21 07:20:37 UTC
----------------------------------------------------------------------

**/

using ccl;
using ccl.Attributes;
using ccl.ShaderNodes;
using ccl.ShaderNodes.Sockets;
using ccl.NodeSockets;
using System;
using System.Collections.Generic;
namespace ccl
{
    using cclext;
    public class Particle
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public Particle() {}

        public Particle(IntPtr intPtr) { Ptr = intPtr; }
        public float Size {
            get { return CSycles.particle_get_size(Ptr); }
            set { CSycles.particle_set_size(Ptr, value); }
        }

        public float Age {
            get { return CSycles.particle_get_age(Ptr); }
            set { CSycles.particle_set_age(Ptr, value); }
        }

        public float3 AngularVelocity {
            get { return CSycles.particle_get_angular_velocity(Ptr); }
            set { CSycles.particle_set_angular_velocity(Ptr, value); }
        }

        public float3 Velocity {
            get { return CSycles.particle_get_velocity(Ptr); }
            set { CSycles.particle_set_velocity(Ptr, value); }
        }

        public int Index {
            get { return CSycles.particle_get_index(Ptr); }
            set { CSycles.particle_set_index(Ptr, value); }
        }

        public float Lifetime {
            get { return CSycles.particle_get_lifetime(Ptr); }
            set { CSycles.particle_set_lifetime(Ptr, value); }
        }

        public float4 Rotation {
            get { return CSycles.particle_get_rotation(Ptr); }
            set { CSycles.particle_set_rotation(Ptr, value); }
        }

        public float3 Location {
            get { return CSycles.particle_get_location(Ptr); }
            set { CSycles.particle_set_location(Ptr, value); }
        }
    }

}