# ccycles smoke test

A console program that drives Cycles the way RhinoCycles does, through csycles and
ccycles, without Rhino. It renders a 160x120 quad lit by one light against a black
world, 4 samples on the first device, and writes `smoketest.ppm` next to the
executable. A non-uniform image means the stack works end to end.

## Windows

From this folder, with a payload in `big_libs` (the committed one will do):

    dotnet build ..\src\csycles\csycles.csproj -c Debug
    dotnet build smoketest.csproj -c Release
    xcopy /E /I /Y ..\..\..\..\..\..\big_libs\RhinoCycles\ccycles\win\release bin\Release\net48
    bin\Release\net48\smoketest.exe

The `xcopy` puts `ccycles.dll`, its DLLs, `lib/` and `source/` beside the executable,
which is where it tells Cycles to look. Use `win\local` or `win\debug` instead of
`win\release` to test your own build.

On a native crash ccycles' crash handler prints the native stack (Windows only).
Function names need `ccycles.pdb`, which stays in `build/bin/RelWithDebInfo` and is
not in any payload.

## macOS

    dotnet build smoketest.mac.csproj

builds the same program for net8.0 into `mac-bin/`, compiling the csycles sources
directly. `libccycles.dylib`, its dylibs and `source/` have to be loadable from the
executable's folder.

## Switches

| Variable | Default | Effect |
| --- | --- | --- |
| `SMOKE_DEVMASK` | `CPU` | Device types to enumerate: `CPU`, `CUDA`, `OPTIX`, `HIP`, `METAL`, `ONEAPI`, `All` |
| `SMOKE_CAMZ` | `-12` | Camera Z. The camera looks along +Z at the quad at Z = 0. |
| `SMOKE_LIGHTX` | `4` | Light X |
| `SMOKE_LIGHTZ` | `-6` | Light Z. Positive puts it behind the quad, which then goes dark. |
| `SMOKE_SPOTZ` | unset | Makes the light a spot aimed along this Z. `1` lights the quad, `-1` does not. |
| `SMOKE_AREA` | unset | `1`: a 4x4 area light |
| `SMOKE_EMIT` | unset | `1`: emission instead of diffuse, so a texture lands in the pixels directly |
| `SMOKE_NODE` | unset | Drives the surface colour from one shader node by name, e.g. `rhino_checker_texture` |
| `SMOKE_IMAGE` | unset | Drives the surface colour from this image file through `image_texture` |
| `SMOKE_NOMESH` | unset | `1`: no quad |
| `SMOKE_NOLIGHT` | unset | `1`: no light |
| `SMOKE_NOSTART` | unset | `1`: build the scene and exit without rendering |
| `SMOKE_TIMEOUT` | `600` | Seconds to wait for the render |

`CCYCLES_DIAG_LOG` adds a scene summary (geometry, objects, lights, passes) before
the render; see [../tools/DIAGNOSTICS.md](../tools/DIAGNOSTICS.md).
