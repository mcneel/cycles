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

#include <iostream>
#include <cstdio>
#include <filesystem>
#include <cstdlib>
#include <numeric>
#include <random>
#include <stdexcept>

namespace fs = std::filesystem;

#ifdef _WIN32
#include <windows.h>
#include <eh.h>
#include <exception>
#else
#include <signal.h>
#endif

#include "internal_types.h"
#include "device/device.h"
#include "util/thread.h"

#include <OpenImageIO/imagebuf.h>
#include <OpenImageIO/imagebufalgo.h>

using namespace ccl;

/* Hold all created sessions. */
std::unordered_set<CCSession*> sessions;

static ccl::thread_mutex session_mutex;

class CyclesRenderCrashException : std::exception
{
public:
	CyclesRenderCrashException() : m_nVDE(-1) {}
	CyclesRenderCrashException(unsigned int n) : m_nVDE(n) {}

	unsigned int VDENumber() const { return m_nVDE; }

private:
	unsigned int m_nVDE;
};

#ifdef _WIN32
static void prep_session(ccl::Session *session, std::vector<std::unique_ptr<CCyclesPassOutput>> *passes, CCSession* ccsession);

static const char *seh_exception_code_name(DWORD code)
{
	switch (code) {
	case EXCEPTION_ACCESS_VIOLATION:
		return "EXCEPTION_ACCESS_VIOLATION";
	case EXCEPTION_ARRAY_BOUNDS_EXCEEDED:
		return "EXCEPTION_ARRAY_BOUNDS_EXCEEDED";
	case EXCEPTION_BREAKPOINT:
		return "EXCEPTION_BREAKPOINT";
	case EXCEPTION_DATATYPE_MISALIGNMENT:
		return "EXCEPTION_DATATYPE_MISALIGNMENT";
	case EXCEPTION_FLT_DENORMAL_OPERAND:
		return "EXCEPTION_FLT_DENORMAL_OPERAND";
	case EXCEPTION_FLT_DIVIDE_BY_ZERO:
		return "EXCEPTION_FLT_DIVIDE_BY_ZERO";
	case EXCEPTION_FLT_INVALID_OPERATION:
		return "EXCEPTION_FLT_INVALID_OPERATION";
	case EXCEPTION_FLT_OVERFLOW:
		return "EXCEPTION_FLT_OVERFLOW";
	case EXCEPTION_FLT_STACK_CHECK:
		return "EXCEPTION_FLT_STACK_CHECK";
	case EXCEPTION_FLT_UNDERFLOW:
		return "EXCEPTION_FLT_UNDERFLOW";
	case EXCEPTION_ILLEGAL_INSTRUCTION:
		return "EXCEPTION_ILLEGAL_INSTRUCTION";
	case EXCEPTION_IN_PAGE_ERROR:
		return "EXCEPTION_IN_PAGE_ERROR";
	case EXCEPTION_INT_DIVIDE_BY_ZERO:
		return "EXCEPTION_INT_DIVIDE_BY_ZERO";
	case EXCEPTION_INT_OVERFLOW:
		return "EXCEPTION_INT_OVERFLOW";
	case EXCEPTION_INVALID_DISPOSITION:
		return "EXCEPTION_INVALID_DISPOSITION";
	case EXCEPTION_NONCONTINUABLE_EXCEPTION:
		return "EXCEPTION_NONCONTINUABLE_EXCEPTION";
	case EXCEPTION_PRIV_INSTRUCTION:
		return "EXCEPTION_PRIV_INSTRUCTION";
	case EXCEPTION_SINGLE_STEP:
		return "EXCEPTION_SINGLE_STEP";
	case EXCEPTION_STACK_OVERFLOW:
		return "EXCEPTION_STACK_OVERFLOW";
	default:
		return "UNKNOWN_SEH_EXCEPTION";
	}
}

static int log_seh_exception(const char *stage, EXCEPTION_POINTERS *exception_info)
{
	if (exception_info == nullptr || exception_info->ExceptionRecord == nullptr) {
		fprintf(stderr, "ccycles: %s caught SEH exception without exception record\n", stage);
		fflush(stderr);
		return EXCEPTION_EXECUTE_HANDLER;
	}

	const EXCEPTION_RECORD *record = exception_info->ExceptionRecord;
	fprintf(stderr,
		"ccycles: %s caught SEH exception code=0x%08X name=%s address=%p flags=0x%08X params=%lu\n",
		stage,
		(unsigned int)record->ExceptionCode,
		seh_exception_code_name(record->ExceptionCode),
		record->ExceptionAddress,
		(unsigned int)record->ExceptionFlags,
		(unsigned long)record->NumberParameters);
	fflush(stderr);

	if ((record->ExceptionCode == EXCEPTION_ACCESS_VIOLATION ||
		 record->ExceptionCode == EXCEPTION_IN_PAGE_ERROR) &&
		record->NumberParameters >= 2)
	{
		const char *operation = "unknown";
		switch (record->ExceptionInformation[0]) {
		case 0:
			operation = "read";
			break;
		case 1:
			operation = "write";
			break;
		case 8:
			operation = "execute";
			break;
		}
		fprintf(stderr,
			"ccycles: %s access violation operation=%s target=%p\n",
			stage,
			operation,
			(void *)record->ExceptionInformation[1]);
		fflush(stderr);
	}

	return EXCEPTION_EXECUTE_HANDLER;
}

static ccl::Session *create_session_with_seh_logging(const ccl::SessionParams &params,
	                                                 const ccl::SceneParams &scene_params)
{
	__try {
		return new ccl::Session(params, scene_params);
	}
	__except (log_seh_exception("cycles_session_create new Session", GetExceptionInformation())) {
		return nullptr;
	}
}

static bool prep_session_with_seh_logging(ccl::Session *session,
	                                      std::vector<std::unique_ptr<CCyclesPassOutput>> *passes,
	                                      CCSession *ccsession)
{
	__try {
		prep_session(session, passes, ccsession);
		return true;
	}
	__except (log_seh_exception("cycles_session_create prep_session", GetExceptionInformation())) {
		return false;
	}
}
#endif

/* Find pointers for CCSession and ccl::Session. Return false if either fails. */
bool session_find(ccl::Session* sid, CCSession** ccsess, ccl::Session** session)
{
	ccl::thread_scoped_lock lock(session_mutex);
  auto sidhit = [sid](CCSession* i) { return i->session == sid; };
  auto found = std::find_if(sessions.cbegin(), sessions.cend(), sidhit);
	if (found != sessions.cend()) {
		*ccsess = (*found);
		if(*ccsess!=nullptr) *session = (*ccsess)->session;
		return *ccsess!=nullptr && *session!=nullptr;
	}
	return false;
}

/**
 * Clean up resources acquired during this run of Cycles.
 */
void _cleanup_sessions()
{
	ccl::thread_scoped_lock session_lock(session_mutex);
	ccl::thread_scoped_lock params_lock(session_params_mutex);

	for (CCSession* se : sessions) {
		if (se == nullptr) continue;

		{
			if(se->session) {
				delete se->session;
				se->session = nullptr;
			}
		}
		delete se;
	}

	sessions.clear();
	for (ccl::SessionParams* params : session_params) {
		delete params;
	}
	session_params.clear();
}

CCSession* CCSession::create(int width, int height, unsigned int buffer_stride) {
	CCSession* se = new CCSession();
	se->width = width;
	se->height = height;
	se->_size_has_changed = false;

	return se;
}

bool CCSession::size_has_changed() {
	bool rc = _size_has_changed;
	_size_has_changed = false;
	return rc;
}

CCyclesPassOutput::CCyclesPassOutput()
	: m_lock(), m_pass_type(PASS_COMBINED), m_width(0), m_height(0), m_pixels()
{
}

void CCyclesPassOutput::lock()
{
	m_lock.lock();
}

void CCyclesPassOutput::unlock()
{
	m_lock.unlock();
}

ccl::PassType CCyclesPassOutput::get_pass_type() const
{
	return m_pass_type;
}

void CCyclesPassOutput::set_pass_type(ccl::PassType value)
{
	m_pass_type = value;
}

int CCyclesPassOutput::get_width() const
{
	return m_width;
}

void CCyclesPassOutput::set_width(int width)
{
	m_width = width;
}

int CCyclesPassOutput::get_height() const
{
	return m_height;
}

void CCyclesPassOutput::set_height(int height)
{
	m_height = height;
}

int CCyclesPassOutput::get_pixel_size() const
{
	return m_pixel_size;
}

void CCyclesPassOutput::set_pixel_size(int pixel_size)
{
	m_pixel_size = pixel_size;
}

std::vector<float> &CCyclesPassOutput::pixels()
{
	return m_pixels;
}


CCyclesOutputDriver::CCyclesOutputDriver(std::vector<std::unique_ptr<CCyclesPassOutput>> *full_passes,
										 CCyclesOutputDriver::LogFunction log,
										 CCSession* ccsession)
	: full_passes(full_passes), log_(log), ccsession_(ccsession)
{
}

CCyclesOutputDriver::~CCyclesOutputDriver()
{
}

bool CCyclesOutputDriver::write_or_update_render_tile(const Tile &tile)
{
	if (full_passes == nullptr)
		return false;

	bool doing_tiles = !(tile.size == tile.full_size);
#if 0
	const int width = tile.size.x;
	const int height = tile.size.y;
	vector<float> pixels(width * height * 1);

	if (tile.get_sample() < 2 && tile.get_pass_pixels("depth", 1, pixels.data())) {
		//// !!!!!!!!!!!!! Remember to change path to something useful on dev machine
		//fs::path save_path = "C:/Users/jesterKing/check_cycles_output.png";
		fs::path save_path = "/Users/jesterking/check_cycles_output.exr";
		//// !!!!!!!!!!!!! Remember to change path to something useful on dev machine
		unique_ptr<ImageOutput> image_output(ImageOutput::create("exr"));
		ImageSpec spec(width, height, 1, TypeDesc::FLOAT);
		if(nullptr != image_output &&image_output->open(save_path.string(), spec))
		{
			ImageBuf image_buffer(spec,
				pixels.data(),
				AutoStride,
				width * 1 * sizeof(float),
				AutoStride);
			/* Write to disk and close */
			image_buffer.set_write_format(TypeDesc::FLOAT);
			image_buffer.write(image_output.get());
			image_output->close();
		}
	}
#endif

	if (doing_tiles) {
		tile_passes.resize(full_passes->size());

		for (int i = 0; i < tile_passes.size(); i++) {
			auto &tile_pass = tile_passes[i];

			ccl::PassType pass_type = (*full_passes)[i]->get_pass_type();

			PassInfo pass_info = Pass::get_info(pass_type);

			const int width = tile.size.x;
			const int height = tile.size.y;
			const int tile_size = width * height * pass_info.num_components;

			if (tile_pass.size() < tile_size) {
				tile_pass.resize(width * height * pass_info.num_components);
			}

			if (!tile.get_pass_pixels(
					pass_type_as_string(pass_type), pass_info.num_components, tile_pass.data())) {
				log_("Failed to read render pass pixels");
				return false;
			}
		}

		for (int i = 0; i < tile_passes.size(); i++) {
			auto &tile_pass = tile_passes[i];
			auto &full_pass = (*full_passes)[i];

			if(full_pass->get_pass_type() == PASS_DEPTH && tile.get_sample() > 1)
			{
				continue;
			}

			full_pass->lock();

			PassInfo pass_info = Pass::get_info(full_pass->get_pass_type());

			const int pixel_stride = pass_info.num_components;
			const int pixel_stride_bytes = pixel_stride * sizeof(float);

			const int tile_width = tile.size.x;
			const int tile_height = tile.size.y;
			const int tile_stride = tile_width * pixel_stride;
			const float *tile_buffer = tile_pass.data();

			const int full_width = tile.full_size.x;
			const int full_height = tile.full_size.y;
			const int full_stride = full_width * pixel_stride;

			full_pass->set_width(full_width);
			full_pass->set_height(full_height);
			full_pass->pixels().resize(full_height * full_stride);

			const float *full_buffer = full_pass->pixels().data() + tile.offset.y * full_stride +
									   tile.offset.x * pixel_stride;

			for (int row = 0; row < tile_height; row++) {
				memcpy((void *)(full_buffer + row * full_stride),
					   (void *)(tile_buffer + row * tile_stride),
					   tile_stride * sizeof(float));
			}

			full_pass->unlock();
		}
	}
	else {
		for (auto &pass : *full_passes) {
			bool upscale = tile.resolution_divider > ccsession_->params.pixel_size ||
						   ccsession_->params.pixel_size > 1;
			if (!upscale && pass->get_pass_type() == PASS_DEPTH && tile.get_sample() > 1) {
				continue;
			}

			pass->lock();

			PassInfo pass_info = Pass::get_info(pass->get_pass_type());

			const int target_width = tile.full_size.x;
			const int target_height = tile.full_size.y;
			pass->set_width(target_width);
			pass->set_height(target_height);
			pass->set_pixel_size(tile.resolution_divider);

			pass->pixels().resize(target_width * target_height * pass_info.num_components);
			if (!tile.get_pass_pixels(pass_type_as_string(pass->get_pass_type()),
									  pass_info.num_components,
									  pass->pixels().data())) {
				log_("Failed to read render pass pixels");
				pass->unlock();

				return false;
			}

			/* In case we have pixel_size > 1 we need to move data so that we get
			 * pixels in top-left quadrant.
			 */
			if(upscale) {
				const int ps =
					tile.resolution_divider > ccsession_->params.pixel_size
					? tile.resolution_divider
					: ccsession_->params.pixel_size;
				const int source_width = target_width / ps;
				const int source_height = target_height / ps;
				const int stride = pass_info.num_components;

				float *pixeldata = pass->pixels().data();

				const int source_scanline_width = source_width * stride;
				const int target_scanline_width = target_width * stride;
				for (int y = source_height - 1; y >= 0; y--)
				{
						const int source_idx = y * source_scanline_width;
						const int target_idx = y * target_scanline_width;
						memcpy(pixeldata + target_idx, pixeldata + source_idx, source_scanline_width*sizeof(float));
				}
			}

			pass->unlock();
		}
	}

	return true;
}

void CCyclesOutputDriver::write_render_tile(const Tile &tile)
{
	// no implementation needed
	// only update_render_tile is useful for RhinoCycles
}

bool CCyclesOutputDriver::update_render_tile(const Tile &tile)
{
	return write_or_update_render_tile(tile);
}

static void log_print(const std::string& msg)
{
	std::cout << msg << std::endl;
#ifdef WIN32
	OutputDebugString(msg.c_str());
	OutputDebugString("\n");
#endif
}

static void prep_session(ccl::Session *session, std::vector<std::unique_ptr<CCyclesPassOutput>> *passes, CCSession* ccsession)
{
	try {
		if (session == nullptr) {
			fprintf(stderr, "ccycles: prep_session received null session\n");
			fflush(stderr);
			throw std::runtime_error("prep_session received null session");
		}
		if (session->scene == nullptr) {
			fprintf(stderr, "ccycles: prep_session received session with null scene\n");
			fflush(stderr);
			throw std::runtime_error("prep_session received session with null scene");
		}

		fprintf(stderr,
			"ccycles: prep_session enter session=%p scene=%p passes=%p ccsession=%p\n",
			(void *)session,
			(void *)session->scene,
			(void *)passes,
			(void *)ccsession);
		fflush(stderr);

		ccl::Scene *scene = session->scene;
		fprintf(stderr,
			"ccycles: prep_session scene pointers camera=%p integrator=%p background=%p default_background=%p default_surface=%p shader_manager=%p\n",
			(void *)scene->camera,
			(void *)scene->integrator,
			(void *)scene->background,
			(void *)scene->default_background,
			(void *)scene->default_surface,
			(void *)scene->shader_manager);
		fflush(stderr);

		ccl::Camera *cam = scene->camera;
		if (cam == nullptr) {
			fprintf(stderr, "ccycles: prep_session camera is null\n");
			fflush(stderr);
			throw std::runtime_error("prep_session camera is null");
		}
		fprintf(stderr, "ccycles: prep_session camera setup begin camera=%p\n", (void *)cam);
		fflush(stderr);
		cam->set_full_height(512);
		cam->set_full_width(512);
		cam->compute_auto_viewplane();
		cam->need_flags_update = true;
		cam->update(session->scene);
		fprintf(stderr, "ccycles: prep_session camera setup ok\n");
		fflush(stderr);

		fprintf(stderr, "ccycles: prep_session creating output driver\n");
		fflush(stderr);
		session->set_output_driver(std::make_unique<CCyclesOutputDriver>(passes, log_print, ccsession));
		fprintf(stderr, "ccycles: prep_session output driver set\n");
		fflush(stderr);

		ccl::Integrator *integrator = scene->integrator;
		if (integrator == nullptr) {
			fprintf(stderr, "ccycles: prep_session integrator is null\n");
			fflush(stderr);
			throw std::runtime_error("prep_session integrator is null");
		}
		fprintf(stderr, "ccycles: prep_session integrator setup begin integrator=%p\n", (void *)integrator);
		fflush(stderr);
		integrator->set_use_light_tree(true);
		integrator->set_light_sampling_threshold(0.01f);
		integrator->set_use_adaptive_sampling(true);
		integrator->set_adaptive_min_samples(1);
		integrator->set_adaptive_threshold(0.01f);
		integrator->set_denoiser_type(ccl::DENOISER_NONE);
		integrator->set_guiding_distribution_type(ccl::GUIDING_TYPE_DIRECTIONAL_QUAD_TREE);
		fprintf(stderr, "ccycles: prep_session integrator setup ok\n");
		fflush(stderr);

		// This needs to be here (for now) so that the node will register itself
		// through the dynamic initialization of the global variable. If not here
		// compiler will optimize away the code in the .cpp file.
		fprintf(stderr, "ccycles: prep_session registering AzimuthAltitudeTransformNode\n");
		fflush(stderr);
		AzimuthAltitudeTransformNode derp;
		fprintf(stderr, "ccycles: prep_session AzimuthAltitudeTransformNode registered\n");
		fflush(stderr);

		{
			fprintf(stderr, "ccycles: prep_session background shader setup begin\n");
			fflush(stderr);
			if (scene->background == nullptr) {
				fprintf(stderr, "ccycles: prep_session background is null\n");
				fflush(stderr);
				throw std::runtime_error("prep_session background is null");
			}
			Shader *bgsh = scene->default_background;
			if (bgsh == nullptr) {
				fprintf(stderr, "ccycles: prep_session default_background is null\n");
				fflush(stderr);
				throw std::runtime_error("prep_session default_background is null");
			}
			scene->background->set_transparent_glass(true);
			ccl::ShaderGraph *graph = new ccl::ShaderGraph();
			ccl::OutputNode *out = graph->output();
			ustring nodename("background_shader");
			ccl::ShaderNode *shn = nullptr;
			const ccl::NodeType *ntype = ccl::NodeType::find(nodename);
			fprintf(stderr, "ccycles: prep_session background node type=%p\n", (const void *)ntype);
			fflush(stderr);
			if (ntype == nullptr) {
				fprintf(stderr, "ccycles: prep_session background_shader node type lookup failed\n");
				fflush(stderr);
				throw std::runtime_error("prep_session background_shader node type lookup failed");
			}
			shn = (ShaderNode *)ntype->create(ntype);
			fprintf(stderr, "ccycles: prep_session background shader node=%p graph=%p\n", (void *)shn, (void *)graph);
			fflush(stderr);
			shn->set_owner(graph);
			{
				std::random_device r;
				std::mt19937 gen(r());	 // Standard mersenne_twister_engine seeded with rd()
				std::uniform_real_distribution<> dist(0.0, 1.0);
				ccl::BackgroundNode *bgn = (ccl::BackgroundNode *)shn;
				bgn->set_color(ccl::make_float3(dist(gen), dist(gen), dist(gen)));
				bgn->set_strength(1.5f);
			}
			graph->add(shn);
			graph->connect(shn->output("Background"), out->input("Surface"));
			bgsh->set_graph(graph);
			bgsh->tag_update(scene);
			fprintf(stderr, "ccycles: prep_session background shader setup ok shader=%p\n", (void *)bgsh);
			fflush(stderr);
		}

		{
			fprintf(stderr, "ccycles: prep_session surface shader setup begin\n");
			fflush(stderr);
			auto default_surface_shader = scene->default_surface;
			if (default_surface_shader == nullptr) {
				fprintf(stderr, "ccycles: prep_session default_surface is null\n");
				fflush(stderr);
				throw std::runtime_error("prep_session default_surface is null");
			}
			auto graph = new ccl::ShaderGraph();
			auto out = graph->output();
			ustring nodename("diffuse_bsdf");
			ccl::ShaderNode* shader_node = nullptr;
			const ccl::NodeType *ntype = ccl::NodeType::find(nodename);
			fprintf(stderr, "ccycles: prep_session surface node type=%p\n", (const void *)ntype);
			fflush(stderr);
			if (ntype == nullptr) {
				fprintf(stderr, "ccycles: prep_session diffuse_bsdf node type lookup failed\n");
				fflush(stderr);
				throw std::runtime_error("prep_session diffuse_bsdf node type lookup failed");
			}
			shader_node = (ShaderNode *)ntype->create(ntype);
			fprintf(stderr,
				"ccycles: prep_session surface shader node=%p graph=%p default_surface=%p\n",
				(void *)shader_node,
				(void *)graph,
				(void *)default_surface_shader);
			fflush(stderr);
			shader_node->set_owner(graph);
			{
				std::random_device r;
				std::mt19937 gen(r());	 // Standard mersenne_twister_engine seeded with rd()
				std::uniform_real_distribution<> dist(0.0, 1.0);
				auto diff = (ccl::DiffuseBsdfNode *)shader_node;
				diff->set_color(ccl::make_float3(dist(gen), dist(gen), dist(gen)));
				diff->set_roughness(1.0f);
			}
			graph->add(shader_node);
			graph->connect(shader_node->output("BSDF"), out->input("Surface"));
			default_surface_shader->set_graph(graph);
			default_surface_shader->tag_update(scene);
			fprintf(stderr, "ccycles: prep_session surface shader setup ok\n");
			fflush(stderr);

			fprintf(stderr, "ccycles: prep_session setting Rhino noise tables\n");
			fflush(stderr);
			if (session->scene->shader_manager == nullptr) {
				fprintf(stderr, "ccycles: prep_session shader_manager is null\n");
				fflush(stderr);
				throw std::runtime_error("prep_session shader_manager is null");
			}
			session->scene->shader_manager->set_rhino_perlin_noise_table(ccycles_rhino_perlin_noise_table);
			session->scene->shader_manager->set_rhino_impulse_noise_table(ccycles_rhino_impulse_noise_table);
			session->scene->shader_manager->set_rhino_vc_noise_table(ccycles_rhino_vc_noise_table);
			session->scene->shader_manager->set_rhino_aaltonen_noise_table(ccycles_rhino_aaltonen_noise_table);
			fprintf(stderr, "ccycles: prep_session Rhino noise tables set\n");
			fflush(stderr);
		}

		fprintf(stderr, "ccycles: prep_session exit\n");
		fflush(stderr);
	}
	catch (const std::exception &e) {
		fprintf(stderr, "ccycles: prep_session caught std::exception: %s\n", e.what());
		fflush(stderr);
		throw;
	}
	catch (...) {
		fprintf(stderr, "ccycles: prep_session caught unknown exception\n");
		fflush(stderr);
		throw;
	}
}

#ifdef __cplusplus
extern "C" {
#endif

static void cleanup_failed_session_create(CCSession *session)
{
	if (session == nullptr) {
		return;
	}

	if (session->params_original_handle != nullptr) {
		ccl::thread_scoped_lock params_lock(session_params_mutex);
		if (auto search = session_params.find(session->params_original_handle); search != session_params.end()) {
			session_params.erase(search);
		}
		delete session->params_original_handle;
		session->params_original_handle = nullptr;
	}

	delete session;
}

CCL_CAPI ccl::Session* CDECL cycles_session_create(ccl::SessionParams* _session_parameters)
{
	ccl::thread_scoped_lock lock(session_mutex);

	fprintf(stderr, "ccycles: cycles_session_create enter handle=%p\n", (void *)_session_parameters);
	fflush(stderr);

	CCSession* session = CCSession::create(10, 10, 4);
	{
		ccl::thread_scoped_lock params_lock(session_params_mutex);
		auto param_it = session_params.find(_session_parameters);
		if (param_it == session_params.end()) {
			fprintf(stderr,
				"ccycles: cycles_session_create params handle not found handle=%p\n",
				(void *)_session_parameters);
			fflush(stderr);
			delete session;
			return nullptr;
		}

		ccl::SessionParams *params = *param_it;
		if (params == nullptr) {
			fprintf(stderr,
				"ccycles: cycles_session_create params pointer is null handle=%p\n",
				(void *)_session_parameters);
			fflush(stderr);
			delete session;
			return nullptr;
		}

		session->params = *params;
		session->params_original_handle = params;
	}

	try {
		int csesid{ -1 };
		int hid{ 0 };

		// TODO: XXXX these are hardcoded params/sceneparams
		session->params.tile_size = 512;
		session->params.use_auto_tile = false;
		session->params.experimental = true;
		session->params.shadingsystem = ccl::SHADINGSYSTEM_SVM;
		session->scene_params.shadingsystem = ccl::SHADINGSYSTEM_SVM;
		fprintf(stderr,
			"ccycles: cycles_session_create device_type=%s desc='%s' id='%s' num=%d threads=%d background=%d experimental=%d\n",
			ccl::Device::string_from_type(session->params.device.type).c_str(),
			session->params.device.description.c_str(),
			session->params.device.id.c_str(),
			session->params.device.num,
			session->params.threads,
			(int)session->params.background,
			(int)session->params.experimental);
		fflush(stderr);

		fprintf(stderr, "ccycles: cycles_session_create calling new Session\n");
		fflush(stderr);
#ifdef _WIN32
		session->session = create_session_with_seh_logging(session->params, session->scene_params);
#else
		session->session = new ccl::Session(session->params, session->scene_params);
#endif
		if (session->session == nullptr) {
			fprintf(stderr, "ccycles: cycles_session_create new Session returned null\n");
			fflush(stderr);
			cleanup_failed_session_create(session);
			return nullptr;
		}
		fprintf(stderr,
			"ccycles: cycles_session_create new Session ok session=%p scene=%p device=%p\n",
			(void *)session->session,
			session->session != nullptr ? (void *)session->session->scene : nullptr,
			session->session != nullptr ? (void *)session->session->device : nullptr);
		fflush(stderr);

		fprintf(stderr, "ccycles: cycles_session_create calling prep_session session=%p\n", (void *)session->session);
		fflush(stderr);
#ifdef _WIN32
		const bool prep_session_ok = prep_session_with_seh_logging(session->session, &session->passes, session);
#else
		prep_session(session->session, &session->passes, session);
		const bool prep_session_ok = true;
#endif
		if (!prep_session_ok) {
			fprintf(stderr, "ccycles: cycles_session_create prep_session failed after SEH\n");
			fflush(stderr);
			cleanup_failed_session_create(session);
			return nullptr;
		}
		fprintf(stderr, "ccycles: cycles_session_create prep_session ok session=%p\n", (void *)session->session);
		fflush(stderr);

		sessions.insert(session);
		csesid = (unsigned int)(sessions.size() - 1);
		fprintf(stderr,
			"ccycles: cycles_session_create returning session=%p handle_index=%d\n",
			(void *)session->session,
			csesid);
		fflush(stderr);

		return session->session;
	}
	catch (const std::exception &e) {
		fprintf(stderr, "ccycles: cycles_session_create caught std::exception: %s\n", e.what());
		fflush(stderr);
		cleanup_failed_session_create(session);
		return nullptr;
	}
	catch (...) {
		fprintf(stderr, "ccycles: cycles_session_create caught unknown exception\n");
		fflush(stderr);
		cleanup_failed_session_create(session);
		return nullptr;
	}
}

CCL_CAPI void CDECL cycles_session_destroy(ccl::Session* session_id)
{
	ccl::thread_scoped_lock lock(session_mutex);

	auto found = sessions.cend();
	for (auto it = sessions.cbegin(); it != sessions.cend(); ++it) {
		if ((*it)->session == session_id) {
			found = it;
			break;
		}
	}
	if (found == sessions.cend()) {
		return;
	}

	CCSession* ccsess = *found;

	sessions.erase(ccsess);
	if (ccsess->params_original_handle != nullptr) {
		ccl::thread_scoped_lock params_lock(session_params_mutex);
		if (auto search = session_params.find(ccsess->params_original_handle); search != session_params.end()) {
			session_params.erase(search);
		}
		delete ccsess->params_original_handle;
		ccsess->params_original_handle = nullptr;
	}
	delete ccsess;
}

CCL_CAPI void CDECL cycles_session_clear_passes(ccl::Session* session_id)
{
	ccl::vector<ccl::Pass*>& passes = session_id->scene->passes;
	for (ccl::Pass *pass : passes) {
		session_id->scene->delete_node(pass);
	}

	passes.clear();

	ccl::Session *session = nullptr;
	CCSession *ccsess = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		ccsess->passes.clear();
	}
}

CCL_CAPI void CDECL cycles_session_add_pass(ccl::Session *session_id, int pass_id)
{
	ccl::PassType passtype = (ccl::PassType)pass_id;

	ccl::Pass *pass = session_id->scene->create_node<ccl::Pass>();
	pass->set_name(ustring(pass_type_as_string(passtype)));
	pass->set_type(passtype);

	ccl::Session *session = nullptr;
	CCSession *ccsess = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		std::unique_ptr<CCyclesPassOutput> outputpass = std::make_unique<CCyclesPassOutput>();
		outputpass->set_pass_type(passtype);

		ccsess->passes.push_back(std::move(outputpass));
	}
}


CCL_CAPI int CDECL cycles_session_reset(ccl::Session* session_id, int width, int height, int samples, int full_x, int full_y, int full_width, int full_height, int pixel_size)
{
	int rc = 0;
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		try {
			logger.logit("Reset session ", session_id, ". width ", width, " height ", height, " samples ", samples);
			ccsess->buffer_params.full_x = full_x;
			ccsess->buffer_params.full_y = full_y;
			ccsess->buffer_params.full_width = full_width;
			ccsess->buffer_params.full_height = full_height;
			ccsess->buffer_params.width = width;
			ccsess->buffer_params.height = height;

			ccsess->params.samples = samples;
			ccsess->params.pixel_size = pixel_size;

			// TODO: XXXX remove temporary camera adjustment
			//ccl::Camera *cam = session->scene->camera;
			//cam->set_full_width(full_width);
			//cam->set_full_height(full_height);
			//cam->compute_auto_viewplane();
			//cam->need_flags_update = true;
			//cam->update(session->scene);

			session->reset(ccsess->params, ccsess->buffer_params);
		}
		catch (CyclesRenderCrashException)
		{
			rc = -13;
		}
		catch (...)
		{
			rc = -13;
		}
	}
	return rc;
}

CCL_CAPI void CDECL cycles_session_cancel(ccl::Session* session_id, const char *cancel_message)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit("Cancel session ", session_id, " with message ", cancel_message);
		session->progress.set_cancel(std::string(cancel_message));
	}
}

CCL_CAPI void CDECL cycles_session_quickcancel(ccl::Session* sessionPtr)
{
	sessionPtr->cancel(true);
}

CCL_CAPI void CDECL cycles_session_start(ccl::Session* session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit("Starting session ", session_id);
		session->start();
	}
}

CCL_CAPI void CDECL cycles_session_wait(ccl::Session* session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit("Waiting for session ", session_id);
		session->wait();
	}
}

CCL_CAPI void CDECL cycles_session_set_pause(ccl::Session* session_id, bool pause)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		session->set_pause(pause);
	}
}

CCL_CAPI void CDECL cycles_session_set_samples(ccl::Session* session_id, int samples)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		session->set_samples(samples);
	}
}

CCL_CAPI void CDECL cycles_session_retain_float_buffer(
	ccl::Session *session_id, int passtype, int width, int height, float **pixels, int* pixel_size)
{
	CCSession *ccsess = nullptr;
	ccl::Session *session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		if (ccsess) {
			for (auto &pass : ccsess->passes) {
				if (passtype == pass->get_pass_type() && width == pass->get_width() &&
					height == pass->get_height()) {
					pass->lock();
					*pixels = pass->pixels().data();
					*pixel_size = pass->get_pixel_size();
					break;
				}
			}
		}
	}
}

CCL_CAPI void CDECL cycles_session_release_float_buffer(ccl::Session *session_id,
										 int passtype)
{
	CCSession *ccsess = nullptr;
	ccl::Session *session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		if (ccsess) {
			for (auto &pass : ccsess->passes) {
				if (passtype == pass->get_pass_type()) {
					pass->unlock();
					break;
				}
			}
		}
	}
}

CCL_CAPI void CDECL cycles_progress_reset(ccl::Session *session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		session->progress.reset();
	}
}

CCL_CAPI int CDECL cycles_progress_get_sample(ccl::Session* session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		return session->progress.get_current_sample();
	}
	return INT_MIN;
}

CCL_CAPI int CDECL cycles_progress_get_rendered_tiles(ccl::Session *session_id)
{
	CCSession *ccsess = nullptr;
	ccl::Session *session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		return session->progress.get_rendered_tiles();
	}
	return INT_MIN;
}

CCL_CAPI void CDECL cycles_progress_get_time(ccl::Session* session_id, double *total_time, double* sample_time)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		return session->progress.get_time(*total_time, *sample_time);
	}
}

/* Get cycles render progress. Note that progress will be clamped to 1.0f. */
CCL_CAPI void CDECL cycles_progress_get_progress(ccl::Session* session_id, float* progress)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		*progress = session->progress.get_progress();
		if (*progress > 1.0f) *progress = 1.0f;
	}
}

CCL_CAPI void* CDECL cycles_string_holder_new()
{
	return new StringHolder();
}

CCL_CAPI void CDECL cycles_string_holder_delete(void* strholder)
{
	StringHolder* holder = (StringHolder*)strholder;
	delete holder;
	holder = nullptr;
}

CCL_CAPI const char* CDECL cycles_string_holder_get(void* strholder)
{
	StringHolder* holder = (StringHolder*)strholder;
	if(holder!=nullptr) {
		return holder->thestring.c_str();
	}
	return "";
}

CCL_CAPI bool CDECL cycles_progress_get_status(ccl::Session* session_id, void* strholder)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		StringHolder* holder = (StringHolder*)strholder;
		std::string substatus{ "" };
		session->progress.get_status(holder->thestring, substatus);
		return true;
	}

	return false;
}

CCL_CAPI bool CDECL cycles_progress_get_substatus(ccl::Session* session_id, void* strholder)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		StringHolder* holder = (StringHolder*)strholder;
		std::string status{ "" };
		session->progress.get_status(status, holder->thestring);
		return true;
	}

	return false;
}

#ifdef __cplusplus
}
#endif
