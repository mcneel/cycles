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

Code generated at: 2025-11-21 07:20:37 UTC
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
    public class SocketType
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public SocketType() {}

        public SocketType(IntPtr intPtr) { Ptr = intPtr; }
        public IntPtr NodeType {
            get { return CSycles.sockettype_get_node_type(Ptr); }
            set { CSycles.sockettype_set_node_type(Ptr, value); }
        }

        public string UiName {
            get { return CSycles.sockettype_get_ui_name(Ptr); }
            set { CSycles.sockettype_set_ui_name(Ptr, value); }
        }

        public static string TypeName(Type type) {
            return CSycles.sockettype_type_name(type);
        }

        public static bool IsFloat3(Type type) {
            return CSycles.sockettype_is_float3(type);
        }

        public static IntPtr ZeroDefaultValue() {
            return CSycles.sockettype_zero_default_value();
        }

        public ulong ModifiedFlagBit {
            get { return CSycles.sockettype_get_modified_flag_bit(Ptr); }
            set { CSycles.sockettype_set_modified_flag_bit(Ptr, value); }
        }

        public SocketType_Type Type {
            get { return CSycles.sockettype_get_type(Ptr); }
            set { CSycles.sockettype_set_type(Ptr, value); }
        }

        public int StructOffset {
            get { return CSycles.sockettype_get_struct_offset(Ptr); }
            set { CSycles.sockettype_set_struct_offset(Ptr, value); }
        }

        public bool IsArray() {
            return CSycles.sockettype_is_array(Ptr);
        }

        public static long Size(Type type) {
            return CSycles.sockettype_size(type);
        }

        public IntPtr DefaultValue {
            get { return CSycles.sockettype_get_default_value(Ptr); }
            set { CSycles.sockettype_set_default_value(Ptr, value); }
        }

        public int Flags {
            get { return CSycles.sockettype_get_flags(Ptr); }
            set { CSycles.sockettype_set_flags(Ptr, value); }
        }

        public static long MaxSize() {
            return CSycles.sockettype_max_size();
        }

        public IntPtr EnumValues {
            get { return CSycles.sockettype_get_enum_values(Ptr); }
            set { CSycles.sockettype_set_enum_values(Ptr, value); }
        }

        public long Size1() {
            return CSycles.sockettype_size_1(Ptr);
        }
    }

}