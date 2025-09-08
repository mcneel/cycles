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
namespace ccl
{
    using cclext;

    public class BackgroundNodeInputs : NodeInputs
    {
        public FloatNodeSocket TransparentRoughnessThreshold { get; private set; }
        public StringNodeSocket LightGroup { get; private set; }
        public UintNodeSocket Visibility { get; private set; }
        public BoolNodeSocket TransparentGlass { get; private set; }
        public BoolNodeSocket UseShader { get; private set; }
        public NodeNodeSocket Shader { get; private set; }
        public BoolNodeSocket Transparent { get; private set; }
        public FloatNodeSocket VolumeStepSize { get; private set; }

        public BackgroundNodeInputs(Node parentNode)
        {
            TransparentRoughnessThreshold = new FloatNodeSocket(parentNode, "Transparent Roughness Threshold", "transparent_roughness_threshold", true);
            AddSocket(TransparentRoughnessThreshold);
            LightGroup = new StringNodeSocket(parentNode, "Light Group", "lightgroup", true);
            AddSocket(LightGroup);
            Visibility = new UintNodeSocket(parentNode, "Visibility", "visibility", true);
            AddSocket(Visibility);
            TransparentGlass = new BoolNodeSocket(parentNode, "Transparent Glass", "transparent_glass", true);
            AddSocket(TransparentGlass);
            UseShader = new BoolNodeSocket(parentNode, "Use Shader", "use_shader", true);
            AddSocket(UseShader);
            Shader = new NodeNodeSocket(parentNode, "Shader", "shader", true);
            AddSocket(Shader);
            Transparent = new BoolNodeSocket(parentNode, "Transparent", "transparent", true);
            AddSocket(Transparent);
            VolumeStepSize = new FloatNodeSocket(parentNode, "Volume Step Size", "volume_step_size", true);
            AddSocket(VolumeStepSize);
        }
    }
    [Node("background")]
    public class Background : Node
    {
        public BackgroundNodeInputs BackgroundNodeInputs { get; set; }
        public BackgroundNodeInputs ins => BackgroundNodeInputs;

        public Background() : this("a background node") { }

        public Background(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Background(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            BackgroundNodeInputs = new BackgroundNodeInputs(this);

        }
        public static IntPtr GetNodeType() {
            return CSycles.background_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "transparent_roughness_threshold":
                    /* background . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'transparent_roughness_threshold', 'ui_name': 'Transparent Roughness Threshold'} */
                    {
                    CSycles.background_set_transparent_roughness_threshold(this.Ptr, data);
                    }
                    break;
            case "volume_step_size":
                    /* background . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'volume_step_size', 'ui_name': 'Volume Step Size'} */
                    {
                    CSycles.background_set_volume_step_size(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "transparent_glass":
                    /* background . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'transparent_glass', 'ui_name': 'Transparent Glass'} */
                    {
                    CSycles.background_set_transparent_glass(this.Ptr, data);
                    }
                    break;
            case "use_shader":
                    /* background . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_shader', 'ui_name': 'Use Shader'} */
                    {
                    CSycles.background_set_use_shader(this.Ptr, data);
                    }
                    break;
            case "transparent":
                    /* background . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'transparent', 'ui_name': 'Transparent'} */
                    {
                    CSycles.background_set_transparent(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (setter)");
            }
        }

        internal override void SetUint(string name, uint data)
        {
            switch(name) {
            case "visibility":
                    /* background . {'datatype': 'UINT', 'default_value': 'PATH_RAY_ALL_VISIBILITY', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'visibility', 'ui_name': 'Visibility'} */
                    {
                    CSycles.background_set_visibility(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "lightgroup":
                    /* background . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'lightgroup', 'ui_name': 'Light Group'} */
                    {
                    CSycles.background_set_lightgroup(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (setter)");
            }
        }

        internal override void SetNode(string name, IntPtr data)
        {
            switch(name) {
            case "shader":
                    /* background . {'datatype': 'NODE', 'default_value': '', 'default_value_type': '', 'is_input': True, 'member_name': 'shader', 'ui_name': 'Shader'} */
                    {
                    CSycles.background_set_shader(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "transparent_roughness_threshold":
                /* background . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'transparent_roughness_threshold', 'ui_name': 'Transparent Roughness Threshold'} */
                {
                    return CSycles.background_get_transparent_roughness_threshold(this.Ptr);
                }
            case "volume_step_size":
                /* background . {'datatype': 'FLOAT', 'default_value': '0.1f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'volume_step_size', 'ui_name': 'Volume Step Size'} */
                {
                    return CSycles.background_get_volume_step_size(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "transparent_glass":
                /* background . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'transparent_glass', 'ui_name': 'Transparent Glass'} */
                {
                    return CSycles.background_get_transparent_glass(this.Ptr);
                }
            case "use_shader":
                /* background . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_shader', 'ui_name': 'Use Shader'} */
                {
                    return CSycles.background_get_use_shader(this.Ptr);
                }
            case "transparent":
                /* background . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'transparent', 'ui_name': 'Transparent'} */
                {
                    return CSycles.background_get_transparent(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (getter)");
            }
        }

        internal override uint GetUint(string name)
        {
            switch(name) {
            case "visibility":
                /* background . {'datatype': 'UINT', 'default_value': 'PATH_RAY_ALL_VISIBILITY', 'default_value_type': 'uint', 'is_input': True, 'member_name': 'visibility', 'ui_name': 'Visibility'} */
                {
                    return CSycles.background_get_visibility(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "lightgroup":
                /* background . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'lightgroup', 'ui_name': 'Light Group'} */
                {
                    return CSycles.background_get_lightgroup(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (getter)");
            }
        }

        internal override IntPtr GetNode(string name)
        {
            switch(name) {
            case "shader":
                /* background . {'datatype': 'NODE', 'default_value': '', 'default_value_type': '', 'is_input': True, 'member_name': 'shader', 'ui_name': 'Shader'} */
                {
                    return CSycles.background_get_shader(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Background (getter)");
            }
        }

#endregion
    }

}