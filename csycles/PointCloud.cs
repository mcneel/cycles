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

    public class PointCloudNodeInputs : NodeInputs
    {
        public FloatArrayNodeSocket Radius { get; private set; }
        public PointArrayNodeSocket Points { get; private set; }
        public IntArrayNodeSocket Shader { get; private set; }

        public PointCloudNodeInputs(Node parentNode)
        {
            Radius = new FloatArrayNodeSocket(parentNode, "Radius", "radius", true);
            AddSocket(Radius);
            Points = new PointArrayNodeSocket(parentNode, "Points", "points", true);
            AddSocket(Points);
            Shader = new IntArrayNodeSocket(parentNode, "Shader", "shader", true);
            AddSocket(Shader);
        }
    }
    public class PointCloud : Geometry
    {
        public PointCloudNodeInputs PointCloudNodeInputs { get; set; }
        public PointCloudNodeInputs ins => PointCloudNodeInputs;

        public PointCloud() : this("a pointcloud node") { }

        public PointCloud(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal PointCloud(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            PointCloudNodeInputs = new PointCloudNodeInputs(this);

        }
        public void Pack(IntPtr scene, IntPtr packed_points, IntPtr packed_shader) {
            CSycles.pointcloud_pack(Ptr, scene, packed_points, packed_shader);
        }
        public override PrimitiveType PrimitiveType() {
            return CSycles.pointcloud_primitive_type(Ptr);
        }
        public void AddPoint(float3 co, float radius, int shader) {
            CSycles.pointcloud_add_point(Ptr, co, radius, shader);
        }
        public override void Clear(bool preserve_shaders) {
            CSycles.pointcloud_clear(Ptr, preserve_shaders);
        }
        public long NumPoints() {
            return CSycles.pointcloud_num_points(Ptr);
        }
        public void Reserve(int numpoints) {
            CSycles.pointcloud_reserve(Ptr, numpoints);
        }
        public void CopyCenterToMotionStep(int motion_step) {
            CSycles.pointcloud_copy_center_to_motion_step(Ptr, motion_step);
        }

        public static IntPtr GetNodeType() {
            return CSycles.pointcloud_get_node_type();
        }

        public override void ApplyTransform(Transform tfm, bool apply_to_motion) {
            CSycles.pointcloud_apply_transform(Ptr, tfm, apply_to_motion);
        }

        public override void ComputeBounds() {
            CSycles.pointcloud_compute_bounds(Ptr);
        }

        public void Resize(int numpoints) {
            CSycles.pointcloud_resize(Ptr, numpoints);
        }
#region Setters

        internal override void SetFloatArray(string name, List<float> data)
        {
            switch(name) {
            case "radius":
                    /* pointcloud . {'datatype': 'FLOAT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'radius', 'ui_name': 'Radius'} */
                    {
                    CSycles.pointcloud_set_radius(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointCloud (setter)");
            }
        }

        internal override void SetIntArray(string name, List<int> data)
        {
            switch(name) {
            case "shader":
                    /* pointcloud . {'datatype': 'INT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'shader', 'ui_name': 'Shader'} */
                    {
                    CSycles.pointcloud_set_shader(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointCloud (setter)");
            }
        }

        internal override void SetPointArray(string name, List<float3> data)
        {
            switch(name) {
            case "points":
                    /* pointcloud . {'datatype': 'POINT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'points', 'ui_name': 'Points'} */
                    {
                    CSycles.pointcloud_set_points(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointCloud (setter)");
            }
        }

#endregion
#region Getters

        internal override List<float> GetFloatArray(string name)
        {
            switch(name) {
            case "radius":
                /* pointcloud . {'datatype': 'FLOAT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'radius', 'ui_name': 'Radius'} */
                {
                    return CSycles.pointcloud_get_radius(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointCloud (getter)");
            }
        }

        internal override List<int> GetIntArray(string name)
        {
            switch(name) {
            case "shader":
                /* pointcloud . {'datatype': 'INT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'shader', 'ui_name': 'Shader'} */
                {
                    return CSycles.pointcloud_get_shader(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointCloud (getter)");
            }
        }

        internal override List<float3> GetPointArray(string name)
        {
            switch(name) {
            case "points":
                /* pointcloud . {'datatype': 'POINT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'points', 'ui_name': 'Points'} */
                {
                    return CSycles.pointcloud_get_points(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type PointCloud (getter)");
            }
        }

#endregion
    }

}