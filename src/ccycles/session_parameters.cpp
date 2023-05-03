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

#include <unordered_set>
#include <algorithm>

#include "internal_types.h"

/* Hold all created session parameters. */
std::unordered_set<ccl::SessionParams*> session_params;

#define SESSION_PARAM_BOOL(session_params_id, varname) \
	PARAM_BOOL(session_params, session_params_id, varname)

#define SESSION_PARAM(session_params_id, varname) \
	PARAM(session_params, session_params_id, varname)

#define SESSION_PARAM_CAST(session_params_id, typecast, varname) \
	PARAM_CAST(session_params, session_params_id, typecast, varname)

ccl::SessionParams* cycles_session_params_create(unsigned int device_id)
{
	ccl::SessionParams* params = new ccl::SessionParams();

	GETDEVICE(params->device, device_id);
	session_params.insert(params);
	logger.logit("Created session parameters for device ", device_id);

	return params;
}

void cycles_session_params_set_device(ccl::SessionParams* session_params_id, unsigned int device)
{
	if (auto search = session_params.find(session_params_id); search != session_params.end()) {
		GETDEVICE((*search)->device, device)
	}
}

void cycles_session_params_set_background(ccl::SessionParams* session_params_id, unsigned int background)
{
	if (auto search = session_params.find(session_params_id); search != session_params.end()) {
		(*search)->background = background != 0;
	}
}

void cycles_session_params_set_progressive_refine(ccl::SessionParams* session_params_id, unsigned int progressive_refine)
{
		// TODO: XXXX no longer available
	//SESSION_PARAM_BOOL(session_params_id, progressive_refine)
}
void cycles_session_params_set_output_path(ccl::SessionParams* session_params_id, const char *output_path)
{
	/*if (session_params_id < session_params.size()) {
		session_params[session_params_id].output_path = std::string(output_path);
		logger.logit("Set output_path to: ", session_params[session_params_id].output_path);
	}*/
}

void cycles_session_params_set_progressive(ccl::SessionParams* session_params_id, unsigned int progressive)
{
		// TODO: XXXX no longer available
	//SESSION_PARAM_BOOL(session_params_id, progressive)
}

void cycles_session_params_set_experimental(ccl::SessionParams* session_params_id, unsigned int experimental)
{
	if (auto search = session_params.find(session_params_id); search != session_params.end()) {
		(*search)->experimental = experimental != 0;
	}
}

void cycles_session_params_set_samples(ccl::SessionParams* session_params_id, int samples)
{
	if (auto search = session_params.find(session_params_id); search != session_params.end()) {
		(*search)->samples = samples;
	}
}

void cycles_session_params_set_tile_size(ccl::SessionParams* session_params_id, unsigned int tile_size)
{
	if (auto search = session_params.find(session_params_id); search != session_params.end()) {
		(*search)->tile_size = tile_size;
	}
}

void cycles_session_params_set_tile_order(ccl::SessionParams* session_params_id, unsigned int tile_order)
{
		// TODO: XXXX no longer exists
	//SESSION_PARAM_CAST(session_params_id, ccl::TileOrder, tile_order);
}

void cycles_session_params_set_start_resolution(ccl::SessionParams* session_params_id, int start_resolution)
{
		// TODO: XXXX no longer exists
	//SESSION_PARAM(session_params_id, start_resolution);
}

void cycles_session_params_set_threads(ccl::SessionParams* session_params_id, unsigned int threads)
{
	if (auto search = session_params.find(session_params_id); search != session_params.end()) {
		(*search)->threads = threads;
	}
}
void cycles_session_params_set_display_buffer_linear(ccl::SessionParams* session_params_id, unsigned int display_buffer_linear)
{
		// TODO: XXXX no longer exists
	//SESSION_PARAM_BOOL(session_params_id, display_buffer_linear)
}
void cycles_session_params_set_skip_linear_to_srgb_conversion(ccl::SessionParams* session_params_id, unsigned int skip_linear_to_srgb_conversion)
{
	// SESSION_PARAM_BOOL(session_params_id, skip_linear_to_srgb_conversion)
}
void cycles_session_params_set_cancel_timeout(ccl::SessionParams* session_params_id, double cancel_timeout)
{
		// TODO: XXXX no longer exists
	//SESSION_PARAM(session_params_id, cancel_timeout);
}
void cycles_session_params_set_reset_timeout(ccl::SessionParams* session_params_id, double reset_timeout)
{
		// TODO: XXXX no longer exists
	//SESSION_PARAM(session_params_id, reset_timeout);
}
void cycles_session_params_set_text_timeout(ccl::SessionParams* session_params_id, double text_timeout)
{
		// TODO: XXXX no longer exists
	//SESSION_PARAM(session_params_id, text_timeout);
}

/* TODO [NATHANLOOK] see if progressive_update_timeout needs to be APIfied. */

void cycles_session_params_set_shadingsystem(ccl::SessionParams* session_params_id, unsigned int shadingsystem)
{
	if (auto search = session_params.find(session_params_id); search != session_params.end()) {
		(*search)->shadingsystem = (ccl::ShadingSystem)shadingsystem;
	}
}

void cycles_session_params_set_pixel_size(ccl::SessionParams* session_params_id, unsigned int pixel_size)
{
	if (auto search = session_params.find(session_params_id); search != session_params.end()) {
		(*search)->pixel_size = (ccl::ShadingSystem)pixel_size;
	}
}
