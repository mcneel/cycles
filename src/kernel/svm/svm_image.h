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

ccl_device float4 svm_image_texture(KernelGlobals *kg, int id, float x, float y, uint srgb, uint use_alpha)
{
	float4 r = kernel_tex_image_interp(kg, id, x, y);
	const float alpha = r.w;

	if(use_alpha && alpha != 1.0f && alpha != 0.0f) {
		r /= alpha;
		const int texture_type = kernel_tex_type(id);
		if(texture_type == IMAGE_DATA_TYPE_BYTE4 ||
		   texture_type == IMAGE_DATA_TYPE_BYTE)
		{
			r = min(r, make_float4(1.0f, 1.0f, 1.0f, 1.0f));
		}
		r.w = alpha;
	}

	if(srgb) {
		r = color_srgb_to_scene_linear_v4(r);
	}

	return r;
}

/* Remap coordnate from 0..1 box to -1..-1 */
ccl_device_inline float3 texco_remap_square(float3 co)
{
	return (co - make_float3(0.5f, 0.5f, 0.5f)) * 2.0f;
}

ccl_device void svm_node_tex_image(KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node)
{
	uint id = node.y;
	uint co_offset, out_offset, alpha_offset, srgb;

	decode_node_uchar4(node.z, &co_offset, &out_offset, &alpha_offset, &srgb);

	float3 co = stack_load_float3(stack, co_offset);
	float2 tex_co;
	uint use_alpha = stack_valid(alpha_offset);
	if(node.w == NODE_IMAGE_PROJ_SPHERE) {
		co = texco_remap_square(co);
		tex_co = map_to_sphere(co);
	}
	else if(node.w == NODE_IMAGE_PROJ_TUBE) {
		co = texco_remap_square(co);
		tex_co = map_to_tube(co);
	}
	else {
		tex_co = make_float2(co.x, co.y);
	}
	float4 f = svm_image_texture(kg, id, tex_co.x, tex_co.y, srgb, use_alpha);

	if(stack_valid(out_offset))
		stack_store_float3(stack, out_offset, make_float3(f.x, f.y, f.z));
	if(stack_valid(alpha_offset))
		stack_store_float(stack, alpha_offset, f.w);
}

ccl_device void svm_node_tex_image_box(KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node)
{
	/* get object space normal */
	float3 N = sd->N;

	N = sd->N;
	object_inverse_normal_transform(kg, sd, &N);

	/* project from direction vector to barycentric coordinates in triangles */
	float3 signed_N = N;

	N.x = fabsf(N.x);
	N.y = fabsf(N.y);
	N.z = fabsf(N.z);

	N /= (N.x + N.y + N.z);

	/* basic idea is to think of this as a triangle, each corner representing
	 * one of the 3 faces of the cube. in the corners we have single textures,
	 * in between we blend between two textures, and in the middle we a blend
	 * between three textures.
	 *
	 * the Nxyz values are the barycentric coordinates in an equilateral
	 * triangle, which in case of blending, in the middle has a smaller
	 * equilateral triangle where 3 textures blend. this divides things into
	 * 7 zones, with an if() test for each zone */

	float3 weight = make_float3(0.0f, 0.0f, 0.0f);
	float blend = __int_as_float(node.w);
	float limit = 0.5f*(1.0f + blend);

	/* first test for corners with single texture */
	if(N.x > limit*(N.x + N.y) && N.x > limit*(N.x + N.z)) {
		weight.x = 1.0f;
	}
	else if(N.y > limit*(N.x + N.y) && N.y > limit*(N.y + N.z)) {
		weight.y = 1.0f;
	}
	else if(N.z > limit*(N.x + N.z) && N.z > limit*(N.y + N.z)) {
		weight.z = 1.0f;
	}
	else if(blend > 0.0f) {
		/* in case of blending, test for mixes between two textures */
		if(N.z < (1.0f - limit)*(N.y + N.x)) {
			weight.x = N.x/(N.x + N.y);
			weight.x = saturate((weight.x - 0.5f*(1.0f - blend))/blend);
			weight.y = 1.0f - weight.x;
		}
		else if(N.x < (1.0f - limit)*(N.y + N.z)) {
			weight.y = N.y/(N.y + N.z);
			weight.y = saturate((weight.y - 0.5f*(1.0f - blend))/blend);
			weight.z = 1.0f - weight.y;
		}
		else if(N.y < (1.0f - limit)*(N.x + N.z)) {
			weight.x = N.x/(N.x + N.z);
			weight.x = saturate((weight.x - 0.5f*(1.0f - blend))/blend);
			weight.z = 1.0f - weight.x;
		}
		else {
			/* last case, we have a mix between three */
			weight.x = ((2.0f - limit)*N.x + (limit - 1.0f))/(2.0f*limit - 1.0f);
			weight.y = ((2.0f - limit)*N.y + (limit - 1.0f))/(2.0f*limit - 1.0f);
			weight.z = ((2.0f - limit)*N.z + (limit - 1.0f))/(2.0f*limit - 1.0f);
		}
	}
	else {
		/* Desperate mode, no valid choice anyway, fallback to one side.*/
		weight.x = 1.0f;
	}

	/* now fetch textures */
	uint co_offset, out_offset, alpha_offset, srgb;
	decode_node_uchar4(node.z, &co_offset, &out_offset, &alpha_offset, &srgb);

	float3 co = stack_load_float3(stack, co_offset);
	uint id = node.y;

	float4 f = make_float4(0.0f, 0.0f, 0.0f, 0.0f);
	uint use_alpha = stack_valid(alpha_offset);

	/* Map so that no textures are flipped, rotation is somewhat arbitrary. */
	if(weight.x > 0.0f) {
		float2 uv = make_float2((signed_N.x < 0.0f)? 1.0f - co.y: co.y, co.z);
		f += weight.x*svm_image_texture(kg, id, uv.x, uv.y, srgb, use_alpha);
	}
	if(weight.y > 0.0f) {
		float2 uv = make_float2((signed_N.y > 0.0f)? 1.0f - co.x: co.x, co.z);
		f += weight.y*svm_image_texture(kg, id, uv.x, uv.y, srgb, use_alpha);
	}
	if(weight.z > 0.0f) {
		float2 uv = make_float2((signed_N.z > 0.0f)? 1.0f - co.y: co.y, co.x);
		f += weight.z*svm_image_texture(kg, id, uv.x, uv.y, srgb, use_alpha);
	}

	if(stack_valid(out_offset))
		stack_store_float3(stack, out_offset, make_float3(f.x, f.y, f.z));
	if(stack_valid(alpha_offset))
		stack_store_float(stack, alpha_offset, f.w);
}

/*** Rhino-style environment projections *****/

ccl_device_inline float3 env_spherical(float3 R)
{
	//float3 Rc = make_float3(R.y, -R.z, -R.x);

	float x = -R.z;
	float y = -R.x;
	float z = R.y;

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

ccl_device_inline float3 env_world_emap(float3 R)
{
	float x = R.x;
	float y = R.y;
	float z = R.z;

	float m = 2.0f*sqrt((x * x) + (y * y) + (z + 1.0f)*(z + 1.0f));

	float u = x / m + 0.5f;
	float v = y / m + 0.5f;

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

ccl_device_inline float3 env_emap(float3 R)
{
	R = make_float3( R.y, -R.z, -R.x );

	return env_emap_act( R );
}

ccl_device_inline float3 env_light_probe(float3 R)
{
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

	return make_float3( u, v , 0.0f);
}

ccl_device_inline float3 env_box(float3 R)
{
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

	return make_float3( u, v , 0.0f);
}

ccl_device_inline int env_get_main_axis_index( float3 v )
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

ccl_device_inline float3 env_cubemap(float3 R)
{
	R = make_float3( R.y, -R.z, -R.x );

	int main_axis = env_get_main_axis_index( R );
	float main_axis_dir = (main_axis == 0 ? R.x : (main_axis == 1 ? R.y : R.z));

	int sub_texture_index;

	if( main_axis == 0 ) {
		sub_texture_index = (main_axis_dir >= 0.0f ? 0 : 1);
	}
	else if( main_axis == 1 ) {
		sub_texture_index = (main_axis_dir >= 0.0f ? 3 : 2);
	}
	else {
		sub_texture_index = (main_axis_dir >= 0.0f ? 4 : 5);
	}

	float sub_texture_offset = (float)( sub_texture_index ) / 6.0f;
	float ma = fabsf( main_axis_dir );

	float sc = 0.0f;
	float tc = 0.0f;

	if( sub_texture_index == 0 )
	{
		sc = -R.z;
		tc = -R.y;
	}
	else if( sub_texture_index == 1 )
	{
		sc = R.z;
		tc = -R.y;
	}
	else if( sub_texture_index == 2 )
	{
		sc = R.x;
		tc = -R.z;
	}
	else if( sub_texture_index == 3 )
	{
		sc = R.x;
		tc = R.z;
	}
	else if( sub_texture_index == 4 )
	{
		sc = R.x;
		tc = -R.y;
	}
	else if( sub_texture_index == 5 )
	{
		sc = -R.x;
		tc = -R.y;
	}

	float u = (sc / ma + 1.0f) / 12.0f + sub_texture_offset;
	float v = (tc / ma + 1.0f) / 2.0f;

	return make_float3( u, v , 0.0f);
}

ccl_device_inline float3 env_cubemap_vertical_cross(float3 R)
{
	R = make_float3( R.y, -R.z, -R.x );

	int main_axis = env_get_main_axis_index( R );
	float main_axis_dir = (main_axis == 0 ? R.x : ( main_axis == 1 ? R.y : R.z ) );

	int sub_texture_index = (2 * main_axis) + (main_axis_dir >= 0.0f ? 0 : 1);

	float u_sub_tex_start = (sub_texture_index == 1 ? 0.0f : (sub_texture_index == 0 ? (2.0f / 3.0f) : (1.0f / 3.0f ) ) );
	float v_sub_tex_start = (sub_texture_index == 5 ? 0.0f : (sub_texture_index == 2 ? (1.0f / 4.0f) : (sub_texture_index == 3 ? (3.0f / 4.0f) : (2.0f / 4.0f ) ) ) );

	float ma = fabsf( main_axis_dir );

	float sc = 0.0f;
	float tc = 0.0f;

	if( sub_texture_index == 0 )
	{
		sc = -R.z;
		tc = -R.y;
	}
	else if( sub_texture_index == 1 )
	{
		sc = R.z;
		tc = -R.y;
	}
	else if( sub_texture_index == 2 )
	{
		sc = R.x;
		tc = R.z;
	}
	else if( sub_texture_index == 3 )
	{
		sc = R.x;
		tc = -R.z;
	}
	else if( sub_texture_index == 4 )
	{
		sc = R.x;
		tc = -R.y;
	}
	else if( sub_texture_index == 5 )
	{
		sc = R.x;
		tc = R.y;
	}

	float u = (sc / ma + 1.0f) / 6.0f + u_sub_tex_start;
	float v = (tc / ma + 1.0f) / 8.0f + v_sub_tex_start;

	return make_float3( u, v , 0.0f);
}

ccl_device_inline float3 env_cubemap_horizontal_cross(float3 R)
{
	R = make_float3( R.y, -R.z, -R.x );

	int main_axis = env_get_main_axis_index( R );
	float main_axis_dir = (main_axis == 0 ? R.x : (main_axis == 1 ? R.y : R.z));

	int sub_texture_index = (2 * main_axis) + (main_axis_dir >= 0.0f ? 0 : 1);

	float u_sub_tex_start = sub_texture_index == 1 ? 0.0f : (sub_texture_index == 0 ? (2.0f / 4.0f) : (sub_texture_index == 5 ? (3.0f / 4.0f) : (1.0f / 4.0f) ) );
	float v_sub_tex_start = sub_texture_index == 2 ? 0.0f : (sub_texture_index == 3 ? (2.0f / 3.0f) : (1.0f / 3.0f) );

	float ma = fabsf( main_axis_dir );

	float sc = 0.0f;
	float tc = 0.0f;

	if( sub_texture_index == 0 )
	{
		sc = -R.z;
		tc = -R.y;
	}
	else if( sub_texture_index == 1 )
	{
		sc = R.z;
		tc = -R.y;
	}
	else if( sub_texture_index == 2 )
	{
		sc = R.x;
		tc = R.z;
	}
	else if( sub_texture_index == 3 )
	{
		sc = R.x;
		tc = -R.z;
	}
	else if( sub_texture_index == 4 )
	{
		sc = R.x;
		tc = -R.y;
	}
	else if( sub_texture_index == 5 )
	{
		sc = -R.x;
		tc = -R.y;
	}

	float u = (sc / ma + 1.0f) / 8.0f + u_sub_tex_start;
	float v = (tc / ma + 1.0f) / 6.0f + v_sub_tex_start;

	return make_float3( u, v , 0.0f);
}

ccl_device_inline float3 env_hemispherical(float3 R)
{
	R = make_float3( R.y, -R.z, -R.x );
	float3 hemi = normalize( make_float3( R.x, min( R.y, 0.0f ), R.z ) );
	float3 uv = env_spherical(hemi);
	uv.y = 2.0f*uv.y - 1.0f;

	return uv;
}
ccl_device void svm_node_tex_environment(KernelGlobals *kg, ShaderData *sd, float *stack, uint4 node)
{
	uint id = node.y;
	uint co_offset, out_offset, alpha_offset, srgb;
	uint projection = node.w;

	decode_node_uchar4(node.z, &co_offset, &out_offset, &alpha_offset, &srgb);

	float3 co = stack_load_float3(stack, co_offset);
	float2 uv;

	co = safe_normalize(co);

	if(projection == 0)
		uv = direction_to_equirectangular(co);
	else if (projection == 1)
		uv = direction_to_mirrorball(co);
	else if (projection == 2) {
		float3 P = sd->P;
		Transform worldtocamera = kernel_data.cam.worldtocamera;
		Transform cameratondc = kernel_data.cam.cameratondc;

		P = normalize(P);
		P = transform_direction(&worldtocamera, P);
		P = P / dot(P, make_float3(0.0f, 0.0f, 1.0f));
		P = transform_point(&cameratondc, P);
		uv.x = P.x;
		uv.y = P.y;
	}
	else if (projection == 3) {
		co = env_emap(co);
	}
	else if (projection == 4) {
		co = env_box(co);
	}
	else if (projection == 5) {
		co = env_light_probe(co);
	}
	else if (projection == 6) {
		co = env_cubemap(co);
	}
	else if (projection == 7) {
		co = env_cubemap_horizontal_cross(co);
	}
	else if (projection == 8) {
		co = env_cubemap_vertical_cross(co);
	}
	else if (projection == 9) {
		co = env_hemispherical(co);
	}
	else if (projection == 10) {
		co = make_float3(co.y, -co.z, -co.x);
		co = env_spherical(co);
	}

	if (projection >= 3 && projection <= 10)
	{
		uv.x = co.x;
		uv.y = co.y;
	}

	uint use_alpha = stack_valid(alpha_offset);
	float4 f = svm_image_texture(kg, id, uv.x, uv.y, srgb, use_alpha);

	if(stack_valid(out_offset))
		stack_store_float3(stack, out_offset, make_float3(f.x, f.y, f.z));
	if(stack_valid(alpha_offset))
		stack_store_float(stack, alpha_offset, f.w);
}

CCL_NAMESPACE_END

