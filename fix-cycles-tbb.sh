#!/usr/bin/env bash
#
# Rename the Mac payload's oneTBB to a private name (RH-98415). Rhino's Frameworks already
# has libtbb.dylib (TBB 2020.3, which USD needs; whichever copy lands last breaks the other's
# users) and libtbb.12.dylib (oneTBB 12.7, too old for Cycles and also used by OIDN and ODA).
# fix-cycles-sxs.ps1 solves the same on Windows. `make release` runs this; by hand:
#   ./fix-cycles-tbb.sh [install-dir]        # default: install
# Signing is left alone: install_name_tool invalidates it, and the Rhino build re-signs.

set -euo pipefail

INSTALL="${1:-install}"
LIB="$INSTALL/lib"
OLD="@rpath/libtbb.dylib"
NEW="@rpath/libtbb.12.cycles.dylib"

# Rename and repoint are checked separately: each `make release` relinks libccycles.dylib
# against the old name even when a previous run already renamed lib/.
if [ -f "$LIB/libtbb.dylib" ]; then
	# oneTBB, not 2020.3, or we would be renaming the wrong thing.
	if ! strings -a "$LIB/libtbb.dylib" | grep -q "oneTBB"; then
		echo "fix-cycles-tbb: $LIB/libtbb.dylib is not oneTBB - refusing" >&2
		exit 1
	fi
	mv "$LIB/libtbb.dylib" "$LIB/libtbb.12.cycles.dylib"
	install_name_tool -id "$NEW" "$LIB/libtbb.12.cycles.dylib"
	echo "  renamed libtbb.dylib -> libtbb.12.cycles.dylib"
elif [ ! -f "$LIB/libtbb.12.cycles.dylib" ]; then
	echo "fix-cycles-tbb: no oneTBB in $LIB" >&2
	exit 1
fi

# Every payload binary that referenced the old name, plus libccycles itself.
for f in "$LIB"/*.dylib "$INSTALL/libccycles.dylib"; do
	[ -f "$f" ] || continue
	if otool -L "$f" | grep -q "$OLD"; then
		install_name_tool -change "$OLD" "$NEW" "$f"
		echo "  repointed $(basename "$f")"
	fi
done

# Drop oneTBB's allocator: nothing in the payload links it, and its filenames clash with
# the TBB 2020.3 allocator Rhino ships in Contents/Frameworks.
for m in libtbbmalloc.dylib libtbbmalloc_proxy.dylib; do
	[ -f "$LIB/$m" ] || continue
	for f in "$LIB"/*.dylib "$INSTALL/libccycles.dylib"; do
		case "$(basename "$f")" in libtbbmalloc.dylib|libtbbmalloc_proxy.dylib) continue ;; esac
		if otool -L "$f" | tail -n +2 | grep -q "/$m "; then
			echo "fix-cycles-tbb: $(basename "$f") links $m - it has to be renamed like libtbb, not dropped" >&2
			exit 1
		fi
	done
	rm "$LIB/$m"
	echo "  dropped $m"
done

echo "fix-cycles-tbb: done - payload oneTBB is libtbb.12.cycles.dylib"

# Nothing may still reference the old name, or it will resolve to Rhino's TBB 2020.3.
for f in "$LIB"/*.dylib "$INSTALL/libccycles.dylib"; do
	[ -f "$f" ] || continue
	if otool -L "$f" | grep -q "$OLD"; then
		echo "fix-cycles-tbb: $(basename "$f") still references $OLD" >&2
		exit 1
	fi
done
