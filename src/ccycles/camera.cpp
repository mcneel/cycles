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

void cycles_camera_set_size(ccl::Session* session_id, unsigned int width, unsigned int height)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
        // TODO: find out if need to set full_width and full_height
        sce->camera->set_screen_size(width, height);
		// TODO: APIfy need_[device_]update
		sce->camera->need_flags_update = true;
		sce->camera->need_device_update = true;
	}
}

unsigned int cycles_camera_get_width(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		return (unsigned int)sce->camera->get_full_width();
	}

	return 0;
}

unsigned int cycles_camera_get_height(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		return (unsigned int)sce->camera->get_full_height();
	}

	return 0;
}

void cycles_camera_set_type(ccl::Session* session_id, camera_type type)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_camera_type((ccl::CameraType)type);
	}
}

void cycles_camera_set_panorama_type(ccl::Session* session_id, panorama_type type)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_panorama_type((ccl::PanoramaType)type);
	}
}

void cycles_camera_set_matrix(ccl::Session* session_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		ccl::Transform mat = ccl::make_transform(a, b, c, d, e, f, g, h, i, j, k, l);
		sce->camera->set_matrix(mat);
	}
}

void cycles_camera_compute_auto_viewplane(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->compute_auto_viewplane();
	}
}

void cycles_camera_set_viewplane(ccl::Session* session_id, float left, float right, float top, float bottom)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->viewplane.left = left;
		sce->camera->viewplane.right = right;
		sce->camera->viewplane.top = top;
		sce->camera->viewplane.bottom = bottom;
	}

}

void cycles_camera_update(ccl::Session* session_id)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->update(sce);

		// 2023-05-22 David E. (RH-74901)
		// NOTE: This is a hack. I'm calling tag_modified() here
		// because otherwise the updated camera will get ignored in 
		// Scene::update_kernel_features() due to need_update() returning
		// false. It seems like camera->update(...) should not get called
		// while we're rendering with CyclesX. Instead, we should be
		// doing what the standalone app does:
		//   camera->set_matrix(matrix);
		//   camera->need_flags_update = true;
		//   camera->need_device_update = true;
		// The code above will set only what's needed and correctly tag
		// the camera that it's been modified.
		// Please see YouTrack item RH-74901.
		sce->camera->tag_modified();
	}
}

void cycles_camera_set_fov(ccl::Session* session_id, float fov)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_fov(fov);
	}
}

void cycles_camera_set_sensor_width(ccl::Session* session_id, float sensor_width)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_sensorwidth(sensor_width);
	}
}

void cycles_camera_set_sensor_height(ccl::Session* session_id, float sensor_height)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_sensorheight(sensor_height);
	}
}

void cycles_camera_set_nearclip(ccl::Session* session_id, float nearclip)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_nearclip(nearclip);
	}
}

void cycles_camera_set_farclip(ccl::Session* session_id, float farclip)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_farclip(farclip);
	}
}

void cycles_camera_set_aperturesize(ccl::Session* session_id, float aperturesize)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_aperturesize(aperturesize);
	}
}

void cycles_camera_set_aperture_ratio(ccl::Session* session_id, float aperture_ratio)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_aperture_ratio(aperture_ratio);
	}
}

void cycles_camera_set_blades(ccl::Session* session_id, unsigned int blades)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_blades(blades);
	}
}

void cycles_camera_set_bladesrotation(ccl::Session* session_id, float bladesrotation)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_bladesrotation(bladesrotation);
	}
}

void cycles_camera_set_focaldistance(ccl::Session* session_id, float focaldistance)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_focaldistance(focaldistance);
	}
}

void cycles_camera_set_shuttertime(ccl::Session* session_id, float shuttertime)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_shuttertime(shuttertime);
	}
}

void cycles_camera_set_fisheye_fov(ccl::Session* session_id, float fisheye_fov)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_fisheye_fov(fisheye_fov);
	}
}

void cycles_camera_set_fisheye_lens(ccl::Session* session_id, float fisheye_lens)
{
	ccl::Scene* sce = nullptr;
	if(scene_find(session_id, &sce)) {
		sce->camera->set_fisheye_lens(fisheye_lens);
	}
}
