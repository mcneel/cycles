Cycles for Rhino
================

McNeel's fork of [Cycles](https://www.cycles-renderer.org), Blender's path tracer,
as used by Rhino's Raytraced viewport and Rhino Render. It is upstream Cycles v5.2.0
plus:

- **ccycles** (`src/ccycles`): a C API over Cycles, built as `ccycles.dll` /
  `libccycles.dylib`.
- **csycles** (`src/csycles`): the C# P/Invoke wrapper over ccycles.
- **Rhino's shader nodes and edits inside upstream files**: Rhino's procedural
  textures as SVM nodes, in-memory images, decal masking and mirrored tiling on the
  image texture node, and light handling.

`src/util/version.h` reads 5.3.0: upstream bumps the number right after tagging 5.2.0.

## How it fits into Rhino

Paths from the root of the Rhino repository:

| Path | What |
| --- | --- |
| `src4/rhino4/Plug-ins/RDK/cycles-core` | this repository (a submodule) |
| `src4/rhino4/Plug-ins/RDK/RhinoCycles` | the Rhino plug-in that uses it ([README](../RhinoCycles/README.md)) |
| `big_libs/RhinoCycles/ccycles/win/`, `.../osx/` | the prebuilt Cycles payload: library, dependency libraries, GPU kernels (`lib/`), kernel sources (`source/`) |

A normal Rhino build never compiles Cycles. `RhinoCyclesCore.csproj` (Windows) and
`src4/BuildSolutions/MacDotNetMakefile` (macOS) copy the payload from `big_libs`
into the build output, and RhinoCyclesCore loads it through csycles. Building
Cycles from source writes a new payload into `big_libs` and takes the same route.
See [BUILDING.md](BUILDING.md).

## Where things are

| Path | What |
| --- | --- |
| `src/ccycles` | C API; `ccycles.vcxproj` drives `build_cycles.ps1` from `Rhino.sln` |
| `src/csycles` | C# wrapper; `csycles.csproj` is in `Rhino.sln` |
| `src/scene/rhino_shader_nodes.*`, `src/kernel/svm/svm_rhino_*.h` | Rhino's SVM nodes |
| `src/scene/image_rhino.*` | in-memory images handed over by Rhino |
| `build_cycles.ps1`, `kernel_arches.ps1`, `publish_payload.ps1` | Windows build, shipping GPU architectures, full payload |
| `GNUmakefile`, `fix-cycles-rpaths.sh`, `fix-cycles-tbb.sh` | macOS build and payload fixups |
| `make.bat` | upstream wrapper; `make.bat update` fetches the Windows libraries |
| `lib/` | Blender's precompiled libraries, as submodules fetched on demand |
| `tools/run_checks.ps1`, `tools/audit_*.py`, `tools/check_lib_bundle.ps1` | static checks |
| `tools/DIAGNOSTICS.md` | runtime diagnostic switches |
| `smoketest/` | console harness that renders through csycles without Rhino |

Upstream only, not used by Rhino: `src/app`, `src/hydra`, `web/`, `.gitea/`,
`tools/sync_*.py`, `tools/update_lib_submodules.py`.

## More

- [BUILDING.md](BUILDING.md): build on Windows and macOS, publish a payload.
- [KNOWN-ISSUES.md](KNOWN-ISSUES.md): open issues and expected look changes from Cycles 3.5.
- After merging upstream, run `tools/run_checks.ps1`. Rhino's edits inside upstream
  files fail silently when a merge drops them; the audits catch the known ways.
