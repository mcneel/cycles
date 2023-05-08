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

void cycles_integrator_tag_update(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->tag_update(sce, ccl::Integrator::UPDATE_ALL);
	}
}

// Integrator settings
void cycles_integrator_set_max_bounce(ccl::Session* session_id, int max_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_bounce(max_bounce);
	}
}

void cycles_integrator_set_min_bounce(ccl::Session* session_id, int min_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_min_bounce(min_bounce);
	}
}

void cycles_integrator_set_no_caustics(ccl::Session* session_id, bool no_caustics)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_caustics_reflective(!no_caustics);
		sce->integrator->set_caustics_refractive(!no_caustics);
	}
}

void cycles_integrator_set_no_shadows(ccl::Session* session_id, bool no_shadows)
{
#if 0
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->no_shadows = no_shadows;
	}
#endif
}

// TODO: XXXX rename sample functions, they are now setting max bounce.
// integrator has changed to do adaptive sampling where possible. This
// means evaluating a pixel may stop before max bounce is reached.

void cycles_integrator_set_diffuse_samples(ccl::Session* session_id, int diffuse_samples)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_diffuse_bounce(diffuse_samples);
	}
}

void cycles_integrator_set_glossy_samples(ccl::Session* session_id, int glossy_samples)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_glossy_bounce(glossy_samples);
	}
}

void cycles_integrator_set_transmission_samples(ccl::Session* session_id, int transmission_samples)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_transmission_bounce(transmission_samples);
	}
}

void cycles_integrator_set_ao_samples(ccl::Session* session_id, int ao_samples)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
        sce->integrator->set_ao_bounces(ao_samples);
	}
}

void cycles_integrator_set_mesh_light_samples(ccl::Session* session_id, int mesh_light_samples)
{
    // TODO: XXXX revisit light sampling approaches
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_li;
	}
    */
}

void cycles_integrator_set_subsurface_samples(ccl::Session* session_id, int subsurface_samples)
{
    // TODO: XXXX re-evaluate SSS sampling, if still necessary
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->subsurface_samples = subsurface_samples;
	}
    */
}

void cycles_integrator_set_volume_samples(ccl::Session* session_id, int volume_samples)
{
    // TODO: XXXX wrap volume related integrator settings
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_volume_max_steps(volume_samples);
	}
}

void cycles_integrator_set_max_diffuse_bounce(ccl::Session* session_id, int max_diffuse_bounce)
{
    // TODO: XXXX fold into diffuse bounce
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_diffuse_bounce(max_diffuse_bounce);
	}
}

void cycles_integrator_set_max_glossy_bounce(ccl::Session* session_id, int max_glossy_bounce)
{
    // TODO: XXXX fold into glossy bounce
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_glossy_bounce(max_glossy_bounce);
	}
}

void cycles_integrator_set_max_transmission_bounce(ccl::Session* session_id, int max_transmission_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_transmission_bounce(max_transmission_bounce);
	}
}

void cycles_integrator_set_max_volume_bounce(ccl::Session* session_id, int max_volume_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_max_volume_bounce(max_volume_bounce);
	}
}

void cycles_integrator_set_transparent_max_bounce(ccl::Session* session_id, int transparent_max_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_transparent_max_bounce(transparent_max_bounce);
	}
}

void cycles_integrator_set_transparent_min_bounce(ccl::Session* session_id, int transparent_min_bounce)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_transparent_min_bounce(transparent_min_bounce);
	}
}

void cycles_integrator_set_aa_samples(ccl::Session* session_id, int aa_samples)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_aa_samples(aa_samples);
	}
}

void cycles_integrator_set_filter_glossy(ccl::Session* session_id, float filter_glossy)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_filter_glossy(filter_glossy);
	}
}

void cycles_integrator_set_method(ccl::Session* session_id, int method)
{
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->method = (ccl::Integrator::Method)method;
	}
    */
}

void cycles_integrator_set_sample_all_lights_direct(ccl::Session* session_id, bool sample_all_lights_direct)
{
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->sample_all_lights_direct = sample_all_lights_direct;
	}
     */
}

void cycles_integrator_set_sample_all_lights_indirect(ccl::Session* session_id, bool sample_all_lights_indirect)
{
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->sample_all_lights_indirect = sample_all_lights_indirect;
	}
    */
}

void cycles_integrator_set_volume_step_size(ccl::Session* session_id, float volume_step_size)
{
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->volume_step_size = volume_step_size;
	}
    */
}

void cycles_integrator_set_volume_max_steps(ccl::Session* session_id, int volume_max_steps)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_volume_max_steps(volume_max_steps);
	}
}

/* \todo update Cycles code to allow for caustics form separation
void cycles_integrator_set_caustics_relective(ccl::Session* session_id, int caustics_relective)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->caustics_relective = caustics_relective;
	}
}

void cycles_integrator_set_caustics_refractive(ccl::Session* session_id, int caustics_refractive)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->caustics_refractive = caustics_refractive;
	}
}
*/

void cycles_integrator_set_seed(ccl::Session* session_id, int seed)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_seed(seed);
	}
}

void cycles_integrator_set_sampling_pattern(ccl::Session* session_id, sampling_pattern pattern)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_sampling_pattern((ccl::SamplingPattern)pattern);
	}
}

void cycles_integrator_set_sample_clamp_direct(ccl::Session* session_id, float sample_clamp_direct)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_sample_clamp_direct(sample_clamp_direct);
	}
}

void cycles_integrator_set_sample_clamp_indirect(ccl::Session* session_id, float sample_clamp_indirect)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_sample_clamp_indirect(sample_clamp_indirect);
	}
}

void cycles_integrator_set_light_sampling_threshold(ccl::Session* session_id, float light_sampling_threshold)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->integrator->set_light_sampling_threshold(light_sampling_threshold);
	}
}
