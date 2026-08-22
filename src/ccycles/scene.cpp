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

/* Find pointers for CCScene and ccl::Scene. Return false if either fails. */
bool scene_find(ccl::Session* sid, ccl::Scene** sce)
{
	(*sce) = sid->scene.get();
	return *sce != nullptr;
}


/* Find a ccl::Shader in a given ccl::Scene, based on shader_id
*/
ccl::Shader* find_shader_in_scene(ccl::Scene* sce, unsigned int shader_id)
{
	/* 5.2: Scene::shaders is a unique_ptr_vector - indexed, not iterated. */
	if (shader_id < sce->shaders.size()) {
		return sce->shaders[shader_id];
	}
	return nullptr;
}

unsigned int get_idx_for_shader_in_scene(ccl::Scene* sce, ccl::Shader* sh)
{
	for (size_t idx = 0; idx < sce->shaders.size(); idx++) {
		if (sce->shaders[idx] == sh) {
			return (unsigned int)idx;
		}
	}
	return (unsigned int)-1;

}

/* implement CCScene methods*/

void CCScene::builtin_image_info(const std::string& builtin_name, void* builtin_data, ccl::ImageMetaData& imdata) //bool& is_float, int& width, int& height, int& depth, int& channels)
{
	CCImage* img = static_cast<CCImage*>(builtin_data);
	imdata.width = img->width;
	imdata.height = img->height;
	/* ImageMetaData has no depth in 5.2; 3D image metadata was dropped. */
	imdata.channels = img->channels;

	assert(false);
	// TODO: XXXX figure out how to do images
    // TODO: XXXX probably just utilize OIIO directly
    //imdata.is_float = img->is_float;
}

bool CCScene::builtin_image_pixels(const std::string& builtin_name, void* builtin_data, int tile, unsigned char* pixels, const size_t pixels_size, const bool associate_alpha, const bool free_cache)
{
	CCImage* img = static_cast<CCImage*>(builtin_data);
	memcpy(pixels, img->builtin_data, (size_t)(img->width*img->height*img->channels)*sizeof(unsigned char));
	return false;
}

bool CCScene::builtin_image_float_pixels(const std::string& builtin_name, void* builtin_data, int tile, float* pixels, const size_t pixels_size, const bool associate_alpha, const bool free_cache)
{
	CCImage* img = static_cast<CCImage*>(builtin_data);
	memcpy(pixels, img->builtin_data, (size_t)(img->width*img->height*img->channels)*sizeof(float));
	return false;
}

/* *** */

#ifdef __cplusplus
extern "C" {
#endif

CCL_CAPI unsigned int CDECL cycles_scene_create(unsigned int scene_params_id, unsigned int session_id)
{
	return UINT_MAX;
}

CCL_CAPI void CDECL cycles_scene_set_default_surface_shader(ccl::Session *session_id, ccl::Shader *shader_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->default_surface = shader_id;
		logger.logit("Scene ", session_id, " set default surface shader ", shader_id);
	}
}

CCL_CAPI ccl::Shader* CDECL cycles_scene_get_default_surface_shader(ccl::Session *session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		return sce->default_surface;
	}

	return nullptr;
}

CCL_CAPI ccl::Shader* CDECL cycles_scene_get_background_shader(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		return sce->default_background;
	}
	return nullptr;
}

/* Set shader_id as default background shader for session_id.
 * Note that shader_id is the ID for the shader specific to this scene.
 * 
 * The correct ID can be found with cycles_scene_shader_id. The ID is also
 * returned from cycles_scene_add_shader.
 */
CCL_CAPI void CDECL cycles_scene_set_background_shader(ccl::Session *session_id, ccl::Shader *shader_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->default_background = shader_id;
		sce->background->set_shader(shader_id);
		sce->background->set_use_shader(true);
		sce->background->tag_update(sce);
		logger.logit("Scene ", session_id, " set background shader ", shader_id);
	}
}

CCL_CAPI void CDECL cycles_scene_reset(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->reset();
	}
}

CCL_CAPI bool CDECL cycles_scene_try_lock(ccl::Session* session)
{
	return session->scene->mutex.try_lock();
}

CCL_CAPI void CDECL cycles_scene_lock(ccl::Session* session)
{
	session->scene->mutex.lock();
}

CCL_CAPI void CDECL cycles_scene_unlock(ccl::Session* session)
{
	session->scene->mutex.unlock();
}

#ifdef __cplusplus
}
#endif

/* Temporary diagnostic: report what the scene actually holds. Used by the
 * smoke test to tell "the geometry never arrived" apart from "the camera is
 * not looking at it". */
extern "C" CCL_CAPI void CDECL cycles_debug_scene_stats(ccl::Session *session_id)
{
	ccl::Scene *sce = session_id->scene.get();
	if (sce == nullptr) {
		ccycles_diag("stats: no scene\n");
		return;
	}

	ccycles_diag("stats: geometry=%zu objects=%zu shaders=%zu\n",
	       sce->geometry.size(), sce->objects.size(), sce->shaders.size());

	for (size_t i = 0; i < sce->objects.size(); i++) {
		ccl::Object *ob = sce->objects[i];
		ccl::Geometry *geo = ob->get_geometry();
		ccycles_diag("  object %zu geometry=%p vis=%u\n", i, (void *)geo, ob->get_visibility());
		if (ccl::Mesh *mesh = dynamic_cast<ccl::Mesh *>(geo)) {
			ccycles_diag("    mesh verts=%d tris=%d used_shaders=%zu\n",
			       (int)mesh->num_verts(), (int)mesh->num_triangles(),
			       mesh->get_used_shaders().size());
			/* A per-triangle index pointing at the wrong slot - or a slot holding
			 * an empty shader - renders black with everything else looking
			 * correct, so name the shader each slot resolves to. */
			const ccl::array<ccl::Node *> &us = mesh->get_used_shaders();
			for (size_t k = 0; k < us.size(); k++) {
				ccl::Shader *sh = static_cast<ccl::Shader *>(us[k]);
				ccycles_diag("      slot %zu -> '%s'\n", k,
				             sh == nullptr ? "(null)" : sh->name.c_str());
			}
			const ccl::array<int> &tri_shader = mesh->get_shader();
			int lo = -1, hi = -1;
			for (size_t k = 0; k < tri_shader.size(); k++) {
				if (k == 0 || tri_shader[k] < lo) lo = tri_shader[k];
				if (k == 0 || tri_shader[k] > hi) hi = tri_shader[k];
			}
			ccycles_diag("      tri shader index count=%zu min=%d max=%d\n",
			             tri_shader.size(), lo, hi);
		}
		else if (ccl::Light *lt = dynamic_cast<ccl::Light *>(geo)) {
			const ccl::Transform &t = ob->get_tfm();
			ccycles_diag("    light type=%d co=(%.2f %.2f %.2f) -col2=(%.2f %.2f %.2f)\n",
			       (int)lt->get_light_type(), t.x.w, t.y.w, t.z.w,
			       -t.x.z, -t.y.z, -t.z.z);
		}
		else {
			ccycles_diag("    unknown geometry\n");
		}
	}

	const ccl::Transform &ctfm = sce->camera->get_matrix();
	ccycles_diag("  camera at (%.2f %.2f %.2f) %dx%d fov=%.3f\n", ctfm.x.w, ctfm.y.w, ctfm.z.w,
	       sce->camera->get_full_width(), sce->camera->get_full_height(),
	       sce->camera->get_fov());

	/* A render where the depth pass is right but the combined pass is black
	 * means rays hit geometry and then found nothing to light it with, so
	 * report what could be lighting it. Lights are Geometry in 5.x, so there
	 * is no Scene::lights to walk. */
	size_t num_lights = 0;
	for (ccl::Geometry *geo : sce->geometry) {
		if (ccl::Light *lt = dynamic_cast<ccl::Light *>(geo)) {
			ccl::float3 st = lt->get_strength();
			ccycles_diag("    light %zu type=%d strength=(%f %f %f) enabled=%d "
			             "shadow=%d mis=%d max_bounces=%d shader=%p\n",
			             num_lights, (int)lt->get_light_type(), st.x, st.y, st.z,
			             (int)lt->get_is_enabled(), (int)lt->get_cast_shadow(),
			             (int)lt->get_use_mis(), lt->get_max_bounces(),
			             (void *)(lt->get_used_shaders().empty()
			                          ? nullptr
			                          : lt->get_used_shaders()[0]));
			num_lights++;
		}
	}
	ccycles_diag("  lights=%zu background_shader=%p background_vis=%u\n",
	             num_lights, (void *)sce->background->get_shader(),
	             sce->background->get_visibility());
	ccycles_diag("  integrator: max_bounces=%d diffuse=%d glossy=%d transmission=%d "
	             "volume=%d transparent=%d aa_samples=%d light_tree=%d\n",
	             sce->integrator->get_max_bounce(),
	             sce->integrator->get_max_diffuse_bounce(),
	             sce->integrator->get_max_glossy_bounce(),
	             sce->integrator->get_max_transmission_bounce(),
	             sce->integrator->get_max_volume_bounce(),
	             sce->integrator->get_transparent_max_bounce(),
	             sce->integrator->get_aa_samples(),
	             (int)sce->integrator->get_use_light_tree());
	ccycles_diag("  integrator switches: direct=%d indirect=%d diffuse=%d glossy=%d "
	             "transmission=%d emission=%d ao_factor=%f clamp_direct=%f "
	             "clamp_indirect=%f\n",
	             (int)sce->integrator->get_use_direct_light(),
	             (int)sce->integrator->get_use_indirect_light(),
	             (int)sce->integrator->get_use_diffuse(),
	             (int)sce->integrator->get_use_glossy(),
	             (int)sce->integrator->get_use_transmission(),
	             (int)sce->integrator->get_use_emission(),
	             sce->integrator->get_ao_factor(),
	             sce->integrator->get_sample_clamp_direct(),
	             sce->integrator->get_sample_clamp_indirect());
	ccycles_diag("  film: exposure=%f approx_shadow_catcher=%d display_pass=%d\n",
	             sce->film->get_exposure(),
	             (int)sce->film->get_use_approximate_shadow_catcher(),
	             (int)sce->film->get_display_pass());

	/* Correct geometry, correct lights, no failed connections and still a
	 * black image leaves the socket values themselves. Print what Cycles
	 * actually holds for every unlinked input, which is the thing the C# side
	 * believes it set. */
	for (size_t si = 0; si < sce->shaders.size(); si++) {
		ccl::Shader *sh = sce->shaders[si];
		/* Nothing post-compile here on purpose: emission_estimate, has_surface
		 * and friends are filled in by ShaderManager::device_update, which runs
		 * after session start, so reading them here reports zero for every
		 * shader and reads as "nothing emits". The tile callback reports them
		 * once compilation has actually happened. */
		ccycles_diag("  shader %zu '%s' graph=%p\n", si, sh->name.c_str(),
		             (void *)sh->graph.get());
		if (sh->graph == nullptr) {
			continue;
		}
		for (ccl::ShaderNode *nd : sh->graph->nodes) {
			ccycles_diag("    node '%s' type=%s\n", nd->name.c_str(),
			             nd->type->name.c_str());
			for (ccl::ShaderInput *in : nd->inputs) {
				if (in->link != nullptr) {
					ccycles_diag("      %s <- linked\n", in->socket_type.name.c_str());
					continue;
				}
				switch (in->socket_type.type) {
					case ccl::SocketType::FLOAT:
						ccycles_diag("      %s = %f\n", in->socket_type.name.c_str(),
						             nd->get_float(in->socket_type));
						break;
					case ccl::SocketType::COLOR:
					case ccl::SocketType::VECTOR:
					case ccl::SocketType::POINT:
					case ccl::SocketType::NORMAL: {
						ccl::float3 v = nd->get_float3(in->socket_type);
						ccycles_diag("      %s = (%f %f %f)\n",
						             in->socket_type.name.c_str(), v.x, v.y, v.z);
						break;
					}
					case ccl::SocketType::INT:
					case ccl::SocketType::ENUM:
						ccycles_diag("      %s = %d\n", in->socket_type.name.c_str(),
						             nd->get_int(in->socket_type));
						break;
					case ccl::SocketType::BOOLEAN:
						ccycles_diag("      %s = %d\n", in->socket_type.name.c_str(),
						             (int)nd->get_bool(in->socket_type));
						break;
					default:
						ccycles_diag("      %s = (type %d not printed)\n",
						             in->socket_type.name.c_str(), (int)in->socket_type.type);
						break;
				}
			}
		}
	}
	fflush(stdout);
}
