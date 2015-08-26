#!/bin/bash

CURDIR=$(pwd)
pushd external/libgit2
LIBGIT2SHA=$(git rev-parse HEAD)
popd
echo $LIBGIT2SHA > libgit2_hash.txt
SHORTSHA=${LIBGIT2SHA:0:7}
OS=$(uname -s)

function check_newer_binaries {
	pushd "$CURDIR/mac"
	for i in libgit2-*.dylib;
	do
		OLD=$i
	done
	popd

	if [[ -z $OLD || $OLD == libgit2-*.dylib ]]
	then
		return
	fi

	OLDSHA=${OLD:8:7}
	pushd external/libgit2
	git merge-base --is-ancestor "$LIBGIT2SHA" "$OLDSHA" && echo "Binaries are newer in output directory" && exit 0
	popd
}

if [ "$OS" == "Darwin" ]
then
	BUILDDIR=mac
	PKGPATH="./mac"
	LIBEXT="dylib"
else
	BUILDDIR=external/libgit2/build
	LIBEXT="so"
fi

if [[ -d "$BUILDDIR" ]]
then
    if [[ -f "$BUILDDIR/libgit2-${SHORTSHA}.${LIBEXT}" ]]
    then
        echo "Binaries are the same as in output directory."
        exit 0
    fi
fi

if [ "$OS" == "Darwin" ]; then
	check_newer_binaries

	mkdir -p external/libssh2/build
	pushd external/libssh2/build

	cmake -DCMAKE_BUILD_TYPE:STRING=RelWithDebInfo \
	      -DBUILD_SHARED_LIBS:BOOL=ON \
	      -DENABLE_ZLIB_COMPRESSION:BOOL=ON \
	      -DCMAKE_OSX_ARCHITECTURES="i386;x86_64" \
	      -DCMAKE_SKIP_RPATH=TRUE \
	      ..
	cmake --build .

	popd

	mkdir -p $PKGPATH
	cp external/libssh2/build/src/libssh2.dylib $PKGPATH/
	install_name_tool -id libssh2.dylib $PKGPATH/libssh2.dylib
fi

rm -rf external/libgit2/build
mkdir external/libgit2/build
pushd external/libgit2/build

if [ "$OS" == "Darwin" ]
then
	cmake -DCMAKE_BUILD_TYPE:STRING=RelWithDebInfo \
	      -DUSE_SSH=OFF \
	      -DBUILD_CLAR:BOOL=OFF \
	      -DENABLE_TRACE=ON \
	      -DLIBGIT2_FILENAME=git2-$SHORTSHA \
	      -DCMAKE_OSX_ARCHITECTURES="i386;x86_64" \
	      -DCMAKE_SKIP_RPATH=TRUE \
	      -DLIBSSH2_FOUND=TRUE \
	      -DLIBSSH2_INCLUDE_DIRS="$CURDIR/external/libssh2/include" \
	      -DLIBSSH2_LIBRARY_DIRS="$CURDIR/external/libssh2/build/src" \
	      -DSSH_LIBRARIES="ssh2" \
	      -DHAVE_LIBSSH2_MEMORY_CREDENTIALS=TRUE \
	      ..
else
	cmake -DCMAKE_BUILD_TYPE:STRING=RelWithDebInfo \
	      -DBUILD_CLAR:BOOL=OFF \
	      -DUSE_SSH=ON \
	      -DENABLE_TRACE=ON \
	      -DLIBGIT2_FILENAME=git2-$SHORTSHA \
	      -DCMAKE_SKIP_RPATH=TRUE \
	      ..
fi
cmake --build .

popd

if [ "$OS" != "Darwin" ]
then
	exit 0
fi

for i in $PKGPATH/*;
do
	if [[ "$i" == "$PKGPATH/libssh2.dylib" || "$i" == "$PKGPATH/libgit2-$SHORTSHA.$LIBEXT" ]]
	then
		continue
	fi

	git rm $i
done

cp "external/libgit2/build/libgit2-$SHORTSHA.$LIBEXT" $PKGPATH/
install_name_tool -change libssh2.1.dylib @loader_path/libssh2.dylib "$PKGPATH/libgit2-$SHORTSHA.$LIBEXT"

git stash save
git pull

check_newer_binaries

git stash pop

git add $PKGPATH

git commit -m "Bumping OSX libgit2 to $LIBGIT2SHA"
git push

exit $?
