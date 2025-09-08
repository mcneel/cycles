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

    public class RhinoExposureTextureNodeInputs : Inputs
    {
        public FloatSocket WorldLuminance { get; private set; }
        public FloatSocket Exposure { get; private set; }
        public FloatSocket MaxLuminance { get; private set; }
        public FloatSocket Multiplier { get; private set; }
        public ColorSocket Color { get; private set; }

        public RhinoExposureTextureNodeInputs(ShaderNode parentNode)
        {
            WorldLuminance = new FloatSocket(parentNode, "WorldLuminance", "world_luminance", true);
            AddSocket(WorldLuminance);
            Exposure = new FloatSocket(parentNode, "Exposure", "exposure", true);
            AddSocket(Exposure);
            MaxLuminance = new FloatSocket(parentNode, "MaxLuminance", "max_luminance", true);
            AddSocket(MaxLuminance);
            Multiplier = new FloatSocket(parentNode, "Multiplier", "multiplier", true);
            AddSocket(Multiplier);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
        }
    }
    public class RhinoExposureTextureNodeOutputs : Outputs
    {
        public ColorSocket Color { get; private set; }
        public ColorSocket Alpha { get; private set; }

        public RhinoExposureTextureNodeOutputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "out_color", false);
            AddSocket(Color);
            Alpha = new ColorSocket(parentNode, "Alpha", "out_alpha", false);
            AddSocket(Alpha);
        }
    }

    [ShaderNode(name: "rhino_exposure_texture")]
    public class RhinoExposureTextureNode : ShaderNode
    {
        public RhinoExposureTextureNodeInputs ins => (RhinoExposureTextureNodeInputs)inputs;
        public RhinoExposureTextureNodeOutputs outs => (RhinoExposureTextureNodeOutputs)outputs;
        public RhinoExposureTextureNode(Shader shader) : this(shader, "a rhino_exposure_texture node") { }

        public RhinoExposureTextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal RhinoExposureTextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new RhinoExposureTextureNodeInputs(this);
            outputs = new RhinoExposureTextureNodeOutputs(this);
        }
        public float3 Color {
            get { return CSycles.rhinoexposuretexturenode_get_color(Ptr); }
            set { CSycles.rhinoexposuretexturenode_set_color(Ptr, value); }
        }

        public float Exposure {
            get { return CSycles.rhinoexposuretexturenode_get_exposure(Ptr); }
            set { CSycles.rhinoexposuretexturenode_set_exposure(Ptr, value); }
        }

        public float WorldLuminance {
            get { return CSycles.rhinoexposuretexturenode_get_world_luminance(Ptr); }
            set { CSycles.rhinoexposuretexturenode_set_world_luminance(Ptr, value); }
        }

        public float Multiplier {
            get { return CSycles.rhinoexposuretexturenode_get_multiplier(Ptr); }
            set { CSycles.rhinoexposuretexturenode_set_multiplier(Ptr, value); }
        }

        public float MaxLuminance {
            get { return CSycles.rhinoexposuretexturenode_get_max_luminance(Ptr); }
            set { CSycles.rhinoexposuretexturenode_set_max_luminance(Ptr, value); }
        }

        public static IntPtr GetNodeType() {
            return CSycles.rhinoexposuretexturenode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "world_luminance":
                    /* rhinoexposuretexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'world_luminance', 'ui_name': 'WorldLuminance'} */
                    {
                    CSycles.rhinoexposuretexturenode_set_world_luminance(this.Ptr, data);
                    }
                    break;
            case "exposure":
                    /* rhinoexposuretexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exposure', 'ui_name': 'Exposure'} */
                    {
                    CSycles.rhinoexposuretexturenode_set_exposure(this.Ptr, data);
                    }
                    break;
            case "max_luminance":
                    /* rhinoexposuretexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max_luminance', 'ui_name': 'MaxLuminance'} */
                    {
                    CSycles.rhinoexposuretexturenode_set_max_luminance(this.Ptr, data);
                    }
                    break;
            case "multiplier":
                    /* rhinoexposuretexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'multiplier', 'ui_name': 'Multiplier'} */
                    {
                    CSycles.rhinoexposuretexturenode_set_multiplier(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoExposureTextureNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* rhinoexposuretexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.rhinoexposuretexturenode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoExposureTextureNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "world_luminance":
                /* rhinoexposuretexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'world_luminance', 'ui_name': 'WorldLuminance'} */
                {
                    return CSycles.rhinoexposuretexturenode_get_world_luminance(this.Ptr);
                }
            case "exposure":
                /* rhinoexposuretexturenode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'exposure', 'ui_name': 'Exposure'} */
                {
                    return CSycles.rhinoexposuretexturenode_get_exposure(this.Ptr);
                }
            case "max_luminance":
                /* rhinoexposuretexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'max_luminance', 'ui_name': 'MaxLuminance'} */
                {
                    return CSycles.rhinoexposuretexturenode_get_max_luminance(this.Ptr);
                }
            case "multiplier":
                /* rhinoexposuretexturenode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'multiplier', 'ui_name': 'Multiplier'} */
                {
                    return CSycles.rhinoexposuretexturenode_get_multiplier(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoExposureTextureNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* rhinoexposuretexturenode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.0f,0.0f,0.0f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.rhinoexposuretexturenode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type RhinoExposureTextureNode (getter)");
            }
        }

#endregion
    }

}