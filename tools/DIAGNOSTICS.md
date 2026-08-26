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
objects, per-mesh vertex bound and object transform, per-mesh shader slots,
lights with their strength and transform, the integrator's bounces and switches,
the film state, and every shader graph with the value Cycles holds for each
unlinked input.

The per-mesh **bound and transform** are what make a wrong-looking render
tractable, because a render whose geometry appears collapsed is usually shaded
wrong rather than built wrong. On `tyreel_neon_testv9.3dm` the dump said all 227
meshes sat inside the document bounding box, every object transform was identity
and nothing was NaN — which retired vertex stride, `packed_float3` overruns, UV
buffer overflow and triangle index range in one run, and pointed the search at
the shader graphs where the fault actually was. Note that RhinoCycles bakes world
coordinates into the vertices and leaves the transforms identity, so a
non-identity transform in this dump is itself news.

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
| `CCYCLES_FORCE_OPAQUE=1` | Cut the Alpha and Transmission Weight links on every principled node and set them opaque. Answers "is this material invisible because it is transparent?" in one run. |

`CCYCLES_FORCE_OPAQUE` earned its keep by saying **no**. Material that renders as
nothing is either transparent or transmissive, and Rhino builds both from long
math chains, so forcing them opaque looked like the cheap first cut. It changed
nothing on `tyreel_neon_testv9.3dm` — 15 principled nodes touched, 20 links cut,
identical render — which took the whole alpha chain out of the search and left
the texture path. Keep it for the next material that disappears.

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

**It is one scene, not the port.** The second regression case,
`Material_Scene_Final.3dm` at 600x600, renders **identically** on 5.2 and
shipping: mean absolute difference 0.013 per channel against a run-to-run noise
floor of 0.036, luminance ratio 1.0000, and flat across every luminance band
(0.9996 to 1.0000). So Cycles 5.2 and 3.5 agree exactly on a real scene, and the
4.2% belongs to `rdk_material_scene.3dm` specifically.

That is the single most useful measurement in this file, and it was cheap - two
renders of a scene the harness already knew about. Take it before assuming a
difference is systemic.

**What distinguishes the two scenes is an image texture.** The RhinoCycles log
for the material preview scene names `18_percent_greycard.jpg`; the scene that
matches has no image asset in its log at all. That much stands, and it is where
to look next.

**But the colorspace explanation for it is refuted.** The warning our build
emits once,

    detect_known_colorspace: Colorspace  not found, using scene linear instead

reads like a texture arriving with no colorspace and losing its sRGB decode. It
is not. `u_colorspace_auto` is a **default-constructed, empty** `ustring` - in
5.2 at `util/colorspace.cpp:24` and identically in 3.5 - so an empty request *is*
the auto sentinel, and every `colorspace == u_colorspace_auto` test matches it.

Instrumenting `detect_known_colorspace` to name every resolution settles it. The
whole scene resolves exactly two images:

    requested='' file_hint=''                    is_float=1
    requested='' file_hint='srgb_rec709_scene'   is_float=0

The second is the byte JPEG, and its own file hint says sRGB, so 5.2's fallback
gives it `u_colorspace_srgb` - it **is** decoded. The first is float with no hint,
which resolves to scene linear, and for an EXR environment that is correct.

That also disposes of the bracket numbers innocently, which had looked like
evidence for a missing decode. Forcing every texture to `srgb` overshoots
shipping at 1.0642 and forcing none undershoots at 0.9577 with shipping between,
and the reason is simply that both builds already decode *some* textures and not
others. No regression is needed to produce that spread, and it should never have
been read as pointing at one.

So texture colorspace is now eliminated twice: once by reasoning about builtin
images, and once per-image with the resolver instrumented. The scene-specificity
is the live lead; the colorspace is not it.

**It is not the shadow catcher's visibility.** This branch changed how a shadow
catcher is hidden from reflections, from `AllVisibility & ~PathRay.Reflect` to
`& ~PathRay.Glossy`, because `Reflect` stopped being a visibility bit in 5.x. It
is a real behavioural change, it is scene-specific in exactly the right way -
only scenes with a shadow catcher see it, and the differing scene has one at
`vis=119` - and it is mine, which made it the best candidate left.

It is not the cause. Giving the catcher full visibility instead, verified in the
scene dump as `vis=127` on that object, leaves the difference at 9.957 against
9.956. Unchanged. That also matches what the enums predict: old `Reflect` was
`1 << 1` and 5.2's bit 1 is `Transmit`, with reflections now falling under
`Glossy`, so the mapping is right, and either way both versions keep *diffuse*
visibility - which is what lights the floor.

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
`clamp_direct` and `clamp_indirect` are both 3, which `Integrator` multiplies by 3
for a kernel limit of 9. That made clamping the obvious mechanism - not as the
fault, since `film_clamp_light` and the scaling are identical between versions,
but as an *amplifier*: clamping is biased by construction, so anything making
per-sample radiance spikier becomes a brightness-graded loss at a clamp value
nobody touched.

**That is wrong, and it was worth the one render to find out.** With
`CCYCLES_NO_CLAMP=1`, confirmed applied both by the probe's own log lines and by
the integrator dump reading `clamp_direct=0.000000 clamp_indirect=0.000000`, the
difference against shipping is **9.957** against 9.956 with clamping on. No
effect whatsoever. Clamping does not bite on this scene at this exposure, so it
cannot be amplifying anything.

So the brightness grading is not a clipping artefact. It is a real difference in
how much light arrives, growing with the amount of light arriving - which is a
harder thing to explain and rules out the whole class of "bias knob" theories.

One tooling note from the same experiment. Setting `SampleClampDirect` and
`SampleClampIndirect` in `settings-Scheme__Default.xml` changed nothing, and that
is not evidence about clamping - it is that `ModalRenderEngine` wraps document
settings in a quality preset (`Draft`/`Good`/`Final`/`Low`) unless
`UseDocumentSamples` is set, and the preset supplies its own values. Use the
environment probes rather than the settings file when the point is to change what
Cycles receives, and confirm from the integrator dump that it landed.

### Suspects eliminated

Each looked capable of taking a few percent off lit surfaces while leaving a
directly-viewed background alone. Recorded so nobody pays for them twice.

**Identical between the two Cycles versions, checked by reading both:**

- **Shadow-catcher compositing.** This scene really does use it -
  `approx_shadow_catcher` is on and all three shadow-catcher passes are written -
  and the composite is what produces the floor while the background comes
  straight through, which is the exact shape of the difference.
  `film_calculate_shadow_catcher` and the matte-with-shadow path differ only in
  `const` placement.
- **Adaptive sampling.** It is on, so pixels carry different sample counts and
  are normalised individually, and convergence correlates with brightness. The
  convergence test and its error normalisation are unchanged.
- **The new per-pass `scale`.** 5.2's `film_get_scale` returns
  `kfilm_convert->scale / sample_count` where 3.5 hardcoded `1.0f /
  sample_count`, which looks alarming. `PassInfo::scale` is 1.0 for everything
  except the timing pass.
- **Clamping.** `film_clamp_light` and the times-three scaling from scene value to
  kernel limit are unchanged, and both builds take the same clamp values from
  RhinoCycles' defaults.
- **The light tree.** It landed in Blender 3.5, not 4.x - `light_tree.cpp` is on
  the 3.5 branch and `use_light_tree` defaults true there too.
- **Texture colorspace.** Rhino passes builtin float images, so `auto` resolves to
  no decode in both. See the section above.
- **Denoising.** Off in these renders (`Use Denoising False`).

**Ruled out on the Rhino side, by diffing this branch against its merge base:**

- **The device.** Measured properly once the GPU worked: 0.9577 on HIP and 0.9577
  on CPU.
- **Changed engine defaults.** `Settings/DefaultEngineSettings.cs` is purely
  additive on this branch - 166 lines added, none removed - so no default that
  reaches the integrator moved.
- **The sampling pattern.** This branch changed `SamplingPattern.Sobol` to
  `SobolBurley`, which reads like a behaviour change and is a rename: 3.5 has
  `SAMPLING_PATTERN_SOBOL_BURLEY = 0` and csycles has `SobolBurley = 0`.
- **The `BvhLayout.Default` to `Auto` change**, which sits inside `#if LEGACY` and
  is not compiled.
- **The procedural gamma gate.** `Utilities.cs` now skips the gamma decode for
  trees with no image content or with simulated procedurals, which is a real
  colour change of about the right size and shape. It is not the difference:
  it comes from `60f40ce` (RH-92750, 2026-07-24) and David Eränen's `93c8441`
  and `4a9ee01` (2026-08-06), all reachable from `origin/rhino-9.x`, and the
  shipping build is `9.0.26226` from 2026-08-14 - so shipping has them too.

What is left is the Cycles version itself, which is the thing being changed. A
version bisect through the 4.4 branches would name the upstream commit; nothing
cheaper has worked.

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

What it invalidated, and where each claim now stands after re-measuring on a
build that really does use the GPU:

- **The renders that "never started".** Two of five runs stalling for 20-30
  minutes was this, not a hang in the port. The earlier withdrawal of the hang
  claim was also wrong: those runs really were stuck, just for neither of the
  reasons I gave at the time.
- **"CPU and HIP agree to within the noise floor" - withdrawn, then confirmed.**
  The original measurement had the device set to `-1` falling back, so it compared
  CPU against CPU and said nothing. Re-run with HIP genuinely active
  (`device_hip_init: Found precompiled kernels` present, which it was not before):
  mean absolute difference **0.036** per channel, worst channel 10, luminance
  ratio 1.0000. So the two devices do agree - now on evidence rather than by
  accident, and at coincidentally the same number.
- **"The 4.2% difference is not the device" - withdrawn, then confirmed.**
  Shipping carries `SelectedDeviceStr=0` and has its kernel compiler, so it may
  have been on the GPU while ours was on the CPU, which would have made the whole
  difference a device artefact. It is not: ours on HIP against shipping is 9.959
  at a ratio of 0.9577, ours on CPU against shipping is 9.955 at the same 0.9577.
  The difference is identical on both devices and the device is genuinely out.

The lesson is not about any of those numbers. It is that a silently disabled GPU
made three separate conclusions unsafe at once, and none of them announced itself
- every render still produced a plausible image.

Checks worth running before trusting a render:

    ls src4/bin/Debug/Plug-ins/RhinoCyclesKernelCompiler.exe   # must exist
    ls "<data>/gpus/"*.task                                    # should not just accumulate
    ls "<local>/RhinoCycles/KernelCache"                       # empty means no kernels

## How long a run takes

A full `render_regression.ps1` cycle against `rdk_material_scene.3dm` - kill any
running Rhino, launch a Debug build, wait for the MCP port, render 32 samples,
save - takes **six to seven minutes** on this machine. Budget that before
concluding anything about a run that has not finished; killing a working render
at the six-minute mark and calling it a hang is a mistake already made here.

Two things look like a hang and are not:

- **The diag log stops at device enumeration for the whole render.** The scene
  dump comes from `cycles_debug_scene_stats`, which runs on the line immediately
  before `session->start()`, so its absence is the normal state until a session
  exists - which, during a render, is most of the time.
- **One thread pinned at 100% with the rest waiting.** That is what the host side
  of a GPU render looks like.

A genuine failure shows up as a clean timeout: `_Render` hitting the script's
1800-second limit with `Rhino call "run_command" failed: The request was aborted`.

**And check the timestamp of any image you collect.** `render_regression.ps1`
deletes its output before rendering rather than after, precisely because a render
that throws would otherwise leave the previous good run's image sitting at the
expected path, looking exactly like a result. A wrapper script here duly copied
one out as a result, timestamped before its own run had started.

## Two facts about the test scene, and a dead end

**`rdk_material_scene.3dm` is lit only by its environment.** The RhinoCycles log
says so plainly:

    ConvertLight Distant: enabled=False intensity=1 strength=0 ... radius=0

The one non-environment light in the scene is disabled with zero strength. That
explains why every measurement here is a measurement of environment lighting, and
why this scene cannot separate "the environment lights surfaces differently" from
"light transport differs generally" - there is no other light to compare against.
Answering that needs a scene lit by a local light with the skylight off.

**Scripting this Debug build does not work.** Both `run_python` over MCP and
`-_RunPythonScript` at startup fail to produce output - MCP hits its fixed
300-second cap on even `print("hello")`, and a startup script produced nothing
after ten minutes while Rhino sat idle at 56 seconds of CPU, so it is waiting
rather than working. After a timed-out `run_python`, MCP reports "Rhino is already
running a command" and no further command gets through, so the session is spent.

That matters because authoring a test scene is the obvious way to answer the
question above, and the obvious route to authoring one is closed. Build the scene
by hand in a Release Rhino and commit the .3dm, rather than trying to script a
Debug one.

## Building

Build the **solution**, not single projects:

    MSBuild src4/BuildSolutions/Rhino.sln /p:Configuration=Debug /p:Platform=x64 /m:4

Cycles itself is only built in the `Debug+Cycles` and `ReleaseDebuggable+Cycles`
configurations; the plain ones use the prebuilt payload, which is what almost
everyone wants. For the rest of it, including why native debugging needs a local
Debug build, see *Building* in `RHINO-CYCLES-5.md`. A full incremental pass is about seven minutes here and reports
0 warnings and 0 errors.

Two things learned the hard way:

- **Single-project builds leave the tree inconsistent, in both directions.**
  Building `ccycles.vcxproj` alone updates `big_libs` but not the plug-in output,
  because `RhinoCyclesCore.csproj` is what copies from there - and building
  `RhinoCyclesCore.csproj` alone copies whatever is already in `big_libs` without
  rebuilding Cycles, even in a `+Cycles` configuration. Editing a Cycles source file
  therefore needs both, in that order, or just the solution. Building
  `RhinoCyclesKernelCompiler.csproj` alone needs
  `/p:SolutionDir=<repo>/src4/BuildSolutions/`, because that copy step resolves
  `$(SolutionDir)..\..\big_libs` and the solutions live in `BuildSolutions`, not
  `src4`. Skipping it is what disabled the GPU above.
- **Check the binary is newer than your edit before believing a render.** Stale
  output has produced three wrong conclusions here: a Cycles change that never
  deployed, a missing kernel compiler that silently disabled the GPU, and a
  RhinoCyclesCore build that failed because a leftover Rhino held
  `RhinoWindows.dll` while the script rendered anyway. A build that fails on a
  file lock still leaves the previous binary in place, and the render looks
  perfectly normal. Kill Rhino first, then compare timestamps and abort if the
  binary is older than the source:

      if ((Get-Item $dll).LastWriteTime -lt (Get-Item $src).LastWriteTime) { exit 1 }

- **`error C1001` from MSVC is usually transient.** A full build died with an
  internal compiler error in `Rhino3Utilities.cpp`, reporting a garbled compiler
  filename, which points at memory corruption in the compiler rather than at the
  code. The same build passed on retry with `/m:4`. Retry before investigating,
  and lower the parallelism.

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
