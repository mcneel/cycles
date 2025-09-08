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

    public class SheenBsdfNodeInputs : Inputs
    {
        public EnumSocket Distribution { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket Roughness { get; private set; }
        public NormalSocket Normal { get; private set; }

        public SheenBsdfNodeInputs(ShaderNode parentNode)
        {
            Distribution = new EnumSocket(parentNode, "Distribution", "distribution", true);
            AddSocket(Distribution);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Roughness = new FloatSocket(parentNode, "Roughness", "roughness", true);
            AddSocket(Roughness);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
        }
    }
    public class SheenBsdfNodeOutputs : Outputs
    {
        public ClosureSocket BSDF { get; private set; }

        public SheenBsdfNodeOutputs(ShaderNode parentNode)
        {
            BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF", false);
            AddSocket(BSDF);
        }
    }

    [ShaderNode(name: "sheen_bsdf")]
    public class SheenBsdfNode : BsdfNode
    {
        public enum SheenBsdfNodeDistribution : uint {
            Microfiber = ccl.ClosureType.CLOSURE_BSDF_SHEEN_ID,
            Ashikhmin = ccl.ClosureType.CLOSURE_BSDF_ASHIKHMIN_VELVET_ID,
        }
        public SheenBsdfNodeInputs ins => (SheenBsdfNodeInputs)inputs;
        public SheenBsdfNodeOutputs outs => (SheenBsdfNodeOutputs)outputs;
        public SheenBsdfNode(Shader shader) : this(shader, "a sheen_bsdf node") { }

        public SheenBsdfNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal SheenBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new SheenBsdfNodeInputs(this);
            outputs = new SheenBsdfNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.BSDF;
        }
        public static IntPtr GetNodeType() {
            return CSycles.sheenbsdfnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "roughness":
                    /* sheenbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                    {
                    CSycles.sheenbsdfnode_set_roughness(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SheenBsdfNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* sheenbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.bsdfnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SheenBsdfNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* sheenbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.bsdfnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SheenBsdfNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "distribution":
                    /* sheenbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_SHEEN_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'distribution', 'ui_name': 'Distribution'} */
                    {
                    CSycles.sheenbsdfnode_set_distribution(this.Ptr, (ccl.ClosureType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SheenBsdfNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "roughness":
                /* sheenbsdfnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                {
                    return CSycles.sheenbsdfnode_get_roughness(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SheenBsdfNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* sheenbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.bsdfnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SheenBsdfNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* sheenbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.bsdfnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SheenBsdfNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "distribution":
                /* sheenbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_SHEEN_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'distribution', 'ui_name': 'Distribution'} */
                {
                    return (uint)CSycles.sheenbsdfnode_get_distribution(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SheenBsdfNode (getter)");
            }
        }

#endregion
    }

}