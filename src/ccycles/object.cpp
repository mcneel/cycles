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

unsigned int cycles_scene_add_object(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) 
	{
		auto ob = sce->create_node<ccl::Object>();

		// TODO: APIfy object matrix setting, for now hard-code to be closer to PoC plugin
		ob->set_tfm(ccl::transform_identity());

		logger.logit("Added object ", sce->objects.size() - 1, " to scene ", session_id);

		ob->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);

		return (unsigned int)(sce->objects.size() - 1);
	}

	return UINT_MAX;
}

void cycles_scene_object_set_mesh(ccl::Session* session_id, unsigned int object_id, unsigned int mesh_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) 
	{
		ccl::Object* ob = sce->objects[object_id];
		auto geometry = sce->geometry[mesh_id];
		ob->set_geometry(geometry);
		ob->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

void cycles_object_tag_update(ccl::Session* session_id, unsigned int object_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ob->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

unsigned int cycles_scene_object_get_mesh(ccl::Session* session_id, unsigned int object_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) 
	{
		ccl::Object* ob = sce->objects[object_id];

		//[NATHAN_LOOK] - This looks buggy to me - the iterator increased...

		auto cmeshit = sce->geometry.begin();
		auto cmeshend = sce->geometry.end();
		unsigned int i = 0;
		while (cmeshit != cmeshend) 
		{
			if ((*cmeshit) == ob->get_geometry()) 
			{
				return i;
			}
			++i;
		}
	}

	return UINT_MAX;
}

void cycles_scene_object_set_visibility(unsigned int client, ccl::Session* session_id, unsigned int object_id, unsigned int visibility)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ob->set_visibility(visibility);
		ob->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

void cycles_scene_object_set_shader(unsigned int client, ccl::Session* session_id, unsigned int object_id, unsigned int shader_id)
{
    // TODO: XXXX revisit shader assignment to objects
    // Needed for our approach to block instance shading
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ccl::Shader* sh = find_shader_in_scene(sce, shader_id);
		// TODO: XXXX shader stuff appears to have moved to ccl::Geometry... ob->shader = sh;
		sh->tag_update(sce);
		sh->tag_used(sce);
		ob->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

void cycles_scene_object_set_is_shadowcatcher(unsigned int client, ccl::Session* session_id, unsigned int object_id, bool is_shadowcatcher)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ob->set_is_shadow_catcher(is_shadowcatcher);
		ob->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

void cycles_scene_object_set_mesh_light_no_cast_shadow(unsigned int client, ccl::Session* session_id, unsigned int object_id, bool mesh_light_no_cast_shadow)
{
    // TODO: XXXX port this from old Cycles integration
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ob->mesh_light_no_cast_shadow = mesh_light_no_cast_shadow;
		ob->tag_update(sce);
		sce->light_manager->tag_update(sce);
	}
    */
}

void cycles_scene_object_set_is_block_instance(unsigned int client, ccl::Session* session_id, unsigned int object_id, bool is_block_instance)
{
    // TODO: XXXX port this from old Cycles integration
    // unless there is a better way to do this in new cycles
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ob->is_block_instance = is_block_instance;
		ob->tag_update(sce);
		sce->light_manager->tag_update(sce);
	}
    */
}

void cycles_scene_object_set_cutout(unsigned int client, ccl::Session* session_id, unsigned int object_id, bool cutout)
{
	/*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ob->use_cutout = cutout;
		ob->tag_update(sce);
	}*/
}

void cycles_scene_object_set_ignore_cutout(unsigned int client, ccl::Session* session_id, unsigned int object_id, bool ignore_cutout)
{
	/*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ob->ignore_cutout = ignore_cutout;
		ob->tag_update(sce);
	}*/
}

void _cycles_scene_object_set_transform(ccl::Session* session_id, unsigned int object_id, unsigned int transform_type,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ccl::Transform mat = ccl::make_transform(a, b, c, d, e, f, g, h, i, j, k, l);
		switch (transform_type) {
		case 0:
			ob->set_tfm(mat);
			break;
        // TODO: XXXX port OCS frame from old integration
        /*
		case 1:
			ob->ocs_frame = transform_inverse(mat);
			ob->use_ocs_frame = mat != ccl::transform_identity();
			break;
        */
		}
		ob->tag_update(sce);
	}
}

void cycles_scene_object_set_matrix(ccl::Session* session_id, unsigned int object_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	_cycles_scene_object_set_transform(session_id, object_id, 0,
		a, b, c, d,
		e, f, g, h,
		i, j, k, l);
}

void cycles_scene_object_set_ocs_frame(ccl::Session* session_id, unsigned int object_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	_cycles_scene_object_set_transform(session_id, object_id, 1,
		a, b, c, d,
		e, f, g, h,
		i, j, k, l);
}


void cycles_object_set_pass_id(ccl::Session* session_id, unsigned int object_id, int pass_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ob->set_pass_id(pass_id);
	}
}

void cycles_object_set_random_id(ccl::Session* session_id, unsigned int object_id, unsigned int random_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Object* ob = sce->objects[object_id];
		ob->set_random_id(random_id);
	}
}

void cycles_scene_clear_clipping_planes(ccl::Session* session_id)
{
    // TODO: XXXX port clipping planes (or reimplement)
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->clipping_planes.clear();
		sce->object_manager->need_clipping_plane_update = true;
	}
    */
}

unsigned int cycles_scene_add_clipping_plane(ccl::Session* session_id, float a, float b, float c, float d)
{
    // TODO: XXXX port clipping planes (or reimplement)
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::float4 cp = ccl::make_float4(a, b, c, d);
		sce->clipping_planes.push_back(cp);

		logger.logit("Added clipping plane ", sce->clipping_planes.size() - 1, " to scene ", session_id);

		sce->object_manager->need_clipping_plane_update = true;

		return (unsigned int)(sce->clipping_planes.size() - 1);
	}
    */

	return UINT_MAX;
}

void cycles_scene_discard_clipping_plane(ccl::Session* session_id, unsigned int cp_id)
{
	cycles_scene_set_clipping_plane(session_id, cp_id, FLT_MAX, FLT_MAX, FLT_MAX, FLT_MAX);
}

void cycles_scene_set_clipping_plane(ccl::Session* session_id, unsigned int cp_id, float a, float b, float c, float d)
{
    // TODO: XXXX port clipping planes (or reimplement)
    /*
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::float4 cp = ccl::make_float4(a, b, c, d);
		sce->clipping_planes[cp_id] = cp;

		logger.logit("Setting clipping plane ", sce->clipping_planes.size() - 1, " to scene ", session_id);

		sce->object_manager->need_clipping_plane_update = true;
	}
    */
}
