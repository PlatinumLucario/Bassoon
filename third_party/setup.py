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


# If on OS, we need to fix some issues with the dylibs
# namely the rpath stuff & dylib IDs,  it's a bit of a pain
if is_osx:
    cmds = []

    # First fix Ids
    dylibs = ['ogg', 'vorbis', 'vorbisenc', 'vorbisfile', 'sndfile']
    for lib in dylibs:
        lib_filename = 'lib%s.dylib' % (lib)
        cmd = 'install_name_tool -id "@rpath/{0}" lib/{0}'.format(lib_filename)
        cmds.append(cmd)

    # Now fix the references
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

    # Run the commands
    [os.system(x) for x in cmds]

