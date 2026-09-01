# Known gaps in the 3.5 -> 5.2 port

Things that work differently from shipping Rhino 9 WIP and are not bugs to be
found - they are decisions waiting to be made. Each one says what was measured
or read, what the safe current behaviour is, and what the choice is, so nobody
has to rediscover the shape of the problem before making it.

See `DIAGNOSTICS.md` for the switches and tooling used to establish any of this.
See `PORT-HISTORY.md` for where the earlier 4.x attempts live and what they
already decided.

## Two principled inputs are silently dropped

Blender 4.0 removed `Subsurface Color` and `Transmission Roughness` from the
Principled BSDF. csycles keeps both sockets marked `Retired`, which is a real
no-op: `SocketBase.Connect` returns early and `ShaderNode` skips the socket when
pushing values. Nothing throws and nothing reaches Cycles.

RhinoCycles still writes to both, so two Rhino inputs have no effect: the PBR
subsurface scattering colour, and the PBR opacity roughness (plus
`RefractionRoughness` on the standard path).

### What the 4.4 attempt decided

Worth reading before deciding again, because this ground has been covered twice.
On `origin/build/lars/UpdateToVersion44_RebaseOn9`, `Shaders/RhinoFullNxt.cs`:

    768   PbrRoughness            -> principled.ins.Roughness
    778   PbrSubsurfaceColor      -> principled.ins.BaseColor   (guarded on radius and weight > 0)
    788   PbrTransmissionRoughness -> principled.ins.Roughness

which is the Blender-consistent direction: subsurface takes its colour from base
colour in 4.x, and transmission takes its roughness from the main roughness. On
the standard path the same two were left as TODOs rather than decided:

    1119  // TODO principledbsdf117.ins.SubsurfaceColor.Value = ...
    1133  // TODO principledbsdf117.ins.Roughness.Value = part.RefractionRoughness;

### What actually survived in 4.4

Both of those mappings connect a *second* graph to an input that is already
taken - `BaseColor` at line 757 by the base-colour-with-AO graph, and
`Roughness` at line 768 by `PbrRoughness`. `ShaderGraph::connect` refuses that:

    if (to->link) {
      LOG_WARNING << "Graph connect: input already connected.";
      return;
    }

so the **first** connection wins and the later one is dropped. It is
unconditional rather than occasional, because `PbrGraphForSlot` always builds a
`ValueNode` or `ColorNode` and connects it whether or not the slot is switched
on, so `Roughness` and `BaseColor` are never free by the time lines 788 and 778
run.

What survived in 4.4 was therefore the surface roughness and the ordinary base
colour, with the opacity-roughness and subsurface-colour mappings silently
dropped. Two things are worth being precise about, because both were stated
wrongly at first: `ccycles` reports the drop through `ccycles_diag` ("input %s
already connected"), and the only assertion involved is csycles-side and
`#if DEBUG` only - `ShaderGraph::connect` merely logs a warning. So this is not a
crash and is not what made that branch unstable.

### The decision: keep it

Asked which of Rhino's two roughness controls should survive and how the SSS
colour should reach base colour, the answer was "what survived in 4.4" and "same
as 4.4 for now". Since 4.4 effectively kept the surface roughness and ignored the
subsurface colour, and our `Retired` sockets do exactly that, **the current
behaviour is the decision and no code change follows.** The difference is only
that we drop the two inputs deliberately and visibly rather than by losing a
race between two connections.

Two Rhino controls consequently have no effect and should eventually be hidden or
marked in the UI rather than silently ignored:

- PBR opacity roughness, and `RefractionRoughness` on the standard path
- PBR subsurface scattering colour

Revisiting means combining rather than connecting twice - mixing the subsurface
colour into the base-colour graph before it reaches `BaseColor`, and choosing a
single roughness. Both are user-visible and neither is forced by 5.2, which is
why they can wait.

## A parameter exposed as both a member and a socket loses the member

`Shader.WriteDataToNodes` calls `SetEnums()`, then `SetDirectMembers()`, then
`SetSockets()`. `SetSockets` pushes every non-retired input socket, including ones
nobody assigned, because a socket holds its C# default from construction. So where a
node carries the same Cycles parameter both ways, the socket write lands last and
silently overwrites the member. Nothing throws; the value simply does not arrive.

Found 2026-08-29 on `BumpNode.invert`. RhinoCycles set the member
(`RhinoFullNxt.cs`, base and clearcoat bump), and `BumpNode.ParseXml` set it too, so
**negative bump amounts never inverted and XML-loaded bumps lost invert as well**.
Nathan's 4.4 branch had already moved the RhinoCycles call sites to
`ins.Invert.Value`; reverting that branch brought the member form back, which is how
a fixed bug returned unnoticed.

**Fixed** by making the property a view onto the socket, so both spellings and the XML
path stay correct: `public bool Invert { get => ins.Invert.Value; set => ... }`.
RhinoCycles `96f682a` also moved its two call sites to the socket.

`tools/audit_member_socket_clash.py` now covers this as a fourth drift class in
`run_checks.ps1`. It detects a delegating property and does not report it, so the fix
above is recognised rather than needing an exception. The one remaining clash,
`MusgraveTexture.dimension`, is listed as accepted for the same reason the three
unregistered node types are: no reachable caller.

## A bump texture blacked out the whole surface

**Fixed.** `NODE_SET_BUMP` had lost its `break` in the SVM interpreter
(`kernel/svm/svm.h`) and fell through into `RHINO_NODE_TEX_COORD`, the case Rhino
splices in beneath it.

The fall-through runs the texture coordinate handler immediately after the bump node:
it reads a packed node from the *current* instruction offset, so it misinterprets
whatever node follows, writes into stack slots belonging to other nodes - including the
one the bump had just written - and advances the offset wrongly. Every bump-mapped
surface went black. `Brian25YearRhinoGlas.3dm` rendered its tabletop at mean 7.2 against
shipping's 62.2; with the `break` restored it renders at 59.7.

**Why it cost days.** The falling-through node looks innocent. It computes and stores
its value correctly, and the damage happens a few instructions later, so every
experiment aimed at the bump node came back identical: zero strength, a NaN guard,
changing the no-feature fallback, passing the input straight through, and finally
storing a hardcoded `sd->N` - five different edits, five byte-identical renders. That
should have been read much sooner as "this code's output does not reach the image"
rather than as five separate failed hypotheses about its internals.

Everything else measured true and pointed away from the bug: compile-time logging showed
the bump writing stack slot 11 and the principled BSDF reading slot 11, both compiled
into `SHADER_TYPE_SURFACE`, in the right order. The wiring was never wrong.

`tools/audit_svm_dispatch.py` now checks every `SVM_CASE` terminates, as a fifth static
drift class in `run_checks.ps1`. It was verified against the broken tree before being
committed: it reports exactly this case and nothing else.

**Relevant to the next version bump.** This is the failure mode of splicing Rhino's
cases into upstream's `switch`: an upstream edit near an inserted case can silently
remove the `break` that separates them. Run the audit after merging upstream.

## Light direction: two competing fixes, only one should exist

Open, and the choice matters more than it looks.

`ccycles/light.cpp` flips the light basis for area lights only:

    if (type == ccl::LIGHT_AREA && have_dir) { z = -z; }

The never-merged `origin/build/lars/UpdateToVersion44_RebaseOn9` line instead flips in
C#, in `ShaderConverter.ConvertLight` right after `strength *= enabled`, for **every**
light type:

    dir *= -1.0f;

Applying both double-negates area lights, so this is a choice, not two fixes.

Evidence that the global form may be the right one: `smoketest/README.md` states
`SMOKE_SPOTZ=1` lights the quad and `-1` does not. The measured sweep showed the
reverse - only `-1` lit it. That inversion is unexplained under the area-only fix and is
exactly what a missing global flip produces.

**The experiment**, if picking this up: add the C# flip, remove the `LIGHT_AREA` guard,
then check three things against shipping - the area-lit model above, the `SMOKE_SPOTZ`
sweep (expect `+1` to light the quad, matching the README), and a point or directional
light. If spots return to the documented behaviour, replace the ccycles flip rather than
keeping it alongside.

## The background is the top colour where shipping uses the bottom

Open. `SimpleVaseTest.3dm` renders a white background in dev and a grey one in
shipping. Nothing else in that image differs - vase, table, light pool, shadow and
caustics all match - but it is the darkest scene in the set, so a blown background
dominates its mean and made it look like the worst regression of the six.

Measured on the float output, not the 8-bit one (the 8-bit clips at 255 and hides all
of this):

| | background, linear |
| --- | --- |
| shipping | 0.332 |
| dev | 0.997 |
| dev, forced to use `Color2` | 0.358 |

The document is `BackgroundStyle.SolidColor` with top white and bottom (160,160,160),
and **both** builds report exactly that. Dev renders the top colour; shipping renders
the bottom one. Forcing dev's `bg_color_or_texture.Color1` to `Color2AsFloat4`
reproduces shipping to within 8%, so the bulk of the difference is simply which input
that Mix node takes. The residual looks like a transfer-function difference: 0.6275
raised to 2.2 is 0.358, the sRGB curve gives 0.347, shipping gives 0.332.

**What this is not.** Verified, so it does not need redoing: the RhinoCycles background
code is byte-identical to `origin/rhino-9.x` (the only diff in those four files is an
unrelated shadow-catcher visibility line); both Rhinos report identical background
settings and no RDK background environment; the environment texture resolves to a real
file and is not a load failure; the `mix` node's sockets (`fac`, `color1`, `color2`)
match between csycles and 5.2 exactly; and re-enabling
`CurrentBackgroundShader?.Reset()` from the 4.4 branch changes nothing (it does run).

So identical inputs and identical C# produce different backgrounds, which puts the
difference inside Cycles' evaluation of that graph - `Fac` is 0 in dev, and a Mix with
`Fac` 0 must return `Color1`. For shipping to show `Color2`, its `Fac` would have to be
1, meaning `HasBgEnvTexture` is true there.

**Why it is not resolved.** Confirming that needs shipping's internals, and there is no
way to see them: `CCYCLES_BG_TAP` and the other switches are additions on this branch,
so the shipping build has none of them, and csycles is not in this repository on
`rhino9_cycles35` (it lives in the separate CCSycles repo). Building the shipping branch
with the diagnostics added was attempted and is blocked: `build_cycles_for_rhino.ps1`
requires VS2019 BuildTools, which is not installed here, and drives a CUDA build on an
AMD machine.

**Both builds build the same graph.** Shipping can dump its own background graph -
`DumpEnvironmentShaderGraph` in the RhinoCycles settings makes both builds write
`%USERPROFILE%/rhinobg_<id>.dot`, and it is present on `origin/rhino-9.x` as well as
here. That is the only way to see inside the shipping build, which has none of the
`CCYCLES_` switches.

Comparing the two dumps with `dotdiff.py` (in the render harness, normalises the
per-run pointers away): **55 nodes on both sides, same names, same values.**
`bg_color_or_texture` is `Fac 0, Color1 (1,1,1), Color2 (0,0,0)` in *both*.
`skylight_strength_factor` is identical in both. The only differences are socket UI
labels that upstream renamed - `Image` to `Color`, `R/G/B` to `Red/Green/Blue` - which
are cosmetic, since connections resolve on the internal name.

So identical inputs, identical C#, identical graphs, different pixels. The difference
is in how the graph is **evaluated**, not how it is built.

**The ratio is exactly 3.00** (0.9974 against 0.3322), which looks like three channels
being summed where one is wanted, or a strength applied once against three times -
not a colour difference. That is the thread to pull: the background closure and the
film/exposure path, not the shader graph.

Note the earlier guess in this section - that dev ignores the environment because
`BackgroundFill` is `SolidColor` - is **wrong**; shipping computes the same
`BackgroundFill` from the same document and still renders differently.

### Measured inside shipping, with a tap

The shipping branch **can be built here**: `origin/rhino9_cycles35` configures and
builds with VS2022 (no VS2019 needed, despite what `build_cycles_for_rhino.ps1`
hardcodes) given the 3.5-era libs at `<parent>/lib/win64_vc15`. Build
`--target ccycles --config RelWithDebInfo`; a Debug build is the wrong CRT for the
installed Rhino and makes it fail to render. `CCYCLES_BG_TAP` ports across with one
change: `background->get_shader()` returns `Node*` there and needs the cast.

With that DLL swapped into the installed Rhino (back the original up first), the same
scene reproduces exactly, and tapping gives:

| tapped node | shipping | dev |
| --- | --- | --- |
| `bg_color_or_texture` | 0.698 | 0.996 |
| `gradient_or_other` | 0.698 | 0.996 |
| final, untapped | 0.332 | 0.997 |

Dev is flat from the first node to the output. Shipping is flat at 0.698 through the
colour chain and then drops to 0.332 at the last stage, so **shipping applies a further
factor of about 0.48 between `gradient_or_other` and `final_bg` that dev does not**.
That is the thing to find; the colour chain itself is not where they diverge.

### The mix stage, measured

Tapped in the instrumented shipping build, reading a background-only patch out of the
`.hdr` per channel (patch 180,0 40x20 at 400x240, 5 samples - a background pixel is one
shader evaluation with no path variance, so it converges at sample 1; the 5-sample
untapped read 0.3331 matches the 250-sample sweep's 0.332):

| tapped node | shipping | dev |
| --- | --- | --- |
| `mix.Fac` (`if_not_cam_nor_transm_nor_glossyrefl`) | 0.0000 | 0.0000 |
| `mix.Color1` (`refl_bg_or_custom_env`) | 0.6622 | 0.9961 |
| `mix.Color2` (`light_with_bg_or_sky`) | 0.3323 | 0.9968 |
| `mix.Color`, tapped | 0.3331 | - |
| untapped | 0.3331 | 0.9965 |

Tapping `mix` reproduces the untapped value exactly, so the tap itself is neutral.
Every channel is equal in every one of those reads - it is neutral grey, not a channel
dropping out - and the three shipping numbers are 1/3, 2/3 and 3/3 of white.

`Fac` is 0 in both, so the earlier guess that the socket default 0.5 was standing in is
wrong. And with `Fac` 0 the mix must return `Color1`: `MixNode` packs `(fac, c1, c2)`
and `svm_node_mix` unpacks the same order into `interp(c1, c2, t)`, in **both** trees.
Shipping returns 0.3331 where its `Color1` is 0.6622.

### The graphs really are equivalent

The 24 apparent connection differences between the two dumps are all socket renames
(`Image` to `Color`, `R/G/B` to `Red/Green/Blue`) from the separate/combine drift.
Resolved pointer-accurately with the harness's `dotptr.py` - which keeps the two nodes
both named `skylight_strength_factor`, and the three named `maximum`, distinct - the
wiring is identical, including which of the two `skylight_strength_factor` nodes feeds
the background multipliers (the `max`, value 1.0) and which feeds the reflection ones
(the `mul`, value 0.0).

Read literally for a camera ray the graph gives **white**: `mix.Fac` 0 selects
`refl_bg_or_custom_env`, whose `Fac` (`refl_env_when_enabled = mul(1.0, 0)`) is 0 and so
selects `gradient_or_other`, whose `Fac` is a literal 0 and so selects
`factored_bg_color`, which is `bg_color_or_texture`'s literal white through a gamma of
2.2 times 1.0. Dev renders 0.9965. **Dev is faithful to the graph; shipping applies a
factor of 1/3 that the graph does not describe** - and when a tap makes the graph
smaller that factor becomes 2/3 rather than staying put.

Also visible in the literals, and worth keeping: `refl_color_or_texture.Color1` is
0.358654, which is `(160/255)^2.2` - the document's background *bottom* colour at gamma
2.2. `bg_color_or_texture.Color1` is white, the *top* colour. `bg_env_texture` is
`(null)` in both; the style is SolidColor and no environment image is involved.

### Why the dumps could not settle it

`cycles_shader_dump_graph`, which is what the `DumpEnvironmentShaderGraph` setting
calls, runs where RhinoCycles *builds* the graph - before `simplify()`, constant folding
and `finalize()`. Identical dumps therefore only prove both builds construct the same
graph, not that they compile the same one, and 3.5 and 5.2 do not have the same
optimizer. 5.2's `finalize` has also dropped shipping's
`else if (do_simplify) simplify_settings(scene)` branch.

`CCYCLES_DUMP_FINAL=<prefix>` (added to `ShaderGraph::finalize` in both trees, temporary)
dumps the background graph again afterwards. Dev's post-finalize graph is **12 nodes**,
almost all of it folded to constants, and it confirms the reading above: both
`refl_bg_or_custom_env.Color1` and `light_with_bg_or_sky.Color1` are the literal
`(1,1,1)`, their `Color2` the literal `(0,0,0)`, and every gate resolves to 0 for a
camera ray, so all three mixes return white.

Shipping's post-finalize graph is **the same 12 nodes with the same 16 connections and
the same folded constants**. `dotdiff` finds two differing entries and both are the
`light_path` node's output list: dev has a fifteenth output, `Portal Depth`, appended
after `Transmission Depth`. Appended last, so nothing shifts, and both trees'
`LightPathNode::compile` maps outputs to `NODE_LP_*` by name.

So the compiled graph is exonerated too, and that makes the tapped renders damning: the
tap runs in `cycles_session_start`, before `session->start()`, so the tapped graph gets
folded as well - and tapping `refl_bg_or_custom_env` and `light_with_bg_or_sky` compiles
two structurally identical folded graphs (`Color1` literal white, `Color2` literal black,
`Fac` from a light path chain) which render 2/3 and 1/3. For a camera ray both `Fac`
values must be 0 and both must render white. So `Fac` is not 0.

### Root cause: shipping loses a third of the weight and mislabels half the rest

Tapping the `light_path` node measures its outputs directly. `CCYCLES_BG_TAP` accepts
`<node>:<Socket Name>`, and this is the one tap family whose numbers may be compared with
each other: every such render folds to the identical four node graph
(`light_path.<socket> -> convert_float_to_color -> final_bg.Color`), so only the socket
differs. Taps of *different* nodes each fold to a different graph, which is why the
earlier bisect produced numbers that no single model could satisfy.

Every light path output, background patch, 5 samples:

| socket | shipping | dev |
| --- | --- | --- |
| `Is Camera Ray` | 0.3323 | 0.9967 |
| `Ray Depth` | 0.7743 | 0.9967 |
| `Is Shadow Ray` | 0 | 0 |
| `Is Diffuse Ray` | 0 | 0 |
| `Is Glossy Ray` | 0 | 0 |
| `Is Singular Ray` | 0 | 0 |
| `Is Reflection Ray` | 0 | 0 |
| `Is Transmission Ray` | 0 | 0 |
| `Is Volume Scatter Ray` | 0 | 0 |
| `Transparent Depth`, `Diffuse Depth`, `Glossy Depth`, `Transmission Depth` | 0 | 0 |

**This refutes the earlier reading in this section that shipping evaluates the background
three times as camera, glossy+reflection and diffuse.** Glossy, diffuse, reflection and
transmission are all exactly zero in both builds; there are no such contributions.

The two other taps become interpretable once their folded graphs are read. Tapping
`refl_bg_or_custom_env` folds to that node with `Color1` literal white, `Color2` literal
black, and `Fac` still linked to `use_reflect_refract = IsGlossy * IsReflection`. Both of
those are zero for every contribution, so `Fac` is zero for every contribution and the
node yields white for all of them - that tap measures **total weight**, not a colour.
Tapping `light_with_bg_or_sky` folds to `Fac <- non_camera_rays = 1 - IsCameraRay`, so it
yields white only for camera contributions and measures **camera weight** - which is why
it reads identically to the `Is Camera Ray` tap, as it must.

| | shipping | dev |
| --- | --- | --- |
| total weight reaching the pixel | 0.662 | 0.996 |
| of it, camera-flagged | 0.332 | 0.997 |
| of it, carrying no ray type flag at all | 0.330 | ~0 |

Two separate effects, then, not one:

1. Shipping's background pixel receives only about **two thirds** of the weight dev's
   does. A third of the energy is missing outright.
2. Of what remains, **half carries no ray type flag** - not camera, not glossy, not
   diffuse, nothing. Rhino's graph routes anything that is not a camera ray to black, so
   those contributions render as black and the visible result is the camera third alone.

That is the whole 3x: shipping 1/3 against dev 1.0. Dev puts all of its weight into a
single camera contribution, which is why its gating never fires.

### Where the extra contributions come from: the shadow catcher keeps tracing

A tap cannot see who invoked the shader, so shipping's `shade_background.h` was
instrumented directly. `CCYCLES_BG_EVAL_TALLY=<file>` counts every background shader
evaluation by call site and path flag, with mean bounce, MIS weight and path throughput.
Two sites were instrumented: `integrate_background` and `integrate_distant_lights`.

Note for anyone reusing this: the guard has to be `#ifndef __KERNEL_GPU__`. There is no
`__KERNEL_CPU__` in either tree, and using it compiles the whole thing out silently -
the first run produced an empty tally and an otherwise perfect render.

620,000 evaluations over a 400x240 five sample frame, and the shape is unmistakable:

| flag | calls | bounce | mis | throughput |
| --- | --- | --- | --- | --- |
| `0xe0009001` | 77154 | 0.0000 | 1.0000 | 1.0000 |
| `0x6400200a` | 77149 | 1.0000 | 1.0000 | 0.8684 |
| `0x2400600a` | 76927 | 1.0003 | 1.0000 | 0.8688 |
| `0x0400600a` | 374927 | 1.2487 | 1.0000 | 0.5052 |
| (eight more, together under 2%) | | | | |

**`integrate_distant_lights` never evaluates a background light at all** - not one
`distantbg` call - so the earlier guess that the background-as-a-light path was the source
is wrong too.

The three near-identical counts are the answer. `0xe0009001` decodes as
`CAMERA | MIS_SKIP | TRANSPARENT_BACKGROUND | SHADOW_CATCHER_HIT | SHADOW_CATCHER_PASS |
SHADOW_CATCHER_BACKGROUND` at bounce 0, and `integrator_shade_background` ends with:

    if (INTEGRATOR_STATE(state, path, flag) & PATH_RAY_SHADOW_CATCHER_BACKGROUND) {
      /* Special case for shadow catcher where we want to fill the background pass
       * behind the shadow catcher but also continue tracing the path. */
      INTEGRATOR_STATE_WRITE(state, path, flag) &= ~PATH_RAY_SHADOW_CATCHER_BACKGROUND;
      integrator_intersect_next_kernel_after_shadow_catcher_background<...>(kg, state);
      return;
    }

So a background sample writes the background and **does not terminate**. The path carries
on, escapes again, and shades the background again - which is why there are three groups
of ~77,000 rather than one, all from the same samples, the later two now carrying
`DIFFUSE | REFLECT` and `DIFFUSE_ANCESTOR` at bounce 1. Both builds' logs show
`ApplyGroundPlaneChanges`, so Rhino's ground plane is the shadow catcher here.

That also explains why the per-flag tap readings looked flagless: those later writes do
carry ray type flags, but they land in shadow catcher passes rather than in the pixel the
`.hdr` shows, which is why `Is Diffuse Ray` reads 0 over the background patch.

### The camera ray's background colour is discarded, by design

`film_write_background` was instrumented the same way, recording which of its two branches
runs and the value each would deposit.

**No `fw:transp` calls at all**, so `kernel_data.background.transparent` is false and the
transparent-background branch never runs - every write goes to
`film_write_combined_transparent_pass`. What that shows:

| write | calls | bounce | throughput | value deposited |
| --- | --- | --- | --- | --- |
| `0xe0009001` | 80221 | 0.0000 | 1.0000 | **1.0000** |
| `0x6400200a` | 80210 | 1.0000 | 0.8679 | 0.0000 |
| `0x2400600a` | 79990 | 1.0003 | 0.8683 | 0.0000 |

So the camera write carries the full white background, and both shadow catcher
continuations carry nothing. And `film_write_combined_transparent_pass` opens with

    if (film_write_shadow_catcher_transparent(kg, path_flag, contribution, transparent, buffer)) {
      return;
    }

whose first act is

    if (path_flag & PATH_RAY_SHADOW_CATCHER_BACKGROUND) {
      return true;
    }

`0xe0009001` has bit 31 set, which is exactly `PATH_RAY_SHADOW_CATCHER_BACKGROUND`. **The
camera ray's background colour is therefore discarded from the combined pass on purpose**,
because the path is about to continue and shade the background again - and that later
write, now flagged `DIFFUSE | REFLECT`, is the one Rhino's graph deliberately routes to the
sky branch, which folds to black. Hence value 0.0000 on both continuations.

That is the whole mechanism, and it explains why the light path gating matters so much in
shipping and not at all here: in shipping the visible background does **not** come from the
camera ray's evaluation, so the gating decides what is seen. In this branch the camera
write survives, the gating never fires, and the background is simply the camera colour -
white.

**Proven**: one camera-flagged evaluation per background sample at bounce 0 with MIS weight,
throughput and deposited value all exactly 1; two shadow catcher continuations of the same
sample depositing 0; no distant-light evaluation; no transparent-background branch; and the
camera write discarded by the `PATH_RAY_SHADOW_CATCHER_BACKGROUND` early return.

**Inferred, not yet measured**: that the visible third arrives via the background pass.

### It is alpha_matte, and every kernel function in the chain is identical

`film_calculate_shadow_catcher_matte_with_shadow` was instrumented in **both** trees,
recording the composite's terms keyed by the result so pixels with the same outcome
aggregate. The background cluster in each:

| | shipping | this branch |
| --- | --- | --- |
| result | 0.333 | 1.000 |
| `alpha` | 0.000 | 0.000 |
| `alpha_matte` | **0.667** | **0.000** |
| `scale` | 0.4399 | 0.4355 |
| `scale_exposure` | 1.0000 | 1.0000 |
| `background_scale_exposure` | 0.4399 | 0.4355 |
| `color_background` | 1.0000 | 1.0000 |

`alpha_over = color_matte * alpha + color_background * (1 - alpha_matte)` gives
`1.0 * (1 - 0.667) = 0.333` and `1.0 * (1 - 0) = 1.0`. So the scales agree to within noise
and the background colour is 1.0 in both - **the entire difference is `alpha_matte`**. The
divergence in `film_get_scale_and_scale_exposure` recorded above is real but is *not* the
cause; that lead is dead.

Since `alpha` is 0 in both, `alpha_matte = 1 - saturate(average(shadow_catcher))`, so
`film_calculate_shadow_catcher` returns 0.333 in shipping and 1.000 here. It returns
exactly one from

    /* If there is no shadow catcher object in this pixel, there is no modification of the
     * light needed, so return one. */
    if (num_samples == 0.0f) {
      return one_float3();
    }

so `pass_shadow_catcher_sample_count` is **zero on sky pixels here and non-zero in
shipping**. Shipping performs the shadow catcher split on background pixels; this branch
does not.

And that is where the kernel stops being the answer. `film_calculate_shadow_catcher`,
`film_calculate_shadow_catcher_matte_with_shadow` and
`film_write_shadow_catcher_bounce_data` are byte-identical between the trees, and
`integrator/shadow_catcher.h` differs only in includes and parameter types - no logic
change anywhere in the split.

**So the remaining difference is not in the Cycles kernel.** But it is not in the scene
setup either, which the next round checked and cleared.

### The scene setup matches too

`CCYCLES_PASS_PROBE` (with `CCYCLES_DIAG_LOG`) reports the scene-side state at the last
sample. For this branch:

    probe: approx_shadow_catcher=1 background_transparent=0
    probe: has_shadow_catcher=1 passes=10

and all ten passes are allocated, including the complete shadow catcher set -
`PASS_SHADOW_CATCHER` (53), `PASS_SHADOW_CATCHER_SAMPLE_COUNT` (54),
`PASS_SHADOW_CATCHER_MATTE` (55), `PASS_SHADOW_CATCHER_BACKGROUND_SAMPLE_COUNT` (47),
`PASS_SHADOW_CATCHER_TRANSPARENT_SAMPLE_COUNT` (46), alongside `PASS_COMBINED`,
`PASS_BACKGROUND`, `PASS_DEPTH`, `PASS_ADAPTIVE_AUX_BUFFER` and `PASS_SAMPLE_COUNT`.

So the scene does have a shadow catcher here, it is the ground plane, the approximate
background composite is on, the background is not transparent, and every pass the
composite needs exists. None of those is the difference.

Decoding the recorded path flags with each tree's own enum confirms the paths match as
well:

| | shipping | this branch |
| --- | --- | --- |
| camera eval | `CAMERA, MIS_SKIP, SHADOW_CATCHER_BACKGROUND, SHADOW_CATCHER_HIT, SHADOW_CATCHER_PASS, TRANSPARENT_BACKGROUND` | identical apart from `CAMERA`, which 5.x deleted from `PathRayFlag` |
| continuation 1 | `DIFFUSE, DIFFUSE_ANCESTOR, REFLECT, SHADOW_CATCHER_HIT, SHADOW_CATCHER_PASS, SURFACE_PASS` | identical apart from `DIFFUSE`, which moved to the visibility mask |
| continuation 2 | `DIFFUSE, DIFFUSE_ANCESTOR, REFLECT, SHADOW_CATCHER_HIT, SINGLE_PASS_DONE, SURFACE_PASS` | same |

`Object::visibility_for_tracing` also matches: shipping masks with
`PATH_RAY_ALL_VISIBILITY`, this branch asserts the equivalent instead, which is an upstream
5.x change and not a behaviour difference.

### Root cause: PASS_SHADOW_CATCHER is never written here

`film_calculate_shadow_catcher` was instrumented in both trees to log the three pass values
its ratio is built from, read at the offsets the kernel has already resolved by pass type.
(`CCYCLES_PASS_PROBE` cannot do this: it looks passes up by name via `get_pass_pixels`, and
every pass but `combined` and `depth` has an empty name, so it calls them unavailable when
they are allocated.)

`color_catcher`, by shadow catcher sample count:

| n | shipping | this branch |
| --- | --- | --- |
| 1 | 0.6165 | **0.0000** |
| 2 | 1.6288 | **0.0000** |
| 3 | 1.8337 | **0.0000** |
| 4 | 3.2338 | **0.0000** |
| 5 | 3.0237 | **0.0000** |

**`PASS_SHADOW_CATCHER` is populated in shipping and entirely empty here.** The pass is
allocated - it is type 53 in the probe's list - it simply never receives a non-zero
contribution. With `color_catcher` zero, `safe_divide_shadow_catcher` falls back to one, so
`shadow_catcher` is 1, `alpha_matte` is 0, and the composite passes the background through
at full brightness. Shipping's non-zero value gives 0.333. That is the whole difference.

Both write sites and the `kernel_shadow_catcher_is_object_pass` predicate guarding them are
identical between the trees, so the code is not what differs - the contributions arriving
there are zero, meaning the shadow catcher object path carries no light here.

### The light path gating is NOT the cause

The obvious suspect was Rhino's shadow catcher network in `RhinoFullNxt.cs`:

    lightpath.outs.IsReflectionRay.Connect(pathadder.ins.Value1);
    lightpath.outs.IsDiffuseRay.Connect(pathadder.ins.Value2);
    pathadder.outs.Value.Connect(refl_flipper.ins.Fac);
    lastclosure.Connect(refl_flipper.ins.Closure1);
    noshow.outs.BSDF.Connect(refl_flipper.ins.Closure2);

`pathadder` is a clamped add and `Closure2` is a transparent BSDF, so a `Fac` of one makes
the ground plane contribute nothing. Since 5.x answers these outputs from the visibility
mask rather than the path flag - the change behind the clipping plane and
`NODE_TEXCO_WINDOW` bugs above - it looked likely that they now read one where 3.5 read
zero.

**They do not.** `svm_node_light_path` was instrumented in both trees to log what each
output returns, keyed by output type and path flag. `IsReflectionRay` matches row for row:

| path | shipping | this branch |
| --- | --- | --- |
| `SHADOW_CATCHER_HIT` | 0.0 | 0.0 |
| `SHADOW_CATCHER_HIT, PASS` | 0.0 | 0.0 |
| `SHADOW_CATCHER_BACKGROUND, HIT, PASS` | 0.0 | 0.0 |
| `REFLECT, SURFACE_PASS` (376k calls) | 1.0 | 1.0 |
| `REFLECT, HIT, SURFACE_PASS` | 1.0 | 1.0 |
| `REFLECT, HIT, PASS, SURFACE_PASS` | 1.0 | 1.0 |

`IsDiffuseRay` matches too. So the shadow catcher shader's `Fac` is the same in both builds
and this lead is closed.

### The missing write: the shadow catcher gets no direct light here

Both `film_write_pass_spectrum(pass_shadow_catcher, contribution)` sites were instrumented
in both trees, recording the guard's result next to the contribution:

| site | shipping | this branch |
| --- | --- | --- |
| `scw1 write` | 81064 calls, contribution **0.6114** | **never fires** |
| `scw2 write` | 81064 calls, contribution 0.0000 | 81040 calls, contribution 0.0000 |

So it is not "the write happens with zero" - site 2 does that in both. It is that **site 1
never happens here at all**, and site 1 is the one carrying light.

The two sites sit in different functions reached by different routes:

  * `scw1` is in `film_write_shadow_catcher`, called from `film_write_combined_pass` -
    the direct light and surface emission route.
  * `scw2` is in `film_write_shadow_catcher_transparent`, called from
    `film_write_combined_transparent_pass`.

`film_write_combined_pass` has the same four callers in both trees, and
`Scene::device_update` runs the managers in the same order either side (objects, geometry,
object flags, lights, integrator), so the integrator snapshots `has_shadow_catcher` after
objects are loaded in both. The probe also reports `has_shadow_catcher=1` here and the
shadow catcher passes are allocated, so `film_write_shadow_catcher` would not be returning
at its `!kernel_data.integrator.has_shadow_catcher` early out.

What that leaves, measured: **the ground plane receives no direct light contribution in
this branch**, so nothing ever reaches `film_write_combined_pass` on a shadow catcher path,
`PASS_SHADOW_CATCHER` stays zero, `film_calculate_shadow_catcher` divides by zero and falls
back to one, `alpha_matte` becomes zero, and the background is composited at full
brightness instead of a third.

### film_write_direct_light is never called here at all

All four `film_write_combined_pass` call sites were instrumented in both trees. Shipping
produces exactly **one** row - `cpB`, 81064 calls, every one on flag `0x60009001`
(`SHADOW_CATCHER_HIT | SHADOW_CATCHER_PASS`), carrying 0.6114 - and that is the same count
and the same contribution as the `scw1` write that fills `PASS_SHADOW_CATCHER`. This branch
produces **no rows at all**.

`cpB` sits immediately before an unconditional call, with no early return between it and
the top of `film_write_direct_light`; the kernel objects are newer than the edit; and other
probes in the same header (`scw2`, 81037 calls) fire in the same run. So the instrument is
live and the function genuinely does not run.

`film_write_direct_light` has exactly one caller in each tree, `integrator_shade_shadow` in
`shade_shadow.h`, so **no shadow ray in this build ever writes its direct light
contribution**. That is next-event estimation not landing, which is a much larger problem
than the background it was found through - the background is merely the place where it
became visible, because the shadow catcher pass has no other source.

The structural difference is in the same function. Shipping:

    const bool opaque = integrate_transparent_shadow(kg, state, num_hits);
    if (opaque) { integrator_shadow_path_terminate(...); return; }

This branch:

    const TransparentShadowResult result = integrate_transparent_shadow(kg, state, packed_num_hits);
    if (result == TRANSPARENT_SHADOW_EVAL_CACHE_MISS) {
      integrator_shadow_path_cache_miss(state, DEVICE_KERNEL_INTEGRATOR_SHADE_SHADOW);
      return;
    }
    if (result == TRANSPARENT_SHADOW_EVAL_OPAQUE) { integrator_shadow_path_terminate(...); return; }

`TRANSPARENT_SHADOW_EVAL_CACHE_MISS` is a 5.x addition, part of the same shader-cache
mechanism as the `SD_CACHE_MISS` check in `light_sample_shader_eval_forward`. If that path
always reports a miss - because nothing in Rhino's integration ever services the cache and
re-queues the work - every shadow ray returns there and direct lighting silently
disappears.

**RETRACTED - the conclusion below is wrong.** 5.x restructured next-event estimation out
of `shade_shadow` entirely. It has kernels 3.5 does not:
`DEVICE_KERNEL_INTEGRATOR_SHADE_LIGHT_NEE`,
`DEVICE_KERNEL_INTEGRATOR_SHADE_DEDICATED_LIGHT`, with `shade_dedicated_light.h` and
`shade_light.h` alongside `shade_shadow.h`. Direct light now lands through
`film_write_surface_emission` (`shade_light.h:73`), not `film_write_direct_light`. So
`integrator_shade_shadow` never running is **normal for 5.x**, not a defect, and this
branch is not missing next-event estimation. Lars rendering Brian's glass model on CPU and
finding it good is what prompted the recheck; a renderer with no NEE would not look good.

What remains true and measured: `PASS_SHADOW_CATCHER` is empty here and populated in
shipping, and none of the four `film_write_combined_pass` call sites fires here. Since 5.x
reaches the shadow catcher write by a different route, the question is now whether the new
NEE kernels carry the shadow catcher write at all - upstream restructured this code and
Rhino's shadow catcher handling may simply not have been carried across to the new path.
That is where to resume.

The superseded reading follows, kept because the measurements in it are sound even though
the conclusion drawn from them was not.

**Measured:** `integrator_shade_shadow` was instrumented at entry and at every exit
in both trees. This branch produces **no rows at all - not even the entry marker** - in a
run where the same tally emitted 226 `approx`, 62 `lp` and 10 `scw2` rows, so the
instrument was live. The shadow kernel therefore never runs here at all. It is not the
write failing, and not the cache-miss exit being taken: no shadow ray is ever shaded.

Together with the 376k background evaluations at bounce 1 carrying `DIFFUSE | REFLECT`,
that means this branch lights the scene purely by BSDF-sampled environment hits, with no
next-event estimation whatsoever. That is consistent with renders being brighter in places
(no MIS split of the energy) and noisier, and it fully accounts for `PASS_SHADOW_CATCHER`
being empty.

Still to measure: the shipping side of the same counters, to confirm the contrast rather
than infer it, and then why the kernel is never scheduled - whether the shadow paths are
never queued or the device never dispatches that kernel.

**The cache-miss reading below was a hypothesis, not a measurement, and is now superseded.** What is measured is only that
`film_write_direct_light` never runs. The probe that settles it counts entries to
`integrator_shade_shadow` and which of its three exits each takes, in both trees. Given how
many strong-looking readings in this section have been wrong, it should be measured before
being believed or acted on.


One difference found while looking, not yet shown to matter: `Scene::has_shadow_catcher`
here skips objects whose geometry `is_light()`, which shipping does not do. It is an
upstream 5.x change, and since the probe still reports 1 it is not the cause, but it is
worth remembering if the object set ever changes.


Two cautions for whoever continues. The absolute numbers are read out of the saved
`.hdr`, and shipping's `.hdr` and `.bmp` do not agree with each other under the same
transform, so trust the ratios rather than the values. And the difference is not a
global exposure change: the lit tabletop differs by 1.35 where the background differs
by 3.00, which is what indirect light from a 3x brighter background would do.

## A mix node in a volume shader dereferences null

`MixNode::is_linear_operation()` ended with

    return use_clamp == false && input("Factor")->link == nullptr;

but `MixNode` registers its factor socket as `"Fac"`. `"Factor"` is `MixColorNode`'s
name for it, and `ShaderNode::input()` returns `nullptr` for a name the node does not
have, so this dereferences null for every blend, add, multiply or subtract mix. Nor does
the `use_clamp == false &&` short-circuit save it: `mix` defaults `use_clamp` to false.

It is reachable from `ShaderGraph::optimize_volume_output`, which returns early unless
the graph's `Volume` output is linked - so it is a latent crash on any volumetric
material containing a mix node, not something the test models hit yet. The four sibling
nodes on the same line (`MixColorNode`, `MixFloatNode`, `MixVectorNode`,
`MixVectorNonUniformNode`) and `FloatCurveNode` all genuinely register `"Factor"` and
are correct as written.

Arrived with the 5.2 rebase, from upstream `e063549d4` "Detect volume attribute nodes
that can use stochastic sampling" - not from the 4.4 port. Fixed by asking for `"Fac"`.

## Two Rhino kernel functions asked a path flag whether it is a camera ray

3.5 had `PATH_RAY_CAMERA = (1U << 0U)` in `PathRayFlag`. 5.x **removed it**: camera-ness
moved to a separate `PathRayVisibility` mask, `INTEGRATOR_STATE(state, path, visibility)`,
and bit 0 of a path flag is now `PATH_RAY_REFLECT`. Two Rhino functions were ported by
substituting the new constant into the old test, which compiles and silently asks about
reflection rays instead:

`path_state.h`, `path_clip_ray` - Rhino's clipping planes:

    const uint32_t path_flag = INTEGRATOR_STATE(state, path, flag);
    if ((path_flag & PATH_RAY_VISIBILITY_CAMERA) == PATH_RAY_VISIBILITY_CAMERA) {

A camera ray never satisfies that, so **clipping planes stop clipping directly visible
geometry** and start clipping reflection rays.

`svm/tex_coord.h`, `svm_rhino_node_tex_coord` (the grafted `RHINO_NODE_TEX_COORD`),
`NODE_TEXCO_WINDOW`:

    if ((path_flag & PATH_RAY_VISIBILITY_CAMERA) && sd->object == OBJECT_NONE &&

`texcoord.Window` is what feeds the background gradient's coordinates, so this one
misplaces the gradient under an orthographic camera. Upstream's own
`svm_node_tex_coord_eval` a hundred lines earlier reads `path_visibility` correctly,
which is what makes the grafted copy stand out.

Both fixed by reading the visibility mask. `svm_rhino_node_tex_coord` took `path_flag`
only for this test, so it now takes `path_visibility`; the dispatch in `svm.h` already
had it in scope. Worth a targeted check of clipping planes and of a gradient background
under an orthographic camera - neither is covered by the current test models.

## HIP against CPU: it is one material, and its textures are in memory

Narrowed from "HIP renders Brian's model wrong" to a single material, without a rebuild.
All on the dev build, only the device changing:

| test | CPU | HIP | agree |
| --- | --- | --- | --- |
| plain, whole frame | R 0.0806 G 0.1227 B 0.2481 | R **0.0000** G 0.0229 B 0.1047 | no |
| `CCYCLES_NO_LIGHT_TREE=1` | 0.1504 | 0.0424 | no |
| `CCYCLES_FLAT_SHADERS=1` | 0.2209 | 0.2209 | **yes** |
| `RHDIFF_REPLACEMAT=Glass_Crystal` | 0.1564 | 0.0549 | no |
| `RHDIFF_REPLACEMAT=lamp-shade` | 0.1048 | 0.0113 | no |
| `RHDIFF_REPLACEMAT=Paper` | 0.0968 | 0.0968 | **yes** |

The red channel is **exactly zero** over the whole frame on HIP in the plain case, which is
why the image reads as uniformly blue rather than merely dark - a channel gone, not
attenuation.

Flat shaders agreeing puts this in shader evaluation rather than lighting or the film, and
the light tree makes no difference either way. Replacing just `Paper` with an untextured
grey PBR makes the devices agree exactly, red included.

`RHDIFF_MATDUMP` says what `Paper` is:

    MAT 'Paper' type='Physically Based'
      CHILD slot='pbr-base-color' type='Bitmap Texture' file='C:\Users\GEBRUI~1\...\V7\TextureCache\E6AD.jpg' exists=False
      CHILD slot='pbr-bump'       type='Bitmap Texture' file='C:\Users\GEBRUI~1\...\V7\TextureCache\E6EC.jpg' exists=False

Both files point at another machine's temp texture cache and **do not exist**, so the pixels
are embedded in the .3dm and reach Cycles through the in-memory image path - the one stubbed
out in 5.2 behind an undefined `LEGACY_IMAGES` and implemented here by
`RhinoMemoryImageLoader` in `scene/image_rhino.cpp`. That code was added earlier in this
port, and the note elsewhere in this file that it "has no test that exercises it" was
wrong: Brian's model exercises it, and is the only model in the set that does.

Scope: `SimpleVaseTest` and `GjisGlasTalerSimplified2` render identically on CPU and HIP, so
this is not a general GPU problem. Synthetic scenes built for this - a rectangular light
over a floor and box, with and without a lamp shade cone - also agree exactly on both
devices, so neither area lights nor enclosed lights are involved.

`RhinoMemoryImageLoader::load_pixels` has the same signature as the base and as the oiio,
sky and vdb loaders, so the override binds correctly, and nothing in it is device-specific -
which is what makes a device-dependent failure interesting.

**Not yet separated:** `Paper` has *two* in-memory textures, base colour and bump, and bump
is its own suspect - 5.x replaced `svm_node_tex_coord_bump_dx/dy` with the dual-number
`svm_node_tex_coord_derivative`, and Rhino's `RHINO_NODE_TEX_COORD` is grafted onto that, as
its own comment in `svm/tex_coord.h` says. So this is either the in-memory pixels never
reaching the device, or the bump derivative graft differing on GPU.

The experiment that separates them: a synthetic material with a **file-based** texture on
both devices, in base-colour-only and bump-only variants. File-based agreeing while embedded
diverges pins it on the in-memory path; both diverging pins it on textures or bump generally.

Reproducer, no rebuild needed:

    .\devcmp.ps1                                        # diverges
    .\devcmp.ps1 -Switch RHDIFF_REPLACEMAT -Value Paper  # agrees

## HIP renders an emissive-lit scene far darker than CPU

Same build, same model, same everything but the device. `Brian25YearRhinoGlas` on HIP loses
the neutral illumination entirely: the emissive panel is still there and the glass still
shows blue specular, but the ground plane is black, with no diffuse light and no shadow. On
CPU the floor is lit and gridded and casts a shadow.

Reproduced in the harness at 5 samples, 400x240, via `scprobe.ps1 -Device 0` against
`-Device 1`, reading the combined pass host-side:

| | CPU | HIP | ratio |
| --- | --- | --- | --- |
| nonzero pixels | 205072 | 105410 | 1.95 |
| mean | 0.4207 | 0.3119 | |
| R | 0.1294 | 0.0272 | 4.75 |
| G | 0.1841 | 0.0549 | 3.35 |
| B | 0.3694 | 0.1654 | 2.23 |

Not a uniform darkening: blue survives best and red and green collapse, and half the lit
pixels go to zero. The blue is the emitter seen directly and in specular; what is lost is
everything that should arrive as neutral illumination.

**It is scene-dependent, not universal.** `SimpleVaseTest` renders the same on both devices
- background patch 0.9965 either way, matching every CPU measurement in this file. The
difference between the two scenes is the light: SimpleVaseTest is lit by the environment,
Brian's model by an emissive area panel in an otherwise dark scene. So the suspicion is
area/emissive light sampling on GPU, which is also where the unresolved light direction
question in this document sits.

Ruled out already:

  * Stale GPU kernels. HIP has no prebuilt binaries here; it compiles at runtime from the
    kernel source copy installed at `big_libs/RhinoCycles/ccycles/win/debug/source/kernel`,
    and that copy's timestamps match the working tree exactly and contain the
    `NODE_SET_BUMP` `break` fix. The kernel cache directory is empty but kernels load.
  * Kernel build failures. The run logs no error, warning or fallback, and reports
    `Loading render kernels` normally.

Note for whoever picks this up: **the `CCYCLES_BG_EVAL_TALLY` instrumentation cannot see
HIP.** Every kernel-side probe in it is guarded by `#ifndef __KERNEL_GPU__`, so on GPU it
compiles out and produces an empty file - which looks exactly like a diagnostic that found
nothing. `CCYCLES_PASS_PROBE` does work on both devices, because it reads the tile passes
host-side in the output driver. Enabling the per-component light passes and comparing them
across devices is the obvious next probe: it would say directly whether the diffuse direct
component is what goes missing.

## Three node types are not registered

`velvet_bsdf`, `anisotropic_bsdf` and `musgrave_texture` are referenced by
csycles and are not registered node types in 5.2. Each is referenced from
exactly one csycles file and has **no** RhinoCycles callers, so nothing in Rhino
asks for them today and the fallback cannot be reached from a Rhino material.

That is why the fallback was accepted rather than made faithful: it is
unreachable, and being unfaithful in unreachable code is cheaper than being
wrong about what the replacement should be. Upstream replaced velvet with a
sheen model and musgrave with the noise texture's added modes, and anisotropy
moved into the principled node's own anisotropic inputs.

Worth revisiting only if a Rhino caller appears, or if csycles is meant to be a
faithful Cycles binding independent of what Rhino uses - which is a question
about what csycles is for, not about these three nodes.

## The renders are 4.2% darker than shipping

`ours/shipping` is 0.9577 on the material preview scene. This is measured and
narrowed rather than guessed - the full account, including what has been
eliminated and by what evidence, is the "What the remaining difference against
shipping is, and is not" section of `DIAGNOSTICS.md`.

The short version: it is not the materials, not the environment, not the
texture colorspace, not the light tree, not shadow-catcher compositing, not
adaptive sampling, not denoising, and not the device. It only affects surfaces
lit by the environment, it grows monotonically with pixel brightness, and the
directly-viewed background is exact to within the noise floor.

## How a Cycles source build should be triggered

**Decided: a solution configuration.** `Debug+Cycles` and `ReleaseDebuggable+Cycles` build
Cycles from source; the plain configurations use the prebuilt payload from
`big_libs`, exactly as Rhino 9.x did. Visual Studio shows them in its
configuration dropdown and RhinoBuilder in its Configurations list, so the same
switch exists in both tools with no checkbox, no environment variable and nothing
to remember. Off by default and impossible to trip by accident.

### Why not automatically

Two automatic schemes were built and thrown away, so the reasoning is worth
keeping.

**Timestamps cannot work.** A Makefile-style vcxproj declares no source files, so
MSBuild has nothing to compare. Supplying the file list does not rescue it either:
git sets mtime to checkout time, so on a fresh clone the sources and the payload
get the same mtime in an arbitrary order and staleness becomes a coin flip. A cold
walk of the 891 sources also took about 10s here.

**Content comparison works but is not worth it.** A fingerprint of the sources -
the cycles commit plus a hash of local modifications - stamped into the payload
does detect changes correctly, and was verified doing so. It was dropped anyway,
because it rests on a rule a person can forget: whoever changes Cycles must commit
a rebuilt payload, and if they do not, everyone else quietly runs binaries that do
not match their tree. Trading an explicit choice for a silent failure mode is a
bad trade. An earlier variant keyed on the CMake build tree existing had the same
shape of problem in reverse - Clean would have silently switched it off.

The version that lost is recorded here rather than in the code: see
`tools/cycles_build_if_needed.ps1` at cycles commit 06b0060b1 if it is ever wanted
back.

### Independent of the above

Two fixes stand on their own and do not depend on which option wins:

- `RhinoCyclesCore` has **no solution dependency** on `ccycles` - it has no
  `ProjectDependencies` entry at all. Adding one makes the deploy trap
  impossible instead of documented: building the plug-in would build Cycles
  first, so "build both, in that order" stops being knowledge anyone needs.
- The build says nothing about **which** payload it copied, or whether Cycles was
  built from source. Both states are currently invisible and both have produced
  wrong conclusions in this port. Two one-line messages retire most of the
  "check the binary is newer than your edit" ritual.
