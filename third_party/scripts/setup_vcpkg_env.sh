#!/usr/bin/env bash
#
# This will set up the VCPKG environment into
# '/home/$USER/source/vcpkg' on Linux
# or '/User/$USER/source/vcpkg' on macOS
#
# It's very useful if you want to save time setting up the dependencies
mkdir ~/source
pushd ~/source
git clone https://github.com/microsoft/vcpkg.git
./vcpkg/bootstrap-vcpkg.sh
export VCPKG_DIR=~/source/vcpkg
popd
