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

#ifdef __cplusplus
extern "C" {
#endif

CCL_CAPI void CDECL cycles_scene_set_background_transparent(ccl::Session* session_id, bool transparent)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->background->set_transparent(transparent);
		sce->background->tag_update(sce);
		logger.logit("Scene ", session_id, " set background transparent", transparent);
	}
}

CCL_CAPI void CDECL cycles_scene_set_background_visibility(ccl::Session* session_id, unsigned int path_ray_flag)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->background->set_visibility((ccl::PathRayFlag)path_ray_flag);
		sce->background->tag_update(sce);
		logger.logit("Scene ", session_id, " set background path ray visibility ", path_ray_flag);
	}
}

#ifdef __cplusplus
}
#endif

