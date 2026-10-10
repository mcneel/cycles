#!/usr/bin/env bash
#
# Replace the build's machine-specific absolute rpaths in libccycles.dylib with
# @loader_path ones that reach the deps wherever it is deployed (RH-96549).
# `make release` runs this on Mac; by hand only for other builds:
#   ./fix-cycles-rpaths.sh [path/to/libccycles.dylib]   # default: install/libccycles.dylib

set -euo pipefail

DYLIB="${1:-install/libccycles.dylib}"

rpaths() { otool -l "$DYLIB" | awk '/LC_RPATH/{f=1} f && / path /{print $2; f=0}' | sort -u; }

# Start empty: every baked-in rpath is machine-specific, and this makes re-running safe.
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
