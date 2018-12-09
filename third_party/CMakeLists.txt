
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


##== Build libsndfile (and the OGG/Voribs/FLAC crew)

# OGG 1st
ExternalProject_Add(ogg
    URL https://github.com/xiph/ogg/archive/v1.3.3.tar.gz
    PREFIX ${THIRD_PARTY_DIR}
    CONFIGURE_COMMAND ${THIRD_PARTY_DIR}/src/ogg/autogen.sh             # First need to run autogen.sh
        COMMAND ${THIRD_PARTY_DIR}/src/ogg/configure                    # Then configure.sh
                --prefix=${THIRD_PARTY_DIR}
    BUILD_IN_SOURCE 1
)
