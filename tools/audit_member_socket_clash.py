"""Audit csycles for parameters exposed as both a direct member and an input socket.

A fourth way this port drifts silently, alongside the renamed socket, the renumbered
enum and the packed SVM node that the other audits cover.

`Shader.WriteDataToNodes` calls `SetEnums()`, then `SetDirectMembers()`, then
`SetSockets()`. `SetSockets` pushes *every* non-retired input socket, including ones the
caller never assigned, because a socket holds its C# default from construction. So where
a node exposes the same Cycles parameter both ways, the socket write lands last and
silently overwrites whatever `SetDirectMembers` sent.

Nothing throws. The value simply does not arrive, and the node renders with the socket
default.

Found doing this by hand on 2026-08-29: `BumpNode` carries `invert` as both, RhinoCycles
was setting the member, and negative bump amounts consequently never inverted. Nathan's
4.4 branch had already moved those call sites to `ins.Invert.Value`; the revert of that
branch brought the member form back, which is exactly the kind of regression this audit
is meant to catch.

Only *input* sockets count. An output socket sharing a member's name is harmless, since
`SetSockets` never touches outputs - `ValueNode.value` looks like a clash and is not.

Exit code 0 if no node exposes a parameter both ways, 1 otherwise.
"""

from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
SHADER_NODES = ROOT / "src" / "csycles" / "ShaderNodes"

# shadernode_set_member_*/set_enum_* name their target explicitly.
MEMBER_RE = re.compile(r'shadernode_set_(?:member|enum)_?\w*\(\s*Id\s*,\s*"([^"]+)"')
# Socket construction: new XSocket(parentNode, "Display", "internal")
SOCKET_RE = re.compile(r'new\s+\w*Socket\(\s*parentNode\s*,\s*"[^"]*"\s*,\s*"([^"]+)"')
INPUTS_CLASS_RE = re.compile(r'class\s+\w*Inputs\s*:\s*Inputs\b')
CLASS_RE = re.compile(r"^\s*(?:public|internal|private)?\s*class\s+\w+", re.M)

# Known and accepted. Keep the reason with the entry so it can be re-judged.
ACCEPTED = {
    # musgrave_texture is not a registered node type in 5.2 and has no RhinoCycles
    # caller, so the clash is unreachable. See PORTING-GAPS.md.
    ("MusgraveTexture.cs", "dimension"),
}


def delegating_members(src: str) -> set[str]:
    """Members that are a view onto their socket rather than an independent value.

    `public bool Invert { get => ins.Invert.Value; ... }` cannot be clobbered, because
    reading it reads the socket. That is the fix for a clash, not an instance of one, so
    detect it rather than listing each one as accepted.
    """
    names: set[str] = set()
    for m in re.finditer(r"get\s*=>\s*ins\.(\w+)\.Value", src):
        names.add(m.group(1))
    return names


def input_socket_names(src: str) -> set[str]:
    """Internal names of sockets declared in the node's *Inputs class only."""
    bounds = [m.start() for m in CLASS_RE.finditer(src)] + [len(src)]
    names: set[str] = set()
    for start, end in zip(bounds, bounds[1:]):
        block = src[start:end]
        if INPUTS_CLASS_RE.search(block):
            names.update(SOCKET_RE.findall(block))
    return names


def main() -> int:
    if not SHADER_NODES.is_dir():
        print(f"csycles ShaderNodes not found at {SHADER_NODES}")
        return 1

    clashes: list[tuple[str, str]] = []
    accepted_seen: list[tuple[str, str]] = []
    for path in sorted(SHADER_NODES.rglob("*.cs")):
        src = path.read_text(encoding="utf-8-sig", errors="replace")
        # A delegating member reads through to the socket, so it is not a clash.
        safe = {n.lower() for n in delegating_members(src)}
        both = sorted(
            n for n in set(MEMBER_RE.findall(src)) & input_socket_names(src)
            if n.lower() not in safe
        )
        for name in both:
            entry = (path.name, name)
            (accepted_seen if entry in ACCEPTED else clashes).append(entry)

    for filename, name in clashes:
        print(f"{filename}: '{name}' is both a direct member and an input socket - "
              f"SetSockets runs last and overwrites the member")
    for filename, name in accepted_seen:
        print(f"{filename}: '{name}' clashes, accepted (see PORTING-GAPS.md)")

    if clashes:
        print(f"member/socket clash: {len(clashes)} unaccepted, "
              f"{len(accepted_seen)} accepted")
        return 1
    print(f"member/socket clash: none unaccepted, {len(accepted_seen)} accepted")
    return 0


if __name__ == "__main__":
    sys.exit(main())
