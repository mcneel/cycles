"""Audit the SVM interpreter for a case that falls through into the next one.

Rhino splices its `RHINO_NODE_*` cases into upstream's switch in `kernel/svm/svm.h`. A
case that loses its `break` runs the next handler on the wrong node data, which overwrites
its output - so edits to the node itself change nothing (`NODE_SET_BUMP` falling into
`RHINO_NODE_TEX_COORD` turned every bump-mapped surface black).

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
