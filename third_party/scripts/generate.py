#!/usr/bin/env python3
# Copyright (c)  2023  Xiaomi Corporation

import os
import argparse
import re
import shutil
from pathlib import Path

import jinja2

# This function will add the version number string
def get_version(l):
    if l == "pa": # If it's portaudio
        return "19.7.5" # It will return with this version number
    elif l == "sf": # If it's sndfile
        return "1.2.2" # It will return with this version number
    else: # Failsafe, incase the library name is neither of them
        return ""

# This function will open the '[libraryname].csproj.runtime.in' file
def read_proj_file(filename):
    with open(filename) as f:
        return f.read()

# This function creates a dictionary to store template strings
def get_dict(l):
    version = get_version(l)
    return {
        "version": get_version(l),
    }

# This function creates one project at a time and applies the template strings
def create_project(s, ln, p, p_name, p_label, libs, a, d):
    # First, the directories need to be made
    os.makedirs('../projects/' + ln[0] + '/' + p_name + '/' + 'lib-' + a, exist_ok=True)
    
    # Then we create the .csproj file by opening it in write mode
    with open("../projects/" + ln[0] + "/" + p_name + "/" + ln[0] + ".runtime." + a + ".csproj", "w") as f:
        ci = [] # A list needs to be created for the ci variable
        for lib in libs: # Next, we begin the for loop for the lib names
            # If the path for the file matches for either one, it will copy the file to its respective directory
            if Path('../lib/codecs/' + p_name + '-' + a + '/' + lib).is_file():
                shutil.copyfile('../lib/codecs/' + p_name + '-' + a + '/' + lib, '../projects/' + ln[0] + '/' + p_name + '/' + 'lib-' + a + '/' + lib)
            elif Path('../lib/' + ln[0] + '/' + p_name + '-' + a + '/' + lib).is_file():
                shutil.copyfile('../lib/' + ln[0] + '/' + p_name + '-' + a + '/' + lib, '../projects/' + ln[0] + '/' + p_name + '/' + 'lib-' + a + '/' + lib)
            
            # This will append the text of the Content Include to the 'ci' list variable
            ci.append(
                "    <Content Include=" + '"' + 'lib-' + a + '/' + lib + '">\n' + 
                "      <PackagePath>runtimes/" + d["dotnet_rid"] + "/native/%(Filename)%(Extension)</PackagePath>\n" + 
                "      <Pack>true</Pack>\n" + 
                "      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\n" + 
                "    </Content>\n")
            
            # This will join the content in the 'ci' list to one string to add to the content_include template
            d["content_include"] = ' '.join(ci)
        
        # After the loop is over, this will write to the file based on what's assigned to the templates
        environment = jinja2.Environment() # First, the jinja2 environment
        template = environment.from_string(s) # Then using the templates to convert from the strings
        s = template.render(**d) # Now we render the templates!
        f.write(s) # And then we write to the .csproj file
    
    # Now we copy the README.md to its respective project directory
    shutil.copyfile('../projects/' + ln[0] + '/README.md', '../projects/' + ln[0] + '/' + p_name + '/README.md')

# This will process for x64 based architecture
def process_x64(s, l, ln, p, p_name, p_label, libs):
    d = get_dict(l)
    a = "x64"
    d["dotnet_rid"] = p + "-" + a
    d["cpu"] = "x86-64"
    d["platform"] = p_label
    create_project(s, ln, p, p_name, p_label, libs, a, d)

# This will process for ARM64 based architectures
def process_arm64(s, l, ln, p, p_name, p_label, libs):
    d = get_dict(l)
    a = "arm64"
    d["dotnet_rid"] = p + "-" + a
    d["cpu"] = "ARM64"
    d["platform"] = p_label
    create_project(s, ln, p, p_name, p_label, libs, a, d)

# This will process the Linux libraries
def process_linux(s, l, ln):
    libs = []
    for lib in ln:
        # Vcpkg creates a static Linux portaudio library by default
        if lib == "portaudio": # If it's portaudio, append it with a .a extension
            libs.append("lib" + lib + ".a")
        else: # Otherwise if it's anything else, append it with a .so extension
            libs.append("lib" + lib + ".so")
    p = "linux"
    p_name = p
    p_label = "Linux"
    process_x64(s, l, ln, p, p_name, p_label, libs)
    
    # Because Vcpkg doesn't have dynamic Linux ARM64 libraries yet,
    # we have to rely on the static ones instead
    libs.clear()
    for lib in ln:
        libs.append("lib" + lib + ".a")
    process_arm64(s, l, ln, p, p_name, p_label, libs)

# This will process the macOS libraries
def process_macos(s, l, ln):
    libs = []
    for lib in ln:
        libs.append("lib" + lib + ".dylib") # All macOS libs are named as 'lib[libraryname].dylib'
    p = "osx"
    p_name = "macos"
    p_label = "macOS"
    process_x64(s, l, ln, p, p_name, p_label, libs)
    process_arm64(s, l, ln, p, p_name, p_label, libs)

# This will process the Windows libraries
def process_windows(s, l, ln):
    libs = []
    for lib in ln:
        # Vcpkg hasn't fixed the name inconsistancy with mp3lame yet, so it's still named 'libmp3lame'
        if lib == "mp3lame": # So if the library is mp3lame
            libs.append("lib" + lib + ".dll") # We make sure the name matches the file name
        else: # Otherwise if it's any other library
            libs.append(lib + ".dll") # We'll just name it as [libraryname].dll as usual
    p = "win"
    p_name = "windows"
    p_label = "Windows"
    process_x64(s, l, ln, p, p_name, p_label, libs)
    process_arm64(s, l, ln, p, p_name, p_label, libs)

# This will process the portaudio library
def process_portaudio():
    # First, we need to read the project file
    s = read_proj_file("../projects/portaudio/portaudio.csproj.runtime.in")
    l = "pa" # 'pa', short for portaudio, used for the get_version() function
    ln = ["portaudio"] # We only need portaudio for this one
    process_macos(s, l, ln)
    process_linux(s, l, ln)
    process_windows(s, l, ln)

# This will process the sndfile library
def process_sndfile():
    # First, we need to read the project file
    s = read_proj_file("../projects/sndfile/sndfile.csproj.runtime.in")
    l = "sf" # 'sf', short for sndfile, used for the get_version() function
    
    # This will create a list of the sndfile library and its dependencies
    ln = [
        "sndfile",
        "FLAC",
        "mp3lame",
        "mpg123",
        "out123",
        "syn123",
        "ogg",
        "opus",
        "vorbis",
        "vorbisenc",
        "vorbisfile",
        ]
    process_macos(s, l, ln)
    process_linux(s, l, ln)
    process_windows(s, l, ln)

# The main function
def main():
    process_portaudio()
    process_sndfile()


if __name__ == "__main__":
    main()
