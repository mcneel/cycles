#!/usr/bin/env python3
"""Check that add_node_packed is only used for Rhino's own SVM node types.

5.2 replaced the packed add_node(type, a, b, c) SVM encoding with per-node
structs. The port added SVMCompiler::add_node_packed to keep the old wire
format, which is correct for the RHINO_NODE_* types because their kernel readers
in kernel/svm/svm_rhino_procedurals.h were left on the old layout as well. It is
wrong for any stock Cycles node, whose kernel reader now decodes a struct.

Nothing catches that mistake: it compiles, links, asserts nothing, and writes
the node's fields to the wrong places.

RhinoTextureCoordinateNode emitted NODE_GEOMETRY and NODE_ATTR through the
packed path, and SVMNodeGeometry packs geom_type, bump_offset,
store_derivatives and out_offset as four bytes of a single uint. One value per
uint meant out_offset was read from byte 3 of the first uint - zero - so the
node wrote its result to stack slot 0 while the consumer read the slot it had
been promised. rhino_texture_coordinate's Generated output came back as
(0,0,0), every environment texture sampled a degenerate direction, and every
render was black.

Run it from anywhere:

    python tools/audit_svm_nodes.py

Exits non-zero on any stock node type reached through the packed path.
"""

import os
import re
import sys

REPO = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SCENE = os.path.join(REPO, "src", "scene")

# The overloads that continue a node already begun by a typed call. Their first
# argument is payload, not a node type, so there is nothing to check.
CONTINUATION_FIRST_ARGS = re.compile(
    r"^\s*(?:"
    r"__float_as_int|encode_uchar4|compiler\.encode_uchar4|make_float\d?|"
    r"ob_tfm|ob_itfm|tfm|uvw|pxyz|nxyz|cells|dots_data_count|point_height|"
    r"const\b|uint\b"
    r")"
)


def resolve_type_vars(src):
    """Local aliases such as: ShaderNodeType texco_node = RHINO_NODE_TEX_COORD;"""
    out = {}
    for m in re.finditer(r"\bShaderNodeType\s+(\w+)\s*=\s*([A-Za-z_][\w:]*)\s*;", src):
        out[m.group(1)] = m.group(2).split("::")[-1]
    return out


def first_argument(src, open_paren):
    """Text of the first argument of a call whose '(' is at open_paren."""
    depth, i, start = 0, open_paren, open_paren + 1
    while i < len(src):
        c = src[i]
        if c in "([":
            depth += 1
        elif c in ")]":
            depth -= 1
            if depth == 0:
                return src[start:i]
        elif c == "," and depth == 1:
            return src[start:i]
        i += 1
    return ""


def line_of(src, index):
    return src.count("\n", 0, index) + 1


def main():
    if not os.path.isdir(SCENE):
        print("cannot find %s" % SCENE)
        return 2

    problems, checked = [], 0
    for fn in sorted(os.listdir(SCENE)):
        if not fn.endswith(".cpp"):
            continue
        path = os.path.join(SCENE, fn)
        src = open(path, encoding="utf8", errors="replace").read()
        aliases = resolve_type_vars(src)

        for m in re.finditer(r"\badd_node_packed\s*\(", src):
            arg = first_argument(src, m.end() - 1).strip()
            if not arg:
                continue
            checked += 1
            if CONTINUATION_FIRST_ARGS.match(arg):
                continue
            name = arg.split("::")[-1].strip()
            resolved = aliases.get(name, name)
            if resolved.startswith("RHINO_NODE_"):
                continue
            # A float3/float4 payload overload, or something we cannot resolve.
            if not re.match(r"^[A-Za-z_]\w*$", resolved):
                continue
            if not resolved.startswith("NODE_") and resolved not in aliases.values():
                continue
            problems.append((fn, line_of(src, m.start()), arg.strip(), resolved))

    for fn, line, arg, resolved in problems:
        shown = arg if arg == resolved else "%s (= %s)" % (arg, resolved)
        print("STOCK NODE  %s:%d  add_node_packed(%s)" % (fn, line, shown))
    if problems:
        print()
        print("These reach a stock Cycles node through the packed layout, which its")
        print("kernel reader no longer uses. Emit them as")
        print("  compiler.add_node(this, TYPE, SVMNodeXxx{...})")
        print("copying the field list from the stock node's own compile() nearby.")

    print()
    print("%d add_node_packed call(s) checked, %d problem(s)" % (checked, len(problems)))
    return 1 if problems else 0


if __name__ == "__main__":
    sys.exit(main())
