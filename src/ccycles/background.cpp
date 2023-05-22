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

/* Set shader_id as default background shader for session_id.
 * Note that shader_id is the ID for the shader specific to this scene.
 * 
 * The correct ID can be found with cycles_scene_shader_id. The ID is also
 * returned from cycles_scene_add_shader.
 */
void cycles_scene_set_background_shader(ccl::Session* session_id, unsigned int shader_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Shader* bg = find_shader_in_scene(sce, shader_id);
		if (bg != nullptr) {
			sce->default_background = bg;
			sce->background->set_shader(bg);
			sce->background->set_use_shader(true);
			sce->background->tag_update(sce);
			logger.logit("Scene ", session_id, " set background shader ", shader_id);
		}
	}
}

ccl::Shader* cycles_scene_get_background_shader(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		return sce->default_background;
	}
	return nullptr;
}

void cycles_scene_set_background_transparent(ccl::Session* session_id, unsigned int transparent)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->background->set_transparent(transparent == 1);
		sce->background->tag_update(sce);
		logger.logit("Scene ", session_id, " set background transparent", transparent);
	}
}

void cycles_scene_set_background_visibility(ccl::Session* session_id, unsigned int path_ray_flag)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->background->set_visibility((ccl::PathRayFlag)path_ray_flag);
		sce->background->tag_update(sce);
		logger.logit("Scene ", session_id, " set background path ray visibility ", path_ray_flag);
	}
}
