/**
Copyright 2014-2024 Robert McNeel and Associates

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
**/

namespace ccl
{
	/// <summary>
	/// Access to session Integrator settings for Cycles
	/// </summary>
	public class Integrator
	{
		/// <summary>
		/// Reference to session to which these integrator settings belong.
		/// </summary>
		internal Session Session { get; set; }

		/// <summary>
		/// Create a new integrator settings representation
		/// </summary>
		/// <param name="session"></param>
		internal Integrator(Session session)
		{
			Session = session;
		}

		/// <summary>
		/// Tag the integrator as changed.
		/// </summary>
		public void TagForUpdate()
		{
			CSycles.integrator_tag_update(Session.Id);
		}

		/// <summary>
		/// Set the maximum amount of bounces for a ray.
		/// </summary>
		public int MaxBounce
		{
			set
			{
				CSycles.integrator_set_max_bounce(Session.Id, value);

			}
		}

		/// <summary>
		/// Set the minimum amount of bounces for a ray.
		/// </summary>
		public int MinBounce
		{
			set
			{
				CSycles.integrator_set_min_bounce(Session.Id, value);

			}
		}

		/// <summary>
		/// Set the maximum amount of bounces for a transparency ray.
		///
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int TransparentMaxBounce
		{
			set
			{
				CSycles.integrator_set_transparent_max_bounce(Session.Id, value);
			}
		}
		/// <summary>
		/// Set the minimum amount of bounces for a transparency ray.
		///
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int TransparentMinBounce
		{
			set
			{
				CSycles.integrator_set_transparent_min_bounce(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of bounces for a diffuse ray.
		///
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int MaxDiffuseBounce
		{
			set
			{
				CSycles.integrator_set_max_diffuse_bounce(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of bounces for a glossy ray.
		/// </summary>
		public int MaxGlossyBounce
		{
			set
			{
				CSycles.integrator_set_max_glossy_bounce(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the maximimum amount of bounces for transmission rays.
		///
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int MaxTransmissionBounce
		{
			set
			{
				CSycles.integrator_set_max_transmission_bounce(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of bounces for volume rays.
		///
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int MaxVolumeBounce
		{
			set
			{
				CSycles.integrator_set_max_volume_bounce(Session.Id, value);
			}
		}

		/// <summary>
		/// Set to true if no caustics should be used
		/// </summary>
		public bool NoCaustics
		{
			set
			{
				CSycles.integrator_set_no_caustics(Session.Id, value);
			}
		}

		/// <summary>
		/// Set to true for reflective caustics
		/// </summary>
		public bool CausticsReflective
		{
			set
			{
				CSycles.integrator_set_caustics_reflective(Session.Id, value);
			}
		}

		/// <summary>
		/// Set to true for refractive caustics
		/// </summary>
		public bool CausticsRefractive
		{
			set
			{
				CSycles.integrator_set_caustics_refractive(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of bounces for AO rays
		///
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public int AoBounces
		{
			set
			{
				CSycles.integrator_set_ao_bounces(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of factor for AO rays
		///
		/// Used when BranchedPath tracing is set.
		/// </summary>
		public float AoFactor
		{
			set
			{
				CSycles.integrator_set_ao_factor(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of distance for AO
		/// </summary>
		public float AoDistance
		{
			set
			{
				CSycles.integrator_set_ao_distance(Session.Id, value);
			}
		}

		/// <summary>
		/// Set additive factor for AO
		/// </summary>
		public float AoAdditiveFactor
		{
			set
			{
				CSycles.integrator_set_ao_additive_factor(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of volume samples
		/// </summary>
		public int VolumeSamples
		{
			set
			{
				CSycles.integrator_set_volume_max_steps(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the amount of AA samples per ray
		/// </summary>
		public int AaSamples
		{
			set
			{
				CSycles.integrator_set_aa_samples(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the glossy filter size
		/// </summary>
		public float FilterGlossy
		{
			set
			{
				CSycles.integrator_set_filter_glossy(Session.Id, value);
			}
		}

		/// <summary>
		/// Set to true if all direct lights should be used.
		/// </summary>
		public bool UseDirectLight
		{
			set
			{
				CSycles.integrator_set_use_direct_light(Session.Id, value);
			}
		}

		/// <summary>
		/// Set to true if all indirect lights should be used.
		/// </summary>
		public bool UseIndirectLight
		{
			set
			{
				CSycles.integrator_set_use_indirect_light(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the step rate for volume tracing
		/// </summary>
		public float VolumeStepRate
		{
			set
			{
				CSycles.integrator_set_volume_step_rate(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the maximum amount of steps for volume tracing.
		/// </summary>
		public int VolumeMaxSteps
		{
			set
			{
				CSycles.integrator_set_volume_max_steps(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the seed for sampling
		/// </summary>
		public int Seed
		{
			set
			{
				CSycles.integrator_set_seed(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the clamp value for direct samples.
		/// </summary>
		public float SampleClampDirect
		{
			set
			{
				CSycles.integrator_set_sample_clamp_direct(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the clamp value for indirect samples.
		/// </summary>
		public float SampleClampIndirect
		{
			set
			{
				CSycles.integrator_set_sample_clamp_indirect(Session.Id, value);
			}
		}

		/// <summary>
		/// Set light sampling threshold. Contribution below given threshold will be discarded.
		/// </summary>
		public float LightSamplingThreshold
		{
			set
			{
				CSycles.integrator_set_light_sampling_threshold(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the sampling pattern to use (CMJ or Sobol).
		/// </summary>
		public SamplingPattern SamplingPattern
		{
			set
			{
				CSycles.integrator_set_sampling_pattern(Session.Id, value);
			}
		}

		/// <summary>
		/// Set to true to use light trees.
		/// </summary>
		public bool UseLightTree
		{
			set
			{
				CSycles.integrator_set_use_light_tree(Session.Id, value);
			}
		}

		/// <summary>
		/// Set to true to use adaptive sampling.
		/// </summary>
		public bool UseAdaptiveSampling
		{
			set
			{
				CSycles.integrator_set_use_adaptive_sampling(Session.Id, value);
			}
		}

		/// <summary>
		/// Set to 0 to let Cycles choose the best number of samples
		/// from which on adaptive sampling will be used.
		/// </summary>
		public int AdaptiveMinSamples
		{
			set
			{
				CSycles.integrator_set_adaptive_min_samples(Session.Id, value);
			}
		}

		/// <summary>
		/// Set the adaptive sampling threshold. When changes in a pixel
		/// fall below this threshold, the pixel is considered converged.
		/// </summary>
		public float AdaptiveThreshold
		{
			set {
				CSycles.integrator_set_adaptive_threshold(Session.Id, value);
			}
		}
	}
}
