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

std::vector<CCScene*> scenes;

/* Find pointers for CCScene and ccl::Scene. Return false if either fails. */
bool scene_find(unsigned int scid, CCScene** csce, ccl::Scene** sce)
{
	if (0 <= (scid) && (scid) < scenes.size()) {
		*csce = scenes[scid];
		if((*csce)!=nullptr) *sce = (*csce)->scene;
		return *sce != nullptr && *csce!=nullptr;
	}
	return false;
}

void set_ccscene_null(unsigned int scene_id)
{
	scenes[scene_id] = nullptr;
}

void scene_clear_pointer(ccl::Scene* sce)
{
		for (CCScene* csc : scenes) {
			if (csc->scene == sce) {
				csc->scene = nullptr; /* don't delete here, since session deconstructor takes care of it. */
			}
		}
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

void cycles_scene_set_default_surface_shader(unsigned int scene_id, unsigned int shader_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		ccl::Shader* sh = find_shader_in_scene(sce, shader_id);
		sce->default_surface = sh;
		logger.logit("Scene ", scene_id, " set default surface shader ", shader_id);
	}
}

unsigned int cycles_scene_get_default_surface_shader(unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		return get_idx_for_shader_in_scene(sce, sce->default_surface);
	}

	return UINT_MAX;
}

void cycles_scene_reset(unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->reset();
	}
}

bool cycles_scene_try_lock(unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		return sce->mutex.try_lock();
	}
	return false;
}

void cycles_scene_lock(unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->mutex.lock();
	}
}

void cycles_scene_unlock(unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->mutex.unlock();
	}
}

