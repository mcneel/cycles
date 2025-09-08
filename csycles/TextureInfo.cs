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
    public class TextureInfo
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public TextureInfo() {}

        public TextureInfo(IntPtr intPtr) { Ptr = intPtr; }
        public uint Height {
            get { return CSycles.textureinfo_get_height(Ptr); }
            set { CSycles.textureinfo_set_height(Ptr, value); }
        }

        public uint Interpolation {
            get { return CSycles.textureinfo_get_interpolation(Ptr); }
            set { CSycles.textureinfo_set_interpolation(Ptr, value); }
        }

        public uint DataType {
            get { return CSycles.textureinfo_get_data_type(Ptr); }
            set { CSycles.textureinfo_set_data_type(Ptr, value); }
        }

        public uint Depth {
            get { return CSycles.textureinfo_get_depth(Ptr); }
            set { CSycles.textureinfo_set_depth(Ptr, value); }
        }

        public ulong Data {
            get { return CSycles.textureinfo_get_data(Ptr); }
            set { CSycles.textureinfo_set_data(Ptr, value); }
        }

        public uint UseTransform3d {
            get { return CSycles.textureinfo_get_use_transform_3d(Ptr); }
            set { CSycles.textureinfo_set_use_transform_3d(Ptr, value); }
        }

        public uint Extension {
            get { return CSycles.textureinfo_get_extension(Ptr); }
            set { CSycles.textureinfo_set_extension(Ptr, value); }
        }

        public Transform Transform3d {
            get { return CSycles.textureinfo_get_transform_3d(Ptr); }
            set { CSycles.textureinfo_set_transform_3d(Ptr, value); }
        }

        public uint Width {
            get { return CSycles.textureinfo_get_width(Ptr); }
            set { CSycles.textureinfo_set_width(Ptr, value); }
        }
    }

}