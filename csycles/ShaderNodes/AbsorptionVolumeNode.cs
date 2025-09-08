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

    public class AbsorptionVolumeNodeInputs : Inputs
    {
        public FloatSocket Density { get; private set; }
        public ColorSocket Color { get; private set; }

        public AbsorptionVolumeNodeInputs(ShaderNode parentNode)
        {
            Density = new FloatSocket(parentNode, "Density", "density", true);
            AddSocket(Density);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    public class AbsorptionVolumeNodeOutputs : Outputs
    {
        public ClosureSocket Volume { get; private set; }

        public AbsorptionVolumeNodeOutputs(ShaderNode parentNode)
        {
            Volume = new ClosureSocket(parentNode, "Volume", "volume", false);
            AddSocket(Volume);
        }
    }

    [ShaderNode(name: "absorption_volume")]
    public class AbsorptionVolumeNode : VolumeNode
    {
        public AbsorptionVolumeNodeInputs ins => (AbsorptionVolumeNodeInputs)inputs;
        public AbsorptionVolumeNodeOutputs outs => (AbsorptionVolumeNodeOutputs)outputs;
        public AbsorptionVolumeNode(Shader shader) : this(shader, "a absorption_volume node") { }

        public AbsorptionVolumeNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal AbsorptionVolumeNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new AbsorptionVolumeNodeInputs(this);
            outputs = new AbsorptionVolumeNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.Volume;
        }
        public static IntPtr GetNodeType() {
            return CSycles.absorptionvolumenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "density":
                    /* absorptionvolumenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'density', 'ui_name': 'Density'} */
                    {
                    CSycles.volumenode_set_density(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AbsorptionVolumeNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* absorptionvolumenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.volumenode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AbsorptionVolumeNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "density":
                /* absorptionvolumenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'density', 'ui_name': 'Density'} */
                {
                    return CSycles.volumenode_get_density(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AbsorptionVolumeNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* absorptionvolumenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.volumenode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AbsorptionVolumeNode (getter)");
            }
        }

#endregion
    }

}