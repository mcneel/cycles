#!/usr/bin/env python3
"""Check csycles' enums against the Cycles ones they mirror.

csycles passes these across the C API as plain integers, so a member that has
drifted is not a compile error on either side - it is a value that quietly means
something else. That has happened four times on the 5.2 port:

  PathRay      carried the pre-4.x layout where visibility doubled as the path
               flags, so AllVisibility was 0x3fff against a legal 0x7f and
               Object::visibility_for_tracing() asserted. Transmit, Diffuse,
               Glossy and the shadow bits had also renumbered.
  DeviceType   was missing HIPRT, so DeviceTypeMask.METAL was HIPRT's bit.
  PassType     was missing three entries and had the denoising passes inline in
               the data range, putting everything from Mist onward one or more
               places out.

Run it from anywhere:

    python tools/audit_enums.py

Exits non-zero if any mapped enum disagrees, so it can gate a build.
"""

import os
import re
import sys

REPO = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SRC = os.path.join(REPO, "src")

# csycles enum -> (Cycles enum, member prefix to strip)
#
# Only enums whose numbering crosses the C API belong here. Anything Rhino
# invented has no counterpart and is listed in RHINO_ONLY instead, so the
# coverage this tool actually gives you stays visible.
# csycles enum -> (Cycles enum, member prefix, {csycles name: cycles name} aliases)
#
# Only enums whose numbering crosses the C API belong here. Anything Rhino
# invented has no counterpart and is listed in RHINO_ONLY instead, so the
# coverage this tool actually gives you stays visible.
#
# Aliases exist where csycles chose a different word for the same member. They
# are deliberately explicit: an alias is a claim that two names mean the same
# thing, and that claim should be reviewed rather than guessed by the tool.
MAPPING = {
    "DeviceType": ("DeviceType", "DEVICE_", {}),
    "ShadingSystem": ("ShadingSystem", "SHADINGSYSTEM_", {}),
    "SamplingPattern": ("SamplingPattern", "SAMPLING_PATTERN_", {}),
    "BvhLayout": ("KernelBVHLayout", "BVH_LAYOUT_", {}),  # ccl::BVHLayout aliases this
    "CameraType": ("CameraType", "CAMERA_", {}),
    "PanoramaType": ("PanoramaType", "PANORAMA_", {}),
    "FilterType": ("FilterType", "FILTER_", {}),
    "LightType": ("LightType", "LIGHT_", {"Distant": "SUN"}),
    "InterpolationType": ("InterpolationType", "INTERPOLATION_", {}),
    "PathRay": ("PathRayVisibilityFlag", "PATH_RAY_VISIBILITY_",
                {"Hidden": "NONE", "AllVisibility": "ALL"}),
    "PassType": ("PassType", "PASS_", {}),
}

# Count sentinels and the like: present upstream, no reason for csycles to carry
# them, and their absence says nothing about drift.
IGNORE_MEMBERS = {
    "NUM_TYPES", "SAMPLING_NUM_PATTERNS", "NUM", "PASS_NUM",
    "CATEGORY_LIGHT_END", "CATEGORY_DATA_END", "CATEGORY_DENOISING_END",
    "CATEGORY_BAKE_END",
}

RHINO_ONLY = {
    "DecalDirection",  # Rhino decals, no Cycles equivalent
    "DeviceTypeMask",  # derived from DeviceType bits, checked implicitly
    "IntegratorMethod",  # retired upstream; csycles keeps it for old callers
    "BvhType",  # BVHType lives in bvh/params.h and is not passed over the API
}


def normalise(name):
    return name.replace("_", "").lower()


def evaluate(expr, known):
    """Evaluate an enum initialiser using the members already parsed."""
    expr = re.sub(r"\b(\d+)[uUlL]+\b", r"\1", expr)  # 1U -> 1
    expr = expr.replace("~", " ~ ")
    try:
        return int(eval(expr, {"__builtins__": {}}, dict(known)))  # noqa: S307
    except Exception:
        return None


def parse_members(body, prefix=""):
    """Parse an enum body into an ordered list of (name, value)."""
    body = re.sub(r"/\*.*?\*/", "", body, flags=re.S)
    body = re.sub(r"//[^\n]*", "", body)
    body = re.sub(r"///[^\n]*", "", body)
    members, known, nxt = [], {}, 0
    for item in body.split(","):
        item = item.strip()
        if not item:
            continue
        if "=" in item:
            name, expr = item.split("=", 1)
            name = name.strip()
            value = evaluate(expr.strip(), known)
            if value is None:
                continue
        else:
            name, value = item, nxt
        name = name.strip()
        if not re.match(r"^[A-Za-z_][A-Za-z0-9_]*$", name):
            continue
        short = name[len(prefix):] if prefix and name.startswith(prefix) else name
        known[name] = value
        known[short] = value
        members.append((short, value))
        nxt = value + 1
    return members


def cs_enums(path):
    src = open(path, encoding="utf8", errors="replace").read()
    out = {}
    for m in re.finditer(r"public\s+enum\s+(\w+)\s*(?::\s*\w+\s*)?\{(.*?)\n\t\}", src, re.S):
        out[m.group(1)] = parse_members(m.group(2))
    return out


def ccl_enums(root):
    out = {}
    for dirpath, _dirs, files in os.walk(root):
        for fn in files:
            if not fn.endswith((".h", ".hpp")):
                continue
            path = os.path.join(dirpath, fn)
            try:
                src = open(path, encoding="utf8", errors="replace").read()
            except OSError:
                continue
            for m in re.finditer(r"\benum\s+(?:class\s+)?(\w+)\s*(?::\s*[\w:]+\s*)?\{(.*?)\}\s*;",
                                 src, re.S):
                name = m.group(1)
                if name not in out:
                    out[name] = (m.group(2), path)
    return out


def main():
    cs_path = os.path.join(SRC, "csycles", "enums.cs")
    if not os.path.exists(cs_path):
        print("cannot find %s" % cs_path)
        return 2

    cs = cs_enums(cs_path)
    ccl = ccl_enums(SRC)

    problems = 0
    for cs_name in sorted(cs):
        if cs_name in RHINO_ONLY:
            continue
        if cs_name not in MAPPING:
            print("UNMAPPED  %s - add it to MAPPING or RHINO_ONLY" % cs_name)
            problems += 1
            continue
        ccl_name, prefix, aliases = MAPPING[cs_name]
        if ccl_name not in ccl:
            print("MISSING   %s -> ccl::%s not found in the headers" % (cs_name, ccl_name))
            problems += 1
            continue
        body, where = ccl[ccl_name]
        want = [(k, v) for k, v in parse_members(body, prefix) if k not in IGNORE_MEMBERS]
        have = {aliases.get(k, k): v for k, v in cs[cs_name]}
        want_by_norm = {normalise(k): v for k, v in want}
        have_norms = {normalise(x) for x in have}

        bad = [(k, v, want_by_norm[normalise(k)])
               for k, v in have.items()
               if normalise(k) in want_by_norm and want_by_norm[normalise(k)] != v]
        absent = [k for k, _ in want if normalise(k) not in have_norms]
        shared = len(have_norms & set(want_by_norm))

        # A rename of every member leaves nothing to compare, which would
        # otherwise read as a clean pass.
        thin = shared == 0 or (shared < len(have) / 2 and len(have) > 2)

        if bad or absent or thin:
            problems += 1
            print("DRIFT     %s vs ccl::%s  (%s)"
                  % (cs_name, ccl_name, os.path.relpath(where, REPO)))
            if thin:
                print("            only %d of csycles' %d members share a name with Cycles -"
                      % (shared, len(have)))
                print("            too little overlap to conclude anything; check by hand")
            for k, mine, theirs in bad:
                print("            %-40s csycles=%-6s cycles=%s" % (k, mine, theirs))
            for k in absent:
                print("            %-40s missing from csycles" % k)
        else:
            print("ok        %-18s %d members, %d shared" % (cs_name, len(want), shared))

    print()
    print("%d enum(s) checked, %d unchecked by choice, %d problem(s)"
          % (len(MAPPING), len(RHINO_ONLY), problems))
    return 1 if problems else 0


if __name__ == "__main__":
    sys.exit(main())
