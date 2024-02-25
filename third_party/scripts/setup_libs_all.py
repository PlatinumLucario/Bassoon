#!/usr/bin/python3

import os, sys, platform
import pathlib
import subprocess
import shutil

from pathlib import Path

# Some OS flags
is_windows = (sys.platform == 'win32') or (sys.platform == 'msys')
is_linux = sys.platform == 'linux'
is_macos = sys.platform == 'darwin'

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

# Make sure the executable is correctly defined, based on the OS
if is_windows:
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg.exe')
else:
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg')

# Pull the latest commits from VCPKG repo, then upgrade all libraries
proc = subprocess.Popen(['git', 'pull'], cwd=vcpkg_dir)
proc.wait()
proc = subprocess.Popen([vcpkg_exe, 'upgrade', '--no-dry-run'])
proc.wait()

def BuildWindowsDeps(deps):
    # Build the x86-64 version of the Windows dependencies
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

    # First make sure the lib directory is there
    os.makedirs(pathlib.Path('../lib/codecs/win-x64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/portaudio/win-x64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/sndfile/win-x64'), exist_ok=True)

    # Install the deps
    proc = subprocess.Popen([vcpkg_exe, 'install', *deps, '--overlay-triplets=dynamic-triplets'])
    proc.wait()
    print("All x86-64 Windows libraries built successfully!")

    # Now get the dlls that we really want
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
    
    # Clear deps list and re-add needed deps
    # Required to prevent the following error:
    # <unknown>:1:23: error: expected eof
    #  on expression: libsndfile:x64-windows:arm64-windows
    #                                       ^
    deps.clear()
    deps = [
        'libsndfile',       # Also will grab libflac, libogg, and libvorbis for us
        'portaudio',
    ]

    # Build the ARM64 version of the Windows dependencies
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

    # First make sure the lib directory is there
    os.makedirs(pathlib.Path('../lib/codecs/win-arm64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/portaudio/win-arm64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/sndfile/win-arm64'), exist_ok=True)

    # Install the deps
    proc = subprocess.Popen([vcpkg_exe, 'install', *deps, '--overlay-triplets=dynamic-triplets'])
    proc.wait()
    print("All ARM64 Windows libraries built successfully!")

    # Now get the dlls that we really want
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
    
    # Clear deps list and re-add needed deps
    # Required to prevent the following error:
    # <unknown>:1:23: error: expected eof
    #  on expression: libsndfile:arm64-windows:x64-linux
    #                                         ^
    deps.clear()
    deps = [
        'libsndfile',       # Also will grab libflac, libogg, and libvorbis for us
        'portaudio',
    ]

def BuildLinuxDeps(deps):
    # Build the x86-64 version of the Linux dependencies
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
    libs_portaudio = [
        'libportaudio.a',
    ]

    # First make sure the lib directory is there
    os.makedirs(pathlib.Path('../lib/codecs/linux-x64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/portaudio/linux-x64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/sndfile/linux-x64'), exist_ok=True)

    # Install the deps
    proc = subprocess.Popen([vcpkg_exe, 'install', *deps, '--overlay-triplets=dynamic-triplets'])
    proc.wait()
    print("All x86-64 Linux libraries built successfully!")

    # Now get the dlls that we really want
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

    # Clear deps list and re-add needed deps
    # Required to prevent the following error:
    # <unknown>:1:23: error: expected eof
    #  on expression: libsndfile:x64-linux:arm64-linux
    #                                     ^
    deps.clear()
    deps = [
        'libsndfile:arm64-linux',       # Also will grab libflac, libogg, and libvorbis for us
        'portaudio:arm64-linux',
    ]

    # Build the ARM64 version of the Linux dependencies
    # Note: VCPKG doesn't support building dynamic libs yet,
    #       so we have to use the static ones instead.
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

    # First make sure the lib directory is there
    os.makedirs(pathlib.Path('../lib/codecs/linux-arm64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/portaudio/linux-arm64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/sndfile/linux-arm64'), exist_ok=True)

    # Install the deps
    proc = subprocess.Popen([vcpkg_exe, 'install', *deps, '--overlay-triplets=dynamic-triplets'])
    proc.wait()
    print("All ARM64 Linux libraries built successfully!")

    # Now get the dlls that we really want
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
    
    # Clear deps list and re-add needed deps
    # Required to prevent the following error:
    # <unknown>:1:23: error: expected eof
    #  on expression: libsndfile:arm64-linux:x64-osx-dynamic
    #                                       ^
    deps.clear()
    deps = [
        'libsndfile',       # Also will grab libflac, libogg, and libvorbis for us
        'portaudio',
    ]

def BuildMacOSDeps(deps):
    # Build the x86-64 version of the macOS dependencies
    deps[0] = "libsndfile:x64-osx-dynamic"
    deps[1] = "portaudio:x64-osx-dynamic"
    
    lib_src_dir = 'installed/x64-osx-dynamic/lib/'
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
    os.makedirs(pathlib.Path('../lib/codecs/macos-x64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/portaudio/macos-x64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/sndfile/macos-x64'), exist_ok=True)

    # Install the deps
    proc = subprocess.Popen([vcpkg_exe, 'install', *deps, '--overlay-triplets=dynamic-triplets'])
    proc.wait()
    print("All x86-64 macOS libraries built successfully!")

    # Now get the dlls that we really want
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
    
    # Clear deps list and re-add needed deps
    # Required to prevent the following error:
    # <unknown>:1:23: error: expected eof
    #  on expression: libsndfile:x64-os:arm64-osx-dynamic
    #                                  ^
    deps.clear()
    deps = [
        'libsndfile:arm64-osx-dynamic',       # Also will grab libflac, libogg, and libvorbis for us
        'portaudio:arm64-osx-dynamic',
    ]

    # Build the ARM64 version of the macOS dependencies
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
    os.makedirs(pathlib.Path('../lib/codecs/macos-arm64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/portaudio/macos-arm64'), exist_ok=True)
    os.makedirs(pathlib.Path('../lib/sndfile/macos-arm64'), exist_ok=True)

    # Install the deps
    proc = subprocess.Popen([vcpkg_exe, 'install', *deps, '--overlay-triplets=dynamic-triplets'])
    proc.wait()
    print("All ARM64 macOS libraries built successfully!")

    # Now get the dlls that we really want
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
    #     dylibs = ['opus', 'mp3lame', 'FLAC', 'ogg', 'vorbis', 'vorbisenc', 'vorbisfile', 'sndfile']
    #     for lib in dylibs:
    #         lib_filename = 'lib%s.dylib' % (lib)
    #         if Path('../lib/codecs/macos-x64/{0}'.format(lib_filename)).is_file():
    #             cmd = 'install_name_tool -id "@rpath/{0}" ../lib/codec/macos-x64/{0}'.format(lib_filename)
    #             cmds.append(cmd)
    #         if Path('../lib/sndfile/macos-x64/{0}'.format(lib_filename)).is_file():
    #             cmd = 'install_name_tool -id "@rpath/{0}" ../lib/sndfile/macos-x64/{0}'.format(lib_filename)
    #             cmds.append(cmd)
    #         if Path('../lib/codecs/macos-arm64/{0}'.format(lib_filename)).is_file():
    #             cmd = 'install_name_tool -id "@rpath/{0}" ../lib/codec/macos-arm64/{0}'.format(lib_filename)
    #             cmds.append(cmd)
    #         if Path('../lib/sndfile/macos-arm64/{0}'.format(lib_filename)).is_file():
    #             cmd = 'install_name_tool -id "@rpath/{0}" ../lib/sndfile/macos-arm64/{0}'.format(lib_filename)
    #             cmds.append(cmd)
    #     print("Done!")

    #     # Now fix the references
    #     print("Fixing rpath references...")
    #     changes = [
    #         # (old, new, [targets])
    #         ('opus.0', 'opus',            ['opus', 'sndfile']),
    #         ('mp3lame.0', 'mp3lame',        ['mp3lame', 'sndfile', 'mpg123', 'syn123', 'out123']),
    #         ('FLAC.12.1.0', 'FLAC',              ['FLAC', 'sndfile']),
    #         ('ogg.0.8.5', 'ogg',                ['FLAC', 'sndfile', 'vorbis', 'vorbisenc', 'vorbisfile']),
    #         ('vorbis.0.4.9', 'vorbis',          ['sndfile', 'vorbisenc', 'vorbisfile']),
    #         ('vorbisfile.3.3.8', 'vorbisfile',  ['sndfile']),
    #         ('vorbisenc.2.0.1.2', 'vorbisenc',    ['sndfile']),
    #         ('sndfile.1.0.37', 'sndfile',        ['sndfile']),
    #     ]
    #     for (o, n, targets) in changes:
    #         for t in targets:
    #             if Path('../lib/codecs/macos-x64/lib{2}.dylib').is_file():
    #                 change_cmd = 'install_name_tool -change "@rpath/lib{0}.dylib" "@rpath/lib{1}.dylib" ../lib/codecs/macos-x64/lib{2}.dylib'.format(o, n, t)
    #                 cmds.append(change_cmd)
    #             if Path('../lib/sndfile/macos-x64/lib{2}.dylib').is_file():
    #                 change_cmd = 'install_name_tool -change "@rpath/lib{0}.dylib" "@rpath/lib{1}.dylib" ../lib/sndfile/macos-x64/lib{2}.dylib'.format(o, n, t)
    #                 cmds.append(change_cmd)
    #             if Path('../lib/codecs/macos-arm64/lib{2}.dylib').is_file():
    #                 change_cmd = 'install_name_tool -change "@rpath/lib{0}.dylib" "@rpath/lib{1}.dylib" ../lib/codecs/macos-arm64/lib{2}.dylib'.format(o, n, t)
    #                 cmds.append(change_cmd)
    #             if Path('../lib/sndfile/macos-arm64/lib{2}.dylib').is_file():
    #                 change_cmd = 'install_name_tool -change "@rpath/lib{0}.dylib" "@rpath/lib{1}.dylib" ../lib/sndfile/macos-arm64/lib{2}.dylib'.format(o, n, t)
    #                 cmds.append(change_cmd)
    #     print("Done!")

    #     # Run the commands
    #     print("Running neccessary commands...")
    #     [os.system(x) for x in cmds]
    #     print("Done!")
        
def Main():
    # Currently, I don't know how to compile libraries of every OS within one OS
    if is_windows:
        BuildWindowsDeps(deps)
    elif is_linux:
        BuildLinuxDeps(deps)
    elif is_macos:
        BuildMacOSDeps(deps)
    print("All neccessary tasks completed!")

# Runs the main function
Main()
