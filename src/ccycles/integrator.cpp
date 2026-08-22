/**
Copyright 2014-2017 Robert McNeel and Associates

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

#include "internal_types.h"

// TODO: XXXX investigate light tree usage
// TODO: XXXX provide access to adaptive sampling
// TODO: XXXX provide access to guiding parameters

#ifdef __cplusplus
extern "C" {
#endif

CCL_CAPI void CDECL cycles_integrator_tag_update(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->tag_update(sce, ccl::Integrator::UPDATE_ALL);
	}
}

// Integrator settings
CCL_CAPI void CDECL cycles_integrator_set_max_bounce(ccl::Session* session_id, int max_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_bounce(max_bounce);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_min_bounce(ccl::Session* session_id, int min_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_min_bounce(min_bounce);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_no_caustics(ccl::Session* session_id, bool no_caustics)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_caustics_reflective(!no_caustics);
		sce->integrator->set_caustics_refractive(!no_caustics);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_ao_bounces(ccl::Session* session_id, int ao_bounces)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
        sce->integrator->set_ao_bounces(ao_bounces);
	}
}
CCL_CAPI void CDECL cycles_integrator_set_ao_factor(ccl::Session *session_id, float ao_factor)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
        sce->integrator->set_ao_factor(ao_factor);
	}
}
CCL_CAPI void CDECL cycles_integrator_set_ao_distance(ccl::Session *session_id, float ao_distance)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
        sce->integrator->set_ao_distance(ao_distance);
	}
}
CCL_CAPI void CDECL cycles_integrator_set_ao_additive_factor(ccl::Session *session_id, float ao_additive_factor)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
        sce->integrator->set_ao_additive_factor(ao_additive_factor);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_max_diffuse_bounce(ccl::Session* session_id, int max_diffuse_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_diffuse_bounce(max_diffuse_bounce);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_max_glossy_bounce(ccl::Session* session_id, int max_glossy_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_glossy_bounce(max_glossy_bounce);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_max_transmission_bounce(ccl::Session* session_id, int max_transmission_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_transmission_bounce(max_transmission_bounce);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_max_volume_bounce(ccl::Session* session_id, int max_volume_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_volume_bounce(max_volume_bounce);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_transparent_max_bounce(ccl::Session* session_id, int transparent_max_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_transparent_max_bounce(transparent_max_bounce);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_transparent_min_bounce(ccl::Session* session_id, int transparent_min_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_transparent_min_bounce(transparent_min_bounce);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_aa_samples(ccl::Session* session_id, int aa_samples)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_aa_samples(aa_samples);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_filter_glossy(ccl::Session* session_id, float filter_glossy)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_filter_glossy(filter_glossy);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_use_direct_light(ccl::Session *session_id, bool use_direct_light)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_use_direct_light(use_direct_light);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_use_indirect_light(ccl::Session *session_id, bool use_indirect_light)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_use_indirect_light(use_indirect_light);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_volume_step_rate(ccl::Session* session_id, float volume_step_rate)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_volume_step_rate(volume_step_rate);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_volume_max_steps(ccl::Session* session_id, int volume_max_steps)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_volume_max_steps(volume_max_steps);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_caustics_reflective(ccl::Session* session_id, bool caustics_reflective)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_caustics_reflective(caustics_reflective);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_caustics_refractive(ccl::Session* session_id, bool caustics_refractive)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_caustics_refractive(caustics_refractive);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_seed(ccl::Session* session_id, int seed)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_seed(seed);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_sampling_pattern(ccl::Session* session_id, sampling_pattern pattern)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_sampling_pattern((ccl::SamplingPattern)pattern);
	}
}

/* Experiment switch: clamping is biased by design - it discards the part of a
 * sample's radiance above the limit - so any change in the per-sample radiance
 * distribution shows up as a brightness-graded difference even when the clamp
 * value itself is untouched. CCYCLES_NO_CLAMP=1 sets both limits to 0, which
 * Cycles reads as FLT_MAX, so a render can be compared with the bias removed.
 *
 * Note that Integrator multiplies whatever it is given by 3 before handing it
 * to the kernel, so a scene value of 3 is a kernel limit of 9. That scaling is
 * the same in 3.5 and 5.2. */
static float ccycles_clamp_override(float value, const char *which)
{
	const char *no_clamp = getenv("CCYCLES_NO_CLAMP");
	if (no_clamp != nullptr && no_clamp[0] == 0x31 && value != 0.0f) {
		ccycles_diag("disabling %s clamp (was %f)\n", which, value);
		return 0.0f;
	}
	return value;
}

CCL_CAPI void CDECL cycles_integrator_set_sample_clamp_direct(ccl::Session* session_id, float sample_clamp_direct)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_sample_clamp_direct(ccycles_clamp_override(sample_clamp_direct, "direct"));
	}
}

CCL_CAPI void CDECL cycles_integrator_set_sample_clamp_indirect(ccl::Session* session_id, float sample_clamp_indirect)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_sample_clamp_indirect(ccycles_clamp_override(sample_clamp_indirect, "indirect"));
	}
}

CCL_CAPI void CDECL cycles_integrator_set_light_sampling_threshold(ccl::Session* session_id, float light_sampling_threshold)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_light_sampling_threshold(light_sampling_threshold);
	}
}

CCL_CAPI void CDECL cycles_integrators_set_use_light_tree(ccl::Session* session_id, bool use_light_tree)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		/* Experiment switch: the light tree is how 5.x picks an emitter, and a
		 * background emitter goes through it differently than a local light.
		 * CCYCLES_NO_LIGHT_TREE forces the older distribution instead. */
		const char *no_tree = getenv("CCYCLES_NO_LIGHT_TREE");
		if (no_tree != nullptr && no_tree[0] == 0x31) {
			ccycles_diag("forcing use_light_tree off (was %d)\n", (int)use_light_tree);
			use_light_tree = false;
		}
		sce->integrator->set_use_light_tree(use_light_tree);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_use_adaptive_sampling(ccl::Session* session_id, bool use_adaptive_sampling)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_use_adaptive_sampling(use_adaptive_sampling);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_adaptive_min_samples(ccl::Session* session_id, int adaptive_min_samples)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_adaptive_min_samples(adaptive_min_samples);
	}
}

CCL_CAPI void CDECL cycles_integrator_set_adaptive_threshold(ccl::Session* session_id, float adaptive_threshold)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_adaptive_threshold(adaptive_threshold);
	}
}

#ifdef __cplusplus
}
#endif
