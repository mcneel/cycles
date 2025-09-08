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

    public class BumpNodeInputs : Inputs
    {
        public FloatSocket Strength { get; private set; }
        public FloatSocket SampleCenter { get; private set; }
        public FloatSocket Distance { get; private set; }
        public FloatSocket SampleX { get; private set; }
        public FloatSocket FilterWidth { get; private set; }
        public BoolSocket Invert { get; private set; }
        public FloatSocket SampleY { get; private set; }
        public BoolSocket UseObjectSpace { get; private set; }
        public NormalSocket Normal { get; private set; }
        public FloatSocket Height { get; private set; }

        public BumpNodeInputs(ShaderNode parentNode)
        {
            Strength = new FloatSocket(parentNode, "Strength", "strength", true);
            AddSocket(Strength);
            SampleCenter = new FloatSocket(parentNode, "SampleCenter", "sample_center", true);
            AddSocket(SampleCenter);
            Distance = new FloatSocket(parentNode, "Distance", "distance", true);
            AddSocket(Distance);
            SampleX = new FloatSocket(parentNode, "SampleX", "sample_x", true);
            AddSocket(SampleX);
            FilterWidth = new FloatSocket(parentNode, "Filter Width", "filter_width", true);
            AddSocket(FilterWidth);
            Invert = new BoolSocket(parentNode, "Invert", "invert", true);
            AddSocket(Invert);
            SampleY = new FloatSocket(parentNode, "SampleY", "sample_y", true);
            AddSocket(SampleY);
            UseObjectSpace = new BoolSocket(parentNode, "UseObjectSpace", "use_object_space", true);
            AddSocket(UseObjectSpace);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            Height = new FloatSocket(parentNode, "Height", "height", true);
            AddSocket(Height);
        }
    }
    public class BumpNodeOutputs : Outputs
    {
        public NormalSocket Normal { get; private set; }

        public BumpNodeOutputs(ShaderNode parentNode)
        {
            Normal = new NormalSocket(parentNode, "Normal", "normal", false);
            AddSocket(Normal);
        }
    }

    [ShaderNode(name: "bump")]
    public class BumpNode : ShaderNode
    {
        public BumpNodeInputs ins => (BumpNodeInputs)inputs;
        public BumpNodeOutputs outs => (BumpNodeOutputs)outputs;
        public BumpNode(Shader shader) : this(shader, "a bump node") { }

        public BumpNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal BumpNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new BumpNodeInputs(this);
            outputs = new BumpNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.bumpnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "strength":
                    /* bumpnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                    {
                    CSycles.bumpnode_set_strength(this.Ptr, data);
                    }
                    break;
            case "sample_center":
                    /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_center', 'ui_name': 'SampleCenter'} */
                    {
                    CSycles.bumpnode_set_sample_center(this.Ptr, data);
                    }
                    break;
            case "distance":
                    /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distance', 'ui_name': 'Distance'} */
                    {
                    CSycles.bumpnode_set_distance(this.Ptr, data);
                    }
                    break;
            case "sample_x":
                    /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_x', 'ui_name': 'SampleX'} */
                    {
                    CSycles.bumpnode_set_sample_x(this.Ptr, data);
                    }
                    break;
            case "filter_width":
                    /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'filter_width', 'ui_name': 'Filter Width'} */
                    {
                    CSycles.bumpnode_set_filter_width(this.Ptr, data);
                    }
                    break;
            case "sample_y":
                    /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_y', 'ui_name': 'SampleY'} */
                    {
                    CSycles.bumpnode_set_sample_y(this.Ptr, data);
                    }
                    break;
            case "height":
                    /* bumpnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'height', 'ui_name': 'Height'} */
                    {
                    CSycles.bumpnode_set_height(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BumpNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* bumpnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.bumpnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BumpNode (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "invert":
                    /* bumpnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'invert', 'ui_name': 'Invert'} */
                    {
                    CSycles.bumpnode_set_invert(this.Ptr, data);
                    }
                    break;
            case "use_object_space":
                    /* bumpnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_object_space', 'ui_name': 'UseObjectSpace'} */
                    {
                    CSycles.bumpnode_set_use_object_space(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BumpNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "strength":
                /* bumpnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'strength', 'ui_name': 'Strength'} */
                {
                    return CSycles.bumpnode_get_strength(this.Ptr);
                }
            case "sample_center":
                /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_center', 'ui_name': 'SampleCenter'} */
                {
                    return CSycles.bumpnode_get_sample_center(this.Ptr);
                }
            case "distance":
                /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'distance', 'ui_name': 'Distance'} */
                {
                    return CSycles.bumpnode_get_distance(this.Ptr);
                }
            case "sample_x":
                /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_x', 'ui_name': 'SampleX'} */
                {
                    return CSycles.bumpnode_get_sample_x(this.Ptr);
                }
            case "filter_width":
                /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'filter_width', 'ui_name': 'Filter Width'} */
                {
                    return CSycles.bumpnode_get_filter_width(this.Ptr);
                }
            case "sample_y":
                /* bumpnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_y', 'ui_name': 'SampleY'} */
                {
                    return CSycles.bumpnode_get_sample_y(this.Ptr);
                }
            case "height":
                /* bumpnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'height', 'ui_name': 'Height'} */
                {
                    return CSycles.bumpnode_get_height(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BumpNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* bumpnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.bumpnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BumpNode (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "invert":
                /* bumpnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'invert', 'ui_name': 'Invert'} */
                {
                    return CSycles.bumpnode_get_invert(this.Ptr);
                }
            case "use_object_space":
                /* bumpnode . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_object_space', 'ui_name': 'UseObjectSpace'} */
                {
                    return CSycles.bumpnode_get_use_object_space(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type BumpNode (getter)");
            }
        }

#endregion
    }

}