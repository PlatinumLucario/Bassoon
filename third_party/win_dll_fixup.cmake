# If we are on windows, after the builds are done, we want to copy the actual
# C#/.NET compatible native .dll file.  If you look in the `lib/` folder,
# you'll see a bunch of `*.dll.a` files.  Those are not compatible with the
# Windows .NET runtime.  Regular (compatable) .dlls are built, but they are
# put into the `bin/` dir.  We copy them over to `lib/`, and rename them

# The third (and only "true") argument should be our working dir, or the `third_party/`
# directory
set(THIRD_PARTY_DIR ${CMAKE_ARGV3})

# Handy vars
set(BIN_DIR ${THIRD_PARTY_DIR}/bin)
set(LIB_DIR ${THIRD_PARTY_DIR}/lib)

# Copy
file(GLOB WIN_DLLS ${BIN_DIR}/*.dll)
file(INSTALL ${WIN_DLLS} DESTINATION ${LIB_DIR})

# Rename
file(RENAME ${LIB_DIR}/libsndfile-1.dll   ${LIB_DIR}/sndfile.dll)
file(RENAME ${LIB_DIR}/libportaudio-2.dll ${LIB_DIR}/portaudio.dll)
