/* SPDX-License-Identifier: Apache-2.0
 * Copyright 2021-2022 Blender Foundation */

#include "session/output_driver.h"

#include "util/function.h"
#include "util/image.h"
#include "util/string.h"
#include "util/unique_ptr.h"
#include "util/vector.h"

CCL_NAMESPACE_BEGIN

#if defined(BLABLA)

class OIIOOutputDriver : public OutputDriver {
 public:
  typedef function<void(const string &)> LogFunction;

  OIIOOutputDriver(const string_view filepath, const string_view pass, LogFunction log);
  virtual ~OIIOOutputDriver();

  void write_render_tile(const Tile &tile) override;

 protected:
  string filepath_;
  string pass_;
  LogFunction log_;
};

#endif

CCL_NAMESPACE_END
