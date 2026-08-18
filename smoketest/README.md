# ccycles smoke test

A console harness that drives Cycles the way RhinoCycles does - through
`ccycles.dll` and the `csycles` P/Invoke wrapper - and writes a PPM. It exists
because "it compiles" and "it renders" turned out to be very different things
during the 3.5 to 5.2 port: the first build that linked cleanly produced a
uniform grey image, and four separate faults were hiding behind it.

## Running

    dotnet build smoketest.csproj -c Release
    copy ..\build\bin\RelWithDebInfo\ccycles.dll rel\
    copy bin\Release\net48\smoketest.exe rel\
    cd rel
    .\smoketest.exe

It prints scene statistics, renders, and writes `smoketest.ppm` next to the
executable. Exit code 0 and a non-uniform image means the stack works
end to end.

## Knobs

Every one of these exists because it settled a question that reading the code
could not.

| Variable | Default | What it is for |
| --- | --- | --- |
| `SMOKE_CAMZ` | `-12` | Camera position on Z. Cycles looks down **+Z** here, which is worth knowing before reading any of these images. |
| `SMOKE_LIGHTX` | `4` | Light position on X. The lit half of the quad must follow it. |
| `SMOKE_LIGHTZ` | `-6` | Light position on Z. Positive is behind the quad, so the quad goes dark. |
| `SMOKE_SPOTZ` | unset | Makes the light a spot aimed along this Z. Set it to `1` and `-1`: only `1` lights the quad. This is what pinned down the light basis. |
| `SMOKE_AREA` | unset | `1` makes it a 4x4 area light with explicit axisu/axisv. |
| `SMOKE_NODE` | unset | Drives the surface colour from one shader node by its registered name, e.g. `rhino_checker_texture`. One node per process, so a crash in one does not hide the rest. |
| `SMOKE_NOMESH` | unset | `1` leaves the quad out. Isolates geometry faults from light faults. |
| `SMOKE_NOLIGHT` | unset | `1` leaves the light out. |
| `SMOKE_NOSTART` | unset | `1` builds the scene and exits without rendering. |

`sweep.sh` in `rel/` runs `SMOKE_NODE` across all 22 Rhino shader nodes.

## Reading the output

The world is an explicitly black shader. That matters: the default background
is a random colour per session, which swamps anything the light contributes
and made every early reading meaningless. If you add a background, expect the
absolute levels to stop meaning anything and compare within a single run.

## When it crashes

`cycles_debug_install_crash_handler()` is called at startup. ccycles runs under
a managed host, so a native access violation would otherwise arrive as an
`AccessViolationException` with no native frames. The handler runs ahead of the
CLR and prints a symbolised stack, which is how the light-tree null dereference
was found. It needs `ccycles.pdb` beside the DLL.
