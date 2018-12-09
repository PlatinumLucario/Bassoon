cmake_minimum_required(VERSION 3.0)

include(${CMAKE_ROOT}/Modules/ExternalProject.cmake)

set(THIRD_PARTY_DIR ${CMAKE_SOURCE_DIR})
message(STATUS ${THIRD_PARTY_DIR})


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

# Vorbis 2nd
ExternalProject_Add(vorbis
    URL https://github.com/xiph/vorbis/archive/v1.3.6.tar.gz
    PREFIX ${THIRD_PARTY_DIR}
    CONFIGURE_COMMAND ${THIRD_PARTY_DIR}/src/vorbis/autogen.sh          # First need to run autogen.sh
        COMMAND ${THIRD_PARTY_DIR}/src/vorbis/configure                 # Then configure.sh
                --prefix=${THIRD_PARTY_DIR}
    BUILD_IN_SOURCE 1
)

# And FLAC 3rd
ExternalProject_Add(flac
    URL https://github.com/xiph/flac/archive/1.3.2.tar.gz
    PREFIX ${THIRD_PARTY_DIR}
    CONFIGURE_COMMAND ${THIRD_PARTY_DIR}/src/flac/autogen.sh            # First need to run autogen.sh
        COMMAND ${THIRD_PARTY_DIR}/src/flac/configure                   # Then configure.sh
                --prefix=${THIRD_PARTY_DIR}
    BUILD_IN_SOURCE 1
)
add_dependencies(flac ogg)

# Now we can build libsndfile
ExternalProject_Add(libsndfile
    URL https://github.com/erikd/libsndfile/archive/1.0.28.tar.gz
    PREFIX ${THIRD_PARTY_DIR}
    CONFIGURE_COMMAND ${THIRD_PARTY_DIR}/src/libsndfile/autogen.sh      # First need to run autogen.sh
        COMMAND CFLAGS=-I${THIRD_PARTY_DIR}/include                     #   (Unforntately, searching for FLAC is a little broken, so I need to add that `CFLAGS` thingy)
                ${THIRD_PARTY_DIR}/src/libsndfile/configure             # Then configure.sh
                --prefix=${THIRD_PARTY_DIR}
    BUILD_IN_SOURCE 1
)
add_dependencies(flac ogg vorbis flac)
