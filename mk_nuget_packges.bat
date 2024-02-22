:: Creates the NuGet packages


:: Download or update the dependencies
pushd .\third_party\scripts
.\run.bat
popd

:: Create the release packages
dotnet pack -c Release .\src\SndFileSharp
dotnet pack -c Release .\src\PortAudioSharp
dotnet pack -c Release .\src\Bassoon

:: copy them over to the root dir
xcopy /y .\src\SndFileSharp\bin\Release\*.nupkg .\pack
xcopy /y .\src\PortAudioSharp\bin\Release\*.nupkg .\pack
xcopy /y .\src\Bassoon\bin\Release\*.nupkg .\pack
