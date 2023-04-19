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

void _cleanup_scenes()
{
	// clear out scene params vector
	scene_params.clear();
	auto scend = scenes.end();
	auto scit = scenes.begin();

	for (CCScene* sce : scenes) {
		if (sce) {
			if (sce->scene) {
				delete sce->scene;
			}
			delete sce;
		}
	}

	scenes.clear();
}

unsigned int cycles_scene_create(unsigned int client_id, unsigned int scene_params_id, unsigned int session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {

		ccl::SceneParams* params = nullptr;
		bool found_params{ false };

		if (scene_params_id < scene_params.size()) {
			params = scene_params[scene_params_id];
			found_params = true;
		}

		if (found_params && params!=nullptr) {
			int cscid{ -1 };
			if (scenes.size() > 0) {
				int hid{ 0 };

				// look for first CCScene that has no ccl::Scene set.
				// if we find one, use it. Otherwise we'll just create a new one.
				for (CCScene* sce : scenes) {
					if (sce == nullptr) {
						// we found a cleaned out case, create new CCScene and use it
						CCScene* scene = new CCScene();
						scenes[hid] = scene;
						cscid = hid;
						break;
					}
					++hid;
				}
			}
			if (cscid == -1) {
				CCScene* scene = new CCScene();
				scenes.push_back(scene);
				cscid = (unsigned int)(scenes.size() - 1);
			}

			_init_shaders(client_id, cscid);

			scenes[cscid]->scene = new ccl::Scene(*params, session->device);
			scenes[cscid]->params_id = scene_params_id;
            // TODO: XXXX revisit image handling. preference: use OIIO
            /*
			scenes[cscid]->scene->image_manager->builtin_image_info_cb = function_bind(&CCScene::builtin_image_info, scenes[cscid], std::placeholders::_1, std::placeholders::_2, std::placeholders::_3);
			scenes[cscid]->scene->image_manager->builtin_image_pixels_cb = function_bind(&CCScene::builtin_image_pixels, scenes[cscid], std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4, std::placeholders::_5, std::placeholders::_6, std::placeholders::_7);
			scenes[cscid]->scene->image_manager->builtin_image_float_pixels_cb = function_bind(&CCScene::builtin_image_float_pixels, scenes[cscid], std::placeholders::_1, std::placeholders::_2, std::placeholders::_3, std::placeholders::_4, std::placeholders::_5, std::placeholders::_6, std::placeholders::_7);
             */
			
			// TODO: Is this the right spot?
            // TODO: XXXX port rhino procedurals from old integration
            /*
			scenes[cscid]->scene->shader_manager->set_rhino_perlin_noise_table(ccycles_rhino_perlin_noise_table);
			scenes[cscid]->scene->shader_manager->set_rhino_impulse_noise_table(ccycles_rhino_impulse_noise_table);
			scenes[cscid]->scene->shader_manager->set_rhino_vc_noise_table(ccycles_rhino_vc_noise_table);
			scenes[cscid]->scene->shader_manager->set_rhino_aaltonen_noise_table(ccycles_rhino_aaltonen_noise_table);
            */

			logger.logit(client_id, "Created scene ", cscid, " with scene_params ", scene_params_id, " and device ", session->device->info.id);
			return cscid;
		}
		else {
			return UINT_MAX;
		}
	}
	return UINT_MAX;
}

void cycles_scene_set_default_surface_shader(unsigned int client_id, unsigned int scene_id, unsigned int shader_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		ccl::Shader* sh = find_shader_in_scene(sce, shader_id);
		sce->default_surface = sh;
		logger.logit(client_id, "Scene ", scene_id, " set default surface shader ", shader_id);
	}
}

unsigned int cycles_scene_get_default_surface_shader(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		return get_idx_for_shader_in_scene(sce, sce->default_surface);
	}

	return UINT_MAX;
}

void cycles_scene_reset(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->reset();
	}
}

bool cycles_scene_try_lock(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		return sce->mutex.try_lock();
	}
	return false;
}

void cycles_scene_lock(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->mutex.lock();
	}
}

void cycles_scene_unlock(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->mutex.unlock();
	}
}

