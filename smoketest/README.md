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
| `SMOKE_EMIT` | unset | `1` drives emission rather than a BSDF, intended to make a texture's output land in the pixels directly. **Does not work yet** - the surface renders black. The member name is not the reason: csycles' EmissionNode uses `strength`, which matches `SOCKET_IN_FLOAT(strength, "Strength", 10.0f)` in shader_nodes.cpp, and the socket audit agrees. Worth retrying, see below. |
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

## Open question: are the texture patterns right?

The node sweep proves all 22 Rhino nodes construct, compile through the SVM
layer and execute without crashing. It does **not** prove they produce the
right pattern - every procedural texture comes out flat here.

Three things have been ruled out as explanations: the missing per-mesh
attribute upload from the 4.4 line (porting it changed nothing), the
texture-coordinate input (wiring `texture_coordinate` in changed nothing),
and anything Rhino-specific - stock `checker_texture` and `noise_texture`
are equally flat in this harness.

That reasoning pointed at the harness rather than the port, and on the evidence
listed it still does: a fault in Rhino's own nodes would not make stock
`checker_texture` flat as well.

It is worth re-running regardless. A port bug of very nearly this shape was found
and fixed afterwards - `rhino_texture_coordinate` emitted its Generated output
through the pre-5.2 packed SVM layout, so `out_offset` was read as zero and the
node wrote to stack slot 0 while its consumer read the slot it had been
promised. Every environment texture then sampled a degenerate direction, which is
what made every render black. A constant vector arriving at a texture is exactly
how a pattern comes out flat, so the two observations here deserve another look
now that it is fixed, both this and SMOKE_EMIT.

If they are still flat, the harness explanation stands and the remaining check is
a comparison against shipping Rhino with real materials. `tools/render_regression.ps1`
now does that part on Rhino's own material preview scenes.

## When it crashes

`cycles_debug_install_crash_handler()` is called at startup. ccycles runs under
a managed host, so a native access violation would otherwise arrive as an
`AccessViolationException` with no native frames. The handler runs ahead of the
CLR and prints a symbolised stack, which is how the light-tree null dereference
was found. It needs `ccycles.pdb` beside the DLL.
