"""Audit RhinoCycles for writes to csycles sockets that are marked Retired.

csycles keeps sockets upstream removed as `{ Retired = true }` properties so call sites
compile, and silently drops connections and values sent to them. If nothing else sets that
parameter, the node renders on a Cycles default nobody chose (e.g. Glass Frost sent to
TransmissionRoughness left Roughness at 0.5, frosted even at Frost 0).

Matching is by property name, so a name retired on one node and live on another is
reported: a false positive costs one line in ACCEPTED.

Exit code 0 if every write to a retired socket is in ACCEPTED, 1 otherwise.
"""

from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
SHADER_NODES = ROOT / "src" / "csycles" / "ShaderNodes"
# cycles-core lives at <rhino>/src4/rhino4/Plug-ins/RDK/cycles-core, RhinoCycles beside it.
RHINOCYCLES = ROOT.parent / "RhinoCycles"

# Socket construction carrying a Retired marker, on one line or split over two:
#   Foo = new FloatSocket(parentNode, "Foo", "foo") { Retired = true };
RETIRED_RE = re.compile(
    r"(\w+)\s*=\s*new\s+\w*Socket\([^;]*?\)\s*\{[^}]*\bRetired\s*=\s*true",
    re.S,
)

# A write is either a value assignment or a Connect/ToList target on .ins.<Name>.
def write_re(name: str) -> re.Pattern[str]:
    return re.compile(r"\.ins\.%s\b" % re.escape(name))


# Known and accepted, with the reason. Per file, so a new call site elsewhere still reports.
ACCEPTED: set[tuple[str, str]] = set()


def retired_sockets() -> dict[str, list[str]]:
    """Retired socket property names -> the csycles files declaring them."""
    found: dict[str, list[str]] = {}
    for path in sorted(SHADER_NODES.rglob("*.cs")):
        src = path.read_text(encoding="utf-8", errors="replace")
        for m in RETIRED_RE.finditer(src):
            found.setdefault(m.group(1), []).append(path.name)
    return found


def main() -> int:
    if not SHADER_NODES.is_dir():
        print("no csycles ShaderNodes directory at %s" % SHADER_NODES)
        return 2
    if not RHINOCYCLES.is_dir():
        print("no RhinoCycles checkout at %s - skipping" % RHINOCYCLES)
        return 0

    retired = retired_sockets()
    if not retired:
        print("no retired sockets declared in csycles - nothing to audit")
        return 0

    patterns = {name: write_re(name) for name in retired}
    problems: list[str] = []
    accepted_hits = 0

    for path in sorted(RHINOCYCLES.rglob("*.cs")):
        try:
            lines = path.read_text(encoding="utf-8", errors="replace").splitlines()
        except OSError:
            continue
        for n, line in enumerate(lines, 1):
            if ".ins." not in line:
                continue
            for name, pat in patterns.items():
                if not pat.search(line):
                    continue
                if (path.name, name) in ACCEPTED:
                    accepted_hits += 1
                    continue
                problems.append(
                    "%s:%d writes .ins.%s, retired in %s\n      %s"
                    % (path.name, n, name, ", ".join(retired[name]), line.strip())
                )

    print(
        "audit_retired_socket_writes: %d retired socket(s), %d accepted write(s), "
        "%d problem(s)" % (len(retired), accepted_hits, len(problems))
    )
    for p in problems:
        print("  " + p)
    if problems:
        print(
            "\n  A write to a retired socket is dropped in silence. Check what else sets "
            "that\n  parameter: if nothing does, the node is rendering on a Cycles "
            "default nobody chose."
        )
    return 1 if problems else 0


if __name__ == "__main__":
    sys.exit(main())
