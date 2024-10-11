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

#ifndef __CYCLES__H__
#define __CYCLES__H__

#ifdef WIN32
#ifdef CCL_CAPI_DLL
#define CCL_CAPI __declspec (dllexport)
#else
#define CCL_CAPI __declspec (dllimport)
#endif
#ifndef CDECL
#define CDECL __cdecl
#endif
#ifndef UTFCHAR
#define UTFCHAR wchar_t
#endif
#else
#define CCL_CAPI
#ifndef CDECL
#define CDECL
#endif
#ifndef UTFCHAR
#define UTFCHAR char
#endif
#endif

/*

// conversion matrix for rhino -> cycles view.
ccl::Transform camConvertMat = ccl::make_transform(
1.0f, 0.0f, 0.0f, 0.0f,
0.0f, -1.0f, 0.0f, 0.0f,
0.0f, 0.0f, -1.0f, 1.0f

*/

#ifdef __cplusplus
extern "C" {
#endif

/**
 * \defgroup ccycles CCycles
 * CCycles is the low-level C-API for Cycles. Using this API one can
 * set up the Cycles render engine, push geometry and shaders to it
 * and govern the render process.
 */
/**
 * \defgroup ccycles_scene Scene API
 * The CCycles scene API provides functions to set up a scene for Cycles.
 * A scene will hold object, mesh, light and shader information for the
 * render engine to use during the render process.
 * \ingroup ccycles
 */
/**
 * \defgroup ccycles_shader Shader API
 * The CCycles shader API provides functions to create Cycles
 * shaders giving access to all available shader nodes.
 * \ingroup ccycles
 */
/**
 * \defgroup ccycles_mesh Mesh API
 * The CCycles mesh API provides functions to create Cycles
 * meshes.
 * \ingroup ccycles
 */
/**
 * \defgroup ccycles_object Object API
 * The CCycles object API provides functions to create Cycles
 * objects.
 * \ingroup ccycles
 */
/**
 * \defgroup ccycles_session Session API
 * The CCycles session API provides functions to create Cycles
 * sessions, administer callbacks and manage render processes.
 * \ingroup ccycles
 */

/***********************************/

class StringHolder
{
public:
	std::string thestring;
};


/**
 * Logger function signature. Used to register a logging
 * function with CCycles using
 * \ingroup ccycles
 */
typedef void(CDECL *LOGGER_FUNC_CB)(const char* msg);

/**
 * Initialise Cycles by querying available devices.
 * \ingroup ccycles
 */
CCL_CAPI void CDECL cycles_initialise(unsigned int mask = ccl::DEVICE_MASK_ALL);

/**
 * Initialise paths for Cycles to look in (pre-compiled kernels, cached kernels, kernel code)
 * \ingroup ccycles
 */
CCL_CAPI void CDECL cycles_path_init(const char* path, const char* user_path);

/**
 * Set an environment variable for Cycles
 * \ingroup ccycles
 */
CCL_CAPI void CDECL cycles_putenv(const char* var, const char* val);

/**
 * Set the CPU kernel type to use. 1 is split, 0 is regular.
 * \ingroup ccycles
 */
CCL_CAPI void CDECL cycles_debug_set_cpu_kernel(unsigned int state);

/**
 * Pass 1 to allow QBVH usage in CPU kernel.
 * \ingroup ccycles
 */
CCL_CAPI void CDECL cycles_debug_set_cpu_allow_qbvh(unsigned int state);

/**
 * Set the CUDA kernel type to use. 1 is split, 0 is regular.
 * \ingroup ccycles
 */
CCL_CAPI void CDECL cycles_debug_set_cuda_kernel(unsigned int state);

/**
 * Clean up everything, we're done.
 * \ingroup ccycles
 * \todo Add session specific cleanup, so we don't accidently delete sessions that are in progress.
 */
CCL_CAPI void CDECL cycles_shutdown();

/**
 * Add a logger function. This will be called only in debug builds.
 * \ingroup ccycles
 */
CCL_CAPI void CDECL cycles_set_logger(LOGGER_FUNC_CB logger_func_);

/**
 * Set to true if logger output should be sent to std::cout as well.
 *
 * Note that this is global to the logger.
 */
CCL_CAPI void CDECL cycles_log_to_stdout(int tostdout);

/**
 * Create a new client.
 *
 * This is mainly used for determining what logger functions belong to which client and session.
 *
 * Note that a client needs to be released with cycles_release_client when no longer necessary.
 */
CCL_CAPI unsigned int CDECL cycles_new_client();

/**
 * Release a client from usage.
 */
CCL_CAPI void CDECL cycles_release_client();

/* Create scene parameters, to be used when creating a new scene. */
CCL_CAPI unsigned int CDECL cycles_scene_params_create(unsigned int shadingsystem, unsigned int bvh_type, unsigned int use_bvh_spatial_split, int bvh_layout, unsigned int persistent_data);

/* Set scene parameters*/

/** Set scene parameter: BVH type. */
CCL_CAPI void CDECL cycles_scene_params_set_bvh_type(unsigned int scene_params_id, unsigned int type);
/** Set scene parameter: use BVH spatial split. */
CCL_CAPI void CDECL cycles_scene_params_set_bvh_spatial_split(unsigned int scene_params_id, unsigned int use);
/** Set scene parameter: use qBVH. */
CCL_CAPI void CDECL cycles_scene_params_set_qbvh(unsigned int scene_params_id, unsigned int use);
/** Set scene parameter: Shading system (SVM / OSL).
 * Note that currently SVM is only supported for RhinoCycles. No effort yet has been taken to enable OSL.
 */
CCL_CAPI void CDECL cycles_scene_params_set_shadingsystem(unsigned int scene_params_id, unsigned int system);
/** Set scene parameter: use persistent data. */
CCL_CAPI void CDECL cycles_scene_params_set_persistent_data(unsigned int scene_params_id, unsigned int use);

/**
 * Create a new mesh in session_id, using shader_id
 * \ingroup ccycles_scene
 */
CCL_CAPI ccl::Geometry* CDECL cycles_scene_add_mesh(ccl::Session* session_id, ccl::Shader *shader_id);
/**
 * Create a new mesh for object_id in session_id, using shader_id
 * \ingroup ccycles_scene
 */
//CCL_CAPI unsigned int CDECL cycles_scene_add_mesh_object(ccl::Session* session_id, unsigned int object_id, unsigned int shader_id);
/**
 * Create a new object for session_id
 * \ingroup ccycles_scene
 */
CCL_CAPI ccl::Object* CDECL cycles_scene_add_object(ccl::Session* session_id);
/**
 * Set transformation matrix for object
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_scene_object_set_matrix(ccl::Session* session_id, ccl::Object*,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	);
/**
 * Set OCS frame for object
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_scene_object_set_ocs_frame(ccl::Session* session_id, ccl::Object*,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	);
/**
 * Set object mesh
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_scene_object_set_geometry(ccl::Session* session_id, ccl::Object*, ccl::Geometry*);
/**
 * Set visibility flag for object
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_scene_object_set_visibility(ccl::Session* session_id, ccl::Object*, unsigned int visibility);
/**
 * Set object shader
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_scene_object_set_shader(ccl::Session* session_id, ccl::Object*, ccl::Shader* shader_id);
/**
 * Set is_shadow_catcher flag for object
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_scene_object_set_is_shadowcatcher(ccl::Session* session_id, ccl::Object*, bool is_shadowcatcher);
/**
 * Set mesh_light_no_cast_shadow flag for object. This is to signal that this mesh light shouldn't cast shadows.
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_scene_object_set_mesh_light_no_cast_shadow(ccl::Session* session_id, ccl::Object*, bool mesh_light_no_cast_shadow);
/**
 * Tag object for update
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_object_tag_update(ccl::Session* session_id, ccl::Object*);

/**
 * Set the pass id
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_object_set_pass_id(ccl::Session* session_id, ccl::Object*, int pass_id);

/**
 * Set the random id
 * \ingroup ccycles_object
 */
CCL_CAPI void CDECL cycles_object_set_random_id(ccl::Session* session_id, ccl::Object*, unsigned int random_id);

/**
 * Clear clipping planes list.
 */
CCL_CAPI void CDECL cycles_scene_clear_clipping_planes(ccl::Session* session_id);

/**
 * Add a clipping plane equation.
 */
CCL_CAPI unsigned int CDECL cycles_scene_add_clipping_plane(ccl::Session* session_id, float a, float b, float c, float d);

/**
 * Discard clipping plane (abcd are all set to FLT_MAX).
 */
CCL_CAPI void CDECL cycles_scene_discard_clipping_plane(ccl::Session* session_id, unsigned int cp_id);

/**
 * Set a clipping plane equation.
 */
CCL_CAPI void CDECL cycles_scene_set_clipping_plane(ccl::Session* session_id, unsigned int cp_id, float a, float b, float c, float d);

enum class sampling_pattern : unsigned int {
	SOBOL = 0,
	CMJ
};

/** Different camera types. */
enum class camera_type : unsigned int {
	PERSPECTIVE = 0,
	ORTHOGRAPHIC,
	PANORAMA
};

/** Different panorama types of camera. */
enum class panorama_type : unsigned int {
	EQUIRECTANGLUAR = 0,
	FISHEYE_EQUIDISTANT,
	FISHEYE_EQUISOLID
};

/** Set the size/resolution of the camera. This equals to pixel resolution. */
CCL_CAPI void CDECL cycles_camera_set_size(ccl::Session* session_id, unsigned int width, unsigned int height);
/** Get the camera width. */
CCL_CAPI unsigned int CDECL cycles_camera_get_width(ccl::Session* session_id);
/** Get the camera height. */
CCL_CAPI unsigned int CDECL cycles_camera_get_height(ccl::Session* session_id);
/** Set the camera type. */
CCL_CAPI void CDECL cycles_camera_set_type(ccl::Session* session_id, camera_type type);
/** Set the camera panorama type. */
CCL_CAPI void CDECL cycles_camera_set_panorama_type(ccl::Session* session_id, panorama_type type);
/** Set the transformation matrix for the camera. */
CCL_CAPI void CDECL cycles_camera_set_matrix(ccl::Session* session_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	);
/** Compute the auto viewplane for scene camera. */
CCL_CAPI void CDECL cycles_camera_compute_auto_viewplane(ccl::Session* session_id);
/** Set viewplane for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_viewplane(ccl::Session* session_id, float left, float right, float top, float bottom);
/** Update camera. Should be called after changing settings on a scene camera. */
CCL_CAPI void CDECL cycles_camera_update(ccl::Session* session_id);
/** Set the Field of View for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_fov(ccl::Session* session_id, float fov);
/** Set the sensor width for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_sensor_width(ccl::Session* session_id, float sensor_width);
/** Set the sensor height for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_sensor_height(ccl::Session* session_id, float sensor_height);
/** Set the near clip for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_nearclip(ccl::Session* session_id, float nearclip);
/** Set the far clip for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_farclip(ccl::Session* session_id, float farclip);
/** Set the aperture size for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_aperturesize(ccl::Session* session_id, float aperturesize);
/** Set the aperture ratio for anamorphic lens bokeh. */
CCL_CAPI void CDECL cycles_camera_set_aperture_ratio(ccl::Session* session_id, float aperture_ratio);
/** Set camera blades count. */
CCL_CAPI void CDECL cycles_camera_set_blades(ccl::Session* session_id, unsigned int blades);
/** Set camera blade rotation. */
CCL_CAPI void CDECL cycles_camera_set_bladesrotation(ccl::Session* session_id, float bladesrotation);
/** Set the focal distance for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_focaldistance(ccl::Session* session_id, float focaldistance);
/** Set the shutter time for scene camera. Used mainly with motion blur aspect of rendering process. */
CCL_CAPI void CDECL cycles_camera_set_shuttertime(ccl::Session* session_id, float shuttertime);
/** Set the field of view for fisheye camera. */
CCL_CAPI void CDECL cycles_camera_set_fisheye_fov(ccl::Session* session_id, float fisheye_fov);
/** Set the lens for fisheye camera. */
CCL_CAPI void CDECL cycles_camera_set_fisheye_lens(ccl::Session* session_id, float fisheye_lens);

/* Mesh geometry API */
CCL_CAPI void CDECL cycles_mesh_set_verts(ccl::Session* session_id, ccl::Geometry* mesh, float *verts, unsigned int vcount);
CCL_CAPI void CDECL cycles_mesh_set_tris(ccl::Session* session_id, ccl::Geometry* mesh, int *faces, unsigned int fcount, ccl::Shader *shader_id, unsigned int smooth);
CCL_CAPI void CDECL cycles_mesh_set_triangle(ccl::Session* session_id, ccl::Geometry* mesh, unsigned tri_idx, unsigned int v0, unsigned int v1, unsigned int v2, ccl::Shader *shader_id, unsigned int smooth);
CCL_CAPI void CDECL cycles_mesh_add_triangle(ccl::Session* session_id, ccl::Geometry* mesh, unsigned int v0, unsigned int v1, unsigned int v2, ccl::Shader *shader_id, unsigned int smooth);
CCL_CAPI void CDECL cycles_mesh_set_uvs(ccl::Session* session_id, ccl::Geometry* mesh, float *uvs, unsigned int uvcount, const char *uvmap_name);
CCL_CAPI void CDECL cycles_mesh_set_vertex_normals(ccl::Session* session_id, ccl::Geometry* mesh, float *vnormals, unsigned int vnormalcount);
CCL_CAPI void CDECL cycles_mesh_set_vertex_colors(ccl::Session* session_id, ccl::Geometry* mesh, float *vcolors, unsigned int vcolorcount);
CCL_CAPI void CDECL cycles_mesh_set_smooth(ccl::Session* session_id, ccl::Geometry* mesh, unsigned int smooth);
CCL_CAPI void CDECL cycles_geometry_clear(ccl::Session* session_id, ccl::Geometry* geo);
CCL_CAPI void CDECL cycles_mesh_reserve(ccl::Session* session_id, ccl::Geometry* mesh, unsigned vcount, unsigned fcount);
CCL_CAPI void CDECL cycles_mesh_resize(ccl::Session* session_id, ccl::Geometry* mesh, unsigned vcount, unsigned fcount);
CCL_CAPI void CDECL cycles_geometry_tag_rebuild(ccl::Session* session_id, ccl::Geometry* geo);
CCL_CAPI void CDECL cycles_geometry_set_shader(ccl::Session* session_id, ccl::Geometry* mesh, ccl::Shader *shader_id);
CCL_CAPI void CDECL cycles_mesh_attr_tangentspace(ccl::Session* session_id, ccl::Geometry* mesh, const char* uvmap_name);

/* Shader API */

#undef TRANSPARENT

// NOTE: keep in sync with available Cycles nodes
enum class shadernode_type : unsigned int {
	BACKGROUND = 0,
	OUTPUT,
	DIFFUSE,
	ANISOTROPIC,
	TRANSLUCENT,
	TRANSPARENT,
	VELVET,
	TOON,
	GLOSSY,
	GLASS,
	REFRACTION,
	HAIR,
	EMISSION,
	AMBIENT_OCCLUSION,
	ABSORPTION_VOLUME,
	SCATTER_VOLUME,
	SUBSURFACE_SCATTERING,
	VALUE,
	COLOR,
	MIX_CLOSURE,
	ADD_CLOSURE,
	INVERT,
	MIX,
	GAMMA,
	WAVELENGTH,
	BLACKBODY,
	CAMERA,
	FRESNEL,
	MATH,
	IMAGE_TEXTURE,
	ENVIRONMENT_TEXTURE,
	BRICK_TEXTURE,
	SKY_TEXTURE,
	CHECKER_TEXTURE,
	NOISE_TEXTURE,
	WAVE_TEXTURE,
	MAGIC_TEXTURE,
	MUSGRAVE_TEXTURE,
	TEXTURE_COORDINATE,
	BUMP,
	RGBTOBW,
	RGBTOLUMINANCE,
	LIGHTPATH,
	LIGHTFALLOFF,
	LAYERWEIGHT,
	GEOMETRYINFO,
	VORONOI_TEXTURE,
	COMBINE_XYZ,
	SEPARATE_XYZ,
	HSV_SEPARATE,
	HSV_COMBINE,
	RGB_SEPARATE,
	RGB_COMBINE,
	MAPPING,
	HOLDOUT,
	HUE_SAT,
	BRIGHT_CONTRAST,
	GRADIENT_TEXTURE,
	COLOR_RAMP,
	VECT_MATH,
	PRINCIPLED_BSDF,
	ATTRIBUTE,
	NORMALMAP,
	WIREFRAME,
	OBJECTINFO,
	TANGENT,
	DISPLACEMENT,
	RHINO_TEXTURE_COORDINATE,
	RHINO_MATRIX_MATH,
	RHINO_AZIMUTH_ALTITUDE_TRANSFORM,
	RHINO_CHECKER_TEXTURE,
	RHINO_NOISE_TEXTURE,
	RHINO_WAVES_TEXTURE,
	RHINO_WAVES_WIDTH_TEXTURE,
	RHINO_PERTURBING_PART1_TEXTURE,
	RHINO_PERTURBING_PART2_TEXTURE,
	RHINO_GRADIENT_TEXTURE,
	RHINO_BLEND_TEXTURE,
	RHINO_EXPOSURE_TEXTURE,
	RHINO_FBM_TEXTURE,
	RHINO_GRID_TEXTURE,
	RHINO_PROJECTION_CHANGER_TEXTURE,
	RHINO_MASK_TEXTURE,
	RHINO_PERLIN_MARBLE_TEXTURE,
	RHINO_PHYSICAL_SKY_TEXTURE,
	RHINO_TEXTURE_ADJUSTMENT_TEXTURE,
	RHINO_TILE_TEXTURE,
	RHINO_NORMAL_PART1_TEXTURE,
	RHINO_NORMAL_PART2_TEXTURE,
};

/***** LIGHTS ****/

/**
 * Different light types available for Cycles
 */
enum class light_type: unsigned int {
	Point = 0,
	Sun, /* = distant, also Hemi */
	Background,
	Area,
	Spot,
	Triangle,
};

CCL_CAPI ccl::Light *CDECL cycles_create_light(ccl::Session* session_id, ccl::Shader *light_shader_id);
CCL_CAPI void CDECL cycles_light_set_type(ccl::Session *session_id, ccl::Light *light_id, light_type type);
CCL_CAPI void CDECL cycles_light_set_angle(ccl::Session *session_id, ccl::Light *light_id, float angle);
CCL_CAPI void CDECL cycles_light_set_spot_angle(ccl::Session *session_id, ccl::Light *light_id, float spot_angle);
CCL_CAPI void CDECL cycles_light_set_spot_smooth(ccl::Session *session_id, ccl::Light *light_id, float spot_smooth);
CCL_CAPI void CDECL cycles_light_set_cast_shadow(ccl::Session *session_id, ccl::Light *light_id, unsigned int cast_shadow);
CCL_CAPI void CDECL cycles_light_set_use_mis(ccl::Session *session_id, ccl::Light *light_id, unsigned int use_mis);
CCL_CAPI void CDECL cycles_light_set_samples(ccl::Session *session_id, ccl::Light *light_id, unsigned int samples);
CCL_CAPI void CDECL cycles_light_set_max_bounces(ccl::Session *session_id, ccl::Light *light_id, unsigned int max_bounces);
CCL_CAPI void CDECL cycles_light_set_map_resolution(ccl::Session *session_id, ccl::Light *light_id, unsigned int map_resolution);
CCL_CAPI void CDECL cycles_light_set_sizeu(ccl::Session *session_id, ccl::Light *light_id, float sizeu);
CCL_CAPI void CDECL cycles_light_set_sizev(ccl::Session *session_id, ccl::Light *light_id, float sizev);
CCL_CAPI void CDECL cycles_light_set_axisu(ccl::Session *session_id, ccl::Light *light_id, float axisux, float axisuy, float axisuz);
CCL_CAPI void CDECL cycles_light_set_axisv(ccl::Session *session_id, ccl::Light *light_id, float axisvx, float axisvy, float axisvz);
CCL_CAPI void CDECL cycles_light_set_size(ccl::Session *session_id, ccl::Light *light_id, float size);
CCL_CAPI void CDECL cycles_light_set_dir(ccl::Session *session_id, ccl::Light *light_id, float dirx, float diry, float dirz);
CCL_CAPI void CDECL cycles_light_set_co(ccl::Session *session_id, ccl::Light *light_id, float cox, float coy, float coz);
CCL_CAPI void CDECL cycles_light_tag_update(ccl::Session* session_id, ccl::Light *light_id);

CCL_CAPI void CDECL cycles_film_set_exposure(ccl::Session* session_id, float exposure);
CCL_CAPI void CDECL cycles_film_set_filter(ccl::Session* session_id, unsigned int filter_type, float filter_width);
CCL_CAPI void CDECL cycles_film_set_use_sample_clamp(ccl::Session* session_id, bool use_sample_clamp);
CCL_CAPI void CDECL cycles_film_tag_update(ccl::Session* session_id);
CCL_CAPI void CDECL cycles_film_set_use_approximate_shadow_catcher(ccl::Session *session, bool use_approximate_shadow_catcher);

CCL_CAPI void CDECL cycles_f4_add(ccl::float4 a, ccl::float4 b, ccl::float4& res);
CCL_CAPI void CDECL cycles_f4_sub(ccl::float4 a, ccl::float4 b, ccl::float4& res);
CCL_CAPI void CDECL cycles_f4_mul(ccl::float4 a, ccl::float4 b, ccl::float4& res);
CCL_CAPI void CDECL cycles_f4_div(ccl::float4 a, ccl::float4 b, ccl::float4& res);

CCL_CAPI void CDECL cycles_tfm_inverse(const ccl::Transform& t, ccl::Transform& res);
CCL_CAPI void CDECL cycles_tfm_lookat(const ccl::float3& position, const ccl::float3& look, const ccl::float3& up, ccl::Transform& res);
CCL_CAPI void CDECL cycles_tfm_rotate_around_axis(float angle, const ccl::float3& axis, ccl::Transform& res);

CCL_CAPI void CDECL cycles_apply_gamma_to_byte_buffer(unsigned char* rgba_buffer, size_t size_in_bytes, float gamma);
CCL_CAPI void CDECL cycles_apply_gamma_to_float_buffer(float* rgba_buffer, size_t size_in_bytes, float gamma);

CCL_CAPI void CDECL cycles_set_rhino_perlin_noise_table(int* data, unsigned int count);
CCL_CAPI void CDECL cycles_set_rhino_impulse_noise_table(float* data, unsigned int count);
CCL_CAPI void CDECL cycles_set_rhino_vc_noise_table(float* data, unsigned int count);
CCL_CAPI void CDECL cycles_set_rhino_aaltonen_noise_table(const int* data, unsigned int count);

#ifdef __cplusplus
}
#endif

#endif
