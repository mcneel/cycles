/* SPDX-FileCopyrightText: 2011-2022 Blender Foundation
 *
 * SPDX-License-Identifier: Apache-2.0 */

#pragma once

#include "util/unique_ptr.h"
#include "util/vector.h"

CCL_NAMESPACE_BEGIN

class Device;
class DeviceInfo;
class Profiler;
class Stats;

bool device_optix_init();

/* Rhino: what device_optix_init found, so the host can say why OptiX offers no devices:
 * 0 started (or never tried), 1 the NVIDIA driver is older than this OptiX SDK supports,
 * 2 OptiX failed to start for another reason. */
int device_optix_init_result();

/* Rhino: the oldest NVIDIA driver branch that runs the OptiX SDK this build uses, e.g. 590
 * for OptiX 9.1. 0 without OptiX, or for an SDK newer than the table in device.cpp. */
int device_optix_minimum_driver();

unique_ptr<Device> device_optix_create(const DeviceInfo &info,
                                       Stats &stats,
                                       Profiler &profiler,
                                       bool headless);

void device_optix_info(const vector<DeviceInfo> &cuda_devices, vector<DeviceInfo> &devices);

CCL_NAMESPACE_END
