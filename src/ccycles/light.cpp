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

ccl::Light *cycles_create_light(ccl::Session* session_id, ccl::Shader *light_shader_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Light* l = new ccl::Light();
		l->set_angle(0.009180f); // use default value as in Blender UI (0.526deg)
		l->set_shader(light_shader_id);
		sce->lights.push_back(l);
		return l;
	}

	return nullptr;
}

/* type = 0: point, 1: sun, 2: background, 3: area, 4: spot, 5: triangle. */
void cycles_light_set_type(ccl::Session *session_id, ccl::Light *light_id, light_type type)
{
	LIGHT_FIND(session_id, light_id)
		l->set_light_type((ccl::LightType)type);
	LIGHT_FIND_END()
}

void cycles_light_set_cast_shadow(ccl::Session *session_id, ccl::Light *light_id, unsigned int cast_shadow)
{
	LIGHT_FIND(session_id, light_id)
		l->set_cast_shadow(cast_shadow == 1);
	LIGHT_FIND_END()
}

void cycles_light_set_use_mis(ccl::Session *session_id, ccl::Light *light_id, unsigned int use_mis)
{
	LIGHT_FIND(session_id, light_id)
		l->set_use_mis(use_mis == 1);
	LIGHT_FIND_END()
}

void cycles_light_set_samples(ccl::Session *session_id, ccl::Light *light_id, unsigned int samples)
{
	assert(false);
    /*
	LIGHT_FIND(session_id, light_id)
		l->samples = samples;
	LIGHT_FIND_END()
    */
}

void cycles_light_set_max_bounces(ccl::Session *session_id, ccl::Light *light_id, unsigned int max_bounces)
{
	LIGHT_FIND(session_id, light_id)
		l->set_max_bounces(max_bounces);
	LIGHT_FIND_END()
}

void cycles_light_set_map_resolution(ccl::Session *session_id, ccl::Light *light_id, unsigned int map_resolution)
{
	LIGHT_FIND(session_id, light_id)
		l->set_map_resolution(map_resolution);
	LIGHT_FIND_END()
}

void cycles_light_set_angle(ccl::Session *session_id, ccl::Light *light_id, float angle)
{
	LIGHT_FIND(session_id, light_id)
		l->set_angle(angle);
	LIGHT_FIND_END()
}

void cycles_light_set_spot_angle(ccl::Session *session_id, ccl::Light *light_id, float spot_angle)
{
	LIGHT_FIND(session_id, light_id)
		l->set_spot_angle(spot_angle);
	LIGHT_FIND_END()
}

void cycles_light_set_spot_smooth(ccl::Session *session_id, ccl::Light *light_id, float spot_smooth)
{
	LIGHT_FIND(session_id, light_id)
		l->set_spot_smooth(spot_smooth);
	LIGHT_FIND_END()
}

void cycles_light_set_sizeu(ccl::Session *session_id, ccl::Light *light_id, float sizeu)
{
	LIGHT_FIND(session_id, light_id)
		l->set_sizeu(sizeu);
	LIGHT_FIND_END()
}

void cycles_light_set_sizev(ccl::Session *session_id, ccl::Light *light_id, float sizev)
{
	LIGHT_FIND(session_id, light_id)
		l->set_sizev(sizev);
	LIGHT_FIND_END()
}

void cycles_light_set_axisu(ccl::Session *session_id, ccl::Light *light_id, float axisux, float axisuy, float axisuz)
{
	LIGHT_FIND(session_id, light_id)
		l->set_axisu(ccl::make_float3(axisux, axisuy, axisuz));
	LIGHT_FIND_END()
}

void cycles_light_set_axisv(ccl::Session *session_id, ccl::Light *light_id, float axisvx, float axisvy, float axisvz)
{
	LIGHT_FIND(session_id, light_id)
		l->set_axisv(ccl::make_float3(axisvx, axisvy, axisvz));
	LIGHT_FIND_END()
}

void cycles_light_set_size(ccl::Session *session_id, ccl::Light *light_id, float size)
{
	LIGHT_FIND(session_id, light_id)
		l->set_size(size);
	LIGHT_FIND_END()
}

void cycles_light_set_dir(ccl::Session *session_id, ccl::Light *light_id, float dirx, float diry, float dirz)
{
	LIGHT_FIND(session_id, light_id)
		l->set_dir(ccl::make_float3(dirx, diry, dirz));
	LIGHT_FIND_END()
}

void cycles_light_set_co(ccl::Session *session_id, ccl::Light *light_id, float cox, float coy, float coz)
{
	LIGHT_FIND(session_id, light_id)
		l->set_co(ccl::make_float3(cox, coy, coz));
	LIGHT_FIND_END()
}

void cycles_light_tag_update(ccl::Session *session_id, ccl::Light *light_id)
{
	LIGHT_FIND(session_id, light_id)
		l->tag_update(sce);
	LIGHT_FIND_END()
}
