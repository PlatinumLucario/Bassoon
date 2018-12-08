#!/bin/bash
#
# Sets the dev environment

# Project root
BASSOON_ROOT=`pwd`
export BASSOON_ROOT

# Third Party libraries
LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$BASSOON_ROOT/third_party/lib
PKG_CONFIG_PATH=$PKG_CONFIG_PATH:$BASSOON_ROOT/third_party/lib/pkgconfig

export LD_LIBRARY_PATH
export PKG_CONFIG_PATH
