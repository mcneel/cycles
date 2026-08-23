# Rhino + Cycles 5.2 — current state

Rhino 10's Cycles integration, moved from 3.5 to 5.2. **It renders correctly, on
CPU and on HIP, and the full Rhino solution builds clean.** One scene still
differs from shipping by 4.2%; a second, independent scene matches shipping
exactly. Nothing here is blocked.

Written 2026-08-22. Supersedes nothing. The older harvester notes still describe
the csycles generator accurately, but live only in history — `git show
596472b8b:tools/wrappergen/README.md` — and assume a layout that no longer
exists (see *Plan*).

## Where the code is

Paths are relative to the Rhino checkout root.

| What | Where |
| --- | --- |
| Cycles itself, plus the Rhino port | `src4/rhino4/Plug-ins/RDK/cycles` (own repo, branch `lars/10.x/cycles-5.2`) |
| `ccycles` — the C API over Cycles | `.../cycles/src/ccycles` |
| `csycles` — the C# P/Invoke layer | `.../cycles/src/csycles` |
| `RhinoCycles` — the Rhino plug-in | `src4/rhino4/Plug-ins/RDK/RhinoCycles` (branch `lars/10.x/cycles-modernization`) |
| Prebuilt Cycles payload | `big_libs/RhinoCycles/ccycles/win/{debug,release}` |
| Solutions | `src4/BuildSolutions/Rhino.sln` |

`csycles` used to be a sibling repository (`CCSycles`); it now lives inside the
cycles repo. Anything referring to a separate checkout is out of date.

## Building

Build the **solution**, not single projects:

    MSBuild src4/BuildSolutions/Rhino.sln /p:Configuration=Debug /p:Platform=x64 /m:4

That is all most people need — Cycles comes prebuilt from `big_libs`, so no CMake,
CUDA or OptiX SDK is required. To build Cycles *from source* instead, set
`RHINOCYCLESDEV=1` first; GPU backends are auto-detected and a machine with no GPU
SDK still gets a working CPU build.

Two traps, both of which have cost real time:

- **Single-project builds leave the tree inconsistent, both ways.**
  `ccycles.vcxproj` alone updates `big_libs` but not the plug-in output;
  `RhinoCyclesCore.csproj` alone copies `big_libs` without rebuilding Cycles. A
  Cycles edit needs both, in that order — or just the solution.
- **GPU rendering needs `RhinoCyclesKernelCompiler.exe`**, which only a solution
  build produces. Without it every render silently falls back to CPU or stalls,
  and nothing says so.

After any partial build, check the binary is newer than your edit before
believing a render. A build that fails on a file lock leaves the old binary in
place and the render looks perfectly normal.

## Checking it works

    powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1            # static audits, ~1s
    powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1 -Render    # + golden images, minutes

Non-zero exit on failure, so it can gate a build. The audits catch the three ways
this port drifts silently: a renamed or retyped socket, a renumbered enum, and a
stock SVM node emitted in Rhino's packed layout. All three currently pass.

## Where we are

**Working:** renders match shipping on `Material_Scene_Final.3dm` to within the
noise floor (0.013 per channel against a 0.036 floor). CPU and HIP agree (0.036).
Full solution build is 0 warnings, 0 errors. All static audits pass.

**Open:**

| Issue | Status |
| --- | --- |
| `rdk_material_scene.3dm` renders 4.2% darker than shipping | One scene only. Device, clamping, colorspace, materials, light tree, shadow-catcher visibility and adaptive sampling all eliminated by measurement. Mechanism unknown. |
| `smoketest` never produces pixels | Hangs inside session start at "Updating Shaders". Harness only; Rhino itself renders. |
| Python scripting in the Debug build | Unusable — MCP `run_python` and `-_RunPythonScript` both time out. Blocks authoring test scenes. |

## Hacks and workarounds

Deliberate, and worth knowing before touching the surrounding code.

- **`add_node_packed`** keeps the pre-5.2 packed SVM encoding for the
  `RHINO_NODE_*` types, whose kernel readers were left on the old layout. Wrong
  for any stock node — emitting one that way turned every render black once.
  Guarded by `tools/audit_svm_nodes.py` (102 call sites checked).
- **Two Principled inputs are silently dropped.** Blender 4.0 removed
  `Subsurface Color` and `Transmission Roughness`; csycles keeps both marked
  `Retired`, which no-ops connections and values. Rhino's PBR subsurface colour
  and opacity roughness therefore have no effect — a deliberate decision matching
  what the 4.4 attempt effectively did. Those two UI controls should eventually be
  marked or hidden.
- **`musgrave_texture` is gone upstream**; its enum setters are accepted and
  ignored. `velvet_bsdf` and `anisotropic_bsdf` are likewise unregistered. None of
  the three has a RhinoCycles caller, so the fallback is unreachable.
- **ccycles' builtin-image paths are dead code** — `get_ccimage` and
  `builtin_image_info` are `assert(false)` behind `#if LEGACY_IMAGES`. Images
  reach Cycles as files. Anything calling them would assert.
- **A duplicate-device assert in `device.h` is now a warning**, naming both
  colliding devices instead of killing the process.
- **A light-basis guard** ignores a zero-length direction; Rhino's background
  light arrives with a NaN transform.
- **12 `CCYCLES_*` environment switches** are compiled in and off unless set —
  scene dumps, pass probes, background-graph taps, clamp and colorspace
  overrides. See `tools/DIAGNOSTICS.md`.

## Plan

1. **Mark or hide the two dead PBR controls** (subsurface colour, opacity
   roughness) so users are not adjusting inputs with no effect.
2. **Revive `tools/wrappergen`** for the next Cycles bump — it generates the
   csycles binding, which is exactly the layer that drifts. Budget for its
   per-file review pass: the generator overwrites hand-made improvements, and its
   README assumes the old sibling checkout. The audits only *detect* this drift;
   generating the binding prevents it.
3. **Decide the 4.2% scene.** A 3-point bisect through the 4.4 branches would
   name the upstream change; it needs an LFS fetch and a build of the 4.4
   RhinoCycles branch. Otherwise document and park it.
4. **Fix the smoketest**, or drop it in favour of `run_checks.ps1 -Render`, which
   does work.

## Going deeper

- `tools/DIAGNOSTICS.md` — every diagnostic switch, what each established, and
  what has been eliminated and how
- `tools/PORTING-GAPS.md` — the things needing a decision rather than a fix
- `tools/PORT-HISTORY.md` — the earlier 4.x attempts, what they hold, and what
  reusing them saves
