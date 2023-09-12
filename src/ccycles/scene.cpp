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
	(*sce) = sid->scene;
	return *sce != nullptr;
}


/* Find a ccl::Shader in a given ccl::Scene, based on shader_id
*/
ccl::Shader* find_shader_in_scene(ccl::Scene* sce, unsigned int shader_id)
{
	auto b = sce->shaders.cbegin();
	auto e = sce->shaders.cend();
	ccl::Shader* bg = nullptr;
	unsigned int idx = 0;
	while (b != e) {
		if (idx == shader_id) {
			bg = *b;
			break;
		}
		idx++;
		b++;
	}
	return bg;
}

unsigned int get_idx_for_shader_in_scene(ccl::Scene* sce, ccl::Shader* sh)
{
	auto b = sce->shaders.cbegin();
	auto e = sce->shaders.cend();
	unsigned int idx = 0;
	while (b != e) {
		if (*b == sh) {
			return idx;
		}
		idx++;
		b++;
	}
	return -1;

}

/* implement CCScene methods*/

void CCScene::builtin_image_info(const std::string& builtin_name, void* builtin_data, ccl::ImageMetaData& imdata) //bool& is_float, int& width, int& height, int& depth, int& channels)
{
	CCImage* img = static_cast<CCImage*>(builtin_data);
	imdata.width = img->width;
	imdata.height = img->height;
	imdata.depth = img->depth;
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

unsigned int cycles_scene_create(unsigned int scene_params_id, unsigned int session_id)
{
	return UINT_MAX;
}

void cycles_scene_set_default_surface_shader(ccl::Session *session_id, ccl::Shader *shader_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->default_surface = shader_id;
		logger.logit("Scene ", session_id, " set default surface shader ", shader_id);
	}
}

ccl::Shader *cycles_scene_get_default_surface_shader(ccl::Session *session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		return sce->default_surface;
	}

	return nullptr;
}

void cycles_scene_reset(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->reset();
	}
}

bool cycles_scene_try_lock(ccl::Session* session)
{
	return session->scene->mutex.try_lock();
}

void cycles_scene_lock(ccl::Session* session)
{
	session->scene->mutex.lock();
}

void cycles_scene_unlock(ccl::Session* session)
{
	session->scene->mutex.unlock();
}

