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
    public class Ray
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public Ray() {}

        public Ray(IntPtr intPtr) { Ptr = intPtr; }
        public float Tmin {
            get { return CSycles.ray_get_tmin(Ptr); }
            set { CSycles.ray_set_tmin(Ptr, value); }
        }

        public float Dd {
            get { return CSycles.ray_get_dd(Ptr); }
            set { CSycles.ray_set_dd(Ptr, value); }
        }

        public float Time {
            get { return CSycles.ray_get_time(Ptr); }
            set { CSycles.ray_set_time(Ptr, value); }
        }

        public float Dp {
            get { return CSycles.ray_get_dp(Ptr); }
            set { CSycles.ray_set_dp(Ptr, value); }
        }

        public float3 D {
            get { return CSycles.ray_get_d(Ptr); }
            set { CSycles.ray_set_d(Ptr, value); }
        }

        public RaySelfPrimitives Self {
            get { return CSycles.ray_get_self(Ptr); }
            set { CSycles.ray_set_self(Ptr, value); }
        }

        public float Tmax {
            get { return CSycles.ray_get_tmax(Ptr); }
            set { CSycles.ray_set_tmax(Ptr, value); }
        }

        public float3 P {
            get { return CSycles.ray_get_p(Ptr); }
            set { CSycles.ray_set_p(Ptr, value); }
        }
    }

}