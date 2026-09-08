#!/usr/bin/env bash
#
# Give the Mac payload's oneTBB a name of its own, so it stops colliding with the
# TBB that Rhino ships for USD. See RH-98415.
#
# The Mac deployment copies the payload's lib/ into Rhino's shared
# Contents/Frameworks. Both Rhino and Cycles want to own the filename
# `libtbb.dylib` there, and they are not interchangeable:
#
#   Rhino's  libtbb.dylib  is TBB 2020.3 - the pre-oneTBB API. USD needs it and
#                          uses 45 symbols that oneTBB does not have.
#   Cycles'  libtbb.dylib  is oneTBB. Cycles and its dependencies need 29
#                          tbb::detail::r1 symbols that 2020.3 does not have.
#
# Whichever copy lands last wins, and the loser's consumers then resolve against a
# library that cannot satisfy them. In practice Rhino's wins, and Cycles dies in
# ccl::TaskScheduler::init - or, once past that, in embree::TaskScheduler::create.
#
# So the payload's oneTBB is given a name nothing else uses. Rhino also ships a
# libtbb.12.dylib (oneTBB 12.7, for OpenImageDenoise and the ODA components), and
# that one is too old for Cycles - it lacks
# tbb::detail::r1::get_thread_reference_vertex. Taking over that filename would
# work, but it would upgrade a library those other components depend on in order
# to satisfy Cycles. A private name keeps the blast radius inside Cycles: if this
# is wrong, only Cycles breaks. It also matches Windows, where the payload already
# owns its dependencies.
#
# The cost is a third TBB runtime in the process. That is an oversubscription
# concern rather than a correctness one, and two already coexist (2020.3 and 12.7).
#
# `make release` runs this for you on Mac, together with fix-cycles-rpaths.sh, so a
# regenerated payload cannot miss it. Run it by hand only if you built some other way:
#   ./fix-cycles-tbb.sh [install-dir]        # default: install
#
# Signing is left alone on purpose: install_name_tool invalidates signatures, and
# the Rhino build re-signs the frameworks it deploys.

set -euo pipefail

INSTALL="${1:-install}"
LIB="$INSTALL/lib"
OLD="@rpath/libtbb.dylib"
NEW="@rpath/libtbb.12.cycles.dylib"

# Rename the library itself, if this run is the one that finds it. Note that the
# rename and the repointing below are checked separately on purpose: `make release`
# rebuilds libccycles.dylib every time, and the rebuilt one references the old name
# again even though lib/ was renamed by a previous run. Treating "lib/ is already
# renamed" as "nothing to do" would leave that fresh binary pointing at a library
# that is not there, and nothing downstream checks for it.
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

echo "fix-cycles-tbb: done - payload oneTBB is libtbb.12.cycles.dylib"

# Nothing may still reference the old name, or it will resolve to Rhino's TBB 2020.3.
for f in "$LIB"/*.dylib "$INSTALL/libccycles.dylib"; do
	[ -f "$f" ] || continue
	if otool -L "$f" | grep -q "$OLD"; then
		echo "fix-cycles-tbb: $(basename "$f") still references $OLD" >&2
		exit 1
	fi
done
