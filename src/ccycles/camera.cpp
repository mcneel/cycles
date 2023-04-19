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

void cycles_camera_set_size(unsigned int client_id, unsigned int scene_id, unsigned int width, unsigned int height)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
        // TODO: find out if need to set full_width and full_height
        sce->camera->set_screen_size(width, height);
		// TODO: APIfy need_[device_]update
		sce->camera->need_flags_update = true;
		sce->camera->need_device_update = true;
	}
}

/*
unsigned int cycles_camera_get_width(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		return (unsigned int)sce->camera->width;
	}

	return 0;
}

unsigned int cycles_camera_get_height(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		return (unsigned int)sce->camera->height;
	}

	return 0;
}
 */

void cycles_camera_set_type(unsigned int client_id, unsigned int scene_id, camera_type type)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->camera->set_camera_type((ccl::CameraType)type);
	}
}

void cycles_camera_set_panorama_type(unsigned int client_id, unsigned int scene_id, panorama_type type)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->camera->set_panorama_type((ccl::PanoramaType)type);
	}
}

void cycles_camera_set_matrix(unsigned int client_id, unsigned int scene_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		ccl::Transform mat = ccl::make_transform(a, b, c, d, e, f, g, h, i, j, k, l);
		logger.logit(client_id, "Setting camera matrix in scene ", scene_id, " to\n",
			"\t[", a, ",", b, ",", c, ",", d, "\n",
			"\t ", e, ",", f, ",", g, ",", h, "\n",
			"\t ", i, ",", j, ",", k, ",", l, "\n"
			);
		sce->camera->set_matrix(mat);
	}
}

void cycles_camera_compute_auto_viewplane(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Computing auto viewplane for scene ", scene_id); 
		sce->camera->compute_auto_viewplane();
	}
}

void cycles_camera_set_viewplane(unsigned int client_id, unsigned int scene_id, float left, float right, float top, float bottom)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Set viewplane for scene ", scene_id, " to ", left, ":", right, ":", top, ":", bottom); 
		sce->camera->viewplane.left = left;
		sce->camera->viewplane.right = right;
		sce->camera->viewplane.top = top;
		sce->camera->viewplane.bottom = bottom;
	}

}

void cycles_camera_update(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Updating camera for scene ", scene_id); 
		sce->camera->need_flags_update = true;
        sce->camera->need_device_update = true;
		sce->camera->update(sce);
	}
}

void cycles_camera_set_fov(unsigned int client_id, unsigned int scene_id, float fov)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera fov to ", fov);
		sce->camera->set_fov(fov);
	}
}

void cycles_camera_set_sensor_width(unsigned int client_id, unsigned int scene_id, float sensor_width)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera sensor_width to ", sensor_width);
		sce->camera->set_sensorwidth(sensor_width);
	}
}

void cycles_camera_set_sensor_height(unsigned int client_id, unsigned int scene_id, float sensor_height)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera sensor_height to ", sensor_height);
		sce->camera->set_sensorheight(sensor_height);
	}
}

void cycles_camera_set_nearclip(unsigned int client_id, unsigned int scene_id, float nearclip)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera nearclip to ", nearclip);
		sce->camera->set_nearclip(nearclip);
	}
}

void cycles_camera_set_farclip(unsigned int client_id, unsigned int scene_id, float farclip)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera farclip to ", farclip);
		sce->camera->set_farclip(farclip);
	}
}

void cycles_camera_set_aperturesize(unsigned int client_id, unsigned int scene_id, float aperturesize)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera aperturesize to ", aperturesize);
		sce->camera->set_aperturesize(aperturesize);
	}
}

void cycles_camera_set_aperture_ratio(unsigned int client_id, unsigned int scene_id, float aperture_ratio)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera aperture_ratio to ", aperture_ratio);
		sce->camera->set_aperture_ratio(aperture_ratio);
	}
}

void cycles_camera_set_blades(unsigned int client_id, unsigned int scene_id, unsigned int blades)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera blades to ", blades);
		sce->camera->set_blades(blades);
	}
}

void cycles_camera_set_bladesrotation(unsigned int client_id, unsigned int scene_id, float bladesrotation)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera bladesrotation to ", bladesrotation);
		sce->camera->set_bladesrotation(bladesrotation);
	}
}

void cycles_camera_set_focaldistance(unsigned int client_id, unsigned int scene_id, float focaldistance)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera focaldistance to ", focaldistance);
		sce->camera->set_focaldistance(focaldistance);
	}
}

void cycles_camera_set_shuttertime(unsigned int client_id, unsigned int scene_id, float shuttertime)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera shuttertime to ", shuttertime);
		sce->camera->set_shuttertime(shuttertime);
	}
}

void cycles_camera_set_fisheye_fov(unsigned int client_id, unsigned int scene_id, float fisheye_fov)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera fisheye_fov to ", fisheye_fov);
		sce->camera->set_fisheye_fov(fisheye_fov);
	}
}

void cycles_camera_set_fisheye_lens(unsigned int client_id, unsigned int scene_id, float fisheye_lens)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		logger.logit(client_id, "Setting camera fisheye_lens to ", fisheye_lens);
		sce->camera->set_fisheye_lens(fisheye_lens);
	}
}
