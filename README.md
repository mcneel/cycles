Cycles Renderer
===============

Cycles is a path tracing renderer focused on interactivity and ease of use, while supporting many production features.

https://www.cycles-renderer.org

## Building

Cycles can be built as a standalone application or a Hydra render delegate. See [BUILDING.md](BUILDING.md) for instructions.

## Examples

The repository contains example xml scenes which could be used for testing.

Example usage:

    ./cycles scene_monkey.xml

You can also use optional parameters (see `./cycles --help`), like:

    ./cycles --samples 100 --output ./image.png scene_monkey.xml

For the OSL scene you need to enable the OSL shading system:

    ./cycles --shadingsys osl scene_osl_stripes.xml
	
## Building ccycles for Rhino

**In a hurry: [QUICKSTART.md](QUICKSTART.md).** One page, no history.

Run `bootstrap.exe /cycles` from the repo root once, which installs the GPU SDKs
on top of a normal bootstrap. Then Cycles builds from Visual Studio like any
other Rhino project: build `src4/BuildSolutions/Rhino.sln` in the `Debug+Cycles`
or `ReleaseDebuggable+Cycles` configuration and `ccycles.vcxproj` configures and
builds Cycles, installs it into a payload under
`big_libs/RhinoCycles/ccycles/win`, and `RhinoCyclesCore.csproj` copies it into
the plug-in output. The plain `Debug` and `Release` configurations use the
prebuilt payload instead, so no CMake, CUDA or OptiX SDK is needed. RhinoBuilder
offers the same configurations.

Such a build makes kernels for the GPUs in your own machine only - a kernel for
a card you do not own cannot be tested - and fills the rest in from the committed
payload. It writes a gitignored `local` payload rather than the committed one, so
it cannot replace what everyone else runs with kernels for a single card.
`RhinoCyclesCore` prefers `local` while it is newer than the committed payload,
so your Rhino runs what you built and a pull that republishes takes over again by
itself.

Building a single project is the usual mistake: `ccycles.vcxproj` alone updates
the payload but not the plug-in output, and `RhinoCyclesCore.csproj` alone copies
whatever the payload already holds without rebuilding Cycles. Do both, or the
solution.

To publish a payload - which is what a kernel change needs before it merges, or
everyone on a plain build gets a new `ccycles.dll` with the old kernels:

    powershell -File publish_payload.ps1

One command. It builds every backend for every shipping architecture, checks the
result file by file, writes a manifest, and stages the payload in `big_libs`; it
prints the two commits to make and does not make them. It requires all four
SDKs, and stops rather than shipping a payload missing a backend.

`tools/run_checks.ps1` answers whether the tree is sound, including whether the
committed payload still matches the kernel sources. A second, and its exit code
gates.

See `RHINO-CYCLES-5.md` for the state of the port, and `tools/DIAGNOSTICS.md` for
the diagnostic switches and what each one established.

All of the above is Windows. The Mac has no equivalent tooling - no
`publish_payload.ps1`, no manifest, no `run_checks.ps1` - and it needs far less of
it, because Metal compiles its kernels from the shipped `source/` tree at runtime,
so there are no kernel binaries to build or verify. A Mac payload is the dylib, its
dependencies, and `source/`. See [MACOS-PLAN.md](MACOS-PLAN.md).

Building it:

    make release

which also runs two fixups that the payload is not fit to ship without, so that
regenerating it cannot skip them:

- `fix-cycles-rpaths.sh` replaces the machine-specific absolute rpaths the build
  bakes into `libccycles.dylib` with portable `@loader_path` ones (RH-96549).
  `MacDotNetMakefile` fails the Rhino build if this was not done.
- `fix-cycles-tbb.sh` renames the payload's oneTBB to `libtbb.12.cycles.dylib` and
  repoints the payload at it. Both Rhino and Cycles otherwise want to own
  `libtbb.dylib` in `Contents/Frameworks`, and they are not interchangeable: Rhino's
  is TBB 2020.3, which USD needs, and Cycles' is oneTBB (RH-98415).

Then copy `install/*` into `big_libs/RhinoCycles/ccycles/osx/release`. Use
`rsync -a --delete` rather than `cp -r`: `cp` merges the new `source/` over the old
one and leaves both kernel generations in place, and on Mac `source/` completeness
is the correctness condition.

The committed Mac payload is still Cycles 3.5 and does not match a 5.x `csycles`.

The previous procedure for this - twelve manual steps per platform, editing
`cycles_device.vcxproj` by hand, ResourceHacker, and copying DLLs into `big_libs`
- no longer applies: it assumed a nested `RDK/cycles/cycles` directory and a
`Cycles.sln`, neither of which exists since the submodule collapse. It is
preserved in this file's history (`git log -p -- README.md`) in case the Mac
notes are still wanted.

## Contact

For help building or running Cycles, see the channels listed here:

https://www.cycles-renderer.org/development/
