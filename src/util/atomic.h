/* SPDX-License-Identifier: Apache-2.0
 * Copyright 2014-2022 Blender Foundation */

#ifndef __UTIL_ATOMIC_H__
#define __UTIL_ATOMIC_H__

#ifndef __KERNEL_GPU__

/* ALB 2026.11.27 RH-100740 Uses compiler intrinsics instead of Blender's GPL atomic_ops.h. */
#  include <cstddef>
#  include <cstdint>
#  include <cstring>

#  if defined(_MSC_VER)
#    include <intrin.h>

static_assert(sizeof(size_t) == sizeof(__int64), "Cycles needs a 64-bit build on Windows.");

inline uint32_t atomic_load_relaxed_uint32(const uint32_t *p)
{
  return *(const volatile uint32_t *)p;
}

inline uint32_t atomic_fetch_and_add_uint32(uint32_t *p, uint32_t x)
{
  return (uint32_t)_InterlockedExchangeAdd((volatile long *)p, (long)x);
}

inline uint32_t atomic_cas_uint32(uint32_t *v, uint32_t old_val, uint32_t new_val)
{
  return (uint32_t)_InterlockedCompareExchange((volatile long *)v, (long)new_val, (long)old_val);
}

inline size_t atomic_fetch_and_add_z(size_t *p, size_t x)
{
  return (size_t)_InterlockedExchangeAdd64((volatile __int64 *)p, (__int64)x);
}

inline size_t atomic_cas_z(size_t *v, size_t old_val, size_t new_val)
{
  return (size_t)_InterlockedCompareExchange64(
      (volatile __int64 *)v, (__int64)new_val, (__int64)old_val);
}

#  else

inline uint32_t atomic_load_relaxed_uint32(const uint32_t *p)
{
  return __atomic_load_n(p, __ATOMIC_RELAXED);
}

inline uint32_t atomic_fetch_and_add_uint32(uint32_t *p, uint32_t x)
{
  return __atomic_fetch_add(p, x, __ATOMIC_SEQ_CST);
}

inline uint32_t atomic_cas_uint32(uint32_t *v, uint32_t old_val, uint32_t new_val)
{
  __atomic_compare_exchange_n(v, &old_val, new_val, false, __ATOMIC_SEQ_CST, __ATOMIC_SEQ_CST);
  return old_val;
}

inline size_t atomic_fetch_and_add_z(size_t *p, size_t x)
{
  return __atomic_fetch_add(p, x, __ATOMIC_SEQ_CST);
}

inline size_t atomic_cas_z(size_t *v, size_t old_val, size_t new_val)
{
  __atomic_compare_exchange_n(v, &old_val, new_val, false, __ATOMIC_SEQ_CST, __ATOMIC_SEQ_CST);
  return old_val;
}

#  endif

inline unsigned int atomic_add_and_fetch_u(unsigned int *p, unsigned int x)
{
  return atomic_fetch_and_add_uint32((uint32_t *)p, x) + x;
}

inline size_t atomic_add_and_fetch_z(size_t *p, size_t x)
{
  return atomic_fetch_and_add_z(p, x) + x;
}

inline size_t atomic_sub_and_fetch_z(size_t *p, size_t x)
{
  return atomic_fetch_and_add_z(p, (size_t)0 - x) - x;
}

/* Returns the value before the update. */
inline size_t atomic_fetch_and_update_max_z(size_t *p, size_t x)
{
  size_t prev = atomic_cas_z(p, 0, 0);
  while (prev < x) {
    const size_t seen = atomic_cas_z(p, prev, x);
    if (seen == prev) {
      return prev;
    }
    prev = seen;
  }
  return prev;
}

/* Returns the value before the swap. */
inline float atomic_cas_float(float *v, float old_val, float new_val)
{
  uint32_t old_bits, new_bits;
  memcpy(&old_bits, &old_val, sizeof(float));
  memcpy(&new_bits, &new_val, sizeof(float));
  const uint32_t prev_bits = atomic_cas_uint32((uint32_t *)v, old_bits, new_bits);
  float prev;
  memcpy(&prev, &prev_bits, sizeof(float));
  return prev;
}

inline float atomic_add_and_fetch_fl(float *p, const float x)
{
  uint32_t *bits = (uint32_t *)p;
  uint32_t old_bits = atomic_load_relaxed_uint32(bits);
  for (;;) {
    float old_val;
    memcpy(&old_val, &old_bits, sizeof(float));
    const float new_val = old_val + x;
    uint32_t new_bits;
    memcpy(&new_bits, &new_val, sizeof(float));
    const uint32_t seen = atomic_cas_uint32(bits, old_bits, new_bits);
    if (seen == old_bits) {
      return new_val;
    }
    old_bits = seen;
  }
}

#  define atomic_add_and_fetch_float(p, x) atomic_add_and_fetch_fl((p), (x))
#  define atomic_compare_and_swap_float(p, old_val, new_val) \
    atomic_cas_float((p), (old_val), (new_val))

#  define atomic_fetch_and_inc_uint32(p) atomic_fetch_and_add_uint32((p), 1)
#  define atomic_fetch_and_dec_uint32(p) atomic_fetch_and_add_uint32((p), -1)

#  define CCL_LOCAL_MEM_FENCE 0
#  define ccl_barrier(flags) ((void)0)

#else /* __KERNEL_GPU__ */

#  if defined(__KERNEL_CUDA__) || defined(__KERNEL_HIP__)

#    define atomic_add_and_fetch_float(p, x) (atomicAdd((float *)(p), (float)(x)) + (float)(x))

#    define atomic_fetch_and_add_uint32(p, x) atomicAdd((unsigned int *)(p), (unsigned int)(x))
#    define atomic_fetch_and_sub_uint32(p, x) atomicSub((unsigned int *)(p), (unsigned int)(x))
#    define atomic_fetch_and_inc_uint32(p) atomic_fetch_and_add_uint32((p), 1)
#    define atomic_fetch_and_dec_uint32(p) atomic_fetch_and_sub_uint32((p), 1)
#    define atomic_fetch_and_or_uint32(p, x) atomicOr((unsigned int *)(p), (unsigned int)(x))

ccl_device_inline float atomic_compare_and_swap_float(volatile float *dest,
                                                      const float old_val,
                                                      const float new_val)
{
  union {
    unsigned int int_value;
    float float_value;
  } new_value, prev_value, result;
  prev_value.float_value = old_val;
  new_value.float_value = new_val;
  result.int_value = atomicCAS((unsigned int *)dest, prev_value.int_value, new_value.int_value);
  return result.float_value;
}

#    define CCL_LOCAL_MEM_FENCE
#    define ccl_barrier(flags) __syncthreads()

#  endif /* __KERNEL_CUDA__ */

#  ifdef __KERNEL_METAL__

// global address space versions
ccl_device_inline float atomic_add_and_fetch_float(volatile ccl_global float *_source,
                                                   const float operand)
{
  volatile ccl_global atomic_int *source = (ccl_global atomic_int *)_source;
  union {
    int int_value;
    float float_value;
  } new_value, prev_value;
  prev_value.int_value = atomic_load_explicit(source, memory_order_relaxed);
  do {
    new_value.float_value = prev_value.float_value + operand;
  } while (!atomic_compare_exchange_weak_explicit(source,
                                                  &prev_value.int_value,
                                                  new_value.int_value,
                                                  memory_order_relaxed,
                                                  memory_order_relaxed));

  return new_value.float_value;
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_add_uint32(device T *p, int x)
{
  return atomic_fetch_add_explicit((device atomic_uint *)p, x, memory_order_relaxed);
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_sub_uint32(device T *p, int x)
{
  return atomic_fetch_sub_explicit((device atomic_uint *)p, x, memory_order_relaxed);
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_inc_uint32(device T *p)
{
  return atomic_fetch_add_explicit((device atomic_uint *)p, 1, memory_order_relaxed);
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_dec_uint32(device T *p)
{
  return atomic_fetch_sub_explicit((device atomic_uint *)p, 1, memory_order_relaxed);
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_or_uint32(device T *p, int x)
{
  return atomic_fetch_or_explicit((device atomic_uint *)p, x, memory_order_relaxed);
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_add_uint32(threadgroup T *p, int x)
{
  return atomic_fetch_add_explicit((threadgroup atomic_uint *)p, x, memory_order_relaxed);
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_sub_uint32(threadgroup T *p, int x)
{
  return atomic_fetch_sub_explicit((threadgroup atomic_uint *)p, x, memory_order_relaxed);
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_inc_uint32(threadgroup T *p)
{
  return atomic_fetch_add_explicit((threadgroup atomic_uint *)p, 1, memory_order_relaxed);
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_dec_uint32(threadgroup T *p)
{
  return atomic_fetch_sub_explicit((threadgroup atomic_uint *)p, 1, memory_order_relaxed);
}

template<class T> ccl_device_inline uint32_t atomic_fetch_and_or_uint32(threadgroup T *p, int x)
{
  return atomic_fetch_or_explicit((threadgroup atomic_uint *)p, x, memory_order_relaxed);
}

ccl_device_inline float atomic_compare_and_swap_float(volatile ccl_global float *dest,
                                                      const float old_val,
                                                      const float new_val)
{
  int prev_value;
  prev_value = __float_as_int(old_val);
  atomic_compare_exchange_weak_explicit((ccl_global atomic_int *)dest,
                                        &prev_value,
                                        __float_as_int(new_val),
                                        memory_order_relaxed,
                                        memory_order_relaxed);
  return __int_as_float(prev_value);
}

#    define atomic_store(p, x) atomic_store_explicit(p, x, memory_order_relaxed)
#    define atomic_fetch(p) atomic_load_explicit(p, memory_order_relaxed)

#    define CCL_LOCAL_MEM_FENCE mem_flags::mem_threadgroup
#    define ccl_barrier(flags) threadgroup_barrier(flags)

#  endif /* __KERNEL_METAL__ */

#  ifdef __KERNEL_ONEAPI__

ccl_device_inline float atomic_add_and_fetch_float(ccl_global float *p, float x)
{
  sycl::atomic_ref<float,
                   sycl::memory_order::relaxed,
                   sycl::memory_scope::device,
                   sycl::access::address_space::ext_intel_global_device_space>
      atomic(*p);
  return atomic.fetch_add(x);
}

ccl_device_inline float atomic_compare_and_swap_float(ccl_global float *source,
                                                      float old_val,
                                                      float new_val)
{
  sycl::atomic_ref<float,
                   sycl::memory_order::relaxed,
                   sycl::memory_scope::device,
                   sycl::access::address_space::ext_intel_global_device_space>
      atomic(*source);
  atomic.compare_exchange_weak(old_val, new_val);
  return old_val;
}

ccl_device_inline unsigned int atomic_fetch_and_add_uint32(ccl_global unsigned int *p,
                                                           unsigned int x)
{
  sycl::atomic_ref<unsigned int,
                   sycl::memory_order::relaxed,
                   sycl::memory_scope::device,
                   sycl::access::address_space::ext_intel_global_device_space>
      atomic(*p);
  return atomic.fetch_add(x);
}

ccl_device_inline int atomic_fetch_and_add_uint32(ccl_global int *p, int x)
{
  sycl::atomic_ref<int,
                   sycl::memory_order::relaxed,
                   sycl::memory_scope::device,
                   sycl::access::address_space::ext_intel_global_device_space>
      atomic(*p);
  return atomic.fetch_add(x);
}

ccl_device_inline unsigned int atomic_fetch_and_sub_uint32(ccl_global unsigned int *p,
                                                           unsigned int x)
{
  sycl::atomic_ref<unsigned int,
                   sycl::memory_order::relaxed,
                   sycl::memory_scope::device,
                   sycl::access::address_space::ext_intel_global_device_space>
      atomic(*p);
  return atomic.fetch_sub(x);
}

ccl_device_inline int atomic_fetch_and_sub_uint32(ccl_global int *p, int x)
{
  sycl::atomic_ref<int,
                   sycl::memory_order::relaxed,
                   sycl::memory_scope::device,
                   sycl::access::address_space::ext_intel_global_device_space>
      atomic(*p);
  return atomic.fetch_sub(x);
}

ccl_device_inline unsigned int atomic_fetch_and_inc_uint32(ccl_global unsigned int *p)
{
  return atomic_fetch_and_add_uint32(p, 1);
}

ccl_device_inline int atomic_fetch_and_inc_uint32(ccl_global int *p)
{
  return atomic_fetch_and_add_uint32(p, 1);
}

ccl_device_inline unsigned int atomic_fetch_and_dec_uint32(ccl_global unsigned int *p)
{
  return atomic_fetch_and_sub_uint32(p, 1);
}

ccl_device_inline int atomic_fetch_and_dec_uint32(ccl_global int *p)
{
  return atomic_fetch_and_sub_uint32(p, 1);
}

ccl_device_inline unsigned int atomic_fetch_and_or_uint32(ccl_global unsigned int *p,
                                                          unsigned int x)
{
  sycl::atomic_ref<unsigned int,
                   sycl::memory_order::relaxed,
                   sycl::memory_scope::device,
                   sycl::access::address_space::ext_intel_global_device_space>
      atomic(*p);
  return atomic.fetch_or(x);
}

ccl_device_inline int atomic_fetch_and_or_uint32(ccl_global int *p, int x)
{
  sycl::atomic_ref<int,
                   sycl::memory_order::relaxed,
                   sycl::memory_scope::device,
                   sycl::access::address_space::ext_intel_global_device_space>
      atomic(*p);
  return atomic.fetch_or(x);
}

#  endif /* __KERNEL_ONEAPI__ */

#endif /* __KERNEL_GPU__ */

#endif /* __UTIL_ATOMIC_H__ */
