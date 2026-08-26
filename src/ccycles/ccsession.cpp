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
#include <filesystem>
#include <cstdlib>
#include <numeric>
#include <random>

namespace fs = std::filesystem;

#ifdef _WIN32
#include <eh.h>
#include <exception>
#else
#include <signal.h>
#endif

#include "internal_types.h"
#include "scene/background.h"
#include "scene/shader_nodes.h"
#include "scene/shader_graph.h"

extern "C" CCL_CAPI void CDECL cycles_debug_scene_stats(ccl::Session *session_id);
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

/* Find pointers for CCSession and ccl::Session. Return false if either fails. */
bool session_find(ccl::Session* sid, CCSession** ccsess, ccl::Session** session)
{
  auto sidhit = [sid](CCSession* i) { return i->session == sid; };
  auto found = std::find_if(sessions.cbegin(), sessions.cend(), sidhit);
	ccl::thread_scoped_lock lock(session_mutex);
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
				ccycles_diag("get_pass_pixels failed for tile pass '%s'\n",
				             pass_type_as_string(pass_type));
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
		/* TEMPORARY: with a shadow catcher in the scene Cycles writes the picture
		 * across combined, shadow_catcher_matte, shadow_catcher and background,
		 * and reading only "combined" can legitimately give black. Report what is
		 * in each of them once, at the last sample. */
		/* Shader flags like emission_estimate and has_surface_spatial_varying are
		 * only filled in by ShaderManager::device_update, which runs after
		 * session start - reading them in cycles_debug_scene_stats gives zeros
		 * for every shader and means nothing. Report them once from here, where
		 * compilation has definitely happened. */
		static const bool want_shader_flags = getenv("CCYCLES_DIAG_LOG") != nullptr;
		if (want_shader_flags) {
			static bool shader_flags_done = false;
			if (!shader_flags_done && ccsession_ != nullptr && ccsession_->session != nullptr) {
				shader_flags_done = true;
				ccl::Scene *sc = ccsession_->session->scene.get();
				if (sc != nullptr) {
					for (size_t si = 0; si < sc->shaders.size(); si++) {
						ccl::Shader *sh = sc->shaders[si];
						ccycles_diag("compiled shader %zu '%s' emission=(%f %f %f) "
						             "sampling=%d surface=%d spatial_varying=%d "
						             "light_path=%d const_emission=%d\n",
						             si, sh->name.c_str(), sh->emission_estimate.x,
						             sh->emission_estimate.y, sh->emission_estimate.z,
						             (int)sh->emission_sampling, (int)sh->has_surface,
						             (int)sh->has_surface_spatial_varying,
						             (int)sh->has_light_path_node,
						             (int)sh->emission_is_constant);
					}
				}
			}
		}
		static const bool want_probe = getenv("CCYCLES_PASS_PROBE") != nullptr;
		if (want_probe && tile.get_sample() > 1) {
			static bool probed = false;
			if (!probed) {
				probed = true;
				const char *names[] = {"combined", "shadow_catcher_matte",
				                       "shadow_catcher", "background"};
				const int w = tile.full_size.x, h = tile.full_size.y;
				std::vector<float> probe(size_t(w) * size_t(h) * 4);
				for (const char *nm : names) {
					if (!tile.get_pass_pixels(nm, 4, probe.data())) {
						ccycles_diag("probe: pass '%s' unavailable\n", nm);
						continue;
					}
					float lo = probe[0], hi = probe[0];
					double sum = 0.0;
					size_t nz = 0;
					for (float v : probe) {
						if (v < lo) lo = v;
						if (v > hi) hi = v;
						sum += v;
						if (v != 0.0f) nz++;
					}
					ccycles_diag("probe: '%s' %dx%d nonzero=%zu min=%f max=%f mean=%f\n",
					             nm, w, h, nz, lo, hi, sum / double(probe.size()));
				}
			}
		}
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
				ccycles_diag("get_pass_pixels failed for tile pass '%s'\n",
				             pass_type_as_string(pass->get_pass_type()));
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
	ccl::Camera *cam = session->scene->camera;
	cam->set_full_height(512);
	cam->set_full_width(512);
	cam->compute_auto_viewplane();
	cam->need_flags_update = true;
	cam->update(session->scene.get());

	session->set_output_driver(std::make_unique<CCyclesOutputDriver>(passes, log_print, ccsession));

	ccl::Scene *scene = session->scene.get();
	ccl::Integrator *integrator = scene->integrator;

	integrator->set_use_light_tree(true);
	integrator->set_light_sampling_threshold(0.01f);
	integrator->set_use_adaptive_sampling(true);
	integrator->set_adaptive_min_samples(1);
	integrator->set_adaptive_threshold(0.01f);
	integrator->set_denoiser_type(ccl::DENOISER_NONE);
	integrator->set_guiding_distribution_type(ccl::GUIDING_TYPE_DIRECTIONAL_QUAD_TREE);

	// This needs to be here (for now) so that the node will register itself
	// through the dynamic initialization of the global variable. If not here
	// compiler will optimize away the code in the .cpp file.
	AzimuthAltitudeTransformNode derp;

	{
		scene->background->set_transparent_glass(true);
		Shader *bgsh = scene->default_background;
		ccl::unique_ptr<ccl::ShaderGraph> graph = ccl::make_unique<ccl::ShaderGraph>();
		ccl::OutputNode *out = graph->output();
		ccl::BackgroundNode *shn = graph->create_node<ccl::BackgroundNode>();
		{
			std::random_device r;
			std::mt19937 gen(r());	 // Standard mersenne_twister_engine seeded with rd()
			std::uniform_real_distribution<> dist(0.0, 1.0);
			ccl::BackgroundNode *bgn = (ccl::BackgroundNode *)shn;
			bgn->set_color(ccl::make_float3(dist(gen), dist(gen), dist(gen)));
			bgn->set_strength(1.5f);
		}
		graph->connect(shn->output("Background"), out->input("Surface"));
		bgsh->set_graph(std::move(graph));
		bgsh->tag_update(scene);
	}

	{
		auto default_surface_shader = scene->default_surface;
		ccl::unique_ptr<ccl::ShaderGraph> graph = ccl::make_unique<ccl::ShaderGraph>();
		auto out = graph->output();
		ccl::DiffuseBsdfNode *shader_node = graph->create_node<ccl::DiffuseBsdfNode>();
		{
			std::random_device r;
			std::mt19937 gen(r());	 // Standard mersenne_twister_engine seeded with rd()
			std::uniform_real_distribution<> dist(0.0, 1.0);
			auto diff = (ccl::DiffuseBsdfNode *)shader_node;
			diff->set_color(ccl::make_float3(dist(gen), dist(gen), dist(gen)));
			diff->set_roughness(1.0f);
		}
		graph->connect(shader_node->output("BSDF"), out->input("Surface"));
		default_surface_shader->set_graph(std::move(graph));
		default_surface_shader->tag_update(scene);

		session->scene->shader_manager->set_rhino_perlin_noise_table(ccycles_rhino_perlin_noise_table);
		session->scene->shader_manager->set_rhino_impulse_noise_table(ccycles_rhino_impulse_noise_table);
		session->scene->shader_manager->set_rhino_vc_noise_table(ccycles_rhino_vc_noise_table);
		session->scene->shader_manager->set_rhino_aaltonen_noise_table(ccycles_rhino_aaltonen_noise_table);
	}
}

#ifdef __cplusplus
extern "C" {
#endif

CCL_CAPI ccl::Session* CDECL cycles_session_create(ccl::SessionParams* _session_parameters)
{
	ccl::thread_scoped_lock lock(session_mutex);

	ccl::SessionParams *params = (*(session_params.find(_session_parameters)));
	if (params == nullptr)
		return nullptr;

	int csesid{ -1 };
	int hid{ 0 };

	CCSession* session = CCSession::create(10, 10, 4);

	// TODO: XXXX these are hardcoded params/sceneparams
	session->params = *params;
	session->params.tile_size = 512;
	session->params.use_auto_tile = false;
	/* SessionParams::experimental was removed in 5.2. */
	session->params.shadingsystem = ccl::SHADINGSYSTEM_SVM;

	session->scene_params.shadingsystem = ccl::SHADINGSYSTEM_SVM;

	session->session = new ccl::Session(session->params, session->scene_params);

	prep_session(session->session, &session->passes, session);

	sessions.insert(session);
	csesid = (unsigned int)(sessions.size() - 1);

	return session->session;
}

CCL_CAPI void CDECL cycles_session_destroy(ccl::Session* session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		sessions.erase(ccsess);
		if (auto search = session_params.find(&ccsess->params); search != session_params.end()) {
			session_params.erase(*search);
			delete *search;
		}
		delete ccsess;
	}
}

CCL_CAPI void CDECL cycles_session_clear_passes(ccl::Session* session_id)
{
	/* 5.2: Scene::passes is a unique_ptr_vector; index it. */
	for (size_t pi = session_id->scene->passes.size(); pi-- > 0;) {
		ccl::Pass *pass = session_id->scene->passes[pi];
		session_id->scene->delete_node(pass);
	}


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
			//cam->update(session->scene.get());

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

		/* Experiment switch: Rhino's background graph is thirty-odd nodes with
		 * light_path gating, so "the environment does not light anything" could
		 * be that graph or it could be background light sampling underneath.
		 * CCYCLES_SIMPLE_BACKGROUND swaps in the smallest graph that must emit -
		 * one background_shader wired to the output - to tell those apart. */
		/* The background graph is ~40 nodes of light_path gating and the static
		 * socket values do not say which branch a given ray takes. Dump the
		 * topology so the path to final_bg can be traced. */
		/* The background graph feeds skylight colour from an environment texture
		 * (assets/RhinoStudio8.exr) through sky_color_or_texture, whose Fac is a
		 * plain 1.0 selecting the texture over the solid colour. Flipping it to 0
		 * takes the image sampling out of the path while leaving every gate and
		 * factor intact - which separates "the image samples black" from
		 * "the gating picks a black branch". */
		/* Static socket values cannot show what a node actually evaluates to, and
		 * SVM will not tell you either. CCYCLES_BG_TAP=<node name> rewires that
		 * node's first colour output straight into final_bg's Color, so the render
		 * shows what that point in the graph produces. Bisecting a chain with it
		 * takes a handful of runs. */
		const char *tap = getenv("CCYCLES_BG_TAP");
		if (tap != nullptr && tap[0] != 0) {
			ccl::Scene *tsce = session->scene.get();
			ccl::Shader *tsh = (tsce != nullptr)
			                       ? static_cast<ccl::Shader *>(tsce->background->get_shader())
			                       : nullptr;
			if (tsh != nullptr && tsh->graph != nullptr) {
				ccl::ShaderNode *srcnode = nullptr;
				ccl::ShaderNode *bgnode = nullptr;
				for (ccl::ShaderNode *nd : tsh->graph->nodes) {
					if (nd->name == ccl::ustring(tap)) srcnode = nd;
					if (nd->name == ccl::ustring("final_bg")) bgnode = nd;
				}
				if (srcnode == nullptr || bgnode == nullptr) {
					ccycles_diag("tap: node '%s' or final_bg not found\n", tap);
				}
				else {
					ccl::ShaderOutput *from = nullptr;
					for (ccl::ShaderOutput *o : srcnode->outputs) {
						if (o->type() == ccl::SocketType::COLOR ||
						    o->type() == ccl::SocketType::FLOAT ||
						    o->type() == ccl::SocketType::VECTOR ||
						    o->type() == ccl::SocketType::POINT ||
						    o->type() == ccl::SocketType::NORMAL) {
							from = o;
							break;
						}
					}
					ccl::ShaderInput *to = bgnode->input("Color");
					if (from != nullptr && to != nullptr) {
						tsh->graph->disconnect(to);
						tsh->graph->connect(from, to);
						ccycles_diag("tap: final_bg.Color <- %s.%s\n", tap,
						             from->socket_type.name.c_str());
						tsh->tag_update(tsce);
						tsce->background->tag_update(tsce);
					}
					else {
						ccycles_diag("tap: no colour output on '%s'\n", tap);
					}
				}
			}
		}

		const char *sky_from_color = getenv("CCYCLES_BG_SKY_FROM_COLOR");
		if (sky_from_color != nullptr && sky_from_color[0] == 0x31) {
			ccl::Scene *fsce = session->scene.get();
			ccl::Shader *fsh = (fsce != nullptr)
			                       ? static_cast<ccl::Shader *>(fsce->background->get_shader())
			                       : nullptr;
			if (fsh != nullptr && fsh->graph != nullptr) {
				for (ccl::ShaderNode *nd : fsh->graph->nodes) {
					if (nd->name == ccl::ustring("sky_color_or_texture")) {
						ccl::MixNode *mixnd = dynamic_cast<ccl::MixNode *>(nd);
						if (mixnd != nullptr) {
							ccycles_diag("forcing sky_color_or_texture Fac %f -> 0\n",
							             mixnd->get_fac());
							mixnd->set_fac(0.0f);
						}
					}
					/* Report every environment texture and the file it points at. */
					if (ccl::EnvironmentTextureNode *env =
					        dynamic_cast<ccl::EnvironmentTextureNode *>(nd)) {
						ccycles_diag("  env texture '%s' filename='%s'\n",
						             nd->name.c_str(), env->get_filename().c_str());
					}
				}
				fsh->tag_update(fsce);
				fsce->background->tag_update(fsce);
			}
		}

		/* Experiment: 4.x turned specular_tint and sheen_tint from floats into
		 * colours, where untinted is white. A scalar tint amount mapped straight
		 * onto grey therefore asks for no specular at all when the amount is 0.
		 * Forcing both white says how much of the gap against shipping Rhino that
		 * accounts for, before rewiring anything. */
		/* Experiment: with no OCIO config, an "auto" colorspace no longer resolves
		 * to sRGB for byte images - detect_known_colorspace only consults OCIO for
		 * that - so it lands on scene linear and no decode happens. Set this to
		 * data, srgb or linear to force every image texture and measure which way
		 * it moves the render. */
		const char *tex_cs = getenv("CCYCLES_TEX_COLORSPACE");
		if (tex_cs != nullptr && tex_cs[0] != 0) {
			ccl::ustring want;
			if (strcmp(tex_cs, "data") == 0) want = ccl::u_colorspace_data;
			else if (strcmp(tex_cs, "srgb") == 0) want = ccl::u_colorspace_srgb;
			else if (strcmp(tex_cs, "linear") == 0) want = ccl::u_colorspace_scene_linear;
			else ccycles_diag("CCYCLES_TEX_COLORSPACE: unknown value '%s'\n", tex_cs);
			ccl::Scene *csce = session->scene.get();
			int n = 0;
			if (!want.empty() && csce != nullptr) {
				for (ccl::Shader *sh : csce->shaders) {
					if (sh->graph == nullptr) continue;
					for (ccl::ShaderNode *nd : sh->graph->nodes) {
						if (ccl::ImageTextureNode *it =
						        dynamic_cast<ccl::ImageTextureNode *>(nd)) {
							it->set_colorspace(want);
							n++;
						}
					}
					sh->tag_update(csce);
				}
				ccycles_diag("forced colorspace '%s' on %d image texture(s)\n",
				             want.c_str(), n);
			}
		}

		const char *white_tint = getenv("CCYCLES_WHITE_TINTS");
		if (white_tint != nullptr && white_tint[0] == 0x31) {
			ccl::Scene *wsce = session->scene.get();
			int touched = 0;
			if (wsce != nullptr) {
				for (ccl::Shader *sh : wsce->shaders) {
					if (sh->graph == nullptr) continue;
					for (ccl::ShaderNode *nd : sh->graph->nodes) {
						ccl::PrincipledBsdfNode *pb =
						    dynamic_cast<ccl::PrincipledBsdfNode *>(nd);
						if (pb == nullptr) continue;
						ccl::ShaderInput *st = nd->input("Specular Tint");
						ccl::ShaderInput *sn = nd->input("Sheen Tint");
						if (st != nullptr && st->link == nullptr) {
							pb->set_specular_tint(ccl::one_float3());
						}
						if (sn != nullptr && sn->link == nullptr) {
							pb->set_sheen_tint(ccl::one_float3());
						}
						/* A linked tint has to be cut loose or the link wins. */
						if (st != nullptr && st->link != nullptr) {
							sh->graph->disconnect(st);
							pb->set_specular_tint(ccl::one_float3());
						}
						if (sn != nullptr && sn->link != nullptr) {
							sh->graph->disconnect(sn);
							pb->set_sheen_tint(ccl::one_float3());
						}
						touched++;
					}
					sh->tag_update(wsce);
				}
			}
			ccycles_diag("forced white specular/sheen tint on %d principled node(s)\n",
			             touched);
		}

		/* Cut the alpha and transmission chains loose on every principled node.
		 * A material that renders as nothing is either fully transparent or
		 * fully transmissive, and Rhino builds both from long math chains, so
		 * the cheapest way to find out which is to force them opaque and look. */
		const char *force_opaque = getenv("CCYCLES_FORCE_OPAQUE");
		if (force_opaque != nullptr && force_opaque[0] == 0x31) {
			ccl::Scene *osce = session->scene.get();
			int touched = 0, cut = 0;
			if (osce != nullptr) {
				const char *names[] = {"Alpha", "Transmission Weight"};
				for (ccl::Shader *sh : osce->shaders) {
					if (sh->graph == nullptr) continue;
					for (ccl::ShaderNode *nd : sh->graph->nodes) {
						ccl::PrincipledBsdfNode *pb =
						    dynamic_cast<ccl::PrincipledBsdfNode *>(nd);
						if (pb == nullptr) continue;
						for (const char *nm : names) {
							ccl::ShaderInput *in = nd->input(nm);
							if (in == nullptr) continue;
							if (in->link != nullptr) {
								sh->graph->disconnect(in);
								cut++;
							}
						}
						pb->set_alpha(1.0f);
						pb->set_transmission_weight(0.0f);
						touched++;
					}
					sh->tag_update(osce);
				}
			}
			ccycles_diag("forced opaque on %d principled node(s), %d link(s) cut\n",
			             touched, cut);
		}

		const char *dump_bg = getenv("CCYCLES_DUMP_BG");
		if (dump_bg != nullptr && dump_bg[0] != 0) {
			ccl::Scene *dsce = session->scene.get();
			ccl::Shader *dsh = (dsce != nullptr)
			                       ? static_cast<ccl::Shader *>(dsce->background->get_shader())
			                       : nullptr;
			if (dsh != nullptr && dsh->graph != nullptr) {
				dsh->graph->dump_graph(dump_bg);
				ccycles_diag("dumped background graph to %s\n", dump_bg);
			}
		}

		const char *simple_bg = getenv("CCYCLES_SIMPLE_BACKGROUND");
		if (simple_bg != nullptr && simple_bg[0] == 0x31) {
			ccl::Scene *bgsce = session->scene.get();
			ccl::Shader *bgshader =
			    (bgsce != nullptr)
			        ? static_cast<ccl::Shader *>(bgsce->background->get_shader())
			        : nullptr;
			if (bgshader != nullptr) {
				auto graph = ccl::make_unique<ccl::ShaderGraph>();
				ccl::BackgroundNode *bg = graph->create_node<ccl::BackgroundNode>();
				bg->set_color(ccl::make_float3(0.8f, 0.8f, 0.8f));
				bg->set_strength(1.0f);
				graph->connect(bg->output("Background"),
				               graph->output()->input("Surface"));
				bgshader->set_graph(std::move(graph));
				bgshader->tag_update(bgsce);
				bgsce->background->tag_update(bgsce);
				ccycles_diag("replaced background shader '%s' with a plain white "
				             "background_shader\n", bgshader->name.c_str());
			}
		}

		/* Replace every surface shader with one flat diffuse, leaving the
		 * background and the lights alone. This is the geometry-vs-shading
		 * question asked properly: anything that still fails to appear is
		 * genuinely absent from the render rather than shaded into invisibility.
		 * RhinoCycles' own DebugSimpleShaders cannot answer it, because it drives
		 * the diffuse colour from the uvmap1 attribute and so paints anything
		 * with UVs at or above 1 pure white - against a white background that
		 * looks exactly like missing geometry. */
		const char *flat = getenv("CCYCLES_FLAT_SHADERS");
		if (flat != nullptr && flat[0] == 0x31) {
			ccl::Scene *fsce = session->scene.get();
			int flattened = 0, skipped = 0;
			if (fsce != nullptr) {
				const ccl::Shader *bgsh =
				    static_cast<const ccl::Shader *>(fsce->background->get_shader());
				for (ccl::Shader *sh : fsce->shaders) {
					/* Lights carry emissive shaders; flattening those would put the
					 * scene in the dark and answer nothing. They are named by
					 * RhinoCycles, so go by name. */
					const std::string nm = sh->name.string();
					if (sh == bgsh || nm == "light" || nm == "default_light" ||
					    nm == "default_background") {
						skipped++;
						continue;
					}
					auto graph = ccl::make_unique<ccl::ShaderGraph>();
					ccl::DiffuseBsdfNode *d = graph->create_node<ccl::DiffuseBsdfNode>();
					d->set_color(ccl::make_float3(0.55f, 0.55f, 0.55f));
					d->set_roughness(0.0f);
					graph->connect(d->output("BSDF"), graph->output()->input("Surface"));
					sh->set_graph(std::move(graph));
					sh->tag_update(fsce);
					flattened++;
				}
			}
			ccycles_diag("flattened %d shader(s) to grey diffuse, skipped %d\n",
			             flattened, skipped);
		}

		cycles_debug_scene_stats(session_id);
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
			bool found = false;
			for (auto &pass : ccsess->passes) {
				if (passtype == pass->get_pass_type() && width == pass->get_width() &&
					height == pass->get_height()) {
					pass->lock();
					*pixels = pass->pixels().data();
					*pixel_size = pass->get_pixel_size();
					found = true;

					/* TEMPORARY diagnostic: is the pass empty, or is the caller
					 * losing pixels we handed over? */
					{
						const std::vector<float> &px = pass->pixels();
						float lo = 0.0f, hi = 0.0f;
						double sum = 0.0;
						size_t nonzero = 0;
						if (!px.empty()) {
							lo = hi = px[0];
							for (float v : px) {
								if (v < lo) lo = v;
								if (v > hi) hi = v;
								sum += v;
								if (v != 0.0f) nonzero++;
							}
						}
						ccycles_diag("retain_float_buffer: pass %d %dx%d floats=%zu nonzero=%zu "
						             "min=%f max=%f mean=%f pixel_size=%d\n",
						             passtype, width, height, px.size(), nonzero, lo, hi,
						             px.empty() ? 0.0 : sum / double(px.size()), *pixel_size);
					}
					break;
				}
			}
			if (!found) {
				/* Nothing matched, so *pixels keeps whatever the caller passed in.
				 * Say so, and say what was on offer. */
				ccycles_diag("retain_float_buffer: NO PASS for type %d at %dx%d\n",
				             passtype, width, height);
				for (auto &pass : ccsess->passes) {
					ccycles_diag("  have pass type %d at %dx%d\n", pass->get_pass_type(),
					             pass->get_width(), pass->get_height());
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
