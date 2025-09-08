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

    public class RhinoWavesWidthTextureNodeInputs : Inputs
    {
        public PointSocket UVW { get; private set; }
        public EnumSocket WaveType { get; private set; }

        public RhinoWavesWidthTextureNodeInputs(ShaderNode parentNode)
        {
            UVW = new PointSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            WaveType = new EnumSocket(parentNode, "WaveType", "wave_type", true);
            AddSocket(WaveType);
        }
    }
    public class RhinoWavesWidthTextureNodeOutputs : Outputs
    {
        public PointSocket UVW { get; private set; }

        public RhinoWavesWidthTextureNodeOutputs(ShaderNode parentNode)
        {
            UVW = new PointSocket(parentNode, "UVW", "out_uvw", false);
            AddSocket(UVW);
        }
    }

    [ShaderNode(name: "rhino_waves_width_texture")]
    public class RhinoWavesWidthTextureNode : ShaderNode
    {
        public enum RhinoWavesWidthTextureNodeWaveType : uint {
            Linear = ccl.RhinoProceduralWavesType.RHINO_WAVES_LINEAR,
            Radial = ccl.RhinoProceduralWavesType.RHINO_WAVES_RADIAL,
        }
        public RhinoWavesWidthTextureNodeInputs ins => (RhinoWavesWidthTextureNodeInputs)inputs;
        public RhinoWavesWidthTextureNodeOutputs outs => (RhinoWavesWidthTextureNodeOutputs)outputs;
        public RhinoWavesWidthTextureNode(Shader shader) : this(shader, "a rhino_waves_width_texture node") { }

        public RhinoWavesWidthTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoWavesWidthTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoWavesWidthTextureNodeInputs(this);
            outputs = new RhinoWavesWidthTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinowaveswidthtexturenode_get_uvw(Ptr); }
            set { CSycles.rhinowaveswidthtexturenode_set_uvw(Ptr, value); }
        }

        public RhinoProceduralWavesType WaveType {
            get { return CSycles.rhinowaveswidthtexturenode_get_wave_type(Ptr); }
            set { CSycles.rhinowaveswidthtexturenode_set_wave_type(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinowaveswidthtexturenode_get_node_type();
        }
#region Setters

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinowaveswidthtexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinowaveswidthtexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesWidthTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "wave_type":
                    /* rhinowaveswidthtexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_WAVES_LINEAR', 'default_value_type': 'RhinoProceduralWavesType', 'is_input': True, 'member_name': 'wave_type', 'ui_name': 'WaveType'} */
                    {
                    CSycles.rhinowaveswidthtexturenode_set_wave_type(this.Ptr, (ccl.RhinoProceduralWavesType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesWidthTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinowaveswidthtexturenode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinowaveswidthtexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesWidthTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "wave_type":
                /* rhinowaveswidthtexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_WAVES_LINEAR', 'default_value_type': 'RhinoProceduralWavesType', 'is_input': True, 'member_name': 'wave_type', 'ui_name': 'WaveType'} */
                {
                    return (uint)CSycles.rhinowaveswidthtexturenode_get_wave_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoWavesWidthTextureNode (getter)");
            }
        }

#endregion
    }

}