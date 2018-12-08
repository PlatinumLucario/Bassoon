
cmake_minimum_required(VERSION 3.0)

include(${CMAKE_ROOT}/Modules/ExternalProject.cmake)

set(THIRD_PARTY_DIR "$ENV{BASSOON_ROOT}/third_party")

# Build Portaudio
ExternalProject_Add(portaudio
    URL http://www.portaudio.com/archives/pa_stable_v190600_20161030.tgz
    PREFIX ${THIRD_PARTY_DIR}
    CONFIGURE_COMMAND ${THIRD_PARTY_DIR}/src/portaudio/configure
                      --prefix=${THIRD_PARTY_DIR}
                      --disable-mac-universal
    BUILD_IN_SOURCE 1
)
