#pragma once

/* TEMPORARY, part of the SimpleVaseTest background chase. Not for merging.
 *
 * Tapping the background shader says what it computes, but not who invoked it or what the
 * result gets scaled by. Reading every light_path output showed that a background pixel in
 * shipping carries about two thirds of the weight it does in the 5.2 branch, of which only
 * half is camera-flagged - and no tap can see where that comes from.
 *
 * CCYCLES_BG_EVAL_TALLY=<file> counts every background shader evaluation by call site and
 * path flag, with the mean bounce depth, the mean MIS weight it is scaled by, and the mean
 * path throughput - film_write_background multiplies the shader result by throughput, so a
 * throughput below one shows up as a uniform darkening of every background pixel.
 *
 * Header-only with function-local statics, so one set of counters per translation unit,
 * and shade_background.h is compiled once per CPU kernel arch variant. That is harmless:
 * Cycles picks a single arch at runtime, so only the active variant ever increments.
 *
 * Note the guard at the call sites is #ifndef __KERNEL_GPU__. There is no __KERNEL_CPU__
 * in this tree - using it silently compiled the whole thing out.
 */

#include <cstdint>
#include <cstdio>
#include <cstdlib>
#include <map>
#include <mutex>
#include <string>

namespace rhino_bg_tally {

struct Entry {
  uint64_t calls = 0;
  double bounce_sum = 0.0;
  double transparent_sum = 0.0;
  double weight_sum = 0.0;
  double throughput_sum = 0.0;
  double value_sum = 0.0;
  double alpha_sum = 0.0;
};

inline void record(const char *site,
                   uint32_t flag,
                   int bounce,
                   int transparent_bounce,
                   float mis_weight,
                   float throughput,
                   float value = 0.0f,
                   float alpha = 0.0f)
{
  static const char *path = getenv("CCYCLES_BG_EVAL_TALLY");
  if (path == nullptr || path[0] == 0) {
    return;
  }

  static std::mutex mtx;
  static std::map<std::string, Entry> table;
  static uint64_t total = 0;

  char key[96];
  snprintf(key, sizeof(key), "%-10s 0x%08x", site, flag);

  std::lock_guard<std::mutex> lock(mtx);
  Entry &e = table[key];
  e.calls++;
  e.bounce_sum += bounce;
  e.transparent_sum += transparent_bounce;
  e.weight_sum += mis_weight;
  e.throughput_sum += throughput;
  e.value_sum += value;
  e.alpha_sum += alpha;
  ++total;

  /* The harness kills Rhino rather than closing it, so static destructors never run and an
   * atexit dump would be lost. Rewrite the whole file periodically instead; the counts are
   * only read as proportions, so a truncated tail does not matter. */
  if ((total % 20000) != 0) {
    return;
  }
  FILE *f = fopen(path, "w");
  if (f == nullptr) {
    return;
  }
  fprintf(f, "total_background_shader_evals %llu\n", (unsigned long long)total);
  for (const auto &kv : table) {
    const Entry &v = kv.second;
    const double n = (double)v.calls;
    fprintf(f,
            "%s calls %9llu share %.4f bounce %.4f transp %.4f mis %.4f throughput %.4f "
            "value %.4f alpha %.4f\n",
            kv.first.c_str(),
            (unsigned long long)v.calls,
            n / (double)total,
            v.bounce_sum / n,
            v.transparent_sum / n,
            v.weight_sum / n,
            v.throughput_sum / n,
            v.value_sum / n,
            v.alpha_sum / n);
  }
  fclose(f);
}


/* The final background pixel is
 *   alpha_over = color_matte * alpha + color_background * (1 - alpha_matte)
 * in film_calculate_shadow_catcher_matte_with_shadow, so what it is worth knowing per
 * pixel is those terms and the three scales feeding them. The discriminating values go in
 * the key so pixels with the same outcome aggregate: the background cluster is then the
 * row whose result is the measured 0.333 (shipping) or 0.997 (5.2 branch).
 *
 * The four numeric slots are reused here as
 *   mis        -> scale
 *   throughput -> scale_exposure
 *   value      -> background_scale_exposure
 *   alpha      -> average(color_background)
 */
inline void record_matte(const char *site,
                         float result,
                         float alpha,
                         float alpha_matte,
                         float scale,
                         float scale_exposure,
                         float background_scale_exposure,
                         float color_background)
{
  char key[64];
  snprintf(key, sizeof(key), "%s res=%.3f a=%.3f am=%.3f", site, result, alpha, alpha_matte);
  record(key, 0u, 0, 0, scale, scale_exposure, background_scale_exposure, color_background);
}

/* The shadow catcher term is combined_no_matte / color_catcher, and that ratio is what
 * differs between the trees - a third against one. Reading it here rather than through
 * CCYCLES_PASS_PROBE is deliberate: that probe looks passes up by name via
 * get_pass_pixels, and every pass except combined and depth has an empty name, so it
 * reports the shadow catcher passes "unavailable" when they are in fact allocated. Here
 * the kernel has already resolved them by type through kfilm_convert->pass_*.
 *
 * Slots: mis -> color_catcher, throughput -> color_combined, value -> color_matte,
 * alpha -> num_samples.
 */
inline void record_sc(
    float result, float num_samples, float catcher, float combined, float matte)
{
  char key[64];
  snprintf(key, sizeof(key), "sc res=%.3f n=%.0f", result, num_samples);
  record(key, 0u, 0, 0, catcher, combined, matte, num_samples);
}
}  // namespace rhino_bg_tally
