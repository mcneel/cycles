Cycles 5 on macOS - where to start
==================================

Everything in [QUICKSTART.md](QUICKSTART.md) and the Building section of
[RHINO-CYCLES-5.md](RHINO-CYCLES-5.md) is Windows-only. This is what the Mac side
looks like today, measured rather than remembered, and a proposed order of work.

## What exists

The entire Mac deployment is eighteen lines of `cp` in
`src4/BuildSolutions/MacDotNetMakefile` (lines 336-355):

    libccycles.dylib  ->  RhinoCycles.rhp/ and Frameworks/
    lib/*.dylib       ->  Frameworks/
    source/           ->  RhinoCycles.rhp/RhinoCycles/source
    install_name_tool -add_rpath  on the copied dylib

It reads `big_libs/RhinoCycles/ccycles/osx/release` - hardcoded, no `debug`, no
`local`, no freshness test. Line 348 deletes `RhinoCycles/lib` on purpose
([RH-47672](https://mcneel.myjetbrains.com/youtrack/issue/RH-47672)).

## The good news: no kernel binaries

There is not a single `.metallib` in the payload. Metal compiles kernels at
runtime from the shipped `source/` tree, so the problem that produced
`kernel_arches.ps1`, `publish_payload.ps1` and the 41-file manifest **does not
exist on macOS**. A Mac payload is a dylib, its dependency dylibs, and `source/`.
No arch lists, no inherited kernels, no local-vs-release kernel gap - a Mac dev
who builds Cycles gets the real thing on the first try.

That also means `source/` completeness *is* the correctness condition on Mac. If
it is stale or partial, kernels fail to compile at runtime rather than at build
time.

## What will break first

`big_libs/RhinoCycles/ccycles/osx` was last touched by `161a49194` (2026-07-22, a
9.x merge). Its shipped kernel source is 207 files and contains
`bsdf_microfacet_multi.h` and `bsdf_principled_diffuse.h` - both removed upstream
in Cycles 4.0. Our tree has 391 files, with `bsdf_sheen.h`, `bsdf_ray_portal.h`
and `volume_draine.h`.

So the committed Mac payload is a **Cycles 3.5 dylib**, and Rhino 10 on a Mac
would load it against our 5.3 `csycles`. Expect missing exports, not subtle
render differences.

## What has no Mac counterpart at all

| Windows | Mac |
|---|---|
| `bootstrap.exe /cycles` installs CUDA, OptiX, ROCm | nothing - and probably nothing needed: Metal ships in Xcode |
| `ccycles.vcxproj` -> `build_cycles.ps1` | nothing; only `GNUmakefile` (upstream) and the Linux `make_*.sh` |
| `publish_payload.ps1`, `kernel_arches.ps1`, manifest | nothing |
| `RhinoCyclesCore.csproj` picks `local` while newer | makefile is hardcoded to `release` |
| `tools/run_checks.ps1` | nothing |
| `RhinoCycles_ListDevices` payload lines | works as soon as `ccycles_payload.json` is copied - it is managed code |

## Proposed order

1. **Build `libccycles.dylib` 5.3 on the Mac at all.** CMake with Metal on. This
   is the only step that has to come first; everything else is plumbing.
2. **Publish an arm64 payload** into `osx/release` in the existing layout, plus
   `ccycles_payload.json` so `ListDevices` answers there too. A `publish_payload.sh`
   can be much simpler than the PowerShell one - no arch lists to verify, just
   dylib + `lib/` + `source/` + manifest.
3. **Update `MacDotNetMakefile`** to copy the manifest, and to prefer `local` over
   `release` while it is newer - the same rule `RhinoCyclesCore.csproj` uses, so a
   Mac dev's own build wins and a pull takes over again by itself.
4. **A Mac build entry point** equivalent to `ccycles.vcxproj`, so building Rhino
   builds Cycles. Where this hangs off the Mac build is the open design question.
5. **Bootstrap** last, if it turns out to be needed at all.

## Open question

Rhino 10 on Mac: arm64 only, or universal with x86_64? That decides whether the
publish step is one build or two plus `lipo`, and it is the one thing in this list
that is not ours to decide.
