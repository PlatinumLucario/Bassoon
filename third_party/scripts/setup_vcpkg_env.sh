#!/usr/bin/env bash
#
# This will set up the VCPKG environment into
# '/home/$USER/source/vcpkg' on Linux
# or '/User/$USER/source/vcpkg' on macOS
#
# It's very useful if you want to save time setting up the dependencies
if [[ -z "$VCPKG_DIR" ]]; then
	mkdir ~/source
	pushd ~/source
	if [[ -z "./vcpkg" ]]; then
		git clone https://github.com/microsoft/vcpkg.git
		./vcpkg/bootstrap-vcpkg.sh
	fi
	popd
	export VCPKG_DIR=~/source/vcpkg
fi
