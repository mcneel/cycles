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

void cycles_film_set_exposure(unsigned int client_id, unsigned int scene_id, float exposure)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->film->set_exposure(exposure);
		sce->film->tag_modified();
	}
}

void cycles_film_set_filter(unsigned int client_id, unsigned int scene_id, unsigned int filter_type, float filter_width)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->film->set_filter_type((ccl::FilterType)filter_type);
		if (sce->film->get_filter_type() == ccl::FilterType::FILTER_BOX) sce->film->set_filter_width(1.0f);
		else sce->film->set_filter_width(filter_width);
	}
}

void cycles_film_set_use_sample_clamp(unsigned int client_id, unsigned int scene_id, bool use_sample_clamp)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		//sce->film->use_sample_clamp = use_sample_clamp;
	}
}

void cycles_film_tag_update(unsigned int client_id, unsigned int scene_id)
{
	CCScene* csce = nullptr;
	ccl::Scene* sce = nullptr;
	if(scene_find(scene_id, &csce, &sce)) {
		sce->film->tag_modified();
	}
}

