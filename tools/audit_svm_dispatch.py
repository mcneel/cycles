"""Audit the SVM interpreter for a case that falls through into the next one.

A fifth way this port drifts silently. Rhino splices its own `RHINO_NODE_*` cases into
upstream's `switch` in `kernel/svm/svm.h`. If the case above an inserted one loses its
`break`, execution runs the inserted handler immediately afterwards: it reads a packed
node from the *current* instruction offset, so it misinterprets whatever node follows,
writes into stack slots that belong to other nodes, and advances the offset wrongly.

Nothing throws. The node whose case fell through appears to work - it computes and
stores its value correctly - and then the fall-through overwrites it. That is what makes
this so expensive to find: every experiment aimed at the falling-through node itself
comes back identical, because its output is clobbered a few instructions later.

Found 2026-08-29. `NODE_SET_BUMP` had lost its `break` and fell into
`RHINO_NODE_TEX_COORD`, which turned every bump-mapped surface black -
`Brian25YearRhinoGlas.3dm` rendered its tabletop at mean 7.2 against shipping's 62.2.
Five separate edits to `svm_node_set_bump` all rendered identically before the dispatch
itself was checked.

Exit code 0 if every SVM_CASE terminates, 1 otherwise.
"""

from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent
SVM_H = ROOT / "src" / "kernel" / "svm" / "svm.h"

CASE_RE = re.compile(r"^\s*SVM_CASE\((\w+)\)")
# A case is terminated by break/return/continue somewhere in its body. Nested switches
# would fool this, and there are none here; if one is added, tighten to brace depth.
TERMINATOR_RE = re.compile(r"\b(?:break|return|continue)\b")


def main() -> int:
    if not SVM_H.is_file():
        print(f"svm.h not found at {SVM_H}")
        return 1

    lines = SVM_H.read_text(encoding="utf-8", errors="replace").splitlines()
    starts = [i for i, line in enumerate(lines) if CASE_RE.match(line)]
    if not starts:
        print("no SVM_CASE labels found - has the interpreter been restructured?")
        return 1

    unterminated: list[tuple[int, str, str]] = []
    for start, nxt in zip(starts, starts[1:] + [len(lines)]):
        body = lines[start + 1:nxt]
        if any(TERMINATOR_RE.search(line) for line in body):
            continue
        name = CASE_RE.match(lines[start]).group(1)
        falls_into = "the end of the switch"
        if nxt < len(lines):
            m = CASE_RE.match(lines[nxt])
            if m:
                falls_into = m.group(1)
        unterminated.append((start + 1, name, falls_into))

    for line_no, name, falls_into in unterminated:
        print(f"svm.h:{line_no}: {name} has no break and falls into {falls_into} - "
              f"its output will be overwritten and the instruction offset misread")

    if unterminated:
        print(f"svm dispatch: {len(unterminated)} unterminated case(s) "
              f"of {len(starts)}")
        return 1
    print(f"svm dispatch: all {len(starts)} cases terminate")
    return 0


if __name__ == "__main__":
    sys.exit(main())
