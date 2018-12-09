#!/bin/bash
#
# Sets the dev environment

# Project root
BASSOON_ROOT=`pwd`
export BASSOON_ROOT

# Third Party libraries
LD_LIBRARY_PATH=$LD_LIBRARY_PATH:$BASSOON_ROOT/third_party/lib
export LD_LIBRARY_PATH
