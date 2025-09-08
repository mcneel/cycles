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

    public class LayerWeightNodeInputs : Inputs
    {
        public NormalSocket Normal { get; private set; }
        public FloatSocket Blend { get; private set; }

        public LayerWeightNodeInputs(ShaderNode parentNode)
        {
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            Blend = new FloatSocket(parentNode, "Blend", "blend", true);
            AddSocket(Blend);
        }
    }
    public class LayerWeightNodeOutputs : Outputs
    {
        public FloatSocket Facing { get; private set; }
        public FloatSocket Fresnel { get; private set; }

        public LayerWeightNodeOutputs(ShaderNode parentNode)
        {
            Facing = new FloatSocket(parentNode, "Facing", "facing", false);
            AddSocket(Facing);
            Fresnel = new FloatSocket(parentNode, "Fresnel", "fresnel", false);
            AddSocket(Fresnel);
        }
    }

    [ShaderNode(name: "layer_weight")]
    public class LayerWeightNode : ShaderNode
    {
        public LayerWeightNodeInputs ins => (LayerWeightNodeInputs)inputs;
        public LayerWeightNodeOutputs outs => (LayerWeightNodeOutputs)outputs;
        public LayerWeightNode(Shader shader) : this(shader, "a layer_weight node") { }

        public LayerWeightNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal LayerWeightNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new LayerWeightNodeInputs(this);
            outputs = new LayerWeightNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.layerweightnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "blend":
                    /* layerweightnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'blend', 'ui_name': 'Blend'} */
                    {
                    CSycles.layerweightnode_set_blend(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type LayerWeightNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* layerweightnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.layerweightnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type LayerWeightNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "blend":
                /* layerweightnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'blend', 'ui_name': 'Blend'} */
                {
                    return CSycles.layerweightnode_get_blend(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type LayerWeightNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* layerweightnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.layerweightnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type LayerWeightNode (getter)");
            }
        }

#endregion
    }

}