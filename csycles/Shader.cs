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
using System.IO;
using System.Text;
using System.Xml;
namespace ccl
{
    using cclext;

    public class ShaderNodeInputs : NodeInputs
    {
        public EnumNodeSocket VolumeInterpolationMethod { get; private set; }
        public BoolNodeSocket UseTransparentShadow { get; private set; }
        public BoolNodeSocket BumpMapCorrection { get; private set; }
        public FloatNodeSocket VolumeStepRate { get; private set; }
        public EnumNodeSocket DisplacementMethod { get; private set; }
        public BoolNodeSocket HeterogeneousVolume { get; private set; }
        public IntNodeSocket PassID { get; private set; }
        public EnumNodeSocket VolumeSamplingMethod { get; private set; }
        public EnumNodeSocket EmissionSamplingMethod { get; private set; }

        public ShaderNodeInputs(Node parentNode)
        {
            VolumeInterpolationMethod = new EnumNodeSocket(parentNode, "Volume Interpolation Method", "volume_interpolation_method", true);
            AddSocket(VolumeInterpolationMethod);
            UseTransparentShadow = new BoolNodeSocket(parentNode, "Use Transparent Shadow", "use_transparent_shadow", true);
            AddSocket(UseTransparentShadow);
            BumpMapCorrection = new BoolNodeSocket(parentNode, "Bump Map Correction", "use_bump_map_correction", true);
            AddSocket(BumpMapCorrection);
            VolumeStepRate = new FloatNodeSocket(parentNode, "Volume Step Rate", "volume_step_rate", true);
            AddSocket(VolumeStepRate);
            DisplacementMethod = new EnumNodeSocket(parentNode, "Displacement Method", "displacement_method", true);
            AddSocket(DisplacementMethod);
            HeterogeneousVolume = new BoolNodeSocket(parentNode, "Heterogeneous Volume", "heterogeneous_volume", true);
            AddSocket(HeterogeneousVolume);
            PassID = new IntNodeSocket(parentNode, "Pass ID", "pass_id", true);
            AddSocket(PassID);
            VolumeSamplingMethod = new EnumNodeSocket(parentNode, "Volume Sampling Method", "volume_sampling_method", true);
            AddSocket(VolumeSamplingMethod);
            EmissionSamplingMethod = new EnumNodeSocket(parentNode, "Emission Sampling Method", "emission_sampling_method", true);
            AddSocket(EmissionSamplingMethod);
        }
    }
    [Node("shader")]
    public class Shader : Node
    {
        public enum ShaderDisplacementMethod : uint {
            Bump = ccl.DisplacementMethod.DISPLACE_BUMP,
            True = ccl.DisplacementMethod.DISPLACE_TRUE,
            Both = ccl.DisplacementMethod.DISPLACE_BOTH,
        }
        public enum ShaderEmissionSamplingMethod : uint {
            None = ccl.EmissionSampling.EMISSION_SAMPLING_NONE,
            Auto = ccl.EmissionSampling.EMISSION_SAMPLING_AUTO,
            Front = ccl.EmissionSampling.EMISSION_SAMPLING_FRONT,
            Back = ccl.EmissionSampling.EMISSION_SAMPLING_BACK,
            FrontBack = ccl.EmissionSampling.EMISSION_SAMPLING_FRONT_BACK,
        }
        public enum ShaderVolumeInterpolationMethod : uint {
            Linear = ccl.VolumeInterpolation.VOLUME_INTERPOLATION_LINEAR,
            Cubic = ccl.VolumeInterpolation.VOLUME_INTERPOLATION_CUBIC,
        }
        public enum ShaderVolumeSamplingMethod : uint {
            Distance = ccl.VolumeSampling.VOLUME_SAMPLING_DISTANCE,
            Equiangular = ccl.VolumeSampling.VOLUME_SAMPLING_EQUIANGULAR,
            MultipleImportance = ccl.VolumeSampling.VOLUME_SAMPLING_MULTIPLE_IMPORTANCE,
        }
        public ShaderNodeInputs ShaderNodeInputs { get; set; }
        public ShaderNodeInputs ins => ShaderNodeInputs;

        public Shader() : this("a shader node") { }

        public Shader(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Shader(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            ShaderNodeInputs = new ShaderNodeInputs(this);

        }
        public static Shader FromIntPtr(IntPtr ptr) {
            Shader shader = new Shader(ptr);
            OutputNode outnode = (OutputNode)CSycles.CreateShaderNode(shader, CSycles.shader_get_outputnode(shader), "output");

            return shader;
        }

        public void TagUpdate(Scene scene)
        {
            CSycles.shader_tag_update(this, scene);
        }

        public OutputNode Output { get; internal set; }

        readonly internal List<ShaderNode> m_nodes = new List<ShaderNode>();
        /// <summary>
        /// Add a ShaderNode to the shader. This will create the node in Cycles, set
        /// any values for sockets and direct members.
        /// </summary>
        /// <param name="node">ShaderNode to add</param>
        internal void AddNode(ShaderNode node)
        {
            m_nodes.Add(node);
        }

        private void CommonConstructor()
        {
            int nodeCount = CSycles.shader_node_count(Ptr);
            for (int i = 0; i < nodeCount; i++)
            {
                IntPtr shn = CSycles.shader_node_get(Ptr, i);
                string name = CSycles.shadernode_get_name(shn);
                ShaderNode n = CSycles.CreateShaderNode(this, shn, name);
            }
        }

        /// <summary>
        /// Clear the shader graph for this node, so it can be repopulated.
        /// </summary>
        public void Recreate(Scene scene)
        {
            CSycles.shader_new_graph(this, scene);

            m_nodes.Clear();
            CommonConstructor();
        }

        /// <summary>
        /// Make the actual connection between nodes.
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="fromout"></param>
        /// <param name="toNode"></param>
        /// <param name="toin"></param>
        public bool Connect(ShaderNode fromNode, string fromout, ShaderNode toNode, string toin)
        {
            return CSycles.shader_connect_nodes(this, fromNode, fromout, toNode, toin);
        }

        /// <summary>
        /// Disconnect given node.
        /// </summary>
        /// <param name="fromNode"></param>
        /// <param name="fromOut"></param>
        public void Disconnect(ShaderNode fromNode, string fromOut)
        {
            CSycles.shader_disconnect_node(this, fromNode, fromOut);
        }

        public ShaderGraph Graph {
            get {
                return CSycles.shader_get_graph(this);
            }
        }

        /// <summary>
        /// Create node graph in the given shader from the passed XML.
        ///
        /// Note that you should call FinalizeConstructor on shader if you are not further
        /// changing the graph.
        /// </summary>
        /// <param name="shader">Shader to populate with nodes from the XML representation.</param>
        /// <param name="shaderXml">The XML representation for the shader.</param>
        /// <param name="finalize">Set to true if the shader should be finalized.</param>
        public static void ShaderFromXml(Shader shader, string shaderXml, bool finalize)
        {
            var xmlmem = Encoding.UTF8.GetBytes(shaderXml);
            using (var xmlstream = new MemoryStream(xmlmem))
            {
                var settings = new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Fragment,
                    IgnoreComments = true,
                    IgnoreProcessingInstructions = true,
                    IgnoreWhitespace = true
                };
                var reader = XmlReader.Create(xmlstream, settings);
                Utilities.Instance.ReadNodeGraph(shader, reader, finalize);
            }
        }

        public void DumpGraph(string filename)
        {
            CSycles.shader_dump_graph(Ptr, filename);
        }

        public void EstimateEmission() {
            CSycles.shader_estimate_emission(Ptr);
        }

        public uint Id {
            get { return CSycles.shader_get_id(Ptr); }
            set { CSycles.shader_set_id(Ptr, value); }
        }

        public void TagUsed(Scene scene) {
            CSycles.shader_tag_used(Ptr, scene.Ptr);
        }

        public float PrevVolumeStepRate {
            get { return CSycles.shader_get_prev_volume_step_rate(Ptr); }
            set { CSycles.shader_set_prev_volume_step_rate(Ptr, value); }
        }
        public float3 EmissionEstimate {
            get { return CSycles.shader_get_emission_estimate(Ptr); }
            set { CSycles.shader_set_emission_estimate(Ptr, value); }
        }

        public bool HasSurface {
            get { return CSycles.shader_get_has_surface(Ptr); }
            set { CSycles.shader_set_has_surface(Ptr, value); }
        }
        public bool EmissionIsConstant {
            get { return CSycles.shader_get_emission_is_constant(Ptr); }
            set { CSycles.shader_set_emission_is_constant(Ptr, value); }
        }

        public bool HasSurfaceSpatialVarying {
            get { return CSycles.shader_get_has_surface_spatial_varying(Ptr); }
            set { CSycles.shader_set_has_surface_spatial_varying(Ptr, value); }
        }

        public bool HasVolumeAttributeDependency {
            get { return CSycles.shader_get_has_volume_attribute_dependency(Ptr); }
            set { CSycles.shader_set_has_volume_attribute_dependency(Ptr, value); }
        }
        public bool HasVolumeSpatialVarying {
            get { return CSycles.shader_get_has_volume_spatial_varying(Ptr); }
            set { CSycles.shader_set_has_volume_spatial_varying(Ptr, value); }
        }

        public bool HasSurfaceRaytrace {
            get { return CSycles.shader_get_has_surface_raytrace(Ptr); }
            set { CSycles.shader_set_has_surface_raytrace(Ptr, value); }
        }
        public bool HasSurfaceLink() {
            return CSycles.shader_has_surface_link(Ptr);
        }

        public bool HasVolume {
            get { return CSycles.shader_get_has_volume(Ptr); }
            set { CSycles.shader_set_has_volume(Ptr, value); }
        }

        public EmissionSampling EmissionSampling {
            get { return CSycles.shader_get_emission_sampling(Ptr); }
            set { CSycles.shader_set_emission_sampling(Ptr, value); }
        }

        public bool HasVolumeConnected {
            get { return CSycles.shader_get_has_volume_connected(Ptr); }
            set { CSycles.shader_set_has_volume_connected(Ptr, value); }
        }
        public static IntPtr GetNodeType() {
            return CSycles.shader_get_node_type();
        }

        public bool HasDisplacement {
            get { return CSycles.shader_get_has_displacement(Ptr); }
            set { CSycles.shader_set_has_displacement(Ptr, value); }
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "volume_step_rate":
                    /* shader . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'volume_step_rate', 'ui_name': 'Volume Step Rate'} */
                    {
                    CSycles.shader_set_volume_step_rate(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Shader (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "use_transparent_shadow":
                    /* shader . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transparent_shadow', 'ui_name': 'Use Transparent Shadow'} */
                    {
                    CSycles.shader_set_use_transparent_shadow(this.Ptr, data);
                    }
                    break;
            case "use_bump_map_correction":
                    /* shader . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_bump_map_correction', 'ui_name': 'Bump Map Correction'} */
                    {
                    CSycles.shader_set_use_bump_map_correction(this.Ptr, data);
                    }
                    break;
            case "heterogeneous_volume":
                    /* shader . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'heterogeneous_volume', 'ui_name': 'Heterogeneous Volume'} */
                    {
                    CSycles.shader_set_heterogeneous_volume(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Shader (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "pass_id":
                    /* shader . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'pass_id', 'ui_name': 'Pass ID'} */
                    {
                    CSycles.shader_set_pass_id(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Shader (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "volume_interpolation_method":
                    /* shader . {'datatype': 'ENUM', 'default_value': 'VOLUME_INTERPOLATION_LINEAR', 'default_value_type': 'VolumeInterpolation', 'is_input': True, 'member_name': 'volume_interpolation_method', 'ui_name': 'Volume Interpolation Method'} */
                    {
                    CSycles.shader_set_volume_interpolation_method(this.Ptr, (ccl.VolumeInterpolation)data);
                    }
                    break;
            case "displacement_method":
                    /* shader . {'datatype': 'ENUM', 'default_value': 'DISPLACE_BUMP', 'default_value_type': 'DisplacementMethod', 'is_input': True, 'member_name': 'displacement_method', 'ui_name': 'Displacement Method'} */
                    {
                    CSycles.shader_set_displacement_method(this.Ptr, (ccl.DisplacementMethod)data);
                    }
                    break;
            case "volume_sampling_method":
                    /* shader . {'datatype': 'ENUM', 'default_value': 'VOLUME_SAMPLING_MULTIPLE_IMPORTANCE', 'default_value_type': 'VolumeSampling', 'is_input': True, 'member_name': 'volume_sampling_method', 'ui_name': 'Volume Sampling Method'} */
                    {
                    CSycles.shader_set_volume_sampling_method(this.Ptr, (ccl.VolumeSampling)data);
                    }
                    break;
            case "emission_sampling_method":
                    /* shader . {'datatype': 'ENUM', 'default_value': 'EMISSION_SAMPLING_AUTO', 'default_value_type': 'EmissionSampling', 'is_input': True, 'member_name': 'emission_sampling_method', 'ui_name': 'Emission Sampling Method'} */
                    {
                    CSycles.shader_set_emission_sampling_method(this.Ptr, (ccl.EmissionSampling)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Shader (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "volume_step_rate":
                /* shader . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'volume_step_rate', 'ui_name': 'Volume Step Rate'} */
                {
                    return CSycles.shader_get_volume_step_rate(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Shader (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "use_transparent_shadow":
                /* shader . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transparent_shadow', 'ui_name': 'Use Transparent Shadow'} */
                {
                    return CSycles.shader_get_use_transparent_shadow(this.Ptr);
                }
            case "use_bump_map_correction":
                /* shader . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_bump_map_correction', 'ui_name': 'Bump Map Correction'} */
                {
                    return CSycles.shader_get_use_bump_map_correction(this.Ptr);
                }
            case "heterogeneous_volume":
                /* shader . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'heterogeneous_volume', 'ui_name': 'Heterogeneous Volume'} */
                {
                    return CSycles.shader_get_heterogeneous_volume(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Shader (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "pass_id":
                /* shader . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'pass_id', 'ui_name': 'Pass ID'} */
                {
                    return CSycles.shader_get_pass_id(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Shader (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "volume_interpolation_method":
                /* shader . {'datatype': 'ENUM', 'default_value': 'VOLUME_INTERPOLATION_LINEAR', 'default_value_type': 'VolumeInterpolation', 'is_input': True, 'member_name': 'volume_interpolation_method', 'ui_name': 'Volume Interpolation Method'} */
                {
                    return (uint)CSycles.shader_get_volume_interpolation_method(this.Ptr);
                }
            case "displacement_method":
                /* shader . {'datatype': 'ENUM', 'default_value': 'DISPLACE_BUMP', 'default_value_type': 'DisplacementMethod', 'is_input': True, 'member_name': 'displacement_method', 'ui_name': 'Displacement Method'} */
                {
                    return (uint)CSycles.shader_get_displacement_method(this.Ptr);
                }
            case "volume_sampling_method":
                /* shader . {'datatype': 'ENUM', 'default_value': 'VOLUME_SAMPLING_MULTIPLE_IMPORTANCE', 'default_value_type': 'VolumeSampling', 'is_input': True, 'member_name': 'volume_sampling_method', 'ui_name': 'Volume Sampling Method'} */
                {
                    return (uint)CSycles.shader_get_volume_sampling_method(this.Ptr);
                }
            case "emission_sampling_method":
                /* shader . {'datatype': 'ENUM', 'default_value': 'EMISSION_SAMPLING_AUTO', 'default_value_type': 'EmissionSampling', 'is_input': True, 'member_name': 'emission_sampling_method', 'ui_name': 'Emission Sampling Method'} */
                {
                    return (uint)CSycles.shader_get_emission_sampling_method(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Shader (getter)");
            }
        }

#endregion
    }

}