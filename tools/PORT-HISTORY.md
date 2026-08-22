# Where the prior Cycles port work lives

This is the third attempt at moving Rhino off Cycles 3.5. The first two left
branches behind that are still worth reading, and a code generator that is worth
reviving for the next version bump. Written down because none of it is
discoverable from the current tree - the generator is not on HEAD, and the
branches are named after people and versions rather than intent.

## The lineage

Cycles 3.5 was stable in Rhino. Nathan Letwory then ported to Cycles 4 and got
it working, though a bit unstable. He retired, the work was reverted to 3.5 to
buy time to learn the codebase, and this branch is the move from 3.5 to 5.2 for
Rhino 10.

So the 4.x work was not abandoned because it was a dead end. It was abandoned
because the person holding it left.

## The branches

In the **cycles** repository:

| Ref | Date | What it is |
| --- | --- | --- |
| `origin/rhino9_cycles35` | | The shipping baseline. The thing to diff against when asking "did this change between versions". |
| `origin/build/lars/UpdateToVersion44` | 2026-01 | The 4.4 upstream bump. |
| `rhino9_cycles44_master` | 2025-04 | Tag on the 4.4 line. |
| `596472b8b` | 2025-09 | Carries `tools/wrappergen`, the code generator. Not on HEAD. |

In the **RhinoCycles** repository:

| Ref | Date | What it is |
| --- | --- | --- |
| `origin/build/lars/UpdateToVersion44_RebaseOn9` | 2026-01 | The 4.4 Rhino-side work, already rebased onto 9.x. The useful one. |
| `origin/build/lars/RevertCycles44` | 2026-01-13 | The revert. |
| `origin/jesterKing/9.0/RhinoCycles_cycles44` | | Nathan's own 4.4 branch. |

## What 4.4 would and would not have saved

Measured rather than estimated, so the next person can decide instead of guess.

**Upstream C++ - saves little.** The delta to 5.2 is 839 files and about 170k
lines from 4.4, against 1137 files and 194k lines from 3.5. Only around a quarter
smaller, because the change that cost the most on this port - SVM per-node
structs replacing the packed `add_node(type, a, b, c)` encoding, which turned
every render black - is a 5.x change and was ahead of us from either starting
point.

**Rhino-side C# - saves a lot.** Of the 39 files this branch changed in
RhinoCycles, **23 were also changed by the 4.4 attempt**: the engine plumbing,
device commands, settings, `Utilities.cs`, `Shaders/RhinoFullNxt.cs`. That work
was duplicated here and should have been mined on day one. It is the concrete
lesson of this document.

**Except for the part that looks most valuable.** The Principled BSDF socket
*semantics* on that branch are recorded intent rather than working code - see
`PORTING-GAPS.md`, which has the file and line numbers. Its clearcoat mapping
carries an open `// TODO Check Clearcoat replacement: IOR or Weight?`, it assigns
a float to what became a colour socket, and its two migration mappings connect a
second graph to an already-connected input, which `ShaderGraph::connect` refuses.
Copy the plumbing from that branch; check the material translation against
Cycles.

**csycles moved.** It was a sibling repository (`CCSycles`, checked out next to
`cycles`) and now lives inside this one at `src/csycles`. So its history does not
transfer as commits even where the content applies.

## Nathan's generator

`tools/wrappergen` at `596472b8b`, 91 files, "Generated C# API for Cycles". It
harvests type information from the Cycles headers in a Docker container and
generates three things: `ccycles.cpp` (the wrapper C API), `CSycles.cs` (the
P/Invoke layer) and one `*Node.cs` per shader node.

**What it covers** is exactly the layer that drifts silently and is tedious by
hand: node types, sockets, their names and types. That is the right tool for a
version bump.

**What it does not cover** is most of what this port cost: the SVM encoding in
C++, enum values crossing the C API, Rhino's own material translation in
`RhinoFullNxt.cs`, and light transport. Reviving it would not have shortened this
particular port much - but the audits in `DIAGNOSTICS.md` only *detect* binding
drift, and generating the binding *prevents* it.

**The catch, which is the reason this section exists.** The generator overwrites
hand-made improvements, so its output has to be reviewed file by file and some
files reverted after every run. Known cases from Nathan's own notes:

- `csycles/Light.cs`, `Scene.cs`, `Shader.cs` - hand-written versions return and
  accept real classes; the generator emits `IntPtr`.
- `csycles/Progress.cs` - the generator drops the `out` keyword on
  `GetTime(out double, out double)`.
- `csycles/Session.cs` - the generator drops the `if (!_destroyed)` guard around
  `session_cancel`.
- `csycles/ShaderNodes/RGBCurvesNode.cs` - generated as a `NOTYET` comment rather
  than a call.
- `csycles/ShaderNodes/VectorCurvesNode.cs` - generated incorrectly; needs a
  return value.

Also note the README assumes the old layout, with `cycles` and `csycles` as
sibling checkouts mounted separately into the container. Both paths need updating
for `src/csycles`.

## If you are picking this up

1. Diff `origin/rhino9_cycles35` against HEAD before believing anything changed
   between versions. Most of the "it must be X" theories on this port died there,
   cheaply.
2. Check `UpdateToVersion44_RebaseOn9` before writing Rhino-side C#. Take the
   plumbing, verify the material translation.
3. Run `tools/run_checks.ps1`. It is one command and it covers the three ways
   this port has drifted silently.
4. For the next version bump, revive `tools/wrappergen` rather than hand-editing
   csycles - and budget for the review pass above.
