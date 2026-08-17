# Building Cycles for Rhino

## Short version

**You almost certainly do not need to build Cycles.** Open `Rhino.sln` and build
as usual. Prebuilt `ccycles` binaries are restored automatically from
`big_libs`, and nothing here requires CMake, CUDA, or an OptiX SDK.

Read on only if you are changing Cycles or the C API itself.

## Prerequisites

Only needed when building Cycles from source:

| Tool | Notes |
| --- | --- |
| Visual Studio 2022 | With the *Desktop development with C++* workload. VS2019 is no longer required. |
| CMake | On `PATH`. |
| Git + Git LFS | Git LFS is how the precompiled dependency bundle is fetched. |
| Python 3 | On `PATH`. |

Optional, each enabling one GPU backend. Anything missing is switched off
rather than failing the build, so a machine with none of these still produces a
working CPU-only Cycles:

| Backend | Detected via |
| --- | --- |
| CUDA | `CUDA_PATH`, or the newest toolkit under `C:\Program Files\NVIDIA GPU Computing Toolkit\CUDA` |
| OptiX | `OPTIX_ROOT_DIR`, or the newest `OptiX SDK *` under `C:\ProgramData\NVIDIA Corporation` |
| HIP | `HIP_PATH` |
| oneAPI | `level-zero` inside the precompiled library bundle |

Subversion is **not** required. Neither is ResourceHacker, unless you are
stamping resources onto the prebuilt third-party DLLs (see below).

## Building from Visual Studio

Set the `RHINOCYCLESDEV` environment variable to anything non-empty, then build
`Rhino.sln`. The `ccycles` project under the *CCSycles* solution folder
configures and builds Cycles into the Rhino `Plug-ins` output directory.

With `RHINOCYCLESDEV` unset the `ccycles` project does nothing and the prebuilt
binaries are used. That is the default, and what most people want.

## Building from the command line

```powershell
cd cycles
.\build_cycles.ps1 -Configuration Release
```

Useful switches:

| Switch | Effect |
| --- | --- |
| `-Devices cpu` | Force a CPU-only build. |
| `-Devices cuda,optix` | Enable a specific set instead of auto-detection. |
| `-ConfigureOnly` | Configure and stop, leaving `build\Cycles.sln` to open in VS. |
| `-CudaBinaries` | Build the full cubin set rather than PTX only. Slow. |
| `-InstallDir <path>` | Override where binaries land. |

The first run fetches the precompiled dependency bundle into `cycles/lib/` via
`make update`. That is a large Git LFS download and takes a while.

## Release builds

`cycles/make_rhino_all.ps1` is the full release pipeline: clean, configure,
build, stamp version resources, copy outputs into `big_libs/RhinoCycles`, and
run the Docker-based HIP kernel build. `build_cycles.ps1` is the developer
inner loop; `make_rhino_all.ps1` is what produces what ships.

## Version resources and the OpenImageIO clash

Rhino loads its own OpenImageIO, OpenEXR and TBB into the same process as
Cycles. `ccycles.dll` therefore carries a Win32 side-by-side private assembly
manifest listing the Cycles-side copies as assembly members, so the loader
resolves them within the ccycles assembly context instead of by bare filename.

That manifest, and `ccycles.dll`'s VERSIONINFO, are compiled in at link time by
`cycles/src/ccycles/CMakeLists.txt` — see `ccycles_manifest.xml.in`. No manual
step, no external tool.

`versioninfo_changer.ps1` still exists for the binaries we ship but do not
build: `openvdb.dll` and the oneAPI JIT DLL come out of the precompiled bundle,
so resources can only be attached afterwards. It needs ResourceHacker on `PATH`
and is invoked by `make_rhino_all.ps1`.

## Layout

| Path | What it is |
| --- | --- |
| `cycles/` | The Cycles fork (submodule, `mcneel/cycles`). |
| `cycles/src/ccycles/` | The C API around Cycles. Built by the Cycles CMake build. |
| `csycles/` | The C# wrapper over the C API. Built by `Rhino.sln`. |
| `ccycles.vcxproj` | Makefile-style project that drives the Cycles CMake build from `Rhino.sln`. |
