
/** BEGIN MANUAL C++ **/

class StringHolder
{
public:
    std::string thestring;
};

/* Cycles does not have a dimensions enum, but for autogen it is convenient.
 * Dimensions are used in several noise textures.
 */
namespace ccl {
enum Dimensions {
    DIM1D = 1,
    DIM2D = 2,
    DIM3D = 3,
    DIM4D = 4,
};
}

class CCSession;

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
    std::unique_ptr<ccl::Session> session;

    int width{ 0 };
    int height{ 0 };

    ccl::BufferParams buffer_params;

    std::vector<std::unique_ptr<CCyclesPassOutput>> passes;

    /* Create a new CCSession, initialise all necessary memory. */
    static CCSession* create(int width, int height, unsigned int buffer_stride);

    /* Returns true if size was changed. Will reset the has_changed flag. */
    bool size_has_changed();

    ~CCSession() {
        session.reset();
    }

private:
    bool _size_has_changed;

protected:
    /* Protected constructor, use CCSession::create to create a new CCSession. */
    CCSession()
    {  }
};

CCSession* CCSession::create(int width, int height, unsigned int buffer_stride) {
    CCSession* se = new CCSession();
    se->width = width;
    se->height = height;
    se->_size_has_changed = false;

    return se;
}

CCyclesPassOutput::CCyclesPassOutput()
: m_lock(), m_pass_type(ccl::PassType::PASS_COMBINED), m_width(0), m_height(0), m_pixels()
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
: log_(log), ccsession_(ccsession), full_passes(full_passes)
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
    const int channels = 4;
    ccl::vector<float> pixels(width * height * 1);
    std::string passname{pass_type_as_string(ccl::PassType::PASS_COMBINED)};

    if (tile.get_sample() < 2 && tile.get_pass_pixels(passname.c_str(), channels, pixels.data())) {
        //// !!!!!!!!!!!!! Remember to change path to something useful on dev machine
        //fs::path save_path = "C:/Users/jesterKing/check_cycles_output.png";
        fs::path save_path = "C:/Users/Testing/check_cycles_output.exr";
        //fs::path save_path = "/Users/jesterking/check_cycles_output.exr";
        //// !!!!!!!!!!!!! Remember to change path to something useful on dev machine
        std::unique_ptr<OIIO::ImageOutput> image_output(OIIO::ImageOutput::create("exr"));
        OIIO::ImageSpec spec(width, height, 4, OIIO::TypeDesc::FLOAT);
        if(nullptr != image_output &&image_output->open(save_path.string(), spec))
        {
            OIIO::ImageBuf image_buffer(spec,
                                  pixels.data(),
                                  OIIO::AutoStride,
                                  width * channels * sizeof(float),
                                  OIIO::AutoStride);
            /* Write to disk and close */
            image_buffer.set_write_format(OIIO::TypeDesc::FLOAT);
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

            ccl::PassInfo pass_info = ccl::Pass::get_info(pass_type);

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

            if(full_pass->get_pass_type() == ccl::PassType::PASS_DEPTH && tile.get_sample() > 1)
            {
                continue;
            }

            full_pass->lock();

            ccl::PassInfo pass_info = ccl::Pass::get_info(full_pass->get_pass_type());

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
            if (!upscale && pass->get_pass_type() == ccl::PassType::PASS_DEPTH && tile.get_sample() > 1) {
                continue;
            }

            pass->lock();

            ccl::PassInfo pass_info = ccl::Pass::get_info(pass->get_pass_type());

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

std::unordered_set<CCSession*> sessions;

ccl::vector<float> ccycles_rhino_perlin_noise_table;
ccl::vector<float> ccycles_rhino_impulse_noise_table;
ccl::vector<float> ccycles_rhino_vc_noise_table;
ccl::vector<float> ccycles_rhino_aaltonen_noise_table;

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
    ccl::Scene* scene = session->scene.get();

    scene->shader_manager->set_rhino_perlin_noise_table(ccycles_rhino_perlin_noise_table);
    scene->shader_manager->set_rhino_impulse_noise_table(ccycles_rhino_impulse_noise_table);
    scene->shader_manager->set_rhino_vc_noise_table(ccycles_rhino_vc_noise_table);
    scene->shader_manager->set_rhino_aaltonen_noise_table(ccycles_rhino_aaltonen_noise_table);

    /*ccl::Camera *cam = scene->camera;
    cam->set_full_height(512);
    cam->set_full_width(512);
    cam->compute_auto_viewplane();
    cam->need_flags_update = true;
    cam->update(session->scene);*/

    session->set_output_driver(std::make_unique<CCyclesOutputDriver>(passes, log_print, ccsession));

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
    // TODO AzimuthAltitudeTransformNode derp;

    /*
    {
        scene->background->set_transparent_glass(true);
        ccl::Shader *bgsh = scene->default_background;
        std::unique_ptr<ccl::ShaderGraph> graph = std::make_unique<ccl::ShaderGraph>();
        ccl::OutputNode *out = graph->output();
        ustring nodename("background_shader");
        ccl::ShaderNode *shn = nullptr;
        const ccl::NodeType *ntype = ccl::NodeType::find(nodename);
        shn = graph->create_node(ntype);
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
        std::unique_ptr<ccl::ShaderGraph> graph = std::make_unique<ccl::ShaderGraph>();
        auto out = graph->output();
        ustring nodename("diffuse_bsdf");
        ccl::ShaderNode* shader_node = nullptr;
        const ccl::NodeType *ntype = ccl::NodeType::find(nodename);
        shader_node = graph->create_node(ntype);
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
    }
    */
}

class GammaLUT
{
public:
    GammaLUT(float gamma)
    {
        for (unsigned int i = 0; i <= 255; i++)
        {
            lut[i] = (unsigned char)(255.f * powf(i / 255.f, gamma));
        }
    }

    unsigned char Lookup(unsigned char in)
    {
        assert(in <= 255);
        return lut[in];
    }
private:
    unsigned char lut[256];
};

/** END MANUAL C++ **/

