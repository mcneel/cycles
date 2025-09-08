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
    public class TextureMapping
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public TextureMapping() {}

        public TextureMapping(IntPtr intPtr) { Ptr = intPtr; }
        public float3 Max {
            get { return CSycles.texturemapping_get_max(Ptr); }
            set { CSycles.texturemapping_set_max(Ptr, value); }
        }

        public float3 Translation {
            get { return CSycles.texturemapping_get_translation(Ptr); }
            set { CSycles.texturemapping_set_translation(Ptr, value); }
        }

        public TextureMapping_Mapping XMapping {
            get { return CSycles.texturemapping_get_x_mapping(Ptr); }
            set { CSycles.texturemapping_set_x_mapping(Ptr, value); }
        }

        public TextureMapping_Projection Projection {
            get { return CSycles.texturemapping_get_projection(Ptr); }
            set { CSycles.texturemapping_set_projection(Ptr, value); }
        }

        public bool UseMinmax {
            get { return CSycles.texturemapping_get_use_minmax(Ptr); }
            set { CSycles.texturemapping_set_use_minmax(Ptr, value); }
        }

        public TextureMapping_Mapping ZMapping {
            get { return CSycles.texturemapping_get_z_mapping(Ptr); }
            set { CSycles.texturemapping_set_z_mapping(Ptr, value); }
        }

        public float3 Min {
            get { return CSycles.texturemapping_get_min(Ptr); }
            set { CSycles.texturemapping_set_min(Ptr, value); }
        }

        public TextureMapping_Type Type {
            get { return CSycles.texturemapping_get_type(Ptr); }
            set { CSycles.texturemapping_set_type(Ptr, value); }
        }

        public Transform ComputeTransform() {
            return CSycles.texturemapping_compute_transform(Ptr);
        }

        public float3 Rotation {
            get { return CSycles.texturemapping_get_rotation(Ptr); }
            set { CSycles.texturemapping_set_rotation(Ptr, value); }
        }

        public float3 Scale {
            get { return CSycles.texturemapping_get_scale(Ptr); }
            set { CSycles.texturemapping_set_scale(Ptr, value); }
        }

        public bool Skip() {
            return CSycles.texturemapping_skip(Ptr);
        }

        public TextureMapping_Mapping YMapping {
            get { return CSycles.texturemapping_get_y_mapping(Ptr); }
            set { CSycles.texturemapping_set_y_mapping(Ptr, value); }
        }
    }

}