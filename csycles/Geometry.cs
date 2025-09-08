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
    public class Geometry : Node
    {
        public Geometry() : this("a geometry node") { }

        public Geometry(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Geometry(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
        }
        public void TagUpdate(Scene scene)
        {
            CSycles.geometry_tag_update(this, scene);
        }

        public void AddShader(Shader shader)
        {
            CSycles.geometry_add_shader(Ptr, shader.Ptr);
        }

        public void SetShader(Shader shader)
        {
            CSycles.geometry_set_shader(Ptr, shader.Ptr);
        }

        public bool TransformApplied {
            get { return CSycles.geometry_get_transform_applied(Ptr); }
            set { CSycles.geometry_set_transform_applied(Ptr, value); }
        }

        public virtual PrimitiveType PrimitiveType() {
            return CSycles.geometry_primitive_type(Ptr);
        }

        public int MotionStep(float time) {
            return CSycles.geometry_motion_step(Ptr, time);
        }

        public bool IsHair() {
            return CSycles.geometry_is_hair(Ptr);
        }

        public bool TransformNegativeScaled {
            get { return CSycles.geometry_get_transform_negative_scaled(Ptr); }
            set { CSycles.geometry_set_transform_negative_scaled(Ptr, value); }
        }

        public virtual void ApplyTransform(Transform tfm, bool apply_to_motion) {
            CSycles.geometry_apply_transform(Ptr, tfm, apply_to_motion);
        }

        public void SetUseMotionBlur(bool value) {
            CSycles.geometry_set_use_motion_blur(Ptr, value);
        }

        public bool GetUseMotionBlur() {
            return CSycles.geometry_get_use_motion_blur(Ptr);
        }

        public long AttrMapOffset {
            get { return CSycles.geometry_get_attr_map_offset(Ptr); }
            set { CSycles.geometry_set_attr_map_offset(Ptr, value); }
        }

        public bool IsMesh() {
            return CSycles.geometry_is_mesh(Ptr);
        }

        public virtual void ComputeBounds() {
            CSycles.geometry_compute_bounds(Ptr);
        }

        public void SetMotionSteps(uint value) {
            CSycles.geometry_set_motion_steps(Ptr, value);
        }

        public bool IsLight() {
            return CSycles.geometry_is_light(Ptr);
        }

        public long PrimOffset {
            get { return CSycles.geometry_get_prim_offset(Ptr); }
            set { CSycles.geometry_set_prim_offset(Ptr, value); }
        }

        public virtual bool HasMotionBlur() {
            return CSycles.geometry_has_motion_blur(Ptr);
        }

        public static IntPtr GetNodeBaseType() {
            return CSycles.geometry_get_node_base_type();
        }

        public bool IsInstanced() {
            return CSycles.geometry_is_instanced(Ptr);
        }

        public virtual void Clear(bool preserve_shaders) {
            CSycles.geometry_clear(Ptr, preserve_shaders);
        }

        public bool HasVolume {
            get { return CSycles.geometry_get_has_volume(Ptr); }
            set { CSycles.geometry_set_has_volume(Ptr, value); }
        }

        public bool IsVolume() {
            return CSycles.geometry_is_volume(Ptr);
        }

        public uint GetMotionSteps() {
            return CSycles.geometry_get_motion_steps(Ptr);
        }

        public Transform TransformNormal {
            get { return CSycles.geometry_get_transform_normal(Ptr); }
            set { CSycles.geometry_set_transform_normal(Ptr, value); }
        }

        public float MotionTime(int step) {
            return CSycles.geometry_motion_time(Ptr, step);
        }

        public long Index {
            get { return CSycles.geometry_get_index(Ptr); }
            set { CSycles.geometry_set_index(Ptr, value); }
        }

        public Geometry_Type GeometryType {
            get { return CSycles.geometry_get_geometry_type(Ptr); }
            set { CSycles.geometry_set_geometry_type(Ptr, value); }
        }

        public bool HasTrueDisplacement() {
            return CSycles.geometry_has_true_displacement(Ptr);
        }

        public bool IsPointcloud() {
            return CSycles.geometry_is_pointcloud(Ptr);
        }
    }

}