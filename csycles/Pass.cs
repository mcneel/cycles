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

    public class PassNodeInputs : NodeInputs
    {
        public StringNodeSocket Name { get; private set; }
        public BoolNodeSocket IncludeAlbedo { get; private set; }
        public EnumNodeSocket Mode { get; private set; }
        public StringNodeSocket LightGroup { get; private set; }
        public EnumNodeSocket Type { get; private set; }

        public PassNodeInputs(Node parentNode)
        {
            Name = new StringNodeSocket(parentNode, "Name", "name", true);
            AddSocket(Name);
            IncludeAlbedo = new BoolNodeSocket(parentNode, "Include Albedo", "include_albedo", true);
            AddSocket(IncludeAlbedo);
            Mode = new EnumNodeSocket(parentNode, "Mode", "mode", true);
            AddSocket(Mode);
            LightGroup = new StringNodeSocket(parentNode, "Light Group", "lightgroup", true);
            AddSocket(LightGroup);
            Type = new EnumNodeSocket(parentNode, "Type", "type", true);
            AddSocket(Type);
        }
    }
    [Node("pass")]
    public class Pass : Node
    {
        public PassNodeInputs PassNodeInputs { get; set; }
        public PassNodeInputs ins => PassNodeInputs;

        public Pass() : this("a pass node") { }

        public Pass(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Pass(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            PassNodeInputs = new PassNodeInputs(this);

        }
        public bool IsWritten() {
            return CSycles.pass_is_written(Ptr);
        }
        public static IntPtr GetTypeEnum() {
            return CSycles.pass_get_type_enum();
        }

        public static IntPtr GetNodeType() {
            return CSycles.pass_get_node_type();
        }
        public static IntPtr GetModeEnum() {
            return CSycles.pass_get_mode_enum();
        }
#region Setters

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "include_albedo":
                    /* pass . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'include_albedo', 'ui_name': 'Include Albedo'} */
                    {
                    CSycles.pass_set_include_albedo(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Pass (setter)");
            }
        }

        internal override void SetString(string name, string data)
        {
            switch(name) {
            case "name":
                    /* pass . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'name', 'ui_name': 'Name'} */
                    {
                    CSycles.pass_set_name(this.Ptr, data);
                    }
                    break;
            case "lightgroup":
                    /* pass . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'lightgroup', 'ui_name': 'Light Group'} */
                    {
                    CSycles.pass_set_lightgroup(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Pass (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "mode":
                    /* pass . {'datatype': 'ENUM', 'default_value': 'PassMode::DENOISED', 'default_value_type': 'PassMode', 'is_input': True, 'member_name': 'mode', 'ui_name': 'Mode'} */
                    {
                    CSycles.pass_set_mode(this.Ptr, (ccl.PassMode)data);
                    }
                    break;
            case "type":
                    /* pass . {'datatype': 'ENUM', 'default_value': 'PASS_COMBINED', 'default_value_type': 'PassType', 'is_input': True, 'member_name': 'type', 'ui_name': 'Type'} */
                    {
                    CSycles.pass_set_type(this.Ptr, (ccl.PassType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Pass (setter)");
            }
        }

#endregion
#region Getters

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "include_albedo":
                /* pass . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'include_albedo', 'ui_name': 'Include Albedo'} */
                {
                    return CSycles.pass_get_include_albedo(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Pass (getter)");
            }
        }

        internal override string GetString(string name)
        {
            switch(name) {
            case "name":
                /* pass . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'name', 'ui_name': 'Name'} */
                {
                    return CSycles.pass_get_name(this.Ptr);
                }
            case "lightgroup":
                /* pass . {'datatype': 'STRING', 'default_value': 'ustring()', 'default_value_type': 'ustring', 'is_input': True, 'member_name': 'lightgroup', 'ui_name': 'Light Group'} */
                {
                    return CSycles.pass_get_lightgroup(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Pass (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "mode":
                /* pass . {'datatype': 'ENUM', 'default_value': 'PassMode::DENOISED', 'default_value_type': 'PassMode', 'is_input': True, 'member_name': 'mode', 'ui_name': 'Mode'} */
                {
                    return (uint)CSycles.pass_get_mode(this.Ptr);
                }
            case "type":
                /* pass . {'datatype': 'ENUM', 'default_value': 'PASS_COMBINED', 'default_value_type': 'PassType', 'is_input': True, 'member_name': 'type', 'ui_name': 'Type'} */
                {
                    return (uint)CSycles.pass_get_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Pass (getter)");
            }
        }

#endregion
    }

}