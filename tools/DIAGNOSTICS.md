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
| `CCYCLES_NO_LIGHT_TREE=1` | Force the old light distribution. Note RhinoCycles never sets `use_light_tree`, so this only bites where ccycles does. |
| `CCYCLES_BG_SKY_FROM_COLOR=1` | Force `sky_color_or_texture`'s Fac to 0, taking the environment image out of the skylight path. |

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

**It is graded by brightness, and dark pixels are brighter rather than darker.**
Bucketing by the shipping render's luminance:

| shipping luminance | ours/shipping |
| --- | --- |
| 43-84 | 1.0657 |
| 85-127 | 0.9990 |
| 128-170 | 0.9936 |
| 171-212 | 0.9752 |
| 213-255 | 0.9589 |

That rules out both of the cheap explanations. A global gain would be flat across
every band; a gamma would pull the midtones off worst and both ends toward 1.
This does neither - it grows monotonically with brightness, and the darkest band
overshoots.

Energy leaving the bright end while the dark end gains is what clamping does, and
this scene clamps: `clamp_direct` and `clamp_indirect` are both 3, which
`Integrator` multiplies by 3 to give a kernel limit of 9. Neither the clamp
values (both builds use RhinoCycles' defaults, and neither settings file
overrides them) nor the code changed - `film_clamp_light` and the times-three
scaling are the same in 3.5 and 5.2, checked line by line. So clamping is not the
fault. It is the amplifier: it is biased by construction, so anything that makes
per-sample radiance spikier turns into a brightness-graded loss at a clamp value
nobody touched.

That points the search at what changed in per-sample radiance for light coming
from the background, and `CCYCLES_NO_CLAMP=1` exists to take the amplifier out of
the picture while looking.

One thing to be careful of when reading a region map of this scene: the
middle-left ninth measures 0.8691, much the worst of the nine, which reads like a
shadow problem. It is not - that region contains no dark pixels at all. It is the
brightest part of the floor, and it is consistent with the table above rather
than an exception to it.

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

These need no build and no Rhino:

    python tools/audit_sockets.py               # node types, socket names, socket types
    python tools/audit_sockets.py --unexposed   # what Cycles offers that csycles does not
    python tools/audit_enums.py                 # enum members and values against Cycles
    python tools/audit_svm_nodes.py             # add_node_packed on stock node types

Each exits non-zero on a real problem, so any of them can gate a build. Between
them they cover the three ways this port has silently drifted: a renamed or
retyped socket, a renumbered enum, and a stock SVM node emitted in Rhino's packed
layout.
