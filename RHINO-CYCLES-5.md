# Rhino + Cycles 5.2 — current state

Rhino 10's Cycles integration, moved from 3.5 to 5.2. **It renders correctly, on
CPU and on HIP, and the full Rhino solution builds clean.** One scene still
differs from shipping by 4.2%; a second, independent scene matches shipping
exactly. Nothing here is blocked.

Updated 2026-08-25: **the two regression scenes were not representative, and the
claim above was too confident.** A real model with decals and a rectangular area
light (`tyreel_neon_testv9.3dm`) rendered badly wrong, and held two independent
regressions, both of the same shape — a Rhino edit inside an upstream file that
did not survive the bump:

1. `ImageTextureNode` lost Rhino's decal masking and "Mirrored" tiling. Fixed;
   `tools/audit_rhino_stock_sockets.py` now guards it.
2. Lights were visible to camera. 5.x removed `set_use_camera(false)` and nothing
   replaced it, so a rectangular light at the studio default strength of 17.2 sat
   in front of the lens and blew out 84% of the frame. Fixed in
   `src/ccycles/light.cpp` by setting the light Object's visibility.

Both were already answered on the 4.4 branch, which is why *Plan* item 1 is now
"start the next bump from 4.4". A third issue of the same family is still open —
see *Open*. See also *Hacks and workarounds*.

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

That is all most people need. Cycles comes prebuilt from `big_libs`, so no
CMake, CUDA or OptiX SDK is required.

If you only want to get building, [QUICKSTART.md](QUICKSTART.md) is one page and
says nothing about why. The rest of this section is the same ground with the
reasoning attached.

### Setting up to build Cycles

    bootstrap.exe /cycles

Once. It installs the GPU SDKs on top of what a normal bootstrap already does:
the CUDA toolkit via winget, and the OptiX headers from NVIDIA's public
repository - which is all the build needs, so no developer login is involved.
ROCm is the exception and stays manual, because AMD publishes a download page
rather than a stable file URL and its installer cannot install the SDK without
the driver; bootstrap offers to open the page. You only need ROCm to build AMD
kernels, which means publishing a payload, or testing on an AMD card.

Nothing here is needed for a normal Rhino build, and `/cycles` is opt-in, so
nobody who is not working on Cycles is affected.

**Overrides, which nobody needs to set.** Three environment variables change what
`/cycles` installs. All have working defaults, so they exist for the unusual case
rather than the normal one - which is why `bootstrap /help` does not list them.

| Variable | Default |
| --- | --- |
| `RHINO_CUDA_VERSION` | `12.9` |
| `RHINO_OPTIX_REPO` | `https://github.com/NVIDIA/optix-dev.git` |
| `RHINO_ROCM_INSTALLER` | unset - offers AMD's download page |

The CUDA version is pinned rather than latest on purpose: CUDA 13 dropped Maxwell,
Pascal and Volta, which are four of the nine cubins Rhino ships, and 12.9 is what
the MSVC toolset pinning in `build_cycles.ps1` is validated against.

`RHINO_ROCM_INSTALLER` is the only one with a plausible use: point it at a copy of
AMD's HIP SDK installer on a share and the ROCm step launches that instead of
opening the download page. It still runs the installer's own UI, because its
command line cannot install the SDK without the driver, and that is not a choice
to make silently on someone else's machine.

Set them machine- or user-level - `setx NAME value /M` - not in a shell.
bootstrap relaunches itself elevated, and the elevated process does not inherit a
session-only variable.

### Building Cycles itself

Pick a different configuration. That is the whole mechanism.

| Configuration | Cycles |
| --- | --- |
| `Debug`, `Release`, `ReleaseDebuggable` | prebuilt payload, as in Rhino 9.x |
| `Debug+Cycles`, `ReleaseDebuggable+Cycles` | built from source, kernels for the GPUs in your machine |

Visual Studio lists them in the configuration dropdown; RhinoBuilder lists them
in Configurations. Same choice, both tools, nothing to set up and nothing to
remember. The plain configurations are untouched, so a developer who never edits
Cycles cannot trip it.

**Kernels are built for your own GPUs, not for every architecture Cycles
supports.** Rebuilding kernels is how a kernel change gets tested, and a kernel
for a card you do not own cannot be tested - so a machine with one AMD GPU
builds one fatbin rather than twenty-two. Everything else is filled in from the
committed payload, and the build says so. Two consequences worth knowing:

- Those inherited kernels are as old as the last publish, so they do not contain
  your change. If a GPU in your machine is one whose SDK you have not installed,
  the build warns loudly, because then a kernel edit will not show up in your
  own renders however often you rebuild.
- A narrow build does not overwrite the committed payload. It writes
  `big_libs/RhinoCycles/ccycles/win/local`, which is gitignored and which
  `RhinoCyclesCore` prefers while it is newer than the committed one - so your
  Rhino runs what you built, a pull that republishes the payload takes over
  again by itself, and nothing has to be remembered or deleted.

### Publishing a payload

    powershell -File publish_payload.ps1

This is what everyone else runs, so it is the step that matters when a kernel
change is going out. It builds every backend for every shipping architecture,
checks the result file by file against the lists in `kernel_arches.ps1`, prunes
kernels we no longer ship, writes `ccycles_payload.json`, and stages the payload
in `big_libs` - it does not commit, because the message should name the kernel
change that made a republish necessary.

It refuses to stage an incomplete payload, and it requires all four SDKs: a
machine missing one stops with a message naming what to install rather than
quietly shipping a payload without that backend.

Kernel code changed means a republish is needed. `ccycles_payload.json` records
a hash of the kernel sources - `src/kernel` and `src/util`, because Cycles' own
dependency tracking covers only the first - so a payload can be compared against
the tree it should have been built from. `tools/run_checks.ps1` makes that
comparison and fails when they differ.

**`DockerfileHIP`, `DockerfileOneAPI`, `make_hip.sh` and `make_oneapi.sh` are
unwired on purpose.** They build kernels in a Linux container, which was how the
retired pipeline produced the HIP fatbins, and nothing calls them now: ROCm
installs on Windows and `hipcc` cross-compiles, so a publisher needs no AMD
hardware and no container. They are kept for one case that has never been tested
- whether AMD's Windows installer will install the SDK on a machine with no AMD
GPU at all. If it refuses, that is the way round it. Note both are stale: they
copy `*.fatbin` where this tree produces `*.fatbin.zst`, and their architecture
lists have drifted from `kernel_arches.ps1`.

Why the configuration choice is explicit rather than automatic: `ccycles.vcxproj`
drives CMake and so declares no source files, leaving MSBuild nothing to compare
- and git rewrites mtimes on checkout, so timestamps would make a fresh clone a
coin flip. An explicit configuration is the honest way to say it.

### From Visual Studio

Nothing differs from RhinoBuilder. Both run the same MSBuild against the same
solution, and the decision about Cycles is made inside `ccycles.vcxproj` either
way. There is nothing to set in the IDE, and no reason to leave it.

Two things are still worth knowing:

- **`ccycles` is in the solution** — under `Cycles/CCSycles` next to `csycles`,
  and built in every x64 configuration. MSBuild's Makefile `Build` target has no
  inputs or outputs, so its command runs on every build; the stamp comparison,
  not an up-to-date check, is what decides whether it does any work.
- **Native debugging needs the debug payload.** `RhinoCyclesCore` copies the
  `debug` payload only when that folder exists in `big_libs`, and only `release`
  is committed — the debug one is 444 MB and gitignored. So on a fresh checkout a
  Debug Rhino runs *release* Cycles and stepping into `ccycles` gets you nothing.
  One `Debug+Cycles` build produces one.

Two traps, both of which have cost real time:

- **Single-project builds from the command line still leave the tree
  inconsistent.** `ccycles.vcxproj` alone updates `big_libs` but not the plug-in
  output; `RhinoCyclesCore.csproj` alone copies `big_libs` without rebuilding
  Cycles. Inside the solution this is handled — `RhinoCyclesCore` now depends on
  `ccycles`, so Build in Visual Studio does both in the right order — but a
  solution dependency does not exist outside the solution, so `MSBuild
  RhinoCyclesCore.csproj` on its own is still the old trap.
- **Two separate things compile GPU kernels. Do not confuse them.**

  *At build time*, and only when a source build is triggered, `build_cycles.ps1`
  compiles every architecture's kernel. This is the expensive part of a source
  build — 18 HIP fatbins at about 3m20s each, plus CUDA and OptiX — and it is
  most of what a source build actually costs you. Incremental passes only redo
  the kernels whose sources changed. Otherwise nothing is
  compiled: the kernels arrive precompiled in the payload, as
  `big_libs/RhinoCycles/ccycles/win/<cfg>/lib/kernel_*.fatbin.zst` and
  `kernel_*.ptx.zst`. That is what every build gets when Cycles has not changed.

  *At runtime*, `RhinoCyclesKernelCompiler.exe` compiles for the device actually
  present on the machine. Rhino launches it as a child process on a background
  thread at plug-in load, under the `StartGpuKernelCompiler` setting (default
  true). It ships with Rhino and has nothing to do with source builds — but
  only a solution build puts it in the plug-in output, and without it GPU
  rendering silently falls back to CPU or stalls, and nothing says so.

After any partial build, check the binary is newer than your edit before
believing a render. A build that fails on a file lock leaves the old binary in
place and the render looks perfectly normal.

## Checking it works

    powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1            # static audits, ~1s
    powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1 -Render    # + golden images, minutes

Non-zero exit on failure, so it can gate a build. The audits catch the four ways
this port drifts silently: a renamed or retyped socket, a renumbered enum, a
stock SVM node emitted in Rhino's packed layout, and a Rhino extension to a stock
node losing its wiring. All four currently pass.

The fourth was added after the first three all passed on a tree whose decals were
broken — worth remembering when the audits are green and the pixels are not.

### Which Cycles is loaded

`RhinoCycles_ListDevices` prints the version and the path of the `ccycles.dll`
actually in use, which is how you catch a stale prebuilt payload sitting next to
Rhino — the file is otherwise indistinguishable from a fresh one.

**It reports `Cycles 5.3.0`, and that is correct for a 5.2 tree.** Upstream bumps
the version number straight after tagging, so `src/util/version.h` already reads
5.3.0 *at the `v5.2.0` tag itself*. This tree is that tag —
`88896d0e0 Merge upstream Cycles v5.2.0 into the Rhino fork` is the only upstream
merge in the history, and everything after it is Rhino port work. So "5.2"
throughout these documents and "5.3.0" in the readout are both right; they answer
different questions. Do not "fix" either to match the other.

## Where we are

**Working:** renders match shipping on `Material_Scene_Final.3dm` to within the
noise floor (0.013 per channel against a 0.036 floor). CPU and HIP agree (0.036).
Full solution build is 0 warnings, 0 errors. All static audits pass.

**Open:**

| Issue | Status |
| --- | --- |
| Shadow catchers show no shadow | Rhino's ground plane catches shadows but renders as plain background: measured on `tyreel_neon_testv9.3dm` at a flat 255 across the ground band where shipping has a 245-252 gradient, 2,861 soft-grey pixels against shipping's 89,541. `CCYCLES_NO_SHADOW_CATCHER=1` makes the plane an ordinary surface and the shadows appear, so lighting and shadowing are fine — it is the compositing. `get_pass_pixels` redirects a `combined` request to `PASS_SHADOW_CATCHER_MATTE` via `BufferParams::get_actual_display_pass`, and returns the raw combined pass **silently** if that matte is missing. Not yet confirmed which. |
| `rdk_material_scene.3dm` renders 4.2% darker than shipping | One scene only. Device, clamping, colorspace, materials, light tree, shadow-catcher visibility and adaptive sampling all eliminated by measurement. Mechanism unknown. |
| `smoketest` never produces pixels | Hangs inside session start at "Updating Shaders". Harness only; Rhino itself renders. |
| Python scripting in the Debug build | Unusable — MCP `run_python` and `-_RunPythonScript` both time out. Blocks authoring test scenes. |

## Hacks and workarounds

Deliberate, and worth knowing before touching the surrounding code.

- **Rhino's lights are not photographed, and 5.x has no flag for it.** Before 5.2
  this was `set_use_camera(false)` on the light. The per-light flags are gone;
  lights are `Geometry` with an `Object`, so it is the Object's ray visibility
  now, set in `CCyclesLight::flush()` to `PATH_RAY_VISIBILITY_ALL & ~CAMERA`.
  `BackgroundLight` is exempt — camera rays that escape the scene have to reach
  it or the environment goes black. Leaving it at the default is not subtle: a
  Rhino rectangular light at the studio default strength of 17.2 blows out
  everything behind it and reads as missing geometry, not as over-exposure.
- **Rhino extends stock nodes, not only its own.** `ImageTextureNode` carries an
  `alternate_tiles` flag ("Mirrored" repeat, which folds the coordinate on
  alternate tiles instead of wrapping it) and a `DecalUsage` input that masks a
  decal to its footprint. Before 5.2 both went out as a second packed SVM word.
  They are fields on `SVMNodeTexImage` now. Taking such a node from upstream
  deletes the behaviour **silently** — the sockets stay declared, the
  `alternate_tile()` helper stays in the kernel, compiler and kernel still agree
  with each other, and only the pixels are wrong. That is what broke decals on
  the 5.2 port. `tools/audit_rhino_stock_sockets.py` checks the wiring rather
  than the declarations, because declarations were never what broke.
  `DecalForward` is in that table too, marked deliberately unused: the output
  gets a stack slot but `decal_data_read` has never written it, in 3.5 or 5.2.
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

1. **Start the next bump from 4.4, not from 3.5.** Nathan's 4.x work was reverted
   because he retired, not because it was wrong, and it is the only record of what
   Rhino's edits inside upstream files look like after the 4.x upheaval. Both
   regressions found on 2026-08-25 — the image node's decal masking and the
   lights being visible to camera — were already answered on that branch, and
   both got through here because this port diffed only against 3.5, which cannot
   tell "upstream changed this" from "we dropped this". Until that rebase
   happens, diff against **both** baselines. See `tools/PORT-HISTORY.md`.
2. **Mark or hide the two dead PBR controls** (subsurface colour, opacity
   roughness) so users are not adjusting inputs with no effect.
3. **Revive `tools/wrappergen`** for the next Cycles bump — it generates the
   csycles binding, which is exactly the layer that drifts. Budget for its
   per-file review pass: the generator overwrites hand-made improvements, and its
   README assumes the old sibling checkout. The audits only *detect* this drift;
   generating the binding prevents it.
4. **Decide the 4.2% scene.** A 3-point bisect through the 4.4 branches would
   name the upstream change; it needs an LFS fetch and a build of the 4.4
   RhinoCycles branch. Otherwise document and park it.
5. **Fix the smoketest**, or drop it in favour of `run_checks.ps1 -Render`, which
   does work.
6. ~~Decide how a source build should be triggered.~~ **Done.** The CMake build
   tree is the switch, so the flag has to be found once and never again; the
   reasoning and the rejected alternatives are in `tools/PORTING-GAPS.md`. The
   two fixes that went with it are also in: `RhinoCyclesCore` now depends on
   `ccycles`, so deploying cannot be forgotten, and the build says which payload
   it used.

## Going deeper

- `tools/DIAGNOSTICS.md` — every diagnostic switch, what each established, and
  what has been eliminated and how
- `tools/PORTING-GAPS.md` — the things needing a decision rather than a fix
- `tools/PORT-HISTORY.md` — the earlier 4.x attempts, what they hold, and what
  reusing them saves
