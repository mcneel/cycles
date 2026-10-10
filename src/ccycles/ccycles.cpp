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

#include "device/optix/device.h"
#include "util/log.h"
#include "util/path.h"
#include "ccycles.h"

/* Hold the device information found on the system after initialisation. */
std::vector<ccl::DeviceInfo> devices;
std::vector<ccl::DeviceInfo> multi_devices;

ccl::vector<float> ccycles_rhino_perlin_noise_table;
ccl::vector<float> ccycles_rhino_impulse_noise_table;
ccl::vector<float> ccycles_rhino_vc_noise_table;
ccl::vector<float> ccycles_rhino_aaltonen_noise_table;

/* Need to initialise only once :) */
bool initialised{ false };
Logger logger;

void cycles_path_init(const char* path, const char* user_path)
{
	ccl::path_init(std::string(path), std::string(user_path));
}

/* Cycles' own log is discarded without a sink; with CCYCLES_DIAG_LOG set it goes
 * to ccycles_diag. */
static void ccycles_cycles_log(const ccl::LogLevel level,
                               const char *file_line,
                               const char *func,
                               const char *msg)
{
	ccycles_diag("cycles[%s] %s: %s\n", ccl::log_level_to_string(level),
	             func == nullptr ? "" : func, msg == nullptr ? "" : msg);
}

void cycles_initialise(unsigned int mask)
{
	if (!initialised) {
		const char *diag = getenv("CCYCLES_DIAG_LOG");
		if (diag != nullptr && diag[0] != 0) {
			ccl::log_init(ccycles_cycles_log);
			const char *lvl = getenv("CCYCLES_LOG_LEVEL");
			ccl::log_level_set(lvl != nullptr && lvl[0] != 0 ? ccl::log_string_to_level(lvl)
			                                                 : ccl::LOG_LEVEL_INFO);
		}
		devices.clear();
		multi_devices.clear();
		devices = ccl::Device::available_devices(mask);
		initialised = true;
	}
}

unsigned int cycles_failed_gpus_mask()
{
	return ccl::Device::failed_gpus_mask();
}

const char* cycles_gpu_init_error(unsigned int device_type)
{
	static std::string last_error;
	last_error = ccl::Device::gpu_init_error(static_cast<ccl::DeviceType>(device_type));
	return last_error.c_str();
}

int cycles_optix_init_result()
{
	return ccl::device_optix_init_result();
}

int cycles_optix_minimum_driver()
{
	return ccl::device_optix_minimum_driver();
}

void cycles_shutdown()
{
	if (!initialised) {
		return;
	}

	_cleanup_sessions();
}

void cycles_log_to_stdout(int tostdout)
{
	logger.tostdout = tostdout == 1;
}

void cycles_set_rhino_perlin_noise_table(int* data, unsigned int count)
{
	ccycles_rhino_perlin_noise_table.resize(count);

	for (int i = 0; i < count; i++)
	{
		ccycles_rhino_perlin_noise_table[i] = (float)data[i];
	}
}

void cycles_set_rhino_impulse_noise_table(float* data, unsigned int count)
{
	ccycles_rhino_impulse_noise_table.resize(count);

	for (int i = 0; i < count; i++)
	{
		ccycles_rhino_impulse_noise_table[i] = (float)data[i];
	}
}

void cycles_set_rhino_vc_noise_table(float* data, unsigned int count)
{
	ccycles_rhino_vc_noise_table.resize(count);

	for (int i = 0; i < count; i++)
	{
		ccycles_rhino_vc_noise_table[i] = (float)data[i];
	}
}

void cycles_set_rhino_aaltonen_noise_table(const int* data, unsigned int count)
{
	ccycles_rhino_aaltonen_noise_table.resize(count);

	for (int i = 0; i < count; i++)
	{
		ccycles_rhino_aaltonen_noise_table[i] = (float)data[i];
	}
}
