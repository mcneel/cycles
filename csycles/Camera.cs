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

    public class CameraNodeInputs : NodeInputs
    {
        public EnumNodeSocket StereoEye { get; private set; }
        public EnumNodeSocket RollingShutterType { get; private set; }
        public BoolNodeSocket UseSphericalStereo { get; private set; }
        public FloatNodeSocket InterocularDistance { get; private set; }
        public FloatNodeSocket ShutterTime { get; private set; }
        public EnumNodeSocket MotionPosition { get; private set; }
        public FloatNodeSocket ConvergenceDistance { get; private set; }
        public TransformNodeSocket Matrix { get; private set; }
        public BoolNodeSocket UsePoleMerge { get; private set; }
        public FloatNodeSocket OffscreenDicingScale { get; private set; }
        public TransformArrayNodeSocket Motion { get; private set; }
        public FloatNodeSocket PoleMergeAngleFrom { get; private set; }
        public FloatNodeSocket LatitudeMax { get; private set; }
        public EnumNodeSocket PanoramaType { get; private set; }
        public IntNodeSocket FullWidth { get; private set; }
        public FloatNodeSocket PoleMergeAngleTo { get; private set; }
        public EnumNodeSocket Type { get; private set; }
        public FloatNodeSocket FOV { get; private set; }
        public IntNodeSocket FullHeight { get; private set; }
        public FloatNodeSocket SensorWidth { get; private set; }
        public FloatNodeSocket FOVPre { get; private set; }
        public BoolNodeSocket UsePerspectiveMotion { get; private set; }
        public FloatNodeSocket SensorHeight { get; private set; }
        public FloatNodeSocket FOVPost { get; private set; }
        public FloatNodeSocket FocalDistance { get; private set; }
        public FloatNodeSocket ApertureRatio { get; private set; }
        public FloatNodeSocket FisheyeLens { get; private set; }
        public FloatNodeSocket NearClip { get; private set; }
        public FloatNodeSocket FisheyePolynomialK0 { get; private set; }
        public FloatArrayNodeSocket ShutterCurve { get; private set; }
        public FloatNodeSocket ApertureSize { get; private set; }
        public UintNodeSocket Blades { get; private set; }
        public FloatNodeSocket FisheyePolynomialK1 { get; private set; }
        public FloatNodeSocket FarClip { get; private set; }
        public FloatNodeSocket FisheyeFOV { get; private set; }
        public FloatNodeSocket FisheyePolynomialK2 { get; private set; }
        public FloatNodeSocket FisheyePolynomialK3 { get; private set; }
        public FloatNodeSocket RollingShutterDuration { get; private set; }
        public FloatNodeSocket LongitudeMax { get; private set; }
        public FloatNodeSocket FisheyePolynomialK4 { get; private set; }
        public FloatNodeSocket CentralCylindricalRangeUMin { get; private set; }
        public FloatNodeSocket LatitudeMin { get; private set; }
        public FloatNodeSocket BladesRotation { get; private set; }
        public FloatNodeSocket CentralCylindricalRangeUMax { get; private set; }
        public FloatNodeSocket LongitudeMin { get; private set; }
        public FloatNodeSocket CentralCylindricalRangeVMin { get; private set; }
        public FloatNodeSocket CentralCylindricalRangeVMax { get; private set; }

        public CameraNodeInputs(Node parentNode)
        {
            StereoEye = new EnumNodeSocket(parentNode, "Stereo Eye", "stereo_eye", true);
            AddSocket(StereoEye);
            RollingShutterType = new EnumNodeSocket(parentNode, "Rolling Shutter Type", "rolling_shutter_type", true);
            AddSocket(RollingShutterType);
            UseSphericalStereo = new BoolNodeSocket(parentNode, "Use Spherical Stereo", "use_spherical_stereo", true);
            AddSocket(UseSphericalStereo);
            InterocularDistance = new FloatNodeSocket(parentNode, "Interocular Distance", "interocular_distance", true);
            AddSocket(InterocularDistance);
            ShutterTime = new FloatNodeSocket(parentNode, "Shutter Time", "shuttertime", true);
            AddSocket(ShutterTime);
            MotionPosition = new EnumNodeSocket(parentNode, "Motion Position", "motion_position", true);
            AddSocket(MotionPosition);
            ConvergenceDistance = new FloatNodeSocket(parentNode, "Convergence Distance", "convergence_distance", true);
            AddSocket(ConvergenceDistance);
            Matrix = new TransformNodeSocket(parentNode, "Matrix", "matrix", true);
            AddSocket(Matrix);
            UsePoleMerge = new BoolNodeSocket(parentNode, "Use Pole Merge", "use_pole_merge", true);
            AddSocket(UsePoleMerge);
            OffscreenDicingScale = new FloatNodeSocket(parentNode, "Offscreen Dicing Scale", "offscreen_dicing_scale", true);
            AddSocket(OffscreenDicingScale);
            Motion = new TransformArrayNodeSocket(parentNode, "Motion", "motion", true);
            AddSocket(Motion);
            PoleMergeAngleFrom = new FloatNodeSocket(parentNode, "Pole Merge Angle From", "pole_merge_angle_from", true);
            AddSocket(PoleMergeAngleFrom);
            LatitudeMax = new FloatNodeSocket(parentNode, "Latitude Max", "latitude_max", true);
            AddSocket(LatitudeMax);
            PanoramaType = new EnumNodeSocket(parentNode, "Panorama Type", "panorama_type", true);
            AddSocket(PanoramaType);
            FullWidth = new IntNodeSocket(parentNode, "Full Width", "full_width", true);
            AddSocket(FullWidth);
            PoleMergeAngleTo = new FloatNodeSocket(parentNode, "Pole Merge Angle To", "pole_merge_angle_to", true);
            AddSocket(PoleMergeAngleTo);
            Type = new EnumNodeSocket(parentNode, "Type", "camera_type", true);
            AddSocket(Type);
            FOV = new FloatNodeSocket(parentNode, "FOV", "fov", true);
            AddSocket(FOV);
            FullHeight = new IntNodeSocket(parentNode, "Full Height", "full_height", true);
            AddSocket(FullHeight);
            SensorWidth = new FloatNodeSocket(parentNode, "Sensor Width", "sensorwidth", true);
            AddSocket(SensorWidth);
            FOVPre = new FloatNodeSocket(parentNode, "FOV Pre", "fov_pre", true);
            AddSocket(FOVPre);
            UsePerspectiveMotion = new BoolNodeSocket(parentNode, "Use Perspective Motion", "use_perspective_motion", true);
            AddSocket(UsePerspectiveMotion);
            SensorHeight = new FloatNodeSocket(parentNode, "Sensor Height", "sensorheight", true);
            AddSocket(SensorHeight);
            FOVPost = new FloatNodeSocket(parentNode, "FOV Post", "fov_post", true);
            AddSocket(FOVPost);
            FocalDistance = new FloatNodeSocket(parentNode, "Focal Distance", "focaldistance", true);
            AddSocket(FocalDistance);
            ApertureRatio = new FloatNodeSocket(parentNode, "Aperture Ratio", "aperture_ratio", true);
            AddSocket(ApertureRatio);
            FisheyeLens = new FloatNodeSocket(parentNode, "Fisheye Lens", "fisheye_lens", true);
            AddSocket(FisheyeLens);
            NearClip = new FloatNodeSocket(parentNode, "Near Clip", "nearclip", true);
            AddSocket(NearClip);
            FisheyePolynomialK0 = new FloatNodeSocket(parentNode, "Fisheye Polynomial K0", "fisheye_polynomial_k0", true);
            AddSocket(FisheyePolynomialK0);
            ShutterCurve = new FloatArrayNodeSocket(parentNode, "Shutter Curve", "shutter_curve", true);
            AddSocket(ShutterCurve);
            ApertureSize = new FloatNodeSocket(parentNode, "Aperture Size", "aperturesize", true);
            AddSocket(ApertureSize);
            Blades = new UintNodeSocket(parentNode, "Blades", "blades", true);
            AddSocket(Blades);
            FisheyePolynomialK1 = new FloatNodeSocket(parentNode, "Fisheye Polynomial K1", "fisheye_polynomial_k1", true);
            AddSocket(FisheyePolynomialK1);
            FarClip = new FloatNodeSocket(parentNode, "Far Clip", "farclip", true);
            AddSocket(FarClip);
            FisheyeFOV = new FloatNodeSocket(parentNode, "Fisheye FOV", "fisheye_fov", true);
            AddSocket(FisheyeFOV);
            FisheyePolynomialK2 = new FloatNodeSocket(parentNode, "Fisheye Polynomial K2", "fisheye_polynomial_k2", true);
            AddSocket(FisheyePolynomialK2);
            FisheyePolynomialK3 = new FloatNodeSocket(parentNode, "Fisheye Polynomial K3", "fisheye_polynomial_k3", true);
            AddSocket(FisheyePolynomialK3);
            RollingShutterDuration = new FloatNodeSocket(parentNode, "Rolling Shutter Duration", "rolling_shutter_duration", true);
            AddSocket(RollingShutterDuration);
            LongitudeMax = new FloatNodeSocket(parentNode, "Longitude Max", "longitude_max", true);
            AddSocket(LongitudeMax);
            FisheyePolynomialK4 = new FloatNodeSocket(parentNode, "Fisheye Polynomial K4", "fisheye_polynomial_k4", true);
            AddSocket(FisheyePolynomialK4);
            CentralCylindricalRangeUMin = new FloatNodeSocket(parentNode, "Central Cylindrical Range U Min", "central_cylindrical_range_u_min", true);
            AddSocket(CentralCylindricalRangeUMin);
            LatitudeMin = new FloatNodeSocket(parentNode, "Latitude Min", "latitude_min", true);
            AddSocket(LatitudeMin);
            BladesRotation = new FloatNodeSocket(parentNode, "Blades Rotation", "bladesrotation", true);
            AddSocket(BladesRotation);
            CentralCylindricalRangeUMax = new FloatNodeSocket(parentNode, "Central Cylindrical Range U Max", "central_cylindrical_range_u_max", true);
            AddSocket(CentralCylindricalRangeUMax);
            LongitudeMin = new FloatNodeSocket(parentNode, "Longitude Min", "longitude_min", true);
            AddSocket(LongitudeMin);
            CentralCylindricalRangeVMin = new FloatNodeSocket(parentNode, "Central Cylindrical Range V Min", "central_cylindrical_range_v_min", true);
            AddSocket(CentralCylindricalRangeVMin);
            CentralCylindricalRangeVMax = new FloatNodeSocket(parentNode, "Central Cylindrical Range V Max", "central_cylindrical_range_v_max", true);
            AddSocket(CentralCylindricalRangeVMax);
        }
    }
    [Node("camera")]
    public class Camera : Node
    {
        public enum CameraMotionPosition : uint {
            Start = ccl.MotionPosition.MOTION_POSITION_START,
            Center = ccl.MotionPosition.MOTION_POSITION_CENTER,
            End = ccl.MotionPosition.MOTION_POSITION_END,
        }
        public enum CameraPanoramaType : uint {
            Equirectangular = ccl.PanoramaType.PANORAMA_EQUIRECTANGULAR,
            FisheyeEquidistant = ccl.PanoramaType.PANORAMA_FISHEYE_EQUIDISTANT,
            FisheyeEquisolid = ccl.PanoramaType.PANORAMA_FISHEYE_EQUISOLID,
            Mirrorball = ccl.PanoramaType.PANORAMA_MIRRORBALL,
            FisheyeLensPolynomial = ccl.PanoramaType.PANORAMA_FISHEYE_LENS_POLYNOMIAL,
            EquiangularCubemapFace = ccl.PanoramaType.PANORAMA_EQUIANGULAR_CUBEMAP_FACE,
            PanoramaCentralCylindrical = ccl.PanoramaType.PANORAMA_CENTRAL_CYLINDRICAL,
        }
        public enum CameraRollingShutterType : uint {
            None = ccl.Camera_RollingShutterType.ROLLING_SHUTTER_NONE,
            Top = ccl.Camera_RollingShutterType.ROLLING_SHUTTER_TOP,
        }
        public enum CameraStereoEye : uint {
            None = ccl.Camera_StereoEye.STEREO_NONE,
            Left = ccl.Camera_StereoEye.STEREO_LEFT,
            Right = ccl.Camera_StereoEye.STEREO_RIGHT,
        }
        public enum CameraType : uint {
            Perspective = ccl.CameraType.CAMERA_PERSPECTIVE,
            Orthograph = ccl.CameraType.CAMERA_ORTHOGRAPHIC,
            Panorama = ccl.CameraType.CAMERA_PANORAMA,
        }
        public CameraNodeInputs CameraNodeInputs { get; set; }
        public CameraNodeInputs ins => CameraNodeInputs;

        public Camera() : this("a camera node") { }

        public Camera(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Camera(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            CameraNodeInputs = new CameraNodeInputs(this);

        }
        public void TagUpdate(Scene scene)
        {
            CSycles.camera_tag_update(this, scene);
        }

        public float GetViewplaneTop() {
            return CSycles.camera_get_viewplane_top(Ptr);
        }
        public float3 FullDy {
            get { return CSycles.camera_get_full_dy(Ptr); }
            set { CSycles.camera_set_full_dy(Ptr, value); }
        }
        public float WorldToRasterSize(float3 P) {
            return CSycles.camera_world_to_raster_size(Ptr, P);
        }

        public long ShutterTableOffset {
            get { return CSycles.camera_get_shutter_table_offset(Ptr); }
            set { CSycles.camera_set_shutter_table_offset(Ptr, value); }
        }

        public float GetViewportCameraBorderLeft() {
            return CSycles.camera_get_viewport_camera_border_left(Ptr);
        }
        public void SetViewplaneLeft(float value) {
            CSycles.camera_set_viewplane_left(Ptr, value);
        }

        public float GetViewplaneBottom() {
            return CSycles.camera_get_viewplane_bottom(Ptr);
        }
        public float GetViewplaneLeft() {
            return CSycles.camera_get_viewplane_left(Ptr);
        }
        public void SetBorderRight(float value) {
            CSycles.camera_set_border_right(Ptr, value);
        }
        public float GetBorderLeft() {
            return CSycles.camera_get_border_left(Ptr);
        }
        public float MotionTime(int step) {
            return CSycles.camera_motion_time(Ptr, step);
        }

        public void SetViewplaneBottom(float value) {
            CSycles.camera_set_viewplane_bottom(Ptr, value);
        }

        public void SetViewplaneRight(float value) {
            CSycles.camera_set_viewplane_right(Ptr, value);
        }
        public float3 FullDx {
            get { return CSycles.camera_get_full_dx(Ptr); }
            set { CSycles.camera_set_full_dx(Ptr, value); }
        }
        public float GetViewplaneRight() {
            return CSycles.camera_get_viewplane_right(Ptr);
        }
        public void SetViewportCameraBorderTop(float value) {
            CSycles.camera_set_viewport_camera_border_top(Ptr, value);
        }
        public void SetBorderBottom(float value) {
            CSycles.camera_set_border_bottom(Ptr, value);
        }

        public int MotionStep(float time) {
            return CSycles.camera_motion_step(Ptr, time);
        }
        public void SetViewportCameraBorderLeft(float value) {
            CSycles.camera_set_viewport_camera_border_left(Ptr, value);
        }
        public Transform Cameratondc {
            get { return CSycles.camera_get_cameratondc(Ptr); }
            set { CSycles.camera_set_cameratondc(Ptr, value); }
        }
        public void SetViewplaneTop(float value) {
            CSycles.camera_set_viewplane_top(Ptr, value);
        }
        public float3 Dx {
            get { return CSycles.camera_get_dx(Ptr); }
            set { CSycles.camera_set_dx(Ptr, value); }
        }
        public float3 FrustumTopNormal {
            get { return CSycles.camera_get_frustum_top_normal(Ptr); }
            set { CSycles.camera_set_frustum_top_normal(Ptr, value); }
        }

        public int PreviousNeedMotion {
            get { return CSycles.camera_get_previous_need_motion(Ptr); }
            set { CSycles.camera_set_previous_need_motion(Ptr, value); }
        }

        public float3 FrustumBottomNormal {
            get { return CSycles.camera_get_frustum_bottom_normal(Ptr); }
            set { CSycles.camera_set_frustum_bottom_normal(Ptr, value); }
        }
        public float GetBorderTop() {
            return CSycles.camera_get_border_top(Ptr);
        }
        public void SetViewportCameraBorderRight(float value) {
            CSycles.camera_set_viewport_camera_border_right(Ptr, value);
        }
        public float GetViewportCameraBorderRight() {
            return CSycles.camera_get_viewport_camera_border_right(Ptr);
        }
        public float3 FrustumLeftNormal {
            get { return CSycles.camera_get_frustum_left_normal(Ptr); }
            set { CSycles.camera_set_frustum_left_normal(Ptr, value); }
        }
        public Transform Worldtocamera {
            get { return CSycles.camera_get_worldtocamera(Ptr); }
            set { CSycles.camera_set_worldtocamera(Ptr, value); }
        }
        public float GetViewportCameraBorderTop() {
            return CSycles.camera_get_viewport_camera_border_top(Ptr);
        }
        public void SetScreenSize(int width_, int height_) {
            CSycles.camera_set_screen_size(Ptr, width_, height_);
        }
        public float GetViewportCameraBorderBottom() {
            return CSycles.camera_get_viewport_camera_border_bottom(Ptr);
        }
        public Transform Cameratoworld {
            get { return CSycles.camera_get_cameratoworld(Ptr); }
            set { CSycles.camera_set_cameratoworld(Ptr, value); }
        }
        public float3 Dy {
            get { return CSycles.camera_get_dy(Ptr); }
            set { CSycles.camera_set_dy(Ptr, value); }
        }
        public void ComputeAutoViewplane() {
            CSycles.camera_compute_auto_viewplane(Ptr);
        }

        public void SetBorderTop(float value) {
            CSycles.camera_set_border_top(Ptr, value);
        }

        public bool UseMotion() {
            return CSycles.camera_use_motion(Ptr);
        }
        public float GetBorderBottom() {
            return CSycles.camera_get_border_bottom(Ptr);
        }
        public void SetViewportCameraBorderBottom(float value) {
            CSycles.camera_set_viewport_camera_border_bottom(Ptr, value);
        }
        public float3 FrustumRightNormal {
            get { return CSycles.camera_get_frustum_right_normal(Ptr); }
            set { CSycles.camera_set_frustum_right_normal(Ptr, value); }
        }
        public static IntPtr GetNodeType() {
            return CSycles.camera_get_node_type();
        }
        public float GetBorderRight() {
            return CSycles.camera_get_border_right(Ptr);
        }

        public void SetBorderLeft(float value) {
            CSycles.camera_set_border_left(Ptr, value);
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "interocular_distance":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.065f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'interocular_distance', 'ui_name': 'Interocular Distance'} */
                    {
                    CSycles.camera_set_interocular_distance(this.Ptr, data);
                    }
                    break;
            case "shuttertime":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'shuttertime', 'ui_name': 'Shutter Time'} */
                    {
                    CSycles.camera_set_shuttertime(this.Ptr, data);
                    }
                    break;
            case "convergence_distance":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '30.0f*0.065f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'convergence_distance', 'ui_name': 'Convergence Distance'} */
                    {
                    CSycles.camera_set_convergence_distance(this.Ptr, data);
                    }
                    break;
            case "offscreen_dicing_scale":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'offscreen_dicing_scale', 'ui_name': 'Offscreen Dicing Scale'} */
                    {
                    CSycles.camera_set_offscreen_dicing_scale(this.Ptr, data);
                    }
                    break;
            case "pole_merge_angle_from":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '60.0f*M_PI_F/180.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'pole_merge_angle_from', 'ui_name': 'Pole Merge Angle From'} */
                    {
                    CSycles.camera_set_pole_merge_angle_from(this.Ptr, data);
                    }
                    break;
            case "latitude_max":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '(1.5707963267948966f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'latitude_max', 'ui_name': 'Latitude Max'} */
                    {
                    CSycles.camera_set_latitude_max(this.Ptr, data);
                    }
                    break;
            case "pole_merge_angle_to":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '75.0f*M_PI_F/180.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'pole_merge_angle_to', 'ui_name': 'Pole Merge Angle To'} */
                    {
                    CSycles.camera_set_pole_merge_angle_to(this.Ptr, data);
                    }
                    break;
            case "fov":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '(0.7853981633974830f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fov', 'ui_name': 'FOV'} */
                    {
                    CSycles.camera_set_fov(this.Ptr, data);
                    }
                    break;
            case "sensorwidth":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.036f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sensorwidth', 'ui_name': 'Sensor Width'} */
                    {
                    CSycles.camera_set_sensorwidth(this.Ptr, data);
                    }
                    break;
            case "fov_pre":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '(0.7853981633974830f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fov_pre', 'ui_name': 'FOV Pre'} */
                    {
                    CSycles.camera_set_fov_pre(this.Ptr, data);
                    }
                    break;
            case "sensorheight":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.024f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sensorheight', 'ui_name': 'Sensor Height'} */
                    {
                    CSycles.camera_set_sensorheight(this.Ptr, data);
                    }
                    break;
            case "fov_post":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '(0.7853981633974830f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fov_post', 'ui_name': 'FOV Post'} */
                    {
                    CSycles.camera_set_fov_post(this.Ptr, data);
                    }
                    break;
            case "focaldistance":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '10.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'focaldistance', 'ui_name': 'Focal Distance'} */
                    {
                    CSycles.camera_set_focaldistance(this.Ptr, data);
                    }
                    break;
            case "aperture_ratio":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'aperture_ratio', 'ui_name': 'Aperture Ratio'} */
                    {
                    CSycles.camera_set_aperture_ratio(this.Ptr, data);
                    }
                    break;
            case "fisheye_lens":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '10.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_lens', 'ui_name': 'Fisheye Lens'} */
                    {
                    CSycles.camera_set_fisheye_lens(this.Ptr, data);
                    }
                    break;
            case "nearclip":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '1e-5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'nearclip', 'ui_name': 'Near Clip'} */
                    {
                    CSycles.camera_set_nearclip(this.Ptr, data);
                    }
                    break;
            case "fisheye_polynomial_k0":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k0', 'ui_name': 'Fisheye Polynomial K0'} */
                    {
                    CSycles.camera_set_fisheye_polynomial_k0(this.Ptr, data);
                    }
                    break;
            case "aperturesize":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'aperturesize', 'ui_name': 'Aperture Size'} */
                    {
                    CSycles.camera_set_aperturesize(this.Ptr, data);
                    }
                    break;
            case "fisheye_polynomial_k1":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k1', 'ui_name': 'Fisheye Polynomial K1'} */
                    {
                    CSycles.camera_set_fisheye_polynomial_k1(this.Ptr, data);
                    }
                    break;
            case "farclip":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '1e5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'farclip', 'ui_name': 'Far Clip'} */
                    {
                    CSycles.camera_set_farclip(this.Ptr, data);
                    }
                    break;
            case "fisheye_fov":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '(3.1415926535897932f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_fov', 'ui_name': 'Fisheye FOV'} */
                    {
                    CSycles.camera_set_fisheye_fov(this.Ptr, data);
                    }
                    break;
            case "fisheye_polynomial_k2":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k2', 'ui_name': 'Fisheye Polynomial K2'} */
                    {
                    CSycles.camera_set_fisheye_polynomial_k2(this.Ptr, data);
                    }
                    break;
            case "fisheye_polynomial_k3":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k3', 'ui_name': 'Fisheye Polynomial K3'} */
                    {
                    CSycles.camera_set_fisheye_polynomial_k3(this.Ptr, data);
                    }
                    break;
            case "rolling_shutter_duration":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'rolling_shutter_duration', 'ui_name': 'Rolling Shutter Duration'} */
                    {
                    CSycles.camera_set_rolling_shutter_duration(this.Ptr, data);
                    }
                    break;
            case "longitude_max":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '(3.1415926535897932f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'longitude_max', 'ui_name': 'Longitude Max'} */
                    {
                    CSycles.camera_set_longitude_max(this.Ptr, data);
                    }
                    break;
            case "fisheye_polynomial_k4":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k4', 'ui_name': 'Fisheye Polynomial K4'} */
                    {
                    CSycles.camera_set_fisheye_polynomial_k4(this.Ptr, data);
                    }
                    break;
            case "central_cylindrical_range_u_min":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '', 'default_value_type': 'float', 'is_input': True, 'member_name': 'central_cylindrical_range_u_min', 'ui_name': 'Central Cylindrical Range U Min'} */
                    {
                    CSycles.camera_set_central_cylindrical_range_u_min(this.Ptr, data);
                    }
                    break;
            case "latitude_min":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '', 'default_value_type': 'float', 'is_input': True, 'member_name': 'latitude_min', 'ui_name': 'Latitude Min'} */
                    {
                    CSycles.camera_set_latitude_min(this.Ptr, data);
                    }
                    break;
            case "bladesrotation":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'bladesrotation', 'ui_name': 'Blades Rotation'} */
                    {
                    CSycles.camera_set_bladesrotation(this.Ptr, data);
                    }
                    break;
            case "central_cylindrical_range_u_max":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '(3.1415926535897932f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'central_cylindrical_range_u_max', 'ui_name': 'Central Cylindrical Range U Max'} */
                    {
                    CSycles.camera_set_central_cylindrical_range_u_max(this.Ptr, data);
                    }
                    break;
            case "longitude_min":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '', 'default_value_type': 'float', 'is_input': True, 'member_name': 'longitude_min', 'ui_name': 'Longitude Min'} */
                    {
                    CSycles.camera_set_longitude_min(this.Ptr, data);
                    }
                    break;
            case "central_cylindrical_range_v_min":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '-1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'central_cylindrical_range_v_min', 'ui_name': 'Central Cylindrical Range V Min'} */
                    {
                    CSycles.camera_set_central_cylindrical_range_v_min(this.Ptr, data);
                    }
                    break;
            case "central_cylindrical_range_v_max":
                    /* camera . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'central_cylindrical_range_v_max', 'ui_name': 'Central Cylindrical Range V Max'} */
                    {
                    CSycles.camera_set_central_cylindrical_range_v_max(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_spherical_stereo":
                    /* camera . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_spherical_stereo', 'ui_name': 'Use Spherical Stereo'} */
                    {
                    CSycles.camera_set_use_spherical_stereo(this.Ptr, data);
                    }
                    break;
            case "use_pole_merge":
                    /* camera . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_pole_merge', 'ui_name': 'Use Pole Merge'} */
                    {
                    CSycles.camera_set_use_pole_merge(this.Ptr, data);
                    }
                    break;
            case "use_perspective_motion":
                    /* camera . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_perspective_motion', 'ui_name': 'Use Perspective Motion'} */
                    {
                    CSycles.camera_set_use_perspective_motion(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "full_width":
                    /* camera . {'datatype': 'INT', 'default_value': '1024', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_width', 'ui_name': 'Full Width'} */
                    {
                    CSycles.camera_set_full_width(this.Ptr, data);
                    }
                    break;
            case "full_height":
                    /* camera . {'datatype': 'INT', 'default_value': '512', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_height', 'ui_name': 'Full Height'} */
                    {
                    CSycles.camera_set_full_height(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (setter)");
            }
        }

        internal override void SetUint(string name, uint data)
        {
            switch(name) {
            case "blades":
                    /* camera . {'datatype': 'UINT', 'default_value': '0', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'blades', 'ui_name': 'Blades'} */
                    {
                    CSycles.camera_set_blades(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (setter)");
            }
        }

        internal override void SetTransform(string name, Transform data)
        {
            switch(name) {
            case "matrix":
                    /* camera . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'matrix', 'ui_name': 'Matrix'} */
                    {
                    CSycles.camera_set_matrix(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "stereo_eye":
                    /* camera . {'datatype': 'ENUM', 'default_value': 'STEREO_NONE', 'default_value_type': 'Camera::StereoEye', 'is_input': True, 'member_name': 'stereo_eye', 'ui_name': 'Stereo Eye'} */
                    {
                    CSycles.camera_set_stereo_eye(this.Ptr, (ccl.Camera_StereoEye)data);
                    }
                    break;
            case "rolling_shutter_type":
                    /* camera . {'datatype': 'ENUM', 'default_value': 'ROLLING_SHUTTER_NONE', 'default_value_type': 'Camera::RollingShutterType', 'is_input': True, 'member_name': 'rolling_shutter_type', 'ui_name': 'Rolling Shutter Type'} */
                    {
                    CSycles.camera_set_rolling_shutter_type(this.Ptr, (ccl.Camera_RollingShutterType)data);
                    }
                    break;
            case "motion_position":
                    /* camera . {'datatype': 'ENUM', 'default_value': 'MOTION_POSITION_CENTER', 'default_value_type': 'MotionPosition', 'is_input': True, 'member_name': 'motion_position', 'ui_name': 'Motion Position'} */
                    {
                    CSycles.camera_set_motion_position(this.Ptr, (ccl.MotionPosition)data);
                    }
                    break;
            case "panorama_type":
                    /* camera . {'datatype': 'ENUM', 'default_value': 'PANORAMA_EQUIRECTANGULAR', 'default_value_type': 'PanoramaType', 'is_input': True, 'member_name': 'panorama_type', 'ui_name': 'Panorama Type'} */
                    {
                    CSycles.camera_set_panorama_type(this.Ptr, (ccl.PanoramaType)data);
                    }
                    break;
            case "camera_type":
                    /* camera . {'datatype': 'ENUM', 'default_value': 'CAMERA_PERSPECTIVE', 'default_value_type': 'CameraType', 'is_input': True, 'member_name': 'camera_type', 'ui_name': 'Type'} */
                    {
                    CSycles.camera_set_camera_type(this.Ptr, (ccl.CameraType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (setter)");
            }
        }

        internal override void SetFloatArray(string name, List<float> data)
        {
            switch(name) {
            case "shutter_curve":
                    /* camera . {'datatype': 'FLOAT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'shutter_curve', 'ui_name': 'Shutter Curve'} */
                    {
                    CSycles.camera_set_shutter_curve(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (setter)");
            }
        }

        internal override void SetTransformArray(string name, List<Transform> data)
        {
            switch(name) {
            case "motion":
                    /* camera . {'datatype': 'TRANSFORM_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'motion', 'ui_name': 'Motion'} */
                    {
                    CSycles.camera_set_motion(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "interocular_distance":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.065f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'interocular_distance', 'ui_name': 'Interocular Distance'} */
                {
                    return CSycles.camera_get_interocular_distance(this.Ptr);
                }
            case "shuttertime":
                /* camera . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'shuttertime', 'ui_name': 'Shutter Time'} */
                {
                    return CSycles.camera_get_shuttertime(this.Ptr);
                }
            case "convergence_distance":
                /* camera . {'datatype': 'FLOAT', 'default_value': '30.0f*0.065f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'convergence_distance', 'ui_name': 'Convergence Distance'} */
                {
                    return CSycles.camera_get_convergence_distance(this.Ptr);
                }
            case "offscreen_dicing_scale":
                /* camera . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'offscreen_dicing_scale', 'ui_name': 'Offscreen Dicing Scale'} */
                {
                    return CSycles.camera_get_offscreen_dicing_scale(this.Ptr);
                }
            case "pole_merge_angle_from":
                /* camera . {'datatype': 'FLOAT', 'default_value': '60.0f*M_PI_F/180.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'pole_merge_angle_from', 'ui_name': 'Pole Merge Angle From'} */
                {
                    return CSycles.camera_get_pole_merge_angle_from(this.Ptr);
                }
            case "latitude_max":
                /* camera . {'datatype': 'FLOAT', 'default_value': '(1.5707963267948966f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'latitude_max', 'ui_name': 'Latitude Max'} */
                {
                    return CSycles.camera_get_latitude_max(this.Ptr);
                }
            case "pole_merge_angle_to":
                /* camera . {'datatype': 'FLOAT', 'default_value': '75.0f*M_PI_F/180.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'pole_merge_angle_to', 'ui_name': 'Pole Merge Angle To'} */
                {
                    return CSycles.camera_get_pole_merge_angle_to(this.Ptr);
                }
            case "fov":
                /* camera . {'datatype': 'FLOAT', 'default_value': '(0.7853981633974830f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fov', 'ui_name': 'FOV'} */
                {
                    return CSycles.camera_get_fov(this.Ptr);
                }
            case "sensorwidth":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.036f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sensorwidth', 'ui_name': 'Sensor Width'} */
                {
                    return CSycles.camera_get_sensorwidth(this.Ptr);
                }
            case "fov_pre":
                /* camera . {'datatype': 'FLOAT', 'default_value': '(0.7853981633974830f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fov_pre', 'ui_name': 'FOV Pre'} */
                {
                    return CSycles.camera_get_fov_pre(this.Ptr);
                }
            case "sensorheight":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.024f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sensorheight', 'ui_name': 'Sensor Height'} */
                {
                    return CSycles.camera_get_sensorheight(this.Ptr);
                }
            case "fov_post":
                /* camera . {'datatype': 'FLOAT', 'default_value': '(0.7853981633974830f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fov_post', 'ui_name': 'FOV Post'} */
                {
                    return CSycles.camera_get_fov_post(this.Ptr);
                }
            case "focaldistance":
                /* camera . {'datatype': 'FLOAT', 'default_value': '10.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'focaldistance', 'ui_name': 'Focal Distance'} */
                {
                    return CSycles.camera_get_focaldistance(this.Ptr);
                }
            case "aperture_ratio":
                /* camera . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'aperture_ratio', 'ui_name': 'Aperture Ratio'} */
                {
                    return CSycles.camera_get_aperture_ratio(this.Ptr);
                }
            case "fisheye_lens":
                /* camera . {'datatype': 'FLOAT', 'default_value': '10.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_lens', 'ui_name': 'Fisheye Lens'} */
                {
                    return CSycles.camera_get_fisheye_lens(this.Ptr);
                }
            case "nearclip":
                /* camera . {'datatype': 'FLOAT', 'default_value': '1e-5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'nearclip', 'ui_name': 'Near Clip'} */
                {
                    return CSycles.camera_get_nearclip(this.Ptr);
                }
            case "fisheye_polynomial_k0":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k0', 'ui_name': 'Fisheye Polynomial K0'} */
                {
                    return CSycles.camera_get_fisheye_polynomial_k0(this.Ptr);
                }
            case "aperturesize":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'aperturesize', 'ui_name': 'Aperture Size'} */
                {
                    return CSycles.camera_get_aperturesize(this.Ptr);
                }
            case "fisheye_polynomial_k1":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k1', 'ui_name': 'Fisheye Polynomial K1'} */
                {
                    return CSycles.camera_get_fisheye_polynomial_k1(this.Ptr);
                }
            case "farclip":
                /* camera . {'datatype': 'FLOAT', 'default_value': '1e5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'farclip', 'ui_name': 'Far Clip'} */
                {
                    return CSycles.camera_get_farclip(this.Ptr);
                }
            case "fisheye_fov":
                /* camera . {'datatype': 'FLOAT', 'default_value': '(3.1415926535897932f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_fov', 'ui_name': 'Fisheye FOV'} */
                {
                    return CSycles.camera_get_fisheye_fov(this.Ptr);
                }
            case "fisheye_polynomial_k2":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k2', 'ui_name': 'Fisheye Polynomial K2'} */
                {
                    return CSycles.camera_get_fisheye_polynomial_k2(this.Ptr);
                }
            case "fisheye_polynomial_k3":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k3', 'ui_name': 'Fisheye Polynomial K3'} */
                {
                    return CSycles.camera_get_fisheye_polynomial_k3(this.Ptr);
                }
            case "rolling_shutter_duration":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'rolling_shutter_duration', 'ui_name': 'Rolling Shutter Duration'} */
                {
                    return CSycles.camera_get_rolling_shutter_duration(this.Ptr);
                }
            case "longitude_max":
                /* camera . {'datatype': 'FLOAT', 'default_value': '(3.1415926535897932f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'longitude_max', 'ui_name': 'Longitude Max'} */
                {
                    return CSycles.camera_get_longitude_max(this.Ptr);
                }
            case "fisheye_polynomial_k4":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'fisheye_polynomial_k4', 'ui_name': 'Fisheye Polynomial K4'} */
                {
                    return CSycles.camera_get_fisheye_polynomial_k4(this.Ptr);
                }
            case "central_cylindrical_range_u_min":
                /* camera . {'datatype': 'FLOAT', 'default_value': '', 'default_value_type': 'float', 'is_input': True, 'member_name': 'central_cylindrical_range_u_min', 'ui_name': 'Central Cylindrical Range U Min'} */
                {
                    return CSycles.camera_get_central_cylindrical_range_u_min(this.Ptr);
                }
            case "latitude_min":
                /* camera . {'datatype': 'FLOAT', 'default_value': '', 'default_value_type': 'float', 'is_input': True, 'member_name': 'latitude_min', 'ui_name': 'Latitude Min'} */
                {
                    return CSycles.camera_get_latitude_min(this.Ptr);
                }
            case "bladesrotation":
                /* camera . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'bladesrotation', 'ui_name': 'Blades Rotation'} */
                {
                    return CSycles.camera_get_bladesrotation(this.Ptr);
                }
            case "central_cylindrical_range_u_max":
                /* camera . {'datatype': 'FLOAT', 'default_value': '(3.1415926535897932f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'central_cylindrical_range_u_max', 'ui_name': 'Central Cylindrical Range U Max'} */
                {
                    return CSycles.camera_get_central_cylindrical_range_u_max(this.Ptr);
                }
            case "longitude_min":
                /* camera . {'datatype': 'FLOAT', 'default_value': '', 'default_value_type': 'float', 'is_input': True, 'member_name': 'longitude_min', 'ui_name': 'Longitude Min'} */
                {
                    return CSycles.camera_get_longitude_min(this.Ptr);
                }
            case "central_cylindrical_range_v_min":
                /* camera . {'datatype': 'FLOAT', 'default_value': '-1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'central_cylindrical_range_v_min', 'ui_name': 'Central Cylindrical Range V Min'} */
                {
                    return CSycles.camera_get_central_cylindrical_range_v_min(this.Ptr);
                }
            case "central_cylindrical_range_v_max":
                /* camera . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'central_cylindrical_range_v_max', 'ui_name': 'Central Cylindrical Range V Max'} */
                {
                    return CSycles.camera_get_central_cylindrical_range_v_max(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_spherical_stereo":
                /* camera . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_spherical_stereo', 'ui_name': 'Use Spherical Stereo'} */
                {
                    return CSycles.camera_get_use_spherical_stereo(this.Ptr);
                }
            case "use_pole_merge":
                /* camera . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_pole_merge', 'ui_name': 'Use Pole Merge'} */
                {
                    return CSycles.camera_get_use_pole_merge(this.Ptr);
                }
            case "use_perspective_motion":
                /* camera . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_perspective_motion', 'ui_name': 'Use Perspective Motion'} */
                {
                    return CSycles.camera_get_use_perspective_motion(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "full_width":
                /* camera . {'datatype': 'INT', 'default_value': '1024', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_width', 'ui_name': 'Full Width'} */
                {
                    return CSycles.camera_get_full_width(this.Ptr);
                }
            case "full_height":
                /* camera . {'datatype': 'INT', 'default_value': '512', 'default_value_type': 'int', 'is_input': True, 'member_name': 'full_height', 'ui_name': 'Full Height'} */
                {
                    return CSycles.camera_get_full_height(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (getter)");
            }
        }

        internal override uint GetUint(string name)
        {
            switch(name) {
            case "blades":
                /* camera . {'datatype': 'UINT', 'default_value': '0', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'blades', 'ui_name': 'Blades'} */
                {
                    return CSycles.camera_get_blades(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (getter)");
            }
        }

        internal override Transform GetTransform(string name)
        {
            switch(name) {
            case "matrix":
                /* camera . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'matrix', 'ui_name': 'Matrix'} */
                {
                    return CSycles.camera_get_matrix(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "stereo_eye":
                /* camera . {'datatype': 'ENUM', 'default_value': 'STEREO_NONE', 'default_value_type': 'Camera::StereoEye', 'is_input': True, 'member_name': 'stereo_eye', 'ui_name': 'Stereo Eye'} */
                {
                    return (uint)CSycles.camera_get_stereo_eye(this.Ptr);
                }
            case "rolling_shutter_type":
                /* camera . {'datatype': 'ENUM', 'default_value': 'ROLLING_SHUTTER_NONE', 'default_value_type': 'Camera::RollingShutterType', 'is_input': True, 'member_name': 'rolling_shutter_type', 'ui_name': 'Rolling Shutter Type'} */
                {
                    return (uint)CSycles.camera_get_rolling_shutter_type(this.Ptr);
                }
            case "motion_position":
                /* camera . {'datatype': 'ENUM', 'default_value': 'MOTION_POSITION_CENTER', 'default_value_type': 'MotionPosition', 'is_input': True, 'member_name': 'motion_position', 'ui_name': 'Motion Position'} */
                {
                    return (uint)CSycles.camera_get_motion_position(this.Ptr);
                }
            case "panorama_type":
                /* camera . {'datatype': 'ENUM', 'default_value': 'PANORAMA_EQUIRECTANGULAR', 'default_value_type': 'PanoramaType', 'is_input': True, 'member_name': 'panorama_type', 'ui_name': 'Panorama Type'} */
                {
                    return (uint)CSycles.camera_get_panorama_type(this.Ptr);
                }
            case "camera_type":
                /* camera . {'datatype': 'ENUM', 'default_value': 'CAMERA_PERSPECTIVE', 'default_value_type': 'CameraType', 'is_input': True, 'member_name': 'camera_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.camera_get_camera_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (getter)");
            }
        }

        internal override List<float> GetFloatArray(string name)
        {
            switch(name) {
            case "shutter_curve":
                /* camera . {'datatype': 'FLOAT_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'shutter_curve', 'ui_name': 'Shutter Curve'} */
                {
                    return CSycles.camera_get_shutter_curve(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (getter)");
            }
        }

        internal override List<Transform> GetTransformArray(string name)
        {
            switch(name) {
            case "motion":
                /* camera . {'datatype': 'TRANSFORM_ARRAY', 'default_value': None, 'default_value_type': None, 'is_input': True, 'member_name': 'motion', 'ui_name': 'Motion'} */
                {
                    return CSycles.camera_get_motion(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Camera (getter)");
            }
        }

#endregion
    }

}