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

    public class ObjectNodeInputs : NodeInputs
    {
        public StringNodeSocket LightGroup { get; private set; }
        public IntNodeSocket PassID { get; private set; }
        public UintNodeSocket LightSetIndex { get; private set; }
        public BoolNodeSocket UseHoldout { get; private set; }
        public Uint64NodeSocket LightSetMembership { get; private set; }
        public BoolNodeSocket HideonMissingMotion { get; private set; }
        public UintNodeSocket ShadowSetIndex { get; private set; }
        public PointNodeSocket DupliGenerated { get; private set; }
        public Uint64NodeSocket ShadowSetMembership { get; private set; }
        public Point2NodeSocket DupliUV { get; private set; }
        public BoolNodeSocket UseOCSFrame { get; private set; }
        public TransformArrayNodeSocket Motion { get; private set; }
        public TransformNodeSocket OCSFrame { get; private set; }
        public FloatNodeSocket ShadowTerminatorShadingOffset { get; private set; }
        public TransformNodeSocket OCSFrameNormal { get; private set; }
        public FloatNodeSocket ShadowTerminatorGeometryOffset { get; private set; }
        public StringNodeSocket AssetName { get; private set; }
        public ColorNodeSocket Color { get; private set; }
        public BoolNodeSocket ShadowCatcher { get; private set; }
        public BoolNodeSocket MeshLightNoCastShadow { get; private set; }
        public BoolNodeSocket CastShadowCaustics { get; private set; }
        public BoolNodeSocket ReceiveShadowCaustics { get; private set; }
        public BoolNodeSocket BakeTarget { get; private set; }
        public TransformNodeSocket Transform { get; private set; }
        public NodeNodeSocket ParticleSystem { get; private set; }
        public UintNodeSocket Visibility { get; private set; }
        public IntNodeSocket ParticleIndex { get; private set; }
        public NodeNodeSocket Shader { get; private set; }
        public FloatNodeSocket Alpha { get; private set; }
        public FloatNodeSocket AODistance { get; private set; }
        public NodeNodeSocket Geometry { get; private set; }
        public UintNodeSocket RandomID { get; private set; }

        public ObjectNodeInputs(Node parentNode)
        {
            LightGroup = new StringNodeSocket(parentNode, "Light Group", "lightgroup", true);
            AddSocket(LightGroup);
            PassID = new IntNodeSocket(parentNode, "Pass ID", "pass_id", true);
            AddSocket(PassID);
            LightSetIndex = new UintNodeSocket(parentNode, "Light Set Index", "receiver_light_set", true);
            AddSocket(LightSetIndex);
            UseHoldout = new BoolNodeSocket(parentNode, "Use Holdout", "use_holdout", true);
            AddSocket(UseHoldout);
            LightSetMembership = new Uint64NodeSocket(parentNode, "Light Set Membership", "light_set_membership", true);
            AddSocket(LightSetMembership);
            HideonMissingMotion = new BoolNodeSocket(parentNode, "Hide on Missing Motion", "hide_on_missing_motion", true);
            AddSocket(HideonMissingMotion);
            ShadowSetIndex = new UintNodeSocket(parentNode, "Shadow Set Index", "blocker_shadow_set", true);
            AddSocket(ShadowSetIndex);
            DupliGenerated = new PointNodeSocket(parentNode, "Dupli Generated", "dupli_generated", true);
            AddSocket(DupliGenerated);
            ShadowSetMembership = new Uint64NodeSocket(parentNode, "Shadow Set Membership", "shadow_set_membership", true);
            AddSocket(ShadowSetMembership);
            DupliUV = new Point2NodeSocket(parentNode, "Dupli UV", "dupli_uv", true);
            AddSocket(DupliUV);
            UseOCSFrame = new BoolNodeSocket(parentNode, "Use OCS Frame", "use_ocs_frame", true);
            AddSocket(UseOCSFrame);
            Motion = new TransformArrayNodeSocket(parentNode, "Motion", "motion", true);
            AddSocket(Motion);
            OCSFrame = new TransformNodeSocket(parentNode, "OCS Frame", "ocs_frame", true);
            AddSocket(OCSFrame);
            ShadowTerminatorShadingOffset = new FloatNodeSocket(parentNode, "Shadow Terminator Shading Offset", "shadow_terminator_shading_offset", true);
            AddSocket(ShadowTerminatorShadingOffset);
            OCSFrameNormal = new TransformNodeSocket(parentNode, "OCS Frame Normal", "ocs_frame_normal", true);
            AddSocket(OCSFrameNormal);
            ShadowTerminatorGeometryOffset = new FloatNodeSocket(parentNode, "Shadow Terminator Geometry Offset", "shadow_terminator_geometry_offset", true);
            AddSocket(ShadowTerminatorGeometryOffset);
            AssetName = new StringNodeSocket(parentNode, "Asset Name", "asset_name", true);
            AddSocket(AssetName);
            Color = new ColorNodeSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            ShadowCatcher = new BoolNodeSocket(parentNode, "Shadow Catcher", "is_shadow_catcher", true);
            AddSocket(ShadowCatcher);
            MeshLightNoCastShadow = new BoolNodeSocket(parentNode, "Mesh Light No Cast Shadow", "mesh_light_no_cast_shadow", true);
            AddSocket(MeshLightNoCastShadow);
            CastShadowCaustics = new BoolNodeSocket(parentNode, "Cast Shadow Caustics", "is_caustics_caster", true);
            AddSocket(CastShadowCaustics);
            ReceiveShadowCaustics = new BoolNodeSocket(parentNode, "Receive Shadow Caustics", "is_caustics_receiver", true);
            AddSocket(ReceiveShadowCaustics);
            BakeTarget = new BoolNodeSocket(parentNode, "Bake Target", "is_bake_target", true);
            AddSocket(BakeTarget);
            Transform = new TransformNodeSocket(parentNode, "Transform", "tfm", true);
            AddSocket(Transform);
            ParticleSystem = new NodeNodeSocket(parentNode, "Particle System", "particle_system", true);
            AddSocket(ParticleSystem);
            Visibility = new UintNodeSocket(parentNode, "Visibility", "visibility", true);
            AddSocket(Visibility);
            ParticleIndex = new IntNodeSocket(parentNode, "Particle Index", "particle_index", true);
            AddSocket(ParticleIndex);
            Shader = new NodeNodeSocket(parentNode, "Shader", "shader", true);
            AddSocket(Shader);
            Alpha = new FloatNodeSocket(parentNode, "Alpha", "alpha", true);
            AddSocket(Alpha);
            AODistance = new FloatNodeSocket(parentNode, "AO Distance", "ao_distance", true);
            AddSocket(AODistance);
            Geometry = new NodeNodeSocket(parentNode, "Geometry", "geometry", true);
            AddSocket(Geometry);
            RandomID = new UintNodeSocket(parentNode, "Random ID", "random_id", true);
            AddSocket(RandomID);
        }
    }
    [Node("object")]
    public class Object : Node
    {
        public ObjectNodeInputs ObjectNodeInputs { get; set; }
        public ObjectNodeInputs ins => ObjectNodeInputs;

        public Object() : this("a object node") { }

        public Object(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Object(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            ObjectNodeInputs = new ObjectNodeInputs(this);

        }
        public void TagUpdate(Scene scene)
        {
            CSycles.object_tag_update(this, scene);
        }
        public bool IntersectsVolume {
            get { return CSycles.object_get_intersects_volume(Ptr); }
            set { CSycles.object_set_intersects_volume(Ptr, value); }
        }
        public bool IsTraceable() {
            return CSycles.object_is_traceable(Ptr);
        }
        public float ComputeVolumeStepSize() {
            return CSycles.object_compute_volume_step_size(Ptr);
        }
        public int GetDeviceIndex() {
            return CSycles.object_get_device_index(Ptr);
        }
        public uint VisibilityForTracing() {
            return CSycles.object_visibility_for_tracing(Ptr);
        }
        public bool UsableAsLight() {
            return CSycles.object_usable_as_light(Ptr);
        }
        public float MotionTime(int step) {
            return CSycles.object_motion_time(Ptr, step);
        }
        public bool HasShadowLinking() {
            return CSycles.object_has_shadow_linking(Ptr);
        }
        public int MotionStep(float time) {
            return CSycles.object_motion_step(Ptr, time);
        }
        public bool UseMotion() {
            return CSycles.object_use_motion(Ptr);
        }
        public void ApplyTransform(bool apply_to_motion) {
            CSycles.object_apply_transform(Ptr, apply_to_motion);
        }
        public void ComputeBounds(bool motion_blur) {
            CSycles.object_compute_bounds(Ptr, motion_blur);
        }
        public int Index {
            get { return CSycles.object_get_index(Ptr); }
            set { CSycles.object_set_index(Ptr, value); }
        }
        public bool HasLightLinking() {
            return CSycles.object_has_light_linking(Ptr);
        }
        public static IntPtr GetNodeType() {
            return CSycles.object_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "shadow_terminator_shading_offset":
                    /* object . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'shadow_terminator_shading_offset', 'ui_name': 'Shadow Terminator Shading Offset'} */
                    {
                    CSycles.object_set_shadow_terminator_shading_offset(this.Ptr, data);
                    }
                    break;
            case "shadow_terminator_geometry_offset":
                    /* object . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'shadow_terminator_geometry_offset', 'ui_name': 'Shadow Terminator Geometry Offset'} */
                    {
                    CSycles.object_set_shadow_terminator_geometry_offset(this.Ptr, data);
                    }
                    break;
            case "alpha":
                    /* object . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha', 'ui_name': 'Alpha'} */
                    {
                    CSycles.object_set_alpha(this.Ptr, data);
                    }
                    break;
            case "ao_distance":
                    /* object . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ao_distance', 'ui_name': 'AO Distance'} */
                    {
                    CSycles.object_set_ao_distance(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "dupli_generated":
                    /* object . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'dupli_generated', 'ui_name': 'Dupli Generated'} */
                    {
                    CSycles.object_set_dupli_generated(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetPoint2(string name, float2 data)
        {
            switch(name) {
            case "dupli_uv":
                    /* object . {'datatype': 'POINT2', 'default_value': 'zero_float2()', 'default_value_type': 'float2', 'is_input': True, 'member_name': 'dupli_uv', 'ui_name': 'Dupli UV'} */
                    {
                    CSycles.object_set_dupli_uv(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* object . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.object_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_holdout":
                    /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_holdout', 'ui_name': 'Use Holdout'} */
                    {
                    CSycles.object_set_use_holdout(this.Ptr, data);
                    }
                    break;
            case "hide_on_missing_motion":
                    /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'hide_on_missing_motion', 'ui_name': 'Hide on Missing Motion'} */
                    {
                    CSycles.object_set_hide_on_missing_motion(this.Ptr, data);
                    }
                    break;
            case "use_ocs_frame":
                    /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_ocs_frame', 'ui_name': 'Use OCS Frame'} */
                    {
                    CSycles.object_set_use_ocs_frame(this.Ptr, data);
                    }
                    break;
            case "is_shadow_catcher":
                    /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_shadow_catcher', 'ui_name': 'Shadow Catcher'} */
                    {
                    CSycles.object_set_is_shadow_catcher(this.Ptr, data);
                    }
                    break;
            case "mesh_light_no_cast_shadow":
                    /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'mesh_light_no_cast_shadow', 'ui_name': 'Mesh Light No Cast Shadow'} */
                    {
                    CSycles.object_set_mesh_light_no_cast_shadow(this.Ptr, data);
                    }
                    break;
            case "is_caustics_caster":
                    /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_caustics_caster', 'ui_name': 'Cast Shadow Caustics'} */
                    {
                    CSycles.object_set_is_caustics_caster(this.Ptr, data);
                    }
                    break;
            case "is_caustics_receiver":
                    /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_caustics_receiver', 'ui_name': 'Receive Shadow Caustics'} */
                    {
                    CSycles.object_set_is_caustics_receiver(this.Ptr, data);
                    }
                    break;
            case "is_bake_target":
                    /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_bake_target', 'ui_name': 'Bake Target'} */
                    {
                    CSycles.object_set_is_bake_target(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "pass_id":
                    /* object . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'pass_id', 'ui_name': 'Pass ID'} */
                    {
                    CSycles.object_set_pass_id(this.Ptr, data);
                    }
                    break;
            case "particle_index":
                    /* object . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'particle_index', 'ui_name': 'Particle Index'} */
                    {
                    CSycles.object_set_particle_index(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetUint(string name, uint data)
        {
            switch(name) {
            case "receiver_light_set":
                    /* object . {'datatype': 'UINT', 'default_value': '0', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'receiver_light_set', 'ui_name': 'Light Set Index'} */
                    {
                    CSycles.object_set_receiver_light_set(this.Ptr, data);
                    }
                    break;
            case "blocker_shadow_set":
                    /* object . {'datatype': 'UINT', 'default_value': '0', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'blocker_shadow_set', 'ui_name': 'Shadow Set Index'} */
                    {
                    CSycles.object_set_blocker_shadow_set(this.Ptr, data);
                    }
                    break;
            case "visibility":
                    /* object . {'datatype': 'UINT', 'default_value': 'uint.MaxValue', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'visibility', 'ui_name': 'Visibility'} */
                    {
                    CSycles.object_set_visibility(this.Ptr, data);
                    }
                    break;
            case "random_id":
                    /* object . {'datatype': 'UINT', 'default_value': '0', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'random_id', 'ui_name': 'Random ID'} */
                    {
                    CSycles.object_set_random_id(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetUint64(string name, ulong data)
        {
            switch(name) {
            case "light_set_membership":
                    /* object . {'datatype': 'UINT64', 'default_value': 'ulong.MaxValue', 'default_value_type': 'uint64_t', 'is_input': True, 'member_name': 'light_set_membership', 'ui_name': 'Light Set Membership'} */
                    {
                    CSycles.object_set_light_set_membership(this.Ptr, data);
                    }
                    break;
            case "shadow_set_membership":
                    /* object . {'datatype': 'UINT64', 'default_value': 'ulong.MaxValue', 'default_value_type': 'uint64_t', 'is_input': True, 'member_name': 'shadow_set_membership', 'ui_name': 'Shadow Set Membership'} */
                    {
                    CSycles.object_set_shadow_set_membership(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "lightgroup":
                    /* object . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'lightgroup', 'ui_name': 'Light Group'} */
                    {
                    CSycles.object_set_lightgroup(this.Ptr, data);
                    }
                    break;
            case "asset_name":
                    /* object . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'asset_name', 'ui_name': 'Asset Name'} */
                    {
                    CSycles.object_set_asset_name(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetTransform(string name, Transform data)
        {
            switch(name) {
            case "ocs_frame":
                    /* object . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'ocs_frame', 'ui_name': 'OCS Frame'} */
                    {
                    CSycles.object_set_ocs_frame(this.Ptr, data);
                    }
                    break;
            case "ocs_frame_normal":
                    /* object . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'ocs_frame_normal', 'ui_name': 'OCS Frame Normal'} */
                    {
                    CSycles.object_set_ocs_frame_normal(this.Ptr, data);
                    }
                    break;
            case "tfm":
                    /* object . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'tfm', 'ui_name': 'Transform'} */
                    {
                    CSycles.object_set_tfm(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetNode(string name, IntPtr data)
        {
            switch(name) {
            case "particle_system":
                    /* object . {'datatype': 'NODE', 'default_value': '', 'default_value_type': '', 'is_input': True, 'member_name': 'particle_system', 'ui_name': 'Particle System'} */
                    {
                    CSycles.object_set_particle_system(this.Ptr, data);
                    }
                    break;
            case "shader":
                    /* object . {'datatype': 'NODE', 'default_value': '', 'default_value_type': '', 'is_input': True, 'member_name': 'shader', 'ui_name': 'Shader'} */
                    {
                    CSycles.object_set_shader(this.Ptr, data);
                    }
                    break;
            case "geometry":
                    /* object . {'datatype': 'NODE', 'default_value': '', 'default_value_type': '', 'is_input': True, 'member_name': 'geometry', 'ui_name': 'Geometry'} */
                    {
                    CSycles.object_set_geometry(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

        internal override void SetTransformArray(string name, List<Transform> data)
        {
            switch(name) {
            case "motion":
                    /* object . {'datatype': 'TRANSFORM_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'motion', 'ui_name': 'Motion'} */
                    {
                    CSycles.object_set_motion(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "shadow_terminator_shading_offset":
                /* object . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'shadow_terminator_shading_offset', 'ui_name': 'Shadow Terminator Shading Offset'} */
                {
                    return CSycles.object_get_shadow_terminator_shading_offset(this.Ptr);
                }
            case "shadow_terminator_geometry_offset":
                /* object . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'shadow_terminator_geometry_offset', 'ui_name': 'Shadow Terminator Geometry Offset'} */
                {
                    return CSycles.object_get_shadow_terminator_geometry_offset(this.Ptr);
                }
            case "alpha":
                /* object . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha', 'ui_name': 'Alpha'} */
                {
                    return CSycles.object_get_alpha(this.Ptr);
                }
            case "ao_distance":
                /* object . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ao_distance', 'ui_name': 'AO Distance'} */
                {
                    return CSycles.object_get_ao_distance(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "dupli_generated":
                /* object . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'dupli_generated', 'ui_name': 'Dupli Generated'} */
                {
                    return CSycles.object_get_dupli_generated(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override float2 GetPoint2(string name)
        {
            switch(name) {
            case "dupli_uv":
                /* object . {'datatype': 'POINT2', 'default_value': 'zero_float2()', 'default_value_type': 'float2', 'is_input': True, 'member_name': 'dupli_uv', 'ui_name': 'Dupli UV'} */
                {
                    return CSycles.object_get_dupli_uv(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* object . {'datatype': 'COLOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.object_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_holdout":
                /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_holdout', 'ui_name': 'Use Holdout'} */
                {
                    return CSycles.object_get_use_holdout(this.Ptr);
                }
            case "hide_on_missing_motion":
                /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'hide_on_missing_motion', 'ui_name': 'Hide on Missing Motion'} */
                {
                    return CSycles.object_get_hide_on_missing_motion(this.Ptr);
                }
            case "use_ocs_frame":
                /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_ocs_frame', 'ui_name': 'Use OCS Frame'} */
                {
                    return CSycles.object_get_use_ocs_frame(this.Ptr);
                }
            case "is_shadow_catcher":
                /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_shadow_catcher', 'ui_name': 'Shadow Catcher'} */
                {
                    return CSycles.object_get_is_shadow_catcher(this.Ptr);
                }
            case "mesh_light_no_cast_shadow":
                /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'mesh_light_no_cast_shadow', 'ui_name': 'Mesh Light No Cast Shadow'} */
                {
                    return CSycles.object_get_mesh_light_no_cast_shadow(this.Ptr);
                }
            case "is_caustics_caster":
                /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_caustics_caster', 'ui_name': 'Cast Shadow Caustics'} */
                {
                    return CSycles.object_get_is_caustics_caster(this.Ptr);
                }
            case "is_caustics_receiver":
                /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_caustics_receiver', 'ui_name': 'Receive Shadow Caustics'} */
                {
                    return CSycles.object_get_is_caustics_receiver(this.Ptr);
                }
            case "is_bake_target":
                /* object . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_bake_target', 'ui_name': 'Bake Target'} */
                {
                    return CSycles.object_get_is_bake_target(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "pass_id":
                /* object . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'pass_id', 'ui_name': 'Pass ID'} */
                {
                    return CSycles.object_get_pass_id(this.Ptr);
                }
            case "particle_index":
                /* object . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'particle_index', 'ui_name': 'Particle Index'} */
                {
                    return CSycles.object_get_particle_index(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override uint GetUint(string name)
        {
            switch(name) {
            case "receiver_light_set":
                /* object . {'datatype': 'UINT', 'default_value': '0', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'receiver_light_set', 'ui_name': 'Light Set Index'} */
                {
                    return CSycles.object_get_receiver_light_set(this.Ptr);
                }
            case "blocker_shadow_set":
                /* object . {'datatype': 'UINT', 'default_value': '0', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'blocker_shadow_set', 'ui_name': 'Shadow Set Index'} */
                {
                    return CSycles.object_get_blocker_shadow_set(this.Ptr);
                }
            case "visibility":
                /* object . {'datatype': 'UINT', 'default_value': 'uint.MaxValue', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'visibility', 'ui_name': 'Visibility'} */
                {
                    return CSycles.object_get_visibility(this.Ptr);
                }
            case "random_id":
                /* object . {'datatype': 'UINT', 'default_value': '0', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'random_id', 'ui_name': 'Random ID'} */
                {
                    return CSycles.object_get_random_id(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override ulong GetUint64(string name)
        {
            switch(name) {
            case "light_set_membership":
                /* object . {'datatype': 'UINT64', 'default_value': 'ulong.MaxValue', 'default_value_type': 'uint64_t', 'is_input': True, 'member_name': 'light_set_membership', 'ui_name': 'Light Set Membership'} */
                {
                    return CSycles.object_get_light_set_membership(this.Ptr);
                }
            case "shadow_set_membership":
                /* object . {'datatype': 'UINT64', 'default_value': 'ulong.MaxValue', 'default_value_type': 'uint64_t', 'is_input': True, 'member_name': 'shadow_set_membership', 'ui_name': 'Shadow Set Membership'} */
                {
                    return CSycles.object_get_shadow_set_membership(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "lightgroup":
                /* object . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'lightgroup', 'ui_name': 'Light Group'} */
                {
                    return CSycles.object_get_lightgroup(this.Ptr);
                }
            case "asset_name":
                /* object . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'asset_name', 'ui_name': 'Asset Name'} */
                {
                    return CSycles.object_get_asset_name(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override Transform GetTransform(string name)
        {
            switch(name) {
            case "ocs_frame":
                /* object . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'ocs_frame', 'ui_name': 'OCS Frame'} */
                {
                    return CSycles.object_get_ocs_frame(this.Ptr);
                }
            case "ocs_frame_normal":
                /* object . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'ocs_frame_normal', 'ui_name': 'OCS Frame Normal'} */
                {
                    return CSycles.object_get_ocs_frame_normal(this.Ptr);
                }
            case "tfm":
                /* object . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'tfm', 'ui_name': 'Transform'} */
                {
                    return CSycles.object_get_tfm(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override IntPtr GetNode(string name)
        {
            switch(name) {
            case "particle_system":
                /* object . {'datatype': 'NODE', 'default_value': '', 'default_value_type': '', 'is_input': True, 'member_name': 'particle_system', 'ui_name': 'Particle System'} */
                {
                    return CSycles.object_get_particle_system(this.Ptr);
                }
            case "shader":
                /* object . {'datatype': 'NODE', 'default_value': '', 'default_value_type': '', 'is_input': True, 'member_name': 'shader', 'ui_name': 'Shader'} */
                {
                    return CSycles.object_get_shader(this.Ptr);
                }
            case "geometry":
                /* object . {'datatype': 'NODE', 'default_value': '', 'default_value_type': '', 'is_input': True, 'member_name': 'geometry', 'ui_name': 'Geometry'} */
                {
                    return CSycles.object_get_geometry(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

        internal override List<Transform> GetTransformArray(string name)
        {
            switch(name) {
            case "motion":
                /* object . {'datatype': 'TRANSFORM_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'motion', 'ui_name': 'Motion'} */
                {
                    return CSycles.object_get_motion(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Object (getter)");
            }
        }

#endregion
    }

}