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

Code generated at: 2025-12-02 03:24:08 UTC
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
    public class Volume : Mesh
    {
        public Volume() : this("a volume node") { }

        public Volume(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Volume(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
        }
        public override void Clear(bool preserve_shaders) {
            CSycles.volume_clear(Ptr, preserve_shaders);
        }

        public float GetClipping() {
            return CSycles.volume_get_clipping(Ptr);
        }

        public bool GetObjectSpace() {
            return CSycles.volume_get_object_space(Ptr);
        }

        public void SetClipping(float value) {
            CSycles.volume_set_clipping(Ptr, value);
        }

        public float GetStepSize() {
            return CSycles.volume_get_step_size(Ptr);
        }

        public void SetVelocityScale(float value) {
            CSycles.volume_set_velocity_scale(Ptr, value);
        }

        public void SetStepSize(float value) {
            CSycles.volume_set_step_size(Ptr, value);
        }

        public void SetObjectSpace(bool value) {
            CSycles.volume_set_object_space(Ptr, value);
        }

        public float GetVelocityScale() {
            return CSycles.volume_get_velocity_scale(Ptr);
        }
    }

}