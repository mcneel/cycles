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

    public class RhinoProjectionChangerTextureNodeInputs : Inputs
    {
        public EnumSocket OutputProjectionType { get; private set; }
        public VectorSocket UVW { get; private set; }
        public FloatSocket Azimuth { get; private set; }
        public EnumSocket InputProjectionType { get; private set; }
        public FloatSocket Altitude { get; private set; }

        public RhinoProjectionChangerTextureNodeInputs(ShaderNode parentNode)
        {
            OutputProjectionType = new EnumSocket(parentNode, "OutputProjectionType", "output_projection_type", true);
            AddSocket(OutputProjectionType);
            UVW = new VectorSocket(parentNode, "UVW", "uvw", true);
            AddSocket(UVW);
            Azimuth = new FloatSocket(parentNode, "Azimuth", "azimuth", true);
            AddSocket(Azimuth);
            InputProjectionType = new EnumSocket(parentNode, "InputProjectionType", "input_projection_type", true);
            AddSocket(InputProjectionType);
            Altitude = new FloatSocket(parentNode, "Altitude", "altitude", true);
            AddSocket(Altitude);
        }
    }
    public class RhinoProjectionChangerTextureNodeOutputs : Outputs
    {
        public VectorSocket OutputUVW { get; private set; }

        public RhinoProjectionChangerTextureNodeOutputs(ShaderNode parentNode)
        {
            OutputUVW = new VectorSocket(parentNode, "Output UVW", "out_uvw", false);
            AddSocket(OutputUVW);
        }
    }

    [ShaderNode(name: "rhino_projection_changer_texture")]
    public class RhinoProjectionChangerTextureNode : ShaderNode
    {
        public enum RhinoProjectionChangerTextureNodeProjectionChangerType : uint {
            None = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_NONE,
            Planar = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_PLANAR,
            Lightprobe = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_LIGHTPROBE,
            Equirect = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_EQUIRECT,
            Cubemap = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_CUBEMAP,
            VerticalCrossCubemap = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_VERTICAL_CROSS_CUBEMAP,
            HorizontalCrossCubemap = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_HORIZONTAL_CROSS_CUBEMAP,
            Emap = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_EMAP,
            SameAsInput = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_SAME_AS_INPUT,
            Hemispherical = ccl.RhinoProceduralProjectionType.RHINO_PROJECTION_HEMISPHERICAL,
        }
        public RhinoProjectionChangerTextureNodeInputs ins => (RhinoProjectionChangerTextureNodeInputs)inputs;
        public RhinoProjectionChangerTextureNodeOutputs outs => (RhinoProjectionChangerTextureNodeOutputs)outputs;
        public RhinoProjectionChangerTextureNode(Shader shader) : this(shader, "a rhino_projection_changer_texture node") { }

        public RhinoProjectionChangerTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoProjectionChangerTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoProjectionChangerTextureNodeInputs(this);
            outputs = new RhinoProjectionChangerTextureNodeOutputs(this);
        }
        public float3 Uvw {
            get { return CSycles.rhinoprojectionchangertexturenode_get_uvw(Ptr); }
            set { CSycles.rhinoprojectionchangertexturenode_set_uvw(Ptr, value); }
        }

        public float Azimuth {
            get { return CSycles.rhinoprojectionchangertexturenode_get_azimuth(Ptr); }
            set { CSycles.rhinoprojectionchangertexturenode_set_azimuth(Ptr, value); }
        }

        public RhinoProceduralProjectionType OutputProjectionType {
            get { return CSycles.rhinoprojectionchangertexturenode_get_output_projection_type(Ptr); }
            set { CSycles.rhinoprojectionchangertexturenode_set_output_projection_type(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinoprojectionchangertexturenode_get_node_type();
        }

        public RhinoProceduralProjectionType InputProjectionType {
            get { return CSycles.rhinoprojectionchangertexturenode_get_input_projection_type(Ptr); }
            set { CSycles.rhinoprojectionchangertexturenode_set_input_projection_type(Ptr, value); }
        }

        public float Altitude {
            get { return CSycles.rhinoprojectionchangertexturenode_get_altitude(Ptr); }
            set { CSycles.rhinoprojectionchangertexturenode_set_altitude(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "azimuth":
                    /* rhinoprojectionchangertexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'azimuth', 'ui_name': 'Azimuth'} */
                    {
                    CSycles.rhinoprojectionchangertexturenode_set_azimuth(this.Ptr, data);
                    }
                    break;
            case "altitude":
                    /* rhinoprojectionchangertexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'altitude', 'ui_name': 'Altitude'} */
                    {
                    CSycles.rhinoprojectionchangertexturenode_set_altitude(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoProjectionChangerTextureNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "uvw":
                    /* rhinoprojectionchangertexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                    {
                    CSycles.rhinoprojectionchangertexturenode_set_uvw(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoProjectionChangerTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "output_projection_type":
                    /* rhinoprojectionchangertexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_PROJECTION_NONE', 'default_value_type': 'RhinoProceduralProjectionType', 'is_input': True, 'member_name': 'output_projection_type', 'ui_name': 'OutputProjectionType'} */
                    {
                    CSycles.rhinoprojectionchangertexturenode_set_output_projection_type(this.Ptr, (ccl.RhinoProceduralProjectionType)data);
                    }
                    break;
            case "input_projection_type":
                    /* rhinoprojectionchangertexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_PROJECTION_NONE', 'default_value_type': 'RhinoProceduralProjectionType', 'is_input': True, 'member_name': 'input_projection_type', 'ui_name': 'InputProjectionType'} */
                    {
                    CSycles.rhinoprojectionchangertexturenode_set_input_projection_type(this.Ptr, (ccl.RhinoProceduralProjectionType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoProjectionChangerTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "azimuth":
                /* rhinoprojectionchangertexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'azimuth', 'ui_name': 'Azimuth'} */
                {
                    return CSycles.rhinoprojectionchangertexturenode_get_azimuth(this.Ptr);
                }
            case "altitude":
                /* rhinoprojectionchangertexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'altitude', 'ui_name': 'Altitude'} */
                {
                    return CSycles.rhinoprojectionchangertexturenode_get_altitude(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoProjectionChangerTextureNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "uvw":
                /* rhinoprojectionchangertexturenode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'uvw', 'ui_name': 'UVW'} */
                {
                    return CSycles.rhinoprojectionchangertexturenode_get_uvw(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoProjectionChangerTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "output_projection_type":
                /* rhinoprojectionchangertexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_PROJECTION_NONE', 'default_value_type': 'RhinoProceduralProjectionType', 'is_input': True, 'member_name': 'output_projection_type', 'ui_name': 'OutputProjectionType'} */
                {
                    return (uint)CSycles.rhinoprojectionchangertexturenode_get_output_projection_type(this.Ptr);
                }
            case "input_projection_type":
                /* rhinoprojectionchangertexturenode . {'datatype': 'ENUM', 'default_value': 'RHINO_PROJECTION_NONE', 'default_value_type': 'RhinoProceduralProjectionType', 'is_input': True, 'member_name': 'input_projection_type', 'ui_name': 'InputProjectionType'} */
                {
                    return (uint)CSycles.rhinoprojectionchangertexturenode_get_input_projection_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoProjectionChangerTextureNode (getter)");
            }
        }

#endregion
    }

}