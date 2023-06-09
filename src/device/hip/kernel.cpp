/* SPDX-FileCopyrightText: 2011-2022 Blender Foundation
 *
 * SPDX-License-Identifier: Apache-2.0 */

#ifdef WITH_HIP

#  include "device/hip/kernel.h"
#  include "device/hip/device_impl.h"

CCL_NAMESPACE_BEGIN

void HIPDeviceKernels::load(HIPDevice *device)
{
  hipModule_t hipModule = device->hipModule;

  int prev_min_blocks = -1;
  int prev_num_threads_per_block = -1;

  for (int i = 0; i < (int)DEVICE_KERNEL_NUM; i++) {
    HIPDeviceKernel &kernel = kernels_[i];

    /* No mega-kernel used for GPU. */
    if (i == DEVICE_KERNEL_INTEGRATOR_MEGAKERNEL) {
      continue;
    }

    const std::string function_name = std::string("kernel_gpu_") +
                                      device_kernel_as_string((DeviceKernel)i);
    hip_device_assert(device,
                      hipModuleGetFunction(&kernel.function, hipModule, function_name.c_str()));

    if (kernel.function) {
      hip_device_assert(device, hipFuncSetCacheConfig(kernel.function, hipFuncCachePreferL1));

	  // 2023-06-09 David E.
	  // I have been getting division-by-zero errors in the function
	  // 'hipModuleOccupancyMaxPotentialBlockSize' when processing
	  // the following two kernels. Prevent crash by setting hopefully
	  // safe values to 'min_blocks' and 'num_threads_per_block'.
      if (i == DEVICE_KERNEL_INTEGRATOR_SORT_BUCKET_PASS ||
          i == DEVICE_KERNEL_INTEGRATOR_SORT_WRITE_PASS) {
        kernel.min_blocks = prev_min_blocks >= 0 ? prev_min_blocks : 56;
        kernel.num_threads_per_block = prev_num_threads_per_block >= 0 ? prev_num_threads_per_block : 1024;
        continue;
      }

      hip_device_assert(
          device,
          hipModuleOccupancyMaxPotentialBlockSize(
              &kernel.min_blocks, &kernel.num_threads_per_block, kernel.function, 0, 0));

      prev_min_blocks = kernel.min_blocks;
      prev_num_threads_per_block = kernel.num_threads_per_block;
    }
    else {
      LOG(ERROR) << "Unable to load kernel " << function_name;
    }
  }

  loaded = true;
}

const HIPDeviceKernel &HIPDeviceKernels::get(DeviceKernel kernel) const
{
  return kernels_[(int)kernel];
}

bool HIPDeviceKernels::available(DeviceKernel kernel) const
{
  return kernels_[(int)kernel].function != nullptr;
}

CCL_NAMESPACE_END

#endif /* WITH_HIP*/
