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
    # If on windows
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg.exe')
    if is_x64:
        deps = ['%s:x64-windows' % x for x in deps]
        lib_src_dir = 'installed/x64-windows/bin/'
    elif is_aarch64:
        deps = ['%s:arm64-windows' % x for x in deps]
        lib_src_dir = 'installed/arm64-windows/bin/'
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
    # If on linux
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg')
    if is_x64:
        lib_src_dir = 'installed/x64-linux/lib/'
        libs_codecs = [
            'libFLAC.so',
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
    elif is_aarch64:
        lib_src_dir = 'installed/arm64-linux/lib/'
        libs_codecs = [
            'libFLAC.a',
            'libogg.a',
            'libvorbis.a',
            'libvorbisenc.a',
            'libvorbisfile.a',
            'libopus.a',
            'libmp3lame.a',
            'libmpg123.a',
            'libout123.a',
            'libsyn123.a',
        ]
        libs_sndfile = [
            'libsndfile.a',
    ]
    libs_portaudio = [
        'libportaudio.a',
    ]
elif is_macos:
    # If on macos
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg')
    if is_x64:
        deps = ['%s:x64-osx-dynamic' % x for x in deps]
        lib_src_dir = 'installed/x64-osx-dynamic/lib/'
    elif is_aarch64:
        deps = ['%s:arm64-osx-dynamic' % x for x in deps]
        lib_src_dir = 'installed/arm64-osx-dynamic/lib/'
    libs_codecs = [
        'libFLAC.dylib',
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
        os.makedirs(pathlib.Path('../lib/codecs/win-x64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/portaudio/win-x64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/sndfile/win-x64'), exist_ok=True)
    elif is_aarch64:
        os.makedirs(pathlib.Path('../lib/codecs/win-arm64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/portaudio/win-arm64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/sndfile/win-arm64'), exist_ok=True)
elif is_linux:
    if is_x64:
        os.makedirs(pathlib.Path('../lib/codecs/linux-x64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/portaudio/linux-x64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/sndfile/linux-x64'), exist_ok=True)
    elif is_aarch64:
        os.makedirs(pathlib.Path('../lib/codecs/linux-arm64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/portaudio/linux-arm64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/sndfile/linux-arm64'), exist_ok=True)
elif is_macos:
    if is_x64:
        os.makedirs(pathlib.Path('../lib/codecs/macos-x64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/portaudio/macos-x64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/sndfile/macos-x64'), exist_ok=True)
    elif is_aarch64:
        os.makedirs(pathlib.Path('../lib/codecs/macos-arm64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/portaudio/macos-arm64'), exist_ok=True)
        os.makedirs(pathlib.Path('../lib/sndfile/macos-arm64'), exist_ok=True)

# Pull the latest commits from VCPKG repo, then upgrade all libraries
proc = subprocess.Popen(['git', 'pull'], cwd=vcpkg_dir)
proc.wait()
proc = subprocess.Popen([vcpkg_exe, 'upgrade', '--no-dry-run'])
proc.wait()

# Install the deps
proc = subprocess.Popen([vcpkg_exe, 'install', *deps, '--overlay-triplets=dynamic-triplets'])
proc.wait()
print("All libraries built successfully!")

# Now get the dlls that we really want
if is_windows:
    if is_x64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/codecs/win-x64')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/sndfile/win-x64')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/portaudio/win-x64')
        print("All x86-64 Windows libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('../lib/sndfile/win-x64', src_name),
                os.path.join('../lib/sndfile/win-x64', dst_name)
            )
    if is_aarch64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/codecs/win-arm64')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/sndfile/win-arm64')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/portaudio/win-arm64')
        print("All ARM64 Windows libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('../lib/sndfile/win-arm64', src_name),
                os.path.join('../lib/sndfile/win-arm64', dst_name)
            )
elif is_linux:
    if is_x64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/codecs/linux-x64')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/sndfile/linux-x64')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/portaudio/linux-x64')
        print("All x86-64 Linux libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('../lib/sndfile/linux-x64', src_name),
                os.path.join('../lib/sndfile/linux-x64', dst_name)
            )
    if is_aarch64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/codecs/linux-arm64')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/sndfile/linux-arm64')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/portaudio/linux-arm64')
        print("All ARM64 Linux libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('../lib/sndfile/linux-arm64', src_name),
                os.path.join('../lib/sndfile/linux-arm64', dst_name)
            )
elif is_macos:
    if is_x64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/codecs/macos-x64')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/sndfile/macos-x64')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/portaudio/macos-x64')
        print("All x86-64 macOS libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('../lib/sndfile/macos-x64', src_name),
                os.path.join('../lib/sndfile/macos-x64', dst_name)
            )
    if is_aarch64:
        for lib in libs_codecs:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/codecs/macos-arm64')
        for lib in libs_sndfile:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/sndfile/macos-arm64')
        for lib in libs_portaudio:
            src = os.path.join(vcpkg_dir, lib_src_dir, lib)
            shutil.copy(src, '../lib/portaudio/macos-arm64')
        print("All ARM64 macOS libraries copied successfully!")

        # Fix names
        for (src_name, dst_name) in lib_renaming_rules:
            os.rename(
                os.path.join('../lib/sndfile/macos-arm64', src_name),
                os.path.join('../lib/sndfile/macos-arm64', dst_name)
            )


# If on macOS, we need to fix some issues with the dylibs
# namely the rpath stuff & dylib IDs,  it's a bit of a pain
# if is_macos:
#     cmds = []

#     # First fix Ids
#     print("Fixing dylib IDs...")
#     dylibs = ['ogg', 'vorbis', 'vorbisenc', 'vorbisfile', 'sndfile']
#     for lib in dylibs:
#         lib_filename = 'lib%s.dylib' % (lib)
#         cmd = 'install_name_tool -id "@rpath/{0}" lib/{0}'.format(lib_filename)
#         cmds.append(cmd)
#     print("Done!")

#     # Now fix the references
#     print("Fixing rpath references...")
#     changes = [
#         # (old, new, [targets])
#         ('ogg.0', 'ogg',                   ['FLAC', 'sndfile', 'vorbis', 'vorbisenc', 'vorbisfile']),
#         ('vorbis.0.4.8', 'vorbis',         ['sndfile', 'vorbisenc', 'vorbisfile']),
#         ('vorbisfile.3.3.7', 'vorbisfile', ['sndfile']),
#         ('vorbisenc.2.0.11', 'vorbisenc',  ['sndfile']),
#     ]
#     for (o, n, targets) in changes:
#         for t in targets:
#             change_cmd = 'install_name_tool -change "@rpath/lib{0}.dylib" "@rpath/lib{1}.dylib" lib/lib{2}.dylib'.format(o, n, t)
#             cmds.append(change_cmd)
#     print("Done!")

#     # Run the commands
#     print("Running neccessary commands...")
#     [os.system(x) for x in cmds]
#     print("Done!")
print("All neccessary tasks completed!")

