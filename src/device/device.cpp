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

#include <stdlib.h>
#include <string.h>

#include "device/device.h"
#include "device/device_intern.h"

#include "util/util_debug.h"
#include "util/util_foreach.h"
#include "util/util_half.h"
#include "util/util_math.h"
#include "util/util_opengl.h"
#include "util/util_time.h"
#include "util/util_types.h"
#include "util/util_vector.h"
#include "util/util_string.h"

CCL_NAMESPACE_BEGIN

bool Device::need_types_update = true;
bool Device::need_devices_update = true;
vector<DeviceType> Device::types;
vector<DeviceInfo> Device::devices;

/* Device Requested Features */

std::ostream& operator <<(std::ostream &os,
                          const DeviceRequestedFeatures& requested_features)
{
	os << "Experimental features: "
	   << (requested_features.experimental ? "On" : "Off") << std::endl;
	os << "Max closure count: " << requested_features.max_closure << std::endl;
	os << "Max nodes group: " << requested_features.max_nodes_group << std::endl;
	/* TODO(sergey): Decode bitflag into list of names. */
	os << "Nodes features: " << requested_features.nodes_features << std::endl;
	os << "Use Hair: "
	   << string_from_bool(requested_features.use_hair) << std::endl;
	os << "Use Object Motion: "
	   << string_from_bool(requested_features.use_object_motion) << std::endl;
	os << "Use Camera Motion: "
	   << string_from_bool(requested_features.use_camera_motion) << std::endl;
	os << "Use Baking: "
	   << string_from_bool(requested_features.use_baking) << std::endl;
	os << "Use Subsurface: "
	   << string_from_bool(requested_features.use_subsurface) << std::endl;
	os << "Use Volume: "
	   << string_from_bool(requested_features.use_volume) << std::endl;
	os << "Use Branched Integrator: "
	   << string_from_bool(requested_features.use_integrator_branched) << std::endl;
	os << "Use Patch Evaluation: "
	   << string_from_bool(requested_features.use_patch_evaluation) << std::endl;
	os << "Use Transparent Shadows: "
	   << string_from_bool(requested_features.use_transparent) << std::endl;
	os << "Use Principled BSDF: "
	   << string_from_bool(requested_features.use_principled) << std::endl;
	os << "Use Denoising: "
	   << string_from_bool(requested_features.use_denoising) << std::endl;
	return os;
}

/* Device */

Device::~Device()
{
	if(!background && vertex_buffer != 0) {
		glDeleteBuffers(1, &vertex_buffer);
	}
}

void Device::pixels_alloc(device_memory& mem)
{
	mem_alloc("pixels", mem, MEM_READ_WRITE);
}

void Device::pixels_copy_from(device_memory& mem, int y, int w, int h)
{
	if(mem.data_type == TYPE_HALF)
		mem_copy_from(mem, y, w, h, sizeof(half4));
	else
		mem_copy_from(mem, y, w, h, sizeof(uchar4));
}

void Device::pixels_free(device_memory& mem)
{
	mem_free(mem);
}

void GetGLViewportData(GLint width, GLint height, GLint& vp_offset_x, GLint& vp_offset_y, GLint& vp_width, GLint& vp_height)
{
	GLint vp_dims[4] = { 0 };
	glGetIntegerv(GL_VIEWPORT, vp_dims);

	vp_offset_x = vp_dims[0];
	vp_offset_y = vp_dims[1];
	vp_width    = vp_dims[2];
	vp_height   = vp_dims[3];

	// If the width of the GL_VIEWPORT is smaller than the width of the
	// render, then we are dealing with a Rhino Detail which has been
	// moved partially outside the window.
	if (vp_width < width)
	{
		// If the x offset is 0, it has been clamped to 0 due to being partially outside
		// the window to the left.
		if (vp_offset_x == 0)
		{
			// We want to express the offset as negative coordinates instead of clamping to 0.
			vp_offset_x = vp_width - width;
		}
		// In all cases we want the width to be the render width.
		vp_width = width;
	}

	// See comments above.
	if (vp_height < height)
	{
		if (vp_offset_y == 0)
		{
			vp_offset_y = vp_height - height;
		}
		vp_height = height;
	}
}

void Device::draw_pixels(device_memory& rgba, int y, int w, int h, int dx, int dy, int width, int height, int full_width, int full_height, bool transparent,
	const DeviceDrawParams &draw_params)
{
	pixels_copy_from(rgba, y, w, h);

	if(transparent) {
		glEnable(GL_BLEND);
		glBlendFunc(GL_SRC_ALPHA, GL_ONE_MINUS_SRC_ALPHA);
	}

	if(rgba.data_type == TYPE_HALF) {
		GLhalf *data_pointer = (GLhalf*)rgba.data_pointer;

		data_pointer += 4*y*w;

		GLuint texid;
		glGenTextures(1, &texid);
		glActiveTexture(GL_TEXTURE0);
		glBindTexture(GL_TEXTURE_2D, texid);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA16F_ARB, w, h, 0, GL_RGBA, GL_HALF_FLOAT, data_pointer);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);

		// We need to know the GL_VIEWPORT rect information to be able to
		// calculate the correct viewport uv-coordinates when using a Rhino Detail.
		GLint vp_offset_x, vp_offset_y, vp_width, vp_height;
		GetGLViewportData(full_width, full_height, vp_offset_x, vp_offset_y, vp_width, vp_height);

		GLint tex     = glGetUniformLocation(draw_params.program, "tex");
		GLint subsize = glGetUniformLocation(draw_params.program, "subsize");
		GLint alpha   = glGetUniformLocation(draw_params.program, "alpha");
		GLint vp_rect = glGetUniformLocation(draw_params.program, "vp_rect");

		glUniform1i(tex, 0);
		glUniform4f(subsize, (float)dx, (float)dy, (float)width, (float)height);
		glUniform1f(alpha, draw_params.alpha);
		glUniform4f(vp_rect, (float)vp_offset_x, (float)vp_offset_y, (float)vp_width, (float)vp_height);

		GLuint temp_vao = 0;
		glGenVertexArrays(1, &temp_vao);
		glBindVertexArray(temp_vao);
		GLuint temp_vbo = 0;
		glGenBuffers(1, &temp_vbo);

		static const float vertices[] = { -1,-1, 1,-1, 1,1, -1,1 };
		glBindBuffer(GL_ARRAY_BUFFER, temp_vbo);
		glBufferData(GL_ARRAY_BUFFER, sizeof(float) * 8, vertices, GL_STREAM_DRAW);

		glEnableVertexAttribArray(0);
		glVertexAttribPointer(0, 2, GL_FLOAT, false, 0, nullptr);

		glDrawArrays(GL_TRIANGLE_FAN, 0, 4);

		glDisableVertexAttribArray(0);
		glBindBuffer(GL_ARRAY_BUFFER, 0);
		glDeleteBuffers(1, &temp_vbo);
		glBindVertexArray(0);
		glDeleteVertexArrays(1, &temp_vao);
		glBindTexture(GL_TEXTURE_2D, 0);
		glDeleteTextures(1, &texid);

#if OLDSTUFF
		/* for multi devices, this assumes the inefficient method that we allocate
		 * all pixels on the device even though we only render to a subset */
		GLhalf *data_pointer = (GLhalf*)rgba.data_pointer;
		float vbuffer[16], *basep;
		float *vp = NULL;

		data_pointer += 4*y*w;

		/* draw half float texture, GLSL shader for display transform assumed to be bound */
		GLuint texid;
		glGenTextures(1, &texid);
		glBindTexture(GL_TEXTURE_2D, texid);
		glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA16F_ARB, w, h, 0, GL_RGBA, GL_HALF_FLOAT, data_pointer);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
		glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);

		glEnable(GL_TEXTURE_2D);

		if(draw_params.bind_display_space_shader_cb) {
			draw_params.bind_display_space_shader_cb();
		}

		if(GLEW_VERSION_1_5) {
			if(!vertex_buffer)
				glGenBuffers(1, &vertex_buffer);

			glBindBuffer(GL_ARRAY_BUFFER, vertex_buffer);
			/* invalidate old contents - avoids stalling if buffer is still waiting in queue to be rendered */
			glBufferData(GL_ARRAY_BUFFER, 16 * sizeof(float), NULL, GL_STREAM_DRAW);

			vp = (float *)glMapBuffer(GL_ARRAY_BUFFER, GL_WRITE_ONLY);

			basep = NULL;
		}
		else {
			basep = vbuffer;
			vp = vbuffer;
		}

		if(vp) {
			/* texture coordinate - vertex pair */
			vp[0] = 0.0f;
			vp[1] = 0.0f;
			vp[2] = dx;
			vp[3] = dy;

			vp[4] = 1.0f;
			vp[5] = 0.0f;
			vp[6] = (float)width + dx;
			vp[7] = dy;

			vp[8] = 1.0f;
			vp[9] = 1.0f;
			vp[10] = (float)width + dx;
			vp[11] = (float)height + dy;

			vp[12] = 0.0f;
			vp[13] = 1.0f;
			vp[14] = dx;
			vp[15] = (float)height + dy;

			if(vertex_buffer)
				glUnmapBuffer(GL_ARRAY_BUFFER);
		}

		glTexCoordPointer(2, GL_FLOAT, 4 * sizeof(float), basep);
		glVertexPointer(2, GL_FLOAT, 4 * sizeof(float), ((char *)basep) + 2 * sizeof(float));

		glEnableClientState(GL_VERTEX_ARRAY);
		glEnableClientState(GL_TEXTURE_COORD_ARRAY);

		glDrawArrays(GL_TRIANGLE_FAN, 0, 4);

		glDisableClientState(GL_TEXTURE_COORD_ARRAY);
		glDisableClientState(GL_VERTEX_ARRAY);

		if(vertex_buffer) {
			glBindBuffer(GL_ARRAY_BUFFER, 0);
		}

		if(draw_params.unbind_display_space_shader_cb) {
			draw_params.unbind_display_space_shader_cb();
		}

		glBindTexture(GL_TEXTURE_2D, 0);
		glDisable(GL_TEXTURE_2D);
		glDeleteTextures(1, &texid);
#endif
	}
	else {
		/* fallback for old graphics cards that don't support GLSL, half float,
		 * and non-power-of-two textures */
#if OLDSTUFF
		glPixelZoom((float)width/(float)w, (float)height/(float)h);
		glRasterPos2f(dx, dy);

		uint8_t *pixels = (uint8_t*)rgba.data_pointer;

		pixels += 4*y*w;

		glDrawPixels(w, h, GL_RGBA, GL_UNSIGNED_BYTE, pixels);

		glRasterPos2f(0.0f, 0.0f);
		glPixelZoom(1.0f, 1.0f);
#endif
	}

	if(transparent)
		glDisable(GL_BLEND);
}

Device *Device::create(DeviceInfo& info, Stats &stats, bool background)
{
	Device *device;

	switch(info.type) {
		case DEVICE_CPU:
			device = device_cpu_create(info, stats, background);
			break;
#ifdef WITH_CUDA
		case DEVICE_CUDA:
			if(device_cuda_init())
				device = device_cuda_create(info, stats, background);
			else
				device = NULL;
			break;
#endif
#ifdef WITH_MULTI
		case DEVICE_MULTI:
			device = device_multi_create(info, stats, background);
			break;
#endif
#ifdef WITH_NETWORK
		case DEVICE_NETWORK:
			device = device_network_create(info, stats, "127.0.0.1");
			break;
#endif
#ifdef WITH_OPENCL
		case DEVICE_OPENCL:
			if(device_opencl_init())
				device = device_opencl_create(info, stats, background);
			else
				device = NULL;
			break;
#endif
		default:
			return NULL;
	}

	return device;
}

DeviceType Device::type_from_string(const char *name)
{
	if(strcmp(name, "CPU") == 0)
		return DEVICE_CPU;
	else if(strcmp(name, "CUDA") == 0)
		return DEVICE_CUDA;
	else if(strcmp(name, "OPENCL") == 0)
		return DEVICE_OPENCL;
	else if(strcmp(name, "NETWORK") == 0)
		return DEVICE_NETWORK;
	else if(strcmp(name, "MULTI") == 0)
		return DEVICE_MULTI;

	return DEVICE_NONE;
}

string Device::string_from_type(DeviceType type)
{
	if(type == DEVICE_CPU)
		return "CPU";
	else if(type == DEVICE_CUDA)
		return "CUDA";
	else if(type == DEVICE_OPENCL)
		return "OPENCL";
	else if(type == DEVICE_NETWORK)
		return "NETWORK";
	else if(type == DEVICE_MULTI)
		return "MULTI";

	return "";
}

vector<DeviceType>& Device::available_types()
{
	if(need_types_update) {
		types.clear();
		types.push_back(DEVICE_CPU);

#ifdef WITH_CUDA
		if(device_cuda_init())
			types.push_back(DEVICE_CUDA);
#endif

#ifdef WITH_OPENCL
		if(device_opencl_init())
			types.push_back(DEVICE_OPENCL);
#endif

#ifdef WITH_NETWORK
		types.push_back(DEVICE_NETWORK);
#endif

		need_types_update = false;
	}

	return types;
}

vector<DeviceInfo>& Device::available_devices()
{
	if(need_devices_update) {
		devices.clear();
#ifdef WITH_CUDA
		if(device_cuda_init())
			device_cuda_info(devices);
#endif

#ifdef WITH_OPENCL
		if(device_opencl_init())
			device_opencl_info(devices);
#endif

		device_cpu_info(devices);

#ifdef WITH_NETWORK
		device_network_info(devices);
#endif

		need_devices_update = false;
	}

	return devices;
}

string Device::device_capabilities()
{
	string capabilities = "CPU device capabilities: ";
	capabilities += device_cpu_capabilities() + "\n";
#ifdef WITH_CUDA
	if(device_cuda_init()) {
		capabilities += "\nCUDA device capabilities:\n";
		capabilities += device_cuda_capabilities();
	}
#endif

#ifdef WITH_OPENCL
	if(device_opencl_init()) {
		capabilities += "\nOpenCL device capabilities:\n";
		capabilities += device_opencl_capabilities();
	}
#endif

	return capabilities;
}

DeviceInfo Device::get_multi_device(vector<DeviceInfo> subdevices)
{
	assert(subdevices.size() > 1);

	DeviceInfo info;
	info.type = DEVICE_MULTI;
	info.id = "MULTI";
	info.description = "Multi Device";
	info.multi_devices = subdevices;
	info.num = 0;

	info.has_bindless_textures = true;
	info.pack_images = false;
	foreach(DeviceInfo &device, subdevices) {
		assert(device.type == info.multi_devices[0].type);

		info.pack_images |= device.pack_images;
		info.has_bindless_textures &= device.has_bindless_textures;
	}

	return info;
}

void Device::tag_update()
{
	need_types_update = true;
	need_devices_update = true;
}

void Device::free_memory()
{
	need_types_update = true;
	need_devices_update = true;
	types.free_memory();
	devices.free_memory();
}


device_sub_ptr::device_sub_ptr(Device *device, device_memory& mem, int offset, int size, MemoryType type)
 : device(device)
{
	ptr = device->mem_alloc_sub_ptr(mem, offset, size, type);
}

device_sub_ptr::~device_sub_ptr()
{
	device->mem_free_sub_ptr(ptr);
}

CCL_NAMESPACE_END
