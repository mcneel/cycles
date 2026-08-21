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

/* Cycles 5.2 reworked lights considerably:
 *
 *  - Light is abstract; PointLight, SpotLight, AreaLight, SunLight and
 *    BackgroundLight are the concrete classes, so the type must be known at
 *    construction. The Rhino API sets it after creation.
 *  - Lights are Geometry, created through Scene::create_node<T>() and living in
 *    scene->geometry rather than a separate scene->lights list.
 *  - Position and orientation come from an Object transform. The old co, dir,
 *    axisu and axisv sockets are gone.
 *  - set_use_glossy / set_use_transmission / set_use_camera are gone; per-light
 *    ray visibility is expressed through light linking now.
 *
 * CCyclesLight (internal_types.h) absorbs the difference: it buffers the
 * properties, then builds or updates the concrete light and its Object.
 */

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

	/* Shared properties. */
	light->set_cast_shadow(cast_shadow);
	light->set_use_mis(use_mis);
	light->set_max_bounces(max_bounces);

	/* Type specific properties. */
	if (ccl::PointLight *point = dynamic_cast<ccl::PointLight *>(light)) {
		point->set_radius(size);
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
	if (ccl::BackgroundLight *bg = dynamic_cast<ccl::BackgroundLight *>(light)) {
		bg->set_map_resolution(map_resolution);
	}

	/* Placement. Pre-5.2 the light carried co, dir and (for area lights) axisu
	 * and axisv directly; 5.2 takes all of it from the Object transform, with
	 * the light pointing down local -Z.
	 *
	 * The local axis is +Z, not -Z. SpotLight::copy_to_kernel reads the cone
	 * axis as -column2, which reads as though it should be -dir here, but a
	 * spot placed between the camera and a quad only lights the quad with the
	 * sign below - see the smoke test's SMOKE_SPOTZ sweep. Position and cone
	 * shape were checked the same way and are right. */
	/* A background light has no direction, so dir is still zero here and
	 * normalize() of it is NaN. That NaN used to go into the object transform
	 * below, and from there into the light tree, where it poisoned the
	 * importance maths for every light in the scene - nothing was ever
	 * sampled and the whole render came back black with a correct depth pass.
	 * A light with no direction gets the identity basis. */
	const bool have_dir = ccl::len_squared(dir) > 1e-12f;
	const ccl::float3 z = have_dir ? ccl::normalize(dir) : ccl::make_float3(0.0f, 0.0f, 1.0f);
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

void cycles_light_set_samples(ccl::Session *session_id, CCyclesLight *light, unsigned int samples)
{
	light->max_bounces = (int)samples;
	light->flush();
}

void cycles_light_set_max_bounces(ccl::Session *session_id, CCyclesLight *light, unsigned int max_bounces)
{
	light->max_bounces = (int)max_bounces;
	light->flush();
}

void cycles_light_set_map_resolution(ccl::Session *session_id, CCyclesLight *light, unsigned int map_resolution)
{
	light->map_resolution = (int)map_resolution;
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
