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

#pragma once


#include <algorithm>
#include <unordered_set>
#include <vector>
#include <chrono>
#include <thread>
#include <string>
#include <functional>

#pragma warning ( push )

#pragma warning ( disable : 4244 )

#include "scene/background.h"
#include "scene/camera.h"
#include "util/colorspace.h"
#include "device/device.h"
#include "scene/film.h"
#include "scene/shader_graph.h"
#include "scene/integrator.h"
#include "scene/light.h"
#include "scene/mesh.h"
#include "scene/shader_nodes.h"
#include "scene/rhino_shader_nodes.h"
#include "scene/object.h"
#include "scene/scene.h"
#include "session/session.h"
#include "session/display_driver.h"
#include "session/output_driver.h"
#include "scene/shader.h"

#include "util/color.h"
#include <functional>
#include "util/progress.h"
#include "util/string.h"
#include "util/thread.h"

#pragma warning ( pop )

#include "ccycles.h"

#include <cstdarg>
#include <cstdio>
#include <cstring>
#include <cstdlib>

#ifdef WIN32
/* Declared here to keep windows.h (min/max macros and more) out of every ccycles
 * translation unit. Matches WINBASEAPI exactly, so including windows.h too is fine. */
extern "C" __declspec(dllimport) void __stdcall OutputDebugStringA(const char *lpOutputString);
#endif

/* Rhino has no console, so diagnostics go to OutputDebugString (VS Output window
 * or DebugView) as well as stderr, which the standalone test executables do have. */
static inline void ccycles_diag(const char *fmt, ...)
{
	/* Silent unless asked for; read once, as this runs per object and per pass. */
	static const bool want_diag = getenv("CCYCLES_DIAG_LOG") != nullptr;
	if (!want_diag) {
		return;
	}

	/* One buffer for prefix and message: separate OutputDebugStringA calls are
	 * separate records, and a listener can miss one. */
	char msg[1024] = "ccycles: ";
	const size_t prefix_len = strlen(msg);
	va_list args;
	va_start(args, fmt);
	vsnprintf(msg + prefix_len, sizeof(msg) - prefix_len, fmt, args);
	va_end(args);

	fprintf(stderr, "%s", msg);
#ifdef WIN32
	OutputDebugStringA(msg);
#endif

	/* OutputDebugString drops records when the listener lags, which loses bursts
	 * such as a scene dump. Set CCYCLES_DIAG_LOG to a path to get all of them. */
	static FILE *diag_file = nullptr;
	static bool diag_file_tried = false;
	if (!diag_file_tried) {
		diag_file_tried = true;
		const char *path = getenv("CCYCLES_DIAG_LOG");
		if (path != nullptr && path[0] != 0) {
			diag_file = fopen(path, "a");
		}
	}
	if (diag_file != nullptr) {
		fputs(msg, diag_file);
		fflush(diag_file);
	}
}

#define MULTIDEVICEOFFSET 100000
#define ISMULTIDEVICE(id) (id>=MULTIDEVICEOFFSET)
#define MULTIDEVICEIDX(id) (id-MULTIDEVICEOFFSET)
/* Bounds-checked, as an undetected device type has no entries: out of range
 * leaves puthere untouched and logs why. */
#define GETDEVICE(puthere, id) \
	if (ISMULTIDEVICE(id)) { \
		const size_t _gd_idx = (size_t)MULTIDEVICEIDX(id); \
		if (_gd_idx < multi_devices.size()) { \
			(puthere) = multi_devices[_gd_idx]; \
		} \
		else { \
			ccycles_diag(\
			        "GETDEVICE: multi-device %zu requested, only %zu available\n", \
			        _gd_idx, \
			        multi_devices.size()); \
		} \
	} \
	else if ((size_t)(id) < devices.size()) { \
		(puthere) = devices[id]; \
	} \
	else { \
		ccycles_diag(\
		        "GETDEVICE: device %zu requested, only %zu available - was the requested device type detected?\n", \
		        (size_t)(id), \
		        devices.size()); \
	}

class CCSession;

/* Simple class to help with debug logging. */
class Logger {
public:
	bool tostdout{ false };

	/* Variadic: each call streams the head into logger_msg. */
	template<typename T, typename... Tail>
	void logit(T head, Tail... tail) {
		m.lock();
		/* reset logger_msg */
		logger_msg.str("");

		/* Timestamp without the trailing newline std::ctime adds. */
		auto t = std::chrono::system_clock::now();
		std::time_t ts = std::chrono::system_clock::to_time_t(t);
		auto tsstr = std::string{std::ctime(&ts)};
		tsstr = tsstr.substr(0, tsstr.size() - 1);

		/* Timestamp and head, then the tail. */
		logger_msg << tsstr << ": " << head;
		logit_followup( tail...);
	}

private:
	template<typename T, typename... Tail>
	void logit_followup(T head, Tail... tail) {
		logger_msg << head;
		logit_followup(tail...);
	}

	/* End of the recursion: emit the message and release the mutex logit took. */
	void logit_followup() {
#if defined(DEBUG)
		// also print to std::cout if wanted
		if (tostdout) std::cout << logger_msg.str().c_str() << std::endl;
#endif
		m.unlock();
	}

	std::stringstream logger_msg;

	std::mutex m;
};

/* Usage: logger.logit("This is a message", var, var2, " and some more", var3); */
extern Logger logger;

class CCyclesPassOutput {
	public:
		CCyclesPassOutput();

	public:
		void lock();
		void unlock();

		ccl::PassType get_pass_type() const;
		void set_pass_type(ccl::PassType value);

		int get_width() const;
		void set_width(int width);

		int get_height() const;
		void set_height(int height);

		std::vector<float> &pixels();

		int get_pixel_size() const;
		void set_pixel_size(int pixel_size);

	private:
		std::mutex m_lock;
		ccl::PassType m_pass_type;
		int m_width;
		int m_height;
		int m_pixel_size;
		std::vector<float> m_pixels;
};

class CCyclesOutputDriver : public ccl::OutputDriver {
	public:
		typedef std::function<void(const std::string &)> LogFunction;

		CCyclesOutputDriver(std::vector<std::unique_ptr<CCyclesPassOutput>> *full_passes,
							LogFunction log,
							CCSession* ccsession);
		virtual ~CCyclesOutputDriver();

		virtual void write_render_tile(const Tile &tile) override;
		virtual bool update_render_tile(const Tile & /* tile */) override;

	protected:
		bool write_or_update_render_tile(const Tile &tile);

		LogFunction log_;

		CCSession* ccsession_;

		std::vector<std::vector<float>> tile_passes;
		std::vector<std::unique_ptr<CCyclesPassOutput>> *full_passes;
};

class CCyclesDisplayDriver : public ccl::DisplayDriver {
	public:
		typedef std::function<void(const std::string &)> LogFunction;

		CCyclesDisplayDriver(std::vector<std::unique_ptr<CCyclesPassOutput>> *passes,
							 LogFunction log);
		virtual ~CCyclesDisplayDriver();

		virtual void next_tile_begin() override;
		virtual bool update_begin(const Params &params, int width, int height) override;
		virtual void update_end() override;
		virtual ccl::half4 *map_texture_buffer() override;
		virtual void unmap_texture_buffer() override;
		/* 5.2 renamed DisplayDriver::clear() to zero(). */
		virtual void zero() override;
		virtual void draw(const Params &params) override;

// Optional

		/* 5.2 replaced graphics_interop_get() with graphics_interop_get_device(). */
		virtual ccl::GraphicsInteropDevice graphics_interop_get_device() override;
		virtual void graphics_interop_activate() override;
		virtual void graphics_interop_deactivate() override;

	protected:
		LogFunction log_;

		std::vector<ccl::half4> pixels_half4;

		std::vector<std::unique_ptr<CCyclesPassOutput>> *passes;
};

/* Rhino light handle. In 5.2 the type picks the concrete Light class and placement
 * is an Object transform, but the C API sets the type after creation. This buffers
 * the properties and builds the light and its Object on the first setter, so set
 * the type first; a later type change keeps the class. Opaque IntPtr to csycles. flush() composes the transform from the pre-5.2
 * co/dir/axisu/axisv semantics. */
struct CCyclesLight {
	ccl::Session *session{nullptr};
	ccl::Light *light{nullptr};
	ccl::Object *object{nullptr};
	ccl::Shader *shader{nullptr};

	ccl::LightType type{ccl::LIGHT_POINT};
	bool type_set{false};

	ccl::float3 co{ccl::zero_float3()};
	ccl::float3 dir{ccl::make_float3(0.0f, 0.0f, -1.0f)};
	ccl::float3 axisu{ccl::make_float3(1.0f, 0.0f, 0.0f)};
	ccl::float3 axisv{ccl::make_float3(0.0f, 1.0f, 0.0f)};

	float size{0.0f};
	float angle{0.009180f};
	float spot_angle{0.0f};
	float spot_smooth{0.0f};
	float sizeu{0.0f};
	float sizev{0.0f};
	int max_bounces{0};
	bool cast_shadow{true};
	bool use_mis{true};

	/* Creates the concrete light plus its Object, or updates them. */
	void flush();
};

class CCSession final {
public:
	unsigned int id{ 0 };
	ccl::SessionParams params;
	ccl::SceneParams scene_params;
	ccl::Session* session = nullptr;

	int width{ 0 };
	int height{ 0 };

	ccl::BufferParams buffer_params;

	std::vector<std::unique_ptr<CCyclesPassOutput>> passes;

	/* Create a new CCSession, initialise all necessary memory. */
	static CCSession* create(int width, int height, unsigned int buffer_stride);

	~CCSession() {
		if(session != nullptr)
		{
			delete session;
			session = nullptr;
		}
	}

protected:
	/* Protected constructor, use CCSession::create to create a new CCSession. */
	CCSession()
	{	}
};

/* data */
extern std::vector<ccl::DeviceInfo> devices;
extern std::vector<ccl::DeviceInfo> multi_devices;
extern std::unordered_set<ccl::SessionParams*> session_params;

/* rhino procedural data */
extern ccl::vector<float> ccycles_rhino_perlin_noise_table;
extern ccl::vector<float> ccycles_rhino_impulse_noise_table;
extern ccl::vector<float> ccycles_rhino_vc_noise_table;
extern ccl::vector<float> ccycles_rhino_aaltonen_noise_table;


/********************************/
/* Some utility functions		 */
/********************************/

extern bool scene_find(ccl::Session* scid, ccl::Scene** sce);
extern bool session_find(ccl::Session* sid, CCSession** ccsess, ccl::Session** session);

extern void _cleanup_sessions();
