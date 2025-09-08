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

    public class LightNodeInputs : NodeInputs
    {
        public EnumNodeSocket Type { get; private set; }
        public BoolNodeSocket CastShadow { get; private set; }
        public FloatNodeSocket SizeU { get; private set; }
        public BoolNodeSocket UseMis { get; private set; }
        public FloatNodeSocket SizeV { get; private set; }
        public BoolNodeSocket ShadowCaustics { get; private set; }
        public BoolNodeSocket Ellipse { get; private set; }
        public FloatNodeSocket Size { get; private set; }
        public IntNodeSocket MaxBounces { get; private set; }
        public FloatNodeSocket Spread { get; private set; }
        public BoolNodeSocket IsPortal { get; private set; }
        public IntNodeSocket MapResolution { get; private set; }
        public BoolNodeSocket IsEnabled { get; private set; }
        public FloatNodeSocket AverageRadiance { get; private set; }
        public BoolNodeSocket Normalize { get; private set; }
        public BoolNodeSocket IsSphere { get; private set; }
        public ColorNodeSocket Strength { get; private set; }
        public FloatNodeSocket Angle { get; private set; }
        public FloatNodeSocket SpotAngle { get; private set; }
        public FloatNodeSocket SpotSmooth { get; private set; }

        public LightNodeInputs(Node parentNode)
        {
            Type = new EnumNodeSocket(parentNode, "Type", "light_type", true);
            AddSocket(Type);
            CastShadow = new BoolNodeSocket(parentNode, "Cast Shadow", "cast_shadow", true);
            AddSocket(CastShadow);
            SizeU = new FloatNodeSocket(parentNode, "Size U", "sizeu", true);
            AddSocket(SizeU);
            UseMis = new BoolNodeSocket(parentNode, "Use Mis", "use_mis", true);
            AddSocket(UseMis);
            SizeV = new FloatNodeSocket(parentNode, "Size V", "sizev", true);
            AddSocket(SizeV);
            ShadowCaustics = new BoolNodeSocket(parentNode, "Shadow Caustics", "use_caustics", true);
            AddSocket(ShadowCaustics);
            Ellipse = new BoolNodeSocket(parentNode, "Ellipse", "ellipse", true);
            AddSocket(Ellipse);
            Size = new FloatNodeSocket(parentNode, "Size", "size", true);
            AddSocket(Size);
            MaxBounces = new IntNodeSocket(parentNode, "Max Bounces", "max_bounces", true);
            AddSocket(MaxBounces);
            Spread = new FloatNodeSocket(parentNode, "Spread", "spread", true);
            AddSocket(Spread);
            IsPortal = new BoolNodeSocket(parentNode, "Is Portal", "is_portal", true);
            AddSocket(IsPortal);
            MapResolution = new IntNodeSocket(parentNode, "Map Resolution", "map_resolution", true);
            AddSocket(MapResolution);
            IsEnabled = new BoolNodeSocket(parentNode, "Is Enabled", "is_enabled", true);
            AddSocket(IsEnabled);
            AverageRadiance = new FloatNodeSocket(parentNode, "Average Radiance", "average_radiance", true);
            AddSocket(AverageRadiance);
            Normalize = new BoolNodeSocket(parentNode, "Normalize", "normalize", true);
            AddSocket(Normalize);
            IsSphere = new BoolNodeSocket(parentNode, "Is Sphere", "is_sphere", true);
            AddSocket(IsSphere);
            Strength = new ColorNodeSocket(parentNode, "Strength", "strength", true);
            AddSocket(Strength);
            Angle = new FloatNodeSocket(parentNode, "Angle", "angle", true);
            AddSocket(Angle);
            SpotAngle = new FloatNodeSocket(parentNode, "Spot Angle", "spot_angle", true);
            AddSocket(SpotAngle);
            SpotSmooth = new FloatNodeSocket(parentNode, "Spot Smooth", "spot_smooth", true);
            AddSocket(SpotSmooth);
        }
    }
    public class Light : Geometry
    {
        public enum LightType : uint {
            Point = ccl.LightType.LIGHT_POINT,
            Distant = ccl.LightType.LIGHT_DISTANT,
            Background = ccl.LightType.LIGHT_BACKGROUND,
            Area = ccl.LightType.LIGHT_AREA,
            Spot = ccl.LightType.LIGHT_SPOT,
        }
        public LightNodeInputs LightNodeInputs { get; set; }
        public LightNodeInputs ins => LightNodeInputs;

        public Light() : this("a light node") { }

        public Light(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Light(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            LightNodeInputs = new LightNodeInputs(this);

        }
        public Shader GetShader() {
            return new Shader(CSycles.light_get_shader(Ptr));
        }
        public override PrimitiveType PrimitiveType() {
            return CSycles.light_primitive_type(Ptr);
        }
        public static IntPtr GetNodeType() {
            return CSycles.light_get_node_type();
        }

        public override void ApplyTransform(Transform tfm, bool apply_to_motion) {
            CSycles.light_apply_transform(Ptr, tfm, apply_to_motion);
        }

        public override void ComputeBounds() {
            CSycles.light_compute_bounds(Ptr);
        }

        public bool HasContribution(IntPtr scene) {
            return CSycles.light_has_contribution(Ptr, scene);
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "sizeu":
                    /* light . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sizeu', 'ui_name': 'Size U'} */
                    {
                    CSycles.light_set_sizeu(this.Ptr, data);
                    }
                    break;
            case "sizev":
                    /* light . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sizev', 'ui_name': 'Size V'} */
                    {
                    CSycles.light_set_sizev(this.Ptr, data);
                    }
                    break;
            case "size":
                    /* light . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'size', 'ui_name': 'Size'} */
                    {
                    CSycles.light_set_size(this.Ptr, data);
                    }
                    break;
            case "spread":
                    /* light . {'datatype': 'FLOAT', 'default_value': '(3.1415926535897932f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'spread', 'ui_name': 'Spread'} */
                    {
                    CSycles.light_set_spread(this.Ptr, data);
                    }
                    break;
            case "average_radiance":
                    /* light . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'average_radiance', 'ui_name': 'Average Radiance'} */
                    {
                    CSycles.light_set_average_radiance(this.Ptr, data);
                    }
                    break;
            case "angle":
                    /* light . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'angle', 'ui_name': 'Angle'} */
                    {
                    CSycles.light_set_angle(this.Ptr, data);
                    }
                    break;
            case "spot_angle":
                    /* light . {'datatype': 'FLOAT', 'default_value': '(0.7853981633974830f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'spot_angle', 'ui_name': 'Spot Angle'} */
                    {
                    CSycles.light_set_spot_angle(this.Ptr, data);
                    }
                    break;
            case "spot_smooth":
                    /* light . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'spot_smooth', 'ui_name': 'Spot Smooth'} */
                    {
                    CSycles.light_set_spot_smooth(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "strength":
                    /* light . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                    {
                    CSycles.light_set_strength(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "cast_shadow":
                    /* light . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'cast_shadow', 'ui_name': 'Cast Shadow'} */
                    {
                    CSycles.light_set_cast_shadow(this.Ptr, data);
                    }
                    break;
            case "use_mis":
                    /* light . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_mis', 'ui_name': 'Use Mis'} */
                    {
                    CSycles.light_set_use_mis(this.Ptr, data);
                    }
                    break;
            case "use_caustics":
                    /* light . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_caustics', 'ui_name': 'Shadow Caustics'} */
                    {
                    CSycles.light_set_use_caustics(this.Ptr, data);
                    }
                    break;
            case "ellipse":
                    /* light . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'ellipse', 'ui_name': 'Ellipse'} */
                    {
                    CSycles.light_set_ellipse(this.Ptr, data);
                    }
                    break;
            case "is_portal":
                    /* light . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_portal', 'ui_name': 'Is Portal'} */
                    {
                    CSycles.light_set_is_portal(this.Ptr, data);
                    }
                    break;
            case "is_enabled":
                    /* light . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_enabled', 'ui_name': 'Is Enabled'} */
                    {
                    CSycles.light_set_is_enabled(this.Ptr, data);
                    }
                    break;
            case "normalize":
                    /* light . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'normalize', 'ui_name': 'Normalize'} */
                    {
                    CSycles.light_set_normalize(this.Ptr, data);
                    }
                    break;
            case "is_sphere":
                    /* light . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_sphere', 'ui_name': 'Is Sphere'} */
                    {
                    CSycles.light_set_is_sphere(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "max_bounces":
                    /* light . {'datatype': 'INT', 'default_value': '1024', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_bounces', 'ui_name': 'Max Bounces'} */
                    {
                    CSycles.light_set_max_bounces(this.Ptr, data);
                    }
                    break;
            case "map_resolution":
                    /* light . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'map_resolution', 'ui_name': 'Map Resolution'} */
                    {
                    CSycles.light_set_map_resolution(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "light_type":
                    /* light . {'datatype': 'ENUM', 'default_value': 'LIGHT_POINT', 'default_value_type': 'LightType', 'is_input': True, 'member_name': 'light_type', 'ui_name': 'Type'} */
                    {
                    CSycles.light_set_light_type(this.Ptr, (ccl.LightType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "sizeu":
                /* light . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sizeu', 'ui_name': 'Size U'} */
                {
                    return CSycles.light_get_sizeu(this.Ptr);
                }
            case "sizev":
                /* light . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sizev', 'ui_name': 'Size V'} */
                {
                    return CSycles.light_get_sizev(this.Ptr);
                }
            case "size":
                /* light . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'size', 'ui_name': 'Size'} */
                {
                    return CSycles.light_get_size(this.Ptr);
                }
            case "spread":
                /* light . {'datatype': 'FLOAT', 'default_value': '(3.1415926535897932f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'spread', 'ui_name': 'Spread'} */
                {
                    return CSycles.light_get_spread(this.Ptr);
                }
            case "average_radiance":
                /* light . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'average_radiance', 'ui_name': 'Average Radiance'} */
                {
                    return CSycles.light_get_average_radiance(this.Ptr);
                }
            case "angle":
                /* light . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'angle', 'ui_name': 'Angle'} */
                {
                    return CSycles.light_get_angle(this.Ptr);
                }
            case "spot_angle":
                /* light . {'datatype': 'FLOAT', 'default_value': '(0.7853981633974830f)', 'default_value_type': 'float', 'is_input': True, 'member_name': 'spot_angle', 'ui_name': 'Spot Angle'} */
                {
                    return CSycles.light_get_spot_angle(this.Ptr);
                }
            case "spot_smooth":
                /* light . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'spot_smooth', 'ui_name': 'Spot Smooth'} */
                {
                    return CSycles.light_get_spot_smooth(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "strength":
                /* light . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                {
                    return CSycles.light_get_strength(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "cast_shadow":
                /* light . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'cast_shadow', 'ui_name': 'Cast Shadow'} */
                {
                    return CSycles.light_get_cast_shadow(this.Ptr);
                }
            case "use_mis":
                /* light . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_mis', 'ui_name': 'Use Mis'} */
                {
                    return CSycles.light_get_use_mis(this.Ptr);
                }
            case "use_caustics":
                /* light . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_caustics', 'ui_name': 'Shadow Caustics'} */
                {
                    return CSycles.light_get_use_caustics(this.Ptr);
                }
            case "ellipse":
                /* light . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'ellipse', 'ui_name': 'Ellipse'} */
                {
                    return CSycles.light_get_ellipse(this.Ptr);
                }
            case "is_portal":
                /* light . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_portal', 'ui_name': 'Is Portal'} */
                {
                    return CSycles.light_get_is_portal(this.Ptr);
                }
            case "is_enabled":
                /* light . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_enabled', 'ui_name': 'Is Enabled'} */
                {
                    return CSycles.light_get_is_enabled(this.Ptr);
                }
            case "normalize":
                /* light . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'normalize', 'ui_name': 'Normalize'} */
                {
                    return CSycles.light_get_normalize(this.Ptr);
                }
            case "is_sphere":
                /* light . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'is_sphere', 'ui_name': 'Is Sphere'} */
                {
                    return CSycles.light_get_is_sphere(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "max_bounces":
                /* light . {'datatype': 'INT', 'default_value': '1024', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_bounces', 'ui_name': 'Max Bounces'} */
                {
                    return CSycles.light_get_max_bounces(this.Ptr);
                }
            case "map_resolution":
                /* light . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'map_resolution', 'ui_name': 'Map Resolution'} */
                {
                    return CSycles.light_get_map_resolution(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "light_type":
                /* light . {'datatype': 'ENUM', 'default_value': 'LIGHT_POINT', 'default_value_type': 'LightType', 'is_input': True, 'member_name': 'light_type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.light_get_light_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Light (getter)");
            }
        }

#endregion
    }

}