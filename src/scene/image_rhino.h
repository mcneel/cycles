/* SPDX-FileCopyrightText: 2026 Robert McNeel and Associates
 *
 * SPDX-License-Identifier: Apache-2.0 */

#pragma once

#include "scene/image_loader.h"

#include "util/string.h"
#include "util/vector.h"

CCL_NAMESPACE_BEGIN

/* Textures Rhino passes as pixels in memory (generated, or embedded in the .3dm); replaces
 * the builtin image callbacks 5.x removed. The pixels are copied: Cycles reads them in
 * device_update, long after the graph is built, and the caller's buffer may be gone. */
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
