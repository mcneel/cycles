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
#include "device/device.h"
#include "util/thread.h"

#include <OpenImageIO/imagebuf.h>
#include <OpenImageIO/imagebufalgo.h>

using namespace ccl;

/* Hold all created sessions. */
std::vector<CCSession*> sessions;

/* Four vectors to hold registered callback functions.
 * For each created session a corresponding idx into these
 * vectors will exist, but in the case no callback
 * was registered the value for idx will be nullptr.
 */
std::vector<STATUS_UPDATE_CB> status_cbs;
std::vector<TEST_CANCEL_CB> cancel_cbs;
std::vector<RENDER_TILE_CB> update_cbs;
std::vector<RENDER_TILE_CB> write_cbs;
std::vector<DISPLAY_UPDATE_CB> display_update_cbs;
std::unordered_map<ccl::Session*, ccl::vector<ccl::Pass>*> passes_vec;

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

static
void render_crash_translator(unsigned int eCode, EXCEPTION_POINTERS*)
{
	throw CyclesRenderCrashException(eCode);
}
#else
typedef void(__cdecl *_se_translator_function)(int, struct __siginfo *, void*);

static void render_crash_translator(int sig, siginfo_t *siginfo, void *context)
{
	throw CyclesRenderCrashException(siginfo->si_code);
}
#endif

class RenderCrashTranslatorHelper
{
private:
	const _se_translator_function old_SE_translator;
public:
	RenderCrashTranslatorHelper(_se_translator_function new_SE_translator) noexcept
#ifdef _WIN32
		: old_SE_translator{ _set_se_translator(new_SE_translator) } {}
#else
		: old_SE_translator{ nullptr } {
#ifdef ENABLE_MAC_CRASHHANDLING
		struct sigaction sa;
		sa.sa_flags = SA_SIGINFO;
		sigemptyset(&sa.sa_mask);
		sa.sa_sigaction = new_SE_translator;
		sigaction(SIGSEGV, &sa, nullptr);
#endif
	}
#endif
#ifdef _WIN32
	~RenderCrashTranslatorHelper() noexcept { _set_se_translator(old_SE_translator); }
#else
	~RenderCrashTranslatorHelper() noexcept {
#ifdef ENABLE_MAC_CRASHHANDLING
		struct sigaction sa;
		sa.sa_flags = SA_SIGINFO;
		sa.sa_handler = SIG_DFL;
		sigaction(SIGSEGV, &sa, nullptr);
#endif
	}
#endif
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

/* Wrap status update callback. */
void CCSession::status_update(void) {
	#if 0
	TODO: XXXX
	if (status_cbs[this->id] != nullptr) {
		status_cbs[this->id](this->id);
	}
	#endif
}

/* Wrap status update callback. */
void CCSession::test_cancel(void) {
	#if 0
	TODO: XXXX
	if (cancel_cbs[this->id] != nullptr) {
		cancel_cbs[this->id](this->id);
	}
	#endif
}

/* Wrapper callback for render tile update. Copies tile result into session full image buffer. */
/*
 void CCSession::update_render_tile(ccl::RenderTile &tile, bool highlight)
{
}
*/

/* Wrapper callback for render tile write. Copies tile result into session full image buffer. */
/*
void CCSession::write_render_tile(ccl::RenderTile &tile)
{
}
*/

/* Wrapper callback for display update stuff. When this is called one pass has been conducted. */
void CCSession::display_update(int sample)
{
	#if 0
	TODO: XXXX
	if (size_has_changed()) return;
	if (display_update_cbs[this->id] != nullptr) {
		display_update_cbs[this->id](this->id, sample);
	}
	#endif
}

/**
 * Clean up resources acquired during this run of Cycles.
 */
void _cleanup_sessions()
{
	for (CCSession* se : sessions) {
		if (se == nullptr) continue;

		{
			delete se->session;
			se->session = nullptr;
		}
		delete se;
	}

	sessions.clear();
	session_params.clear();

	status_cbs.clear();
	cancel_cbs.clear();
	update_cbs.clear();
	write_cbs.clear();
	update_cbs.clear();
}

CCSession* CCSession::create(int width, int height, unsigned int buffer_stride) {
	CCSession* se = new CCSession();
	se->width = width;
	se->height = height;
	se->pixels.resize(width * height * buffer_stride);
	se->_size_has_changed = false;

	return se;
}

bool CCSession::size_has_changed() {
	bool rc = _size_has_changed;
	_size_has_changed = false;
	return rc;
}

CCyclesDebugDriver::CCyclesDebugDriver(CCyclesDebugDriver::LogFunction log) : log_(log) {}

CCyclesDebugDriver::~CCyclesDebugDriver() {}

void CCyclesDebugDriver::write_render_tile(const Tile &tile)
{
  log_(string_printf(
      "Handling tile layer %s, size %d %d", tile.layer.c_str(), tile.size.x, tile.size.y));

	fs::path userprofile(std::getenv("USERPROFILE"));
	fs::path save_path = userprofile / "integration_testrender.png";

	const int width = tile.size.x;
	const int height = tile.size.y;
	vector<float> pixels(width * height * 4);

	if (!tile.get_pass_pixels("combined", 4, pixels.data())) {
		log_("Failed to read render pass pixels");
		return;
	}

	unique_ptr<ImageOutput> image_output(ImageOutput::create("png"));
	if (image_output == nullptr) {
		log_("Failed to create image file. 1");
		return;
	}

	ImageSpec spec(width, height, 4, TypeDesc::FLOAT);
  if (!image_output->open(save_path.string(), spec)) {
		log_(image_output->geterror());
		log_("Failed to create image file.2");
		return;
	}

	/* Manipulate offset and stride to convert from bottom-up to top-down convention. */
	ImageBuf image_buffer(spec,
												pixels.data() + (height - 1) * width * 4,
												AutoStride,
												-width * 4 * sizeof(float),
												AutoStride);

#define DCOLORSPACE
#if defined(DCOLORSPACE)
	/* Apply gamma correction for (some) non-linear file formats.
	 * TODO: use OpenColorIO view transform if available. */
	if (ColorSpaceManager::detect_known_colorspace(
					u_colorspace_auto, "", image_output->format_name(), true) == u_colorspace_srgb) {
		const float g = 1.0f / 2.2f;
		ImageBufAlgo::pow(image_buffer, image_buffer, {g, g, g, 1.0f});
	}
#endif

	/* Write to disk and close */
	image_buffer.set_write_format(TypeDesc::FLOAT);
	image_buffer.write(image_output.get());
	image_output->close();
}

CCyclesOutputDriver::CCyclesOutputDriver(std::mutex *pixels_mutex,
										 std::vector<float> *pixels,
										 CCyclesOutputDriver::LogFunction log)
	: pixels_mutex(pixels_mutex)
	, pixels(pixels)
	, log_(log)
{
}

CCyclesOutputDriver::~CCyclesOutputDriver()
{
}

void CCyclesOutputDriver::write_render_tile(const Tile &tile)
{
	if (pixels_mutex == nullptr)
		return;

	if (pixels == nullptr)
		return;

	log_(string_printf(
		"Handling tile layer %s, size %d %d", tile.layer.c_str(), tile.size.x, tile.size.y));

	pixels_mutex->lock();

	const int width = tile.size.x;
	const int height = tile.size.y;
	pixels->resize(width * height * 4);

	if (!tile.get_pass_pixels("combined", 4, pixels->data())) {
		log_("Failed to read render pass pixels");
		return;
	}

	pixels_mutex->unlock();
}

bool CCyclesOutputDriver::update_render_tile(const Tile &tile)
{
	if (pixels_mutex == nullptr)
		return false;

	if (pixels == nullptr)
		return false;

	log_(string_printf(
		"Handling tile layer %s, size %d %d", tile.layer.c_str(), tile.size.x, tile.size.y));

	pixels_mutex->lock();

	const int width = tile.size.x;
	const int height = tile.size.y;
	pixels->resize(width * height * 4);

	if (!tile.get_pass_pixels("combined", 4, pixels->data())) {
		log_("Failed to read render pass pixels");
		return false;
	}

	pixels_mutex->unlock();

	return true;
}

CCyclesDisplayDriver::CCyclesDisplayDriver(std::mutex *pixels_mutex,
										   std::vector<float> *float_pixels,
										   CCyclesDisplayDriver::LogFunction log)
	: pixels_mutex(pixels_mutex)
	, pixels(pixels)
	, log_(log)
{
}

CCyclesDisplayDriver::~CCyclesDisplayDriver()
{
}

void CCyclesDisplayDriver::next_tile_begin()
{
	int a = 0;
}

bool CCyclesDisplayDriver::update_begin(const Params &params, int width, int height)
{
	size_t pixel_count = width * height;
	pixels_half4.resize(pixel_count);

	return true;
}

void CCyclesDisplayDriver::update_end()
{
	if (pixels_mutex == nullptr)
		return;

	if (pixels == nullptr)
		return;

	pixels_mutex->lock();

	pixels->resize(pixels_half4.size() * 4);

	for (int i = 0, p = 0; i < pixels_half4.size(); i++) {
		(*pixels)[p++] = half_to_float_image(pixels_half4[i].x);
		(*pixels)[p++] = half_to_float_image(pixels_half4[i].y);
		(*pixels)[p++] = half_to_float_image(pixels_half4[i].z);
		(*pixels)[p++] = half_to_float_image(pixels_half4[i].w);
	}

	pixels_mutex->unlock();
}

ccl::half4 *CCyclesDisplayDriver::map_texture_buffer()
{
	return pixels_half4.data();
}

void CCyclesDisplayDriver::unmap_texture_buffer()
{
	int a = 0;
}

void CCyclesDisplayDriver::clear()
{
	static ccl::half halfzero = float_to_half_image(0.0f);
	static ccl::half halfone = float_to_half_image(1.0f);

	for (int i = 0; i < pixels_half4.size(); i++) {
		pixels_half4[i].x = halfzero;
		pixels_half4[i].y = halfzero;
		pixels_half4[i].z = halfzero;
		pixels_half4[i].w = halfone;
	}
}

void CCyclesDisplayDriver::draw(const Params &params)
{
	int a = 0;
}

CCyclesDisplayDriver::GraphicsInterop CCyclesDisplayDriver::graphics_interop_get()
{
	int a = 0;
	return GraphicsInterop();
}

void CCyclesDisplayDriver::graphics_interop_activate()
{
	int a = 0;
}

void CCyclesDisplayDriver::graphics_interop_deactivate()
{
	int a = 0;
}

static void log_print(const std::string& msg)
{
	std::cout << msg << std::endl;
	OutputDebugString(msg.c_str());
	OutputDebugString("\n");
}

static void prep_session(ccl::Session *session,
						 std::mutex *pixels_mutex,
						 std::vector<float> *output_pixels)
{
	ccl::Camera *cam = session->scene->camera;
	cam->set_full_height(512);
	cam->set_full_width(512);
	cam->compute_auto_viewplane();
	cam->need_flags_update = true;
	cam->update(session->scene);


	//session->set_output_driver(std::make_unique<CCyclesDebugDriver>(log_print));
	session->set_output_driver(
		std::make_unique<CCyclesOutputDriver>(pixels_mutex, output_pixels, log_print));
	//session->set_display_driver(
	//	std::make_unique<CCyclesDisplayDriver>(pixels_mutex, output_pixels, log_print));

	/* add pass for output. */
	ccl::Pass *pass = session->scene->create_node<ccl::Pass>();
	pass->set_name(ustring("combined"));
	pass->set_type(PASS_COMBINED);

	ccl::Scene *scene = session->scene;
	ccl::Integrator *integrator = scene->integrator;

	integrator->set_use_adaptive_sampling(false);
	integrator->set_denoiser_type(ccl::DENOISER_NONE);
	integrator->set_guiding_distribution_type(ccl::GUIDING_TYPE_DIRECTIONAL_QUAD_TREE);

	scene->background->set_transparent_glass(true);
	Shader *bgsh = scene->default_background;
	ccl::ShaderGraph *graph = new ccl::ShaderGraph();
	ccl::OutputNode *out = graph->output();
	ustring nodename("background_shader");
	ccl::ShaderNode *shn = nullptr;
	const ccl::NodeType *ntype = ccl::NodeType::find(nodename);
	shn = (ShaderNode *)ntype->create(ntype);
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
}

ccl::Session* cycles_session_create(ccl::SessionParams* session_params_id)
{
	ccl::thread_scoped_lock lock(session_mutex);
	ccl::SessionParams *params = (*(session_params.find(session_params_id)));

	int csesid{ -1 };
	int hid{ 0 };

	CCSession* session = CCSession::create(10, 10, 4);

	// TODO: XXXX these are hardcoded params/sceneparams
	session->params.background = true;
	session->params.tile_size = 2048;
	session->params.use_auto_tile = true;
	session->params.experimental = true;
	session->params.samples = 50;
	session->params.shadingsystem = ccl::SHADINGSYSTEM_SVM;
	session->params.time_limit = 10.0f;
	session->params.use_resolution_divider = false;

	ccl::DeviceType device_type = ccl::Device::type_from_string("CPU");
	ccl::vector<ccl::DeviceInfo> devices = ccl::Device::available_devices(
			DEVICE_MASK(device_type));
	session->params.device = devices.front();

	session->scene_params.shadingsystem = ccl::SHADINGSYSTEM_SVM;

	session->session = new ccl::Session(session->params, session->scene_params);

	prep_session(session->session, &session->pixels_mutex, &session->pixels);

	ccl::BufferParams bparam;
	bparam.width = 512;
	bparam.height = 512;
	bparam.full_width = 512;
	bparam.full_height = 512;

	//session->session->reset(*params, bparam);

	for(CCSession* csess : sessions) {
		if(csess==nullptr) {
			csesid = hid;
			break;
		}
		hid++;
	}

	if (csesid == -1) {
		sessions.push_back(session);
		csesid = (unsigned int)(sessions.size() - 1);
		status_cbs.push_back(nullptr);
		cancel_cbs.push_back(nullptr);
		update_cbs.push_back(nullptr);
		write_cbs.push_back(nullptr);
		display_update_cbs.push_back(nullptr);
		passes_vec.insert(std::make_pair(session->session, new ccl::vector<ccl::Pass>()));
	}
	else {
		sessions[csesid] = session;
		status_cbs[csesid] = nullptr;
		update_cbs[csesid] = nullptr;
		write_cbs[csesid] = nullptr;
		display_update_cbs[csesid] = nullptr;
		passes_vec[session->session]->clear();
	}


	session->id = csesid;

	return session->session;
}

void cycles_session_set_scene(ccl::Session* session_id, unsigned int scene_id)
{
	// TODO: XXXX Session creation now handles scene creation etc
	/*CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		CCScene* csce = nullptr;
		ccl::Scene* sce = nullptr;
		if (scene_find(scene_id, &csce, &sce)) {
			session->scene = sce;
		}
	}*/
}

void cycles_session_destroy(ccl::Session* session_id, unsigned int scene_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		/* CCScene *csce = nullptr;
		ccl::Scene* sce = nullptr;
		if (scene_find(scene_id, &csce, &sce))
		{
			if(session->scene == sce) {
				csce->scene = nullptr;
				delete csce;
				csce = nullptr;
				set_ccscene_null(scene_id);
			}
		}
		*/

		sessions[ccsess->id] = nullptr;
		delete ccsess;


	}
}

ccl::vector<ccl::Pass>& get_passes(ccl::Session* session_id) {
	ccl::vector<ccl::Pass>* passes = passes_vec[session_id];

	return *passes;
}

void cycles_session_clear_passes(ccl::Session* session_id)
{
	ccl::vector<ccl::Pass>& passes = get_passes(session_id);
	passes.clear();
}

void cycles_session_add_pass(ccl::Session* session_id, int pass_id)
{
	ccl::PassType passtype = (ccl::PassType)pass_id;
	ccl::vector<ccl::Pass>& passes = get_passes(session_id);
  /*
	switch (passtype) {
		case ccl::PASS_COMBINED:
			ccl::Pass::add(passtype, passes, "Combined");
			break;
		case ccl::PASS_DEPTH:
			ccl::Pass::add(passtype, passes, "Depth");
			break;
		case ccl::PASS_NORMAL:
			ccl::Pass::add(passtype, passes, "Normal");
			break;
		case ccl::PASS_DIFFUSE_COLOR:
			ccl::Pass::add(passtype, passes, "DiffCol");
			break;
		case ccl::PASS_OBJECT_ID:
			ccl::Pass::add(passtype, passes, "IndexOB");
			break;
		case ccl::PASS_MATERIAL_ID:
			ccl::Pass::add(passtype, passes, "IndexMA");
			break;
		case ccl::PASS_UV:
			ccl::Pass::add(passtype, passes, "UV");
			break;
		default:
			break;
	}
		*/
}


int cycles_session_reset(ccl::Session* session_id, unsigned int width, unsigned int height, unsigned int samples, unsigned int full_x, unsigned int full_y, unsigned int full_width, unsigned int full_height )
{
	RenderCrashTranslatorHelper render_crash_helper(render_crash_translator);

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

			ccsess->pixels.reserve(full_width * full_height * 4);

			ccsess->params.samples = samples;

			// TODO: XXXX remove temporary camera adjustment
			ccl::Camera *cam = session->scene->camera;
			cam->set_full_width(full_width);
			cam->set_full_height(full_height);
			cam->compute_auto_viewplane();
			cam->need_flags_update = true;
			cam->update(session->scene);


			// TODO: XXXX Passes rework
			/*
			ccl::vector<ccl::Pass>& passes = get_passes(session_id);
			session->scene->film->tag_passes_update(session->scene, passes);
			session->scene->film->display_pass = ccl::PassType::PASS_COMBINED;
			session->scene->film->tag_update(session->scene);

			ccsess->buffer_params.passes = passes;
			*/
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

void cycles_session_set_update_callback(ccl::Session* session_id, void(*update)(unsigned int sid))
{
	#if 0
  TODO: XXXX
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		CCSession* se = sessions[session_id];
		status_cbs[session_id] = update;
		if (update != nullptr) {
			session->progress.set_update_callback(function_bind<void>(&CCSession::status_update, se));
		}
		else {
			session->progress.set_update_callback(nullptr);
		}
		logger.logit("Set status update callback for session ", session_id);
	}
	#endif
}

void cycles_session_set_cancel_callback(ccl::Session* session_id, void(*cancel)(unsigned int sid))
{
  /*
  TODO: XXXX
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		CCSession* se = sessions[session_id];
		cancel_cbs[session_id] = cancel;
		if (cancel != nullptr) {
			session->progress.set_cancel_callback(function_bind<void>(&CCSession::test_cancel, se));
		}
		else {
			session->progress.set_cancel_callback(nullptr);
		}
		logger.logit("Set status cancel callback for session ", session_id);
	}
	*/
}

void cycles_session_set_update_tile_callback(ccl::Session* session_id, RENDER_TILE_CB update_tile_cb)
{
  /*
  TODO: XXXX
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		update_cbs[session_id] = update_tile_cb;
		if (update_tile_cb != nullptr) {
			//session->update_render_tile_cb = function_bind<void>(&CCSession::update_render_tile, ccsess, std::placeholders::_1, std::placeholders::_2);
		}
		else {
			//session->update_render_tile_cb = nullptr;
		}
		logger.logit("Set render tile update callback for session ", session_id);
	}
	*/
}

void cycles_session_set_write_tile_callback(ccl::Session* session_id, RENDER_TILE_CB write_tile_cb)
{
#if 0
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		write_cbs[session_id] = write_tile_cb;
		if (write_tile_cb != nullptr) {
			session->write_render_tile_cb = function_bind<void>(&CCSession::write_render_tile, ccsess, std::placeholders::_1);
		}
		else {
			session->write_render_tile_cb = nullptr;
		}
		logger.logit("Set render tile write callback for session ", session_id);
	}
#endif
}

void cycles_session_set_display_update_callback(ccl::Session* session_id, DISPLAY_UPDATE_CB display_update_cb)
{
#if 0
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		CCSession* se = sessions[session_id];
		display_update_cbs[session_id] = display_update_cb;
		if (display_update_cb != nullptr) {
			session->display_update_cb = function_bind<void>(&CCSession::display_update, ccsess, std::placeholders::_1);
		}
		else {
			session->display_update_cb = nullptr;
		}
		logger.logit("Set display update callback for session ", session_id);
	}
#endif
}

void cycles_session_cancel(ccl::Session* session_id, const char *cancel_message)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit("Cancel session ", session_id, " with message ", cancel_message);
		session->progress.set_cancel(std::string(cancel_message));
	}
}

void cycles_session_start(ccl::Session* session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit("Starting session ", session_id);
		session->start();
	}
}

void cycles_session_prepare_run(ccl::Session* session_id)
{
		// TODO: XXXX revisit session running
		/*
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit("Preparing run for session ", session_id);
		session->prepare_run(ccsess->buffer_params, ccsess->params.samples);
	}
		*/
}

void cycles_session_end_run(ccl::Session* session_id)
{
		// TODO: XXXX revisit session running
		/*
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit("Ending run for session ", session_id);
		session->end_run();
	}
		*/
}


int cycles_session_sample(ccl::Session* session_id)
{
		// TODO: XXXX revisit rendering. check output driver
		/*
	RenderCrashTranslatorHelper render_crash_helper(render_crash_translator);

	try {
		int rc = -1;
		CCSession* ccsess = nullptr;
		ccl::Session* session = nullptr;
		if (session_find(session_id, &ccsess, &session)) {
			logger.logit("Starting session ", session_id);
			rc = session->sample();
		}
		return rc;
	}
	catch (CyclesRenderCrashException)
	{
		return -13;
	}
	catch (...)
	{
		return -13;
	}
		*/
	return 1;
}

void cycles_session_wait(ccl::Session* session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit("Waiting for session ", session_id);
		session->wait();
	}
}

void cycles_session_set_pause(ccl::Session* session_id, bool pause)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		session->set_pause(pause);
	}
}

bool cycles_session_is_paused(ccl::Session* session_id)
{
/*
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		return false; // session->is_paused();
	}*/

	return false;
}

void cycles_session_set_samples(ccl::Session* session_id, int samples)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		session->set_samples(samples);
	}
}

void cycles_session_get_buffer_info(ccl::Session* session_id, unsigned int* buffer_size, unsigned int* buffer_stride)
{
	*buffer_size = 0;
	*buffer_stride = 0;
}

float* cycles_session_get_buffer(ccl::Session* session_id)
{
	return nullptr;
}

void cycles_session_copy_buffer(ccl::Session* session_id, float* pixel_buffer)
{
}

void cycles_session_get_float_buffer(ccl::Session* session_id, int passtype, float** pixels)
{
	//CCSession* ccsess = nullptr;
	//ccl::Session* session = nullptr;
	//if (session_find(session_id, &ccsess, &session)) {
	//	if (ccsess)
	//	{
	//		ccsess->pixels_mutex.lock();
	//		*pixels = ccsess->pixels.data();
	//		ccsess->pixels_mutex.unlock();
	//	}
	//}
}

void cycles_session_retain_float_buffer(ccl::Session *session_id,
										int passtype,
										float **pixels)
{
	CCSession *ccsess = nullptr;
	ccl::Session *session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		if (ccsess) {
			ccsess->pixels_mutex.lock();
			*pixels = ccsess->pixels.data();
		}
	}
}

void cycles_session_release_float_buffer(ccl::Session *session_id,
										 int passtype)
{
	CCSession *ccsess = nullptr;
	ccl::Session *session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		if (ccsess) {
			ccsess->pixels_mutex.unlock();
		}
	}
}

void cycles_progress_reset(ccl::Session *session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		session->progress.reset();
	}
}

int cycles_progress_get_sample(ccl::Session* session_id)
{
		// TODO: revisit result acquisition
		/*
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		ccl::TileManager &tm = session->tile_manager;
		return tm.state.sample;
	}
		*/
	return INT_MIN;
}

void cycles_progress_get_time(ccl::Session* session_id, double *total_time, double* sample_time)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		return session->progress.get_time(*total_time, *sample_time);
	}
}

void cycles_tilemanager_get_sample_info(ccl::Session* session_id, unsigned int* samples, unsigned int* total_samples)
{
		// TODO: XXXX revisit rendering and sampling
		/*
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		*samples = session->tile_manager.state.sample + 1;
		*total_samples = session->tile_manager.num_samples;
	}
		*/
}

/* Get cycles render progress. Note that progress will be clamped to 1.0f. */
void cycles_progress_get_progress(ccl::Session* session_id, float* progress)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		*progress = session->progress.get_progress();
		if (*progress > 1.0f) *progress = 1.0f;
	}
}

class StringHolder
{
public:
	std::string thestring;
};

void* cycles_string_holder_new()
{
	return new StringHolder();
}

void cycles_string_holder_delete(void* strholder)
{
	StringHolder* holder = (StringHolder*)strholder;
	delete holder;
	holder = nullptr;
}

const char* cycles_string_holder_get(void* strholder)
{
	StringHolder* holder = (StringHolder*)strholder;
	if(holder!=nullptr) {
		return holder->thestring.c_str();
	}
	return "";
}

bool cycles_progress_get_status(ccl::Session* session_id, void* strholder)
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

bool cycles_progress_get_substatus(ccl::Session* session_id, void* strholder)
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
