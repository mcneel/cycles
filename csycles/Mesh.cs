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

    public class MeshNodeInputs : NodeInputs
    {
        public PointArrayNodeSocket Vertices { get; private set; }
        public IntArrayNodeSocket Triangles { get; private set; }
        public IntArrayNodeSocket Shader { get; private set; }
        public BooleanArrayNodeSocket Smooth { get; private set; }

        public MeshNodeInputs(Node parentNode)
        {
            Vertices = new PointArrayNodeSocket(parentNode, "Vertices", "verts", true);
            AddSocket(Vertices);
            Triangles = new IntArrayNodeSocket(parentNode, "Triangles", "triangles", true);
            AddSocket(Triangles);
            Shader = new IntArrayNodeSocket(parentNode, "Shader", "shader", true);
            AddSocket(Shader);
            Smooth = new BooleanArrayNodeSocket(parentNode, "Smooth", "smooth", true);
            AddSocket(Smooth);
        }
    }
    public class Mesh : Geometry
    {
        public enum MeshSubdivisionBoundaryInterpolation : uint {
            None = ccl.Mesh_SubdivisionBoundaryInterpolation.SUBDIVISION_BOUNDARY_NONE,
            EdgeOnly = ccl.Mesh_SubdivisionBoundaryInterpolation.SUBDIVISION_BOUNDARY_EDGE_ONLY,
            EdgeAndCorner = ccl.Mesh_SubdivisionBoundaryInterpolation.SUBDIVISION_BOUNDARY_EDGE_AND_CORNER,
        }
        public enum MeshSubdivisionFvarInterpolation : uint {
            None = ccl.Mesh_SubdivisionFVarInterpolation.SUBDIVISION_FVAR_LINEAR_NONE,
            CornersOnly = ccl.Mesh_SubdivisionFVarInterpolation.SUBDIVISION_FVAR_LINEAR_CORNERS_ONLY,
            CornersPlus1 = ccl.Mesh_SubdivisionFVarInterpolation.SUBDIVISION_FVAR_LINEAR_CORNERS_PLUS1,
            CornersPlus2 = ccl.Mesh_SubdivisionFVarInterpolation.SUBDIVISION_FVAR_LINEAR_CORNERS_PLUS2,
            Boundaries = ccl.Mesh_SubdivisionFVarInterpolation.SUBDIVISION_FVAR_LINEAR_BOUNDARIES,
            All = ccl.Mesh_SubdivisionFVarInterpolation.SUBDIVISION_FVAR_LINEAR_ALL,
        }
        public enum MeshSubdivisionType : uint {
            None = ccl.Mesh_SubdivisionType.SUBDIVISION_NONE,
            Linear = ccl.Mesh_SubdivisionType.SUBDIVISION_LINEAR,
            CatmullClark = ccl.Mesh_SubdivisionType.SUBDIVISION_CATMULL_CLARK,
        }
        public MeshNodeInputs MeshNodeInputs { get; set; }
        public MeshNodeInputs ins => MeshNodeInputs;

        public Mesh() : this("a mesh node") { }

        public Mesh(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Mesh(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            MeshNodeInputs = new MeshNodeInputs(this);

        }
        public void SetUvs(List<float2> uvs, string uvmap)
        {
            CSycles.mesh_set_vertex_uvs(this, uvs, uvmap);
        }

        public void SetVertexNormals(List<float3> vns)
        {
            CSycles.mesh_set_vertex_normals(this, vns);
        }

        public void ResizeMesh(int numverts, int numtris) {
            CSycles.mesh_resize_mesh(Ptr, numverts, numtris);
        }

        public void Tessellate(SubdParams parameters) {
            CSycles.mesh_tessellate(Ptr, parameters);
        }

        public override bool HasMotionBlur() {
            return CSycles.mesh_has_motion_blur(Ptr);
        }

        public void ReserveMesh(int numverts, int numtris) {
            CSycles.mesh_reserve_mesh(Ptr, numverts, numtris);
        }
        public void AddTriangle(int v0, int v1, int v2, int shader, bool smooth) {
            CSycles.mesh_add_triangle(Ptr, v0, v1, v2, shader, smooth);
        }

        public void PackShaders(IntPtr scene, IntPtr shader) {
            CSycles.mesh_pack_shaders(Ptr, scene, shader);
        }

        public long VertOffset {
            get { return CSycles.mesh_get_vert_offset(Ptr); }
            set { CSycles.mesh_set_vert_offset(Ptr, value); }
        }

        public void AddVertexCrease(int v, float weight) {
            CSycles.mesh_add_vertex_crease(Ptr, v, weight);
        }

        public bool NeedTesselation() {
            return CSycles.mesh_need_tesselation(Ptr);
        }

        public void PackVerts(IntPtr tri_verts, IntPtr tri_vindex) {
            CSycles.mesh_pack_verts(Ptr, tri_verts, tri_vindex);
        }
        public override void ComputeBounds() {
            CSycles.mesh_compute_bounds(Ptr);
        }

        public long FaceOffset {
            get { return CSycles.mesh_get_face_offset(Ptr); }
            set { CSycles.mesh_set_face_offset(Ptr, value); }
        }

        public void PackNormals(IntPtr vnormal) {
            CSycles.mesh_pack_normals(Ptr, vnormal);
        }

        public void AddVertexSlow(float3 P) {
            CSycles.mesh_add_vertex_slow(Ptr, P);
        }
        public long NumTriangles() {
            return CSycles.mesh_num_triangles(Ptr);
        }
        public override void Clear(bool preserve_shaders) {
            CSycles.mesh_clear(Ptr, preserve_shaders);
        }
        public void AddUndisplaced() {
            CSycles.mesh_add_undisplaced(Ptr);
        }

        public long CornerOffset {
            get { return CSycles.mesh_get_corner_offset(Ptr); }
            set { CSycles.mesh_set_corner_offset(Ptr, value); }
        }

        public void AddVertexNormals() {
            CSycles.mesh_add_vertex_normals(Ptr);
        }

        public void AddEdgeCrease(int v0, int v1, float weight) {
            CSycles.mesh_add_edge_crease(Ptr, v0, v1, weight);
        }
        public override PrimitiveType PrimitiveType() {
            return CSycles.mesh_primitive_type(Ptr);
        }
        public void AddVertex(float3 P) {
            CSycles.mesh_add_vertex(Ptr, P);
        }

        public void CopyCenterToMotionStep(int motion_step) {
            CSycles.mesh_copy_center_to_motion_step(Ptr, motion_step);
        }

        public override void ApplyTransform(Transform tfm, bool apply_to_motion) {
            CSycles.mesh_apply_transform(Ptr, tfm, apply_to_motion);
        }

        public static IntPtr GetNodeType() {
            return CSycles.mesh_get_node_type();
        }
#region Setters

        internal override void SetIntArray(string name, List<int> data)
        {
            switch(name) {
            case "triangles":
                    /* mesh . {'datatype': 'INT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'triangles', 'ui_name': 'Triangles'} */
                    {
                    CSycles.mesh_set_triangles(this.Ptr, data);
                    }
                    break;
            case "shader":
                    /* mesh . {'datatype': 'INT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'shader', 'ui_name': 'Shader'} */
                    {
                    CSycles.mesh_set_shader(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Mesh (setter)");
            }
        }

        internal override void SetBooleanArray(string name, List<bool> data)
        {
            switch(name) {
            case "smooth":
                    /* mesh . {'datatype': 'BOOLEAN_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'smooth', 'ui_name': 'Smooth'} */
                    {
                    CSycles.mesh_set_smooth(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Mesh (setter)");
            }
        }

        internal override void SetPointArray(string name, List<float3> data)
        {
            switch(name) {
            case "verts":
                    /* mesh . {'datatype': 'POINT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'verts', 'ui_name': 'Vertices'} */
                    {
                    CSycles.mesh_set_verts(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Mesh (setter)");
            }
        }

#endregion
#region Getters

        internal override List<int> GetIntArray(string name)
        {
            switch(name) {
            case "triangles":
                /* mesh . {'datatype': 'INT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'triangles', 'ui_name': 'Triangles'} */
                {
                    return CSycles.mesh_get_triangles(this.Ptr);
                }
            case "shader":
                /* mesh . {'datatype': 'INT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'shader', 'ui_name': 'Shader'} */
                {
                    return CSycles.mesh_get_shader(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Mesh (getter)");
            }
        }

        internal override List<bool> GetBooleanArray(string name)
        {
            switch(name) {
            case "smooth":
                /* mesh . {'datatype': 'BOOLEAN_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'smooth', 'ui_name': 'Smooth'} */
                {
                    return CSycles.mesh_get_smooth(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Mesh (getter)");
            }
        }

        internal override List<float3> GetPointArray(string name)
        {
            switch(name) {
            case "verts":
                /* mesh . {'datatype': 'POINT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'verts', 'ui_name': 'Vertices'} */
                {
                    return CSycles.mesh_get_verts(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Mesh (getter)");
            }
        }

#endregion
    }

}