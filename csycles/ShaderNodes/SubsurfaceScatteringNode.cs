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

    public class SubsurfaceScatteringNodeInputs : Inputs
    {
        public ColorSocket Color { get; private set; }
        public VectorSocket Radius { get; private set; }
        public NormalSocket Normal { get; private set; }
        public FloatSocket IOR { get; private set; }
        public FloatSocket Roughness { get; private set; }
        public EnumSocket Method { get; private set; }
        public FloatSocket Anisotropy { get; private set; }
        public FloatSocket Scale { get; private set; }

        public SubsurfaceScatteringNodeInputs(ShaderNode parentNode)
        {
            Color = new ColorSocket(parentNode, "Color", "color", true);
            AddSocket(Color);
            Radius = new VectorSocket(parentNode, "Radius", "radius", true);
            AddSocket(Radius);
            Normal = new NormalSocket(parentNode, "Normal", "normal", true);
            AddSocket(Normal);
            IOR = new FloatSocket(parentNode, "IOR", "subsurface_ior", true);
            AddSocket(IOR);
            Roughness = new FloatSocket(parentNode, "Roughness", "subsurface_roughness", true);
            AddSocket(Roughness);
            Method = new EnumSocket(parentNode, "Method", "method", true);
            AddSocket(Method);
            Anisotropy = new FloatSocket(parentNode, "Anisotropy", "subsurface_anisotropy", true);
            AddSocket(Anisotropy);
            Scale = new FloatSocket(parentNode, "Scale", "scale", true);
            AddSocket(Scale);
        }
    }
    public class SubsurfaceScatteringNodeOutputs : Outputs
    {
        public ClosureSocket BSSRDF { get; private set; }

        public SubsurfaceScatteringNodeOutputs(ShaderNode parentNode)
        {
            BSSRDF = new ClosureSocket(parentNode, "BSSRDF", "BSSRDF", false);
            AddSocket(BSSRDF);
        }
    }

    [ShaderNode(name: "subsurface_scattering")]
    public class SubsurfaceScatteringNode : BsdfNode
    {
        public enum SubsurfaceScatteringNodeMethod : uint {
            Burley = ccl.ClosureType.CLOSURE_BSSRDF_BURLEY_ID,
            RandomWalk = ccl.ClosureType.CLOSURE_BSSRDF_RANDOM_WALK_ID,
            RandomWalkSkin = ccl.ClosureType.CLOSURE_BSSRDF_RANDOM_WALK_SKIN_ID,
        }
        public SubsurfaceScatteringNodeInputs ins => (SubsurfaceScatteringNodeInputs)inputs;
        public SubsurfaceScatteringNodeOutputs outs => (SubsurfaceScatteringNodeOutputs)outputs;
        public SubsurfaceScatteringNode(Shader shader) : this(shader, "a subsurface_scattering node") { }

        public SubsurfaceScatteringNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal SubsurfaceScatteringNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            inputs = new SubsurfaceScatteringNodeInputs(this);
            outputs = new SubsurfaceScatteringNodeOutputs(this);
        }
        public override ClosureSocket GetClosureSocket()
        {
            return outs.BSSRDF;
        }
        public static IntPtr GetNodeType() {
            return CSycles.subsurfacescatteringnode_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "subsurface_ior":
                    /* subsurfacescatteringnode . {'datatype': 'FLOAT', 'default_value': '1.4f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_ior', 'ui_name': 'IOR'} */
                    {
                    CSycles.subsurfacescatteringnode_set_subsurface_ior(this.Ptr, data);
                    }
                    break;
            case "subsurface_roughness":
                    /* subsurfacescatteringnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_roughness', 'ui_name': 'Roughness'} */
                    {
                    CSycles.subsurfacescatteringnode_set_subsurface_roughness(this.Ptr, data);
                    }
                    break;
            case "subsurface_anisotropy":
                    /* subsurfacescatteringnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_anisotropy', 'ui_name': 'Anisotropy'} */
                    {
                    CSycles.subsurfacescatteringnode_set_subsurface_anisotropy(this.Ptr, data);
                    }
                    break;
            case "scale":
                    /* subsurfacescatteringnode . {'datatype': 'FLOAT', 'default_value': '0.01f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                    {
                    CSycles.subsurfacescatteringnode_set_scale(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (setter)");
            }
        }

        internal override void SetVector(string name, float3 data)
        {
            switch(name) {
            case "radius":
                    /* subsurfacescatteringnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.1f,0.1f,0.1f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'radius', 'ui_name': 'Radius'} */
                    {
                    CSycles.subsurfacescatteringnode_set_radius(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (setter)");
            }
        }

        internal override void SetNormal(string name, float3 data)
        {
            switch(name) {
            case "normal":
                    /* subsurfacescatteringnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                    {
                    CSycles.bsdfnode_set_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (setter)");
            }
        }

        internal override void SetColor(string name, float3 data)
        {
            switch(name) {
            case "color":
                    /* subsurfacescatteringnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                    {
                    CSycles.bsdfnode_set_color(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "method":
                    /* subsurfacescatteringnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSSRDF_RANDOM_WALK_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'method', 'ui_name': 'Method'} */
                    {
                    CSycles.subsurfacescatteringnode_set_method(this.Ptr, (ccl.ClosureType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "subsurface_ior":
                /* subsurfacescatteringnode . {'datatype': 'FLOAT', 'default_value': '1.4f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_ior', 'ui_name': 'IOR'} */
                {
                    return CSycles.subsurfacescatteringnode_get_subsurface_ior(this.Ptr);
                }
            case "subsurface_roughness":
                /* subsurfacescatteringnode . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_roughness', 'ui_name': 'Roughness'} */
                {
                    return CSycles.subsurfacescatteringnode_get_subsurface_roughness(this.Ptr);
                }
            case "subsurface_anisotropy":
                /* subsurfacescatteringnode . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'subsurface_anisotropy', 'ui_name': 'Anisotropy'} */
                {
                    return CSycles.subsurfacescatteringnode_get_subsurface_anisotropy(this.Ptr);
                }
            case "scale":
                /* subsurfacescatteringnode . {'datatype': 'FLOAT', 'default_value': '0.01f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scale', 'ui_name': 'Scale'} */
                {
                    return CSycles.subsurfacescatteringnode_get_scale(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (getter)");
            }
        }

        internal override float3 GetVector(string name)
        {
            switch(name) {
            case "radius":
                /* subsurfacescatteringnode . {'datatype': 'VECTOR', 'default_value': 'make_float3(0.1f,0.1f,0.1f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'radius', 'ui_name': 'Radius'} */
                {
                    return CSycles.subsurfacescatteringnode_get_radius(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (getter)");
            }
        }

        internal override float3 GetNormal(string name)
        {
            switch(name) {
            case "normal":
                /* subsurfacescatteringnode . {'datatype': 'NORMAL', 'default_value': 'zero_float3()', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'normal', 'ui_name': 'Normal'} */
                {
                    return CSycles.bsdfnode_get_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (getter)");
            }
        }

        internal override float3 GetColor(string name)
        {
            switch(name) {
            case "color":
                /* subsurfacescatteringnode . {'datatype': 'COLOR', 'default_value': 'make_float3(0.8f,0.8f,0.8f)', 'default_value_type': 'float3', 'is_input': True, 'member_name': 'color', 'ui_name': 'Color'} */
                {
                    return CSycles.bsdfnode_get_color(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "method":
                /* subsurfacescatteringnode . {'datatype': 'ENUM', 'default_value': 'CLOSURE_BSSRDF_RANDOM_WALK_ID', 'default_value_type': 'ClosureType', 'is_input': True, 'member_name': 'method', 'ui_name': 'Method'} */
                {
                    return (uint)CSycles.subsurfacescatteringnode_get_method(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type SubsurfaceScatteringNode (getter)");
            }
        }

#endregion
    }

}