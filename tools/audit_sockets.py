#!/usr/bin/env python3
"""Check csycles' shader node definitions against the Cycles they are built on.

Cycles renames and retires shader node types and sockets between releases, and
nothing in either language catches it: a wrong node type name is a null
dereference, a wrong socket ui name is a connection that silently finds nothing,
and a wrong internal name is a value that silently goes nowhere. The 3.5 -> 5.2
merge hit all three, one Rhino debug session at a time.

Run from the repo root:   python tools/audit_sockets.py
Exits 1 if anything real is wrong, so it can be used as a build gate.
"""
import re, glob, os, sys

SRC = 'src'
CYCLES_NODE_SOURCES = ['scene/shader_nodes.cpp', 'scene/rhino_shader_nodes.cpp']


def cycles_nodes():
    """type name -> {'in','out','param'}: ui name -> internal name.

    Also 'svm_internal': the ui names Cycles marks SVM_INTERNAL, which exist only
    for the compiler and are not something a wrapper should expose.
    """
    src = ''
    for f in CYCLES_NODE_SOURCES:
        p = os.path.join(SRC, f)
        if os.path.exists(p):
            src += open(p, encoding='utf-8', errors='replace').read()
    out = {}
    for m in re.finditer(r'NODE_DEFINE\((\w+)\)\s*\{', src):
        i, depth = m.end() - 1, 0
        while i < len(src):
            if src[i] == '{':
                depth += 1
            elif src[i] == '}':
                depth -= 1
                if depth == 0:
                    break
            i += 1
        body = src[m.end():i]
        tn = re.search(r'NodeType::add\(\s*"([^"]+)"', body)
        if not tn:
            continue
        ent = {'in': {}, 'out': {}, 'param': {}, 'svm_internal': set()}
        # The tail carries the flags, but it must not run past the next SOCKET_:
        # some calls (base_color) wrap across lines, so keying on ');' swallowed
        # the socket that followed.
        pat = (r'SOCKET_(IN_|OUT_)?(\w+)\(\s*(\w+)\s*,\s*"([^"]*)"'
               r'(.*?)(?=SOCKET_|\Z)')
        for sm in re.finditer(pat, body, re.S):
            direction, internal, ui, rest = sm.group(1), sm.group(3), sm.group(4), sm.group(5)
            key = 'in' if direction == 'IN_' else 'out' if direction == 'OUT_' else 'param'
            ent[key][ui] = internal
            if 'SVM_INTERNAL' in rest:
                ent['svm_internal'].add(ui)
        out[tn.group(1)] = ent
    return out


def strip_comments(s):
    """Commented-out socket declarations are not declarations."""
    s = re.sub(r'/\*.*?\*/', '', s, flags=re.S)
    return re.sub(r'//[^\n]*', '', s)


def csycles_nodes():
    """(cycles type, class, file, inputs container, outputs container) plus containers."""
    containers = {}
    for f in sorted(glob.glob(os.path.join(SRC, 'csycles/ShaderNodes/**/*.cs'), recursive=True)):
        s = strip_comments(open(f, encoding='utf-8', errors='replace').read())
        for cm in re.finditer(r'class\s+(\w+)\s*:\s*(Inputs|Outputs)\b', s):
            which = 'in' if cm.group(2) == 'Inputs' else 'out'
            after = s[cm.end():]
            nxt = re.search(r'\n\t(?:public|internal)\s+class ', after)
            blk = after[:nxt.start()] if nxt else after
            socks = {}
            for sm in re.finditer(
                    r'new\s+\w*Socket\s*\(\s*\w+\s*,\s*"([^"]*)"(?:\s*,\s*"([^"]*)")?([^;]*)', blk):
                socks[sm.group(1)] = (sm.group(2), 'Retired' in (sm.group(3) or ''))
            containers[cm.group(1)] = (which, socks)

    nodes = []
    for f in sorted(glob.glob(os.path.join(SRC, 'csycles/ShaderNodes/**/*.cs'), recursive=True)):
        s = strip_comments(open(f, encoding='utf-8', errors='replace').read())
        marks = [(m.start(), m.group(1), m.group(2)) for m in re.finditer(
            r'\[ShaderNode\("([^"]+)"\)\]\s*(?:\r?\n\s*)*public\s+(?:sealed\s+)?class\s+(\w+)', s)]
        for idx, (pos, attr, cls) in enumerate(marks):
            blk = s[pos:marks[idx + 1][0] if idx + 1 < len(marks) else len(s)]
            ov = re.search(r'ShaderNodeTypeName\s*=>\s*"([^"]+)"', blk)
            ins = re.search(r'\((\w+Inputs)\)\s*inputs', blk)
            outs = re.search(r'\((\w+Outputs)\)\s*outputs', blk)
            nodes.append((ov.group(1) if ov else attr, cls, os.path.basename(f),
                          ins.group(1) if ins else None, outs.group(1) if outs else None))
    return nodes, containers


def main():
    cyc = cycles_nodes()
    nodes, containers = csycles_nodes()
    if not cyc or not nodes:
        print('audit_sockets: found no node definitions - run me from the repo root')
        return 2

    # Node types that upstream retired and that Rhino does not instantiate. Listed
    # so the audit stays silent about them while still failing on anything new.
    known_missing_types = {'anisotropic_bsdf', 'musgrave_texture', 'velvet_bsdf',
                           'shadernode base', 'texture_node_base'}

    bad_types, bad_sockets, bad_internal = [], [], []
    for ctype, cls, base, inc, outc in nodes:
        if ctype not in cyc:
            if ctype not in known_missing_types:
                bad_types.append((ctype, cls, base))
            continue
        for cname, which in ((inc, 'in'), (outc, 'out')):
            if not cname or cname not in containers:
                continue
            for ui, (internal, retired) in containers[cname][1].items():
                if retired:
                    continue
                if ui in cyc[ctype][which]:
                    want = cyc[ctype][which][ui]
                    if internal and internal != 'UNSET' and internal.lower() != want.lower():
                        bad_internal.append((ctype, which, ui, internal, want, base))
                elif not any(ui.lower() == k.lower() for k in cyc[ctype]['param']):
                    bad_sockets.append((ctype, which, ui, base, sorted(cyc[ctype][which])))

    for ctype, cls, base in bad_types:
        print(f"NODE TYPE  {ctype!r} ({cls} in {base}) is not registered by this Cycles")
    for ctype, which, ui, base, have in bad_sockets:
        print(f"SOCKET     {ctype} {which} {ui!r} ({base}) does not exist; Cycles has {have}")
    for ctype, which, ui, internal, want, base in bad_internal:
        print(f"INTERNAL   {ctype} {which} {ui!r} ({base}) internal={internal!r}, Cycles wants {want!r}")

    # Sockets Cycles has that csycles never exposes. Not a defect - they fall back
    # to Cycles' own defaults - but it is what Rhino cannot drive, and the list is
    # invisible otherwise. Off by default so this stays a clean build gate.
    if '--unexposed' in sys.argv:
        print()
        total = 0
        for ctype, cls, base, inc, outc in sorted(nodes):
            if ctype not in cyc:
                continue
            exposed = set()
            for cname in (inc, outc):
                if cname and cname in containers:
                    exposed |= {ui.lower() for ui in containers[cname][1]}
            missing = [ui for ui in sorted(cyc[ctype]['in'])
                       if ui.lower() not in exposed
                       and ui not in cyc[ctype].get('svm_internal', set())]
            if missing:
                total += len(missing)
                print('UNEXPOSED  %s: %s' % (ctype, ', '.join(missing)))
        print()
        print('%d socket(s) Cycles offers that csycles does not expose' % total)

    n = len(bad_types) + len(bad_sockets) + len(bad_internal)
    print(f"\naudit_sockets: {len(nodes)} csycles nodes vs {len(cyc)} Cycles node types - "
          f"{n} problem(s)")
    return 1 if n else 0


if __name__ == '__main__':
    sys.exit(main())
