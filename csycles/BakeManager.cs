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
    public class BakeManager
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public BakeManager() {}

        public BakeManager(IntPtr intPtr) { Ptr = intPtr; }
        public void SetBaking(IntPtr scene, bool use) {
            CSycles.bakemanager_set_baking(Ptr, scene, use);
        }

        public bool GetBaking() {
            return CSycles.bakemanager_get_baking(Ptr);
        }

        public void SetUseCamera(bool use_camera) {
            CSycles.bakemanager_set_use_camera(Ptr, use_camera);
        }

        public void SetUseSeed(bool use_seed) {
            CSycles.bakemanager_set_use_seed(Ptr, use_seed);
        }

        public bool GetUseSeed() {
            return CSycles.bakemanager_get_use_seed(Ptr);
        }
    }

}