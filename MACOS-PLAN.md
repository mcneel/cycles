Cycles 5.2 on macOS
===================

This started as a plan. It is now a description of what is there, because the
work is done: Cycles 5.2 builds on macOS, renders on Metal, and the payload is
published. What follows is the state of it, the decisions taken, and what is
still outstanding.

## Status

Verified on Apple M1, macOS 26.5, Xcode 26.5:

- `libccycles` 5.2 builds arm64 with Metal.
- CPU and Metal produce **bit-identical** output (0 of 19200 pixels differ in the
  smoke test).
- Rhino 10.x builds and renders with it, in both Debug and Release.
- `PBRMatTest.3dm` renders correctly against the Cycles 3.5 reference: same
  materials, same textures, no colour shift.

The one measured difference from 3.5 is that 5.2 is uniformly about 1.5%
brighter - mean RGB 144.72/142.44/141.83 against 140.93/138.63/137.95 - with the
delta identical across all three channels, concentrated in shadow penumbra. It is
not a build-configuration artefact: Debug and Release agree to 0.03, and the 3.5
reference was itself a Release build.

## Building and publishing a payload

    make release

That builds it and then runs both payload fixups, so regenerating the prebuilt
cannot skip them:

- `fix-cycles-rpaths.sh` - the build bakes machine-specific absolute rpaths into
  `libccycles.dylib`, which break every deployed copy (RH-96549). `MacDotNetMakefile`
  fails the Rhino build if this was not done.
- `fix-cycles-tbb.sh` - renames the payload's oneTBB to `libtbb.12.cycles.dylib`
  and repoints the payload at it (RH-98415, below).

Then:

    rsync -a --delete install/lib/    <big_libs>/RhinoCycles/ccycles/osx/release/lib/
    rsync -a --delete install/source/ <big_libs>/RhinoCycles/ccycles/osx/release/source/
    cp install/libccycles.dylib       <big_libs>/RhinoCycles/ccycles/osx/release/

`rsync -a --delete`, not `cp -r`. `cp` merges the new `source/` over the old one
and leaves both kernel generations in place; on Mac `source/` completeness *is*
the correctness condition, because Metal compiles from it at runtime.

`make clean` removes `install/` as well as `build/`. It has to: `install/` is only
ever added to, so libraries and CMake cache values from a previous configuration
survive and get published. A build with Embree off once shipped the Embree dylib
from an earlier build with it on, and a stale cache silently kept
`WITH_CYCLES_NATIVE_ONLY=ON` through what looked like a fresh build.

## Architecture: arm64 only

Decided, with evidence rather than preference.

Blender stopped publishing Intel macOS dependencies after 4.5. `lib-macos_x64`
has no 5.0, 5.1 or 5.2 release branch, and its `main` was last touched
2025-06-06 and is missing fourteen of the packages the arm64 set has, including
several Cycles links. Building Cycles 5.x for Intel would mean maintaining that
dependency stack ourselves - which is what `lib/darwin_universal` was for the 3.5
era, and why the committed 3.5 payload is universal.

Intel Mac support is not wanted, so the payload is arm64. `GNUmakefile` takes
`MAC_ARCHS` if that ever changes:

    make release MAC_ARCHS="x86_64;arm64"

## No kernel binaries

There is not a single `.metallib` in the payload. Metal compiles kernels at
runtime from the shipped `source/` tree, so the problem that produced
`kernel_arches.ps1`, `publish_payload.ps1` and the 41-file manifest **does not
exist on macOS**. A Mac payload is a dylib, its dependency dylibs, and `source/`.
No arch lists, no inherited kernels, no local-vs-release kernel gap.

First render after a payload change pays for the compile - 173 s in the smoke
test - and it is cached afterwards, 0.5 s on the second run.

## Deployment

The Mac deployment is the `#RhinoCycles` block of
`src4/BuildSolutions/MacDotNetMakefile`, lines 337-358. Four references to
`big_libs/RhinoCycles/ccycles/osx/release`:

    libccycles.dylib  ->  RhinoCycles.rhp/ and Frameworks/    (345, 357)
    lib/*.dylib       ->  Frameworks/                         (356)
    source/           ->  RhinoCycles.rhp/RhinoCycles/source  (358)

Hardcoded to `release`; no `debug`, no `local`, no freshness test. Line 351
deletes `RhinoCycles/lib` on purpose
([RH-47672](https://mcneel.myjetbrains.com/youtrack/issue/RH-47672)). Lines
346-347 are the RH-96549 guards.

Preferring a `local` payload while it is newer, the way `RhinoCyclesCore.csproj`
does on Windows, is still not implemented here.

## TBB (RH-98415)

The payload's dependencies land in Rhino's *shared* `Contents/Frameworks`, and
both Rhino and Cycles wanted to own `libtbb.dylib` there. They are not
interchangeable:

- Rhino's is **TBB 2020.3**, the pre-oneTBB API. USD needs it and uses 45 symbols
  oneTBB does not have.
- Cycles' is **oneTBB**. Cycles needs 29 `tbb::detail::r1` symbols 2020.3 does not
  have.

Whichever landed last won, and the loser's consumers resolved against a library
that could not satisfy them - Cycles died in `ccl::TaskScheduler::init`, and once
past that, in `embree::TaskScheduler::create`.

Fixed by giving the payload's oneTBB a private name, `libtbb.12.cycles.dylib`.
Nothing of Rhino's changes. Rhino also ships a `libtbb.12.dylib` (oneTBB 12.7)
for OpenImageDenoise and ODA; taking that filename over would also have worked,
but it would raise a library those components depend on in order to satisfy
Cycles. Windows never sees any of this, because the payload lives in its own
folder beside the plug-in.

## Feature parity with Windows

Embree, OSL, OpenImageDenoise and OpenVDB/NanoVDB used to be disabled in
`GNUmakefile`. Those were build workarounds from the 3.5 era - the commits say
"temporarily disable ... in order to get Cycles kernels building" and "Metal
building fixes" - not decisions about what Mac should ship. Windows never
disabled them, so Mac was quietly rendering without denoising, volumes, OSL and
Embree's BVH.

They build and render fine on 5.2 and are back on. Mac and Windows now disable
the same set and nothing more: Alembic, Hydra, the standalone GUI, USD.

## Still outstanding

- **Clipping planes are not supported in 5.2**
  ([RH-98414](https://mcneel.myjetbrains.com/youtrack/issue/RH-98414)). The native
  `clip_all_rays` / `clipping_plane_mask` support was never ported - fifteen files
  across the scene layer, the C API and the CPU, Metal and OptiX kernels - and the
  managed call sites were removed so 5.2 would compile. Deferred deliberately.
- Only `PBRMatTest.3dm` has been rendered end to end, and only on this machine.
  The payload is portable by build flags (`NATIVE_ONLY=OFF`, no `-march=native`),
  but that has not been proven on other hardware.
- No `local`-payload preference in `MacDotNetMakefile`, as above.
- A freshly built `Rhinoceros.app` ignores `-runscript` on its **first** launch and
  sits on the start screen. Kill it and launch again. Worth knowing before
  concluding a headless test has hung.
