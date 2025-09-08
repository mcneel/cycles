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
namespace ccl.ShaderNodes
{
    using cclext;
    [ShaderNode(name: "texture_node", for_public_sdk: false)]
    public class TextureNode : ShaderNode
    {
        public TextureNode(Shader shader) : this(shader, "a texture_node node") { }

        public TextureNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal TextureNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
        }
        public void SetTexMappingMax(float3 value) {
            CSycles.texturenode_set_tex_mapping_max(Ptr, value);
        }

        public void SetTexMappingXMapping(TextureMapping_Mapping value) {
            CSycles.texturenode_set_tex_mapping_x_mapping(Ptr, value);
        }

        public void SetTexMappingScale(float3 value) {
            CSycles.texturenode_set_tex_mapping_scale(Ptr, value);
        }

        public float3 GetTexMappingRotation() {
            return CSycles.texturenode_get_tex_mapping_rotation(Ptr);
        }

        public void SetTexMappingRotation(float3 value) {
            CSycles.texturenode_set_tex_mapping_rotation(Ptr, value);
        }

        public float3 GetTexMappingTranslation() {
            return CSycles.texturenode_get_tex_mapping_translation(Ptr);
        }

        public float3 GetTexMappingMin() {
            return CSycles.texturenode_get_tex_mapping_min(Ptr);
        }

        public void SetTexMappingMin(float3 value) {
            CSycles.texturenode_set_tex_mapping_min(Ptr, value);
        }

        public void SetTexMappingYMapping(TextureMapping_Mapping value) {
            CSycles.texturenode_set_tex_mapping_y_mapping(Ptr, value);
        }

        public void SetTexMappingZMapping(TextureMapping_Mapping value) {
            CSycles.texturenode_set_tex_mapping_z_mapping(Ptr, value);
        }

        public float3 GetTexMappingScale() {
            return CSycles.texturenode_get_tex_mapping_scale(Ptr);
        }

        public void SetTexMappingProjection(TextureMapping_Projection value) {
            CSycles.texturenode_set_tex_mapping_projection(Ptr, value);
        }

        public void SetTexMappingType(TextureMapping_Type value) {
            CSycles.texturenode_set_tex_mapping_type(Ptr, value);
        }

        public bool GetTexMappingUseMinmax() {
            return CSycles.texturenode_get_tex_mapping_use_minmax(Ptr);
        }

        public void SetTexMappingUseMinmax(bool value) {
            CSycles.texturenode_set_tex_mapping_use_minmax(Ptr, value);
        }

        public float3 GetTexMappingMax() {
            return CSycles.texturenode_get_tex_mapping_max(Ptr);
        }

        public void SetTexMappingTranslation(float3 value) {
            CSycles.texturenode_set_tex_mapping_translation(Ptr, value);
        }
    }

}