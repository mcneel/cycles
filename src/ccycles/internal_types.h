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
//#include <ctime>
#include <thread>
//#include <mutex>
#include <string>
#include <functional>

#pragma warning ( push )

#pragma warning ( disable : 4244 )

#include "scene/background.h"
#include "scene/camera.h"
#include "scene/colorspace.h"
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
#include "util/function.h"
#include "util/progress.h"
#include "util/string.h"
#include "util/thread.h"

#pragma warning ( pop )

#include "ccycles.h"

#define MULTIDEVICEOFFSET 100000
#define ISMULTIDEVICE(id) (id>=MULTIDEVICEOFFSET)
#define MULTIDEVICEIDX(id) (id-MULTIDEVICEOFFSET)
#define GETDEVICE(puthere, id) \
	if (ISMULTIDEVICE(id)) { \
		(puthere) = multi_devices[MULTIDEVICEIDX(id)]; \
	} \
	else { \
		(puthere) = devices[id]; \
	}

//extern LOGGER_FUNC_CB logger_func;
extern std::vector<LOGGER_FUNC_CB> loggers;

/* Simple class to help with debug logging. */
class Logger {
public:
	bool tostdout{ false };

	/* Variadic template function so we can handle
	 * any amount of arguments. Each subsequent call
	 * adds the head to logger_msg.
	 */
	template<typename T, typename... Tail>
	void logit(T head, Tail... tail) {
		m.lock();
		/* reset logger_msg */
		logger_msg.str("");

		/* get current time as string. std::ctime returns char* that
		 * has \n at the end. Make sure we don't have that, otherwise
		 * our beautiful output gets broken up.
		 */
		auto t = std::chrono::system_clock::now();
		std::time_t ts = std::chrono::system_clock::to_time_t(t);
		auto tsstr = std::string{std::ctime(&ts)};
		tsstr = tsstr.substr(0, tsstr.size() - 1);

		/* start our new logger_msg with timestamp and head, then
		 * follow up :)
		 */
		logger_msg << tsstr << ": " << head;
		logit_followup( tail...);
	}

private:
	template<typename T, typename... Tail>
	void logit_followup(T head, Tail... tail) {
		logger_msg << head;
		logit_followup(tail...);
	}

	/* The final logit function call that will realise the
	 * stringstream constructed by the variadic template
	 * function. Upon completion reset the logger_msg to
	 * an empty string.
	 */
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

/*
 * The logger facility to use.
 *
 * Usage:
 * logger.logit("This is a message", var, var2, " and some more", var3);
 */
extern Logger logger;

struct CCImage {
		std::string filename;
		void *builtin_data;

		int width;
		int height;
		int depth;
		int channels;
		bool is_float;
};

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

	private:
		std::mutex m_lock;
		ccl::PassType m_pass_type;
		int m_width;
		int m_height;
		std::vector<float> m_pixels;
};

class CCyclesDebugDriver : public ccl::OutputDriver {
	public:
		typedef std::function<void(const std::string &)> LogFunction;

		CCyclesDebugDriver(LogFunction log);
		virtual ~CCyclesDebugDriver();

		void write_render_tile(const Tile &tile) override;

	protected:
		LogFunction log_;
};

class CCyclesOutputDriver : public ccl::OutputDriver {
	public:
		typedef std::function<void(const std::string &)> LogFunction;

		CCyclesOutputDriver(std::vector<std::unique_ptr<CCyclesPassOutput>> *passes, LogFunction log);
		virtual ~CCyclesOutputDriver();

		virtual void write_render_tile(const Tile &tile) override;
		virtual bool update_render_tile(const Tile & /* tile */) override;

	protected:
		LogFunction log_;

		std::vector<std::unique_ptr<CCyclesPassOutput>> *passes;
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
		virtual void clear() override;
		virtual void draw(const Params &params) override;

// Optional

		virtual GraphicsInterop graphics_interop_get() override;
		virtual void graphics_interop_activate() override;
		virtual void graphics_interop_deactivate() override;

	protected:
		LogFunction log_;

		std::vector<ccl::half4> pixels_half4;

		std::vector<std::unique_ptr<CCyclesPassOutput>> *passes;
};

class CCSession final {
public:
	unsigned int id{ 0 };
	ccl::SessionParams params;
	ccl::SceneParams scene_params;
	ccl::Session* session = nullptr;

	/* The status update handler for ccl::Session update callback.
	 */
	void status_update(void);
	/* The test cancel handler for ccl::Session cancellation test
	 */
	void test_cancel(void);

	void display_update(int sample);

	int width{ 0 };
	int height{ 0 };

	ccl::BufferParams buffer_params;

	std::vector<std::unique_ptr<CCyclesPassOutput>> passes;

	/* Create a new CCSession, initialise all necessary memory. */
	static CCSession* create(int width, int height, unsigned int buffer_stride);

	/* Returns true if size was changed. Will reset the has_changed flag. */
	bool size_has_changed();

	~CCSession() {
		delete session;
		session = nullptr;
	}

private:
	bool _size_has_changed;

protected:
	/* Protected constructor, use CCSession::create to create a new CCSession. */
	CCSession()
	{	}
};

class CCShader {
public:
	/* Hold the Cycles shader. */
	ccl::Shader* shader = new ccl::Shader();
	/* Hold the shader node graph. */
	ccl::ShaderGraph* graph = new ccl::ShaderGraph();
	/* Map shader ID in scene to scene ID. */
	std::map<unsigned int, unsigned int> scene_mapping;

	~CCShader() {
		scene_mapping.clear();
	}
};

class CCScene final {
public:
	/* Hold the Cycles scene. */
	ccl::Scene* scene = nullptr;

	unsigned int params_id = -1;

	std::vector<CCImage*> images;

	std::vector<CCShader*> shaders;

	/* Note: depth>1 if volumetric texture (i.e smoke volume data) */

	void builtin_image_info(const std::string& builtin_name, void* builtin_data, ccl::ImageMetaData& meta); // bool& is_float, int& width, int& height, int& depth, int& channels);
	bool builtin_image_pixels(const std::string& builtin_name, void* builtin_data, int tile, unsigned char* pixels, const size_t pixels_size, const bool associate_alpha, const bool free_cache);
	bool builtin_image_float_pixels(const std::string& builtin_name, void* builtin_data, int tile, float* pixels, const size_t pixels_size, const bool associate_alpha, const bool free_cache);

	~CCScene() {
		for(CCImage* image : images) {
			/* don't delete builtin_data, it isn't owned by this */
			image->builtin_data = nullptr;
			delete image;
		}
		images.clear();
		for (CCShader* sh : shaders) {
			if (sh != nullptr) {
				// just setting to nullptr, as scene disposal frees this memory.
				sh->graph = nullptr;
				sh->shader = nullptr;

				sh->scene_mapping.clear();

				delete sh;
			}
		}
		shaders.clear();
	}
};

/* data */
extern std::vector<ccl::SceneParams*> scene_params;
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

extern ccl::Shader* find_shader_in_scene(ccl::Scene* sce, unsigned int shader_id);
extern unsigned int get_idx_for_shader_in_scene(ccl::Scene* sce, ccl::Shader* sh);
extern bool scene_find(ccl::Session* scid, ccl::Scene** sce);
extern bool session_find(ccl::Session* sid, CCSession** ccsess, ccl::Session** session);
extern void scene_clear_pointer(ccl::Scene* sce);
extern void set_ccscene_null(ccl::Session* session_id);

extern void _cleanup_scenes();
extern void _cleanup_sessions();
extern void _init_shaders(ccl::Session* session_id);

/********************************/
/* Some useful defines			*/
/********************************/


/* Set boolean parameter varname of param_type. */
#define PARAM_BOOL(param_type, params_id, varname) \
	if (0 <= params_id && params_id < param_type.size()) { \
		param_type[params_id]-> varname = varname == 1; \
		logger.logit("Set " #param_type " " #varname " to ", varname); \
	}

/* Set parameter varname of param_type. */
#define PARAM(param_type, params_id, varname) \
	if (0 <= params_id && params_id < param_type.size()) { \
		param_type[params_id]-> varname = varname; \
		logger.logit("Set " #param_type " " #varname " to ", varname); \
	}

/* Set parameter varname of param_type, casting to typecast*/
#define PARAM_CAST(param_type, params_id, typecast, varname) \
	if (0 <= params_id && params_id < param_type.size()) { \
		param_type[params_id]-> varname = static_cast<typecast>(varname); \
		logger.logit("Set " #param_type " " #varname " to ", varname, " casting to " #typecast); \
	}

#define LIGHT_FIND(session_id, light_id) \
	ccl::Scene* sce = nullptr; \
	if(scene_find(session_id, &sce)) { \
		ccl::Light *l = light_id; \

#define LIGHT_FIND_END() \
		l->tag_update(sce); \
	}

#define SHADER_VAR2(a,b) a ## b
#define SHADER_VAR(a, b) SHADER_VAR2(a,b)
/* Set a var of shader to val of type. */
#define SHADER_SET(session_id, shid, type, var, val) \
	ccl::Scene* sce = nullptr; \
	if (scene_find(session_id, &sce)) { \
		sh->shader-> var = (type)(val); \
	}

