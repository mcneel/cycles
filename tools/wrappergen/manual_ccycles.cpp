
/** BEGIN MANUAL C-API **/

CCL_CAPI void* cycles_string_holder_new()
{
    return new StringHolder();
}

CCL_CAPI void cycles_string_holder_delete(void* strholder)
{
    StringHolder* holder = (StringHolder*)strholder;
    delete holder;
    holder = nullptr;
}

CCL_CAPI const char* cycles_string_holder_get(void* strholder)
{
    StringHolder* holder = (StringHolder*)strholder;
    if(holder!=nullptr) {
        return holder->thestring.c_str();
    }
    return "";
}

/**
 * Create a new shader graph for the given shader.
 */
CCL_CAPI void call_shader_creategraph(ccl::Shader* shader)
{
    std::unique_ptr<ccl::ShaderGraph> graph = std::make_unique<ccl::ShaderGraph>();
    shader->set_graph(std::move(graph));
}

/**
 * Create a new shadernode in the graph of 'shader'. The node will be of the
 * type specified by the string 'type'.
 *
 * When successful the pointer to the new node is returned, nullptr otherwise.
 */
CCL_CAPI ccl::ShaderNode* call_scene_createnode_shader(ccl::Scene* scene, ccl::Shader* shader, const char* type)
{
    OpenImageIO_v3_0::ustring nodename(type);
    ccl::ShaderGraph* graph = shader->graph.get();
    ccl::ShaderNode* node = nullptr;

    if(nodename == "background") {
        nodename = "background_shader";
    }

    const ccl::NodeType* nodetype = ccl::NodeType::find(nodename);

    if(!nodetype) {
        return nullptr;
    }
    if(nodetype->type != ccl::NodeType::SHADER) {
        return nullptr;
    }
    if(nodetype->create == nullptr) {
        return nullptr;
    }
    node = graph->create_node(nodetype);

    shader->tag_update(scene);

    return node;
}

CCL_CAPI ccl::ShaderGraph* cycles_shader_new_graph(ccl::Shader *shader, ccl::Scene* scene)
{
    std::unique_ptr<ccl::ShaderGraph> graph = std::make_unique<ccl::ShaderGraph>();
    ccl::ShaderGraph* graph_ptr = graph.get();
    shader->set_graph(std::move(graph));
    shader->tag_update(scene);
    return graph_ptr;
}

CCL_CAPI void cycles_shader_dump_graph(ccl::Shader *shader, const char *filename)
{
    shader->graph->dump_graph(filename);
}

CCL_CAPI int cycles_shader_node_count(ccl::Shader *shader)
{
    return shader->graph->nodes.size();
}

CCL_CAPI ccl::ShaderNode* cycles_shader_node_get(ccl::Shader *shader, int idx)
{
    int count = 0;
    return shader->graph->nodes[idx];
}

CCL_CAPI bool cycles_shadernode_get_name(ccl::ShaderNode *shn, void *strholder)
{
    if (shn && strholder) {
        StringHolder *holder = (StringHolder *)strholder;
        std::string name{shn->type->name.c_str()};

        holder->thestring = name;

        return true;
    }

    return false;
}

CCL_CAPI ccl::Shader* call_scene_createshader(ccl::Scene* scene)
{
    ccl::Shader *shader = scene->create_node<ccl::Shader>();
    std::unique_ptr<ccl::ShaderGraph> graph = std::make_unique<ccl::ShaderGraph>();
    shader->set_graph(std::move(graph));
    shader->tag_update(scene);
    return shader;
}

CCL_CAPI ccl::OutputNode* call_shader_get_outputnode(ccl::Shader* shader)
{
    ccl::OutputNode* out = shader->graph->output();
    return out;
}

/**
 * Create a new mesh in the given scene.
 */
CCL_CAPI ccl::Mesh* call_scene_createmesh(ccl::Scene* scene)
{
    ccl::Mesh* mesh = scene->create_node<ccl::Mesh>();
    return mesh;
}

/**
 * Create a new object in the given scene.
 */
CCL_CAPI ccl::Object* call_scene_createobject(ccl::Scene* scene)
{
    ccl::Object* object = scene->create_node<ccl::Object>();
    return object;
}

/**
 * Create a new light in the given scene.
 */
CCL_CAPI ccl::Light* call_scene_createlight(ccl::Scene* scene)
{
    ccl::Light* light = scene->create_node<ccl::Light>();
    return light;
}

/**
 * Create a new pass in the given scene.
 */
CCL_CAPI ccl::Pass* call_scene_createpass(ccl::Scene* scene)
{
    ccl::Pass* pass = scene->create_node<ccl::Pass>();
    return pass;
}

CCL_CAPI void cycles_path_init(const char* path, const char* user_path)
{
    ccl::path_init(std::string(path), std::string(user_path));
}

bool initialised{ false };
std::vector<ccl::DeviceInfo> devices;

CCL_CAPI void cycles_initialise(unsigned int mask)
{
    if(!initialised) {
        devices.clear();
        devices = ccl::Device::available_devices(mask);
        initialised = true;
    }
}

CCL_CAPI unsigned int cycles_number_devices()
{
    return static_cast<unsigned int>(devices.size());
}

CCL_CAPI ccl::DeviceInfo* cycles_get_device_info(unsigned int index)
{
    if(index >= devices.size()) {
        return nullptr;
    }
    return &devices[index];
}

CCL_CAPI CCSession* cycles_prepare_ccsession(ccl::SessionParams** session_params, ccl::SceneParams** scene_params, ccl::BufferParams** buffer_params)
{
    int csesid{ -1 };
    int hid{ 0 };

    CCSession* ccsession = CCSession::create(10, 10, 4);

    *session_params = &(ccsession->params);
    *scene_params = &(ccsession->scene_params);
    *buffer_params = &(ccsession->buffer_params);

    sessions.insert(ccsession);
    csesid = (unsigned int)(sessions.size() - 1);
    ccsession->id = csesid;

    return ccsession;
}

CCL_CAPI ccl::Session* cycles_create_session(CCSession* ccsession, ccl::SessionParams* sessionParams, ccl::SceneParams* sceneParams)
{
    ccsession->session = std::make_unique<ccl::Session>(*sessionParams, *sceneParams);
    prep_session(ccsession->session.get(), &ccsession->passes, ccsession);
    return ccsession->session.get();
}

CCL_CAPI void cycles_session_reset(ccl::Session* session, ccl::SessionParams* session_params, ccl::BufferParams* buffer_params)
{
    session->reset(*session_params, *buffer_params);
}

CCL_CAPI void cycles_session_destroy(CCSession* ccsess)
{
    sessions.erase(ccsess);
    delete ccsess;
}

CCL_CAPI void cycles_session_add_pass(CCSession* ccsess, ccl::Pass* pass)
{
    std::unique_ptr<CCyclesPassOutput> outputpass = std::make_unique<CCyclesPassOutput>();
    outputpass->set_pass_type(pass->get_type());

    ccsess->passes.push_back(std::move(outputpass));
}

CCL_CAPI ccl::SessionParams* cycles_session_get_session_params(CCSession* ccsession)
{
    return &ccsession->params;
}

CCL_CAPI ccl::SceneParams* cycles_session_get_scene_params(CCSession* ccsession)
{
    return &ccsession->scene_params;
}

CCL_CAPI ccl::BufferParams* cycles_session_get_buffer_params(CCSession* ccsession)
{
    return &ccsession->buffer_params;
}

CCL_CAPI ccl::Progress* cycles_get_progress(ccl::Session* session)
{
    return &session->progress;
}

CCL_CAPI ccl::Scene* cycles_session_get_scene(ccl::Session* session)
{
    return session->scene.get();
}

CCL_CAPI bool cycles_progress_get_status(ccl::Progress* progress, void* statusstrholder, void* substatusstrholder)
{
    if (progress != nullptr) {
        StringHolder* statusholder = (StringHolder*)statusstrholder;
        StringHolder* substatusholder = (StringHolder*)substatusstrholder;
        progress->get_status(statusholder->thestring, substatusholder->thestring);
        return true;
    }

    return false;
}

CCL_CAPI void cycles_session_params_set_deviceinfo(ccl::SessionParams* params, ccl::DeviceInfo* devinfo)
{
    params->device = *devinfo;
}

CCL_CAPI void cycles_set_rhino_perlin_noise_table(int* data, unsigned int count)
{
    ccycles_rhino_perlin_noise_table.resize(count);

    for (int i = 0; i < count; i++)
    {
        ccycles_rhino_perlin_noise_table[i] = (float)data[i];
    }
}

CCL_CAPI void cycles_set_rhino_impulse_noise_table(float* data, unsigned int count)
{
    ccycles_rhino_impulse_noise_table.resize(count);

    for (int i = 0; i < count; i++)
    {
        ccycles_rhino_impulse_noise_table[i] = (float)data[i];
    }
}

CCL_CAPI void cycles_set_rhino_vc_noise_table(float* data, unsigned int count)
{
    ccycles_rhino_vc_noise_table.resize(count);

    for (int i = 0; i < count; i++)
    {
        ccycles_rhino_vc_noise_table[i] = (float)data[i];
    }
}

CCL_CAPI void cycles_set_rhino_aaltonen_noise_table(const int* data, unsigned int count)
{
    ccycles_rhino_aaltonen_noise_table.resize(count);

    for (int i = 0; i < count; i++)
    {
        ccycles_rhino_aaltonen_noise_table[i] = (float)data[i];
    }
}

CCL_CAPI void cycles_apply_gamma_to_byte_buffer(unsigned char* rgba_buffer, size_t size_in_bytes, float gamma)
{
    if (gamma > 0.999f && gamma < 1.001f)
        return;

    ccl::uchar4* colbuf = (ccl::uchar4*)rgba_buffer;
    if (nullptr == colbuf)
        return;

    const int pixel_count = size_in_bytes / sizeof(ccl::uchar4);

    GammaLUT lut(gamma);

    #pragma omp parallel for
    for (int i = 0; i < pixel_count; i++)
    {
        colbuf[i].x = lut.Lookup(colbuf[i].x);
        colbuf[i].y = lut.Lookup(colbuf[i].y);
        colbuf[i].z = lut.Lookup(colbuf[i].z);
    }
}

CCL_CAPI void cycles_apply_gamma_to_float_buffer(float* rgba_buffer, size_t size_in_bytes, float gamma)
{
    ccl::float4* colbuf = (ccl::float4*)rgba_buffer;

    const int pixel_count = size_in_bytes / sizeof(ccl::float4);

    #pragma omp parallel for
    for (int i = 0; i < pixel_count; i++)
    {
        const auto red   = powf(colbuf[i].x, gamma);
        const auto green = powf(colbuf[i].y, gamma);
        const auto blue  = powf(colbuf[i].z, gamma);

        colbuf[i].x = red;
        colbuf[i].y = green;
        colbuf[i].z = blue;
    }
}

CCL_CAPI ccl::ShaderGraph* cycles_shader_get_graph(ccl::Shader* shader)
{
    return shader->graph.get();
}

CCL_CAPI bool cycles_shader_connect_nodes(ccl::Shader* shader,
                                          ccl::ShaderNode* from_node,
                                          const char* from,
                                          ccl::ShaderNode* to_node,
                                          const char* to)
{
    bool res = false;
    assert(shader);
    assert(from_node);
    assert(to_node);
    if (shader && from_node && to_node)
    {
        ccl::ShaderInput* to_input = to_node->input(to);
        ccl::ShaderOutput* from_output = from_node->output(from);
        // If to_input->link is not null we already had a link to this.
        res = to_input->link == nullptr;
        if(!res) {
            to_input->disconnect();
            res = true;
        }
        shader->graph->connect(from_node->output(from), to_input);

        if (!res) {
            fprintf(stderr, "input %s already connected, trying from (%s)\n", to, from);
        }
    }

    return res;
}

CCL_CAPI void cycles_shader_disconnect_node(ccl::Shader *shader,
                                            ccl::ShaderNode *from_node,
                                            const char *from)
{
    assert(shader);
    assert(from_node);
    if (shader && from_node)
        shader->graph->disconnect(from_node->input(from));
}

CCL_CAPI bool cycles_scene_try_lock(ccl::Scene* scene)
{
    return scene->mutex.try_lock();
}

CCL_CAPI void cycles_scene_lock(ccl::Scene* scene)
{
    scene->mutex.lock();
}

CCL_CAPI void cycles_scene_unlock(ccl::Scene* scene)
{
    scene->mutex.unlock();
}

CCL_CAPI void cycles_geometry_add_shader(ccl::Geometry* geometry, ccl::Shader* shader)
{
    ccl::array<ccl::Node *> used_shaders = geometry->get_used_shaders();
    used_shaders.push_back_slow(shader);
    geometry->set_used_shaders(used_shaders);
}

CCL_CAPI void cycles_geometry_set_shader(ccl::Geometry* geometry, ccl::Shader* shader)
{
    ccl::array<ccl::Node *> used_shaders;
    used_shaders.push_back_slow(shader);
    geometry->set_used_shaders(used_shaders);
}

CCL_CAPI ccl::ShaderNode* cycles_add_shader_node(ccl::Shader *shader,
                                                 const char *node_type_name,
                                                 const char *name)
{
    ccl::ShaderGraph* graph = shader->graph.get();
    const ccl::NodeType *node_type = ccl::NodeType::find(ustring(node_type_name));
    ccl::ShaderNode *node = graph->create_node(node_type);

    assert(node);

    if (node) {
        node->name = ustring(name);

        /*
        if(ustring(node_type_name) == ustring("tangent")) {
            ccl::TangentNode *tangent = dynamic_cast<ccl::TangentNode *>(node);
            tangent->set_direction_type(ccl::NodeTangentDirectionType::NODE_TANGENT_UVMAP);
            tangent->set_attribute(ustring("uvmap1"));
        }
        */
    }

    return node;
}

CCL_CAPI void cycles_geometry_tag_update(ccl::Geometry* geometry, ccl::Scene* scene)
{
    geometry->tag_update(scene, true);
    scene->light_manager->tag_update(scene, ccl::LightManager::UPDATE_ALL);
}

CCL_CAPI void cycles_object_tag_update(ccl::Object* ob, ccl::Scene* scene)
{
    ob->tag_update(scene);
    scene->light_manager->tag_update(scene, ccl::LightManager::UPDATE_ALL);
}

CCL_CAPI void cycles_shader_tag_update(ccl::Shader* shader, ccl::Scene* scene)
{
    shader->tag_update(scene);
    scene->light_manager->tag_update(scene, ccl::LightManager::UPDATE_ALL);
}

CCL_CAPI void cycles_camera_tag_update(ccl::Camera* cam, ccl::Scene* scene)
{
  cam->need_flags_update = true;
  cam->update(scene);
}

CCL_CAPI void cycles_session_retain_float_buffer(
        CCSession* ccsess,
        int passtype,
        int width, int height,
        float **pixels,
        int* pixel_size)
{
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

CCL_CAPI void cycles_session_release_float_buffer(
    CCSession* ccsess,
    int passtype
)
{
    if (ccsess) {
        for (auto &pass : ccsess->passes) {
            if (passtype == pass->get_pass_type()) {
                pass->unlock();
                break;
            }
        }
    }
}

CCL_CAPI void cycles_mesh_set_vertex_normals(
    ccl::Mesh* mesh,
    ccl::float3* vndata,
    size_t count
)
{
    ccl::Attribute *attr = mesh->attributes.add(ccl::ATTR_STD_VERTEX_NORMAL);
    ccl::float3 *fdata = attr->data_float3();
    for(int i = 0; i < count; i++) {
        fdata[i] = vndata[i];
    }
}

CCL_CAPI void cycles_mesh_set_vertex_uvs(
    ccl::Mesh* mesh,
    ccl::float2* uvdata,
    size_t count,
    const char* _uvmap
)
{
    ccl::ustring uvmap = _uvmap ? ccl::ustring(_uvmap) : ccl::ustring("uvmap1");
    ccl::Attribute *attr = mesh->attributes.add(ccl::ATTR_STD_UV, uvmap);
    //ccl::Attribute *attr = mesh->attributes.add(uvmap, ccl::TypeFloat2, ccl::ATTR_ELEMENT_CORNER);
    ccl::float2 *fdata = attr->data_float2();
    for(int i = 0; i < count; i++) {
        fdata[i] = uvdata[i];
    }
}

/** END MANUAL C-API **/

