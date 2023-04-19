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

unsigned int cycles_create_light(unsigned int client_id, unsigned int scene_id, unsigned int light_shader_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		ccl::Light* l = new ccl::Light();
		l->set_angle(0.009180f); // use default value as in Blender UI (0.526deg)
		ccl::Shader* lightshader = find_shader_in_scene(sce, light_shader_id);
		l->set_shader(lightshader);
		sce->lights.push_back(l);
		logger.logit(client_id, "Adding light ", sce->lights.size() - 1, " to scene ", scene_id, " using light shader ", light_shader_id);
		return (unsigned int)(sce->lights.size() - 1);
	}

	return UINT_MAX;
}

/* type = 0: point, 1: sun, 2: background, 3: area, 4: spot, 5: triangle. */
void cycles_light_set_type(unsigned int client_id, unsigned int scene_id, unsigned int light_id, light_type type)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_light_type((ccl::LightType)type);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " type to ", (unsigned int)type);
	LIGHT_FIND_END()
}

void cycles_light_set_cast_shadow(unsigned int client_id, unsigned int scene_id, unsigned int light_id, unsigned int cast_shadow)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_cast_shadow(cast_shadow == 1);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " cast_shadow to ", cast_shadow == 1 ? "true" : "false");
	LIGHT_FIND_END()
}

void cycles_light_set_use_mis(unsigned int client_id, unsigned int scene_id, unsigned int light_id, unsigned int use_mis)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_use_mis(use_mis == 1);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " use_mis to ", use_mis == 1 ? "true" : "false");
	LIGHT_FIND_END()
}

void cycles_light_set_samples(unsigned int client_id, unsigned int scene_id, unsigned int light_id, unsigned int samples)
{
    /*
	LIGHT_FIND(scene_id, light_id)
		l->samples = samples;
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " samples to ", samples);
	LIGHT_FIND_END()
    */
}

void cycles_light_set_max_bounces(unsigned int client_id, unsigned int scene_id, unsigned int light_id, unsigned int max_bounces)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_max_bounces(max_bounces);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " max_bounces to ", max_bounces);
	LIGHT_FIND_END()
}

void cycles_light_set_map_resolution(unsigned int client_id, unsigned int scene_id, unsigned int light_id, unsigned int map_resolution)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_map_resolution(map_resolution);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " map_resolution to ", map_resolution);
	LIGHT_FIND_END()
}

void cycles_light_set_angle(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float angle)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_angle(angle);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " angle to ", angle);
	LIGHT_FIND_END()
}

void cycles_light_set_spot_angle(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float spot_angle)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_spot_angle(spot_angle);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " spot_angle to ", spot_angle);
	LIGHT_FIND_END()
}

void cycles_light_set_spot_smooth(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float spot_smooth)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_spot_smooth(spot_smooth);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " spot_smooth to ", spot_smooth);
	LIGHT_FIND_END()
}

void cycles_light_set_sizeu(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float sizeu)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_sizeu(sizeu);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " sizeu to ", sizeu);
	LIGHT_FIND_END()
}

void cycles_light_set_sizev(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float sizev)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_sizev(sizev);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " sizev to ", sizev);
	LIGHT_FIND_END()
}

void cycles_light_set_axisu(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float axisux, float axisuy, float axisuz)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_axisu(ccl::make_float3(axisux, axisuy, axisuz));
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " axisu to ", axisux, ",", axisuy, ",", axisuz);
	LIGHT_FIND_END()
}

void cycles_light_set_axisv(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float axisvx, float axisvy, float axisvz)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_axisv(ccl::make_float3(axisvx, axisvy, axisvz));
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " axisv to ", axisvx, ",", axisvy, ",", axisvz);
	LIGHT_FIND_END()
}

void cycles_light_set_size(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float size)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_size(size);
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " size to ", size);
	LIGHT_FIND_END()
}

void cycles_light_set_dir(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float dirx, float diry, float dirz)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_dir(ccl::make_float3(dirx, diry, dirz));
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " dir to ", dirx, ",", diry, ",", dirz);
	LIGHT_FIND_END()
}

void cycles_light_set_co(unsigned int client_id, unsigned int scene_id, unsigned int light_id, float cox, float coy, float coz)
{
	LIGHT_FIND(scene_id, light_id)
		l->set_co(ccl::make_float3(cox, coy, coz));
		logger.logit(client_id, "Setting light ", light_id, " of scene ", scene_id, " co to ", cox, ",", coy, ",", coz);
	LIGHT_FIND_END()
}

void cycles_light_tag_update(unsigned int client_id, unsigned int scene_id, unsigned int light_id)
{
	LIGHT_FIND(scene_id, light_id)
		l->tag_update(sce);
	LIGHT_FIND_END()
}
