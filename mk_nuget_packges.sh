#!/bin/bash
#
# Creates the NuGet packages

# Create the release packages
dotnet pack -c Release $BASSOON_ROOT/src/Bassoon/libsndfileSharp
dotnet pack -c Release $BASSOON_ROOT/src/Bassoon/PortAudioSharp
dotnet pack -c Release $BASSOON_ROOT/src/Bassoon/Bassoon

# copy them over to the root dir
cp $BASSOON_ROOT/src/Bassoon/libsndfileSharp/bin/Release/*.nupkg .
cp $BASSOON_ROOT/src/Bassoon/PortAudioSharp/bin/Release/*.nupkg .
cp $BASSOON_ROOT/src/Bassoon/Bassoon/bin/Release/*.nupkg .
