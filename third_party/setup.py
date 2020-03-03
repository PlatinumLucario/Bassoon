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

# TODO document this part in a readme
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

## export the deps
#proc = subprocess.Popen([vcpkg_exe, 'export', *deps, '--raw'])
#proc.wait()
#
## Get the directory where the DLLs were exported to
#vcpkg_export_dir = list(filter(lambda x: x.startswith('vcpkg-export-'), os.listdir(vcpkg_dir)))[0]
#vcpkg_export_dir = os.path.join(vcpkg_dir, vcpkg_export_dir)

# Now get the dlls that we really want
for dll in dlls:
#    src = os.path.join(vcpkg_export_dir, dll_src_dir, dll)
    src = os.path.join(vcpkg_dir, dll_src_dir, dll)
    shutil.copy(src, 'lib/')

# Remove the export dir from VCPKG
#shutil.rmtree(vcpkg_export_dir)

# Fix names
for (src_name, dst_name) in dll_renaming_rules:
    os.rename(
        os.path.join('lib/', src_name),
        os.path.join('lib/', dst_name)
    )
