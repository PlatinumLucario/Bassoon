#!/usr/bin/python3

import os, sys, platform
import pathlib
import subprocess
import shutil

# Some OS flags
is_windows = (sys.platform == 'win32') or (sys.platform == 'msys')
is_linux = sys.platform == 'linux'
is_macos = sys.platform == 'darwin'

# And CPU architecture flags
is_x64 = (platform.machine() == 'x86_64') or (platform.machine() == 'amd64') or (platform.machine() == 'AMD64')
is_aarch64 = (platform.machine() == 'arm64') or (platform.machine() == 'ARM64') or (platform.machine() == 'aarch64') or (platform.machine() == 'Aarch64') or (platform.machine() == 'AARCH64')

deps = [
    'libsndfile',       # Also will grab libflac, libogg, and libvorbis for us
    'portaudio',
]
vcpkg_exe = ''
libs_codecs = []
libs_sndfile = []
libs_portaudio = []
lib_renaming_rules = [] # list of two-element tuples
lib_src_dir =   ''

# Ensure we have access to the VCPKG executable, should be first argument
vcpkg_dir = os.path.abspath(os.environ.get('VCPKG_DIR'))
if not vcpkg_dir:
    print('Error, need environment variable VCPKG_DIR to point to directory where `vcpkg` executable is')
    sys.exit(1)
elif not os.path.exists(vcpkg_dir):
    print('Error, not able to find %s' % vcpkg_dir)
    sys.exit(1)

# Some OS specific configurations
if is_windows:
    # If on windows, need to build the 64 bit version
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg.exe')
    deps = ['%s:x64-windows' % x for x in deps]
    lib_src_dir = 'installed/x64-windows/bin/'
    libs_codecs = [
        'FLAC.dll',
        'ogg.dll',
        'vorbis.dll',
        'vorbisenc.dll',
        'vorbisfile.dll',
        'opus.dll',
        'libmp3lame.dll',
        'mpg123.dll',
        'out123.dll',
        'syn123.dll',
    ]
    libs_sndfile = [
        'sndfile.dll',
    ]
    libs_portaudio = [
        'portaudio.dll',
    ]
elif is_linux:
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg')
    lib_src_dir = 'installed/x64-linux/lib/'
    libs_codecs = [
        'libFLAC.so',
        'libFLAC++.so',
        'libogg.so',
        'libvorbis.so',
        'libvorbisenc.so',
        'libvorbisfile.so',
        'libopus.so',
        'libmp3lame.so',
        'libmpg123.so',
        'libout123.so',
        'libsyn123.so',
    ]
    libs_sndfile = [
        'libsndfile.so',
    ]
    libs_portaudio = [
        'libportaudio.a',
    ]
elif is_macos:
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg')
    lib_src_dir = 'installed/x64-osx/lib/'
    libs_codecs = [
        'libFLAC.dylib',
        'libFLAC++.dylib',
        'libogg.dylib',
        'libvorbis.dylib',
        'libvorbisenc.dylib',
        'libvorbisfile.dylib',
        'libopus.dylib',
        'libmp3lame.dylib',
        'libmpg123.dylib',
        'libout123.dylib',
        'libsyn123.dylib',
    ]
    libs_sndfile = [
        'libsndfile.dylib',
    ]
    libs_portaudio = [
        'libportaudio.dylib',
    ]

# First make sure the lib directory is there
if is_windows:
    if is_x64:
        os.makedirs(pathlib.Path('lib/codecs/x64-windows'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/portaudio/x64-windows'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/sndfile/x64-windows'), exist_ok=True)
    elif is_aarch64:
        os.makedirs(pathlib.Path('lib/codecs/arm64-windows'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/portaudio/arm64-windows'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/sndfile/arm64-windows'), exist_ok=True)
elif is_linux:
    if is_x64:
        os.makedirs(pathlib.Path('lib/codecs/x64-linux'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/portaudio/x64-linux'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/sndfile/x64-linux'), exist_ok=True)
    elif is_aarch64:
        os.makedirs(pathlib.Path('lib/codecs/arm64-linux'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/portaudio/arm64-linux'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/sndfile/arm64-linux'), exist_ok=True)
elif is_macos:
    if is_x64:
        os.makedirs(pathlib.Path('lib/codecs/x64-macos'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/portaudio/x64-macos'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/sndfile/x64-macos'), exist_ok=True)
    elif is_aarch64:
        os.makedirs(pathlib.Path('lib/codecs/arm64-macos'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/portaudio/arm64-macos'), exist_ok=True)
        os.makedirs(pathlib.Path('lib/sndfile/arm64-macos'), exist_ok=True)

# Install the deps
proc = subprocess.Popen([vcpkg_exe, 'install', *deps, '--overlay-triplets=dynamic-triplets'])
proc.wait()

# Now get the dlls that we really want
if is_windows:
    if is_x64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/codecs/x64-windows')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/sndfile/x64-windows')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/portaudio/x64-windows')
        print("All libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('lib/sndfile/x64-windows', src_name),
                os.path.join('lib/sndfile/x64-windows', dst_name)
            )
    if is_aarch64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/codecs/arm64-windows')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/sndfile/arm64-windows')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/portaudio/arm64-windows')
        print("All libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('lib/sndfile/arm64-windows', src_name),
                os.path.join('lib/sndfile/arm64-windows', dst_name)
            )
elif is_linux:
    if is_x64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/codecs/x64-linux')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/sndfile/x64-linux')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/portaudio/x64-linux')
        print("All libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('lib/sndfile/x64-linux', src_name),
                os.path.join('lib/sndfile/x64-linux', dst_name)
            )
    if is_aarch64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/codecs/arm64-linux')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/sndfile/arm64-linux')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/portaudio/arm64-linux')
        print("All libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('lib/sndfile/arm64-linux', src_name),
                os.path.join('lib/sndfile/arm64-linux', dst_name)
            )
elif is_macos:
    if is_x64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/codecs/x64-macos')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/sndfile/x64-macos')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/portaudio/x64-macos')
        print("All libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('lib/sndfile/x64-macos', src_name),
                os.path.join('lib/sndfile/x64-macos', dst_name)
            )
    if is_aarch64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/codecs/arm64-macos')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/sndfile/arm64-macos')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, 'lib/portaudio/arm64-macos')
        print("All libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('lib/sndfile/arm64-macos', src_name),
                os.path.join('lib/sndfile/arm64-macos', dst_name)
            )


# If on macOS, we need to fix some issues with the dylibs
# namely the rpath stuff & dylib IDs,  it's a bit of a pain
if is_macos:
    cmds = []

    # First fix Ids
    print("Fixing dylib IDs...")
    dylibs = ['ogg', 'vorbis', 'vorbisenc', 'vorbisfile', 'sndfile']
    for lib in dylibs:
        lib_filename = 'lib%s.dylib' % (lib)
        cmd = 'install_name_tool -id "@rpath/{0}" lib/{0}'.format(lib_filename)
        cmds.append(cmd)
    print("Done!")

    # Now fix the references
    print("Fixing rpath references...")
    changes = [
        # (old, new, [targets])
        ('ogg.0', 'ogg',                   ['FLAC', 'sndfile', 'vorbis', 'vorbisenc', 'vorbisfile']),
        ('vorbis.0.4.8', 'vorbis',         ['sndfile', 'vorbisenc', 'vorbisfile']),
        ('vorbisfile.3.3.7', 'vorbisfile', ['sndfile']),
        ('vorbisenc.2.0.11', 'vorbisenc',  ['sndfile']),
    ]
    for (o, n, targets) in changes:
        for t in targets:
            change_cmd = 'install_name_tool -change "@rpath/lib{0}.dylib" "@rpath/lib{1}.dylib" lib/lib{2}.dylib'.format(o, n, t)
            cmds.append(change_cmd)
    print("Done!")

    # Run the commands
    print("Running neccessary commands...")
    [os.system(x) for x in cmds]
    print("Done!")
print("All neccessary tasks completed!")

