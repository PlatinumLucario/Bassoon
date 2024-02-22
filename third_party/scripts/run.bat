:: For Windows commands only
::
:: The packages directory needs to be removed, so it can be made again
rd /s /q ..\packages
::
:: Updates and copies the libraries from VCPKG
.\setup_all.py
::
:: Generates the project files, creates directories and copies the libraries
.\generate.py
::
:: Build and pack projects
:: Linux x64
pushd ..\projects\portaudio\linux
dotnet build -c Release portaudio.runtime.x64.csproj
dotnet pack -c Release portaudio.runtime.x64.csproj -o ..\..\..\packages
popd
pushd ..\projects\sndfile\linux
dotnet build -c Release sndfile.runtime.x64.csproj
dotnet pack -c Release sndfile.runtime.x64.csproj -o ..\..\..\packages
popd
:: Linux ARM64
pushd ..\projects\portaudio\linux
dotnet build -c Release portaudio.runtime.arm64.csproj
dotnet pack -c Release portaudio.runtime.arm64.csproj -o ..\..\..\packages
popd
pushd ..\projects\sndfile\linux
dotnet build -c Release sndfile.runtime.arm64.csproj
dotnet pack -c Release sndfile.runtime.arm64.csproj -o ..\..\..\packages
popd
::
:: macOS x64
pushd ..\projects\portaudio\macos
dotnet build -c Release portaudio.runtime.x64.csproj
dotnet pack -c Release portaudio.runtime.x64.csproj -o ..\..\..\packages
popd
pushd ..\projects\sndfile\macos
dotnet build -c Release sndfile.runtime.x64.csproj
dotnet pack -c Release sndfile.runtime.x64.csproj -o ..\..\..\packages
popd
:: macOS ARM64
pushd ..\projects\portaudio\macos
dotnet build -c Release portaudio.runtime.arm64.csproj
dotnet pack -c Release portaudio.runtime.arm64.csproj -o ..\..\..\packages
popd
pushd ..\projects\sndfile\macos
dotnet build -c Release sndfile.runtime.arm64.csproj
dotnet pack -c Release sndfile.runtime.arm64.csproj -o ..\..\..\packages
popd
::
:: Windows x64
pushd ..\projects\portaudio\windows
dotnet build -c Release portaudio.runtime.x64.csproj
dotnet pack -c Release portaudio.runtime.x64.csproj -o ..\..\..\packages
popd
pushd ..\projects\sndfile\windows
dotnet build -c Release sndfile.runtime.x64.csproj
dotnet pack -c Release sndfile.runtime.x64.csproj -o ..\..\..\packages
popd
:: Windows ARM64
pushd ..\projects\portaudio\windows
dotnet build -c Release portaudio.runtime.arm64.csproj
dotnet pack -c Release portaudio.runtime.arm64.csproj -o ..\..\..\packages
popd
pushd ..\projects\sndfile\windows
dotnet build -c Release sndfile.runtime.arm64.csproj
dotnet pack -c Release sndfile.runtime.arm64.csproj -o ..\..\..\packages
popd
