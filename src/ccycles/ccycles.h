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

#ifdef __cplusplus
extern "C" {
#endif

/** \defgroup ccycles CCycles
 * Low-level C API for setting up and driving the Cycles render engine. */
/** \defgroup ccycles_scene Scene API
 * \ingroup ccycles */
/** \defgroup ccycles_shader Shader API
 * \ingroup ccycles */
/** \defgroup ccycles_mesh Mesh API
 * \ingroup ccycles */
/** \defgroup ccycles_object Object API
 * \ingroup ccycles */
/** \defgroup ccycles_session Session API
 * \ingroup ccycles */

/***********************************/

/* Opaque Rhino light handle (internal_types.h); csycles passes it as IntPtr. */
struct CCyclesLight;

class StringHolder
{
public:
	std::string thestring;
};

/** Initialise Cycles by querying available devices. \ingroup ccycles */
CCL_CAPI void CDECL cycles_initialise(unsigned int mask = ccl::DEVICE_MASK_ALL);

/** DeviceTypeMask of GPUs whose initialisation threw in cycles_initialise; the
 * others are still listed. \ingroup ccycles */
CCL_CAPI unsigned int CDECL cycles_failed_gpus_mask();

/** Init error for a DeviceType from the last cycles_initialise, "" if it did not fail.
 * The pointer is valid until the next cycles_gpu_init_error call. \ingroup ccycles */
CCL_CAPI const char* CDECL cycles_gpu_init_error(unsigned int device_type);

/** Why OptiX offered no devices; not a GPU failure, CUDA still renders on the card.
 * 0 started (or never tried), 1 driver too old for this build's OptiX SDK, 2 other.
 * \ingroup ccycles */
CCL_CAPI int CDECL cycles_optix_init_result();

/** Oldest NVIDIA driver branch for this build's OptiX SDK (590 for OptiX 9.1); 0 without
 * OptiX or when the SDK is newer than the lookup table. \ingroup ccycles */
CCL_CAPI int CDECL cycles_optix_minimum_driver();

/** Set where Cycles looks for precompiled kernels, cached kernels and kernel source.
 * \ingroup ccycles */
CCL_CAPI void CDECL cycles_path_init(const char* path, const char* user_path);

/** Clean up everything. \ingroup ccycles
 * \todo Per-session cleanup, so sessions in progress are not deleted. */
CCL_CAPI void CDECL cycles_shutdown();

/** Also send logger output to std::cout. Global to the logger. */
CCL_CAPI void CDECL cycles_log_to_stdout(int tostdout);

/** Create a new mesh in session_id, using shader_id. \ingroup ccycles_scene */
CCL_CAPI ccl::Geometry* CDECL cycles_scene_add_mesh(ccl::Session* session_id, ccl::Shader *shader_id);
/** Create a new object for session_id. \ingroup ccycles_scene */
CCL_CAPI ccl::Object* CDECL cycles_scene_add_object(ccl::Session* session_id);
/** Set transformation matrix for object. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_scene_object_set_matrix(ccl::Session* session_id, ccl::Object*,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	);
/** Set OCS frame for object. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_scene_object_set_ocs_frame(ccl::Session* session_id, ccl::Object*,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	);
/** Set the object's planar UVW mapping. */
CCL_CAPI void CDECL cycles_scene_object_set_planar_uvw_mapping(ccl::Session* session_id, ccl::Object*,
	unsigned int has_mapping,
	unsigned int capped,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	);
/** Set object mesh. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_scene_object_set_geometry(ccl::Session* session_id, ccl::Object*, ccl::Geometry*);
/** Set visibility flag for object. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_scene_object_set_visibility(ccl::Session* session_id, ccl::Object*, unsigned int visibility);
/** Set object shader. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_scene_object_set_shader(ccl::Session* session_id, ccl::Object*, ccl::Shader* shader_id);
/** Set is_shadow_catcher flag for object. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_scene_object_set_is_shadowcatcher(ccl::Session* session_id, ccl::Object*, bool is_shadowcatcher);
/** Set is_solid flag for object. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_scene_object_set_is_solid(ccl::Session* session_id, ccl::Object*, bool is_solid);
/** Stop this object's mesh light from casting shadows. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_scene_object_set_mesh_light_no_cast_shadow(ccl::Session* session_id, ccl::Object*, bool mesh_light_no_cast_shadow);
/** Tag object for update. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_object_tag_update(ccl::Session* session_id, ccl::Object*);

/** Set the pass id. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_object_set_pass_id(ccl::Session* session_id, ccl::Object*, int pass_id);

/** Set the random id. \ingroup ccycles_object */
CCL_CAPI void CDECL cycles_object_set_random_id(ccl::Session* session_id, ccl::Object*, unsigned int random_id);

/** Clear clipping planes list. */
CCL_CAPI void CDECL cycles_scene_clear_clipping_planes(ccl::Session* session_id);

/** Add a clipping plane equation. */
CCL_CAPI unsigned int CDECL cycles_scene_add_clipping_plane(ccl::Session* session_id, float a, float b, float c, float d);

/** Mirrors ccl::SamplingPattern (kernel/types.h); csycles' SamplingPattern sends these. */
enum class sampling_pattern : unsigned int {
	SOBOL_BURLEY = 0,
	TABULATED_SOBOL = 1,
	BLUE_NOISE_PURE = 2,
	BLUE_NOISE_FIRST = 3,
	BLUE_NOISE_ROUND = 4,
	AUTOMATIC = 5,
};

/** Different camera types. */
enum class camera_type : unsigned int {
	PERSPECTIVE = 0,
	ORTHOGRAPHIC,
	PANORAMA
};

/** Set the camera resolution in pixels. */
CCL_CAPI void CDECL cycles_camera_set_size(ccl::Session* session_id, unsigned int width, unsigned int height);
/** Get the camera width. */
CCL_CAPI unsigned int CDECL cycles_camera_get_width(ccl::Session* session_id);
/** Get the camera height. */
CCL_CAPI unsigned int CDECL cycles_camera_get_height(ccl::Session* session_id);
/** Set the camera type. */
CCL_CAPI void CDECL cycles_camera_set_type(ccl::Session* session_id, camera_type type);
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
/** Update the scene camera; call after changing its settings. */
CCL_CAPI void CDECL cycles_camera_update(ccl::Session* session_id);
/** Set the Field of View for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_fov(ccl::Session* session_id, float fov);
/** Set the sensor width for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_sensor_width(ccl::Session* session_id, float sensor_width);
/** Set the sensor height for scene camera. */
CCL_CAPI void CDECL cycles_camera_set_sensor_height(ccl::Session* session_id, float sensor_height);
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

/* Mesh geometry API */
CCL_CAPI void CDECL cycles_mesh_set_verts(ccl::Session* session_id, ccl::Geometry* mesh, float *verts, unsigned int vcount);
CCL_CAPI void CDECL cycles_mesh_set_tris(ccl::Session* session_id, ccl::Geometry* mesh, int *faces, unsigned int fcount, ccl::Shader *shader_id, unsigned int smooth);
CCL_CAPI void CDECL cycles_mesh_set_uvs(ccl::Session* session_id, ccl::Geometry* mesh, float *uvs, unsigned int uvcount, const char *uvmap_name);
CCL_CAPI void CDECL cycles_mesh_set_vertex_normals(ccl::Session* session_id, ccl::Geometry* mesh, float *vnormals, unsigned int vnormalcount);
CCL_CAPI void CDECL cycles_mesh_set_vertex_colors(ccl::Session* session_id, ccl::Geometry* mesh, float *vcolors, unsigned int vcolorcount);
CCL_CAPI void CDECL cycles_geometry_clear(ccl::Session* session_id, ccl::Geometry* geo);
CCL_CAPI void CDECL cycles_mesh_resize(ccl::Session* session_id, ccl::Geometry* mesh, unsigned vcount, unsigned fcount);
CCL_CAPI void CDECL cycles_geometry_tag_rebuild(ccl::Session* session_id, ccl::Geometry* geo);
CCL_CAPI void CDECL cycles_geometry_set_shader(ccl::Session* session_id, ccl::Geometry* mesh, ccl::Shader *shader_id);
CCL_CAPI void CDECL cycles_mesh_attr_tangentspace(ccl::Session* session_id, ccl::Geometry* mesh, const char* uvmap_name);

/***** LIGHTS ****/

/** Light types. */
enum class light_type: unsigned int {
	Point = 0,
	Sun, /* = distant, also Hemi */
	Background,
	Area,
	Spot,
	Triangle,
};

CCL_CAPI CCyclesLight *CDECL cycles_create_light(ccl::Session* session_id, ccl::Shader *light_shader_id);
CCL_CAPI void CDECL cycles_light_set_type(ccl::Session *session_id, CCyclesLight *light_id, light_type type);
CCL_CAPI void CDECL cycles_light_set_angle(ccl::Session *session_id, CCyclesLight *light_id, float angle);
CCL_CAPI void CDECL cycles_light_set_spot_angle(ccl::Session *session_id, CCyclesLight *light_id, float spot_angle);
CCL_CAPI void CDECL cycles_light_set_spot_smooth(ccl::Session *session_id, CCyclesLight *light_id, float spot_smooth);
CCL_CAPI void CDECL cycles_light_set_cast_shadow(ccl::Session *session_id, CCyclesLight *light_id, unsigned int cast_shadow);
CCL_CAPI void CDECL cycles_light_set_use_mis(ccl::Session *session_id, CCyclesLight *light_id, unsigned int use_mis);
CCL_CAPI void CDECL cycles_light_set_max_bounces(ccl::Session *session_id, CCyclesLight *light_id, unsigned int max_bounces);
CCL_CAPI void CDECL cycles_light_set_sizeu(ccl::Session *session_id, CCyclesLight *light_id, float sizeu);
CCL_CAPI void CDECL cycles_light_set_sizev(ccl::Session *session_id, CCyclesLight *light_id, float sizev);
CCL_CAPI void CDECL cycles_light_set_axisu(ccl::Session *session_id, CCyclesLight *light_id, float axisux, float axisuy, float axisuz);
CCL_CAPI void CDECL cycles_light_set_axisv(ccl::Session *session_id, CCyclesLight *light_id, float axisvx, float axisvy, float axisvz);
CCL_CAPI void CDECL cycles_light_set_size(ccl::Session *session_id, CCyclesLight *light_id, float size);
CCL_CAPI void CDECL cycles_light_set_dir(ccl::Session *session_id, CCyclesLight *light_id, float dirx, float diry, float dirz);
CCL_CAPI void CDECL cycles_light_set_co(ccl::Session *session_id, CCyclesLight *light_id, float cox, float coy, float coz);
CCL_CAPI void CDECL cycles_light_tag_update(ccl::Session* session_id, CCyclesLight *light_id);

CCL_CAPI void CDECL cycles_film_set_exposure(ccl::Session* session_id, float exposure);
CCL_CAPI void CDECL cycles_film_set_filter(ccl::Session* session_id, unsigned int filter_type, float filter_width);
CCL_CAPI void CDECL cycles_film_tag_update(ccl::Session* session_id);
CCL_CAPI void CDECL cycles_film_set_use_approximate_shadow_catcher(ccl::Session *session, bool use_approximate_shadow_catcher);

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
