cmake_minimum_required(VERSION 3.0)

include(${CMAKE_ROOT}/Modules/ExternalProject.cmake)

set(THIRD_PARTY_DIR ${CMAKE_SOURCE_DIR})


# Build Portaudio
ExternalProject_Add(portaudio
    URL http://www.portaudio.com/archives/pa_stable_v190600_20161030.tgz
    PREFIX ${THIRD_PARTY_DIR}
    CONFIGURE_COMMAND ${THIRD_PARTY_DIR}/src/portaudio/configure
                      --prefix=${THIRD_PARTY_DIR}
                      --enable-shared
                      --disable-static
                      --with-pic
                      --disable-rpath
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
                --enable-shared
                --disable-static
                --with-pic
                --disable-rpath
    BUILD_IN_SOURCE 1
)

# Vorbis 2nd
ExternalProject_Add(vorbis
    URL https://github.com/xiph/vorbis/archive/v1.3.6.tar.gz
    PREFIX ${THIRD_PARTY_DIR}
    CONFIGURE_COMMAND ${THIRD_PARTY_DIR}/src/vorbis/autogen.sh          # First need to run autogen.sh
        COMMAND ${THIRD_PARTY_DIR}/src/vorbis/configure                 # Then configure.sh
                --prefix=${THIRD_PARTY_DIR}
                --enable-shared
                --disable-static
                --with-pic
                --disable-rpath
    BUILD_IN_SOURCE 1
)
add_dependencies(vorbis ogg)

# And FLAC 3rd
ExternalProject_Add(flac
    URL https://github.com/xiph/flac/archive/1.3.2.tar.gz
    PREFIX ${THIRD_PARTY_DIR}
    CONFIGURE_COMMAND ${THIRD_PARTY_DIR}/src/flac/autogen.sh            # First need to run autogen.sh
        COMMAND ${THIRD_PARTY_DIR}/src/flac/configure                   # Then configure.sh
                --prefix=${THIRD_PARTY_DIR}
                --enable-shared
                --disable-static
                --with-pic
                --disable-rpath
                --disable-xmms-plugin
    BUILD_IN_SOURCE 1
)
add_dependencies(flac ogg)

# Now we can build libsndfile
ExternalProject_Add(libsndfile
    URL https://github.com/erikd/libsndfile/archive/1.0.28.tar.gz
    PREFIX ${THIRD_PARTY_DIR}
    CONFIGURE_COMMAND ${THIRD_PARTY_DIR}/src/libsndfile/autogen.sh      # First need to run autogen.sh
        COMMAND CFLAGS=-I${THIRD_PARTY_DIR}/include                     #   (Unforntately, searching for FLAC is a little broken, so I need to add that `CFLAGS` thingy)
                PKG_CONFIG_PATH=${THIRD_PARTY_DIR}/lib/pkgconfig        #   (Required to filnd OGG/Vorbis/FLAC)
                ${THIRD_PARTY_DIR}/src/libsndfile/configure             # Then configure.sh
                --prefix=${THIRD_PARTY_DIR}
                --enable-shared
                --disable-static
                --with-pic
                --disable-rpath
    BUILD_IN_SOURCE 1
)
add_dependencies(libsndfile ogg vorbis flac)


# Target to signify everything has been built
add_custom_target(all_native_libs_built ALL
                  DEPENDS portaudio libsndfile ogg vorbis flac)


if(WIN32 OR MINGW OR CYGWIN OR MSYS)
    # Fix the location of the windows DLLs
    add_custom_command(TARGET all_native_libs_built
                       POST_BUILD
                       COMMAND ${CMAKE_COMMAND} -P win_dll_fixup.cmake ${THIRD_PARTY_DIR})
endif()
