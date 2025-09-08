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
namespace ccl.ShaderNodes
{
    using cclext;
    [ShaderNode(name: "curves_node", for_public_sdk: false)]
    public class CurvesNode : ShaderNode
    {
        public CurvesNode(Shader shader) : this(shader, "a curves_node node") { }

        public CurvesNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal CurvesNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
        }
        public IntPtr GetCurves() {
            return CSycles.curvesnode_get_curves(Ptr);
        }

        public void SetFac(float value) {
            CSycles.curvesnode_set_fac(Ptr, value);
        }

        public float GetFac() {
            return CSycles.curvesnode_get_fac(Ptr);
        }

        public float GetMaxX() {
            return CSycles.curvesnode_get_max_x(Ptr);
        }

        public void SetExtrapolate(bool value) {
            CSycles.curvesnode_set_extrapolate(Ptr, value);
        }

        public float GetMinX() {
            return CSycles.curvesnode_get_min_x(Ptr);
        }

        public void SetCurves(IntPtr value) {
            CSycles.curvesnode_set_curves(Ptr, value);
        }

        public void SetMaxX(float value) {
            CSycles.curvesnode_set_max_x(Ptr, value);
        }

        public bool GetExtrapolate() {
            return CSycles.curvesnode_get_extrapolate(Ptr);
        }

        public void SetMinX(float value) {
            CSycles.curvesnode_set_min_x(Ptr, value);
        }

        public int GetCurves1() {
            return CSycles.curvesnode_get_curves_1(Ptr);
        }

        public void SetValue(float3 value) {
            CSycles.curvesnode_set_value(Ptr, value);
        }

        public float3 GetValue() {
            return CSycles.curvesnode_get_value(Ptr);
        }
    }

}