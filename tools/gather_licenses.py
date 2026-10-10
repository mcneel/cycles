"""Write src/doc/license/THIRD-PARTY-LICENSES.txt for the libraries the payload ships.

usage: python tools/gather_licenses.py <Blender license.md>

Take license.md from the Blender release whose library bundle lib/ is on: license/license.md
in a Blender install, or release/license/license.md in Blender's repository. Re-run it after
a library bundle update, and edit LIBRARIES when the payload gains or loses a library
(dumpbin /dependents and otool -L show the DLLs; static ones need a look at the binaries).
RhinoCycles.csproj deploys the result with the other license texts, on both platforms.
"""
import os
import re
import sys

# Blender's library name -> where it ships. A name missing from license.md is an error.
LIBRARIES = {
    "Embree": "embree4.dll, libembree4.dylib",
    "oneTBB": "tbb12.dll, tbbmalloc*.dll, libtbb.12.cycles.dylib",
    "OpenImageIO": "OpenImageIO*.dll, libOpenImageIO*.dylib",
    "DPC++": "sycl8.dll, ur_*.dll (Windows)",
    "Cuda Wrangler": "ccycles",
    "HIP Extension Wrangler Library (HIPEW)": "ccycles",
    "Aom": "aom.dll (Windows), inside libOpenImageIO.dylib (macOS)",
    "OpenJPEG": "inside OpenImageIO",
    "OpenJPH": "openjph*.dll, libopenjph.dylib",
    "xxHash Library": "inside OpenImageIO_Util",
    "Imath": "imath.dll, libImath.dylib",
    "Openexr": "Iex, IlmThread, OpenEXR*.dll and .dylib",
    "OpenColorIO": "OpenColorIO*.dll, libOpenColorIO.dylib",
    "libjpeg-turbo": "inside OpenImageIO",
    "Webp": "inside OpenImageIO",
    "Pystring": "inside OpenColorIO",
    "Blosc": "inside OpenVDB",
    "Zstd": "inside OpenVDB",
    "Libheif": "inside OpenImageIO",
    "libpng": "inside OpenImageIO",
    "LibTIFF": "inside OpenImageIO",
    "oneAPI Level Zero": "ze_*.dll (Windows)",
    "Expat": "inside OpenColorIO",
    "Fmt": "inside OpenImageIO",
    "Pugixml": "inside OpenImageIO",
    "Robinmap": "inside OpenImageIO",
    "Deflate": "inside OpenEXR and OpenImageIO",
    "sse2neon": "ccycles (macOS)",
    "OpenVDB": "openvdb.dll, libopenvdb.dylib (with NanoVDB)",
    "OpenSubdiv": "inside ccycles.dll (Windows), libosd*.dylib (macOS)",
    "Zlib": "inside OpenImageIO, OpenColorIO and OpenVDB",
    "minizip-ng": "inside OpenColorIO",
}

# Inside a shipped binary but not in Blender's list: (license section, version, copyright,
# url, ships in).
EXTRA = {
    "yaml-cpp": ("MIT", "bundled with OpenColorIO", "Copyright (c) 2008-2015 Jesse Beder.",
                 "https://github.com/jbeder/yaml-cpp", "inside OpenColorIO"),
}

GPL3 = "GNU General Public License v3.0 or later"
LGPL3 = "GNU Lesser General Public License v3.0 or later"


def parse(md):
    """Sections in order: name -> {"url", "rows": [(lib, url, version, copyright)], "text"}."""
    sections, order = {}, []
    current = None
    lines = md.splitlines()
    i = 0
    while i < len(lines):
        line = lines[i]
        m = re.match(r"^## (?:\[(.+?)\]\((.*?)\)|(.+))", line)
        if m:
            current = m.group(1) or m.group(3).strip()
            sections[current] = {"url": m.group(2) or "", "rows": [], "text": None}
            order.append(current)
        m = re.match(r"^\| \[(.+?)\]\((.*?)\)\S* \| (.*?) \| `(.*?)` \|", line)
        if m and current:
            sections[current]["rows"].append((m.group(1), m.group(2), m.group(3).strip(), m.group(4).strip()))
        m = re.match(r"^(?:</details>)?<details>$", line)
        if m:
            summary = re.match(r"^<summary>(.*?)</summary>", lines[i + 1]).group(1)
            body = []
            i += 2
            while not lines[i].startswith("</details>"):
                body.append(lines[i])
                i += 1
            name = summary if current is None or summary == GPL3 else current
            if summary.startswith("¹"):  # LLVM exception, an addendum to Apache
                name = None
            if name and sections.setdefault(name, {"url": "", "rows": [], "text": None})["text"] is None:
                sections[name]["text"] = "\n".join(body).strip("\n")
                if name not in order:
                    order.append(name)
            continue
        i += 1
    return sections, order


def main():
    md = open(sys.argv[1], encoding="utf-8").read()
    sections, order = parse(md)
    found = {}
    for name in order:
        for lib, url, version, copyright in sections[name]["rows"]:
            if lib in LIBRARIES and lib not in found:
                found[lib] = (name, version, copyright, url, LIBRARIES[lib])
    missing = sorted(set(LIBRARIES) - set(found))
    if missing:
        sys.exit("not in license.md: " + ", ".join(missing))
    found.update(EXTRA)

    used = [n for n in order if any(v[0] == n for v in found.values())]
    if LGPL3 in used and GPL3 not in used:
        used.append(GPL3)  # LGPL-3.0 is a set of additions to the GPL-3.0 and needs its text

    out = [
        "Third-party software in Rhino Render (Cycles 5)",
        "",
        "Rhino Render's Cycles payload contains the libraries below, prebuilt by the Blender",
        "project. Cycles itself is Apache 2.0 (Apache2-license.txt); code it adapted from",
        "elsewhere is listed in SPDX-license-identifiers.txt.",
        "",
        "Generated by cycles-core/tools/gather_licenses.py from Blender's license.md.",
    ]
    for name in used:
        out += ["", "", "=" * 78, name, "=" * 78, ""]
        rows = sorted((lib, v) for lib, v in found.items() if v[0] == name)
        if name == GPL3 and not rows:
            out += ["Included because the GNU LGPL v3.0 above incorporates it.", ""]
        for lib, (_, version, copyright, url, ships) in rows:
            out += ["%s %s - %s" % (lib, version, url), "    " + copyright, "    Ships in: " + ships, ""]
        text = sections[name]["text"]
        if not text:
            sys.exit("no license text for " + name)
        out += ["-" * 78, "", text]

    dest = os.path.join(os.path.dirname(os.path.abspath(__file__)), "..", "src", "doc", "license",
                        "THIRD-PARTY-LICENSES.txt")
    with open(dest, "w", encoding="utf-8", newline="\n") as f:
        f.write("\n".join(out).rstrip() + "\n")
    print("wrote %s: %d libraries, %d licenses" % (os.path.normpath(dest), len(found), len(used)))


if __name__ == "__main__":
    main()
