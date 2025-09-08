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
namespace ccl
{
    using cclext;

    public class IntegratorNodeInputs : NodeInputs
    {
        public EnumNodeSocket DenoiserPrefilter { get; private set; }
        public IntNodeSocket TrainingSamples { get; private set; }
        public FloatNodeSocket SampleClampIndirect { get; private set; }
        public IntNodeSocket MaxDiffuseBounce { get; private set; }
        public BoolNodeSocket DenoiseonGPU { get; private set; }
        public BoolNodeSocket GuideDirectLight { get; private set; }
        public BoolNodeSocket MotionBlur { get; private set; }
        public IntNodeSocket MaxGlossyBounce { get; private set; }
        public EnumNodeSocket DenoiserQuality { get; private set; }
        public BoolNodeSocket UseMISWeights { get; private set; }
        public IntNodeSocket AASamples { get; private set; }
        public IntNodeSocket MaxTransmissionBounce { get; private set; }
        public EnumNodeSocket GuidingDistributionType { get; private set; }
        public BoolNodeSocket UseSampleSubset { get; private set; }
        public IntNodeSocket MaxVolumeBounce { get; private set; }
        public IntNodeSocket MaxBounce { get; private set; }
        public EnumNodeSocket GuidingDirectionalSamplingType { get; private set; }
        public IntNodeSocket SampleSubsetOffset { get; private set; }
        public IntNodeSocket TransparentMinBounce { get; private set; }
        public IntNodeSocket MinBounce { get; private set; }
        public FloatNodeSocket GuidingRoughnessThreshold { get; private set; }
        public IntNodeSocket TransparentMaxBounce { get; private set; }
        public IntNodeSocket SampleSubsetLength { get; private set; }
        public BoolNodeSocket ReflectiveCaustics { get; private set; }
        public BoolNodeSocket UseAdaptiveSampling { get; private set; }
        public IntNodeSocket AOBounces { get; private set; }
        public BoolNodeSocket RefractiveCaustics { get; private set; }
        public FloatNodeSocket AdaptiveThreshold { get; private set; }
        public FloatNodeSocket AOFactor { get; private set; }
        public FloatNodeSocket FilterGlossy { get; private set; }
        public IntNodeSocket AdaptiveMinSamples { get; private set; }
        public FloatNodeSocket AODistance { get; private set; }
        public BoolNodeSocket UseDirectLight { get; private set; }
        public BoolNodeSocket Uselighttreetooptimizemanylightsampling { get; private set; }
        public FloatNodeSocket AOAdditiveFactor { get; private set; }
        public BoolNodeSocket UseIndirectLight { get; private set; }
        public FloatNodeSocket LightSamplingThreshold { get; private set; }
        public IntNodeSocket VolumeMaxSteps { get; private set; }
        public BoolNodeSocket UseDiffuse { get; private set; }
        public EnumNodeSocket SamplingPattern { get; private set; }
        public FloatNodeSocket VolumeStepRate { get; private set; }
        public BoolNodeSocket UseGlossy { get; private set; }
        public FloatNodeSocket ScramblingDistance { get; private set; }
        public BoolNodeSocket Guiding { get; private set; }
        public BoolNodeSocket UseTransmission { get; private set; }
        public BoolNodeSocket UseDenoiser { get; private set; }
        public BoolNodeSocket DeterministicGuiding { get; private set; }
        public BoolNodeSocket UseEmission { get; private set; }
        public EnumNodeSocket DenoiserType { get; private set; }
        public BoolNodeSocket SurfaceGuiding { get; private set; }
        public IntNodeSocket Seed { get; private set; }
        public IntNodeSocket StartSampletoDenoise { get; private set; }
        public FloatNodeSocket SurfaceGuidingProbability { get; private set; }
        public FloatNodeSocket SampleClampDirect { get; private set; }
        public BoolNodeSocket UseAlbedoPassforDenoiser { get; private set; }
        public BoolNodeSocket VolumeGuiding { get; private set; }
        public BoolNodeSocket UseNormalPassforDenoiser { get; private set; }
        public FloatNodeSocket VolumeGuidingProbability { get; private set; }

        public IntegratorNodeInputs(Node parentNode)
        {
            DenoiserPrefilter = new EnumNodeSocket(parentNode, "Denoiser Prefilter", "denoiser_prefilter", true);
            AddSocket(DenoiserPrefilter);
            TrainingSamples = new IntNodeSocket(parentNode, "Training Samples", "guiding_training_samples", true);
            AddSocket(TrainingSamples);
            SampleClampIndirect = new FloatNodeSocket(parentNode, "Sample Clamp Indirect", "sample_clamp_indirect", true);
            AddSocket(SampleClampIndirect);
            MaxDiffuseBounce = new IntNodeSocket(parentNode, "Max Diffuse Bounce", "max_diffuse_bounce", true);
            AddSocket(MaxDiffuseBounce);
            DenoiseonGPU = new BoolNodeSocket(parentNode, "Denoise on GPU", "denoise_use_gpu", true);
            AddSocket(DenoiseonGPU);
            GuideDirectLight = new BoolNodeSocket(parentNode, "Guide Direct Light", "use_guiding_direct_light", true);
            AddSocket(GuideDirectLight);
            MotionBlur = new BoolNodeSocket(parentNode, "Motion Blur", "motion_blur", true);
            AddSocket(MotionBlur);
            MaxGlossyBounce = new IntNodeSocket(parentNode, "Max Glossy Bounce", "max_glossy_bounce", true);
            AddSocket(MaxGlossyBounce);
            DenoiserQuality = new EnumNodeSocket(parentNode, "Denoiser Quality", "denoiser_quality", true);
            AddSocket(DenoiserQuality);
            UseMISWeights = new BoolNodeSocket(parentNode, "Use MIS Weights", "use_guiding_mis_weights", true);
            AddSocket(UseMISWeights);
            AASamples = new IntNodeSocket(parentNode, "AA Samples", "aa_samples", true);
            AddSocket(AASamples);
            MaxTransmissionBounce = new IntNodeSocket(parentNode, "Max Transmission Bounce", "max_transmission_bounce", true);
            AddSocket(MaxTransmissionBounce);
            GuidingDistributionType = new EnumNodeSocket(parentNode, "Guiding Distribution Type", "guiding_distribution_type", true);
            AddSocket(GuidingDistributionType);
            UseSampleSubset = new BoolNodeSocket(parentNode, "Use Sample Subset", "use_sample_subset", true);
            AddSocket(UseSampleSubset);
            MaxVolumeBounce = new IntNodeSocket(parentNode, "Max Volume Bounce", "max_volume_bounce", true);
            AddSocket(MaxVolumeBounce);
            MaxBounce = new IntNodeSocket(parentNode, "Max Bounce", "max_bounce", true);
            AddSocket(MaxBounce);
            GuidingDirectionalSamplingType = new EnumNodeSocket(parentNode, "Guiding Directional Sampling Type", "guiding_directional_sampling_type", true);
            AddSocket(GuidingDirectionalSamplingType);
            SampleSubsetOffset = new IntNodeSocket(parentNode, "Sample Subset Offset", "sample_subset_offset", true);
            AddSocket(SampleSubsetOffset);
            TransparentMinBounce = new IntNodeSocket(parentNode, "Transparent Min Bounce", "transparent_min_bounce", true);
            AddSocket(TransparentMinBounce);
            MinBounce = new IntNodeSocket(parentNode, "Min Bounce", "min_bounce", true);
            AddSocket(MinBounce);
            GuidingRoughnessThreshold = new FloatNodeSocket(parentNode, "Guiding Roughness Threshold", "guiding_roughness_threshold", true);
            AddSocket(GuidingRoughnessThreshold);
            TransparentMaxBounce = new IntNodeSocket(parentNode, "Transparent Max Bounce", "transparent_max_bounce", true);
            AddSocket(TransparentMaxBounce);
            SampleSubsetLength = new IntNodeSocket(parentNode, "Sample Subset Length", "sample_subset_length", true);
            AddSocket(SampleSubsetLength);
            ReflectiveCaustics = new BoolNodeSocket(parentNode, "Reflective Caustics", "caustics_reflective", true);
            AddSocket(ReflectiveCaustics);
            UseAdaptiveSampling = new BoolNodeSocket(parentNode, "Use Adaptive Sampling", "use_adaptive_sampling", true);
            AddSocket(UseAdaptiveSampling);
            AOBounces = new IntNodeSocket(parentNode, "AO Bounces", "ao_bounces", true);
            AddSocket(AOBounces);
            RefractiveCaustics = new BoolNodeSocket(parentNode, "Refractive Caustics", "caustics_refractive", true);
            AddSocket(RefractiveCaustics);
            AdaptiveThreshold = new FloatNodeSocket(parentNode, "Adaptive Threshold", "adaptive_threshold", true);
            AddSocket(AdaptiveThreshold);
            AOFactor = new FloatNodeSocket(parentNode, "AO Factor", "ao_factor", true);
            AddSocket(AOFactor);
            FilterGlossy = new FloatNodeSocket(parentNode, "Filter Glossy", "filter_glossy", true);
            AddSocket(FilterGlossy);
            AdaptiveMinSamples = new IntNodeSocket(parentNode, "Adaptive Min Samples", "adaptive_min_samples", true);
            AddSocket(AdaptiveMinSamples);
            AODistance = new FloatNodeSocket(parentNode, "AO Distance", "ao_distance", true);
            AddSocket(AODistance);
            UseDirectLight = new BoolNodeSocket(parentNode, "Use Direct Light", "use_direct_light", true);
            AddSocket(UseDirectLight);
            Uselighttreetooptimizemanylightsampling = new BoolNodeSocket(parentNode, "Use light tree to optimize many light sampling", "use_light_tree", true);
            AddSocket(Uselighttreetooptimizemanylightsampling);
            AOAdditiveFactor = new FloatNodeSocket(parentNode, "AO Additive Factor", "ao_additive_factor", true);
            AddSocket(AOAdditiveFactor);
            UseIndirectLight = new BoolNodeSocket(parentNode, "Use Indirect Light", "use_indirect_light", true);
            AddSocket(UseIndirectLight);
            LightSamplingThreshold = new FloatNodeSocket(parentNode, "Light Sampling Threshold", "light_sampling_threshold", true);
            AddSocket(LightSamplingThreshold);
            VolumeMaxSteps = new IntNodeSocket(parentNode, "Volume Max Steps", "volume_max_steps", true);
            AddSocket(VolumeMaxSteps);
            UseDiffuse = new BoolNodeSocket(parentNode, "Use Diffuse", "use_diffuse", true);
            AddSocket(UseDiffuse);
            SamplingPattern = new EnumNodeSocket(parentNode, "Sampling Pattern", "sampling_pattern", true);
            AddSocket(SamplingPattern);
            VolumeStepRate = new FloatNodeSocket(parentNode, "Volume Step Rate", "volume_step_rate", true);
            AddSocket(VolumeStepRate);
            UseGlossy = new BoolNodeSocket(parentNode, "Use Glossy", "use_glossy", true);
            AddSocket(UseGlossy);
            ScramblingDistance = new FloatNodeSocket(parentNode, "Scrambling Distance", "scrambling_distance", true);
            AddSocket(ScramblingDistance);
            Guiding = new BoolNodeSocket(parentNode, "Guiding", "use_guiding", true);
            AddSocket(Guiding);
            UseTransmission = new BoolNodeSocket(parentNode, "Use Transmission", "use_transmission", true);
            AddSocket(UseTransmission);
            UseDenoiser = new BoolNodeSocket(parentNode, "Use Denoiser", "use_denoise", true);
            AddSocket(UseDenoiser);
            DeterministicGuiding = new BoolNodeSocket(parentNode, "Deterministic Guiding", "deterministic_guiding", true);
            AddSocket(DeterministicGuiding);
            UseEmission = new BoolNodeSocket(parentNode, "Use Emission", "use_emission", true);
            AddSocket(UseEmission);
            DenoiserType = new EnumNodeSocket(parentNode, "Denoiser Type", "denoiser_type", true);
            AddSocket(DenoiserType);
            SurfaceGuiding = new BoolNodeSocket(parentNode, "Surface Guiding", "use_surface_guiding", true);
            AddSocket(SurfaceGuiding);
            Seed = new IntNodeSocket(parentNode, "Seed", "seed", true);
            AddSocket(Seed);
            StartSampletoDenoise = new IntNodeSocket(parentNode, "Start Sample to Denoise", "denoise_start_sample", true);
            AddSocket(StartSampletoDenoise);
            SurfaceGuidingProbability = new FloatNodeSocket(parentNode, "Surface Guiding Probability", "surface_guiding_probability", true);
            AddSocket(SurfaceGuidingProbability);
            SampleClampDirect = new FloatNodeSocket(parentNode, "Sample Clamp Direct", "sample_clamp_direct", true);
            AddSocket(SampleClampDirect);
            UseAlbedoPassforDenoiser = new BoolNodeSocket(parentNode, "Use Albedo Pass for Denoiser", "use_denoise_pass_albedo", true);
            AddSocket(UseAlbedoPassforDenoiser);
            VolumeGuiding = new BoolNodeSocket(parentNode, "Volume Guiding", "use_volume_guiding", true);
            AddSocket(VolumeGuiding);
            UseNormalPassforDenoiser = new BoolNodeSocket(parentNode, "Use Normal Pass for Denoiser", "use_denoise_pass_normal", true);
            AddSocket(UseNormalPassforDenoiser);
            VolumeGuidingProbability = new FloatNodeSocket(parentNode, "Volume Guiding Probability", "volume_guiding_probability", true);
            AddSocket(VolumeGuidingProbability);
        }
    }
    [Node("integrator")]
    public class Integrator : Node
    {
        public enum IntegratorDenoiserPrefilter : uint {
            None = ccl.DenoiserPrefilter.DENOISER_PREFILTER_NONE,
            Fast = ccl.DenoiserPrefilter.DENOISER_PREFILTER_FAST,
            Accurate = ccl.DenoiserPrefilter.DENOISER_PREFILTER_ACCURATE,
        }
        public enum IntegratorDenoiserQuality : uint {
            High = ccl.DenoiserQuality.DENOISER_QUALITY_HIGH,
            Balanced = ccl.DenoiserQuality.DENOISER_QUALITY_BALANCED,
            Fast = ccl.DenoiserQuality.DENOISER_QUALITY_FAST,
        }
        public enum IntegratorDenoiserType : uint {
            None = ccl.DenoiserType.DENOISER_NONE,
            Optix = ccl.DenoiserType.DENOISER_OPTIX,
            Openimagedenoise = ccl.DenoiserType.DENOISER_OPENIMAGEDENOISE,
        }
        public enum IntegratorGuidingDirectionalSamplingType : uint {
            Mis = ccl.GuidingDirectionalSamplingType.GUIDING_DIRECTIONAL_SAMPLING_TYPE_PRODUCT_MIS,
            Ris = ccl.GuidingDirectionalSamplingType.GUIDING_DIRECTIONAL_SAMPLING_TYPE_RIS,
            Roughness = ccl.GuidingDirectionalSamplingType.GUIDING_DIRECTIONAL_SAMPLING_TYPE_ROUGHNESS,
        }
        public enum IntegratorGuidingDistribution : uint {
            ParallaxAwareVmm = ccl.GuidingDistributionType.GUIDING_TYPE_PARALLAX_AWARE_VMM,
            DirectionalQuadTree = ccl.GuidingDistributionType.GUIDING_TYPE_DIRECTIONAL_QUAD_TREE,
            Vmm = ccl.GuidingDistributionType.GUIDING_TYPE_VMM,
        }
        public enum IntegratorSamplingPattern : uint {
            SobolBurley = ccl.SamplingPattern.SAMPLING_PATTERN_SOBOL_BURLEY,
            TabulatedSobol = ccl.SamplingPattern.SAMPLING_PATTERN_TABULATED_SOBOL,
            BlueNoisePure = ccl.SamplingPattern.SAMPLING_PATTERN_BLUE_NOISE_PURE,
            BlueNoiseFirst = ccl.SamplingPattern.SAMPLING_PATTERN_BLUE_NOISE_FIRST,
            BlueNoiseRound = ccl.SamplingPattern.SAMPLING_PATTERN_BLUE_NOISE_ROUND,
        }
        public IntegratorNodeInputs IntegratorNodeInputs { get; set; }
        public IntegratorNodeInputs ins => IntegratorNodeInputs;

        public Integrator() : this("a integrator node") { }

        public Integrator(string name) :
            base(name)
        {
            FinalizeConstructor();
        }

        internal Integrator(IntPtr intPtr) : base(intPtr)
        {
            FinalizeConstructor();
        }

        private void FinalizeConstructor()
        {
            IntegratorNodeInputs = new IntegratorNodeInputs(this);

        }
        public uint GetKernelFeatures() {
            return CSycles.integrator_get_kernel_features(Ptr);
        }
        public static IntPtr GetNodeType() {
            return CSycles.integrator_get_node_type();
        }
#region Setters

        internal override void SetFloat(string name, float data)
        {
            switch(name) {
            case "sample_clamp_indirect":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '10.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_clamp_indirect', 'ui_name': 'Sample Clamp Indirect'} */
                    {
                    CSycles.integrator_set_sample_clamp_indirect(this.Ptr, data);
                    }
                    break;
            case "guiding_roughness_threshold":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '0.05f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'guiding_roughness_threshold', 'ui_name': 'Guiding Roughness Threshold'} */
                    {
                    CSycles.integrator_set_guiding_roughness_threshold(this.Ptr, data);
                    }
                    break;
            case "adaptive_threshold":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '0.01f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'adaptive_threshold', 'ui_name': 'Adaptive Threshold'} */
                    {
                    CSycles.integrator_set_adaptive_threshold(this.Ptr, data);
                    }
                    break;
            case "ao_factor":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ao_factor', 'ui_name': 'AO Factor'} */
                    {
                    CSycles.integrator_set_ao_factor(this.Ptr, data);
                    }
                    break;
            case "filter_glossy":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'filter_glossy', 'ui_name': 'Filter Glossy'} */
                    {
                    CSycles.integrator_set_filter_glossy(this.Ptr, data);
                    }
                    break;
            case "ao_distance":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '3.40282347e+38F', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ao_distance', 'ui_name': 'AO Distance'} */
                    {
                    CSycles.integrator_set_ao_distance(this.Ptr, data);
                    }
                    break;
            case "ao_additive_factor":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ao_additive_factor', 'ui_name': 'AO Additive Factor'} */
                    {
                    CSycles.integrator_set_ao_additive_factor(this.Ptr, data);
                    }
                    break;
            case "light_sampling_threshold":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'light_sampling_threshold', 'ui_name': 'Light Sampling Threshold'} */
                    {
                    CSycles.integrator_set_light_sampling_threshold(this.Ptr, data);
                    }
                    break;
            case "volume_step_rate":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'volume_step_rate', 'ui_name': 'Volume Step Rate'} */
                    {
                    CSycles.integrator_set_volume_step_rate(this.Ptr, data);
                    }
                    break;
            case "scrambling_distance":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scrambling_distance', 'ui_name': 'Scrambling Distance'} */
                    {
                    CSycles.integrator_set_scrambling_distance(this.Ptr, data);
                    }
                    break;
            case "surface_guiding_probability":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'surface_guiding_probability', 'ui_name': 'Surface Guiding Probability'} */
                    {
                    CSycles.integrator_set_surface_guiding_probability(this.Ptr, data);
                    }
                    break;
            case "sample_clamp_direct":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_clamp_direct', 'ui_name': 'Sample Clamp Direct'} */
                    {
                    CSycles.integrator_set_sample_clamp_direct(this.Ptr, data);
                    }
                    break;
            case "volume_guiding_probability":
                    /* integrator . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'volume_guiding_probability', 'ui_name': 'Volume Guiding Probability'} */
                    {
                    CSycles.integrator_set_volume_guiding_probability(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Integrator (setter)");
            }
        }

        internal override void SetBool(string name, bool data)
        {
            switch(name) {
            case "denoise_use_gpu":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'denoise_use_gpu', 'ui_name': 'Denoise on GPU'} */
                    {
                    CSycles.integrator_set_denoise_use_gpu(this.Ptr, data);
                    }
                    break;
            case "use_guiding_direct_light":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_guiding_direct_light', 'ui_name': 'Guide Direct Light'} */
                    {
                    CSycles.integrator_set_use_guiding_direct_light(this.Ptr, data);
                    }
                    break;
            case "motion_blur":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'motion_blur', 'ui_name': 'Motion Blur'} */
                    {
                    CSycles.integrator_set_motion_blur(this.Ptr, data);
                    }
                    break;
            case "use_guiding_mis_weights":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_guiding_mis_weights', 'ui_name': 'Use MIS Weights'} */
                    {
                    CSycles.integrator_set_use_guiding_mis_weights(this.Ptr, data);
                    }
                    break;
            case "use_sample_subset":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_sample_subset', 'ui_name': 'Use Sample Subset'} */
                    {
                    CSycles.integrator_set_use_sample_subset(this.Ptr, data);
                    }
                    break;
            case "caustics_reflective":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'caustics_reflective', 'ui_name': 'Reflective Caustics'} */
                    {
                    CSycles.integrator_set_caustics_reflective(this.Ptr, data);
                    }
                    break;
            case "use_adaptive_sampling":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_adaptive_sampling', 'ui_name': 'Use Adaptive Sampling'} */
                    {
                    CSycles.integrator_set_use_adaptive_sampling(this.Ptr, data);
                    }
                    break;
            case "caustics_refractive":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'caustics_refractive', 'ui_name': 'Refractive Caustics'} */
                    {
                    CSycles.integrator_set_caustics_refractive(this.Ptr, data);
                    }
                    break;
            case "use_direct_light":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_direct_light', 'ui_name': 'Use Direct Light'} */
                    {
                    CSycles.integrator_set_use_direct_light(this.Ptr, data);
                    }
                    break;
            case "use_light_tree":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_light_tree', 'ui_name': 'Use light tree to optimize many light sampling'} */
                    {
                    CSycles.integrator_set_use_light_tree(this.Ptr, data);
                    }
                    break;
            case "use_indirect_light":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_indirect_light', 'ui_name': 'Use Indirect Light'} */
                    {
                    CSycles.integrator_set_use_indirect_light(this.Ptr, data);
                    }
                    break;
            case "use_diffuse":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_diffuse', 'ui_name': 'Use Diffuse'} */
                    {
                    CSycles.integrator_set_use_diffuse(this.Ptr, data);
                    }
                    break;
            case "use_glossy":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_glossy', 'ui_name': 'Use Glossy'} */
                    {
                    CSycles.integrator_set_use_glossy(this.Ptr, data);
                    }
                    break;
            case "use_guiding":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_guiding', 'ui_name': 'Guiding'} */
                    {
                    CSycles.integrator_set_use_guiding(this.Ptr, data);
                    }
                    break;
            case "use_transmission":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transmission', 'ui_name': 'Use Transmission'} */
                    {
                    CSycles.integrator_set_use_transmission(this.Ptr, data);
                    }
                    break;
            case "use_denoise":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_denoise', 'ui_name': 'Use Denoiser'} */
                    {
                    CSycles.integrator_set_use_denoise(this.Ptr, data);
                    }
                    break;
            case "deterministic_guiding":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'deterministic_guiding', 'ui_name': 'Deterministic Guiding'} */
                    {
                    CSycles.integrator_set_deterministic_guiding(this.Ptr, data);
                    }
                    break;
            case "use_emission":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_emission', 'ui_name': 'Use Emission'} */
                    {
                    CSycles.integrator_set_use_emission(this.Ptr, data);
                    }
                    break;
            case "use_surface_guiding":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_surface_guiding', 'ui_name': 'Surface Guiding'} */
                    {
                    CSycles.integrator_set_use_surface_guiding(this.Ptr, data);
                    }
                    break;
            case "use_denoise_pass_albedo":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_denoise_pass_albedo', 'ui_name': 'Use Albedo Pass for Denoiser'} */
                    {
                    CSycles.integrator_set_use_denoise_pass_albedo(this.Ptr, data);
                    }
                    break;
            case "use_volume_guiding":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_volume_guiding', 'ui_name': 'Volume Guiding'} */
                    {
                    CSycles.integrator_set_use_volume_guiding(this.Ptr, data);
                    }
                    break;
            case "use_denoise_pass_normal":
                    /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_denoise_pass_normal', 'ui_name': 'Use Normal Pass for Denoiser'} */
                    {
                    CSycles.integrator_set_use_denoise_pass_normal(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Integrator (setter)");
            }
        }

        internal override void SetInt(string name, int data)
        {
            switch(name) {
            case "guiding_training_samples":
                    /* integrator . {'datatype': 'INT', 'default_value': '128', 'default_value_type': 'int', 'is_input': True, 'member_name': 'guiding_training_samples', 'ui_name': 'Training Samples'} */
                    {
                    CSycles.integrator_set_guiding_training_samples(this.Ptr, data);
                    }
                    break;
            case "max_diffuse_bounce":
                    /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_diffuse_bounce', 'ui_name': 'Max Diffuse Bounce'} */
                    {
                    CSycles.integrator_set_max_diffuse_bounce(this.Ptr, data);
                    }
                    break;
            case "max_glossy_bounce":
                    /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_glossy_bounce', 'ui_name': 'Max Glossy Bounce'} */
                    {
                    CSycles.integrator_set_max_glossy_bounce(this.Ptr, data);
                    }
                    break;
            case "aa_samples":
                    /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'aa_samples', 'ui_name': 'AA Samples'} */
                    {
                    CSycles.integrator_set_aa_samples(this.Ptr, data);
                    }
                    break;
            case "max_transmission_bounce":
                    /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_transmission_bounce', 'ui_name': 'Max Transmission Bounce'} */
                    {
                    CSycles.integrator_set_max_transmission_bounce(this.Ptr, data);
                    }
                    break;
            case "max_volume_bounce":
                    /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_volume_bounce', 'ui_name': 'Max Volume Bounce'} */
                    {
                    CSycles.integrator_set_max_volume_bounce(this.Ptr, data);
                    }
                    break;
            case "max_bounce":
                    /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_bounce', 'ui_name': 'Max Bounce'} */
                    {
                    CSycles.integrator_set_max_bounce(this.Ptr, data);
                    }
                    break;
            case "sample_subset_offset":
                    /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'sample_subset_offset', 'ui_name': 'Sample Subset Offset'} */
                    {
                    CSycles.integrator_set_sample_subset_offset(this.Ptr, data);
                    }
                    break;
            case "transparent_min_bounce":
                    /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'transparent_min_bounce', 'ui_name': 'Transparent Min Bounce'} */
                    {
                    CSycles.integrator_set_transparent_min_bounce(this.Ptr, data);
                    }
                    break;
            case "min_bounce":
                    /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'min_bounce', 'ui_name': 'Min Bounce'} */
                    {
                    CSycles.integrator_set_min_bounce(this.Ptr, data);
                    }
                    break;
            case "transparent_max_bounce":
                    /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'transparent_max_bounce', 'ui_name': 'Transparent Max Bounce'} */
                    {
                    CSycles.integrator_set_transparent_max_bounce(this.Ptr, data);
                    }
                    break;
            case "sample_subset_length":
                    /* integrator . {'datatype': 'INT', 'default_value': '(1<<24)', 'default_value_type': 'const int', 'is_input': True, 'member_name': 'sample_subset_length', 'ui_name': 'Sample Subset Length'} */
                    {
                    CSycles.integrator_set_sample_subset_length(this.Ptr, data);
                    }
                    break;
            case "ao_bounces":
                    /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'ao_bounces', 'ui_name': 'AO Bounces'} */
                    {
                    CSycles.integrator_set_ao_bounces(this.Ptr, data);
                    }
                    break;
            case "adaptive_min_samples":
                    /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'adaptive_min_samples', 'ui_name': 'Adaptive Min Samples'} */
                    {
                    CSycles.integrator_set_adaptive_min_samples(this.Ptr, data);
                    }
                    break;
            case "volume_max_steps":
                    /* integrator . {'datatype': 'INT', 'default_value': '1024', 'default_value_type': 'int', 'is_input': True, 'member_name': 'volume_max_steps', 'ui_name': 'Volume Max Steps'} */
                    {
                    CSycles.integrator_set_volume_max_steps(this.Ptr, data);
                    }
                    break;
            case "seed":
                    /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'seed', 'ui_name': 'Seed'} */
                    {
                    CSycles.integrator_set_seed(this.Ptr, data);
                    }
                    break;
            case "denoise_start_sample":
                    /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'denoise_start_sample', 'ui_name': 'Start Sample to Denoise'} */
                    {
                    CSycles.integrator_set_denoise_start_sample(this.Ptr, data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Integrator (setter)");
            }
        }

        internal override void SetEnum(string name, object data)
        {
            switch(name) {
            case "denoiser_prefilter":
                    /* integrator . {'datatype': 'ENUM', 'default_value': 'DENOISER_PREFILTER_ACCURATE', 'default_value_type': 'DenoiserPrefilter', 'is_input': True, 'member_name': 'denoiser_prefilter', 'ui_name': 'Denoiser Prefilter'} */
                    {
                    CSycles.integrator_set_denoiser_prefilter(this.Ptr, (ccl.DenoiserPrefilter)data);
                    }
                    break;
            case "denoiser_quality":
                    /* integrator . {'datatype': 'ENUM', 'default_value': 'DENOISER_QUALITY_HIGH', 'default_value_type': 'DenoiserQuality', 'is_input': True, 'member_name': 'denoiser_quality', 'ui_name': 'Denoiser Quality'} */
                    {
                    CSycles.integrator_set_denoiser_quality(this.Ptr, (ccl.DenoiserQuality)data);
                    }
                    break;
            case "guiding_distribution_type":
                    /* integrator . {'datatype': 'ENUM', 'default_value': 'GUIDING_TYPE_PARALLAX_AWARE_VMM', 'default_value_type': 'GuidingDistributionType', 'is_input': True, 'member_name': 'guiding_distribution_type', 'ui_name': 'Guiding Distribution Type'} */
                    {
                    CSycles.integrator_set_guiding_distribution_type(this.Ptr, (ccl.GuidingDistributionType)data);
                    }
                    break;
            case "guiding_directional_sampling_type":
                    /* integrator . {'datatype': 'ENUM', 'default_value': 'GUIDING_DIRECTIONAL_SAMPLING_TYPE_RIS', 'default_value_type': 'GuidingDirectionalSamplingType', 'is_input': True, 'member_name': 'guiding_directional_sampling_type', 'ui_name': 'Guiding Directional Sampling Type'} */
                    {
                    CSycles.integrator_set_guiding_directional_sampling_type(this.Ptr, (ccl.GuidingDirectionalSamplingType)data);
                    }
                    break;
            case "sampling_pattern":
                    /* integrator . {'datatype': 'ENUM', 'default_value': 'SAMPLING_PATTERN_TABULATED_SOBOL', 'default_value_type': 'SamplingPattern', 'is_input': True, 'member_name': 'sampling_pattern', 'ui_name': 'Sampling Pattern'} */
                    {
                    CSycles.integrator_set_sampling_pattern(this.Ptr, (ccl.SamplingPattern)data);
                    }
                    break;
            case "denoiser_type":
                    /* integrator . {'datatype': 'ENUM', 'default_value': 'DENOISER_OPENIMAGEDENOISE', 'default_value_type': 'DenoiserType', 'is_input': True, 'member_name': 'denoiser_type', 'ui_name': 'Denoiser Type'} */
                    {
                    CSycles.integrator_set_denoiser_type(this.Ptr, (ccl.DenoiserType)data);
                    }
                    break;

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Integrator (setter)");
            }
        }

#endregion
#region Getters

        internal override float GetFloat(string name)
        {
            switch(name) {
            case "sample_clamp_indirect":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '10.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_clamp_indirect', 'ui_name': 'Sample Clamp Indirect'} */
                {
                    return CSycles.integrator_get_sample_clamp_indirect(this.Ptr);
                }
            case "guiding_roughness_threshold":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '0.05f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'guiding_roughness_threshold', 'ui_name': 'Guiding Roughness Threshold'} */
                {
                    return CSycles.integrator_get_guiding_roughness_threshold(this.Ptr);
                }
            case "adaptive_threshold":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '0.01f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'adaptive_threshold', 'ui_name': 'Adaptive Threshold'} */
                {
                    return CSycles.integrator_get_adaptive_threshold(this.Ptr);
                }
            case "ao_factor":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ao_factor', 'ui_name': 'AO Factor'} */
                {
                    return CSycles.integrator_get_ao_factor(this.Ptr);
                }
            case "filter_glossy":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'filter_glossy', 'ui_name': 'Filter Glossy'} */
                {
                    return CSycles.integrator_get_filter_glossy(this.Ptr);
                }
            case "ao_distance":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '3.40282347e+38F', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ao_distance', 'ui_name': 'AO Distance'} */
                {
                    return CSycles.integrator_get_ao_distance(this.Ptr);
                }
            case "ao_additive_factor":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'ao_additive_factor', 'ui_name': 'AO Additive Factor'} */
                {
                    return CSycles.integrator_get_ao_additive_factor(this.Ptr);
                }
            case "light_sampling_threshold":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'light_sampling_threshold', 'ui_name': 'Light Sampling Threshold'} */
                {
                    return CSycles.integrator_get_light_sampling_threshold(this.Ptr);
                }
            case "volume_step_rate":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'volume_step_rate', 'ui_name': 'Volume Step Rate'} */
                {
                    return CSycles.integrator_get_volume_step_rate(this.Ptr);
                }
            case "scrambling_distance":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '1.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'scrambling_distance', 'ui_name': 'Scrambling Distance'} */
                {
                    return CSycles.integrator_get_scrambling_distance(this.Ptr);
                }
            case "surface_guiding_probability":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'surface_guiding_probability', 'ui_name': 'Surface Guiding Probability'} */
                {
                    return CSycles.integrator_get_surface_guiding_probability(this.Ptr);
                }
            case "sample_clamp_direct":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '0.0f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'sample_clamp_direct', 'ui_name': 'Sample Clamp Direct'} */
                {
                    return CSycles.integrator_get_sample_clamp_direct(this.Ptr);
                }
            case "volume_guiding_probability":
                /* integrator . {'datatype': 'FLOAT', 'default_value': '0.5f', 'default_value_type': 'float', 'is_input': True, 'member_name': 'volume_guiding_probability', 'ui_name': 'Volume Guiding Probability'} */
                {
                    return CSycles.integrator_get_volume_guiding_probability(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Integrator (getter)");
            }
        }

        internal override bool GetBool(string name)
        {
            switch(name) {
            case "denoise_use_gpu":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'denoise_use_gpu', 'ui_name': 'Denoise on GPU'} */
                {
                    return CSycles.integrator_get_denoise_use_gpu(this.Ptr);
                }
            case "use_guiding_direct_light":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_guiding_direct_light', 'ui_name': 'Guide Direct Light'} */
                {
                    return CSycles.integrator_get_use_guiding_direct_light(this.Ptr);
                }
            case "motion_blur":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'motion_blur', 'ui_name': 'Motion Blur'} */
                {
                    return CSycles.integrator_get_motion_blur(this.Ptr);
                }
            case "use_guiding_mis_weights":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_guiding_mis_weights', 'ui_name': 'Use MIS Weights'} */
                {
                    return CSycles.integrator_get_use_guiding_mis_weights(this.Ptr);
                }
            case "use_sample_subset":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_sample_subset', 'ui_name': 'Use Sample Subset'} */
                {
                    return CSycles.integrator_get_use_sample_subset(this.Ptr);
                }
            case "caustics_reflective":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'caustics_reflective', 'ui_name': 'Reflective Caustics'} */
                {
                    return CSycles.integrator_get_caustics_reflective(this.Ptr);
                }
            case "use_adaptive_sampling":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_adaptive_sampling', 'ui_name': 'Use Adaptive Sampling'} */
                {
                    return CSycles.integrator_get_use_adaptive_sampling(this.Ptr);
                }
            case "caustics_refractive":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'caustics_refractive', 'ui_name': 'Refractive Caustics'} */
                {
                    return CSycles.integrator_get_caustics_refractive(this.Ptr);
                }
            case "use_direct_light":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_direct_light', 'ui_name': 'Use Direct Light'} */
                {
                    return CSycles.integrator_get_use_direct_light(this.Ptr);
                }
            case "use_light_tree":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_light_tree', 'ui_name': 'Use light tree to optimize many light sampling'} */
                {
                    return CSycles.integrator_get_use_light_tree(this.Ptr);
                }
            case "use_indirect_light":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_indirect_light', 'ui_name': 'Use Indirect Light'} */
                {
                    return CSycles.integrator_get_use_indirect_light(this.Ptr);
                }
            case "use_diffuse":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_diffuse', 'ui_name': 'Use Diffuse'} */
                {
                    return CSycles.integrator_get_use_diffuse(this.Ptr);
                }
            case "use_glossy":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_glossy', 'ui_name': 'Use Glossy'} */
                {
                    return CSycles.integrator_get_use_glossy(this.Ptr);
                }
            case "use_guiding":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_guiding', 'ui_name': 'Guiding'} */
                {
                    return CSycles.integrator_get_use_guiding(this.Ptr);
                }
            case "use_transmission":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_transmission', 'ui_name': 'Use Transmission'} */
                {
                    return CSycles.integrator_get_use_transmission(this.Ptr);
                }
            case "use_denoise":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'false', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_denoise', 'ui_name': 'Use Denoiser'} */
                {
                    return CSycles.integrator_get_use_denoise(this.Ptr);
                }
            case "deterministic_guiding":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'deterministic_guiding', 'ui_name': 'Deterministic Guiding'} */
                {
                    return CSycles.integrator_get_deterministic_guiding(this.Ptr);
                }
            case "use_emission":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_emission', 'ui_name': 'Use Emission'} */
                {
                    return CSycles.integrator_get_use_emission(this.Ptr);
                }
            case "use_surface_guiding":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_surface_guiding', 'ui_name': 'Surface Guiding'} */
                {
                    return CSycles.integrator_get_use_surface_guiding(this.Ptr);
                }
            case "use_denoise_pass_albedo":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_denoise_pass_albedo', 'ui_name': 'Use Albedo Pass for Denoiser'} */
                {
                    return CSycles.integrator_get_use_denoise_pass_albedo(this.Ptr);
                }
            case "use_volume_guiding":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_volume_guiding', 'ui_name': 'Volume Guiding'} */
                {
                    return CSycles.integrator_get_use_volume_guiding(this.Ptr);
                }
            case "use_denoise_pass_normal":
                /* integrator . {'datatype': 'BOOLEAN', 'default_value': 'true', 'default_value_type': 'bool', 'is_input': True, 'member_name': 'use_denoise_pass_normal', 'ui_name': 'Use Normal Pass for Denoiser'} */
                {
                    return CSycles.integrator_get_use_denoise_pass_normal(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Integrator (getter)");
            }
        }

        internal override int GetInt(string name)
        {
            switch(name) {
            case "guiding_training_samples":
                /* integrator . {'datatype': 'INT', 'default_value': '128', 'default_value_type': 'int', 'is_input': True, 'member_name': 'guiding_training_samples', 'ui_name': 'Training Samples'} */
                {
                    return CSycles.integrator_get_guiding_training_samples(this.Ptr);
                }
            case "max_diffuse_bounce":
                /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_diffuse_bounce', 'ui_name': 'Max Diffuse Bounce'} */
                {
                    return CSycles.integrator_get_max_diffuse_bounce(this.Ptr);
                }
            case "max_glossy_bounce":
                /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_glossy_bounce', 'ui_name': 'Max Glossy Bounce'} */
                {
                    return CSycles.integrator_get_max_glossy_bounce(this.Ptr);
                }
            case "aa_samples":
                /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'aa_samples', 'ui_name': 'AA Samples'} */
                {
                    return CSycles.integrator_get_aa_samples(this.Ptr);
                }
            case "max_transmission_bounce":
                /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_transmission_bounce', 'ui_name': 'Max Transmission Bounce'} */
                {
                    return CSycles.integrator_get_max_transmission_bounce(this.Ptr);
                }
            case "max_volume_bounce":
                /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_volume_bounce', 'ui_name': 'Max Volume Bounce'} */
                {
                    return CSycles.integrator_get_max_volume_bounce(this.Ptr);
                }
            case "max_bounce":
                /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'max_bounce', 'ui_name': 'Max Bounce'} */
                {
                    return CSycles.integrator_get_max_bounce(this.Ptr);
                }
            case "sample_subset_offset":
                /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'sample_subset_offset', 'ui_name': 'Sample Subset Offset'} */
                {
                    return CSycles.integrator_get_sample_subset_offset(this.Ptr);
                }
            case "transparent_min_bounce":
                /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'transparent_min_bounce', 'ui_name': 'Transparent Min Bounce'} */
                {
                    return CSycles.integrator_get_transparent_min_bounce(this.Ptr);
                }
            case "min_bounce":
                /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'min_bounce', 'ui_name': 'Min Bounce'} */
                {
                    return CSycles.integrator_get_min_bounce(this.Ptr);
                }
            case "transparent_max_bounce":
                /* integrator . {'datatype': 'INT', 'default_value': '7', 'default_value_type': 'int', 'is_input': True, 'member_name': 'transparent_max_bounce', 'ui_name': 'Transparent Max Bounce'} */
                {
                    return CSycles.integrator_get_transparent_max_bounce(this.Ptr);
                }
            case "sample_subset_length":
                /* integrator . {'datatype': 'INT', 'default_value': '(1<<24)', 'default_value_type': 'const int', 'is_input': True, 'member_name': 'sample_subset_length', 'ui_name': 'Sample Subset Length'} */
                {
                    return CSycles.integrator_get_sample_subset_length(this.Ptr);
                }
            case "ao_bounces":
                /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'ao_bounces', 'ui_name': 'AO Bounces'} */
                {
                    return CSycles.integrator_get_ao_bounces(this.Ptr);
                }
            case "adaptive_min_samples":
                /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'adaptive_min_samples', 'ui_name': 'Adaptive Min Samples'} */
                {
                    return CSycles.integrator_get_adaptive_min_samples(this.Ptr);
                }
            case "volume_max_steps":
                /* integrator . {'datatype': 'INT', 'default_value': '1024', 'default_value_type': 'int', 'is_input': True, 'member_name': 'volume_max_steps', 'ui_name': 'Volume Max Steps'} */
                {
                    return CSycles.integrator_get_volume_max_steps(this.Ptr);
                }
            case "seed":
                /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'seed', 'ui_name': 'Seed'} */
                {
                    return CSycles.integrator_get_seed(this.Ptr);
                }
            case "denoise_start_sample":
                /* integrator . {'datatype': 'INT', 'default_value': '0', 'default_value_type': 'int', 'is_input': True, 'member_name': 'denoise_start_sample', 'ui_name': 'Start Sample to Denoise'} */
                {
                    return CSycles.integrator_get_denoise_start_sample(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Integrator (getter)");
            }
        }

        internal override object GetEnum(string name)
        {
            switch(name) {
            case "denoiser_prefilter":
                /* integrator . {'datatype': 'ENUM', 'default_value': 'DENOISER_PREFILTER_ACCURATE', 'default_value_type': 'DenoiserPrefilter', 'is_input': True, 'member_name': 'denoiser_prefilter', 'ui_name': 'Denoiser Prefilter'} */
                {
                    return (uint)CSycles.integrator_get_denoiser_prefilter(this.Ptr);
                }
            case "denoiser_quality":
                /* integrator . {'datatype': 'ENUM', 'default_value': 'DENOISER_QUALITY_HIGH', 'default_value_type': 'DenoiserQuality', 'is_input': True, 'member_name': 'denoiser_quality', 'ui_name': 'Denoiser Quality'} */
                {
                    return (uint)CSycles.integrator_get_denoiser_quality(this.Ptr);
                }
            case "guiding_distribution_type":
                /* integrator . {'datatype': 'ENUM', 'default_value': 'GUIDING_TYPE_PARALLAX_AWARE_VMM', 'default_value_type': 'GuidingDistributionType', 'is_input': True, 'member_name': 'guiding_distribution_type', 'ui_name': 'Guiding Distribution Type'} */
                {
                    return (uint)CSycles.integrator_get_guiding_distribution_type(this.Ptr);
                }
            case "guiding_directional_sampling_type":
                /* integrator . {'datatype': 'ENUM', 'default_value': 'GUIDING_DIRECTIONAL_SAMPLING_TYPE_RIS', 'default_value_type': 'GuidingDirectionalSamplingType', 'is_input': True, 'member_name': 'guiding_directional_sampling_type', 'ui_name': 'Guiding Directional Sampling Type'} */
                {
                    return (uint)CSycles.integrator_get_guiding_directional_sampling_type(this.Ptr);
                }
            case "sampling_pattern":
                /* integrator . {'datatype': 'ENUM', 'default_value': 'SAMPLING_PATTERN_TABULATED_SOBOL', 'default_value_type': 'SamplingPattern', 'is_input': True, 'member_name': 'sampling_pattern', 'ui_name': 'Sampling Pattern'} */
                {
                    return (uint)CSycles.integrator_get_sampling_pattern(this.Ptr);
                }
            case "denoiser_type":
                /* integrator . {'datatype': 'ENUM', 'default_value': 'DENOISER_OPENIMAGEDENOISE', 'default_value_type': 'DenoiserType', 'is_input': True, 'member_name': 'denoiser_type', 'ui_name': 'Denoiser Type'} */
                {
                    return (uint)CSycles.integrator_get_denoiser_type(this.Ptr);
                }

                default: throw new ArgumentException($"Unknown input socket name '{name}' for node type Integrator (getter)");
            }
        }

#endregion
    }

}