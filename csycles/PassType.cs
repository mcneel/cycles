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
    public enum PassType : uint
    {
        PASS_NONE = 0,
        PASS_COMBINED = 1,
        PASS_EMISSION = 2,
        PASS_BACKGROUND = 3,
        PASS_AO = 4,
        PASS_DIFFUSE = 5,
        PASS_DIFFUSE_DIRECT = 6,
        PASS_DIFFUSE_INDIRECT = 7,
        PASS_GLOSSY = 8,
        PASS_GLOSSY_DIRECT = 9,
        PASS_GLOSSY_INDIRECT = 10,
        PASS_TRANSMISSION = 11,
        PASS_TRANSMISSION_DIRECT = 12,
        PASS_TRANSMISSION_INDIRECT = 13,
        PASS_VOLUME = 14,
        PASS_VOLUME_DIRECT = 15,
        PASS_VOLUME_INDIRECT = 16,
        PASS_CATEGORY_LIGHT_END = 31,
        PASS_DEPTH = 32,
        PASS_POSITION = 33,
        PASS_NORMAL = 34,
        PASS_ROUGHNESS = 35,
        PASS_UV = 36,
        PASS_OBJECT_ID = 37,
        PASS_MATERIAL_ID = 38,
        PASS_MOTION = 39,
        PASS_MOTION_WEIGHT = 40,
        PASS_CRYPTOMATTE = 41,
        PASS_AOV_COLOR = 42,
        PASS_AOV_VALUE = 43,
        PASS_ADAPTIVE_AUX_BUFFER = 44,
        PASS_SAMPLE_COUNT = 45,
        PASS_SHADOW_CATCHER_TRANSPARENT_SAMPLE_COUNT = 46,
        PASS_SHADOW_CATCHER_BACKGROUND_SAMPLE_COUNT = 47,
        PASS_DIFFUSE_COLOR = 48,
        PASS_GLOSSY_COLOR = 49,
        PASS_TRANSMISSION_COLOR = 50,
        PASS_MIST = 51,
        PASS_DENOISING_NORMAL = 52,
        PASS_DENOISING_ALBEDO = 53,
        PASS_DENOISING_DEPTH = 54,
        PASS_DENOISING_PREVIOUS = 55,
        PASS_SHADOW_CATCHER = 56,
        PASS_SHADOW_CATCHER_SAMPLE_COUNT = 57,
        PASS_SHADOW_CATCHER_MATTE = 58,
        PASS_GUIDING_COLOR = 59,
        PASS_GUIDING_PROBABILITY = 60,
        PASS_GUIDING_AVG_ROUGHNESS = 61,
        PASS_CATEGORY_DATA_END = 63,
        PASS_BAKE_PRIMITIVE = 64,
        PASS_BAKE_SEED = 65,
        PASS_BAKE_DIFFERENTIAL = 66,
        PASS_CATEGORY_BAKE_END = 95,
        PASS_NUM = 96,
    }
}
