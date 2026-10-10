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

/* 5.2 lights: the type picks the concrete class, lights are Geometry in
 * scene->geometry, placement is the Object transform (no co/dir/axisu/axisv), and
 * ray visibility is the Object's (set_use_glossy/transmission/camera are gone).
 * CCyclesLight (internal_types.h) buffers the properties and builds the light. */

void CCyclesLight::flush()
{
	if (session == nullptr) {
		return;
	}

	ccl::Scene *scene = session->scene.get();

	if (light == nullptr) {
		switch (type) {
			case ccl::LIGHT_POINT:
				light = scene->create_node<ccl::PointLight>();
				break;
			case ccl::LIGHT_SPOT:
				light = scene->create_node<ccl::SpotLight>();
				break;
			case ccl::LIGHT_AREA:
				light = scene->create_node<ccl::AreaLight>();
				break;
			case ccl::LIGHT_SUN:
				light = scene->create_node<ccl::SunLight>();
				break;
			case ccl::LIGHT_BACKGROUND:
				light = scene->create_node<ccl::BackgroundLight>();
				break;
			default:
				light = scene->create_node<ccl::PointLight>();
				break;
		}

		object = scene->create_node<ccl::Object>();
		object->set_geometry(light);
	}

	/* Shared properties. is_shadow_catcher is now an Object flag, default false; without
	 * it the light is skipped on the shadow-catcher pass (SHADER_EXCLUDE_SHADOW_CATCHER)
	 * and every catcher shadow comes out lighter. Blender's sync sets it too. */
	if (object != nullptr) {
		object->set_is_shadow_catcher(true);
	}
	light->set_cast_shadow(cast_shadow);
	light->set_use_mis(use_mis);
	light->set_max_bounces(max_bounces);

	/* Rhino lights are invisible to the camera, now via the Object's ray visibility;
	 * a visible rectangular light blows out everything behind it. The background
	 * light must stay camera-visible or the environment renders black. */
	if (object != nullptr) {
		const bool is_background = (dynamic_cast<ccl::BackgroundLight *>(light) != nullptr);
		object->set_visibility(is_background
		                           ? ccl::PATH_RAY_VISIBILITY_ALL
		                           : (ccl::PATH_RAY_VISIBILITY_ALL &
		                              ~ccl::PATH_RAY_VISIBILITY_CAMERA));
	}

	/* Cycles 4.0 made area lights 4/pi brighter (1/(4 * area) -> 1/(pi * area),
	 * upstream a21af93e6). Scale back as Blender's versioning does for pre-4.0
	 * files; other light types are unchanged. */
	const bool is_area = (dynamic_cast<ccl::AreaLight *>(light) != nullptr);
	light->set_strength(ccl::make_float3(is_area ? M_PI_4_F : 1.0f));

	/* Type specific properties. */
	if (ccl::PointLight *point = dynamic_cast<ccl::PointLight *>(light)) {
		point->set_radius(size);
		/* Oriented disk, as Blender's "Soft Falloff" (on for every light), not 4.0's
		 * hard-edged sphere; differs only near a light with a radius. Spots inherit it. */
		point->set_is_sphere(false);
	}
	if (ccl::SpotLight *spot = dynamic_cast<ccl::SpotLight *>(light)) {
		spot->set_angle(spot_angle);
		spot->set_smooth(spot_smooth);
	}
	if (ccl::AreaLight *area = dynamic_cast<ccl::AreaLight *>(light)) {
		area->set_sizeu(sizeu);
		area->set_sizev(sizev);
	}
	if (ccl::SunLight *sun = dynamic_cast<ccl::SunLight *>(light)) {
		sun->set_angle(angle);
	}

	/* Placement is the Object transform, the light pointing down local -Z. A zero
	 * dir (background light) would normalize to NaN and poison the light tree for
	 * every light, so it gets the identity basis. */
	const bool have_dir = ccl::len_squared(dir) > 1e-12f;
	ccl::float3 z = have_dir ? ccl::normalize(dir) : ccl::make_float3(0.0f, 0.0f, 1.0f);

	/* Area, sun and spot copy_to_kernel read the emission direction (spot: cone
	 * axis) as -column2, so column2 holds -dir. With +dir they aimed away from the
	 * scene (distant lights: RH-98419). */
	if ((type == ccl::LIGHT_AREA || type == ccl::LIGHT_SUN || type == ccl::LIGHT_SPOT) &&
	    have_dir)
	{
		z = -z;
	}
	ccl::float3 x = axisu;
	ccl::float3 y = axisv;

	if (ccl::len_squared(x) < 1e-12f || ccl::len_squared(y) < 1e-12f) {
		/* Non-area lights leave axisu/axisv unset; derive any stable basis. */
		const ccl::float3 up = (fabsf(z.z) < 0.9f) ? ccl::make_float3(0.0f, 0.0f, 1.0f) :
		                                             ccl::make_float3(1.0f, 0.0f, 0.0f);
		x = ccl::normalize(ccl::cross(up, z));
		y = ccl::cross(z, x);
	}
	else {
		x = ccl::normalize(x);
		y = ccl::normalize(y);
	}

	const ccl::Transform tfm = ccl::make_transform(x.x, y.x, z.x, co.x,
	                                               x.y, y.y, z.y, co.y,
	                                               x.z, y.z, z.z, co.z);
	object->set_tfm(tfm);

	if (shader != nullptr) {
		ccl::array<ccl::Node *> used_shaders;
		used_shaders.push_back_slow(shader);
		light->set_used_shaders(used_shaders);
	}
}

CCyclesLight *cycles_create_light(ccl::Session *session_id, ccl::Shader *light_shader_id)
{
	CCyclesLight *handle = new CCyclesLight();
	handle->session = session_id;
	handle->shader = light_shader_id;
	return handle;
}

/* type = 0: point, 1: sun, 2: background, 3: area, 4: spot, 5: triangle. */
void cycles_light_set_type(ccl::Session *session_id, CCyclesLight *light, light_type type)
{
	light->type = (ccl::LightType)type;
	light->type_set = true;
	light->flush();
}

void cycles_light_set_cast_shadow(ccl::Session *session_id, CCyclesLight *light, unsigned int cast_shadow)
{
	light->cast_shadow = (cast_shadow == 1);
	light->flush();
}

void cycles_light_set_use_mis(ccl::Session *session_id, CCyclesLight *light, unsigned int use_mis)
{
	light->use_mis = (use_mis == 1);
	light->flush();
}

void cycles_light_set_max_bounces(ccl::Session *session_id, CCyclesLight *light, unsigned int max_bounces)
{
	light->max_bounces = (int)max_bounces;
	light->flush();
}

void cycles_light_set_angle(ccl::Session *session_id, CCyclesLight *light, float angle)
{
	light->angle = angle;
	light->flush();
}

void cycles_light_set_spot_angle(ccl::Session *session_id, CCyclesLight *light, float spot_angle)
{
	light->spot_angle = spot_angle;
	light->flush();
}

void cycles_light_set_spot_smooth(ccl::Session *session_id, CCyclesLight *light, float spot_smooth)
{
	light->spot_smooth = spot_smooth;
	light->flush();
}

void cycles_light_set_sizeu(ccl::Session *session_id, CCyclesLight *light, float sizeu)
{
	light->sizeu = sizeu;
	light->flush();
}

void cycles_light_set_sizev(ccl::Session *session_id, CCyclesLight *light, float sizev)
{
	light->sizev = sizev;
	light->flush();
}

void cycles_light_set_axisu(ccl::Session *session_id, CCyclesLight *light, float axisux, float axisuy, float axisuz)
{
	light->axisu = ccl::make_float3(axisux, axisuy, axisuz);
	light->flush();
}

void cycles_light_set_axisv(ccl::Session *session_id, CCyclesLight *light, float axisvx, float axisvy, float axisvz)
{
	light->axisv = ccl::make_float3(axisvx, axisvy, axisvz);
	light->flush();
}

void cycles_light_set_size(ccl::Session *session_id, CCyclesLight *light, float size)
{
	light->size = size;
	light->flush();
}

void cycles_light_set_dir(ccl::Session *session_id, CCyclesLight *light, float dirx, float diry, float dirz)
{
	light->dir = ccl::make_float3(dirx, diry, dirz);
	light->flush();
}

void cycles_light_set_co(ccl::Session *session_id, CCyclesLight *light, float cox, float coy, float coz)
{
	light->co = ccl::make_float3(cox, coy, coz);
	light->flush();
}

void cycles_light_tag_update(ccl::Session *session_id, CCyclesLight *light)
{
	if (light->light != nullptr) {
		light->light->tag_update(session_id->scene.get());
	}
}
