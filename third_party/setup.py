#!/usr/bin/python3

import os, sys
import pathlib
import subprocess
import shutil

# Some flags
is_windows = (sys.platform == 'win32') or (sys.platform == 'msys')
is_linux = sys.platform == 'linux'
is_osx = sys.platform == 'darwin'

deps = [
    'libsndfile',       # Also will grab libflac, libogg, and libvorbis for us
    'portaudio',
]
vcpkg_exe = ''
dlls = []
dll_renaming_rules = [] # list of two-element tuples
dll_src_dir =   ''

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
    dll_src_dir = 'installed/x64-windows/bin/'
    dlls = [
        'FLAC.dll',
        'ogg.dll',
        'vorbis.dll',
        'vorbisenc.dll',
        'vorbisfile.dll',
        'libsndfile-1.dll',
        'portaudio.dll',
    ]
    dll_renaming_rules = [
        ('libsndfile-1.dll', 'sndfile.dll')
    ]
elif is_linux:
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg')
    dll_src_dir = 'installed/x64-linux/lib/'
    dlls = [
        'libFLAC.so',
        'libogg.so',
        'libvorbis.so',
        'libvorbisenc.so',
        'libvorbisfile.so',
        'libsndfile-shared.so',
        'libportaudio.so',
    ]
    dll_renaming_rules = [
        ('libsndfile-shared.so', 'libsndfile.so')
    ]
elif is_osx:
    vcpkg_exe = os.path.join(vcpkg_dir, 'vcpkg')
    dll_src_dir = 'installed/x64-osx/lib/'
    dlls = [
        'libFLAC.dylib',
        'libogg.dylib',
        'libvorbis.dylib',
        'libvorbisenc.dylib',
        'libvorbisfile.dylib',
        'libsndfile-shared.dylib',
        'libportaudio.dylib',
    ]
    dll_renaming_rules = [
        ('libsndfile-shared.dylib', 'libsndfile.dylib')
    ]

# First make sure the lib directory is there
pathlib.Path('lib').mkdir(exist_ok=True)


# Install the deps
proc = subprocess.Popen([vcpkg_exe, 'install', *deps, '--overlay-triplets=dynamic-triplets'])
proc.wait()

# Now get the dlls that we really want
for dll in dlls:
    src = os.path.join(vcpkg_dir, dll_src_dir, dll)
    shutil.copy(src, 'lib/')

# Fix names
for (src_name, dst_name) in dll_renaming_rules:
    os.rename(
        os.path.join('lib/', src_name),
        os.path.join('lib/', dst_name)
    )
