/* SPDX-FileCopyrightText: 2011-2022 Blender Foundation
 *
 * SPDX-License-Identifier: Apache-2.0 */

#include "scene/image_rhino.h"

#include "util/image_metadata.h"

CCL_NAMESPACE_BEGIN

RhinoMemoryImageLoader::RhinoMemoryImageLoader(const string &name,
                                               const void *pixels,
                                               const int width,
                                               const int height,
                                               const int channels,
                                               const bool is_float)
    : name_(name), width_(width), height_(height), channels_(channels), is_float_(is_float)
{
  if (pixels == nullptr || width <= 0 || height <= 0 || channels <= 0) {
    /* Leave data_ empty; load_metadata then refuses the image rather than
     * handing the kernel a texture backed by nothing. */
    return;
  }

  const size_t element_size = is_float ? sizeof(float) : sizeof(uint8_t);
  const size_t total = size_t(width) * size_t(height) * size_t(channels) * element_size;
  data_.resize(total);
  memcpy(data_.data(), pixels, total);
}

RhinoMemoryImageLoader::~RhinoMemoryImageLoader() = default;

bool RhinoMemoryImageLoader::load_metadata(ImageMetaData &metadata,
                                           const ImageLoaderParams & /*params*/,
                                           Progress & /*progress*/)
{
  if (data_.empty()) {
    return false;
  }

  metadata.width = width_;
  metadata.height = height_;

  /* The kernel only handles 1 and 4 channel images, so always present 4 and widen
   * in load_pixels if the source had fewer. */
  metadata.channels = 4;
  metadata.type = is_float_ ? IMAGE_DATA_TYPE_FLOAT4 : IMAGE_DATA_TYPE_BYTE4;

  /* Rhino's 8 bit buffers hold sRGB encoded values, the same as the PNG/JPEG files
   * the file based path loads; its float buffers are already linear. Saying so here
   * is what keeps a memory texture matching the identical image loaded from disk. */
  if (!is_float_) {
    metadata.colorspace_file_hint = "sRGB";
  }

  return true;
}

bool RhinoMemoryImageLoader::load_pixels(const ImageMetaData &metadata, void *pixels)
{
  if (data_.empty() || pixels == nullptr) {
    return false;
  }

  const int64_t num_pixels = int64_t(width_) * int64_t(height_);

  /* No vertical flip: the builtin image callbacks this replaces did a straight
   * copy, and Rhino's buffers are in the orientation Cycles expects. */
  if (channels_ == 4) {
    memcpy(pixels, data_.data(), data_.size());
  }
  else if (is_float_) {
    const float *src = reinterpret_cast<const float *>(data_.data());
    float *dst = reinterpret_cast<float *>(pixels);
    for (int64_t i = 0; i < num_pixels; i++) {
      const float r = src[i * channels_ + 0];
      const float g = (channels_ > 1) ? src[i * channels_ + 1] : r;
      const float b = (channels_ > 2) ? src[i * channels_ + 2] : r;
      dst[i * 4 + 0] = r;
      dst[i * 4 + 1] = g;
      dst[i * 4 + 2] = b;
      dst[i * 4 + 3] = 1.0f;
    }
  }
  else {
    const uint8_t *src = data_.data();
    uint8_t *dst = reinterpret_cast<uint8_t *>(pixels);
    for (int64_t i = 0; i < num_pixels; i++) {
      const uint8_t r = src[i * channels_ + 0];
      const uint8_t g = (channels_ > 1) ? src[i * channels_ + 1] : r;
      const uint8_t b = (channels_ > 2) ? src[i * channels_ + 2] : r;
      dst[i * 4 + 0] = r;
      dst[i * 4 + 1] = g;
      dst[i * 4 + 2] = b;
      dst[i * 4 + 3] = 255;
    }
  }

  metadata.conform_pixels(pixels);
  return true;
}

string RhinoMemoryImageLoader::name() const
{
  return name_;
}

bool RhinoMemoryImageLoader::equals(const ImageLoader &other) const
{
  const RhinoMemoryImageLoader &o = (const RhinoMemoryImageLoader &)other;
  /* Rhino reuses a name for a given generated or embedded image, so name plus
   * geometry identifies it without comparing every pixel. */
  return name_ == o.name_ && width_ == o.width_ && height_ == o.height_ &&
         channels_ == o.channels_ && is_float_ == o.is_float_;
}

CCL_NAMESPACE_END
