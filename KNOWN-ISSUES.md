Known issues
============

## Open

- **AMD HIP: device creation sometimes hangs** in `hipStreamCreateWithFlags`, in
  roughly 1 of 25-40 session starts (Radeon 890M, driver 32.0.31036.15). RhinoCycles
  waits 20 s, then renders on the CPU for the rest of the Rhino session and says so
  on the command line and in the Raytraced HUD.
- **NVIDIA CUDA and OptiX: kernels are built but not render-tested** on this branch.
- **Intel oneAPI: built and shipped, not render-tested.** The 4.x line had switched it
  off over a crash on exit (RH-91240) that was never investigated.
- **Built without OSL and without OIDN**, on both platforms. Rhino renders with SVM
  and uses the RDK's own denoisers, which still work. OSL-only features are
  unavailable.
- **Clipping planes clip camera rays only, for every object.** Rhino 9's per-object
  clipping-plane participation (RH-98012) and clipping of all rays under the Product
  preset (RH-95655) are not ported: missing from the kernel, ccycles, csycles and
  RhinoCycles. RH-98414.
- **macOS: Apple Silicon only.** Blender publishes no Intel macOS libraries for
  Cycles 5.

## Expected look changes from Cycles 3.5

Accepted, not bugs.

- **Transmission tint** is applied as in Cycles 5 (`sqrt` of the colour at each
  surface), so tinted glass is lighter than in 3.5.
- **Opaque surfaces are a little brighter at grazing angles**: the Principled BSDF is
  layered since Cycles 4.
- **The Principled default distribution is Multiscatter GGX**, as in Blender; rough
  glass is brighter than 3.5's single-scatter GGX.
- **Some clear glass with thin geometry looks milkier.** Matches Blender 5.2 exactly.
- **Custom (non-PBR) Rhino materials no longer get a derived sheen.**
- **PBR subsurface colour and opacity roughness** have no sockets of their own since
  Cycles 4. The subsurface colour is mixed into the base colour, weighted by the
  non-transmissive share; opacity roughness is blended into the roughness by the
  transmission amount.
- **Cycles Glass material refracts at its IOR setting.** Rhino 9 passed the Frost
  value as IOR (0 at the default Frost), so existing Cycles Glass renders differently.
