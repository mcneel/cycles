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
