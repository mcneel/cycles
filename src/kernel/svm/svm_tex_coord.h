/*
 * Copyright 2011-2013 Blender Foundation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

CCL_NAMESPACE_BEGIN

/* Texture Coordinate Node */

/* wcs_box_coord gives a Rhino-style WCS box texture coordinate mapping. */
ccl_device_inline void wcs_box_coord(KernelGlobals *kg, ShaderData *sd, float3 *data)
{
	float3 N = sd->N;

	int side0 = 0;

	float dx = (*data).x;
	float dy = (*data).y;

	// set side0 = side closest to the point
	int side1 = (fabsf(dx) >= fabsf(dy)) ? 0 : 1;
	float rr = side1 ? dy : dx;
	if (fabsf((*data).z) > fabsf(rr))
		side1 = 2;

	float t1 = side1 ? dy : dx;
	if (t1 < 0.0f)
		side0 = 2 * side1 + 1;
	else
		side0 = 2 * side1 + 2;

	side1 = (fabsf(N.x) >= fabsf(N.y)) ? 0 : 1;
	rr = side1 ? N.y : N.x;
	if (fabsf(N.z) > fabsf(rr))
	{
		side1 = 2;
	}

	switch (side1) {
	case 0: { t1 = N.x; break; }
	case 1: { t1 = N.y; break; }
	default: { t1 = N.z; break; }
	}
	if (0.0f != t1)
	{
		if (t1 < 0.0f)
			side0 = 2 * side1 + 1;
		else
			if (t1 > 0.0f)
				side0 = 2 * side1 + 2;
	}

	// side flag
	//  1 =  left side (x=-1)
	//  2 =  right side (x=+1)
	//  3 =  back side (y=-1)
	//  4 =  front side (y=+1)
	//  5 =  bottom side (z=-1)
	//  6 =  top side (z=+1)
	float3 v = make_float3(0.0f, 0.0f, 0.0f);
	switch (side0)
	{
	case 1:
		v.x = -(*data).y;
		v.y = (*data).z;
		v.z = (*data).x;
		break;
	case 2:
		v.x = (*data).y;
		v.y = (*data).z;
		v.z = (*data).x;
		break;
	case 3:
		v.x = (*data).x;
		v.y = (*data).z;
		v.z = (*data).y;
		break;
	case 4:
		v.x = -(*data).x;
		v.y = (*data).z;
		v.z = (*data).y;
		break;
	case 5:
		v.x = -(*data).x;
		v.y = (*data).y;
		v.z = (*data).z;
		break;
	case 6:
	default:
		v.x = (*data).x;
		v.y = (*data).y;
		v.z = (*data).z;
		break;
	}

	*data = v;
}

ccl_device_inline float3 get_reflected_incoming_ray(KernelGlobals *kg, ShaderData *sd)
{
	float3 n = sd->N;
	float3 i = sd->I;

	float3 refl = 2 * n * dot(i, n) - i;

	refl = normalize(refl);

	return refl;
}

ccl_device_inline float3 env_spherical(KernelGlobals *kg, ShaderData *sd)
{
	float3 R = get_reflected_incoming_ray(kg, sd);

	float3 Rc = make_float3(R.y, -R.z, -R.x);

	float x = -Rc.z;
	float y = -Rc.x;
	float z = Rc.y;

	float theta, phi;

	if( x == 0.0f && y == 0.0f ) {
		theta = 0.0f;
		phi = ( z >= 0.0f ? 0.5f*M_PI_F : -0.5f*M_PI_F );
	}
	else {
		theta = atan2( y, x );
		if( theta < 0.0f )
			theta += 2.0f*M_PI_F;

		float r;
		if ( fabsf( x ) >= fabsf( y ) ) {
			r = y/x;
			r = fabsf(x)*sqrt(1.0f+r*r);
		}
		else {
			r = x/y;
			r = fabsf(y)*sqrt(1.0f+r*r);
		}

		phi = atan( z/r );
	}

	float u = theta / (2.0f*M_PI_F);
	float v = (-phi + 0.5f*M_PI_F) / M_PI_F;

	return make_float3(u, v, 0.0f);
}

ccl_device_inline float3 env_emap_act( float3 R )
{
	float x = R.x;
	float y = R.y;
	float z = R.z;

	float fDivisor = sqrt((x * x) + (y * y));

	if (fDivisor < FLT_MIN) fDivisor = FLT_MIN;

	float f = sin(0.5f * acos(z)) / fDivisor;

	float px = -x * f;
	float py = y * f;

	float u = (1.0f + px) * 0.5f;
	float v = (1.0f - py) * 0.5f;

	return make_float3(u, v, 0.0f);
}

ccl_device_inline float3 env_emap( KernelGlobals *kg, ShaderData *sd )
{
	float3 R = get_reflected_incoming_ray( kg, sd );

	R = make_float3( R.y, -R.z, -R.x );

	return env_emap_act( R );
}

ccl_device_inline float3 env_light_probe( KernelGlobals *kg, ShaderData *sd )
{
	float3 R = get_reflected_incoming_ray( kg, sd );

	R = make_float3( R.y, -R.z, -R.x );

	float x = R.x;
	float y = R.y;
	float z = R.z;

	float fDivisor = sqrt( (x * x) + (y * y) );

	if( fDivisor < FLT_MIN ) fDivisor = FLT_MIN;

	float f = (acos( z ) / M_PI_F) / fDivisor;

	float px = x * f;
	float py = y * f;

	float u = (1.0f + px) * 0.5f;
	float v = (1.0f - py) * 0.5f;

	return make_float3( u, v, 0.0f );
}

ccl_device_inline float3 env_box( KernelGlobals *kg, ShaderData *sd )
{
	float3 R = get_reflected_incoming_ray( kg, sd );

//	Transform w2c = kernel_data.cam.worldtocamera;
//	R = transform_direction( &w2c, R );

	float x_abs = fabsf( R.x );
	float y_abs = fabsf( R.y );
	float z_abs = fabsf( R.z );

	float3 face_o, face_x, face_y;

	if( x_abs > y_abs && x_abs > z_abs ) {
		if( R.x > 0.0f )
		{
			face_o = make_float3( +1.f, +1.f, -1.f );
			face_x = make_float3(  0.f, -1.f,  0.f );
			face_y = make_float3(  0.f,  0.f, +1.f );
		}
		else
		{
			face_o = make_float3( -1.f, -1.f, -1.f );
			face_x = make_float3(  0.f, +1.f,  0.f );
			face_y = make_float3(  0.f,  0.f, +1.f );
		}
	}
	else if( y_abs > z_abs )
	{
		if( R.y > 0.0f )
		{
			face_o = make_float3( -1.f, +1.f, -1.f );
			face_x = make_float3( +1.f,  0.f,  0.f );
			face_y = make_float3(  0.f,  0.f, +1.f );
		}
		else
		{
			face_o = make_float3( +1.f, -1.f, -1.f );
			face_x = make_float3( -1.f,  0.f,  0.f );
			face_y = make_float3(  0.f,  0.f, +1.f );
		}
	}
	else
	{
		if( R.z > 0.0f )
		{
			face_o = make_float3( +1.f, +1.f, +1.f );
			face_x = make_float3(  0.f, -1.f,  0.f );
			face_y = make_float3( -1.f,  0.f,  0.f );
		}
		else
		{
			face_o = make_float3( -1.f, +1.f, -1.f );
			face_x = make_float3(  0.f, -1.f,  0.f );
			face_y = make_float3( +1.f,  0.f,  0.f );
		}
	}

	float3 plane_normal = cross( face_x, face_y );

	float rp_dot = dot( R, plane_normal );
	kernel_assert( rp_dot != 0.0f );

	float t = dot( face_o, plane_normal ) / rp_dot;

	float3 isect = t*R;
	float3 local_isect = isect - face_o;

	float u = dot( local_isect, face_x ) / 2.0f;
	float v = dot( local_isect, face_y ) / 2.0f;

	return make_float3( u, v, 0.0f );
}

ccl_device_inline int GetMainAxisIndex( float3 v )
{
	float x_abs = fabsf( v.x );
	float y_abs = fabsf( v.y );
	float z_abs = fabsf( v.z );

	if( x_abs > y_abs && x_abs > z_abs ) {
		return 0;
	}
	else if( y_abs > z_abs ) {
		return 1;
	}
	else {
		return 2;
	}
}

ccl_device_inline float3 env_cubemap( KernelGlobals *kg, ShaderData *sd )
{
	float3 R = get_reflected_incoming_ray( kg, sd );

	R = make_float3( R.y, -R.z, -R.x );

	int mainAxis = GetMainAxisIndex( R );
	float mainAxisDir = (mainAxis == 0 ? R.x : (mainAxis == 1 ? R.y : R.z));

	int subTextureIndex;

	if( mainAxis == 0 ) {
		subTextureIndex = (mainAxisDir >= 0.0f ? 0 : 1);
	}
	else if( mainAxis == 1 ) {
		subTextureIndex = (mainAxisDir >= 0.0f ? 3 : 2);
	}
	else {
		subTextureIndex = (mainAxisDir >= 0.0f ? 4 : 5);
	}

	float subTextureOffset = (float)( subTextureIndex ) / 6.0f;
	float ma = fabsf( mainAxisDir );

	float sc = 0.0f;
	float tc = 0.0f;

	if( subTextureIndex == 0 )
	{
		sc = -R.z;
		tc = -R.y;
	}
	else if( subTextureIndex == 1 )
	{
		sc = R.z;
		tc = -R.y;
	}
	else if( subTextureIndex == 2 )
	{
		sc = R.x;
		tc = -R.z;
	}
	else if( subTextureIndex == 3 )
	{
		sc = R.x;
		tc = R.z;
	}
	else if( subTextureIndex == 4 )
	{
		sc = R.x;
		tc = -R.y;
	}
	else if( subTextureIndex == 5 )
	{
		sc = -R.x;
		tc = -R.y;
	}

	float u = (sc / ma + 1.0f) / 12.0f + subTextureOffset;
	float v = (tc / ma + 1.0f) / 2.0f;

	return make_float3( u, v, 0.0f );
}

ccl_device_inline float3 env_cubemap_vertical_cross( KernelGlobals *kg, ShaderData *sd )
{
	float3 R = get_reflected_incoming_ray( kg, sd );

	R = make_float3( R.y, -R.z, -R.x );

	int mainAxis = GetMainAxisIndex( R );
	float mainAxisDir = (mainAxis == 0 ? R.x : ( mainAxis == 1 ? R.y : R.z ) );

	int subTextureIndex = (2 * mainAxis) + (mainAxisDir >= 0.0f ? 0 : 1);

	float uSubTexStart = (subTextureIndex == 1 ? 0.0f : (subTextureIndex == 0 ? (2.0f / 3.0f) : (1.0f / 3.0f ) ) );
	float vSubTexStart = (subTextureIndex == 5 ? 0.0f : (subTextureIndex == 2 ? (1.0f / 4.0f) : (subTextureIndex == 3 ? (3.0f / 4.0f) : (2.0f / 4.0f ) ) ) );

	float ma = fabsf( mainAxisDir );

	float sc = 0.0f;
	float tc = 0.0f;

	if( subTextureIndex == 0 )
	{
		sc = -R.z;
		tc = -R.y;
	}
	else if( subTextureIndex == 1 )
	{
		sc = R.z;
		tc = -R.y;
	}
	else if( subTextureIndex == 2 )
	{
		sc = R.x;
		tc = R.z;
	}
	else if( subTextureIndex == 3 )
	{
		sc = R.x;
		tc = -R.z;
	}
	else if( subTextureIndex == 4 )
	{
		sc = R.x;
		tc = -R.y;
	}
	else if( subTextureIndex == 5 )
	{
		sc = R.x;
		tc = R.y;
	}

	float u = (sc / ma + 1.0f) / 6.0f + uSubTexStart;
	float v = (tc / ma + 1.0f) / 8.0f + vSubTexStart;

	return make_float3( u, v, 0.0f );
}

ccl_device_inline float3 env_cubemap_horizontal_cross( KernelGlobals *kg, ShaderData *sd )
{
	float3 R = get_reflected_incoming_ray( kg, sd );

	R = make_float3( R.y, -R.z, -R.x );

	int mainAxis = GetMainAxisIndex( R );
	float mainAxisDir = (mainAxis == 0 ? R.x : (mainAxis == 1 ? R.y : R.z));

	int subTextureIndex = (2 * mainAxis) + (mainAxisDir >= 0.0f ? 0 : 1);

	float uSubTexStart = subTextureIndex == 1 ? 0.0f : (subTextureIndex == 0 ? (2.0f / 4.0f) : (subTextureIndex == 5 ? (3.0f / 4.0f) : (1.0f / 4.0f) ) );
	float vSubTexStart = subTextureIndex == 2 ? 0.0f : (subTextureIndex == 3 ? (2.0f / 3.0f) : (1.0f / 3.0f) );

	float ma = fabsf( mainAxisDir );

	float sc = 0.0f;
	float tc = 0.0f;

	if( subTextureIndex == 0 )
	{
		sc = -R.z;
		tc = -R.y;
	}
	else if( subTextureIndex == 1 )
	{
		sc = R.z;
		tc = -R.y;
	}
	else if( subTextureIndex == 2 )
	{
		sc = R.x;
		tc = R.z;
	}
	else if( subTextureIndex == 3 )
	{
		sc = R.x;
		tc = -R.z;
	}
	else if( subTextureIndex == 4 )
	{
		sc = R.x;
		tc = -R.y;
	}
	else if( subTextureIndex == 5 )
	{
		sc = -R.x;
		tc = -R.y;
	}

	float u = (sc / ma + 1.0f) / 8.0f + uSubTexStart;
	float v = (tc / ma + 1.0f) / 6.0f + vSubTexStart;

	return make_float3( u, v, 0.0f );
}

ccl_device_inline float3 env_hemispherical( KernelGlobals *kg, ShaderData *sd )
{
	float3 R = get_reflected_incoming_ray( kg, sd );

	R = make_float3( R.y, -R.z, -R.x );

	float3 hemi = normalize( make_float3( R.x, min( R.y, 0.0f ), R.z ) );

	return env_emap_act( hemi );
}

ccl_device void svm_node_tex_coord(KernelGlobals *kg,
                                   ShaderData *sd,
                                   int path_flag,
                                   float *stack,
                                   uint4 node,
                                   int *offset)
{
	float3 data;
	uint type = node.y;
	uint out_offset = node.z;

	switch(type) {
		case NODE_TEXCO_OBJECT: {
			data = sd->P;
			if(node.w == 0) {
				if(sd->object != OBJECT_NONE) {
					object_inverse_position_transform(kg, sd, &data);
				}
			}
			else {
				Transform tfm;
				tfm.x = read_node_float(kg, offset);
				tfm.y = read_node_float(kg, offset);
				tfm.z = read_node_float(kg, offset);
				tfm.w = read_node_float(kg, offset);
				data = transform_point(&tfm, data);
			}
			break;
		}
		case NODE_TEXCO_WCS_BOX: {
			data = sd->P;
			if (node.w == 0) {
				if (sd->object != OBJECT_NONE) {
					object_inverse_position_transform(kg, sd, &data);
				}
			}
			else {
				Transform tfm;
				tfm.x = read_node_float(kg, offset);
				tfm.y = read_node_float(kg, offset);
				tfm.z = read_node_float(kg, offset);
				tfm.w = read_node_float(kg, offset);
				data = transform_point(&tfm, data);
			}
			wcs_box_coord(kg, sd, &data);
			break;
		}
		case NODE_TEXCO_NORMAL: {
			data = sd->N;
			object_inverse_normal_transform(kg, sd, &data);
			break;
		}
		case NODE_TEXCO_CAMERA: {
			Transform tfm = kernel_data.cam.worldtocamera;

			if(sd->object != OBJECT_NONE)
				data = transform_point(&tfm, sd->P);
			else
				data = transform_point(&tfm, sd->P + camera_position(kg));
			break;
		}
		case NODE_TEXCO_WINDOW: {
			if((path_flag & PATH_RAY_CAMERA) && sd->object == OBJECT_NONE && kernel_data.cam.type == CAMERA_ORTHOGRAPHIC)
				data = camera_world_to_ndc(kg, sd, sd->ray_P);
			else
				data = camera_world_to_ndc(kg, sd, sd->P);
			data.z = 0.0f;
			break;
		}
		case NODE_TEXCO_REFLECTION: {
			if(sd->object != OBJECT_NONE)
				data = 2.0f*dot(sd->N, sd->I)*sd->N - sd->I;
			else
				data = sd->I;
			break;
		}
		case NODE_TEXCO_DUPLI_GENERATED: {
			data = object_dupli_generated(kg, sd->object);
			break;
		}
		case NODE_TEXCO_DUPLI_UV: {
			data = object_dupli_uv(kg, sd->object);
			break;
		}
		case NODE_TEXCO_VOLUME_GENERATED: {
			data = sd->P;

#ifdef __VOLUME__
			if(sd->object != OBJECT_NONE)
				data = volume_normalized_position(kg, sd, data);
#endif
			break;
		}
		case NODE_TEXCO_ENV_SPHERICAL: {
			data = env_spherical(kg, sd);
			break;
		}
		case NODE_TEXCO_ENV_EMAP: {
			data = env_emap(kg, sd);
			break;
		}
		case NODE_TEXCO_ENV_BOX: {
			data = env_box( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_LIGHTPROBE: {
			data = env_light_probe( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_CUBEMAP: {
			data = env_cubemap( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_CUBEMAP_VERTICAL_CROSS: {
			data = env_cubemap_vertical_cross( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_CUBEMAP_HORIZONTAL_CROSS: {
			data = env_cubemap_horizontal_cross( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_HEMI: {
			data = env_hemispherical( kg, sd );
			break;
		}
	}

	stack_store_float3(stack, out_offset, data);
}

ccl_device void svm_node_tex_coord_bump_dx(KernelGlobals *kg,
                                           ShaderData *sd,
                                           int path_flag,
                                           float *stack,
                                           uint4 node,
                                           int *offset)
{
#ifdef __RAY_DIFFERENTIALS__
	float3 data;
	uint type = node.y;
	uint out_offset = node.z;

	switch(type) {
		case NODE_TEXCO_OBJECT: {
			data = sd->P + sd->dP.dx;
			if(node.w == 0) {
				if(sd->object != OBJECT_NONE) {
					object_inverse_position_transform(kg, sd, &data);
				}
			}
			else {
				Transform tfm;
				tfm.x = read_node_float(kg, offset);
				tfm.y = read_node_float(kg, offset);
				tfm.z = read_node_float(kg, offset);
				tfm.w = read_node_float(kg, offset);
				data = transform_point(&tfm, data);
			}
			break;
		}
		case NODE_TEXCO_WCS_BOX: {
			data = sd->P + sd->dP.dx;
			if (node.w == 0) {
				if (sd->object != OBJECT_NONE) {
					object_inverse_position_transform(kg, sd, &data);
				}
			}
			else {
				Transform tfm;
				tfm.x = read_node_float(kg, offset);
				tfm.y = read_node_float(kg, offset);
				tfm.z = read_node_float(kg, offset);
				tfm.w = read_node_float(kg, offset);
				data = transform_point(&tfm, data);
			}
			wcs_box_coord(kg, sd, &data);
			break;
		}
		case NODE_TEXCO_NORMAL: {
			data = sd->N;
			object_inverse_normal_transform(kg, sd, &data);
			break;
		}
		case NODE_TEXCO_CAMERA: {
			Transform tfm = kernel_data.cam.worldtocamera;

			if(sd->object != OBJECT_NONE)
				data = transform_point(&tfm, sd->P + sd->dP.dx);
			else
				data = transform_point(&tfm, sd->P + sd->dP.dx + camera_position(kg));
			break;
		}
		case NODE_TEXCO_WINDOW: {
			if((path_flag & PATH_RAY_CAMERA) && sd->object == OBJECT_NONE && kernel_data.cam.type == CAMERA_ORTHOGRAPHIC)
				data = camera_world_to_ndc(kg, sd, sd->ray_P + sd->ray_dP.dx);
			else
				data = camera_world_to_ndc(kg, sd, sd->P + sd->dP.dx);
			data.z = 0.0f;
			break;
		}
		case NODE_TEXCO_REFLECTION: {
			if(sd->object != OBJECT_NONE)
				data = 2.0f*dot(sd->N, sd->I)*sd->N - sd->I;
			else
				data = sd->I;
			break;
		}
		case NODE_TEXCO_DUPLI_GENERATED: {
			data = object_dupli_generated(kg, sd->object);
			break;
		}
		case NODE_TEXCO_DUPLI_UV: {
			data = object_dupli_uv(kg, sd->object);
			break;
		}
		case NODE_TEXCO_VOLUME_GENERATED: {
			data = sd->P + sd->dP.dx;

#ifdef __VOLUME__
			if(sd->object != OBJECT_NONE)
				data = volume_normalized_position(kg, sd, data);
#endif
			break;
		}
		case NODE_TEXCO_ENV_SPHERICAL: {
			data = env_spherical(kg, sd);
			break;
		}
		case NODE_TEXCO_ENV_EMAP: {
			data = env_emap(kg, sd);
			break;
		}
		case NODE_TEXCO_ENV_LIGHTPROBE: {
			data = env_light_probe( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_CUBEMAP: {
			data = env_cubemap( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_CUBEMAP_VERTICAL_CROSS: {
			data = env_cubemap_vertical_cross( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_CUBEMAP_HORIZONTAL_CROSS: {
			data = env_cubemap_horizontal_cross( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_HEMI: {
			data = env_hemispherical( kg, sd );
			break;
		}
	}

	stack_store_float3(stack, out_offset, data);
#else
	svm_node_tex_coord(kg, sd, path_flag, stack, node, offset);
#endif
}

ccl_device void svm_node_tex_coord_bump_dy(KernelGlobals *kg,
                                           ShaderData *sd,
                                           int path_flag,
                                           float *stack,
                                           uint4 node,
                                           int *offset)
{
#ifdef __RAY_DIFFERENTIALS__
	float3 data;
	uint type = node.y;
	uint out_offset = node.z;

	switch(type) {
		case NODE_TEXCO_OBJECT: {
			data = sd->P + sd->dP.dy;
			if(node.w == 0) {
				if(sd->object != OBJECT_NONE) {
					object_inverse_position_transform(kg, sd, &data);
				}
			}
			else {
				Transform tfm;
				tfm.x = read_node_float(kg, offset);
				tfm.y = read_node_float(kg, offset);
				tfm.z = read_node_float(kg, offset);
				tfm.w = read_node_float(kg, offset);
				data = transform_point(&tfm, data);
			}
			break;
		}
		case NODE_TEXCO_WCS_BOX: {
			data = sd->P + sd->dP.dy;
			if (node.w == 0) {
				if (sd->object != OBJECT_NONE) {
					object_inverse_position_transform(kg, sd, &data);
				}
			}
			else {
				Transform tfm;
				tfm.x = read_node_float(kg, offset);
				tfm.y = read_node_float(kg, offset);
				tfm.z = read_node_float(kg, offset);
				tfm.w = read_node_float(kg, offset);
				data = transform_point(&tfm, data);
			}
			wcs_box_coord(kg, sd, &data);
			break;
		}
		case NODE_TEXCO_NORMAL: {
			data = sd->N;
			object_inverse_normal_transform(kg, sd, &data);
			break;
		}
		case NODE_TEXCO_CAMERA: {
			Transform tfm = kernel_data.cam.worldtocamera;

			if(sd->object != OBJECT_NONE)
				data = transform_point(&tfm, sd->P + sd->dP.dy);
			else
				data = transform_point(&tfm, sd->P + sd->dP.dy + camera_position(kg));
			break;
		}
		case NODE_TEXCO_WINDOW: {
			if((path_flag & PATH_RAY_CAMERA) && sd->object == OBJECT_NONE && kernel_data.cam.type == CAMERA_ORTHOGRAPHIC)
				data = camera_world_to_ndc(kg, sd, sd->ray_P + sd->ray_dP.dy);
			else
				data = camera_world_to_ndc(kg, sd, sd->P + sd->dP.dy);
			data.z = 0.0f;
			break;
		}
		case NODE_TEXCO_REFLECTION: {
			if(sd->object != OBJECT_NONE)
				data = 2.0f*dot(sd->N, sd->I)*sd->N - sd->I;
			else
				data = sd->I;
			break;
		}
		case NODE_TEXCO_DUPLI_GENERATED: {
			data = object_dupli_generated(kg, sd->object);
			break;
		}
		case NODE_TEXCO_DUPLI_UV: {
			data = object_dupli_uv(kg, sd->object);
			break;
		}
		case NODE_TEXCO_VOLUME_GENERATED: {
			data = sd->P + sd->dP.dy;

#ifdef __VOLUME__
			if(sd->object != OBJECT_NONE)
				data = volume_normalized_position(kg, sd, data);
#endif
			break;
		}
		case NODE_TEXCO_ENV_SPHERICAL: {
			data = env_spherical(kg, sd);
			break;
		}
		case NODE_TEXCO_ENV_EMAP: {
			data = env_emap(kg, sd);
			break;
		}
		case NODE_TEXCO_ENV_LIGHTPROBE: {
			data = env_light_probe( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_CUBEMAP: {
			data = env_cubemap( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_CUBEMAP_VERTICAL_CROSS: {
			data = env_cubemap_vertical_cross( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_CUBEMAP_HORIZONTAL_CROSS: {
			data = env_cubemap_horizontal_cross( kg, sd );
			break;
		}
		case NODE_TEXCO_ENV_HEMI: {
			data = env_hemispherical( kg, sd );
			break;
		}
	}

	stack_store_float3(stack, out_offset, data);
#else
	svm_node_tex_coord(kg, sd, path_flag, stack, node, offset);
#endif
}

ccl_device void svm_node_normal_map(KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node)
{
	uint color_offset, strength_offset, normal_offset, space;
	decode_node_uchar4(node.y, &color_offset, &strength_offset, &normal_offset, &space);

	float3 color = stack_load_float3(stack, color_offset);
	color = 2.0f*make_float3(color.x - 0.5f, color.y - 0.5f, color.z - 0.5f);

	bool is_backfacing = (sd->flag & SD_BACKFACING) != 0;
	float3 N;

	if(space == NODE_NORMAL_MAP_TANGENT) {
		/* tangent space */
		if(sd->object == OBJECT_NONE) {
			stack_store_float3(stack, normal_offset, make_float3(0.0f, 0.0f, 0.0f));
			return;
		}

		/* first try to get tangent attribute */
		const AttributeDescriptor attr = find_attribute(kg, sd, node.z);
		const AttributeDescriptor attr_sign = find_attribute(kg, sd, node.w);
		const AttributeDescriptor attr_normal = find_attribute(kg, sd, ATTR_STD_VERTEX_NORMAL);

		if(attr.offset == ATTR_STD_NOT_FOUND || attr_sign.offset == ATTR_STD_NOT_FOUND || attr_normal.offset == ATTR_STD_NOT_FOUND) {
			stack_store_float3(stack, normal_offset, make_float3(0.0f, 0.0f, 0.0f));
			return;
		}

		/* get _unnormalized_ interpolated normal and tangent */
		float3 tangent = primitive_attribute_float3(kg, sd, attr, NULL, NULL);
		float sign = primitive_attribute_float(kg, sd, attr_sign, NULL, NULL);
		float3 normal;

		if(sd->shader & SHADER_SMOOTH_NORMAL) {
			normal = primitive_attribute_float3(kg, sd, attr_normal, NULL, NULL);
		}
		else {
			normal = sd->Ng;

			/* the normal is already inverted, which is too soon for the math here */
			if(is_backfacing) {
				normal = -normal;
			}

			object_inverse_normal_transform(kg, sd, &normal);
		}

		/* apply normal map */
		float3 B = sign * cross(normal, tangent);
		N = safe_normalize(color.x * tangent + color.y * B + color.z * normal);

		/* transform to world space */
		object_normal_transform(kg, sd, &N);
	}
	else {
		/* strange blender convention */
		if(space == NODE_NORMAL_MAP_BLENDER_OBJECT || space == NODE_NORMAL_MAP_BLENDER_WORLD) {
			color.y = -color.y;
			color.z = -color.z;
		}
	
		/* object, world space */
		N = color;

		if(space == NODE_NORMAL_MAP_OBJECT || space == NODE_NORMAL_MAP_BLENDER_OBJECT)
			object_normal_transform(kg, sd, &N);
		else
			N = safe_normalize(N);
	}

	/* invert normal for backfacing polygons */
	if(is_backfacing) {
		N = -N;
	}

	float strength = stack_load_float(stack, strength_offset);

	if(strength != 1.0f) {
		strength = max(strength, 0.0f);
		N = safe_normalize(sd->N + (N - sd->N)*strength);
	}

	if(is_zero(N)) {
		N = sd->N;
	}

	stack_store_float3(stack, normal_offset, N);
}

ccl_device void svm_node_tangent(KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node)
{
	uint tangent_offset, direction_type, axis;
	decode_node_uchar4(node.y, &tangent_offset, &direction_type, &axis, NULL);

	float3 tangent;

	if(direction_type == NODE_TANGENT_UVMAP) {
		/* UV map */
		const AttributeDescriptor desc = find_attribute(kg, sd, node.z);

		if(desc.offset == ATTR_STD_NOT_FOUND)
			tangent = make_float3(0.0f, 0.0f, 0.0f);
		else
			tangent = primitive_attribute_float3(kg, sd, desc, NULL, NULL);
	}
	else {
		/* radial */
		const AttributeDescriptor desc = find_attribute(kg, sd, node.z);
		float3 generated;

		if(desc.offset == ATTR_STD_NOT_FOUND)
			generated = sd->P;
		else
			generated = primitive_attribute_float3(kg, sd, desc, NULL, NULL);

		if(axis == NODE_TANGENT_AXIS_X)
			tangent = make_float3(0.0f, -(generated.z - 0.5f), (generated.y - 0.5f));
		else if(axis == NODE_TANGENT_AXIS_Y)
			tangent = make_float3(-(generated.z - 0.5f), 0.0f, (generated.x - 0.5f));
		else
			tangent = make_float3(-(generated.y - 0.5f), (generated.x - 0.5f), 0.0f);
	}

	object_normal_transform(kg, sd, &tangent);
	tangent = cross(sd->N, normalize(cross(tangent, sd->N)));
	stack_store_float3(stack, tangent_offset, tangent);
}

CCL_NAMESPACE_END

