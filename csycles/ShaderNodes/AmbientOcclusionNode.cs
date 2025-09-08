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

    public class AmbientOcclusionNodeInputs : Inputs
    {
        public BoolSocket Inside { get; private set; }
        public FloatSocket Distance { get; private set; }
        public IntSocket Samples { get; private set; }
        public BoolSocket OnlyLocal { get; private set; }
        public NormalSocket Normal { get; private set; }
        public ColorSocket Color { get; private set; }

        public AmbientOcclusionNodeInputs(ShaderNode parentNode)
        {
            Inside = new BoolSocket(parentNode, "Inside", "inside", true);
            AddSocket(Inside);
            Distance = new FloatSocket(parentNode, "Distance", "distance", true);
            AddSocket(Distance);
            Samples = new IntSocket(parentNode, "Samples", "samples", true);
            AddSocket(Samples);
            OnlyLocal = new BoolSocket(parentNode, "Only Local", "only_local", true);
            AddSocket(OnlyLocal);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    public class AmbientOcclusionNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }
        public FloatSocket AO { get; private set; }

        public AmbientOcclusionNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
            AO = new FloatSocket(parentNode, "AO", "ao", false);
            AddSocket(AO);
        }
    }

    [ShaderNode(name: "ambient_occlusion")]
    public class AmbientOcclusionNode : ShaderNode
    {
        public AmbientOcclusionNodeInputs ins => (AmbientOcclusionNodeInputs)inputs;
        public AmbientOcclusionNodeOutputs outs => (AmbientOcclusionNodeOutputs)outputs;
        public AmbientOcclusionNode(Shader shader) : this(shader, "a ambient_occlusion node") { }

        public AmbientOcclusionNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal AmbientOcclusionNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new AmbientOcclusionNodeInputs(this);
            outputs = new AmbientOcclusionNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.ambientocclusionnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "distance":
                    /* ambientocclusionnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distance', 'ui_name': 'Distance'} */
                    {
                    CSycles.ambientocclusionnode_set_distance(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* ambientocclusionnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.ambientocclusionnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* ambientocclusionnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.ambientocclusionnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "inside":
                    /* ambientocclusionnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'inside', 'ui_name': 'Inside'} */
                    {
                    CSycles.ambientocclusionnode_set_inside(this.Ptr, data);
                    }
                    break;
            case "only_local":
                    /* ambientocclusionnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'only_local', 'ui_name': 'Only Local'} */
                    {
                    CSycles.ambientocclusionnode_set_only_local(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "samples":
                    /* ambientocclusionnode . {'datatype': 'INT', 'default_value': '16', 'default_value_type': 'int', 'is_input': True, 'member_name': 'samples', 'ui_name': 'Samples'} */
                    {
                    CSycles.ambientocclusionnode_set_samples(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "distance":
                /* ambientocclusionnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distance', 'ui_name': 'Distance'} */
                {
                    return CSycles.ambientocclusionnode_get_distance(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* ambientocclusionnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.ambientocclusionnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* ambientocclusionnode . {'datatype': 'COLOR', 'default_value': 'one_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.ambientocclusionnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "inside":
                /* ambientocclusionnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'inside', 'ui_name': 'Inside'} */
                {
                    return CSycles.ambientocclusionnode_get_inside(this.Ptr);
                }
            case "only_local":
                /* ambientocclusionnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'only_local', 'ui_name': 'Only Local'} */
                {
                    return CSycles.ambientocclusionnode_get_only_local(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "samples":
                /* ambientocclusionnode . {'datatype': 'INT', 'default_value': '16', 'default_value_type': 'int', 'is_input': True, 'member_name': 'samples', 'ui_name': 'Samples'} */
                {
                    return CSycles.ambientocclusionnode_get_samples(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type AmbientOcclusionNode (getter)");
            }
        }

#endregion
    }

}