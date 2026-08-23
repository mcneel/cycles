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

Cycles builds from Visual Studio like any other Rhino project. Set
`RHINOCYCLESDEV=1` and build `src4/BuildSolutions/Rhino.sln`; `ccycles.vcxproj`
configures and builds Cycles, installs it into
`big_libs/RhinoCycles/ccycles/win/{debug,release}`, and `RhinoCyclesCore.csproj`
copies it into the plug-in output. Without `RHINOCYCLESDEV` nothing is built from
source and the prebuilt payload is used, so no CMake, CUDA or OptiX SDK is
needed.

Building a single project is the usual mistake: `ccycles.vcxproj` alone updates
`big_libs` but not the plug-in output, and `RhinoCyclesCore.csproj` alone copies
whatever `big_libs` already holds without rebuilding Cycles. Do both, or the
solution.

To publish a payload, commit the installed files in `big_libs` on a branch. Only
the release payload is tracked; a `Debug/` rule in `big_libs/.gitignore` catches
the debug one, so it stays local unless force-added.

See `RHINO-CYCLES-5.md` for the state of the port, and `tools/DIAGNOSTICS.md` for
the diagnostic switches and what each one established.

The previous procedure for this - twelve manual steps per platform, editing
`cycles_device.vcxproj` by hand, ResourceHacker, and copying DLLs into `big_libs`
- no longer applies: it assumed a nested `RDK/cycles/cycles` directory and a
`Cycles.sln`, neither of which exists since the submodule collapse. It is
preserved in this file's history (`git log -p -- README.md`) in case the Mac
notes are still wanted.

## Contact

For help building or running Cycles, see the channels listed here:

https://www.cycles-renderer.org/development/
