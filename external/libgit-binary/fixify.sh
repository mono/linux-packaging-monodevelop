#!/bin/bash

# This script is used to make dylibs installed for /usr/local be portable.

# Change this to be the path to the dylibs.
LIBRARY_DIR=mac

pushd $LIBRARY_DIR
for i in *; do
	install_name_tool -id "$i" "$i"
	for j in *; do
		test "$i" == "$j" && continue
		install_name_tool -change "/usr/local/lib/$j" "@loader_path/$j" "$i"
	done
done
popd

