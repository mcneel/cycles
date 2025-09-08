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
    public class PassInfo
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public PassInfo() {}

        public PassInfo(IntPtr intPtr) { Ptr = intPtr; }
        public bool SupportDenoise {
            get { return CSycles.passinfo_get_support_denoise(Ptr); }
            set { CSycles.passinfo_set_support_denoise(Ptr, value); }
        }

        public bool UseCompositing {
            get { return CSycles.passinfo_get_use_compositing(Ptr); }
            set { CSycles.passinfo_set_use_compositing(Ptr, value); }
        }

        public bool UseDenoisingAlbedo {
            get { return CSycles.passinfo_get_use_denoising_albedo(Ptr); }
            set { CSycles.passinfo_set_use_denoising_albedo(Ptr, value); }
        }

        public PassType IndirectType {
            get { return CSycles.passinfo_get_indirect_type(Ptr); }
            set { CSycles.passinfo_set_indirect_type(Ptr, value); }
        }

        public PassType DirectType {
            get { return CSycles.passinfo_get_direct_type(Ptr); }
            set { CSycles.passinfo_set_direct_type(Ptr, value); }
        }

        public bool UseExposure {
            get { return CSycles.passinfo_get_use_exposure(Ptr); }
            set { CSycles.passinfo_set_use_exposure(Ptr, value); }
        }

        public bool UseFilter {
            get { return CSycles.passinfo_get_use_filter(Ptr); }
            set { CSycles.passinfo_set_use_filter(Ptr, value); }
        }

        public int NumComponents {
            get { return CSycles.passinfo_get_num_components(Ptr); }
            set { CSycles.passinfo_set_num_components(Ptr, value); }
        }

        public PassType DivideType {
            get { return CSycles.passinfo_get_divide_type(Ptr); }
            set { CSycles.passinfo_set_divide_type(Ptr, value); }
        }

        public bool IsWritten {
            get { return CSycles.passinfo_get_is_written(Ptr); }
            set { CSycles.passinfo_set_is_written(Ptr, value); }
        }
    }

}