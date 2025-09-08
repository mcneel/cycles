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

    public class ScatterVolumeNodeInputs : Inputs
    {
        public FloatSocket Backscatter { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket Alpha { get; private set; }
        public FloatSocket Density { get; private set; }
        public FloatSocket Diameter { get; private set; }
        public FloatSocket Anisotropy { get; private set; }
        public EnumSocket Phase { get; private set; }
        public FloatSocket IOR { get; private set; }

        public ScatterVolumeNodeInputs(ShaderNode parentNode)
        {
            Backscatter = new FloatSocket(parentNode, "Backscatter", "backscatter", true);
            AddSocket(Backscatter);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Alpha = new FloatSocket(parentNode, "Alpha", "alpha", true);
            AddSocket(Alpha);
            Density = new FloatSocket(parentNode, "Density", "density", true);
            AddSocket(Density);
            Diameter = new FloatSocket(parentNode, "Diameter", "diameter", true);
            AddSocket(Diameter);
            Anisotropy = new FloatSocket(parentNode, "Anisotropy", "anisotropy", true);
            AddSocket(Anisotropy);
            Phase = new EnumSocket(parentNode, "Phase", "phase", true);
            AddSocket(Phase);
            IOR = new FloatSocket(parentNode, "IOR", "IOR", true);
            AddSocket(IOR);
        }
    }
    public class ScatterVolumeNodeOutputs : Outputs
    {
        public ClosureSocket Volume { get; private set; }

        public ScatterVolumeNodeOutputs(ShaderNode parentNode)
        {
            Volume = new ClosureSocket(parentNode, "Volume", "volume", false);
            AddSocket(Volume);
        }
    }

    [ShaderNode(name: "scatter_volume")]
    public class ScatterVolumeNode : VolumeNode
    {
        public enum ScatterVolumeNodePhase : uint {
            HenyeyGreenstein = ccl.ClosureType.CLOSURE_VOLUME_HENYEY_GREENSTEIN_ID,
            Mie = ccl.ClosureType.CLOSURE_VOLUME_MIE_ID,
            FournierForand = ccl.ClosureType.CLOSURE_VOLUME_FOURNIER_FORAND_ID,
            Rayleigh = ccl.ClosureType.CLOSURE_VOLUME_RAYLEIGH_ID,
            Draine = ccl.ClosureType.CLOSURE_VOLUME_DRAINE_ID,
        }
        public ScatterVolumeNodeInputs ins => (ScatterVolumeNodeInputs)inputs;
        public ScatterVolumeNodeOutputs outs => (ScatterVolumeNodeOutputs)outputs;
        public ScatterVolumeNode(Shader shader) : this(shader, "a scatter_volume node") { }

        public ScatterVolumeNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal ScatterVolumeNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new ScatterVolumeNodeInputs(this);
            outputs = new ScatterVolumeNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.Volume;
        }
        public float GetIor() {
            return CSycles.scattervolumenode_get_ior(Ptr);
        }
        public void SetIor(float value) {
            CSycles.scattervolumenode_set_ior(Ptr, value);
        }
        public static IntPtr GetNodeType() {
            return CSycles.scattervolumenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "backscatter":
                    /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'backscatter', 'ui_name': 'Backscatter'} */
                    {
                    CSycles.scattervolumenode_set_backscatter(this.Ptr, data);
                    }
                    break;
            case "alpha":
                    /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha', 'ui_name': 'Alpha'} */
                    {
                    CSycles.scattervolumenode_set_alpha(this.Ptr, data);
                    }
                    break;
            case "density":
                    /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'density', 'ui_name': 'Density'} */
                    {
                    CSycles.volumenode_set_density(this.Ptr, data);
                    }
                    break;
            case "diameter":
                    /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '20.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'diameter', 'ui_name': 'Diameter'} */
                    {
                    CSycles.scattervolumenode_set_diameter(this.Ptr, data);
                    }
                    break;
            case "anisotropy":
                    /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                    {
                    CSycles.scattervolumenode_set_anisotropy(this.Ptr, data);
                    }
                    break;
            case "IOR":
                    /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '1.33f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'IOR', 'ui_name': 'IOR'} */
                    {
                    CSycles.scattervolumenode_set_ior(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ScatterVolumeNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* scattervolumenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.volumenode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ScatterVolumeNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "phase":
                    /* scattervolumenode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_VOLUME_HENYEY_GREENSTEIN_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'phase', 'ui_name': 'Phase'} */
                    {
                    CSycles.scattervolumenode_set_phase(this.Ptr, (ccl.ClosureType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ScatterVolumeNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "backscatter":
                /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'backscatter', 'ui_name': 'Backscatter'} */
                {
                    return CSycles.scattervolumenode_get_backscatter(this.Ptr);
                }
            case "alpha":
                /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'alpha', 'ui_name': 'Alpha'} */
                {
                    return CSycles.scattervolumenode_get_alpha(this.Ptr);
                }
            case "density":
                /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'density', 'ui_name': 'Density'} */
                {
                    return CSycles.volumenode_get_density(this.Ptr);
                }
            case "diameter":
                /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '20.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'diameter', 'ui_name': 'Diameter'} */
                {
                    return CSycles.scattervolumenode_get_diameter(this.Ptr);
                }
            case "anisotropy":
                /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                {
                    return CSycles.scattervolumenode_get_anisotropy(this.Ptr);
                }
            case "IOR":
                /* scattervolumenode . {'datatype': 'FLOAT', 'default_value': '1.33f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'IOR', 'ui_name': 'IOR'} */
                {
                    return CSycles.scattervolumenode_get_ior(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ScatterVolumeNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* scattervolumenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.volumenode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ScatterVolumeNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "phase":
                /* scattervolumenode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_VOLUME_HENYEY_GREENSTEIN_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'phase', 'ui_name': 'Phase'} */
                {
                    return (uint)CSycles.scattervolumenode_get_phase(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type ScatterVolumeNode (getter)");
            }
        }

#endregion
    }

}