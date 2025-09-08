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
    public class LightPathNodeOutputs : Outputs
    {
        public FloatSocket DiffuseDepth { get; private set; }
        public FloatSocket IsShadowRay { get; private set; }
        public FloatSocket IsTransmissionRay { get; private set; }
        public FloatSocket GlossyDepth { get; private set; }
        public FloatSocket IsDiffuseRay { get; private set; }
        public FloatSocket IsVolumeScatterRay { get; private set; }
        public FloatSocket TransparentDepth { get; private set; }
        public FloatSocket IsGlossyRay { get; private set; }
        public FloatSocket RayLength { get; private set; }
        public FloatSocket TransmissionDepth { get; private set; }
        public FloatSocket IsSingularRay { get; private set; }
        public FloatSocket RayDepth { get; private set; }
        public FloatSocket IsCameraRay { get; private set; }
        public FloatSocket IsReflectionRay { get; private set; }

        public LightPathNodeOutputs(ShaderNode parentNode)
        {
            DiffuseDepth = new FloatSocket(parentNode, "Diffuse Depth", "diffuse_depth", false);
            AddSocket(DiffuseDepth);
            IsShadowRay = new FloatSocket(parentNode, "Is Shadow Ray", "is_shadow_ray", false);
            AddSocket(IsShadowRay);
            IsTransmissionRay = new FloatSocket(parentNode, "Is Transmission Ray", "is_transmission_ray", false);
            AddSocket(IsTransmissionRay);
            GlossyDepth = new FloatSocket(parentNode, "Glossy Depth", "glossy_depth", false);
            AddSocket(GlossyDepth);
            IsDiffuseRay = new FloatSocket(parentNode, "Is Diffuse Ray", "is_diffuse_ray", false);
            AddSocket(IsDiffuseRay);
            IsVolumeScatterRay = new FloatSocket(parentNode, "Is Volume Scatter Ray", "is_volume_scatter_ray", false);
            AddSocket(IsVolumeScatterRay);
            TransparentDepth = new FloatSocket(parentNode, "Transparent Depth", "transparent_depth", false);
            AddSocket(TransparentDepth);
            IsGlossyRay = new FloatSocket(parentNode, "Is Glossy Ray", "is_glossy_ray", false);
            AddSocket(IsGlossyRay);
            RayLength = new FloatSocket(parentNode, "Ray Length", "ray_length", false);
            AddSocket(RayLength);
            TransmissionDepth = new FloatSocket(parentNode, "Transmission Depth", "transmission_depth", false);
            AddSocket(TransmissionDepth);
            IsSingularRay = new FloatSocket(parentNode, "Is Singular Ray", "is_singular_ray", false);
            AddSocket(IsSingularRay);
            RayDepth = new FloatSocket(parentNode, "Ray Depth", "ray_depth", false);
            AddSocket(RayDepth);
            IsCameraRay = new FloatSocket(parentNode, "Is Camera Ray", "is_camera_ray", false);
            AddSocket(IsCameraRay);
            IsReflectionRay = new FloatSocket(parentNode, "Is Reflection Ray", "is_reflection_ray", false);
            AddSocket(IsReflectionRay);
        }
    }

    [ShaderNode(name: "light_path")]
    public class LightPathNode : ShaderNode
    {
        public LightPathNodeOutputs outs => (LightPathNodeOutputs)outputs;
        public LightPathNode(Shader shader) : this(shader, "a light_path node") { }

        public LightPathNode(Shader shader, string name) :
            base(shader, name)
        {
            FinalizeConstructor();
        }

        internal LightPathNode(Shader shader, IntPtr intPtr) : base(shader, intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            outputs = new LightPathNodeOutputs(this);
        }
        public static IntPtr GetNodeType() {
            return CSycles.lightpathnode_get_node_type();
        }
    }

}