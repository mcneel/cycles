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


#ifdef __cplusplus
extern "C" {
#endif

CCL_CAPI ccl::RhinoMapping* CDECL cycles_scene_add_decal(ccl::Session* session_id)
{
	ccl::Scene* sce = session_id->scene;
	if(sce)
	{
		auto decal= sce->create_node<ccl::RhinoMapping>();

		return decal;
	}

	return nullptr;
}

CCL_CAPI void CDECL cycles_scene_decal_delete(ccl::Session* session, ccl::RhinoMapping* decal)
{
	ccl::Scene* sce = session->scene;
	if(sce)
	{
		sce->delete_node(decal);
	}
}

CCL_CAPI void CDECL cycles_decal_set_pxyz(ccl::RhinoMapping* decal,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	ccl::Transform pxyz = ccl::transform_identity();
	ccl::float4 x = ccl::make_float4(a, b, c, d);
	ccl::float4 y = ccl::make_float4(e, f, g, h);
	ccl::float4 z = ccl::make_float4(i, j, k, l);
	pxyz.x = x;
	pxyz.y = y;
	pxyz.z = z;
	decal->set_pxyz(pxyz);
}

CCL_CAPI void CDECL cycles_decal_set_nxyz(ccl::RhinoMapping* decal,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	ccl::Transform nxyz = ccl::transform_identity();
	ccl::float4 x = ccl::make_float4(a, b, c, d);
	ccl::float4 y = ccl::make_float4(e, f, g, h);
	ccl::float4 z = ccl::make_float4(i, j, k, l);
	nxyz.x = x;
	nxyz.y = y;
	nxyz.z = z;
	decal->set_nxyz(nxyz);
}

CCL_CAPI void CDECL cycles_decal_set_uvw(ccl::RhinoMapping* decal,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	)
{
	ccl::Transform uvw = ccl::transform_identity();
	ccl::float4 x = ccl::make_float4(a, b, c, d);
	ccl::float4 y = ccl::make_float4(e, f, g, h);
	ccl::float4 z = ccl::make_float4(i, j, k, l);
	uvw.x = x;
	uvw.y = y;
	uvw.z = z;
	decal->set_uvw(uvw);
}

#ifdef __cplusplus
}
#endif
