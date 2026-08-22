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
