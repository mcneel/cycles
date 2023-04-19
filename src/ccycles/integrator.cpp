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

void cycles_integrator_tag_update(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->tag_update(sce, ccl::Integrator::UPDATE_ALL);
	}
}

// Integrator settings
void cycles_integrator_set_max_bounce(unsigned int client_id, unsigned int scene_id, int max_bounce)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_max_bounce(max_bounce);
	}
}

void cycles_integrator_set_min_bounce(unsigned int client_id, unsigned int scene_id, int min_bounce)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_min_bounce(min_bounce);
	}
}

void cycles_integrator_set_no_caustics(unsigned int client_id, unsigned int scene_id, bool no_caustics)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_caustics_reflective(!no_caustics);
		sce->integrator->set_caustics_refractive(!no_caustics);
	}
}

void cycles_integrator_set_no_shadows(unsigned int client_id, unsigned int scene_id, bool no_shadows)
{
#if 0
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->no_shadows = no_shadows;
	}
#endif
}

// TODO: XXXX rename sample functions, they are now setting max bounce.
// integrator has changed to do adaptive sampling where possible. This
// means evaluating a pixel may stop before max bounce is reached.

void cycles_integrator_set_diffuse_samples(unsigned int client_id, unsigned int scene_id, int diffuse_samples)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_max_diffuse_bounce(diffuse_samples);
	}
}

void cycles_integrator_set_glossy_samples(unsigned int client_id, unsigned int scene_id, int glossy_samples)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_max_glossy_bounce(glossy_samples);
	}
}

void cycles_integrator_set_transmission_samples(unsigned int client_id, unsigned int scene_id, int transmission_samples)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_max_transmission_bounce(transmission_samples);
	}
}

void cycles_integrator_set_ao_samples(unsigned int client_id, unsigned int scene_id, int ao_samples)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
        sce->integrator->set_ao_bounces(ao_samples);
	}
}

void cycles_integrator_set_mesh_light_samples(unsigned int client_id, unsigned int scene_id, int mesh_light_samples)
{
    // TODO: XXXX revisit light sampling approaches
    /*
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_li;
	}
    */
}

void cycles_integrator_set_subsurface_samples(unsigned int client_id, unsigned int scene_id, int subsurface_samples)
{
    // TODO: XXXX re-evaluate SSS sampling, if still necessary
    /*
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->subsurface_samples = subsurface_samples;
	}
    */
}

void cycles_integrator_set_volume_samples(unsigned int client_id, unsigned int scene_id, int volume_samples)
{
    // TODO: XXXX wrap volume related integrator settings
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_volume_max_steps(volume_samples);
	}
}

void cycles_integrator_set_max_diffuse_bounce(unsigned int client_id, unsigned int scene_id, int max_diffuse_bounce)
{
    // TODO: XXXX fold into diffuse bounce
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_max_diffuse_bounce(max_diffuse_bounce);
	}
}

void cycles_integrator_set_max_glossy_bounce(unsigned int client_id, unsigned int scene_id, int max_glossy_bounce)
{
    // TODO: XXXX fold into glossy bounce
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_max_glossy_bounce(max_glossy_bounce);
	}
}

void cycles_integrator_set_max_transmission_bounce(unsigned int client_id, unsigned int scene_id, int max_transmission_bounce)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_max_transmission_bounce(max_transmission_bounce);
	}
}

void cycles_integrator_set_max_volume_bounce(unsigned int client_id, unsigned int scene_id, int max_volume_bounce)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_max_volume_bounce(max_volume_bounce);
	}
}

void cycles_integrator_set_transparent_max_bounce(unsigned int client_id, unsigned int scene_id, int transparent_max_bounce)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_transparent_max_bounce(transparent_max_bounce);
	}
}

void cycles_integrator_set_transparent_min_bounce(unsigned int client_id, unsigned int scene_id, int transparent_min_bounce)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_transparent_min_bounce(transparent_min_bounce);
	}
}

void cycles_integrator_set_aa_samples(unsigned int client_id, unsigned int scene_id, int aa_samples)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_aa_samples(aa_samples);
	}
}

void cycles_integrator_set_filter_glossy(unsigned int client_id, unsigned int scene_id, float filter_glossy)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_filter_glossy(filter_glossy);
	}
}

void cycles_integrator_set_method(unsigned int client_id, unsigned int scene_id, int method)
{
    /*
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->method = (ccl::Integrator::Method)method;
	}
    */
}

void cycles_integrator_set_sample_all_lights_direct(unsigned int client_id, unsigned int scene_id, bool sample_all_lights_direct)
{
    /*
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->sample_all_lights_direct = sample_all_lights_direct;
	}
     */
}

void cycles_integrator_set_sample_all_lights_indirect(unsigned int client_id, unsigned int scene_id, bool sample_all_lights_indirect)
{
    /*
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->sample_all_lights_indirect = sample_all_lights_indirect;
	}
    */
}

void cycles_integrator_set_volume_step_size(unsigned int client_id, unsigned int scene_id, float volume_step_size)
{
    /*
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->volume_step_size = volume_step_size;
	}
    */
}

void cycles_integrator_set_volume_max_steps(unsigned int client_id, unsigned int scene_id, int volume_max_steps)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_volume_max_steps(volume_max_steps);
	}
}

/* \todo update Cycles code to allow for caustics form separation
void cycles_integrator_set_caustics_relective(unsigned int client_id, unsigned int scene_id, int caustics_relective)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->caustics_relective = caustics_relective;
	}
}

void cycles_integrator_set_caustics_refractive(unsigned int client_id, unsigned int scene_id, int caustics_refractive)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->caustics_refractive = caustics_refractive;
	}
}
*/

void cycles_integrator_set_seed(unsigned int client_id, unsigned int scene_id, int seed)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_seed(seed);
	}
}

void cycles_integrator_set_sampling_pattern(unsigned int client_id, unsigned int scene_id, sampling_pattern pattern)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_sampling_pattern((ccl::SamplingPattern)pattern);
	}
}

void cycles_integrator_set_sample_clamp_direct(unsigned int client_id, unsigned int scene_id, float sample_clamp_direct)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_sample_clamp_direct(sample_clamp_direct);
	}
}

void cycles_integrator_set_sample_clamp_indirect(unsigned int client_id, unsigned int scene_id, float sample_clamp_indirect)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_sample_clamp_indirect(sample_clamp_indirect);
	}
}

void cycles_integrator_set_light_sampling_threshold(unsigned int client_id, unsigned int scene_id, float light_sampling_threshold)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->integrator->set_light_sampling_threshold(light_sampling_threshold);
	}
}
