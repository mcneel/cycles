Building Cycles for Rhino
=========================

Most Rhino developers never build Cycles: the plain configurations deploy the
prebuilt payload from `big_libs`. Build it only to change Cycles, ccycles or the GPU
kernels. Paths starting with `src4/` or `big_libs/` are relative to the Rhino
repository root; all others are relative to cycles-core.

Upstream's standalone and Hydra builds are documented in upstream's own
`BUILDING.md` (https://projects.blender.org/blender/cycles).

## Payloads

A payload is the Cycles library, its dependency libraries, the GPU kernels (`lib/`)
and the kernel sources (`source/`). They live in `big_libs/RhinoCycles/ccycles/`:

| Folder | Written by | In git |
| --- | --- | --- |
| `win/release`, `osx/release` | `publish_payload.ps1` (Windows), `make payload` (macOS) | yes |
| `win/local` | `ReleaseDebuggable+Cycles` (a build with fewer kernels than `release` holds is diverted here) | no |
| `win/debug` | `Debug+Cycles` | no |
| `osx/local` | the macOS Cycles schemes | no |

Which one a build deploys:

- **Windows** (`RhinoCyclesCore.csproj`): `local` if its `ccycles.dll` is newer than
  `release`'s; otherwise `debug` in a Debug build if it exists; otherwise `release`.
  The build prints `RhinoCycles: deployed Cycles payload from ...`. Output:
  `src4/bin/<Debug|Release>/Plug-ins/`, kernels in `Plug-ins/RhinoCycles/lib`,
  sources in `Plug-ins/RhinoCycles/source`.
- **macOS** (`src4/BuildSolutions/MacDotNetMakefile`): `local` if newer than
  `release`, otherwise `release`, into `RhinoCycles.rhp` and `Contents/Frameworks`.

A pull that republishes `release` makes it newer, so it wins again by itself.

## Windows

### Prerequisites

- A Rhino checkout set up with `bootstrap.exe`. It installs Git LFS and Visual
  Studio with Rhino's `.vsconfig`, which covers what Cycles needs from VS:
  - Visual Studio 2022 or newer with the C++ toolset and "C++ CMake tools for
    Windows" (CMake and Ninja come from there).
  - MSVC v143 14.4x. Required whenever GPU kernels are compiled: nvcc and ROCm's
    clang reject newer MSVC.
- `bootstrap.exe /cycles`, once. Also installs the CUDA 12.9 toolkit (winget) and the
  OptiX headers (cloned from NVIDIA's public `optix-dev` repository into
  `C:\ProgramData\NVIDIA Corporation\OptiX SDK <version>`).
- ROCm / HIP SDK 6.x, by hand: bootstrap offers AMD's download page. Only needed to
  build AMD kernels, which publishing requires. No AMD GPU is needed.
- git, python and Git LFS on `PATH` for the first `+Cycles` build, which fetches
  Blender's precompiled libraries (about 6.5 GB) into `lib/windows_x64`.

No SDK is fatal. CUDA and HIP device support is compiled in regardless; a missing SDK
only means those kernels are copied from the committed payload instead of built.
OptiX without its headers is compiled out of your build.

bootstrap's `/cycles` overrides, all optional. Set them with `setx` (machine or user
level), because bootstrap relaunches itself elevated:

| Variable | Default |
| --- | --- |
| `RHINO_CUDA_VERSION` | `12.9` |
| `RHINO_OPTIX_REPO` | `https://github.com/NVIDIA/optix-dev.git` |
| `RHINO_ROCM_INSTALLER` | unset: open AMD's download page. Set to a path to launch that HIP SDK installer. |

### Build

Open `src4/BuildSolutions/Rhino.sln` and build the solution in one of:

| Configuration | Cycles |
| --- | --- |
| `Debug`, `Release`, `ReleaseDebuggable` | prebuilt payload; no CMake or SDK needed |
| `Debug+Cycles` | built from source into `win/debug` |
| `ReleaseDebuggable+Cycles` | built from source into `win/local` |

RhinoBuilder lists the same configurations. From a shell:

    msbuild src4\BuildSolutions\Rhino.sln /p:Configuration="Debug+Cycles" /p:Platform=x64 /m

Both `+Cycles` configurations build Cycles as RelWithDebInfo: release-speed kernels,
and `ccycles.pdb` stays in `build/bin/RelWithDebInfo`, so ccycles is steppable on the
machine that built it. `ccycles.vcxproj` runs

    build_cycles.ps1 -Configuration RelWithDebInfo -InstallDir <payload folder>

with the CMake build tree in `build/` (Rebuild and Clean delete it).
It builds kernels only for the GPUs in this machine (found with `amdgpu-arch` and
`nvidia-smi`) and copies the rest from `win/release`. Those copied kernels do not
contain your kernel changes; the build warns when a GPU in this machine has no SDK.

**Build the solution, not single projects.** `ccycles.vcxproj` alone updates
`big_libs` but not `bin`; `RhinoCyclesCore.csproj` alone deploys whatever `big_libs`
holds. `RhinoCyclesKernelCompiler.exe` only reaches the plug-in folder in a solution
build, and without it GPU rendering silently falls back to the CPU or stalls.

Environment variables `build_cycles.ps1` reads, also from Visual Studio:

| Variable | Effect |
| --- | --- |
| `CYCLES_DEVICES` | kernel backends to build, e.g. `cpu,hip` (from `cpu`, `cuda`, `optix`, `hip`, `oneapi`) |
| `CYCLES_NATIVE_ONLY=1` | CPU kernel for this CPU only. Much faster kernel rebuilds; the result is not portable. |
| `CUDA_PATH`, `OPTIX_ROOT_DIR`, `HIP_PATH` | SDK locations, if not in the default install folders |

Run directly, `build_cycles.ps1` also takes `-Devices`, `-AllArches`, `-Force`
(overwrite `release` with a narrow build), `-ConfigureOnly`, `-Generator vs` (a
`Cycles.sln` to open), `-Jobs` and `-BuildDir`. `-InstallDir` must be absolute. An
unoptimised Cycles, for example:

    powershell -File build_cycles.ps1 -Configuration Debug -InstallDir <rhino>\big_libs\RhinoCycles\ccycles\win\debug

### Library bundle

`lib/windows_x64` is a submodule; its commit pins the Blender library bundle
everyone builds against. Every build checks it with `tools/check_lib_bundle.ps1` and
stops with a red banner and the fix on a mismatch or on unpulled LFS files
(`-AllowLibraryMismatch` overrides). The usual fix, from cycles-core:

    .\make.bat update

Note that it also runs `git pull --rebase` in cycles-core when the tree is clean and
tracks a branch. Moving the pin is a normal commit followed by a republish;
`.github/workflows/lib-bundle-watch.yaml` opens an issue when upstream's pin moves.

### Publish a payload

Needed whenever kernel code (`src/kernel`, `src/util`) changes, or everyone on a
plain build gets the new `ccycles.dll` with the old kernels. From cycles-core:

    powershell -File publish_payload.ps1

It builds every backend for every architecture in `kernel_arches.ps1` (22 HIP
fatbins, 9 CUDA cubins plus one PTX, 3 OptiX modules, oneAPI), checks the files by
name, writes `ccycles_payload.json` (including a hash of the kernel sources), stages
`win/release` in `big_libs` and prints the two commits to make: `big_libs`, then the
`big_libs` pointer in the Rhino repository. It needs CUDA, OptiX and ROCm (oneAPI
comes from the library bundle) and no GPU, and takes about an hour. It stops rather
than stage an incomplete payload. `-SkipBuild` re-checks and re-stages.

### Check

    powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1

About a second, no build needed (python for the audits). Runs the static audits,
checks that the committed payload matches the kernel sources, that the library
bundle is the pinned one, and that the installer's kernel list matches. Exit code is
non-zero on failure. `-Render` adds the golden-image test; see
[KNOWN-ISSUES.md](KNOWN-ISSUES.md).

### Run

Start `src4/bin/Debug/Rhino.exe`. `RhinoCycles_ListDevices` prints the Cycles
version, the path and date of the `ccycles.dll` in use, the payload manifest
(configuration, source commit, kernel counts) and the devices.

- *"Error Loading RhinoCore.dll" on a fresh `bin`*: `csycles.csproj` copies RhinoCore's
  OpenImageIO runtime DLLs from `big_libs` into `bin/<Config>`. Build the solution.
- *A `bin/Release` Rhino (Release, ReleaseDebuggable) has no RhinoCycles*: it runs
  under the `<version>-WIP-Developer-Release-trunk` settings scheme, and bootstrap
  registers plug-ins only under `...-Developer-Debug-trunk`
  (`HKCU\Software\McNeel\Rhinoceros\<scheme>\Plug-Ins`). Register them for the
  Release scheme too.

## macOS

Apple Silicon only: Blender publishes no Intel macOS libraries for Cycles 5.

### Prerequisites

Xcode, git, and CMake (the cmake.org app in `/Applications`, or Homebrew).

### Build

Open `src4/rhino4/MacRhino.xcworkspace` and build the scheme
`RhinoApplication - Debug Cycles` or `RhinoApplication - Release Cycles`. Both run
`make local` in cycles-core, which:

1. fetches Blender's libraries into `lib/macos_arm64` at the pinned commit (about
   2.4 GB, once; `make deps`),
2. builds RelWithDebInfo in `build-local/` and `install-local/`,
3. runs `fix-cycles-rpaths.sh` (portable `@loader_path` rpaths; MacDotNetMakefile
   fails the build without them, RH-96549) and `fix-cycles-tbb.sh` (renames the
   payload's oneTBB to `libtbb.12.cycles.dylib` so it does not replace Rhino's TBB
   2020.3, RH-98415),
4. copies the result into `big_libs/RhinoCycles/ccycles/osx/local`.

`RhinoApplication - Debug` and `- Release` use the prebuilt payload.

There are no kernel binaries on macOS: Metal compiles the kernels from `source/` on
the first render after a payload change (minutes, once; cached afterwards).

### Publish a payload

From cycles-core:

    make payload

runs `make deps`, `make release` (build plus both fixups) and `make publish`
(`rsync --delete` into `osx/release`). Then commit in `big_libs`, and the `big_libs`
pointer in the Rhino repository. `make clean` deletes `build/` and `install/`; do it
after changing CMake options, because `install/` is only ever added to.
