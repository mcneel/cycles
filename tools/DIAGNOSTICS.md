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

## An intermittent spin before the session starts

Two of three measurement runs today never rendered. Rhino came up, the MCP
listener answered, the document loaded, and then one thread span at 100% while
the other 124 waited - 268 seconds of CPU on a single thread and climbing, with
no further output.

It is worth knowing exactly how far it gets, because that bounds the search.
`cycles_debug_scene_stats` runs on the line immediately before
`session->start()`, and its output never appears. So the spin is *before* the
Cycles session starts, which puts it in the Rhino-side conversion of the scene
rather than in Cycles' shader compilation or device update. That is despite the
smoketest reporting its own hang as "Updating Shaders", which happens inside
session start - so the two may not be the same fault, and neither should be
assumed to be the other.

Whether a switch in this document causes it is **open**, and an earlier version
of this section got the reasoning wrong. It said the switches were in the clear
because neither probe's own diagnostic line ever reached the log, so neither
could have run. That does not follow: a thread stuck *inside* the diagnostic
call, or anywhere after it in the same function, leaves exactly the same absence.

What the runs actually say is thinner than that. Three attempts: one with no
switch beyond `CCYCLES_DIAG_LOG`, which rendered; one under
`CCYCLES_NO_LIGHT_TREE`, which span; two under `CCYCLES_NO_CLAMP`, which both
span. `CCYCLES_DIAG_LOG` on its own is fine - the successful run used it and
produced a full scene dump. So the correlation with the two probes is real but
rests on three runs, and both probes are read inside integrator setters that
then call `ccycles_diag`, which makes the probe body and the diagnostic call
equally suspect.

`ccycles_diag` is worth a look if this is chased further. It is `static inline`
in a header with function-local statics, so every translation unit gets its own
`diag_file` and opens the log separately, and the `diag_file_tried` check is an
unguarded race between threads because both statics are constant-initialised and
so carry no thread-safe-init guard. None of that is a spin on inspection, but it
is not code to trust while investigating a hang either.

Practical consequence for anyone measuring: check that the run produced a
`stats: geometry=...` line before believing a timeout, and expect to repeat
runs. There is no diagnosis here yet - no debugger is installed on this machine,
and one spinning thread with no symbols is as far as it went.

## Measuring, rather than looking

`tools/render_regression.ps1` renders fixed scenes and compares them against
stored images. Two numbers make its threshold meaningful: the same build twice
differs by a mean of 0.03 per channel out of 255, and the gap to shipping Rhino
on the same scene is around 10. Anything above 1 is real.

Before believing any difference between two builds, render one of them twice and
measure that first. It is the only way to know what your noise floor is, and here
it is small enough that a 0.05 result means the renderer is deterministic rather
than that the comparison failed.

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
