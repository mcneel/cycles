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
Code generated at: 2025-11-25 13:56:53 UTC
----------------------------------------------------------------------
**/
namespace ccl
{
    public enum PathRayFlag : uint
    {
        PATH_RAY_SHADOW_CATCHER_BACKGROUND = 2147483648,
        PATH_RAY_CAMERA = 1,
        PATH_RAY_REFLECT = 2,
        PATH_RAY_TRANSMIT = 4,
        PATH_RAY_DIFFUSE = 8,
        PATH_RAY_GLOSSY = 16,
        PATH_RAY_SINGULAR = 32,
        PATH_RAY_TRANSPARENT = 64,
        PATH_RAY_VOLUME_SCATTER = 128,
        PATH_RAY_IMPORTANCE_BAKE = 256,
        PATH_RAY_SHADOW_OPAQUE = 512,
        PATH_RAY_SHADOW_TRANSPARENT = 1024,
        PATH_RAY_SHADOW = 1536,
        PATH_RAY_ALL_VISIBILITY = 2047,
        PATH_RAY_MIS_HAD_TRANSMISSION = 2048,
        PATH_RAY_NODE_UNALIGNED = 2048,
        PATH_RAY_MIS_SKIP = 4096,
        PATH_RAY_DIFFUSE_ANCESTOR = 8192,
        PATH_RAY_SINGLE_PASS_DONE = 16384,
        PATH_RAY_TRANSPARENT_BACKGROUND = 32768,
        PATH_RAY_TERMINATE_ON_NEXT_SURFACE = 65536,
        PATH_RAY_TERMINATE_IN_NEXT_VOLUME = 131072,
        PATH_RAY_TERMINATE_AFTER_TRANSPARENT = 262144,
        PATH_RAY_TERMINATE_AFTER_VOLUME = 524288,
        PATH_RAY_TERMINATE = 983040,
        PATH_RAY_EMISSION = 1048576,
        PATH_RAY_SUBSURFACE_RANDOM_WALK = 2097152,
        PATH_RAY_SUBSURFACE_DISK = 4194304,
        PATH_RAY_SUBSURFACE_BACKFACING = 16777216,
        PATH_RAY_SUBSURFACE = 23068672,
        PATH_RAY_DENOISING_FEATURES = 33554432,
        PATH_RAY_SURFACE_PASS = 67108864,
        PATH_RAY_VOLUME_PASS = 134217728,
        PATH_RAY_ANY_PASS = 201326592,
        PATH_RAY_SHADOW_FOR_AO = 268435456,
        PATH_RAY_SHADOW_CATCHER_HIT = 536870912,
        PATH_RAY_SHADOW_CATCHER_PASS = 1073741824,
    }
}
