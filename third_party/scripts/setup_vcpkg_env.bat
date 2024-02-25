:: This will set up the VCPKG environment into
:: C:\src\vcpkg on Windows
::
:: It's very useful if you want to save time setting up the dependencies
mkdir C:\src
pushd C:\src
git clone https://github.com/microsoft/vcpkg.git
.\vcpkg\bootstrap-vcpkg.bat
set VCPKG_DIR=C:\src\vcpkg
popd
