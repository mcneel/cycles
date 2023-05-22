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
#else
#define CCL_CAPI
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
typedef void(__cdecl *LOGGER_FUNC_CB)(const char* msg);

/**
 * Status update function signature. Used to register a status
 * update callback function with CCycles using cycles_session_set_update_callback
 * \ingroup ccycles ccycles_session
 */
typedef void(__cdecl *STATUS_UPDATE_CB)(ccl::Session* session_id);

/**
 * Test cancel function signature. Used to test for cancel condition
 * test callback function with CCycles using cycles_session_set_cancel_callback
 * \ingroup ccycles ccycles_session
 */
typedef void(__cdecl *TEST_CANCEL_CB)(ccl::Session* session_id);

/**
 * Render tile update or write function signature. Used to register a render tile
 * update callback function with CCycles using cycles_session_set_write_tile_callback or
 * cycles_session_set_update_tile_callback
 * \ingroup ccycles ccycles_session
 */
typedef void(__cdecl *RENDER_TILE_CB)(ccl::Session* session_id, unsigned int x, unsigned int y, unsigned int w, unsigned int h, unsigned int sample, unsigned int depth, int passtype, float* pixels, int pixlen);

/**
 * Pixel buffer from DisplayBuffer update function signature.
 * Set update function to CCycles using cycles_session_set_display_update_callback
 * \ingroup ccycles ccycles_session
 */
typedef void(__cdecl *DISPLAY_UPDATE_CB)(ccl::Session* session_id, unsigned int sample);


/**
 * Initialise Cycles by querying available devices.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_initialise(unsigned int mask = ccl::DEVICE_MASK_ALL);

/**
 * Initialise paths for Cycles to look in (pre-compiled kernels, cached kernels, kernel code)
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_path_init(const char* path, const char* user_path);

/**
 * Set an environment variable for Cycles
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_putenv(const char* var, const char* val);

/**
 * Set the CPU kernel type to use. 1 is split, 0 is regular.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_debug_set_cpu_kernel(unsigned int state);

/**
 * Pass 1 to allow QBVH usage in CPU kernel.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_debug_set_cpu_allow_qbvh(unsigned int state);

/**
 * Set the CUDA kernel type to use. 1 is split, 0 is regular.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_debug_set_cuda_kernel(unsigned int state);

/**
 * Set the OpenCL kernel type to use. 1 is split, 0 is mega. -1 means decide
 * automatically based on officially supported devices.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_debug_set_opencl_kernel(int state);

/**
 * Set the OpenCL kernel to be compiled as single program with 1.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_debug_set_opencl_single_program(int state);

/**
 * Set the OpenCL device type allowed.
 * 0 = none
 * 1 = all
 * 2 = default
 * 3 = CPU
 * 4 = GPU
 * 5 = accelerator
 */
CCL_CAPI void __cdecl cycles_debug_set_opencl_device_type(int type);

/**
 * Clean up everything, we're done.
 * \ingroup ccycles
 * \todo Add session specific cleanup, so we don't accidently delete sessions that are in progress.
 */
CCL_CAPI void __cdecl cycles_shutdown();

/**
 * Add a logger function. This will be called only in debug builds.
 * \ingroup ccycles
 */
CCL_CAPI void __cdecl cycles_set_logger(LOGGER_FUNC_CB logger_func_);

/**
 * Set to true if logger output should be sent to std::cout as well.
 *
 * Note that this is global to the logger.
 */
CCL_CAPI void __cdecl cycles_log_to_stdout(int tostdout);

/**
 * Create a new client.
 *
 * This is mainly used for determining what logger functions belong to which client and session.
 *
 * Note that a client needs to be released with cycles_release_client when no longer necessary.
 */
CCL_CAPI unsigned int __cdecl cycles_new_client();

/**
 * Release a client from usage.
 */
CCL_CAPI void __cdecl cycles_release_client();

/**
 * Query number of available devices.
 * \ingroup ccycles
 */
CCL_CAPI unsigned int __cdecl cycles_number_devices();

/**
 * Query number of available (created) multi-devices.
 * \ingroup ccycles
 */
CCL_CAPI unsigned int __cdecl cycles_number_multidevices();

/**
 * Query number of devices in multi-device
 * \ingroup ccycles
 */
CCL_CAPI unsigned int __cdecl cycles_number_multi_subdevices(int i);

/**
 * Query the index of the sub-device in the global device list.
 * \ingroup ccycles
 */
CCL_CAPI unsigned int __cdecl cycles_get_multidevice_subdevice_id(int i, int j);

/* Query number of available CUDA devices. */
CCL_CAPI unsigned int __cdecl cycles_number_cuda_devices();

/* Query name of a device. */
CCL_CAPI const char* __cdecl cycles_device_description(int i);

/* Query capabilities of all devices found. */
CCL_CAPI const char* __cdecl cycles_device_capabilities();

/* Query ID of a device. */
CCL_CAPI const char* __cdecl cycles_device_id(int i);

/* Query the index of a device. The index is the nth for the type the device is of. */
CCL_CAPI int __cdecl cycles_device_num(int i);

/* Query if device supports advanced shading. */
CCL_CAPI bool __cdecl cycles_device_advanced_shading(int i);

/* Query if device is used as display device. */
CCL_CAPI bool __cdecl cycles_device_display_device(int i);

/* Create or get multi device. Return value is index of multi-device in multi-device vector.*/
CCL_CAPI int __cdecl cycles_create_multidevice(int count, int* idx);

/** Query device type.
 * \param i device ID.
 * \returns device type
 * \retval 0 None
 * \retval 1 CPU
 * \retval 2 OPENCL
 * \retval 3 CUDA
 * \retval 4 NETWORK
 * \retval 5 MULTI
 */
CCL_CAPI unsigned int __cdecl cycles_device_type(int i);

/* Create scene parameters, to be used when creating a new scene. */
CCL_CAPI unsigned int __cdecl cycles_scene_params_create(unsigned int shadingsystem, unsigned int bvh_type, unsigned int use_bvh_spatial_split, int bvh_layout, unsigned int persistent_data);

/* Set scene parameters*/

/** Set scene parameter: BVH type. */
CCL_CAPI void __cdecl cycles_scene_params_set_bvh_type(unsigned int scene_params_id, unsigned int type);
/** Set scene parameter: use BVH spatial split. */
CCL_CAPI void __cdecl cycles_scene_params_set_bvh_spatial_split(unsigned int scene_params_id, unsigned int use);
/** Set scene parameter: use qBVH. */
CCL_CAPI void __cdecl cycles_scene_params_set_qbvh(unsigned int scene_params_id, unsigned int use);
/** Set scene parameter: Shading system (SVM / OSL).
 * Note that currently SVM is only supported for RhinoCycles. No effort yet has been taken to enable OSL.
 */
CCL_CAPI void __cdecl cycles_scene_params_set_shadingsystem(unsigned int scene_params_id, unsigned int system);
/** Set scene parameter: use persistent data. */
CCL_CAPI void __cdecl cycles_scene_params_set_persistent_data(unsigned int scene_params_id, unsigned int use);

/**
 * Create a new mesh in session_id, using shader_id
 * \ingroup ccycles_scene
 */
CCL_CAPI ccl::Geometry* __cdecl cycles_scene_add_mesh(ccl::Session* session_id, unsigned int shader_id);
/**
 * Create a new mesh for object_id in session_id, using shader_id
 * \ingroup ccycles_scene
 */
//CCL_CAPI unsigned int __cdecl cycles_scene_add_mesh_object(ccl::Session* session_id, unsigned int object_id, unsigned int shader_id);
/**
 * Create a new object for session_id
 * \ingroup ccycles_scene
 */
CCL_CAPI ccl::Object* __cdecl cycles_scene_add_object(ccl::Session* session_id);
/**
 * Set transformation matrix for object
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_matrix(ccl::Session* session_id, ccl::Object*,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	);
/**
 * Set OCS frame for object
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_ocs_frame(ccl::Session* session_id, ccl::Object*,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	);
/**
 * Set object mesh
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_geometry(ccl::Session* session_id, ccl::Object*, ccl::Geometry*);
/**
 * Set visibility flag for object
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_visibility(ccl::Session* session_id, ccl::Object*, unsigned int visibility);
/**
 * Set object shader
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_shader(ccl::Session* session_id, ccl::Object*, unsigned int shader_id);
/**
 * Set is_shadow_catcher flag for object
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_is_shadowcatcher(ccl::Session* session_id, ccl::Object*, bool is_shadowcatcher);
/**
 * Set mesh_light_no_cast_shadow flag for object. This is to signal that this mesh light shouldn't cast shadows.
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_mesh_light_no_cast_shadow(ccl::Session* session_id, ccl::Object*, bool mesh_light_no_cast_shadow);
/**
 * Set is_block_instance flag for object. This ensures we can handle meshes
 * properly also when only one block instance for a mesh is in the scene.
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_is_block_instance(ccl::Session* session_id, ccl::Object*, bool is_block_instance);
/**
 * Set cutout flag for object. This object is used for cutout/clipping.
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_cutout(ccl::Session* session_id, ccl::Object*, bool cutout);
/**
 * Set ignore_cutout flag for object. Ignore cutout object.
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_scene_object_set_ignore_cutout(ccl::Session* session_id, ccl::Object*, bool ignore_cutout);
/**
 * Tag object for update
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_object_tag_update(ccl::Session* session_id, ccl::Object*);

/**
 * Set the pass id
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_object_set_pass_id(ccl::Session* session_id, ccl::Object*, int pass_id);

/**
 * Set the random id
 * \ingroup ccycles_object
 */
CCL_CAPI void __cdecl cycles_object_set_random_id(ccl::Session* session_id, ccl::Object*, unsigned int random_id);

/**
 * Clear clipping planes list.
 */
CCL_CAPI void __cdecl cycles_scene_clear_clipping_planes(ccl::Session* session_id);

/**
 * Add a clipping plane equation.
 */
CCL_CAPI unsigned int __cdecl cycles_scene_add_clipping_plane(ccl::Session* session_id, float a, float b, float c, float d);

/**
 * Discard clipping plane (abcd are all set to FLT_MAX).
 */
CCL_CAPI void __cdecl cycles_scene_discard_clipping_plane(ccl::Session* session_id, unsigned int cp_id);

/**
 * Set a clipping plane equation.
 */
CCL_CAPI void __cdecl cycles_scene_set_clipping_plane(ccl::Session* session_id, unsigned int cp_id, float a, float b, float c, float d);
/** Tag integrator for update. */
CCL_CAPI void __cdecl cycles_integrator_tag_update(ccl::Session* session_id);
/** Set the maximum bounces for integrator. */
CCL_CAPI void __cdecl cycles_integrator_set_max_bounce(ccl::Session* session_id, int max_bounce);
/** Set the minimum bounces for integrator. */
CCL_CAPI void __cdecl cycles_integrator_set_min_bounce(ccl::Session* session_id, int min_bounce);
CCL_CAPI void __cdecl cycles_integrator_set_ao_factor(ccl::Session* session_id, float ao_factor);
CCL_CAPI void __cdecl cycles_integrator_set_ao_distance(ccl::Session* session_id, float ao_distance);
CCL_CAPI void __cdecl cycles_integrator_set_ao_bounces(ccl::Session* session_id, int ao_bounces);
CCL_CAPI void __cdecl cycles_integrator_set_ao_additive_factor(ccl::Session* session_id, float ao_additive_factor);
/** Set to true if caustics should be skipped.
 * \todo split for caustics_reflective and caustics_refractive.
 */
CCL_CAPI void __cdecl cycles_integrator_set_no_caustics(ccl::Session* session_id, bool no_caustics);
/** Set to true if shadows shouldn't be traced.
 */
CCL_CAPI void __cdecl cycles_integrator_set_no_shadows(ccl::Session* session_id, bool no_shadows);
/** Set the amount of mesh light samples. */
CCL_CAPI void __cdecl cycles_integrator_set_mesh_light_samples(ccl::Session* session_id, int mesh_light_samples);
/** Set the maximum amount of diffuse bounces. */
CCL_CAPI void __cdecl cycles_integrator_set_max_diffuse_bounce(ccl::Session* session_id, int max_diffuse_bounce);
/** Set the maximum amount of glossy bounces. */
CCL_CAPI void __cdecl cycles_integrator_set_max_glossy_bounce(ccl::Session* session_id, int max_glossy_bounce);
/** Set the maximum amount of transmission bounces. */
CCL_CAPI void __cdecl cycles_integrator_set_max_transmission_bounce(ccl::Session* session_id, int max_transmission_bounce);
/** Set the maximum amount of volume bounces. */
CCL_CAPI void __cdecl cycles_integrator_set_max_volume_bounce(ccl::Session* session_id, int max_volume_bounce);
/** Set the maximum amount of transparency bounces. */
CCL_CAPI void __cdecl cycles_integrator_set_transparent_max_bounce(ccl::Session* session_id, int transparent_max_bounce);
/** Set the minimum amount of transparency bounces. */
CCL_CAPI void __cdecl cycles_integrator_set_transparent_min_bounce(ccl::Session* session_id, int transparent_min_bounce);
/** Set the amount of AA samples. */
CCL_CAPI void __cdecl cycles_integrator_set_aa_samples(ccl::Session* session_id, int aa_samples);
/** Set the glossiness filter. */
CCL_CAPI void __cdecl cycles_integrator_set_filter_glossy(ccl::Session* session_id, float filter_glossy);
/** Set integrator method to use (path, branched path).*/
CCL_CAPI void __cdecl cycles_integrator_set_method(ccl::Session* session_id, int method);
/** Set to true if all lights should be directly sampled. */
CCL_CAPI void __cdecl cycles_integrator_set_sample_all_lights_direct(ccl::Session* session_id, bool sample_all_lights_direct);
/** Set to true if all lights should be indirectly sampled. */
CCL_CAPI void __cdecl cycles_integrator_set_sample_all_lights_indirect(ccl::Session* session_id, bool sample_all_lights_indirect);
CCL_CAPI void __cdecl cycles_integrator_set_volume_step_rate(ccl::Session* session_id, float volume_step_rate);
CCL_CAPI void __cdecl cycles_integrator_set_volume_max_steps(ccl::Session* session_id, int volume_max_steps);
CCL_CAPI void __cdecl cycles_integrator_set_caustics_reflective(ccl::Session *session_id, bool caustics_reflective);
CCL_CAPI void __cdecl cycles_integrator_set_caustics_refractive(ccl::Session *session_id, bool caustics_refractive);
CCL_CAPI void __cdecl cycles_integrator_set_seed(ccl::Session* session_id, int seed);

enum class sampling_pattern : unsigned int {
	SOBOL = 0,
	CMJ
};
CCL_CAPI void __cdecl cycles_integrator_set_sampling_pattern(ccl::Session* session_id, sampling_pattern pattern);
CCL_CAPI void __cdecl cycles_integrator_set_sample_clamp_direct(ccl::Session* session_id, float sample_clamp_direct);
CCL_CAPI void __cdecl cycles_integrator_set_sample_clamp_indirect(ccl::Session* session_id, float sample_clamp_indirect);
CCL_CAPI void __cdecl cycles_integrator_set_light_sampling_threshold(ccl::Session* session_id, float light_sampling_threshold);

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
CCL_CAPI void __cdecl cycles_camera_set_size(ccl::Session* session_id, unsigned int width, unsigned int height);
/** Get the camera width. */
CCL_CAPI unsigned int __cdecl cycles_camera_get_width(ccl::Session* session_id);
/** Get the camera height. */
CCL_CAPI unsigned int __cdecl cycles_camera_get_height(ccl::Session* session_id);
/** Set the camera type. */
CCL_CAPI void __cdecl cycles_camera_set_type(ccl::Session* session_id, camera_type type);
/** Set the camera panorama type. */
CCL_CAPI void __cdecl cycles_camera_set_panorama_type(ccl::Session* session_id, panorama_type type);
/** Set the transformation matrix for the camera. */
CCL_CAPI void __cdecl cycles_camera_set_matrix(ccl::Session* session_id,
	float a, float b, float c, float d,
	float e, float f, float g, float h,
	float i, float j, float k, float l
	);
/** Compute the auto viewplane for scene camera. */
CCL_CAPI void __cdecl cycles_camera_compute_auto_viewplane(ccl::Session* session_id);
/** Set viewplane for scene camera. */
CCL_CAPI void __cdecl cycles_camera_set_viewplane(ccl::Session* session_id, float left, float right, float top, float bottom);
/** Update camera. Should be called after changing settings on a scene camera. */
CCL_CAPI void __cdecl cycles_camera_update(ccl::Session* session_id);
/** Set the Field of View for scene camera. */
CCL_CAPI void __cdecl cycles_camera_set_fov(ccl::Session* session_id, float fov);
/** Set the sensor width for scene camera. */
CCL_CAPI void __cdecl cycles_camera_set_sensor_width(ccl::Session* session_id, float sensor_width);
/** Set the sensor height for scene camera. */
CCL_CAPI void __cdecl cycles_camera_set_sensor_height(ccl::Session* session_id, float sensor_height);
/** Set the near clip for scene camera. */
CCL_CAPI void __cdecl cycles_camera_set_nearclip(ccl::Session* session_id, float nearclip);
/** Set the far clip for scene camera. */
CCL_CAPI void __cdecl cycles_camera_set_farclip(ccl::Session* session_id, float farclip);
/** Set the aperture size for scene camera. */
CCL_CAPI void __cdecl cycles_camera_set_aperturesize(ccl::Session* session_id, float aperturesize);
/** Set the aperture ratio for anamorphic lens bokeh. */
CCL_CAPI void __cdecl cycles_camera_set_aperture_ratio(ccl::Session* session_id, float aperture_ratio);
/** Set camera blades count. */
CCL_CAPI void __cdecl cycles_camera_set_blades(ccl::Session* session_id, unsigned int blades);
/** Set camera blade rotation. */
CCL_CAPI void __cdecl cycles_camera_set_bladesrotation(ccl::Session* session_id, float bladesrotation);
/** Set the focal distance for scene camera. */
CCL_CAPI void __cdecl cycles_camera_set_focaldistance(ccl::Session* session_id, float focaldistance);
/** Set the shutter time for scene camera. Used mainly with motion blur aspect of rendering process. */
CCL_CAPI void __cdecl cycles_camera_set_shuttertime(ccl::Session* session_id, float shuttertime);
/** Set the field of view for fisheye camera. */
CCL_CAPI void __cdecl cycles_camera_set_fisheye_fov(ccl::Session* session_id, float fisheye_fov);
/** Set the lens for fisheye camera. */
CCL_CAPI void __cdecl cycles_camera_set_fisheye_lens(ccl::Session* session_id, float fisheye_lens);

/** Create a new session for scene id. */
CCL_CAPI ccl::Session* __cdecl cycles_session_create(ccl::SessionParams* session_params_id);

/** Reset session. */
CCL_CAPI int __cdecl cycles_session_reset(ccl::Session* session_id, unsigned int width, unsigned int height, unsigned int samples, unsigned int full_x, unsigned int full_y, unsigned int full_width, unsigned int full_height );

CCL_CAPI void __cdecl cycles_session_add_pass(ccl::Session* session_id, int pass_id);
CCL_CAPI void __cdecl cycles_session_clear_passes(ccl::Session* session_id);

/** Set the status update callback for session. */
CCL_CAPI void __cdecl cycles_session_set_update_callback(ccl::Session* session_id, void(*update)(unsigned int sid));
/** Set the test cancel callback for session. */
CCL_CAPI void __cdecl cycles_session_set_cancel_callback(ccl::Session* session_id, void(*cancel)(unsigned int sid));
/** Set the render tile update callback for session. */
CCL_CAPI void __cdecl cycles_session_set_update_tile_callback(ccl::Session* session_id, RENDER_TILE_CB update_tile_cb);
/** Set the render tile write callback for session. */
CCL_CAPI void __cdecl cycles_session_set_write_tile_callback(ccl::Session* session_id, RENDER_TILE_CB write_tile_cb);
/** Set the display update callback for session. */
CCL_CAPI void __cdecl cycles_session_set_display_update_callback(ccl::Session* session_id, DISPLAY_UPDATE_CB display_update_cb);
/** Cancel session with cancel_message for log. */
CCL_CAPI void __cdecl cycles_session_cancel(ccl::Session* session_id, const char *cancel_message);
/** Start given session render process. */
CCL_CAPI void __cdecl cycles_session_start(ccl::Session* session_id);
/** Wait for session render process to finish or cancel. */
CCL_CAPI void __cdecl cycles_session_wait(ccl::Session* session_id);
/** Pause (true) or un-pause (false) a render session. */
CCL_CAPI void __cdecl cycles_session_set_pause(ccl::Session* session_id, bool pause);
/** True if session is paused. */
CCL_CAPI bool __cdecl cycles_session_is_paused(ccl::Session* session_id);
/** Set session samples to render. */
CCL_CAPI void __cdecl cycles_session_set_samples(ccl::Session* session_id, int samples);
/** Clear resources for session. */
CCL_CAPI void __cdecl cycles_session_destroy(ccl::Session* session_id);
CCL_CAPI void __cdecl cycles_session_get_float_buffer(ccl::Session* session_id, int passtype, float** pixels);
CCL_CAPI void __cdecl cycles_session_retain_float_buffer(
	ccl::Session *session_id, int passtype, int width, int height, float **pixels);
CCL_CAPI void __cdecl cycles_session_release_float_buffer(ccl::Session *session_id, int passtype);
	/** Get pixel data buffer pointer. */
CCL_CAPI int __cdecl cycles_session_sample(ccl::Session* session_id);
CCL_CAPI void __cdecl cycles_session_end_run(ccl::Session* session_id);


/* session progress access. */
CCL_CAPI void __cdecl cycles_progress_reset(ccl::Session* session_id);
CCL_CAPI int __cdecl cycles_progress_get_sample(ccl::Session* session_id);
CCL_CAPI void __cdecl cycles_progress_get_time(ccl::Session* session_id, double* total_time, double* sample_time);
CCL_CAPI void __cdecl cycles_tilemanager_get_sample_info(ccl::Session* session_id, unsigned int* samples, unsigned int* total_samples);
CCL_CAPI void __cdecl cycles_progress_get_progress(ccl::Session* session_id, float* progress);
CCL_CAPI bool __cdecl cycles_progress_get_status(ccl::Session* session_id, void* strholder);
CCL_CAPI bool __cdecl cycles_progress_get_substatus(ccl::Session* session_id, void* strholder);
CCL_CAPI void* __cdecl cycles_string_holder_new();
CCL_CAPI const char* __cdecl cycles_string_holder_get(void* strholder);
CCL_CAPI void __cdecl cycles_string_holder_delete(void* strholder);

CCL_CAPI ccl::SessionParams* __cdecl cycles_session_params_create(unsigned int device);

CCL_CAPI void __cdecl cycles_session_params_set_device(ccl::SessionParams* session_params_id, unsigned int device);
CCL_CAPI void __cdecl cycles_session_params_set_background(ccl::SessionParams* session_params_id, unsigned int background);
CCL_CAPI void __cdecl cycles_session_params_set_progressive_refine(ccl::SessionParams* session_params_id, unsigned int progressive_refine);
CCL_CAPI void __cdecl cycles_session_params_set_output_path(ccl::SessionParams* session_params_id, const char *output_path);
CCL_CAPI void __cdecl cycles_session_params_set_progressive(ccl::SessionParams* session_params_id, unsigned int progressive);
CCL_CAPI void __cdecl cycles_session_params_set_experimental(ccl::SessionParams* session_params_id, unsigned int experimental);
CCL_CAPI void __cdecl cycles_session_params_set_samples(ccl::SessionParams* session_params_id, int samples);
CCL_CAPI void __cdecl cycles_session_params_set_tile_size(ccl::SessionParams* session_params_id, unsigned int tile_size);
CCL_CAPI void __cdecl cycles_session_params_set_tile_order(ccl::SessionParams* session_params_id, unsigned int tile_order);
CCL_CAPI void __cdecl cycles_session_params_set_start_resolution(ccl::SessionParams* session_params_id, int start_resolution);
CCL_CAPI void __cdecl cycles_session_params_set_threads(ccl::SessionParams* session_params_id, unsigned int threads);
CCL_CAPI void __cdecl cycles_session_params_set_display_buffer_linear(ccl::SessionParams* session_params_id, unsigned int display_buffer_linear);
CCL_CAPI void __cdecl cycles_session_params_set_skip_linear_to_srgb_conversion(ccl::SessionParams* session_params_id, unsigned int skip_linear_to_srgb_conversion);
CCL_CAPI void __cdecl cycles_session_params_set_cancel_timeout(ccl::SessionParams* session_params_id, double cancel_timeout);
CCL_CAPI void __cdecl cycles_session_params_set_reset_timeout(ccl::SessionParams* session_params_id, double reset_timeout);
CCL_CAPI void __cdecl cycles_session_params_set_text_timeout(ccl::SessionParams* session_params_id, double text_timeout);
CCL_CAPI void __cdecl cycles_session_params_set_shadingsystem(ccl::SessionParams* session_params_id, unsigned int shadingsystem);
CCL_CAPI void __cdecl cycles_session_params_set_pixel_size(ccl::SessionParams* session_params_id, unsigned int pixel_size);

/* Create a new scene for specified device. */
CCL_CAPI unsigned int __cdecl cycles_scene_create(unsigned int scene_params_id, ccl::Session* session_id);
CCL_CAPI void __cdecl cycles_scene_set_background_shader(ccl::Session* session_id, unsigned int shader_id);
CCL_CAPI ccl::Shader* __cdecl cycles_scene_get_background_shader(ccl::Session* session_id);
CCL_CAPI void __cdecl cycles_scene_set_background_transparent(ccl::Session* session_id, unsigned int transparent);
CCL_CAPI void __cdecl cycles_scene_set_background_visibility(ccl::Session* session_id, unsigned int path_ray_flag);
CCL_CAPI void __cdecl cycles_scene_reset(ccl::Session* session_id);
CCL_CAPI bool __cdecl cycles_scene_try_lock(ccl::Session* session_id);
CCL_CAPI void __cdecl cycles_scene_lock(ccl::Session* session_id);
CCL_CAPI void __cdecl cycles_scene_unlock(ccl::Session* session_id);

/* Mesh geometry API */
CCL_CAPI void __cdecl cycles_mesh_set_verts(ccl::Session* session_id, ccl::Geometry* mesh, float *verts, unsigned int vcount);
CCL_CAPI void __cdecl cycles_mesh_set_tris(ccl::Session* session_id, ccl::Geometry* mesh, int *faces, unsigned int fcount, unsigned int shader_id, unsigned int smooth);
CCL_CAPI void __cdecl cycles_mesh_set_triangle(ccl::Session* session_id, ccl::Geometry* mesh, unsigned tri_idx, unsigned int v0, unsigned int v1, unsigned int v2, unsigned int shader_id, unsigned int smooth);
CCL_CAPI void __cdecl cycles_mesh_add_triangle(ccl::Session* session_id, ccl::Geometry* mesh, unsigned int v0, unsigned int v1, unsigned int v2, unsigned int shader_id, unsigned int smooth);
CCL_CAPI void __cdecl cycles_mesh_set_uvs(ccl::Session* session_id, ccl::Geometry* mesh, float *uvs, unsigned int uvcount, const char *uvmap_name);
CCL_CAPI void __cdecl cycles_mesh_set_vertex_normals(ccl::Session* session_id, ccl::Geometry* mesh, float *vnormals, unsigned int vnormalcount);
CCL_CAPI void __cdecl cycles_mesh_set_vertex_colors(ccl::Session* session_id, ccl::Geometry* mesh, float *vcolors, unsigned int vcolorcount);
CCL_CAPI void __cdecl cycles_mesh_set_smooth(ccl::Session* session_id, ccl::Geometry* mesh, unsigned int smooth);
CCL_CAPI void __cdecl cycles_geometry_clear(ccl::Session* session_id, ccl::Geometry* geo);
CCL_CAPI void __cdecl cycles_mesh_reserve(ccl::Session* session_id, ccl::Geometry* mesh, unsigned vcount, unsigned fcount);
CCL_CAPI void __cdecl cycles_mesh_resize(ccl::Session* session_id, ccl::Geometry* mesh, unsigned vcount, unsigned fcount);
CCL_CAPI void __cdecl cycles_geometry_tag_rebuild(ccl::Session* session_id, ccl::Geometry* geo);
CCL_CAPI void __cdecl cycles_geometry_set_shader(ccl::Session* session_id, ccl::Geometry* mesh, unsigned int shader_id);
CCL_CAPI void __cdecl cycles_mesh_attr_tangentspace(ccl::Session* session_id, ccl::Geometry* mesh, const char* uvmap_name);

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

CCL_CAPI ccl::Shader* __cdecl cycles_create_shader(ccl::Session* session_id);
CCL_CAPI int __cdecl cycles_shader_node_count(ccl::Shader *shader);
CCL_CAPI ccl::ShaderNode *__cdecl cycles_shader_node_get(ccl::Shader *shader, int idx);
CCL_CAPI bool __cdecl cycles_shadernode_get_name(ccl::ShaderNode *shn, void *strholder);
CCL_CAPI bool __cdecl cycles_shader_get_name(ccl::Shader* shader, void* stringholder);
CCL_CAPI void __cdecl cycles_scene_tag_shader(ccl::Session* session_id, unsigned int shader_id, bool use);
CCL_CAPI unsigned int __cdecl cycles_scene_add_shader(ccl::Session* session_id, unsigned int shader_id);
/** Set shader_id as default surface shader for session_id.
 * Note that shader_id is the ID for the shader specific to this scene.
 *
 * The correct ID can be found with cycles_scene_shader_id. The ID is also
 * returned from cycles_scene_add_shader.
 */
CCL_CAPI void __cdecl cycles_scene_set_default_surface_shader(ccl::Session* session_id, unsigned int shader_id);
/**
 * Return the current default surface shader id for session_id.
 */
CCL_CAPI unsigned int __cdecl cycles_scene_get_default_surface_shader(ccl::Session* session_id);
CCL_CAPI unsigned int __cdecl cycles_scene_shader_id(ccl::Session* session_id, unsigned int shader_id);
CCL_CAPI unsigned int __cdecl cycles_add_shader_node(ccl::Session* session_id, unsigned int shader_id, shadernode_type shn_type);
CCL_CAPI void __cdecl cycles_shadernode_set_attribute_int(ccl::ShaderNode* shnode_id, const char* attribute_name, int value);
CCL_CAPI void __cdecl cycles_shadernode_set_attribute_float(ccl::ShaderNode* shnode_id, const char* attribute_name, float value);
CCL_CAPI void __cdecl cycles_shadernode_set_attribute_vec(ccl::ShaderNode* shnode_id, const char* attribute_name, float x, float y, float z);
CCL_CAPI void __cdecl cycles_shadernode_set_enum(ccl::ShaderNode* shnode_id, const char* enum_name, int value);
CCL_CAPI void __cdecl cycles_shadernode_texmapping_set_transformation(ccl::ShaderNode* shnode, int transform_type, float x, float y, float z);
CCL_CAPI void __cdecl cycles_shadernode_texmapping_set_mapping(ccl::ShaderNode* shnode, ccl::TextureMapping::Mapping x, ccl::TextureMapping::Mapping y, ccl::TextureMapping::Mapping z);
CCL_CAPI void __cdecl cycles_shadernode_texmapping_set_projection(ccl::Session* session_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, ccl::TextureMapping::Projection tm_projection);
CCL_CAPI void __cdecl cycles_shadernode_texmapping_set_type(ccl::Session* session_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, ccl::NodeMappingType tm_type);

CCL_CAPI void __cdecl cycles_shadernode_set_member_bool(ccl::ShaderNode* shnode, const char* member_name, bool value);
CCL_CAPI void __cdecl cycles_shadernode_set_member_float(ccl::ShaderNode* shnode_id, const char* member_name, float value);
CCL_CAPI void __cdecl cycles_shadernode_set_member_int(ccl::ShaderNode* shnode, const char* member_name, int value);
CCL_CAPI void __cdecl cycles_shadernode_set_member_vec(ccl::ShaderNode* shnode, const char* member_name, float x, float y, float z);
CCL_CAPI void __cdecl cycles_shadernode_set_member_string(ccl::ShaderNode* shnode_id, const char* member_name, const char* value);
CCL_CAPI void __cdecl cycles_shadernode_set_member_vec4_at_index(ccl::ShaderNode* shnode, const char* member_name, float x, float y, float z, float w, int index);

CCL_CAPI void __cdecl cycles_shadernode_set_member_float_img(ccl::Session* session_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, const char* img_name, float* img, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels);
CCL_CAPI void __cdecl cycles_shadernode_set_member_byte_img(ccl::Session* session_id, unsigned int shader_id, unsigned int shnode_id, shadernode_type shn_type, const char* member_name, const char* img_name, unsigned char* img, unsigned int width, unsigned int height, unsigned int depth, unsigned int channels);

CCL_CAPI void __cdecl cycles_shader_set_name(ccl::Shader* shader, const char* name);
CCL_CAPI void __cdecl cycles_shader_set_use_mis(ccl::Session* session_id, unsigned int shader_id, unsigned int use_mis);
CCL_CAPI void __cdecl cycles_shader_set_use_transparent_shadow(ccl::Session* session_id, unsigned int shader_id, unsigned int use_transparent_shadow);
CCL_CAPI void __cdecl cycles_shader_set_heterogeneous_volume(ccl::Session* session_id, unsigned int shader_id, unsigned int heterogeneous_volume);
CCL_CAPI void __cdecl cycles_shader_new_graph(ccl::Shader* shader);

CCL_CAPI void __cdecl cycles_shader_connect_nodes(ccl::Session* session_id, unsigned int shader_id, unsigned int from_id, const char* from, unsigned int to_id, const char* to);

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

CCL_CAPI unsigned int __cdecl cycles_create_light(ccl::Session* session_id, unsigned int light_shader_id);
CCL_CAPI void __cdecl cycles_light_set_type(ccl::Session* session_id, unsigned int light_id, light_type type);
CCL_CAPI void __cdecl cycles_light_set_angle(ccl::Session* session_id, unsigned int light_id, float angle);
CCL_CAPI void __cdecl cycles_light_set_spot_angle(ccl::Session* session_id, unsigned int light_id, float spot_angle);
CCL_CAPI void __cdecl cycles_light_set_spot_smooth(ccl::Session* session_id, unsigned int light_id, float spot_smooth);
CCL_CAPI void __cdecl cycles_light_set_cast_shadow(ccl::Session* session_id, unsigned int light_id, unsigned int cast_shadow);
CCL_CAPI void __cdecl cycles_light_set_use_mis(ccl::Session* session_id, unsigned int light_id, unsigned int use_mis);
CCL_CAPI void __cdecl cycles_light_set_samples(ccl::Session* session_id, unsigned int light_id, unsigned int samples);
CCL_CAPI void __cdecl cycles_light_set_max_bounces(ccl::Session* session_id, unsigned int light_id, unsigned int max_bounces);
CCL_CAPI void __cdecl cycles_light_set_map_resolution(ccl::Session* session_id, unsigned int light_id, unsigned int map_resolution);
CCL_CAPI void __cdecl cycles_light_set_sizeu(ccl::Session* session_id, unsigned int light_id, float sizeu);
CCL_CAPI void __cdecl cycles_light_set_sizev(ccl::Session* session_id, unsigned int light_id, float sizev);
CCL_CAPI void __cdecl cycles_light_set_axisu(ccl::Session* session_id, unsigned int light_id, float axisux, float axisuy, float axisuz);
CCL_CAPI void __cdecl cycles_light_set_axisv(ccl::Session* session_id, unsigned int light_id, float axisvx, float axisvy, float axisvz);
CCL_CAPI void __cdecl cycles_light_set_size(ccl::Session* session_id, unsigned int light_id, float size);
CCL_CAPI void __cdecl cycles_light_set_dir(ccl::Session* session_id, unsigned int light_id, float dirx, float diry, float dirz);
CCL_CAPI void __cdecl cycles_light_set_co(ccl::Session* session_id, unsigned int light_id, float cox, float coy, float coz);
CCL_CAPI void __cdecl cycles_light_tag_update(ccl::Session* session_id, unsigned int light_id);

CCL_CAPI void __cdecl cycles_film_set_exposure(ccl::Session* session_id, float exposure);
CCL_CAPI void __cdecl cycles_film_set_filter(ccl::Session* session_id, unsigned int filter_type, float filter_width);
CCL_CAPI void __cdecl cycles_film_set_use_sample_clamp(ccl::Session* session_id, bool use_sample_clamp);
CCL_CAPI void __cdecl cycles_film_tag_update(ccl::Session* session_id);

CCL_CAPI void __cdecl cycles_f4_add(ccl::float4 a, ccl::float4 b, ccl::float4& res);
CCL_CAPI void __cdecl cycles_f4_sub(ccl::float4 a, ccl::float4 b, ccl::float4& res);
CCL_CAPI void __cdecl cycles_f4_mul(ccl::float4 a, ccl::float4 b, ccl::float4& res);
CCL_CAPI void __cdecl cycles_f4_div(ccl::float4 a, ccl::float4 b, ccl::float4& res);

CCL_CAPI void __cdecl cycles_tfm_inverse(const ccl::Transform& t, ccl::Transform& res);
CCL_CAPI void __cdecl cycles_tfm_lookat(const ccl::float3& position, const ccl::float3& look, const ccl::float3& up, ccl::Transform& res);
CCL_CAPI void __cdecl cycles_tfm_rotate_around_axis(float angle, const ccl::float3& axis, ccl::Transform& res);

CCL_CAPI void __cdecl cycles_apply_gamma_to_byte_buffer(unsigned char* rgba_buffer, size_t size_in_bytes, float gamma);
CCL_CAPI void __cdecl cycles_apply_gamma_to_float_buffer(float* rgba_buffer, size_t size_in_bytes, float gamma);

CCL_CAPI void __cdecl cycles_set_rhino_perlin_noise_table(int* data, unsigned int count);
CCL_CAPI void __cdecl cycles_set_rhino_impulse_noise_table(float* data, unsigned int count);
CCL_CAPI void __cdecl cycles_set_rhino_vc_noise_table(float* data, unsigned int count);
CCL_CAPI void __cdecl cycles_set_rhino_aaltonen_noise_table(const int* data, unsigned int count);

#ifdef __cplusplus
}
#endif

#endif
