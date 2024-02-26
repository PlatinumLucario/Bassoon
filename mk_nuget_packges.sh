#!/bin/bash
#
# Sets the current directory as BASSOON_ROOT environment variable
BASSOON_ROOT=`pwd`
export BASSOON_ROOT

# Creates the NuGet packages


# Download or update the dependencies
# pushd ./third_party/scripts
# ./run.sh
# popd

# Create the release packages
dotnet pack -c Release $BASSOON_ROOT/src/SndFileSharp
dotnet pack -c Release $BASSOON_ROOT/src/PortAudioSharp
dotnet pack -c Release $BASSOON_ROOT/src/Bassoon

# copy them over to the root dir
cp $BASSOON_ROOT/src/SndFileSharp/bin/Release/*.nupkg ./pack
cp $BASSOON_ROOT/src/PortAudioSharp/bin/Release/*.nupkg ./pack
cp $BASSOON_ROOT/src/Bassoon/bin/Release/*.nupkg ./pack
