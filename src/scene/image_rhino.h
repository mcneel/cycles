/* SPDX-FileCopyrightText: 2011-2022 Blender Foundation
 *
 * SPDX-License-Identifier: Apache-2.0 */

#pragma once

#include "scene/image_loader.h"

#include "util/string.h"
#include "util/vector.h"

CCL_NAMESPACE_BEGIN

/* Rhino hands a lot of its textures over as pixels already in memory rather than
 * as a file on disk: anything it generates itself, and anything embedded in the
 * .3dm. Cycles used to accept those through the builtin image callbacks, which
 * 5.x removed in favour of ImageLoader subclasses. This is that replacement.
 *
 * The pixels are copied. Cycles reads them during device_update, which happens
 * well after the shader graph is built, and the caller's buffer lifetime is not
 * ours to rely on. */
class RhinoMemoryImageLoader : public ImageLoader {
 public:
  RhinoMemoryImageLoader(const string &name,
                         const void *pixels,
                         const int width,
                         const int height,
                         const int channels,
                         const bool is_float);
  ~RhinoMemoryImageLoader() override;

  bool load_metadata(ImageMetaData &metadata,
                     const ImageLoaderParams &params,
                     Progress &progress) override;

  bool load_pixels(const ImageMetaData &metadata, void *pixels) override;

  string name() const override;

  bool equals(const ImageLoader &other) const override;

 private:
  string name_;
  /* Owned copy of the caller's pixels, held as bytes whatever the source type. */
  vector<uint8_t> data_;
  int width_ = 0;
  int height_ = 0;
  int channels_ = 0;
  bool is_float_ = false;
};

CCL_NAMESPACE_END
