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
    public enum ShaderFlag : uint
    {
        SHADER_SMOOTH_NORMAL = 2147483648,
        SHADER_MASK = 4194303,
        SHADER_EXCLUDE_SHADOW_CATCHER = 4194304,
        SHADER_EXCLUDE_SCATTER = 8388608,
        SHADER_EXCLUDE_CAMERA = 16777216,
        SHADER_EXCLUDE_TRANSMIT = 33554432,
        SHADER_EXCLUDE_GLOSSY = 67108864,
        SHADER_EXCLUDE_DIFFUSE = 134217728,
        SHADER_EXCLUDE_ANY = 264241152,
        SHADER_USE_MIS = 268435456,
        SHADER_AREA_LIGHT = 536870912,
        SHADER_CAST_SHADOW = 1073741824,
    }
}
