# Known gaps in the 3.5 -> 5.2 port

Things that work differently from shipping Rhino 9 WIP and are not bugs to be
found - they are decisions waiting to be made. Each one says what was measured
or read, what the safe current behaviour is, and what the choice is, so nobody
has to rediscover the shape of the problem before making it.

See `DIAGNOSTICS.md` for the switches and tooling used to establish any of this.

## Two principled inputs are silently dropped

Blender 4.0 removed `Subsurface Color` and `Transmission Roughness` from the
Principled BSDF. csycles keeps both sockets, marked `Retired` rather than
deleted, and `Retired` means a real no-op: `SocketBase.Connect` returns early and
`ShaderNode` skips the socket when pushing values. Nothing throws and nothing
reaches Cycles.

RhinoCycles still writes to both, so two Rhino inputs currently have no effect:

| Rhino input | csycles socket | RhinoFullNxt.cs |
| --- | --- | --- |
| PBR subsurface scattering colour | `SubsurfaceColor` | connected at the PBR path, and given a grey default on the standard path |
| PBR opacity roughness, and `RefractionRoughness` on the standard path | `TransmissionRoughness` | connected at the PBR path, set from `part.RefractionRoughness` on the standard path |

So a material with subsurface scattering loses its scattering colour, and glass
with a refraction roughness distinct from its surface roughness loses that
distinction.

**The choice.** In 4.x, subsurface takes its colour from `Base Color`, and
transmission takes its roughness from the main `Roughness`. Both therefore mean
folding one Rhino input into another, and which one wins is visible to users:

- Subsurface colour could be mixed into base colour by the subsurface weight, or
  simply ignored when the weight is zero (which is the common case and costs
  nothing).
- Refraction roughness and surface roughness have to become one number. Rhino
  exposes them separately, so either the refraction control stops working or the
  surface reflection changes with it.

Blender has its own conversion for old files and matching it would be the
defensible default, but that code is not in this repository, so implementing it
from memory would be guesswork. Not done on purpose.

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
