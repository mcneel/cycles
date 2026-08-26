#!/usr/bin/env python3
"""Check that Rhino's additions to *stock* Cycles shader nodes are still wired up.

Rhino does not only add RHINO_NODE_* types. It also extends a handful of stock
Cycles nodes with extra sockets, and those sockets only do anything if the
node's SVM compile() emits them and the kernel reads them back.

Taking such a node from upstream at a Cycles bump deletes that wiring and
nothing complains: the socket stays declared, the kernel helper stays in the
file, compiler and kernel still agree with each other, and only the pixels are
wrong. That is exactly what happened to ImageTextureNode on the 3.5 -> 5.2 port,
where decals stopped being masked to their footprint and "Mirrored" texture
repeat stopped folding.

Declarations are not the thing that breaks, so auditing them is not enough -
this audits the *references* in compile() and in the kernel reader.

Exit code 0 if every expected wiring is present, 1 otherwise.
"""

from __future__ import annotations

import re
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent

# Rhino-added sockets on stock nodes, and where each has to be referenced.
#
# Every entry is a socket Rhino declares on a node type that Cycles owns. Each
# names the C++ identifier, the SVM compile() that must mention it, and the
# kernel function that must read it back. `optional_kernel` marks a socket that
# is deliberately declared but unused - keeping it in the table documents that
# the omission is a decision rather than the same bug.
EXPECTED = [
    {
        "node": "ImageTextureNode",
        "socket": "alternate_tiles",
        "why": 'Rhino "Mirrored" texture repeat folds the coordinate on alternate '
        "tiles; without it the texture wraps instead",
        "kernel_file": "src/kernel/svm/image.h",
        "kernel_func": "svm_node_tex_image",
    },
    {
        "node": "ImageTextureNode",
        "socket": "decalusage",
        "socket_name": "DecalUsage",
        "why": "masks a decal to its footprint; without it the decal image is "
        "sampled over the whole surface and its alpha never reaches zero",
        "kernel_file": "src/kernel/svm/image.h",
        "kernel_func": "svm_node_tex_image",
    },
    {
        "node": "ImageTextureNode",
        "socket": "decalforward",
        "socket_name": "DecalForward",
        "why": "decal facing test",
        "kernel_file": "src/kernel/svm/image.h",
        "kernel_func": "svm_node_tex_image",
        # RhinoTextureCoordinateNode::decal_setup allocates a stack slot for the
        # DecalForward output but decal_data_read has never written it - the
        # stack_store_float is commented out in 3.5 and in 5.2 alike. Recorded
        # so a future reader does not mistake it for this audit's failure mode.
        "optional_kernel": True,
        "optional_compile": True,
    },
]


def read(rel: str) -> str:
    return (ROOT / rel).read_text(encoding="utf-8-sig", errors="replace")


def function_body(text: str, pattern: str) -> str | None:
    """Return the body of the first function whose signature matches pattern."""
    m = re.search(pattern, text, re.M)
    if m is None:
        return None
    rest = text[m.end() :]
    end = rest.find("\n}")
    return rest if end == -1 else rest[:end]


def main() -> int:
    nodes_cpp = read("src/scene/shader_nodes.cpp")
    failures: list[str] = []

    for entry in EXPECTED:
        node = entry["node"]
        socket = entry["socket"]
        names = {socket, entry.get("socket_name", socket)}

        # 1. still declared?
        if not re.search(rf"SOCKET_\w+\(\s*{re.escape(socket)}\s*,", nodes_cpp):
            failures.append(f"{node}: socket '{socket}' is no longer declared")
            continue

        # 2. referenced by the SVM compile()?
        body = function_body(
            nodes_cpp, rf"^void {re.escape(node)}::compile\(SVMCompiler &compiler\)"
        )
        if body is None:
            failures.append(f"{node}: no compile(SVMCompiler&) found")
            continue
        if not entry.get("optional_compile") and not any(n in body for n in names):
            failures.append(
                f"{node}::compile(SVMCompiler&) never references '{socket}'.\n"
                f"    It {entry['why']}.\n"
                f"    Most likely this compile() was replaced with the upstream one."
            )

        # 3. read back by the kernel?
        if entry.get("optional_kernel"):
            continue
        kernel = read(entry["kernel_file"])
        kbody = function_body(
            kernel, rf"^ccl_device\w*\s+\w[\w:<>* ]*\b{re.escape(entry['kernel_func'])}\("
        )
        if kbody is None:
            failures.append(
                f"{entry['kernel_file']}: no {entry['kernel_func']} found"
            )
            continue
        if not any(n in kbody for n in names):
            failures.append(
                f"{entry['kernel_func']} in {entry['kernel_file']} never reads "
                f"'{socket}'.\n"
                f"    It {entry['why']}.\n"
                f"    Most likely this kernel node was replaced with the upstream one."
            )

    checked = len(EXPECTED)
    if failures:
        print(f"audit_rhino_stock_sockets: {len(failures)} problem(s) in {checked} wiring(s)\n")
        for f in failures:
            print(f"  - {f}")
        return 1

    print(f"audit_rhino_stock_sockets: {checked} Rhino socket wiring(s) intact")
    return 0


if __name__ == "__main__":
    sys.exit(main())
