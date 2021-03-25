/*
 * Copyright 2011-2013 Blender Foundation
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

CCL_NAMESPACE_BEGIN

ccl_device float4 film_get_pass_result(KernelGlobals *kg,
                                       ccl_global float *buffer,
                                       float sample_scale,
                                       int index,
                                       bool use_display_sample_scale,
                                       int display_pass_type)
{
  float4 pass_result;

  int display_pass_stride = kernel_data.film.display_pass_stride;
  int display_pass_components = kernel_data.film.display_pass_components;
  int pass_flag = (1 << (display_pass_type % 32));
  if (kernel_data.film.pass_flag & pass_flag) {
    switch (display_pass_type) {
      case PASS_COMBINED:
        display_pass_components = 4;
        display_pass_stride = kernel_data.film.pass_combined;
        break;
      case PASS_NORMAL:
        display_pass_components = 4;
        display_pass_stride = kernel_data.film.pass_normal;
        break;
      case PASS_DEPTH:
        display_pass_components = 1;
        display_pass_stride = kernel_data.film.pass_depth;
        break;
    }
  }

  if (display_pass_type > PASS_CATEGORY_MAIN_END) {
    if (kernel_data.film.light_pass_flag & pass_flag) {
      switch (display_pass_type) {
        case PASS_DIFFUSE_COLOR:
          display_pass_components = 4;
          display_pass_stride = kernel_data.film.pass_diffuse_color;
          break;
      }
    }
  }

  /*if (display_pass_type == PASS_COMBINED || display_pass_type == PASS_NORMAL || display_pass_type == PASS_DIFFUSE_COLOR) {
    display_pass_components = 4;
  }
  else if (display_pass_type == PASS_DEPTH) {
    display_pass_components = 1;
  }*/

  if (display_pass_components == 4) {
    ccl_global float4 *in = (ccl_global float4 *)(buffer + display_pass_stride +
                                                  index * kernel_data.film.pass_stride);
    float alpha = use_display_sample_scale ?
                      (kernel_data.film.use_display_pass_alpha ? in->w : 1.0f / sample_scale) :
                      1.0f;

    pass_result = make_float4(in->x, in->y, in->z, alpha);

    int display_divide_pass_stride = kernel_data.film.display_divide_pass_stride;

    if (display_pass_type == PASS_DIFFUSE_COLOR) {
      display_divide_pass_stride = display_pass_stride;
      alpha = 1.0f;
    }

    if (display_divide_pass_stride != -1) {
      ccl_global float4 *divide_in = (ccl_global float4 *)(buffer + display_divide_pass_stride +
                                                           index * kernel_data.film.pass_stride);
      if (divide_in->x != 0.0f) {
        pass_result.x /= divide_in->x;
      }
      if (divide_in->y != 0.0f) {
        pass_result.y /= divide_in->y;
      }
      if (divide_in->z != 0.0f) {
        pass_result.z /= divide_in->z;
      }
    }

    if (kernel_data.film.use_display_exposure) {
      float exposure = kernel_data.film.exposure;
      pass_result *= make_float4(exposure, exposure, exposure, alpha);
    }
  }
  else if (display_pass_components == 1) {
    ccl_global float *in = (ccl_global float *)(buffer + display_pass_stride +
                                                index * kernel_data.film.pass_stride);
    pass_result = make_float4(*in, *in, *in, 1.0f / sample_scale);
  }

  return pass_result;
}

ccl_device float4 film_map(KernelGlobals *kg, float4 rgba_in, float scale)
{
  float4 result;

  /* conversion to srgb */
  result.x = color_linear_to_srgb(rgba_in.x * scale);
  result.y = color_linear_to_srgb(rgba_in.y * scale);
  result.z = color_linear_to_srgb(rgba_in.z * scale);

  /* clamp since alpha might be > 1.0 due to russian roulette */
  result.w = saturate(rgba_in.w * scale);

  return result;
}

#if 0
ccl_device uchar4 film_float_to_byte(float4 color)
{
  uchar4 result;

  /* simple float to byte conversion */
  result.x = (uchar)(saturate(color.x) * 255.0f);
  result.y = (uchar)(saturate(color.y) * 255.0f);
  result.z = (uchar)(saturate(color.z) * 255.0f);
  result.w = (uchar)(saturate(color.w) * 255.0f);

  return result;
}

ccl_device void kernel_film_convert_to_byte(KernelGlobals *kg,
                                            ccl_global uchar4 *rgba,
                                            ccl_global float *buffer,
                                            float sample_scale,
                                            int x,
                                            int y,
                                            int height,
                                            int offset,
                                            int stride)
{
  /* buffer offset */
  int index = offset + x + y * stride;
  int out_index = offset + (height - (y + 1)) * stride + x;

  bool use_display_sample_scale = (kernel_data.film.display_divide_pass_stride == -1);
  float4 rgba_in = film_get_pass_result(kg, buffer, sample_scale, index, use_display_sample_scale);

  /* map colors */
  float4 float_result = film_map(kg, rgba_in, use_display_sample_scale ? sample_scale : 1.0f);
  uchar4 uchar_result = film_float_to_byte(float_result);

  rgba += out_index;
  *rgba = uchar_result;
}
#endif

ccl_device void kernel_film_convert_to_float(KernelGlobals *kg,
                                             ccl_global float *rgba,
                                             ccl_global float *buffer,
                                             float sample_scale,
                                             int pass_type,
                                             int x,
                                             int y,
                                             int height,
                                             int offset,
                                             int stride,
                                             int full_width,
                                             int full_height,
                                             int pixel_size)
{
  /* buffer offset */
  int input_index = offset + x + y * stride;
  int out_index = offset + x + (height - (y + 1)) * stride;

  /* ADD HERE EXPANDING of pixel into target buffer. Take into account
   * target buffer width and height. Ensure we don't write beyond the
   * width and height of the target buffer to keep result correct
   *
   * NEEDED:
   * full width (full_stride)
   * full height
   * pixel size (resolution)
   *
   */
  /* offset into target buffer */
  int expand_x = 1;
  int expand_y = 1;
  if (pixel_size > 1) {
    int new_y = y * pixel_size;
    out_index = offset + x * pixel_size + (full_height - (new_y + pixel_size)) * full_width;
    expand_x = pixel_size;
    expand_y = pixel_size;
  }

  bool use_display_sample_scale = (kernel_data.film.display_divide_pass_stride == -1);
  float4 rgba_in = film_get_pass_result(kg, buffer, sample_scale, input_index, use_display_sample_scale, pass_type);
  float scale = use_display_sample_scale ? sample_scale : 1.0f;
  if (pass_type == PASS_COMBINED) {
    ccl_global float *out = (ccl_global float *)rgba + out_index * 4;
    for (int write_x = 0; write_x < expand_x; write_x++) {
      for (int write_y = 0; write_y < expand_y; write_y++) {
        float *final_out = out + write_x * 4 + write_y * full_width * 4;
        float4_store_float4(final_out, rgba_in, scale);
      }
    }
  }
  else if (pass_type == PASS_NORMAL || pass_type == PASS_DIFFUSE_COLOR) {
    if(pass_type == PASS_NORMAL) {
      scale = 1.0f;
	}
    ccl_global float *out = (ccl_global float *)rgba + out_index * 3;
    for (int write_x = 0; write_x < expand_x; write_x++) {
      for (int write_y = 0; write_y < expand_y; write_y++) {
        float* final_out = out + write_x * 3 + write_y * full_width * 3;
		float4_store_float3(final_out, rgba_in, scale);
      }
    }
  }
  else if (pass_type == PASS_DEPTH) {
    ccl_global float *out = (ccl_global float *)rgba + out_index;
    for (int write_x = 0; write_x < expand_x; write_x++) {
      for (int write_y = 0; write_y < expand_y; write_y++) {
        float* final_out = out + write_x + write_y * full_width;
		*final_out = rgba_in.x;
      }
    }
  }
}

CCL_NAMESPACE_END
