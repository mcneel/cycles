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

ccl::Object* cycles_scene_add_object(ccl::Session* session_id)
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

		return ob;
	}

	return nullptr;
}

void cycles_scene_object_set_geometry(ccl::Session* session_id, ccl::Object* object, ccl::Geometry* geometry)
{
	ASSERT(object);
	ASSERT(geometry);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) 
	{
		object->set_geometry(geometry);
		object->tag_update(sce);

		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

void cycles_object_tag_update(ccl::Session* session_id, ccl::Object* object)
{
	ASSERT(object);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) 
	{
		object->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

void cycles_scene_object_set_visibility(ccl::Session* session_id, ccl::Object* object, unsigned int visibility)
{
	ASSERT(object);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) 
	{
		object->set_visibility(visibility);
		object->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

void cycles_scene_object_set_shader(ccl::Session *session_id,
									ccl::Object *object,
									ccl::Shader *shader_id)
{
	ASSERT(object);

	ccl::Scene* sce = session_id->scene;
	ccl::Geometry* geometry = object->get_geometry();
	object->set_shader(shader_id);

	bool already_exists = false;

	int shid = 0;
	ccl::array<ccl::Node *> used_shaders = geometry->get_used_shaders();
	for (ccl::Node* node : used_shaders)
	{
		if (node == shader_id) {
			already_exists = true;
			break;
		}
		shid++;
	}

	if (!already_exists)
	{
		used_shaders.push_back_slow(shader_id);
		geometry->set_used_shaders(used_shaders);

		shader_id->tag_update(sce);
		shader_id->tag_used(sce);
		object->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
		shid = used_shaders.size() - 1;
	}

	if (geometry->is_mesh())
	{
		ccl::Mesh *mesh = static_cast<ccl::Mesh *>(geometry);
		auto shids = mesh->get_shader();
		for (int i = 0; i < shids.size(); i++)
		{
			shids[i] = shid;
		}
		mesh->set_shader(shids);
	}

}

void cycles_scene_object_set_is_shadowcatcher(ccl::Session* session_id, ccl::Object* object, bool is_shadowcatcher)
{
	ASSERT(object);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		object->set_is_shadow_catcher(is_shadowcatcher);
		object->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

void cycles_scene_object_set_mesh_light_no_cast_shadow(ccl::Session* session_id, ccl::Object* object, bool mesh_light_no_cast_shadow)
{
	ASSERT(object);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		object->set_mesh_light_no_cast_shadow(mesh_light_no_cast_shadow);
		object->tag_update(sce);
		sce->light_manager->tag_update(sce, ccl::LightManager::UPDATE_ALL);
	}
}

static void _cycles_scene_object_set_transform(ccl::Session* session_id, ccl::Object* object, unsigned int transform_type,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Transform mat = ccl::make_transform(a, b, c, d, e, f, g, h, i, j, k, l);
		switch (transform_type) {
		case 0:
			object->set_tfm(mat);
			break;
		case 1:
			object->set_ocs_frame(transform_inverse(mat));
			object->set_use_ocs_frame(mat != ccl::transform_identity());
			break;
		}
		object->tag_update(sce);
	}
}

void cycles_scene_object_set_matrix(ccl::Session* session_id, ccl::Object* object,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	ASSERT(object);

	_cycles_scene_object_set_transform(session_id, object, 0,
		a, b, c, d,
		e, f, g, h,
		i, j, k, l);
}

void cycles_scene_object_set_ocs_frame(ccl::Session* session_id, ccl::Object* object,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	ASSERT(object);

	_cycles_scene_object_set_transform(session_id, object, 1,
		a, b, c, d,
		e, f, g, h,
		i, j, k, l);
}


void cycles_object_set_pass_id(ccl::Session* session_id, ccl::Object* object, int pass_id)
{
	ASSERT(object);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) 
	{
		object->set_pass_id(pass_id);
	}
}

void cycles_object_set_random_id(ccl::Session* session_id, ccl::Object* object, unsigned int random_id)
{
	ASSERT(object);

	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		object->set_random_id(random_id);
	}
}

void cycles_scene_clear_clipping_planes(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->clipping_planes.clear();
		sce->object_manager->need_clipping_plane_update = true;
	}
}

unsigned int cycles_scene_add_clipping_plane(ccl::Session* session_id, float a, float b, float c, float d)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::float4 cp = ccl::make_float4(a, b, c, d);
		sce->clipping_planes.push_back(cp);

		logger.logit("Added clipping plane ", sce->clipping_planes.size() - 1, " to scene ", session_id);

		sce->object_manager->need_clipping_plane_update = true;

		return (unsigned int)(sce->clipping_planes.size() - 1);
	}

	return UINT_MAX;
}

void cycles_scene_discard_clipping_plane(ccl::Session* session_id, unsigned int cp_id)
{
	cycles_scene_set_clipping_plane(session_id, cp_id, FLT_MAX, FLT_MAX, FLT_MAX, FLT_MAX);
}

void cycles_scene_set_clipping_plane(ccl::Session* session_id, unsigned int cp_id, float a, float b, float c, float d)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::float4 cp = ccl::make_float4(a, b, c, d);
		sce->clipping_planes[cp_id] = cp;

		logger.logit("Setting clipping plane ", sce->clipping_planes.size() - 1, " to scene ", session_id);

		sce->object_manager->need_clipping_plane_update = true;
	}
}
