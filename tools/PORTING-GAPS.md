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

**Decided: the CMake build tree is the switch.** `ccycles.vcxproj` builds from
source when `RHINOCYCLESDEV` is set *or* when `cycles/build/CMakeCache.txt`
exists. The options below are kept because the reasoning rules out the obvious
alternatives, and someone will propose them again.

### What is true today

A source build happens if and only if `RHINOCYCLESDEV` is non-empty.
`ccycles.vcxproj` tests it; nothing else in the tree reads it. Unset, the whole
build step is one `echo`, and the prebuilt payload in `big_libs` is used.
`tools/cycles_dev.ps1` sets, clears and reports it.

**Normal developers already match Rhino 9.x, by the same mechanism.** The 9.x
branch pins `big_libs` at `161a4919`, which carries
`RhinoCycles/ccycles/win/release/ccycles.dll` plus 34 precompiled kernel files -
Windows release only, no debug, exactly as now. Neither branch builds Cycles for
a normal solution build. The only differences are that `ccycles` is now in
`Rhino.sln` (9.x had it only in `RhinoWithExtras.sln`, and 9.x's `Rhino.sln` has
no reference to it at all) and that `csycles` moved from `cycles/csycles` to
`cycles/src/csycles`. Neither costs a developer anything.

So nothing here is a regression against 9.x. The question is only whether a
*Cycles* developer should have to know about a flag.

### Why the obvious mechanism does not work

Making the build detect staleness by file timestamps fails, for two measured
reasons:

- **git rewrites mtimes on checkout.** After a pull, a clean tree looks newer
  than the payload, so every developer would be told to build Cycles - the exact
  outcome that has to be avoided.
- **Walking the tree is not free.** 891 Cycles source files, and a cold
  enumeration took about 10s on Windows here.

The payload also carries no record of what produced it: there is no stamp,
version or revision file anywhere in
`big_libs/RhinoCycles/ccycles/win/release`. So nothing can currently compare
"what is in the tree" against "what the payload is".

### The options

**Chosen: the build tree.** A fresh clone has no `cycles/build` - it is gitignored
as `build*/` - and only CMake creates one, so its presence is a deliberate act and
a reliable signal. No timestamps, no git queries, no stamp file, and nothing that
a checkout can scramble. A normal developer is untouched and cannot trigger it by
accident; a Cycles developer finds the flag once and then never thinks about it
again, because the tree keeps source builds on. The costs are that `-Off` no
longer suffices on its own, and that Clean had to stop deleting the tree - both
recorded in `RHINO-CYCLES-5.md`.

The rejected options follow.

**A. Keep the flag.** No risk, no change. A Cycles developer must know
`RHINOCYCLESDEV` exists, must know a running Visual Studio ignores it until
restarted, and must know that only a solution build deploys. That knowledge is
now written down, which is most of the cost removed.

**B. Detect by revision.** `build_cycles.ps1` records the cycles commit it built
from into a stamp beside the payload, committed with it. Each build compares that
stamp against `git rev-parse HEAD` plus whether the tree is dirty - cheap, and
immune to checkout mtimes. Clean and matching means no work at all; dirty or
moved means build from source. A Cycles developer then configures nothing: edit,
build, and Ninja rebuilds only what changed, kernels included.

  The cost is borne by people who did not ask for it. A checkout where the cycles
  submodule was bumped without a rebuilt payload committed alongside it looks
  stale to everybody, so everybody gets a warning they cannot act on. It also
  adds a `git` call per build, roughly 0.1-0.3s.

**C. B, gated on the toolchain.** Check for CMake/Ninja first; if absent, skip
detection entirely and use the payload silently. A machine that cannot build
Cycles gets no warning, because no warning there could prompt an action. The
proxy is imperfect in both directions: someone with CMake installed for
unrelated reasons still gets the check, and a genuinely mismatched payload goes
unmentioned on the machines least able to notice it.

**D. B, hard-failing instead of warning.** Honest and blocking. Rejected on
sight for a plug-in most developers never touch, but recorded so nobody proposes
it as new.

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
