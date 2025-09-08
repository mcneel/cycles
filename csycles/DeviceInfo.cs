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
    public class DeviceInfo
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public DeviceInfo() {}

        public DeviceInfo(IntPtr intPtr) { Ptr = intPtr; }
        public bool UseHardwareRaytracing {
            get { return CSycles.deviceinfo_get_use_hardware_raytracing(Ptr); }
            set { CSycles.deviceinfo_set_use_hardware_raytracing(Ptr, value); }
        }

        public bool ContainsDeviceType(DeviceType type) {
            return CSycles.deviceinfo_contains_device_type(Ptr, type);
        }

        public bool HasGpuQueue {
            get { return CSycles.deviceinfo_get_has_gpu_queue(Ptr); }
            set { CSycles.deviceinfo_set_has_gpu_queue(Ptr, value); }
        }

        public KernelOptimizationLevel KernelOptimizationLevel {
            get { return CSycles.deviceinfo_get_kernel_optimization_level(Ptr); }
            set { CSycles.deviceinfo_set_kernel_optimization_level(Ptr, value); }
        }

        public bool OperatorNeq(DeviceInfo info) {
            return CSycles.deviceinfo_operator_neq(Ptr, info);
        }

        public bool DisplayDevice {
            get { return CSycles.deviceinfo_get_display_device(Ptr); }
            set { CSycles.deviceinfo_set_display_device(Ptr, value); }
        }

        public bool HasProfiling {
            get { return CSycles.deviceinfo_get_has_profiling(Ptr); }
            set { CSycles.deviceinfo_set_has_profiling(Ptr, value); }
        }

        public int CpuThreads {
            get { return CSycles.deviceinfo_get_cpu_threads(Ptr); }
            set { CSycles.deviceinfo_set_cpu_threads(Ptr, value); }
        }

        public bool HasNanovdb {
            get { return CSycles.deviceinfo_get_has_nanovdb(Ptr); }
            set { CSycles.deviceinfo_set_has_nanovdb(Ptr, value); }
        }

        public int Num {
            get { return CSycles.deviceinfo_get_num(Ptr); }
            set { CSycles.deviceinfo_set_num(Ptr, value); }
        }

        public string ErrorMsg {
            get { return CSycles.deviceinfo_get_error_msg(Ptr); }
            set { CSycles.deviceinfo_set_error_msg(Ptr, value); }
        }

        public bool HasGuiding {
            get { return CSycles.deviceinfo_get_has_guiding(Ptr); }
            set { CSycles.deviceinfo_set_has_guiding(Ptr, value); }
        }

        public bool HasOsl {
            get { return CSycles.deviceinfo_get_has_osl(Ptr); }
            set { CSycles.deviceinfo_set_has_osl(Ptr, value); }
        }

        public bool UseMetalrtByDefault {
            get { return CSycles.deviceinfo_get_use_metalrt_by_default(Ptr); }
            set { CSycles.deviceinfo_set_use_metalrt_by_default(Ptr, value); }
        }

        public string Description {
            get { return CSycles.deviceinfo_get_description(Ptr); }
            set { CSycles.deviceinfo_set_description(Ptr, value); }
        }

        public bool OperatorEq(DeviceInfo info) {
            return CSycles.deviceinfo_operator_eq(Ptr, info);
        }

        public string Id {
            get { return CSycles.deviceinfo_get_id(Ptr); }
            set { CSycles.deviceinfo_set_id(Ptr, value); }
        }

        public DeviceType Type {
            get { return CSycles.deviceinfo_get_type(Ptr); }
            set { CSycles.deviceinfo_set_type(Ptr, value); }
        }

        public bool HasMnee {
            get { return CSycles.deviceinfo_get_has_mnee(Ptr); }
            set { CSycles.deviceinfo_set_has_mnee(Ptr, value); }
        }

        public bool HasPeerMemory {
            get { return CSycles.deviceinfo_get_has_peer_memory(Ptr); }
            set { CSycles.deviceinfo_set_has_peer_memory(Ptr, value); }
        }
    }

}