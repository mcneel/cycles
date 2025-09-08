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

    public class AzimuthAltitudeTransformNodeInputs : Inputs
    {
        public FloatSocket Threshold { get; private set; }
        public FloatSocket Altitude { get; private set; }
        public FloatSocket Azimuth { get; private set; }
        public PointSocket Vector { get; private set; }

        public AzimuthAltitudeTransformNodeInputs(ShaderNode parentNode)
        {
            Threshold = new FloatSocket(parentNode, "Threshold", "threshold", true);
            AddSocket(Threshold);
            Altitude = new FloatSocket(parentNode, "Altitude", "altitude", true);
            AddSocket(Altitude);
            Azimuth = new FloatSocket(parentNode, "Azimuth", "azimuth", true);
            AddSocket(Azimuth);
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
        }
    }
    public class AzimuthAltitudeTransformNodeOutputs : Outputs
    {
        public VectorSocket Vector { get; private set; }

        public AzimuthAltitudeTransformNodeOutputs(ShaderNode parentNode)
        {
            Vector = new VectorSocket(parentNode, "Vector", "vector", false);
            AddSocket(Vector);
        }
    }

    [ShaderNode(name: "azimuth_altitude_transform")]
    public class AzimuthAltitudeTransformNode : ShaderNode
    {
        public AzimuthAltitudeTransformNodeInputs ins => (AzimuthAltitudeTransformNodeInputs)inputs;
        public AzimuthAltitudeTransformNodeOutputs outs => (AzimuthAltitudeTransformNodeOutputs)outputs;
        public AzimuthAltitudeTransformNode(Shader shader) : this(shader, "a azimuth_altitude_transform node") { }

        public AzimuthAltitudeTransformNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal AzimuthAltitudeTransformNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new AzimuthAltitudeTransformNodeInputs(this);
            outputs = new AzimuthAltitudeTransformNodeOutputs(this);
        }
        public float3 Vector {
            get { return CSycles.azimuthaltitudetransformnode_get_vector(Ptr); }
            set { CSycles.azimuthaltitudetransformnode_set_vector(Ptr, value); }
        }

        public float Azimuth {
            get { return CSycles.azimuthaltitudetransformnode_get_azimuth(Ptr); }
            set { CSycles.azimuthaltitudetransformnode_set_azimuth(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.azimuthaltitudetransformnode_get_node_type();
        }

        public float Threshold {
            get { return CSycles.azimuthaltitudetransformnode_get_threshold(Ptr); }
            set { CSycles.azimuthaltitudetransformnode_set_threshold(Ptr, value); }
        }

        public float Altitude {
            get { return CSycles.azimuthaltitudetransformnode_get_altitude(Ptr); }
            set { CSycles.azimuthaltitudetransformnode_set_altitude(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "threshold":
                    /* azimuthaltitudetransformnode . {'datatype': 'FLOAT', 'default_value': '0.001f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'threshold', 'ui_name': 'Threshold'} */
                    {
                    CSycles.azimuthaltitudetransformnode_set_threshold(this.Ptr, data);
                    }
                    break;
            case "altitude":
                    /* azimuthaltitudetransformnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'altitude', 'ui_name': 'Altitude'} */
                    {
                    CSycles.azimuthaltitudetransformnode_set_altitude(this.Ptr, data);
                    }
                    break;
            case "azimuth":
                    /* azimuthaltitudetransformnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'azimuth', 'ui_name': 'Azimuth'} */
                    {
                    CSycles.azimuthaltitudetransformnode_set_azimuth(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AzimuthAltitudeTransformNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* azimuthaltitudetransformnode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.azimuthaltitudetransformnode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AzimuthAltitudeTransformNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "threshold":
                /* azimuthaltitudetransformnode . {'datatype': 'FLOAT', 'default_value': '0.001f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'threshold', 'ui_name': 'Threshold'} */
                {
                    return CSycles.azimuthaltitudetransformnode_get_threshold(this.Ptr);
                }
            case "altitude":
                /* azimuthaltitudetransformnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'altitude', 'ui_name': 'Altitude'} */
                {
                    return CSycles.azimuthaltitudetransformnode_get_altitude(this.Ptr);
                }
            case "azimuth":
                /* azimuthaltitudetransformnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'azimuth', 'ui_name': 'Azimuth'} */
                {
                    return CSycles.azimuthaltitudetransformnode_get_azimuth(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AzimuthAltitudeTransformNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* azimuthaltitudetransformnode . {'datatype': 'POINT', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.azimuthaltitudetransformnode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AzimuthAltitudeTransformNode (getter)");
            }
        }

#endregion
    }

}