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

    public class RhinoTextureCoordinateNodeInputs : Inputs
    {
        public FloatSocket VerticalSweepStart { get; private set; }
        public TransformSocket ObjectTransform { get; private set; }
        public FloatSocket VerticalSweepEnd { get; private set; }
        public NormalSocket NormalIn { get; private set; }
        public FloatSocket Height { get; private set; }
        public FloatSocket HorizontalSweepStart { get; private set; }
        public FloatSocket Radius { get; private set; }
        public BoolSocket FromDupli { get; private set; }
        public FloatSocket HorizontalSweepEnd { get; private set; }
        public EnumSocket DecalDirection { get; private set; }
        public BoolSocket UseTransform { get; private set; }

        public RhinoTextureCoordinateNodeInputs(ShaderNode parentNode)
        {
            VerticalSweepStart = new FloatSocket(parentNode, "Vertical Sweep Start", "vertical_sweep_start", true);
            AddSocket(VerticalSweepStart);
            ObjectTransform = new TransformSocket(parentNode, "Object Transform", "ob_tfm", true);
            AddSocket(ObjectTransform);
            VerticalSweepEnd = new FloatSocket(parentNode, "Vertical Sweep End", "vertical_sweep_end", true);
            AddSocket(VerticalSweepEnd);
            NormalIn = new NormalSocket(parentNode, "NormalIn", "normal_osl", true);
            AddSocket(NormalIn);
            Height = new FloatSocket(parentNode, "Height", "height", true);
            AddSocket(Height);
            HorizontalSweepStart = new FloatSocket(parentNode, "Horizontal Sweep Start", "horizontal_sweep_start", true);
            AddSocket(HorizontalSweepStart);
            Radius = new FloatSocket(parentNode, "Radius", "radius", true);
            AddSocket(Radius);
            FromDupli = new BoolSocket(parentNode, "From Dupli", "from_dupli", true);
            AddSocket(FromDupli);
            HorizontalSweepEnd = new FloatSocket(parentNode, "Horizontal Sweep End", "horizontal_sweep_end", true);
            AddSocket(HorizontalSweepEnd);
            DecalDirection = new EnumSocket(parentNode, "Decal Direction", "decal_projection", true);
            AddSocket(DecalDirection);
            UseTransform = new BoolSocket(parentNode, "Use Transform", "use_transform", true);
            AddSocket(UseTransform);
        }
    }
    public class RhinoTextureCoordinateNodeOutputs : Outputs
    {
        public PointSocket Generated { get; private set; }
        public PointSocket EnvSpherical { get; private set; }
        public PointSocket DecalPlanar { get; private set; }
        public NormalSocket Normal { get; private set; }
        public PointSocket EnvEmap { get; private set; }
        public PointSocket DecalSpherical { get; private set; }
        public PointSocket UV { get; private set; }
        public PointSocket EnvBox { get; private set; }
        public PointSocket DecalCylindrical { get; private set; }
        public PointSocket Object { get; private set; }
        public PointSocket EnvLightProbe { get; private set; }
        public FloatSocket DecalForward { get; private set; }
        public PointSocket Camera { get; private set; }
        public PointSocket EnvCubemap { get; private set; }
        public FloatSocket DecalUsage { get; private set; }
        public PointSocket Window { get; private set; }
        public PointSocket EnvCubemapVerticalCross { get; private set; }
        public NormalSocket Reflection { get; private set; }
        public PointSocket EnvCubemapHorizontalCross { get; private set; }
        public PointSocket WcsBox { get; private set; }
        public PointSocket EnvHemi { get; private set; }
        public PointSocket DecalUv { get; private set; }

        public RhinoTextureCoordinateNodeOutputs(ShaderNode parentNode)
        {
            Generated = new PointSocket(parentNode, "Generated", "generated", false);
            AddSocket(Generated);
            EnvSpherical = new PointSocket(parentNode, "EnvSpherical", "envspherical", false);
            AddSocket(EnvSpherical);
            DecalPlanar = new PointSocket(parentNode, "DecalPlanar", "decalplanar", false);
            AddSocket(DecalPlanar);
            Normal = new NormalSocket(parentNode, "Normal", "normal", false);
            AddSocket(Normal);
            EnvEmap = new PointSocket(parentNode, "EnvEmap", "envemap", false);
            AddSocket(EnvEmap);
            DecalSpherical = new PointSocket(parentNode, "DecalSpherical", "decalspherical", false);
            AddSocket(DecalSpherical);
            UV = new PointSocket(parentNode, "UV", "UV", false);
            AddSocket(UV);
            EnvBox = new PointSocket(parentNode, "EnvBox", "envbox", false);
            AddSocket(EnvBox);
            DecalCylindrical = new PointSocket(parentNode, "DecalCylindrical", "decalcylindrical", false);
            AddSocket(DecalCylindrical);
            Object = new PointSocket(parentNode, "Object", "object", false);
            AddSocket(Object);
            EnvLightProbe = new PointSocket(parentNode, "EnvLightProbe", "envlightprobe", false);
            AddSocket(EnvLightProbe);
            DecalForward = new FloatSocket(parentNode, "DecalForward", "decalforward", false);
            AddSocket(DecalForward);
            Camera = new PointSocket(parentNode, "Camera", "camera", false);
            AddSocket(Camera);
            EnvCubemap = new PointSocket(parentNode, "EnvCubemap", "envcubemap", false);
            AddSocket(EnvCubemap);
            DecalUsage = new FloatSocket(parentNode, "DecalUsage", "decalusage", false);
            AddSocket(DecalUsage);
            Window = new PointSocket(parentNode, "Window", "window", false);
            AddSocket(Window);
            EnvCubemapVerticalCross = new PointSocket(parentNode, "EnvCubemapVerticalCross", "envcubemapverticalcross", false);
            AddSocket(EnvCubemapVerticalCross);
            Reflection = new NormalSocket(parentNode, "Reflection", "reflection", false);
            AddSocket(Reflection);
            EnvCubemapHorizontalCross = new PointSocket(parentNode, "EnvCubemapHorizontalCross", "envcubemaphorizontalcross", false);
            AddSocket(EnvCubemapHorizontalCross);
            WcsBox = new PointSocket(parentNode, "WcsBox", "wcsbox", false);
            AddSocket(WcsBox);
            EnvHemi = new PointSocket(parentNode, "EnvHemi", "envhemi", false);
            AddSocket(EnvHemi);
            DecalUv = new PointSocket(parentNode, "DecalUv", "decaluv", false);
            AddSocket(DecalUv);
        }
    }

    [ShaderNode(name: "rhino_texture_coordinate")]
    public class RhinoTextureCoordinateNode : ShaderNode
    {
        public enum RhinoTextureCoordinateNodeDecalProjection : uint {
            Both = ccl.NodeImageDecalProjection.NODE_IMAGE_DECAL_BOTH,
            Forward = ccl.NodeImageDecalProjection.NODE_IMAGE_DECAL_FORWARD,
            Backward = ccl.NodeImageDecalProjection.NODE_IMAGE_DECAL_BACKWARD,
        }
        public RhinoTextureCoordinateNodeInputs ins => (RhinoTextureCoordinateNodeInputs)inputs;
        public RhinoTextureCoordinateNodeOutputs outs => (RhinoTextureCoordinateNodeOutputs)outputs;
        public RhinoTextureCoordinateNode(Shader shader) : this(shader, "a rhino_texture_coordinate node") { }

        public RhinoTextureCoordinateNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoTextureCoordinateNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoTextureCoordinateNodeInputs(this);
            outputs = new RhinoTextureCoordinateNodeOutputs(this);
        }
        public float3 DecalOrigin {
            get { return CSycles.rhinotexturecoordinatenode_get_decal_origin(Ptr); }
            set { CSycles.rhinotexturecoordinatenode_set_decal_origin(Ptr, value); }
        }
        public float3 DecalAcross {
            get { return CSycles.rhinotexturecoordinatenode_get_decal_across(Ptr); }
            set { CSycles.rhinotexturecoordinatenode_set_decal_across(Ptr, value); }
        }
        public float3 DecalUp {
            get { return CSycles.rhinotexturecoordinatenode_get_decal_up(Ptr); }
            set { CSycles.rhinotexturecoordinatenode_set_decal_up(Ptr, value); }
        }
        public Transform Pxyz {
            get { return CSycles.rhinotexturecoordinatenode_get_pxyz(Ptr); }
            set { CSycles.rhinotexturecoordinatenode_set_pxyz(Ptr, value); }
        }

        public Transform Uvw {
            get { return CSycles.rhinotexturecoordinatenode_get_uvw(Ptr); }
            set { CSycles.rhinotexturecoordinatenode_set_uvw(Ptr, value); }
        }
        public Transform Nxyz {
            get { return CSycles.rhinotexturecoordinatenode_get_nxyz(Ptr); }
            set { CSycles.rhinotexturecoordinatenode_set_nxyz(Ptr, value); }
        }
        public static IntPtr GetNodeType() {
            return CSycles.rhinotexturecoordinatenode_get_node_type();
        }
        public bool HasObjectDependency() {
            return CSycles.rhinotexturecoordinatenode_has_object_dependency(Ptr);
        }

        public string Uvmap {
            get { return CSycles.rhinotexturecoordinatenode_get_uvmap(Ptr); }
            set { CSycles.rhinotexturecoordinatenode_set_uvmap(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "vertical_sweep_start":
                    /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'vertical_sweep_start', 'ui_name': 'Vertical Sweep Start'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_vertical_sweep_start(this.Ptr, data);
                    }
                    break;
            case "vertical_sweep_end":
                    /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'vertical_sweep_end', 'ui_name': 'Vertical Sweep End'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_vertical_sweep_end(this.Ptr, data);
                    }
                    break;
            case "height":
                    /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'height', 'ui_name': 'Height'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_height(this.Ptr, data);
                    }
                    break;
            case "horizontal_sweep_start":
                    /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'horizontal_sweep_start', 'ui_name': 'Horizontal Sweep Start'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_horizontal_sweep_start(this.Ptr, data);
                    }
                    break;
            case "radius":
                    /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'radius', 'ui_name': 'Radius'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_radius(this.Ptr, data);
                    }
                    break;
            case "horizontal_sweep_end":
                    /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'horizontal_sweep_end', 'ui_name': 'Horizontal Sweep End'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_horizontal_sweep_end(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal_osl":
                    /* rhinotexturecoordinatenode . {'datatype': 'NORMAL', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal_osl', 'ui_name': 'NormalIn'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_normal_osl(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "from_dupli":
                    /* rhinotexturecoordinatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'from_dupli', 'ui_name': 'From Dupli'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_from_dupli(this.Ptr, data);
                    }
                    break;
            case "use_transform":
                    /* rhinotexturecoordinatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transform', 'ui_name': 'Use Transform'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_use_transform(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (setter)");
            }
        }

        internal override void SetTransform(string name, Transform data)
        {
            switch(name) {
            case "ob_tfm":
                    /* rhinotexturecoordinatenode . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'ob_tfm', 'ui_name': 'Object Transform'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_ob_tfm(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "decal_projection":
                    /* rhinotexturecoordinatenode . {'datatype': 'ENUM', 'default_value': 'NODE_IMAGE_DECAL_BOTH', 'default_value_type': 'NodeImageDecalProjection', 'is_input': True, 'member_name': 'decal_projection', 'ui_name': 'Decal Direction'} */
                    {
                    CSycles.rhinotexturecoordinatenode_set_decal_projection(this.Ptr, (ccl.NodeImageDecalProjection)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "vertical_sweep_start":
                /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'vertical_sweep_start', 'ui_name': 'Vertical Sweep Start'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_vertical_sweep_start(this.Ptr);
                }
            case "vertical_sweep_end":
                /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'vertical_sweep_end', 'ui_name': 'Vertical Sweep End'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_vertical_sweep_end(this.Ptr);
                }
            case "height":
                /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'height', 'ui_name': 'Height'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_height(this.Ptr);
                }
            case "horizontal_sweep_start":
                /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'horizontal_sweep_start', 'ui_name': 'Horizontal Sweep Start'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_horizontal_sweep_start(this.Ptr);
                }
            case "radius":
                /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'radius', 'ui_name': 'Radius'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_radius(this.Ptr);
                }
            case "horizontal_sweep_end":
                /* rhinotexturecoordinatenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'horizontal_sweep_end', 'ui_name': 'Horizontal Sweep End'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_horizontal_sweep_end(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal_osl":
                /* rhinotexturecoordinatenode . {'datatype': 'NORMAL', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal_osl', 'ui_name': 'NormalIn'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_normal_osl(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "from_dupli":
                /* rhinotexturecoordinatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'from_dupli', 'ui_name': 'From Dupli'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_from_dupli(this.Ptr);
                }
            case "use_transform":
                /* rhinotexturecoordinatenode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transform', 'ui_name': 'Use Transform'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_use_transform(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (getter)");
            }
        }

        internal override Transform GetTransform(string name)
        {
            switch(name) {
            case "ob_tfm":
                /* rhinotexturecoordinatenode . {'datatype': 'TRANSFORM', 'default_value': 'transform_identity()', 'default_value_type': 'Transform', 'is_input': True, 'member_name': 'ob_tfm', 'ui_name': 'Object Transform'} */
                {
                    return CSycles.rhinotexturecoordinatenode_get_ob_tfm(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "decal_projection":
                /* rhinotexturecoordinatenode . {'datatype': 'ENUM', 'default_value': 'NODE_IMAGE_DECAL_BOTH', 'default_value_type': 'NodeImageDecalProjection', 'is_input': True, 'member_name': 'decal_projection', 'ui_name': 'Decal Direction'} */
                {
                    return (uint)CSycles.rhinotexturecoordinatenode_get_decal_projection(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoTextureCoordinateNode (getter)");
            }
        }

#endregion
    }

}