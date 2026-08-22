# Diagnosing a Rhino render through ccycles

Every switch here is read from the environment and off unless set, so a build
with all of them compiled in behaves exactly as before. They exist because the
faults on the 3.5 → 5.2 port share a shape: the build is green, nothing asserts,
and the pixels are wrong. Reading the code does not settle those; measuring does.

Set them before launching Rhino. For a locally built Rhino, see
`tools/render_regression.ps1` for the launch that works — the model goes on the
command line, because opening a document over MCP tears the MCP listener down.

## Getting anything out at all

| Variable | Effect |
| --- | --- |
| `CCYCLES_DIAG_LOG=<path>` | Append every `ccycles_diag` line to that file. |
| `CCYCLES_LOG_LEVEL=<level>` | Route Cycles' own log to the same file. `info` or `debug`. |

Start with these two. `OutputDebugString` alone loses records: it goes through a
single 4KB system buffer with an event handshake, and a listener a moment late
simply drops one, which turns a scene dump into a row of bare prefixes.

`CCYCLES_LOG_LEVEL` matters more than it sounds. ccycles never called
`ccl::log_init`, so everything Cycles knew about devices, kernels, passes, shader
compilation and the light tree was being discarded. `"Number of lights sent to
the device: 1"` in a scene holding two is the sort of line that ends an
investigation early.

With `CCYCLES_DIAG_LOG` set, session start also dumps the scene: geometry,
objects, per-mesh shader slots, lights with their strength and transform, the
integrator's bounces and switches, the film state, and every shader graph with
the value Cycles holds for each unlinked input.

## Where are the pixels

| Variable | Effect |
| --- | --- |
| `CCYCLES_PASS_PROBE=1` | Once per session, report min, max, mean and non-zero count for `combined`, and try `shadow_catcher_matte`, `shadow_catcher` and `background`. |

This separates "Cycles produced nothing" from "the pixels were lost on the way
out". A combined pass of exactly 0.0 for RGB with alpha exactly 1.0 everywhere,
against a sensible depth pass, says rays hit geometry and shading returned
nothing — which is a very different search from a readback bug.

The three shadow-catcher passes are usually reported as unavailable: they are
written with empty names, and `get_pass_pixels` matches on name.

## Which node is wrong

| Variable | Effect |
| --- | --- |
| `CCYCLES_SIMPLE_BACKGROUND=1` | Replace Rhino's background graph with one white `background_shader`. |
| `CCYCLES_BG_TAP=<node name>` | Wire that node's first colour, float or vector output straight into `final_bg`'s Color. |
| `CCYCLES_DUMP_BG=<path>` | Write the background shader graph as graphviz. |

These three found the black render. Rhino's background graph is around forty
nodes of `light_path` gating, so the first question is whose fault it is:
`CCYCLES_SIMPLE_BACKGROUND` lit the scene immediately, which put Cycles in the
clear and Rhino's graph under suspicion.

`CCYCLES_BG_TAP` then makes a render *show* what any node in that graph evaluates
to, so a chain can be bisected in a handful of runs. It was white as far as
`gradient_or_other` and black from `sky_color_or_texture` on; following the
texture's vector chain, every node read zero, including the first. That first
node was `rhino_texture_coordinate`, whose Generated output was being written to
SVM stack slot 0 instead of the slot its consumer read.

A tapped vector shows as a colour, so negative components clamp to black. That
is enough to tell a real direction from all zeros.

## Narrowing a difference against another build

| Variable | Effect |
| --- | --- |
| `CCYCLES_WHITE_TINTS=1` | Force `specular_tint` and `sheen_tint` white on every principled node, cutting links. |
| `CCYCLES_TEX_COLORSPACE=data\|srgb\|linear` | Force every image texture's colorspace. |
| `CCYCLES_NO_SHADOW_CATCHER=1` | Ignore every object's shadow-catcher flag. |
| `CCYCLES_NO_LIGHT_TREE=1` | Force the old light distribution. Of little use for comparing against shipping - see below - and one run under it hung before the session started. |
| `CCYCLES_BG_SKY_FROM_COLOR=1` | Force `sky_color_or_texture`'s Fac to 0, taking the environment image out of the skylight path. |
| `CCYCLES_NO_CLAMP=1` | Set both sample clamps to 0, which Cycles reads as no limit at all. Takes the clamp's bias out of a comparison. |

Use these to size a suspicion before changing code. `CCYCLES_WHITE_TINTS` moved
the material preview scene from 11.27 against shipping Rhino 9 WIP to 9.96, and
the real fix then landed on exactly 9.96 — which is how you know the fix did what
the probe predicted and nothing more.

`CCYCLES_TEX_COLORSPACE` is the cautionary one, and it has now been run down.
It brackets rather than answers: `data` and `linear` are identical to `auto` at a
0.9575 ratio, `srgb` overshoots to 1.0642, and shipping sits between them. One
texture swings the whole frame by eleven percent, so the switch is worth having.

But it is not the difference against shipping, and the reason is worth writing
down because the obvious reading is wrong. Without an OCIO config, 5.2 resolves
`auto` as

    (is_float && file_colorspace is not srgb_rec709_*) ? scene_linear : srgb

so a *byte* image is still decoded as sRGB; only float images come through
untouched. 3.5 decoded byte images as sRGB too, and for float images asked for
`file_colorspace` of "sRGB"/"GammaCorrected" or an empty one with a png, jpeg,
tiff, dpx or jpeg2000 *file format*.

Rhino never hands Cycles a file. Textures arrive as builtin in-memory images
through `builtin_image_float_pixels`, which leaves both `file_colorspace` and
`file_format` empty. So under 3.5 the format test failed and the image was `raw`;
under 5.2 it is `scene_linear`. Neither decodes, and the two agree. That also
confirms these images are float rather than byte - if they were byte, `auto`
would already equal `srgb` and forcing `linear` could not have moved the frame.

The named-colorspace path did lose the old "sRGB" and "GammaCorrected" spellings
and the file-format guess between the two versions. That is a real behavioural
change and it would bite anything passing Cycles a filename. It does not bite
here.

## What the remaining difference against shipping is, and is not

The gap against shipping Rhino 9 WIP on the material preview scene is a ratio of
0.9577 - ours is 4.2% darker. Three measurements place it, and each one removes a
suspect rather than adding one.

**It is not the materials.** Rendering the same scene with a plain material on the
floor gives 0.9507, slightly *worse* than the full PBR scene's 0.9577. Whatever
this is, it does not need a principled node to happen, so the 4.x Principled
rework is not the cause.

**It is not the environment itself.** A background-only render with no geometry in
it comes out at 1.0004, a mean absolute difference of 0.14 per channel against a
noise floor of 0.03. The environment as seen by camera rays is right. It is only
surfaces *lit* by it that are dark.

**It is graded by brightness.** Bucketing by the shipping render's luminance,
with both images smoothed by a 7x7 box first:

| shipping luminance | ours/shipping |
| --- | --- |
| 85-127 | 0.9998 |
| 128-170 | 0.9981 |
| 171-212 | 0.9721 |
| 213-255 | 0.9526 |

That rules out both of the cheap explanations. A global gain would be flat across
every band; a gamma would pull the midtones off worst and both ends toward 1.
This does neither - the midtones match to within 0.2% and the loss grows
monotonically with brightness.

The smoothing is not cosmetic, and this table should not be produced without it.
Bucketing on one image and averaging the other within each bucket is regression
to the mean: if one render were merely noisier than the other, that alone would
produce a monotonic ratio trend of exactly this shape and nothing else. Smoothing
both first removes it, and the trend survives, so it is a real difference. For
the record the two renders are about equally noisy - high-frequency rms of 8.48
for ours against 8.916 for shipping, so if anything ours is the cleaner of the
two.

An earlier version of this note also claimed the darkest pixels come out
*brighter* in ours, at a ratio of 1.0657, and read that as energy being moved
from the bright end to the dark end. That rested on 81 pixels and does not
survive smoothing, which leaves too few dark pixels in this scene to bucket at
all. The monotonic loss is the finding; the redistribution is not.

**The light tree is not it either, and this is worth stating because it looks
like a good candidate.** Many-light sampling changes the per-sample radiance
distribution, which is exactly the kind of thing being hunted here. But it landed
in Blender 3.5, not 4.x: `src/scene/light_tree.cpp` is present on the 3.5 branch
and `use_light_tree` defaults to true there as well. Both builds render with it
on, so it cannot be the difference. Ruled out by reading rather than by
measuring, which is cheaper and in this case just as conclusive.

One caveat on the switch that turns it off. RhinoCycles does now apply its
`UseLightTree` setting - a line was added for that during this port, because the
native session init hardcoded the tree to on and the setting had never reached
Cycles at all. A run with `CCYCLES_NO_LIGHT_TREE=1` then hung, spinning one core
for twenty minutes without ever starting a session. Do not read that as the
switch being at fault: the probe's own "forcing use_light_tree off" line never
appeared in the log, and neither did the session's scene dump, so the hang
happened before the probe could have run. It resembles the pre-session hang the
smoketest sees. Unexplained, one occurrence, and a rerun without the switch
rendered normally.

Energy leaving the bright end is what clamping does, and this scene clamps:
`clamp_direct` and `clamp_indirect` are both 3, which `Integrator` multiplies by
3 to give a kernel limit of 9. Neither the clamp values (both builds use
RhinoCycles' defaults, and neither settings file overrides them) nor the code
changed - `film_clamp_light` and the times-three scaling are the same in 3.5 and
5.2, checked line by line. So clamping is not the fault. It is the amplifier: it
is biased by construction, so anything that makes per-sample radiance spikier
turns into a brightness-graded loss at a clamp value nobody touched.

That points the search at what changed in per-sample radiance for light coming
from the background, and `CCYCLES_NO_CLAMP=1` exists to take the amplifier out of
the picture while looking.

One thing to be careful of when reading a region map of this scene: the
middle-left ninth measures 0.8691, much the worst of the nine, which reads like a
shadow problem. It is not - that region contains no dark pixels at all. It is the
brightest part of the floor, and it is consistent with the table above rather
than an exception to it.

### Suspects eliminated by reading

Each of these looked capable of producing a few percent on lit surfaces while
leaving a directly-viewed background alone, and each turned out to be identical
between the two versions. Recorded so nobody pays for them twice.

- **Shadow-catcher compositing.** This scene really does use it -
  `approx_shadow_catcher` is on and all three shadow-catcher passes are written -
  and the composite is what produces the floor while the background comes
  straight through, which is exactly the shape of the difference.
  `film_calculate_shadow_catcher` and the matte-with-shadow path are the same in
  3.5 and 5.2 apart from `const` placement.
- **Adaptive sampling.** It is on, so pixels carry different sample counts and
  are normalised individually, and convergence correlates with brightness. The
  convergence test and its error normalisation are unchanged.
- **The new per-pass `scale`.** 5.2's `film_get_scale` returns
  `kfilm_convert->scale / sample_count` where 3.5 hardcoded `1.0f /
  sample_count`, which looks alarming. `PassInfo::scale` is 1.0 for everything
  except the timing pass, so `combined` behaves as before.
- **Clamping itself.** `film_clamp_light` and the times-three scaling from scene
  value to kernel limit are unchanged, and both builds take the same clamp
  values from RhinoCycles' defaults.
- **Denoising.** Off in these renders - `log_kernel_features: Use Denoising
  False` - so it cannot be smoothing one and not the other.

## The GPU never rendered, and why that poisoned everything

**`RhinoCyclesKernelCompiler.exe` was missing from the plug-in output.** It is in
`Rhino.sln` and three other solutions, so a full build produces it - but every
build in this investigation was a single project (`ccycles.vcxproj`,
`RhinoCyclesCore.csproj`) invoked directly, and the tree kept a stale
`RhinoCyclesKernelCompiler.dll` from the day before with no apphost beside it.

The failure is entirely silent. RhinoCycles writes one `.task` file per render
into `<data>/gpus/`, launches the compiler that is not there, never gets kernels,
so `KernelCache` stays empty, the GPU never becomes ready, and the render either
falls back to the CPU or stalls in `new ccl::Session`. Nothing says "the GPU is
unavailable".

What it invalidates:

- **The renders that "never started".** Two of five runs stalling for 20-30
  minutes was this, not a hang in the port. Note the earlier withdrawal of that
  claim was wrong in the other direction: those runs really were stuck, just not
  for the reason I first guessed and not for the reason I then said instead.
- **"CPU and HIP agree to within the noise floor."** Both sides of that
  comparison ran on the CPU - the device set to `-1` fell back - so it measured
  CPU against CPU and says nothing about HIP. Withdrawn.
- **"The 4.2% difference is not the device."** That rested on the parity
  measurement above. Shipping's settings carry `SelectedDeviceStr=0` and shipping
  has its kernel compiler, so shipping may well have been rendering on the GPU
  while ours rendered on the CPU. The whole difference needs re-measuring with
  both on the same device before any of its conclusions stand.

Checks worth running before trusting a render:

    ls src4/bin/Debug/Plug-ins/RhinoCyclesKernelCompiler.exe   # must exist
    ls "<data>/gpus/"*.task                                    # should not just accumulate
    ls "<local>/RhinoCycles/KernelCache"                       # empty means no kernels

## Reading the RhinoCycles log

This is how the above was found without a debugger, and it is the first place to
look. RhinoCycles writes `RhinoCycles<timestamp>-<pid>-<salt>.log` into its own
`data` directory next to `settings`, and a dedicated thread flushes it every
200ms - so the log of a stalled run is on disk and current, even while the
process is wedged.

Set `VerboseLogging` to `True` in `settings-Scheme__Default.xml` for the full
trace. The stage lines are coarse but enough to bisect: `RenderWithCycles entry`,
`CreateWorld`, `ModalRenderEngine.Renderer entry`, `CreateSession`,
`ApplyEnvironmentChanges`, `ConvertLight`, `ApplyMeshChanges`.

The trick that located this fault was comparing file *sizes*. Every stalled run's
log was exactly 5414 bytes and every good one was 42516 or more, so they diverged
at a single point - the last line being `ModalRenderEngine.Renderer CreateSession`
with no `Created session ...` after it, which puts the stall inside
`new ccl::Session` and therefore in device creation.

## Prior attempts at this port

`PORT-HISTORY.md` says where the earlier 4.x work lives, what it does and does
not save (measured, not estimated), and where Nathan's csycles generator is and
what reviving it costs. Read it before rewriting anything on the Rhino side.

## Gaps that are decisions, not bugs

`PORTING-GAPS.md` lists what works differently from shipping and is waiting on a
choice rather than on a diagnosis: two principled inputs that csycles accepts and
silently drops, three node types that are not registered and have no Rhino
callers, and the 4.2% difference described above.

## The static audits

All the checks run from one entry point:

    powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1            # audits only
    powershell -ExecutionPolicy Bypass -File tools/run_checks.ps1 -Render    # and the renders

It exits non-zero if anything that ran failed, so it can gate a build. The
audits need no build and no Rhino and take about a second; `-Render` needs a
built Rhino and takes minutes.

Individually:

    python tools/audit_sockets.py               # node types, socket names, socket types
    python tools/audit_sockets.py --unexposed   # what Cycles offers that csycles does not
    python tools/audit_enums.py                 # enum members and values against Cycles
    python tools/audit_svm_nodes.py             # add_node_packed on stock node types

Each exits non-zero on a real problem, so any of them can gate a build. Between
them they cover the three ways this port has silently drifted: a renamed or
retyped socket, a renumbered enum, and a stock SVM node emitted in Rhino's packed
layout.
