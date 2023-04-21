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

#ifdef _WIN32
#include <eh.h>
#include <exception>
#else
#include <signal.h>
#endif

#include "internal_types.h"
#include "device/device.h"
#include "util/thread.h"

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
std::vector<ccl::vector<ccl::Pass>*> passes_vec;

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
bool session_find(unsigned int sid, CCSession** ccsess, ccl::Session** session)
{
	ccl::thread_scoped_lock lock(session_mutex);
	if (0 <= (sid) && (sid) < sessions.size()) {
		*ccsess = sessions[sid];
		if(*ccsess!=nullptr) *session = (*ccsess)->session;
		return *ccsess!=nullptr && *session!=nullptr;
	}
	return false;
}

/* Wrap status update callback. */
void CCSession::status_update(void) {
	if (status_cbs[this->id] != nullptr) {
		status_cbs[this->id](this->id);
	}
}

/* Wrap status update callback. */
void CCSession::test_cancel(void) {
	if (cancel_cbs[this->id] != nullptr) {
		cancel_cbs[this->id](this->id);
	}
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
	if (size_has_changed()) return;
	if (display_update_cbs[this->id] != nullptr) {
		display_update_cbs[this->id](this->id, sample);
	}
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
	log_(string_printf("Handling tile layer %d, size %d %d", tile.layer, tile.size.x, tile.size.y));
}

static void log_print(const std::string& msg)
{
	std::cout << msg << std::endl;
	OutputDebugString(msg.c_str());
	OutputDebugString("\n");
}

unsigned int cycles_session_create(unsigned int client_id, unsigned int session_params_id)
{
	ccl::thread_scoped_lock lock(session_mutex);
	ccl::SessionParams* params = nullptr;
	if (session_params_id < session_params.size()) {
		params = session_params[session_params_id];
	}

	int csesid{ -1 };
	int hid{ 0 };

	CCSession* session = CCSession::create(512, 512, 4);

	// TODO: XXXX these are hardcoded params/sceneparams
	session->params.background = true;
	session->params.tile_size = 64;
	session->params.use_auto_tile = true;

	ccl::DeviceType device_type = ccl::Device::type_from_string("CPU");
	ccl::vector<ccl::DeviceInfo> devices = ccl::Device::available_devices(
			DEVICE_MASK(device_type));
	session->params.device = devices.front();

	session->scene_params.shadingsystem = ccl::SHADINGSYSTEM_SVM;

	session->session = new ccl::Session(session->params, session->scene_params);

	session->session->scene->camera->set_full_height(512);
	session->session->scene->camera->set_full_width(512);
	session->session->scene->camera->compute_auto_viewplane();
	session->session->set_output_driver(std::make_unique<CCyclesDebugDriver>(log_print));

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
		passes_vec.push_back(new ccl::vector<ccl::Pass>());
	}
	else {
		sessions[csesid] = session;
		status_cbs[csesid] = nullptr;
		update_cbs[csesid] = nullptr;
		write_cbs[csesid] = nullptr;
		display_update_cbs[csesid] = nullptr;
		passes_vec[csesid]->clear();
	}


	session->id = csesid;

	logger.logit(client_id, "Created session ", session->id, " with session_params ", session_params_id);

	return session->id;
}

void cycles_session_set_scene(unsigned int client_id, unsigned int session_id, unsigned int scene_id)
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

void cycles_session_destroy(unsigned int client_id, unsigned int session_id, unsigned int scene_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		CCScene* csce = nullptr;
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

		delete ccsess;

		sessions[session_id] = nullptr;
	}
}

ccl::vector<ccl::Pass>& get_passes(unsigned int session_id) {
	ccl::vector<ccl::Pass>* passes = passes_vec[session_id];

	return *passes;
}

void cycles_session_clear_passes(unsigned int client_id, unsigned int session_id)
{
	ccl::vector<ccl::Pass>& passes = get_passes(session_id);
	passes.clear();
}

void cycles_session_add_pass(unsigned int client_id, unsigned int session_id, int pass_id)
{
	ccl::PassType passtype = (ccl::PassType)pass_id;
	ccl::vector<ccl::Pass>& passes = get_passes(session_id);
		// TODO: XXXX Passes rework
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


int cycles_session_reset(unsigned int client_id, unsigned int session_id, unsigned int width, unsigned int height, unsigned int samples, unsigned int full_x, unsigned int full_y, unsigned int full_width, unsigned int full_height )
{
	RenderCrashTranslatorHelper render_crash_helper(render_crash_translator);

	int rc = 0;
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		try {
			logger.logit(client_id, "Reset session ", session_id, ". width ", width, " height ", height, " samples ", samples);
			ccsess->buffer_params.full_x = full_x;
			ccsess->buffer_params.full_y = full_y;
			ccsess->buffer_params.full_width = full_width;
			ccsess->buffer_params.full_height = full_height;
			ccsess->buffer_params.width = width;
			ccsess->buffer_params.height = height;

			ccsess->params.samples = samples;

			ccl::vector<ccl::Pass>& passes = get_passes(session_id);

						// TODO: XXXX Passes rework
						/*
			session->scene->film->tag_passes_update(session->scene, passes);
			session->scene->film->display_pass = ccl::PassType::PASS_COMBINED;
			session->scene->film->tag_update(session->scene);

			ccsess->buffer_params.passes = passes;


			session->reset(ccsess->buffer_params, (int)samples);
						*/
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

void cycles_session_set_update_callback(unsigned int client_id, unsigned int session_id, void(*update)(unsigned int sid))
{
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
		logger.logit(client_id, "Set status update callback for session ", session_id);
	}
}

void cycles_session_set_cancel_callback(unsigned int client_id, unsigned int session_id, void(*cancel)(unsigned int sid))
{
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
		logger.logit(client_id, "Set status cancel callback for session ", session_id);
	}
}

void cycles_session_set_update_tile_callback(unsigned int client_id, unsigned int session_id, RENDER_TILE_CB update_tile_cb)
{
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
		logger.logit(client_id, "Set render tile update callback for session ", session_id);
	}
}

void cycles_session_set_write_tile_callback(unsigned int client_id, unsigned int session_id, RENDER_TILE_CB write_tile_cb)
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
		logger.logit(client_id, "Set render tile write callback for session ", session_id);
	}
#endif
}

void cycles_session_set_display_update_callback(unsigned int client_id, unsigned int session_id, DISPLAY_UPDATE_CB display_update_cb)
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
		logger.logit(client_id, "Set display update callback for session ", session_id);
	}
#endif
}

void cycles_session_cancel(unsigned int client_id, unsigned int session_id, const char *cancel_message)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit(client_id, "Cancel session ", session_id, " with message ", cancel_message);
		session->progress.set_cancel(std::string(cancel_message));
	}
}

void cycles_session_start(unsigned int client_id, unsigned int session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit(client_id, "Starting session ", session_id);
		session->start();
	}
}

void cycles_session_prepare_run(unsigned int client_id, unsigned int session_id)
{
		// TODO: XXXX revisit session running
		/*
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit(client_id, "Preparing run for session ", session_id);
		session->prepare_run(ccsess->buffer_params, ccsess->params.samples);
	}
		*/
}

void cycles_session_end_run(unsigned int client_id, unsigned int session_id)
{
		// TODO: XXXX revisit session running
		/*
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit(client_id, "Ending run for session ", session_id);
		session->end_run();
	}
		*/
}


int cycles_session_sample(unsigned int client_id, unsigned int session_id)
{
		// TODO: XXXX revisit rendering. check output driver
		/*
	RenderCrashTranslatorHelper render_crash_helper(render_crash_translator);

	try {
		int rc = -1;
		CCSession* ccsess = nullptr;
		ccl::Session* session = nullptr;
		if (session_find(session_id, &ccsess, &session)) {
			logger.logit(client_id, "Starting session ", session_id);
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
	return -1;
}

void cycles_session_wait(unsigned int client_id, unsigned int session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		logger.logit(client_id, "Waiting for session ", session_id);
		session->wait();
	}
}

void cycles_session_set_pause(unsigned int client_id, unsigned int session_id, bool pause)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		session->set_pause(pause);
	}
}

bool cycles_session_is_paused(unsigned int client_id, unsigned int session_id)
{
/*
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		return false; // session->is_paused();
	}*/

	return false;
}

void cycles_session_set_samples(unsigned int client_id, unsigned int session_id, int samples)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		session->set_samples(samples);
	}
}

void cycles_session_get_buffer_info(unsigned int client_id, unsigned int session_id, unsigned int* buffer_size, unsigned int* buffer_stride)
{
	*buffer_size = 0;
	*buffer_stride = 0;
}

float* cycles_session_get_buffer(unsigned int client_id, unsigned int session_id)
{
	return nullptr;
}

void cycles_session_copy_buffer(unsigned int client_id, unsigned int session_id, float* pixel_buffer)
{
}

void cycles_session_get_float_buffer(unsigned int client_id, unsigned int session_id, int passtype, float** pixels)
{
		// TODO: XXXX use output driver instead
		/*
	ccl::DeviceDrawParams draw_params = ccl::DeviceDrawParams();
	draw_params.bind_display_space_shader_cb = nullptr;
	draw_params.unbind_display_space_shader_cb = nullptr;

	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		if (ccl::Pass::contains(ccsess->buffer_params.passes, (ccl::PassType)passtype) && session->display_buffers[(ccl::PassType)passtype]) {
			*pixels = (float*)session->display_buffers[(ccl::PassType)passtype]->prepare_pixels(session->device, draw_params);
		}
	}
		*/
}

void cycles_progress_reset(unsigned int client_id, unsigned int session_id)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		session->progress.reset();
	}
}

int cycles_progress_get_sample(unsigned int client_id, unsigned int session_id)
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

void cycles_progress_get_time(unsigned int client_id, unsigned int session_id, double *total_time, double* sample_time)
{
	CCSession* ccsess = nullptr;
	ccl::Session* session = nullptr;
	if (session_find(session_id, &ccsess, &session)) {
		return session->progress.get_time(*total_time, *sample_time);
	}
}

void cycles_tilemanager_get_sample_info(unsigned int client_id, unsigned int session_id, unsigned int* samples, unsigned int* total_samples)
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
void cycles_progress_get_progress(unsigned int client_id, unsigned int session_id, float* progress)
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

bool cycles_progress_get_status(unsigned int client_id, unsigned int session_id, void* strholder)
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

bool cycles_progress_get_substatus(unsigned int client_id, unsigned int session_id, void* strholder)
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
