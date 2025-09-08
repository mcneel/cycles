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

    public class MetallicBsdfNodeInputs : Inputs
    {
        public FloatSocket Anisotropy { get; private set; }
        public ColorSocket BaseColor { get; private set; }
        public VectorSocket IOR { get; private set; }
        public FloatSocket Rotation { get; private set; }
        public VectorSocket Extinction { get; private set; }
        public EnumSocket Distribution { get; private set; }
        public VectorSocket Tangent { get; private set; }
        public EnumSocket fresnel_type { get; private set; }
        public FloatSocket Roughness { get; private set; }
        public NormalSocket Normal { get; private set; }
        public ColorSocket EdgeTint { get; private set; }

        public MetallicBsdfNodeInputs(ShaderNode parentNode)
        {
            Anisotropy = new FloatSocket(parentNode, "Anisotropy", "anisotropy", true);
            AddSocket(Anisotropy);
            BaseColor = new ColorSocket(parentNode, "Base Color", "color", true);
            AddSocket(BaseColor);
            IOR = new VectorSocket(parentNode, "IOR", "ior", true);
            AddSocket(IOR);
            Rotation = new FloatSocket(parentNode, "Rotation", "rotation", true);
            AddSocket(Rotation);
            Extinction = new VectorSocket(parentNode, "Extinction", "k", true);
            AddSocket(Extinction);
            Distribution = new EnumSocket(parentNode, "Distribution", "distribution", true);
            AddSocket(Distribution);
            Tangent = new VectorSocket(parentNode, "Tangent", "tangent", true);
            AddSocket(Tangent);
            fresnel_type = new EnumSocket(parentNode, "fresnel_type", "fresnel_type", true);
            AddSocket(fresnel_type);
            Roughness = new FloatSocket(parentNode, "Roughness", "roughness", true);
            AddSocket(Roughness);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            EdgeTint = new ColorSocket(parentNode, "Edge Tint", "edge_tint", true);
            AddSocket(EdgeTint);
        }
    }
    public class MetallicBsdfNodeOutputs : Outputs
    {
        public ClosureSocket BSDF { get; private set; }

        public MetallicBsdfNodeOutputs(ShaderNode parentNode)
        {
            BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF", false);
            AddSocket(BSDF);
        }
    }

    [ShaderNode(name: "metallic_bsdf")]
    public class MetallicBsdfNode : BsdfNode
    {
        public enum MetallicBsdfNodeDistribution : uint {
            Ggx = ccl.ClosureType.CLOSURE_BSDF_MICROFACET_GGX_ID,
            Beckmann = ccl.ClosureType.CLOSURE_BSDF_MICROFACET_BECKMANN_ID,
            MultiGgx = ccl.ClosureType.CLOSURE_BSDF_MICROFACET_MULTI_GGX_ID,
        }
        public enum MetallicBsdfNodeFresnelType : uint {
            PhysicalConductor = ccl.ClosureType.CLOSURE_BSDF_PHYSICAL_CONDUCTOR,
            F82 = ccl.ClosureType.CLOSURE_BSDF_F82_CONDUCTOR,
        }
        public MetallicBsdfNodeInputs ins => (MetallicBsdfNodeInputs)inputs;
        public MetallicBsdfNodeOutputs outs => (MetallicBsdfNodeOutputs)outputs;
        public MetallicBsdfNode(Shader shader) : this(shader, "a metallic_bsdf node") { }

        public MetallicBsdfNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal MetallicBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new MetallicBsdfNodeInputs(this);
            outputs = new MetallicBsdfNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.BSDF;
        }
        public bool IsIsotropic() {
            return CSycles.metallicbsdfnode_is_isotropic(Ptr);
        }
        public static IntPtr GetNodeType() {
            return CSycles.metallicbsdfnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "anisotropy":
                    /* metallicbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                    {
                    CSycles.metallicbsdfnode_set_anisotropy(this.Ptr, data);
                    }
                    break;
            case "rotation":
                    /* metallicbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'rotation', 'ui_name': 'Rotation'} */
                    {
                    CSycles.metallicbsdfnode_set_rotation(this.Ptr, data);
                    }
                    break;
            case "roughness":
                    /* metallicbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                    {
                    CSycles.metallicbsdfnode_set_roughness(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "ior":
                    /* metallicbsdfnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(2.757f,2.513f,2.231f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'ior', 'ui_name': 'IOR'} */
                    {
                    CSycles.metallicbsdfnode_set_ior(this.Ptr, data);
                    }
                    break;
            case "k":
                    /* metallicbsdfnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(3.867f,3.404f,3.009f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'k', 'ui_name': 'Extinction'} */
                    {
                    CSycles.metallicbsdfnode_set_k(this.Ptr, data);
                    }
                    break;
            case "tangent":
                    /* metallicbsdfnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'tangent', 'ui_name': 'Tangent'} */
                    {
                    CSycles.metallicbsdfnode_set_tangent(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* metallicbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.bsdfnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* metallicbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.617f,0.577f,0.540f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Base Color'} */
                    {
                    CSycles.bsdfnode_set_color(this.Ptr, data);
                    }
                    break;
            case "edge_tint":
                    /* metallicbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.695f,0.726f,0.770f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'edge_tint', 'ui_name': 'Edge Tint'} */
                    {
                    CSycles.metallicbsdfnode_set_edge_tint(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "distribution":
                    /* metallicbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_MICROFACET_MULTI_GGX_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'distribution', 'ui_name': 'Distribution'} */
                    {
                    CSycles.metallicbsdfnode_set_distribution(this.Ptr, (ccl.ClosureType)data);
                    }
                    break;
            case "fresnel_type":
                    /* metallicbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_F82_CONDUCTOR', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'fresnel_type', 'ui_name': 'fresnel_type'} */
                    {
                    CSycles.metallicbsdfnode_set_fresnel_type(this.Ptr, (ccl.ClosureType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "anisotropy":
                /* metallicbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                {
                    return CSycles.metallicbsdfnode_get_anisotropy(this.Ptr);
                }
            case "rotation":
                /* metallicbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'rotation', 'ui_name': 'Rotation'} */
                {
                    return CSycles.metallicbsdfnode_get_rotation(this.Ptr);
                }
            case "roughness":
                /* metallicbsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                {
                    return CSycles.metallicbsdfnode_get_roughness(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "ior":
                /* metallicbsdfnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(2.757f,2.513f,2.231f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'ior', 'ui_name': 'IOR'} */
                {
                    return CSycles.metallicbsdfnode_get_ior(this.Ptr);
                }
            case "k":
                /* metallicbsdfnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(3.867f,3.404f,3.009f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'k', 'ui_name': 'Extinction'} */
                {
                    return CSycles.metallicbsdfnode_get_k(this.Ptr);
                }
            case "tangent":
                /* metallicbsdfnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'tangent', 'ui_name': 'Tangent'} */
                {
                    return CSycles.metallicbsdfnode_get_tangent(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* metallicbsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.bsdfnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* metallicbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.617f,0.577f,0.540f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Base Color'} */
                {
                    return CSycles.bsdfnode_get_color(this.Ptr);
                }
            case "edge_tint":
                /* metallicbsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.695f,0.726f,0.770f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'edge_tint', 'ui_name': 'Edge Tint'} */
                {
                    return CSycles.metallicbsdfnode_get_edge_tint(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "distribution":
                /* metallicbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_MICROFACET_MULTI_GGX_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'distribution', 'ui_name': 'Distribution'} */
                {
                    return (uint)CSycles.metallicbsdfnode_get_distribution(this.Ptr);
                }
            case "fresnel_type":
                /* metallicbsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_F82_CONDUCTOR', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'fresnel_type', 'ui_name': 'fresnel_type'} */
                {
                    return (uint)CSycles.metallicbsdfnode_get_fresnel_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type MetallicBsdfNode (getter)");
            }
        }

#endregion
    }

}