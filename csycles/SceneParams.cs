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
    public class SceneParams
    {
        public IntPtr Ptr { get; private set; } = IntPtr.Zero;

        public SceneParams() {}

        public SceneParams(IntPtr intPtr) { Ptr = intPtr; }
        public bool UseBvhSpatialSplit {
            get { return CSycles.sceneparams_get_use_bvh_spatial_split(Ptr); }
            set { CSycles.sceneparams_set_use_bvh_spatial_split(Ptr, value); }
        }

        public bool UseBvhCompactStructure {
            get { return CSycles.sceneparams_get_use_bvh_compact_structure(Ptr); }
            set { CSycles.sceneparams_set_use_bvh_compact_structure(Ptr, value); }
        }

        public CurveShapeType HairShape {
            get { return CSycles.sceneparams_get_hair_shape(Ptr); }
            set { CSycles.sceneparams_set_hair_shape(Ptr, value); }
        }

        public int TextureLimit {
            get { return CSycles.sceneparams_get_texture_limit(Ptr); }
            set { CSycles.sceneparams_set_texture_limit(Ptr, value); }
        }

        public BVHType BvhType {
            get { return CSycles.sceneparams_get_bvh_type(Ptr); }
            set { CSycles.sceneparams_set_bvh_type(Ptr, value); }
        }

        public int NumBvhTimeSteps {
            get { return CSycles.sceneparams_get_num_bvh_time_steps(Ptr); }
            set { CSycles.sceneparams_set_num_bvh_time_steps(Ptr, value); }
        }

        public ShadingSystem Shadingsystem {
            get { return CSycles.sceneparams_get_shadingsystem(Ptr); }
            set { CSycles.sceneparams_set_shadingsystem(Ptr, value); }
        }

        public bool UseBvhUnalignedNodes {
            get { return CSycles.sceneparams_get_use_bvh_unaligned_nodes(Ptr); }
            set { CSycles.sceneparams_set_use_bvh_unaligned_nodes(Ptr, value); }
        }

        public bool Background {
            get { return CSycles.sceneparams_get_background(Ptr); }
            set { CSycles.sceneparams_set_background(Ptr, value); }
        }

        public BVHLayout BvhLayout {
            get { return CSycles.sceneparams_get_bvh_layout(Ptr); }
            set { CSycles.sceneparams_set_bvh_layout(Ptr, value); }
        }
    }

}