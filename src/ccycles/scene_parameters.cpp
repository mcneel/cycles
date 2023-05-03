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

#define SCENE_PARAM_BOOL(scene_params_id, varname) \
	PARAM_BOOL(scene_params, scene_params_id, varname)

#define SCENE_PARAM_CAST(scene_params_id, typecast, varname) \
	PARAM_CAST(scene_params, scene_params_id, typecast, varname)

std::vector<ccl::SceneParams*> scene_params;

/* Create scene parameters, to be used when creating a new scene. */
unsigned int cycles_scene_params_create(
	unsigned int shadingsystem, unsigned int bvh_type, 
	unsigned int use_bvh_spatial_split, 
	int bvh_layout, unsigned int persistent_data)
{
	ccl::SceneParams* params = new ccl::SceneParams();

	params->shadingsystem = (ccl::ShadingSystem)shadingsystem;
	params->bvh_type = (ccl::BVHType)bvh_type;
	params->use_bvh_spatial_split = use_bvh_spatial_split == 1;
	params->bvh_layout = (ccl::BVHLayout)bvh_layout;
    // TODO: XXXX check if persistent data is done differently
	//params->persistent_data = persistent_data == 1;

	scene_params.push_back(params);

    /*
	logger.logit("Created scene parameters ", scene_params.size() - 1, "\n\tshading system: ", params->shadingsystem, "\n\tbvh_type: ", params->bvh_type, "\n\tuse_bvh_spatial_split: ", params->use_bvh_spatial_split, "\n\tuse_qbvh: ", params->bvh_layout, "\n\tpersistent data: ", params->persistent_data);*/

	return (unsigned int)(scene_params.size() - 1);
}

/* Set scene parameters*/
void cycles_scene_params_set_bvh_type(unsigned int scene_params_id, unsigned int bvh_type)
{
	SCENE_PARAM_CAST(scene_params_id, ccl::BVHType, bvh_type)
}

void cycles_scene_params_set_bvh_spatial_split(unsigned int scene_params_id, unsigned int use_bvh_spatial_split)
{
	SCENE_PARAM_BOOL(scene_params_id, use_bvh_spatial_split)
}
void cycles_scene_params_set_qbvh(unsigned int scene_params_id, unsigned int use_qbvh)
{
    // TODO: XXXX revisit BVH settings, there are different ones now.
    // For now default to BVH_LAYOUT_BVH2, but there will be much better ones
	ccl::BVHLayout bvh_layout = ccl::BVHLayout::BVH_LAYOUT_BVH2;
	SCENE_PARAM_CAST(scene_params_id, ccl::BVHLayout, bvh_layout)
}

void cycles_scene_params_set_shadingsystem(unsigned int scene_params_id, unsigned int shadingsystem)
{
	SCENE_PARAM_CAST(scene_params_id, ccl::ShadingSystem, shadingsystem)
}
void cycles_scene_params_set_persistent_data(unsigned int scene_params_id, unsigned int persistent_data)
{
    // TODO: XXXX no longer exists
	//SCENE_PARAM_BOOL(scene_params_id, persistent_data)
}
