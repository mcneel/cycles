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
		printf("stats: no scene\n");
		return;
	}

	printf("stats: geometry=%zu objects=%zu shaders=%zu\n",
	       sce->geometry.size(), sce->objects.size(), sce->shaders.size());

	for (size_t i = 0; i < sce->objects.size(); i++) {
		ccl::Object *ob = sce->objects[i];
		ccl::Geometry *geo = ob->get_geometry();
		printf("  object %zu geometry=%p vis=%u\n", i, (void *)geo, ob->get_visibility());
		if (ccl::Mesh *mesh = dynamic_cast<ccl::Mesh *>(geo)) {
			printf("    mesh verts=%d tris=%d used_shaders=%zu\n",
			       (int)mesh->num_verts(), (int)mesh->num_triangles(),
			       mesh->get_used_shaders().size());
		}
		else {
			printf("    not a mesh (light?)\n");
		}
	}

	const ccl::Transform &ctfm = sce->camera->get_matrix();
	printf("  camera at (%.2f %.2f %.2f) %dx%d fov=%.3f\n", ctfm.x.w, ctfm.y.w, ctfm.z.w,
	       sce->camera->get_full_width(), sce->camera->get_full_height(),
	       sce->camera->get_fov());
	fflush(stdout);
}
