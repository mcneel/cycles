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
Code generated at: 2025-11-25 12:48:00 UTC
----------------------------------------------------------------------
**/
namespace ccl
{
    public enum BVHLayout : uint
    {
        BVH_LAYOUT_NONE = 0,
        BVH_LAYOUT_BVH2 = 1,
        BVH_LAYOUT_EMBREE = 2,
        BVH_LAYOUT_OPTIX = 4,
        BVH_LAYOUT_MULTI_OPTIX = 8,
        BVH_LAYOUT_MULTI_OPTIX_EMBREE = 16,
        BVH_LAYOUT_METAL = 32,
        BVH_LAYOUT_MULTI_METAL = 64,
        BVH_LAYOUT_MULTI_METAL_EMBREE = 128,
        BVH_LAYOUT_HIPRT = 256,
        BVH_LAYOUT_MULTI_HIPRT = 512,
        BVH_LAYOUT_MULTI_HIPRT_EMBREE = 1024,
        BVH_LAYOUT_EMBREEGPU = 2048,
        BVH_LAYOUT_MULTI_EMBREEGPU = 4096,
        BVH_LAYOUT_MULTI_EMBREEGPU_EMBREE = 8192,
        BVH_LAYOUT_AUTO = 2,
        BVH_LAYOUT_ALL = 16350,
    }
}
