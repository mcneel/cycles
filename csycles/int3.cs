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
    public class int3
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public int3() {}

        public int3(IntPtr intPtr) { Ptr = intPtr; }
        public int X {
            get { return CSycles.int3_get_x(Ptr); }
            set { CSycles.int3_set_x(Ptr, value); }
        }

        public int OperatorIndex(int i) {
            return CSycles.int3_operator_index(Ptr, i);
        }

        public int W {
            get { return CSycles.int3_get_w(Ptr); }
            set { CSycles.int3_set_w(Ptr, value); }
        }

        public int OperatorIndex1(int i) {
            return CSycles.int3_operator_index_1(Ptr, i);
        }

        public int Y {
            get { return CSycles.int3_get_y(Ptr); }
            set { CSycles.int3_set_y(Ptr, value); }
        }

        public int Z {
            get { return CSycles.int3_get_z(Ptr); }
            set { CSycles.int3_set_z(Ptr, value); }
        }
    }

}