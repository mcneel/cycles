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

/* Hold all created session parameters. */
std::vector<ccl::SessionParams*> session_params;

#define SESSION_PARAM_BOOL(session_params_id, varname) \
	PARAM_BOOL(session_params, session_params_id, varname)

#define SESSION_PARAM(session_params_id, varname) \
	PARAM(session_params, session_params_id, varname)

#define SESSION_PARAM_CAST(session_params_id, typecast, varname) \
	PARAM_CAST(session_params, session_params_id, typecast, varname)

unsigned int cycles_session_params_create(unsigned int client_id, unsigned int device_id)
{
	ccl::SessionParams* params = new ccl::SessionParams();

	GETDEVICE(params->device, device_id);
	session_params.push_back(params);
	logger.logit(client_id, "Created session parameters for device ", device_id);

	return (unsigned int)(session_params.size() - 1);
}

void cycles_session_params_set_device(unsigned int client_id, unsigned int session_params_id, unsigned int device)
{
	if (session_params_id < session_params.size()) {
		GETDEVICE(session_params[session_params_id]->device, device)
	}
}

void cycles_session_params_set_background(unsigned int client_id, unsigned int session_params_id, unsigned int background)
{
	SESSION_PARAM_BOOL(session_params_id, background)
}

void cycles_session_params_set_progressive_refine(unsigned int client_id, unsigned int session_params_id, unsigned int progressive_refine)
{
    // TODO: XXXX no longer available
	//SESSION_PARAM_BOOL(session_params_id, progressive_refine)
}
void cycles_session_params_set_output_path(unsigned int client_id, unsigned int session_params_id, const char *output_path)
{
	/*if (session_params_id < session_params.size()) {
		session_params[session_params_id].output_path = std::string(output_path);
		logger.logit(client_id, "Set output_path to: ", session_params[session_params_id].output_path);
	}*/
}

void cycles_session_params_set_progressive(unsigned int client_id, unsigned int session_params_id, unsigned int progressive)
{
    // TODO: XXXX no longer available
	//SESSION_PARAM_BOOL(session_params_id, progressive)
}

void cycles_session_params_set_experimental(unsigned int client_id, unsigned int session_params_id, unsigned int experimental)
{
	SESSION_PARAM_BOOL(session_params_id, experimental)
}

void cycles_session_params_set_samples(unsigned int client_id, unsigned int session_params_id, int samples)
{
	SESSION_PARAM(session_params_id, samples);
}

void cycles_session_params_set_tile_size(unsigned int client_id, unsigned int session_params_id, unsigned int x, unsigned int y)
{
	if (session_params_id < session_params.size()) {
        // TODO: XXXX perculate change to wrapped function, only x should remain
		session_params[session_params_id]->tile_size = x;
	}
}

void cycles_session_params_set_tile_order(unsigned int client_id, unsigned int session_params_id, unsigned int tile_order)
{
    // TODO: XXXX no longer exists
	//SESSION_PARAM_CAST(session_params_id, ccl::TileOrder, tile_order);
}

void cycles_session_params_set_start_resolution(unsigned int client_id, unsigned int session_params_id, int start_resolution)
{
    // TODO: XXXX no longer exists
	//SESSION_PARAM(session_params_id, start_resolution);
}

void cycles_session_params_set_threads(unsigned int client_id, unsigned int session_params_id, unsigned int threads)
{
	SESSION_PARAM(session_params_id, threads);
}
void cycles_session_params_set_display_buffer_linear(unsigned int client_id, unsigned int session_params_id, unsigned int display_buffer_linear)
{
    // TODO: XXXX no longer exists
	//SESSION_PARAM_BOOL(session_params_id, display_buffer_linear)
}
void cycles_session_params_set_skip_linear_to_srgb_conversion(unsigned int client_id, unsigned int session_params_id, unsigned int skip_linear_to_srgb_conversion)
{
	// SESSION_PARAM_BOOL(session_params_id, skip_linear_to_srgb_conversion)
}
void cycles_session_params_set_cancel_timeout(unsigned int client_id, unsigned int session_params_id, double cancel_timeout)
{
    // TODO: XXXX no longer exists
	//SESSION_PARAM(session_params_id, cancel_timeout);
}
void cycles_session_params_set_reset_timeout(unsigned int client_id, unsigned int session_params_id, double reset_timeout)
{
    // TODO: XXXX no longer exists
	//SESSION_PARAM(session_params_id, reset_timeout);
}
void cycles_session_params_set_text_timeout(unsigned int client_id, unsigned int session_params_id, double text_timeout)
{
    // TODO: XXXX no longer exists
	//SESSION_PARAM(session_params_id, text_timeout);
}

/* TODO [NATHANLOOK] see if progressive_update_timeout needs to be APIfied. */

void cycles_session_params_set_shadingsystem(unsigned int client_id, unsigned int session_params_id, unsigned int shadingsystem)
{
	SESSION_PARAM_CAST(session_params_id, ccl::ShadingSystem, shadingsystem);
}

void cycles_session_params_set_pixel_size(unsigned int client_id, unsigned int session_params_id, unsigned int pixel_size)
{
	SESSION_PARAM(session_params_id, pixel_size);
}
