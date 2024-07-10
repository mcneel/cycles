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

#include "util/debug.h"
#include "util/path.h"
#include "ccycles.h"

std::ostream& operator<<(std::ostream& out, shadernode_type const &snt) {
	out << (int)snt;
	return out;
}

/* Hold the device information found on the system after initialisation. */
std::vector<ccl::DeviceInfo> devices;
std::vector<ccl::DeviceInfo> multi_devices;

ccl::vector<float> ccycles_rhino_perlin_noise_table;
ccl::vector<float> ccycles_rhino_impulse_noise_table;
ccl::vector<float> ccycles_rhino_vc_noise_table;
ccl::vector<float> ccycles_rhino_aaltonen_noise_table;

/* Hold the logger function that potentially gets registered by a client. */
LOGGER_FUNC_CB logger_func = nullptr;

/* Need to initialise only once :) */
bool initialised{ false };
Logger logger;

std::vector<LOGGER_FUNC_CB> loggers;

#ifdef __cplusplus
extern "C" {
#endif

CCL_CAPI void CDECL cycles_path_init(const char* path, const char* user_path)
{
	ccl::path_init(std::string(path), std::string(user_path));
}

CCL_CAPI void CDECL cycles_putenv(const char* var, const char* val)
{
#if defined(_WIN32)
	_putenv_s(var, val);
#else
	setenv(var, val, 1);
#endif
}

CCL_CAPI void CDECL cycles_initialise(unsigned int mask)
{
	if (!initialised) {
		devices.clear();
		multi_devices.clear();
		devices = ccl::Device::available_devices(mask);
		initialised = true;
	}
}

CCL_CAPI void CDECL cycles_debug_set_cpu_allow_qbvh(unsigned int state)
{
	assert(false);
    // TODO: XXXX no quad bvh
	ccl::BVHLayout bvh_layout = ccl::BVHLayout::BVH_LAYOUT_BVH2;
	ccl::DebugFlags().cpu.bvh_layout = bvh_layout;
}

CCL_CAPI void CDECL cycles_shutdown()
{
	if (!initialised) {
		return;
	}

	logger_func = nullptr;

	_cleanup_sessions();
}

CCL_CAPI void CDECL cycles_log_to_stdout(int tostdout)
{
	logger.tostdout = tostdout == 1;
}

CCL_CAPI void CDECL cycles_set_logger(LOGGER_FUNC_CB logger_func_)
{
}

CCL_CAPI void CDECL cycles_f4_add(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a + b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

CCL_CAPI void CDECL cycles_f4_sub(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a - b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

CCL_CAPI void CDECL cycles_f4_mul(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a * b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

CCL_CAPI void CDECL cycles_f4_div(ccl::float4 a, ccl::float4 b, ccl::float4& res) {
	ccl::float4 r = a / b;
	res.x = r.x;
	res.y = r.y;
	res.z = r.z;
	res.w = r.w;
}

static void _tfm_copy(const ccl::Transform& source, ccl::Transform& target) {
	target.x.x = source.x.x;
	target.x.y = source.x.y;
	target.x.z = source.x.z;
	target.x.w = source.x.w;

	target.y.x = source.y.x;
	target.y.y = source.y.y;
	target.y.z = source.y.z;
	target.y.w = source.y.w;

	target.z.x = source.z.x;
	target.z.y = source.z.y;
	target.z.z = source.z.z;
	target.z.w = source.z.w;
}

CCL_CAPI void CDECL cycles_tfm_inverse(const ccl::Transform& t, ccl::Transform& res) {
	ccl::Transform r = ccl::transform_inverse(t);

	_tfm_copy(r, res);
}

CCL_CAPI void CDECL cycles_tfm_rotate_around_axis(float angle, const ccl::float3& axis, ccl::Transform& res)
{
	ccl::Transform r = ccl::transform_rotate(angle, axis);

	_tfm_copy(r, res);
}

CCL_CAPI void CDECL cycles_tfm_lookat(const ccl::float3& position, const ccl::float3& look, const ccl::float3& up, ccl::Transform &res)
{
	ccl::Transform r = ccl::transform_identity();
	r[0][3] = position.x;
	r[1][3] = position.y;
	r[2][3] = position.z;
	r[3][3] = 1.0f;

	ccl::float3 dir = ccl::normalize(look - position);


	ccl::float3 right = ccl::cross(ccl::normalize(up), dir); //, ccl::normalize(up));
	ccl::float3 new_up = ccl::cross(dir, right); //right, dir);

	r[0][0] = right.x;
	r[1][0] = right.y;
	r[2][0] = right.z;
	r[3][0] = 0.0f;
	r[0][1] = new_up.x;
	r[1][1] = new_up.y;
	r[2][1] = new_up.z;
	r[3][1] = 0.0f;
	r[0][2] = dir.x;
	r[1][2] = dir.y;
	r[2][2] = dir.z;
	r[3][2] = 0.0f;

	//r = ccl::transform_inverse(r);

	_tfm_copy(r, res);
}

CCL_CAPI void CDECL cycles_set_rhino_perlin_noise_table(int* data, unsigned int count)
{
	ccycles_rhino_perlin_noise_table.resize(count);

	for (int i = 0; i < count; i++)
	{
		ccycles_rhino_perlin_noise_table[i] = (float)data[i];
	}
}

CCL_CAPI void CDECL cycles_set_rhino_impulse_noise_table(float* data, unsigned int count)
{
	ccycles_rhino_impulse_noise_table.resize(count);

	for (int i = 0; i < count; i++)
	{
		ccycles_rhino_impulse_noise_table[i] = (float)data[i];
	}
}

CCL_CAPI void CDECL cycles_set_rhino_vc_noise_table(float* data, unsigned int count)
{
	ccycles_rhino_vc_noise_table.resize(count);

	for (int i = 0; i < count; i++)
	{
		ccycles_rhino_vc_noise_table[i] = (float)data[i];
	}
}

CCL_CAPI void CDECL cycles_set_rhino_aaltonen_noise_table(const int* data, unsigned int count)
{
	ccycles_rhino_aaltonen_noise_table.resize(count);

	for (int i = 0; i < count; i++)
	{
		ccycles_rhino_aaltonen_noise_table[i] = (float)data[i];
	}
}

#ifdef __cplusplus
}
#endif
