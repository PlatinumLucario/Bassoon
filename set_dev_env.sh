#!/bin/bash
#
# Sets the dev environment

# Project root
BASSOON_ROOT=`pwd`
export BASSOON_ROOT

# Third Party libraries
THIRD_PARTY_LIBS=$BASSOON_ROOT/third_party/lib

# Set the appropriate .so/.dll path for each OS/environment
unameOut="$(uname -s)"
case "${unameOut}" in
    Linux* | Darwin*)
        export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$THIRD_PARTY_LIBS;;
    CYGWIN* | MINGW* | MSYS*)
        export PATH=$PATH:$THIRD_PARTY_LIBS;;
esac
