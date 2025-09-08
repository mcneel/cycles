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

    public class GlossyBsdfNodeInputs : Inputs
    {
        public FloatSocket Anisotropy { get; private set; }
        public FloatSocket Rotation { get; private set; }
        public EnumSocket Distribution { get; private set; }
        public VectorSocket Tangent { get; private set; }
        public ColorSocket Color { get; private set; }
        public FloatSocket Roughness { get; private set; }
        public NormalSocket Normal { get; private set; }

        public GlossyBsdfNodeInputs(ShaderNode parentNode)
        {
            Anisotropy = new FloatSocket(parentNode, "Anisotropy", "anisotropy", true);
            AddSocket(Anisotropy);
            Rotation = new FloatSocket(parentNode, "Rotation", "rotation", true);
            AddSocket(Rotation);
            Distribution = new EnumSocket(parentNode, "Distribution", "distribution", true);
            AddSocket(Distribution);
            Tangent = new VectorSocket(parentNode, "Tangent", "tangent", true);
            AddSocket(Tangent);
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Roughness = new FloatSocket(parentNode, "Roughness", "roughness", true);
            AddSocket(Roughness);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
        }
    }
    public class GlossyBsdfNodeOutputs : Outputs
    {
        public ClosureSocket BSDF { get; private set; }

        public GlossyBsdfNodeOutputs(ShaderNode parentNode)
        {
            BSDF = new ClosureSocket(parentNode, "BSDF", "BSDF", false);
            AddSocket(BSDF);
        }
    }

    [ShaderNode(name: "glossy_bsdf")]
    public class GlossyBsdfNode : BsdfNode
    {
        public enum GlossyBsdfNodeDistribution : uint {
            Ggx = ccl.ClosureType.CLOSURE_BSDF_MICROFACET_GGX_ID,
            Beckmann = ccl.ClosureType.CLOSURE_BSDF_MICROFACET_BECKMANN_ID,
            MultiGgx = ccl.ClosureType.CLOSURE_BSDF_MICROFACET_MULTI_GGX_ID,
            AshikhminShirley = ccl.ClosureType.CLOSURE_BSDF_ASHIKHMIN_SHIRLEY_ID,
        }
        public GlossyBsdfNodeInputs ins => (GlossyBsdfNodeInputs)inputs;
        public GlossyBsdfNodeOutputs outs => (GlossyBsdfNodeOutputs)outputs;
        public GlossyBsdfNode(Shader shader) : this(shader, "a glossy_bsdf node") { }

        public GlossyBsdfNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal GlossyBsdfNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new GlossyBsdfNodeInputs(this);
            outputs = new GlossyBsdfNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.BSDF;
        }
        public static IntPtr GetNodeType() {
            return CSycles.glossybsdfnode_get_node_type();
        }
        public bool IsIsotropic() {
            return CSycles.glossybsdfnode_is_isotropic(Ptr);
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "anisotropy":
                    /* glossybsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                    {
                    CSycles.glossybsdfnode_set_anisotropy(this.Ptr, data);
                    }
                    break;
            case "rotation":
                    /* glossybsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'rotation', 'ui_name': 'Rotation'} */
                    {
                    CSycles.glossybsdfnode_set_rotation(this.Ptr, data);
                    }
                    break;
            case "roughness":
                    /* glossybsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                    {
                    CSycles.glossybsdfnode_set_roughness(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "tangent":
                    /* glossybsdfnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'tangent', 'ui_name': 'Tangent'} */
                    {
                    CSycles.glossybsdfnode_set_tangent(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* glossybsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.bsdfnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* glossybsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.bsdfnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "distribution":
                    /* glossybsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_MICROFACET_GGX_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'distribution', 'ui_name': 'Distribution'} */
                    {
                    CSycles.glossybsdfnode_set_distribution(this.Ptr, (ccl.ClosureType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "anisotropy":
                /* glossybsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'anisotropy', 'ui_name': 'Anisotropy'} */
                {
                    return CSycles.glossybsdfnode_get_anisotropy(this.Ptr);
                }
            case "rotation":
                /* glossybsdfnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'rotation', 'ui_name': 'Rotation'} */
                {
                    return CSycles.glossybsdfnode_get_rotation(this.Ptr);
                }
            case "roughness":
                /* glossybsdfnode . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'roughness', 'ui_name': 'Roughness'} */
                {
                    return CSycles.glossybsdfnode_get_roughness(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "tangent":
                /* glossybsdfnode . {'datatype': 'VECTOR', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'tangent', 'ui_name': 'Tangent'} */
                {
                    return CSycles.glossybsdfnode_get_tangent(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* glossybsdfnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.bsdfnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* glossybsdfnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.bsdfnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "distribution":
                /* glossybsdfnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSDF_MICROFACET_GGX_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'distribution', 'ui_name': 'Distribution'} */
                {
                    return (uint)CSycles.glossybsdfnode_get_distribution(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type GlossyBsdfNode (getter)");
            }
        }

#endregion
    }

}