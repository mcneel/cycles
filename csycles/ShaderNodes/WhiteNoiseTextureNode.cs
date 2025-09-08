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

    public class WhiteNoiseTextureNodeInputs : Inputs
    {
        public PointSocket Vector { get; private set; }
        public EnumSocket Dimensions { get; private set; }
        public FloatSocket W { get; private set; }

        public WhiteNoiseTextureNodeInputs(ShaderNode parentNode)
        {
            Vector = new PointSocket(parentNode, "Vector", "vector", true);
            AddSocket(Vector);
            Dimensions = new EnumSocket(parentNode, "Dimensions", "dimensions", true);
            AddSocket(Dimensions);
            W = new FloatSocket(parentNode, "W", "w", true);
            AddSocket(W);
        }
    }
    public class WhiteNoiseTextureNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }
        public FloatSocket Value { get; private set; }

        public WhiteNoiseTextureNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", false);
            AddSocket(Color);
            Value = new FloatSocket(parentNode, "Value", "value", false);
            AddSocket(Value);
        }
    }

    [ShaderNode(name: "white_noise_texture")]
    public class WhiteNoiseTextureNode : ShaderNode
    {
        public enum WhiteNoiseTextureNodeDimensions : uint {
            Dim1d = ccl.Dimensions.DIM1D,
            Dim2d = ccl.Dimensions.DIM2D,
            Dim3d = ccl.Dimensions.DIM3D,
            Dim4d = ccl.Dimensions.DIM4D,
        }
        public WhiteNoiseTextureNodeInputs ins => (WhiteNoiseTextureNodeInputs)inputs;
        public WhiteNoiseTextureNodeOutputs outs => (WhiteNoiseTextureNodeOutputs)outputs;
        public WhiteNoiseTextureNode(Shader shader) : this(shader, "a white_noise_texture node") { }

        public WhiteNoiseTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal WhiteNoiseTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new WhiteNoiseTextureNodeInputs(this);
            outputs = new WhiteNoiseTextureNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.whitenoisetexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "w":
                    /* whitenoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'w', 'ui_name': 'W'} */
                    {
                    CSycles.whitenoisetexturenode_set_w(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WhiteNoiseTextureNode (setter)");
            }
        }

        internal override void SetPoint(string name, float3 data)
        {
            switch(name) {
            case "vector":
                    /* whitenoisetexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                    {
                    CSycles.whitenoisetexturenode_set_vector(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WhiteNoiseTextureNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "dimensions":
                    /* whitenoisetexturenode . {'datatype': 'ENUM', 'default_value': 'DIM3D', 'default_value_type': 'Dimensions', 'is_input': True, 'member_name': 'dimensions', 'ui_name': 'Dimensions'} */
                    {
                    CSycles.whitenoisetexturenode_set_dimensions(this.Ptr, (ccl.Dimensions)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WhiteNoiseTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "w":
                /* whitenoisetexturenode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'w', 'ui_name': 'W'} */
                {
                    return CSycles.whitenoisetexturenode_get_w(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WhiteNoiseTextureNode (getter)");
            }
        }

        internal override float3 GetPoint(string name)
        {
            switch(name) {
            case "vector":
                /* whitenoisetexturenode . {'datatype': 'POINT', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'vector', 'ui_name': 'Vector'} */
                {
                    return CSycles.whitenoisetexturenode_get_vector(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WhiteNoiseTextureNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "dimensions":
                /* whitenoisetexturenode . {'datatype': 'ENUM', 'default_value': 'DIM3D', 'default_value_type': 'Dimensions', 'is_input': True, 'member_name': 'dimensions', 'ui_name': 'Dimensions'} */
                {
                    return (uint)CSycles.whitenoisetexturenode_get_dimensions(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type WhiteNoiseTextureNode (getter)");
            }
        }

#endregion
    }

}