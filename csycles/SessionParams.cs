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
    public class SessionParams
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public SessionParams() {}

        public SessionParams(IntPtr intPtr) { Ptr = intPtr; }
        public Device Device {
            set {
                CSycles.session_params_set_deviceinfo(this, value.Info);
            }
        }

        public int TileSize {
            get { return CSycles.sessionparams_get_tile_size(Ptr); }
            set { CSycles.sessionparams_set_tile_size(Ptr, value); }
        }

        public bool UseResolutionDivider {
            get { return CSycles.sessionparams_get_use_resolution_divider(Ptr); }
            set { CSycles.sessionparams_set_use_resolution_divider(Ptr, value); }
        }

        public int Samples {
            get { return CSycles.sessionparams_get_samples(Ptr); }
            set { CSycles.sessionparams_set_samples(Ptr, value); }
        }

        public double TimeLimit {
            get { return CSycles.sessionparams_get_time_limit(Ptr); }
            set { CSycles.sessionparams_set_time_limit(Ptr, value); }
        }

        public bool Background {
            get { return CSycles.sessionparams_get_background(Ptr); }
            set { CSycles.sessionparams_set_background(Ptr, value); }
        }

        public int PixelSize {
            get { return CSycles.sessionparams_get_pixel_size(Ptr); }
            set { CSycles.sessionparams_set_pixel_size(Ptr, value); }
        }

        public bool UseProfiling {
            get { return CSycles.sessionparams_get_use_profiling(Ptr); }
            set { CSycles.sessionparams_set_use_profiling(Ptr, value); }
        }

        public int SampleSubsetLength {
            get { return CSycles.sessionparams_get_sample_subset_length(Ptr); }
            set { CSycles.sessionparams_set_sample_subset_length(Ptr, value); }
        }

        public bool Headless {
            get { return CSycles.sessionparams_get_headless(Ptr); }
            set { CSycles.sessionparams_set_headless(Ptr, value); }
        }

        public bool UseAutoTile {
            get { return CSycles.sessionparams_get_use_auto_tile(Ptr); }
            set { CSycles.sessionparams_set_use_auto_tile(Ptr, value); }
        }

        public int Threads {
            get { return CSycles.sessionparams_get_threads(Ptr); }
            set { CSycles.sessionparams_set_threads(Ptr, value); }
        }

        public ShadingSystem Shadingsystem {
            get { return CSycles.sessionparams_get_shadingsystem(Ptr); }
            set { CSycles.sessionparams_set_shadingsystem(Ptr, value); }
        }

        public bool UseSampleSubset {
            get { return CSycles.sessionparams_get_use_sample_subset(Ptr); }
            set { CSycles.sessionparams_set_use_sample_subset(Ptr, value); }
        }

        public int SampleSubsetOffset {
            get { return CSycles.sessionparams_get_sample_subset_offset(Ptr); }
            set { CSycles.sessionparams_set_sample_subset_offset(Ptr, value); }
        }

        public bool Experimental {
            get { return CSycles.sessionparams_get_experimental(Ptr); }
            set { CSycles.sessionparams_set_experimental(Ptr, value); }
        }
    }

}