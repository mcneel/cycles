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
	ccl::Light* l = new ccl::Light();
	l->set_angle(0.009180f); // use default value as in Blender UI (0.526deg)
	l->set_shader(light_shader_id);
	l->set_use_camera(false);
	l->set_use_glossy(false);
	l->set_use_transmission(false);
	session_id->scene->lights.push_back(l);
	return l;
}

/* type = 0: point, 1: sun, 2: background, 3: area, 4: spot, 5: triangle. */
void cycles_light_set_type(ccl::Session *session_id, ccl::Light *light_id, light_type type)
{
	light_id->set_light_type((ccl::LightType)type);
}

void cycles_light_set_cast_shadow(ccl::Session *session_id, ccl::Light *light_id, unsigned int cast_shadow)
{
	light_id->set_cast_shadow(cast_shadow == 1);
}

void cycles_light_set_use_mis(ccl::Session *session_id, ccl::Light *light_id, unsigned int use_mis)
{
	light_id->set_use_mis(use_mis == 1);
}

void cycles_light_set_samples(ccl::Session *session_id, ccl::Light *light_id, unsigned int samples)
{
	light_id->set_max_bounces((int)samples);
}

void cycles_light_set_max_bounces(ccl::Session *session_id, ccl::Light *light_id, unsigned int max_bounces)
{
	light_id->set_max_bounces(max_bounces);
}

void cycles_light_set_map_resolution(ccl::Session *session_id, ccl::Light *light_id, unsigned int map_resolution)
{
	light_id->set_map_resolution(map_resolution);
}

void cycles_light_set_angle(ccl::Session *session_id, ccl::Light *light_id, float angle)
{
	light_id->set_angle(angle);
}

void cycles_light_set_spot_angle(ccl::Session *session_id, ccl::Light *light_id, float spot_angle)
{
	light_id->set_spot_angle(spot_angle);
}

void cycles_light_set_spot_smooth(ccl::Session *session_id, ccl::Light *light_id, float spot_smooth)
{
	light_id->set_spot_smooth(spot_smooth);
}

void cycles_light_set_sizeu(ccl::Session *session_id, ccl::Light *light_id, float sizeu)
{
	light_id->set_sizeu(sizeu);
}

void cycles_light_set_sizev(ccl::Session *session_id, ccl::Light *light_id, float sizev)
{
	light_id->set_sizev(sizev);
}

void cycles_light_set_axisu(ccl::Session *session_id, ccl::Light *light_id, float axisux, float axisuy, float axisuz)
{
	light_id->set_axisu(ccl::make_float3(axisux, axisuy, axisuz));
}

void cycles_light_set_axisv(ccl::Session *session_id, ccl::Light *light_id, float axisvx, float axisvy, float axisvz)
{
	light_id->set_axisv(ccl::make_float3(axisvx, axisvy, axisvz));
}

void cycles_light_set_size(ccl::Session *session_id, ccl::Light *light_id, float size)
{
	light_id->set_size(size);
}

void cycles_light_set_dir(ccl::Session *session_id, ccl::Light *light_id, float dirx, float diry, float dirz)
{
	light_id->set_dir(ccl::make_float3(dirx, diry, dirz));
}

void cycles_light_set_co(ccl::Session *session_id, ccl::Light *light_id, float cox, float coy, float coz)
{
	light_id->set_co(ccl::make_float3(cox, coy, coz));
}

void cycles_light_tag_update(ccl::Session *session_id, ccl::Light *light_id)
{
	light_id->tag_update(session_id->scene);
}
