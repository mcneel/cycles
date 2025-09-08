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
    public class OrientationBounds
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public OrientationBounds() {}

        public OrientationBounds(IntPtr intPtr) { Ptr = intPtr; }
        public float CalculateMeasure() {
            return CSycles.orientationbounds_calculate_measure(Ptr);
        }

        public bool IsEmpty() {
            return CSycles.orientationbounds_is_empty(Ptr);
        }

        public float3 Axis {
            get { return CSycles.orientationbounds_get_axis(Ptr); }
            set { CSycles.orientationbounds_set_axis(Ptr, value); }
        }

        public float ThetaO {
            get { return CSycles.orientationbounds_get_theta_o(Ptr); }
            set { CSycles.orientationbounds_set_theta_o(Ptr, value); }
        }

        public float ThetaE {
            get { return CSycles.orientationbounds_get_theta_e(Ptr); }
            set { CSycles.orientationbounds_set_theta_e(Ptr, value); }
        }
    }

}