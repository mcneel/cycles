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
    public class SubdParams
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public SubdParams() {}

        public SubdParams(IntPtr intPtr) { Ptr = intPtr; }
        public float DicingRate {
            get { return CSycles.subdparams_get_dicing_rate(Ptr); }
            set { CSycles.subdparams_set_dicing_rate(Ptr, value); }
        }

        public Transform Objecttoworld {
            get { return CSycles.subdparams_get_objecttoworld(Ptr); }
            set { CSycles.subdparams_set_objecttoworld(Ptr, value); }
        }

        public int SplitThreshold {
            get { return CSycles.subdparams_get_split_threshold(Ptr); }
            set { CSycles.subdparams_set_split_threshold(Ptr, value); }
        }

        public IntPtr Camera {
            get { return CSycles.subdparams_get_camera(Ptr); }
            set { CSycles.subdparams_set_camera(Ptr, value); }
        }

        public bool Ptex {
            get { return CSycles.subdparams_get_ptex(Ptr); }
            set { CSycles.subdparams_set_ptex(Ptr, value); }
        }

        public IntPtr Mesh {
            get { return CSycles.subdparams_get_mesh(Ptr); }
            set { CSycles.subdparams_set_mesh(Ptr, value); }
        }

        public int MaxLevel {
            get { return CSycles.subdparams_get_max_level(Ptr); }
            set { CSycles.subdparams_set_max_level(Ptr, value); }
        }

        public int TestSteps {
            get { return CSycles.subdparams_get_test_steps(Ptr); }
            set { CSycles.subdparams_set_test_steps(Ptr, value); }
        }
    }

}