#!/usr/bin/env bash
#
# Bake portable rpaths into a freshly built libccycles.dylib so the RhinoCycles
# kernel compiler can find the Cycles dependency dylibs on Mac. `make release`
# bakes in machine-specific absolute rpaths and deploys the deps into the Rhino
# app's Frameworks dir; this strips the absolute rpaths and adds @loader_path
# rpaths that reach the deps from each place the dylib is deployed. See RH-96549.
#
# Run from RDK/cycles/cycles after `make release`, before copying install/* to big_libs:
#   ./fix-cycles-rpaths.sh [path/to/libccycles.dylib]   # default: install/libccycles.dylib

set -euo pipefail

DYLIB="${1:-install/libccycles.dylib}"

rpaths() { otool -l "$DYLIB" | awk '/LC_RPATH/{f=1} f && / path /{print $2; f=0}' | sort -u; }

# Clear every rpath, then add exactly the three portable ones. make release bakes in
# only machine-specific absolute rpaths, so there is nothing here worth preserving, and
# starting from empty makes the result exact and re-running safe.
for rp in $(rpaths); do
	install_name_tool -delete_rpath "$rp" "$DYLIB"
done

# @loader_path            - the Contents/Frameworks copy, deps are siblings
# six levels up           - the ManagedPlugIns copy inside the app bundle
# six levels up + the app - the ManagedPlugIns copy at a local build's products root
for rp in \
	"@loader_path" \
	"@loader_path/../../../../../.." \
	"@loader_path/../../../../../../Rhinoceros.app/Contents/Frameworks"; do
	install_name_tool -add_rpath "$rp" "$DYLIB"
done

echo "fix-cycles-rpaths: done. Final rpaths in $DYLIB:"
rpaths | sed 's/^/    /'
