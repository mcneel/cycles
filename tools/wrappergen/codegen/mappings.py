
def _extra_mapping_fun(nodename, member, inp : str):
  if inp == 'string':
    print(nodename, member, inp)
  return inp


def _contains_type(nodename, member, inp : str):
  return inp


# tuple has:
# ( original string, replacement, replacement if return, function )
capi_mappings_through_context = {
  '*': [
    (r'(?<!\w)const string&', 'const char*', 'void', _extra_mapping_fun,),
    (r'(?<!\w)ustring&', 'const char*', 'void', None,),
    (r'(?<!\w)string&', 'const char*', 'void', _extra_mapping_fun,),
    (r'(?<!\w)ustring', 'const char*', 'void', None,),
    (r'(?<!\w)string', 'const char*', 'void', _extra_mapping_fun,),
    (r'BVHLayout', 'ccl::BVHLayout', 'ccl::BVHLayout', _contains_type,),
  ],
  'NodeType': [(r'(?<!\w)Type', 'ccl::NodeType::Type', 'ccl::NodeType::Type', None,)],
  'Geometry': [(r'(?<!\w)Type', 'ccl::Geometry::Type', 'ccl::Geometry::Type', None,)],
  'DeviceInfo': [(r'(?<!\w)DenoiserTypeMask', 'ccl::DenoiserType', 'ccl::DenoiserType', None,)],
  'Node': [(r'(?<!\w)Node', 'ccl::Node', 'ccl::Node', None,)],

  'BoundBox': [(r'(?<!\w)BoundBox(?<!2D)', 'BoundBoxC', 'BoundBoxC', None,)],
}

# tuple has:
# ( original string, capi replacement, csapi replacement, replacement pinv return, replacement c# return, function )
csapi_mappings_through_context = {
  '*': [
    ('ustring', 'const char*', 'void', None,),
    ('string', 'const char*', 'void', _extra_mapping_fun,),
  ],
}

clean_rettype = [
  "inline",
  "static",
  "const",
  "virtual",
]

method_name_replacements = [
  ("==", "_eq"),
  ("!=", "_neq"),
  (" delete", "_del"),
  ("=", "_assign"),
  ("&", "_ref"),
  ("*", "_mult"),
  ("/", "_div"),
  ("+", "_add"),
  ("-", "_sub"),
  ("[]", "_index"),
  ("(),", "_call"),
  ("::", "_"),
]

_float = {
    "cs_socket": "FloatSocket",
    "base_type": "float",
    "ccl_type": "float",
    "cs_socket_type": "float",
    "cs_set": "SetFloat(string name, float data)",
    "cs_get": "float GetFloat(string name)",
}

_typeX = {
    "cs_socket": "XSocket",
    "base_type": "Y",
    "ccl_type": "Z",
    "cs_socket_type": "W",
    "cs_set": "SetX(string name, W data)",
    "cs_get": "W GetX(string name)",
}

_floatArrayX = {
    "cs_socket": "XSocket",
    "base_type": "float*",
    "ccl_type": "float3",
    "cs_socket_type": "List<float>",
    "cs_set": "SetX(string name, W data)",
    "cs_get": "W GetX(string name)",
    "marshal": """int datumSize = Marshal.SizeOf<X>();
        int totalSize = datumSize * count;
        byte[] bytedata = new byte[totalSize];
        Marshal.Copy(data, bytedata, 0, totalSize);
        ReadOnlySpan<byte> byteSpan = new ReadOnlySpan<byte>(bytedata);
        ReadOnlySpan<X> xSpan = MemoryMarshal.Cast<byte, X>(byteSpan);
        var list = new List<X>(xSpan.ToArray());
        return list;""",
    #"marshal_reinterpret": "W[] dn = Array.ConvertAll(orig, e => unchecked((W)e));\n        return new List<W>(dn);",
    "marshal_reinterpret": "",
    "pinv_type": "",
    "stride": 3
}


def _adjust_typeX(name, base_type, ccl_type, cs_type, cs_get_return):
    d = _typeX.copy()
    d['cs_socket'] = d['cs_socket'].replace('X', name)
    d['base_type'] = base_type
    d['ccl_type'] = ccl_type
    d['cs_socket_type'] = cs_type
    d['cs_set'] = d['cs_set'].replace('X', name).replace('W', cs_type)
    d['cs_get_return'] = f'return {cs_get_return};'
    d['cs_get'] = d['cs_get'].replace('X', name).replace('W', cs_type)

    return d


def _adjust_typeArrayX(name, base_type, ccl_type, cs_type, marshal_type, marshal_reint_type, stride):
    d = _floatArrayX.copy()
    d['cs_socket'] = d['cs_socket'].replace('X', name)
    d['base_type'] = base_type
    d['ccl_type'] = ccl_type
    d['cs_socket_type'] = cs_type
    d['stride'] = stride
    d['cs_set'] = d['cs_set'].replace('X', name).replace('W', cs_type)
    d['cs_get'] = d['cs_get'].replace('X', name).replace('W', cs_type)
    d['marshal'] = d['marshal'].replace('X', marshal_type)
    d['pinv_type'] = marshal_type

    return d


socketdatatype_mapping = [
    #  SOCKETDATATYPE                     NAME      C TYPE   CCL TYPE  C# TYPE  CS RET
    [('FLOAT',),            _adjust_typeX("Float", "float", "float", "float", "float.MaxValue")],
    [('VECTOR',),           _adjust_typeX("Vector", "ccl::float3", "ccl::float3", "float3", "new float3(float.MaxValue, float.MaxValue, float.MaxValue)")],
    [('NORMAL',),           _adjust_typeX("Normal", "ccl::float3", "ccl::float3", "float3", "new float3(float.MaxValue, float.MaxValue, float.MaxValue)")],
    [('POINT',),            _adjust_typeX("Point", "ccl::float3", "ccl::float3", "float3", "new float3(float.MaxValue, float.MaxValue, float.MaxValue)")],
    [('POINT2',),           _adjust_typeX("Point2", "ccl::float2", "ccl::float2", "float2", "new float2(float.MaxValue, float.MaxValue)")],
    #  SOCKETDATATYPE                     NAME      C TYPE   CCL TYPE  C# TYPE  CS RET
    [('COLOR',),            _adjust_typeX("Color", "ccl::float3", "ccl::float3", "float3", "new float3(0, 0, 0)")],
    [('CLOSURE',),          _adjust_typeX("Closure", "void*", "void*", "IntPtr", "null")],
    [('BOOLEAN',),          _adjust_typeX("Bool", "bool", "bool", "bool", "false")],
    [('INT',),              _adjust_typeX("Int", "int32_t", "int32_t", "int", "int.MaxValue")],
    [('INT64',),            _adjust_typeX("Int64", "int64_t", "int64_t", "long", "long.MaxValue")],
    #  SOCKETDATATYPE                     NAME      C TYPE   CCL TYPE  C# TYPE  CS RET
    [('UINT',),             _adjust_typeX("Uint", "uint32_t", "uint32_t", "uint", "uint.MaxValue")],
    [('UINT64',),           _adjust_typeX("Uint64", "uint64_t", "uint64_t", "ulong", "ulong.MaxValue")],
    [('STRING',),           _adjust_typeX("String", "std::string", "OpenImageIO_v3_0::ustring", "string", "")],
    [('TRANSFORM',),        _adjust_typeX("Transform", "ccl::Transform", "ccl::Transform", "Transform", "Transform.Identity()")],
    [('NODE',),             _adjust_typeX("Node", "void*", "ccl::Node*", "IntPtr", "IntPtr.Zero")],
    [('ENUM',),             _adjust_typeX("Enum", "uint", "uint", "object", "uint.MinValue")],
    #  SOCKETDATATYPE                           NAME         C TYPE     CCL TYPE  C# TYPE  MARSHAL  MARSHAL CAST  STRIDE
    [('FLOAT_ARRAY',),      _adjust_typeArrayX("FloatArray", "float*", "float", "List<float>", 'float', '', 1)],
    [('INT_ARRAY',),        _adjust_typeArrayX("IntArray", "int*", "int", "List<int>", 'int', '', 1)],
    [('BOOLEAN_ARRAY',),    _adjust_typeArrayX("BooleanArray", "bool*", "bool", "List<bool>", 'bool', '', 1)],
    [('COLOR_ARRAY',),      _adjust_typeArrayX("ColorArray", "ccl::float3*", "ccl::float3", "List<float3>", 'float3', '', 3)],
    #  SOCKETDATATYPE                           NAME         C TYPE     CCL TYPE  C# TYPE  MARSHAL  MARSHAL CAST  STRIDE
    [('VECTOR_ARRAY',),     _adjust_typeArrayX("VectorArray", "ccl::float3*", "ccl::float3", "List<float3>", 'float3', '', 3)],
    [('POINT_ARRAY',),      _adjust_typeArrayX("PointArray", "ccl::float3*", "ccl::float3*", "List<float3>", 'float3', '', 3)],
    [('POINT2_ARRAY',),     _adjust_typeArrayX("Point2Array", "ccl::float2*", "ccl::float2", "List<float2>", 'float2', '', 2)],
    [('TRANSFORM_ARRAY',),  _adjust_typeArrayX("TransformArray", "ccl::Transform*", "ccl::Transform", "List<Transform>", 'Transform', '', 9)],
]


capi_member_skip_on_rettype = [
    "ProjectionTransform",
    "DeviceInfo",
    "ImageHandle",
    "ImageMetaData",
    "ImageParams",
    "NamedSizeStats",
    "Triangle",
    "Mesh::Triangle",

    "Progress",

    "unique_ptr_vector",
    "TextureMapping",

    "half",

    "packed_float3",
    "packed_int3",
    "packed_uint3",
]


# dictionary: member name (partial or full) and tuple
# tuple: (exact match : bool, signature : None|str, arguments : None|str, exceptin: None|list[str], intype: None|list[str]} )
capi_member_to_skip = {
  "_manager" : (False, None, None, None, None, ),  # Internal to scene
  "_mix_weight": (False, None, None, None, None, ),  # internal to shader evaluation
  "add": (True, None, '(const char* name, CreateFunc create, Type type=NONE, const NodeType* base=nullptr)', None, None, ),
  "add_image": (True, None, ['(unique_ptr<ImageLoader>& & loader, const ImageParams& params, const bool builtin=true)', '(vector<unique_ptr<ImageLoader>>& & loaders, const ImageParams& params)', '(const string& filename, const ImageParams& params, const array<int>& tiles)'], None, None, ),
  "add_skip_time": (True, None, None, None, None, ),
  "attribute": (True, None, None, ('UVMapNode', 'NormalMapNode', 'AttributeNode', ), None, ),
  "attributes" : (False, None, None, None, None, ),
  "available_devices" : (True, None, None, None, None, ),  # TODO: reinstate with vector<DeviceInfo>
  "available_types" : (True, None, None, None, None, ),  # TODO: reinstate with vector<DeviceType>
  "backgrounds": (True, None, None, None, None, ),  # TODO:manual
  "border" : (True, None, None, None, None, ),  # Camera - border subelements have own accessors
  "bounds" : (True, None, None, None, None, ),
  "build_bvh" : (True, None, None, None, None, ),
  "bvh" : (True, None, None, None, None, ),
  "cameras": (True, None, None, None, None, ),  # TODO:manual
  "check_peer_access" : (False, None, None, None, None, ),
  "clipping_planes": (True, None, None, None, None, ),  # TODO:manual
  "clone": (True, None, None, None, None, ), # Internal
  "compile" : (False, None, None, None, None, ),
  "compute_bvh" : (True, None, None, None, None, ),  # Geometry::compute_bvh, Cycles internal
  "const_copy_to" : (True, None, None, None, None, ),
  "constant_fold" : (False, None, None, None, None, ),
  "constant_fold" : (True, None, None, None, None, ),
  "contains" : (True, None, None, None, None, ),
  "create" : (False, None, None, None, None, ),  # manual code, creating nodes etc.
  "create_inputs_outputs" : (True, None, None, None, None, ),
  "create_node" : (False, None, None, None, None, ),  # manual code, creating different things, mostly into scene
  "decal_setup" : (True, None, None, None, None, ),
  "denoise_device" : (False, None, None, None, None, ),  # No denoising in Cycles used
  "denoisers" : (False, None, None, None, None, ),
  "description" : (True, "int", None, None, None, ),
  "device" : (True, None, None, None, None, ),  # TODO manual code perhaps. harvester says int where it should be ccl::unique_ptr<ccl::Device>
  "device_free" : (False, None, None, None, None, ),
  "displacement_hash" : (True, None, None, None, None, ),  # Internal
  "dscene": (True, None, None, None, None, ),  # Internal to scene
  "equals": (False, None, None, None, None, ),
  "error_msg" : (True, "int", None, None, None, ),
  "expand": (True, None, None, None, None, ),
  "films": (True, None, None, None, None, ),  # TODO:manual
  "find" : (True, None, None, None, None, ),
  "foreach_device" : (True, None, None, None, None, ),
  "free_memory" : (True, None, None, None, None, ),
  "full_buffer_written_cb" : (True, None, None, None, None, ),  # TODO manual code. Function callback
  "geometry": (True, None, None, ('Object', ), None, ),  # TODO:manual
  "get_adaptive_sampling" : (True, None, None, None, None, ),  # Integrator - internal
  "get_attribute_id": (True, None, None, None, None, ),
  "get_bool_array" : (True, None, None, None, None, ),  # TODO manual
  "get_bvh_layout_mask" : (True, None, None, None, None, ),
  "get_closure_type": (True, None, None, None, None, ),
  "get_cpu_kernel_thread_globals" : (True, None, None, None, None, ),
  "get_cpu_kernels" : (True, None, None, None, None, ),
  "get_cpu_osl_memory" : (False, None, None, None, None, ),
  "get_denoise_params" : (True, None, None, None, None, ),  # Integrator - internal
  "get_feature": (True, None, None, ('VoronoiTextureNode', ), None, ),
  "get_float2_array" : (True, None, None, None, None, ),  # TODO manual
  "get_float3_array" : (True, None, None, None, None, ),  # TODO manual
  "get_float_array" : (True, None, None, None, None, ),  # TODO manual
  "get_group" : (True, None, None, None, None, ),  # only in header, not implemented in Cycles
  "get_guiding_params" : (True, None, None, None, None, ),  # Integrator - internal
  "get_info" : (True, None, None, None, None, ),
  "get_int_array" : (True, None, None, None, None, ),  # TODO manual
  "get_multi_device" : (True, None, None, None, None, ),  # TODO: reinstate with vector<DeviceInfo>
  "get_native_buffer" : (False, None, None, None, None, ),
  "get_node_array" : (True, None, None, None, None, ),  # TODO: manual implementation if access is needed
  "get_node_type" : (True, None, None, None, ('Volume',), ),
  "get_offset" : (True, None, ['(const unique_ptr_vector<Pass>& passes, const Pass* pass)',], None, None, ),
  "get_point" : (True, None, None, None, None, ),  # PointCloud::get_point
  "get_progress" : (True, None, None, None, None, ),  # only in header, not implemented in Cycles
  "get_shader" : (True, None, '(const Scene* scene)', None, None, ),
  "get_status" : (True, None, None, None, None, ),  # Progress, done manually
  "get_string_array" : (True, None, None, None, None, ),  # TODO: manual implementation if access is needed
  "get_svm_slots" : (True, None, None, None, None, ),
  "get_transform_array" : (True, None, None, None, None, ),  # TODO manual
  "get_used_shaders": (True, None, None, None, None, ),  # Geometry::get_used_shaders, Cycles internal
  "get_uv_tiles" : (True, None, None, None, None, ),  # Geometry::get_uv_tiles, Cycles internal"
  "gpu_queue_create" : (True, None, None, None, None, ),
  "graph" : (True, None, None, None, None, ),  # TODO manual, harvester gives int where needs to be ccl::Shader*
  "has_attribute_dependency": (True, None, None, None, None, ),
  "has_bssrdf_bump": (True, None, None, None, None, ),
  "has_bump": (True, None, None, None, None, ),
  "has_spatial_varying": (True, None, None, None, None, ),
  "has_surface_bssrdf": (True, None, None, None, None, ),
  "has_surface_emission": (True, None, None, None, None, ),
  "has_surface_transparent": (True, None, None, None, None, ),
  "has_volume_support": (True, None, None, None, None, ),
  "hash": (True, None, None, None, None, ),
  "id" : (True, "int", None, None, None, ),
  "image_memory" : (False, None, None, None, None, ),
  "input" : (True, None, None, None, None, ),
  "inputs" : (True, None, None, None, None, ),  # NodeType::inputs, Cycles internal
  "integrators": (True, None, None, None, None, ),  # TODO:manual
  "is_resident" : (True, None, None, None, None, ),
  "is_shared" : (False, None, None, None, None, ),
  "kernel_camera" : (False, None, None, None, None, ),
  "kernel_camera_motion" : (True, None, None, None, None, ),
  "lightgroups" : (True, None, None, None, None, ),  # Needs manual code
  "links" : (True, None, None, None, None, ),  # TODO needs manual, int from harvester where templated vector required
  "load_kernels" : (False, None, None, None, None, ),
  "load_osl_kernels" : (True, None, None, None, None, ),
  "lookup_tables" : (True, None, None, None, None, ),  # Internal to scene
  "multi_devices" : (True, None, None, None, None, ),  # TODO: reinstate with vector<DeviceInfo>
  "mutex" : (True, None, None, None, None, ),  # Internal to scene
  "name": (True, None, None, ('Pass', ), None, ),  # TODO: Needs manual attention, should be string but harvester thinks it is int
  "need_attribute" : (True, None, None, None, None, ),  # Geometry::need_attribute, Cycles internal
  "need_attribute": (True, None, None, None, None, ),
  "need_build_bvh" : (True, None, None, None, None, ),  # Geometry::need_build_bvh, Cycles internal
  "need_global_attribute": (True, None, None, None, None, ),
  "need_motion" : (True, None, None, None, None, ),  # Scene::need_motion, Cycles internal
  "objects": (True, None, None, None, None, ),  # TODO:manual
  "operator=": (True, None, None, None, None, ),  # TODO:manual
  "operator_assign": (True, None, None, None, None, ),  # TODO:manual
  "osl_bump_ref" : (True, None, None, None, None, ),  # No OSL support
  "osl_displacement_ref" : (True, None, None, None, None, ),  # No OSL support
  "osl_surface_bump_ref" : (True, None, None, None, None, ),  # No OSL support
  "osl_surface_ref" : (True, None, None, None, None, ),  # No OSL support
  "osl_volume_ref" : (True, None, None, None, None, ),  # No OSL support
  "output" : (True, None, None, None, None, ),
  "outputs" : (True, None, None, None, None, ),  # NodeType::outputs, Cycles outternal
  "pad" : (True, None, None, None, None, ),  # padding variable.
  "params": (True, None, None, None, None, ),  # TODO:manual
  "passes": (True, None, None, None, None, ),  # TODO:manual
  "procedurals": (True, None, None, None, None, ),  # TODO:manual
  "process_full_buffer_from_disk": (True, None, None, None, None, ),  # Unclear if is needed
  "profiler" : (False, None, None, None, None, ),
  "progress" : (True, None, None, None, None, ),  # TODO manual
  "register_input" : (True, None, None, None, None, ),  # NodeType::register_input, Cycles internal
  "register_output" : (True, None, None, None, None, ),  # NodeType::register_output, Cycles internal
  "release_bvh" : (True, None, None, None, None, ),
  "remove_input" : (True, None, None, None, None, ),
  "reset" : (True, None, None, None, ('Session',), ),
  "rhino_aaltonen_noise_table" : (False, None, None, None, None, ),  # TODO needs manual attention
  "rhino_dots_dot_data_table" : (False, None, None, None, None, ),  # TODO needs manual attention
  "rhino_dots_tree_data_table" : (False, None, None, None, None, ),  # TODO needs manual attention
  "rhino_impulse_noise_table" : (False, None, None, None, None, ),  # TODO needs manual attention
  "rhino_perlin_noise_table" : (False, None, None, None, None, ),  # TODO needs manual attention
  "rhino_vc_noise_table" : (False, None, None, None, None, ),  # TODO needs manual attention
  "scene" : (True, None, None, None, None, ),  # TODO manual code. harvester says int where it should be ccl::unique_ptr<ccl::Scene>
  "set" : (True, None, None, None, None, ),  # Node set, Internal?
  "set_cancel_callback" : (True, None, None, None, None, ),  # Progress, done manually
  "set_display_driver" : (True, None, None, None, None, ),  # TODO manual code, reuse Rhino 8 method
  "set_end_time" : (True, None, None, None, None, ),  # Progress, done manually
  "set_graph" : (True, None, None, None, None, ),  # Manual needed to cope with move syntax
  "set_output_driver" : (True, None, None, None, None, ),  # TODO manual code, reuse Rhino 8 method
  "set_status": (False, None, None, None, None, ),
  "set_used_shaders": (True, None, None, None, None, ),  # Geometry::set_used_shaders, Cycles internal
  "shaders": (True, None, None, None, None, ),  # TODO:manual
  "simplify_settings": (True, None, None, None, None, ),
  "stats" : (True, None, None, None, None, ),  # TODO manual
  "string_from_type": (True, None, None, None, None, ),
  "subd": (False, None, None, None, None, ),  # For now disable subd related API .
  "temp_dir" : (True, None, None, None, None, ),  # TODO manual, harvester gives int where needs to be string
  "type_from_string": (True, None, None, None, None, ),
  "types" : (True, None, None, None, None, ),  # TODO: reinstate unordered_map<ustring,NodeType>& NodeType::types()
  "update" : (False, None, None, None, None, ),
  "viewplane" : (True, None, None, None, None, ),  # Camera - viewplane subelements have own accessors
  "viewplane_bounds_get" : (True, None, None, None, None, ),  # Camera - bounds subelements have own accessors
  "viewport_camera_border": (True, None, None, None, None, ),  # Camera viewport camera border subelements have own accessors
}

