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
    public class vfloat8
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public vfloat8() {}

        public vfloat8(IntPtr intPtr) { Ptr = intPtr; }
        public float D {
            get { return CSycles.vfloat8_get_d(Ptr); }
            set { CSycles.vfloat8_set_d(Ptr, value); }
        }

        public float E {
            get { return CSycles.vfloat8_get_e(Ptr); }
            set { CSycles.vfloat8_set_e(Ptr, value); }
        }

        public float G {
            get { return CSycles.vfloat8_get_g(Ptr); }
            set { CSycles.vfloat8_set_g(Ptr, value); }
        }

        public float A {
            get { return CSycles.vfloat8_get_a(Ptr); }
            set { CSycles.vfloat8_set_a(Ptr, value); }
        }

        public float H {
            get { return CSycles.vfloat8_get_h(Ptr); }
            set { CSycles.vfloat8_set_h(Ptr, value); }
        }

        public float OperatorIndex(int i) {
            return CSycles.vfloat8_operator_index(Ptr, i);
        }

        public float F {
            get { return CSycles.vfloat8_get_f(Ptr); }
            set { CSycles.vfloat8_set_f(Ptr, value); }
        }

        public float C {
            get { return CSycles.vfloat8_get_c(Ptr); }
            set { CSycles.vfloat8_set_c(Ptr, value); }
        }

        public float OperatorIndex1(int i) {
            return CSycles.vfloat8_operator_index_1(Ptr, i);
        }

        public float B {
            get { return CSycles.vfloat8_get_b(Ptr); }
            set { CSycles.vfloat8_set_b(Ptr, value); }
        }
    }

}