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
    public class SubEdge
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public SubEdge() {}

        public SubEdge(IntPtr intPtr) { Ptr = intPtr; }
        public int GetVertAlongEdge(int n) {
            return CSycles.subedge_get_vert_along_edge(Ptr, n);
        }

        public int T {
            get { return CSycles.subedge_get_t(Ptr); }
            set { CSycles.subedge_set_t(Ptr, value); }
        }

        public int EndVertIndex {
            get { return CSycles.subedge_get_end_vert_index(Ptr); }
            set { CSycles.subedge_set_end_vert_index(Ptr, value); }
        }

        public int SecondVertIndex {
            get { return CSycles.subedge_get_second_vert_index(Ptr); }
            set { CSycles.subedge_set_second_vert_index(Ptr, value); }
        }

        public int StartVertIndex {
            get { return CSycles.subedge_get_start_vert_index(Ptr); }
            set { CSycles.subedge_set_start_vert_index(Ptr, value); }
        }

        public int MidVertIndex {
            get { return CSycles.subedge_get_mid_vert_index(Ptr); }
            set { CSycles.subedge_set_mid_vert_index(Ptr, value); }
        }
    }

}