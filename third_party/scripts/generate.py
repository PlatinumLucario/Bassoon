#!/usr/bin/env python3
# Copyright (c)  2023  Xiaomi Corporation

import os
import argparse
import re
import shutil
from pathlib import Path

import jinja2

def get_version(l):
    if l == "pa":
        return "19.7.5"
    elif l == "sf":
        return "1.2.2"
    else:
        return ""

def read_proj_file(filename):
    with open(filename) as f:
        return f.read()


def get_dict(l):
    version = get_version(l)
    return {
        "version": get_version(l),
    }

def create_project(s, ln, p, p_name, p_label, libs, a, d):
    os.makedirs('../projects/' + ln[0] + '/' + p_name + '/' + 'lib-' + a, exist_ok=True)
    with open("../projects/" + ln[0] + "/" + p_name + "/" + ln[0] + ".runtime." + a + ".csproj", "w") as f:
        ci = []
        for lib in libs:
            if Path('../lib/codecs/' + p_name + '-' + a + '/' + lib).is_file():
                shutil.copyfile('../lib/codecs/' + p_name + '-' + a + '/' + lib, '../projects/' + ln[0] + '/' + p_name + '/' + 'lib-' + a + '/' + lib)
            elif Path('../lib/' + ln[0] + '/' + p_name + '-' + a + '/' + lib).is_file():
                shutil.copyfile('../lib/' + ln[0] + '/' + p_name + '-' + a + '/' + lib, '../projects/' + ln[0] + '/' + p_name + '/' + 'lib-' + a + '/' + lib)
            ci.append(
                "    <Content Include=" + '"' + 'lib-' + a + '/' + lib + '">\n' + 
                "      <PackagePath>runtimes/" + d["dotnet_rid"] + "/native/%(Filename)%(Extension)</PackagePath>\n" + 
                "      <Pack>true</Pack>\n" + 
                "      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>\n" + 
                "    </Content>\n")
            d["content_include"] = ' '.join(ci)
        environment = jinja2.Environment()
        template = environment.from_string(s)
        s = template.render(**d)
        f.write(s)
    shutil.copyfile('../projects/' + ln[0] + '/README.md', '../projects/' + ln[0] + '/' + p_name + '/README.md')

def process_x64(s, l, ln, p, p_name, p_label, libs):
    d = get_dict(l)
    a = "x64"
    d["dotnet_rid"] = p + "-" + a
    d["cpu"] = "x86-64"
    d["platform"] = p_label
    create_project(s, ln, p, p_name, p_label, libs, a, d)

def process_arm64(s, l, ln, p, p_name, p_label, libs):
    d = get_dict(l)
    a = "arm64"
    d["dotnet_rid"] = p + "-" + a
    d["cpu"] = "ARM64"
    d["platform"] = p_label
    create_project(s, ln, p, p_name, p_label, libs, a, d)

def process_linux(s, l, ln):
    libs = []
    for lib in ln:
        libs.append("lib" + lib + ".so")
    p = "linux"
    p_name = p
    p_label = "Linux"
    process_x64(s, l, ln, p, p_name, p_label, libs)
    process_arm64(s, l, ln, p, p_name, p_label, libs)

def process_macos(s, l, ln):
    libs = []
    for lib in ln:
        libs.append("lib" + lib + ".dylib")
    p = "osx"
    p_name = "macos"
    p_label = "macOS"
    process_x64(s, l, ln, p, p_name, p_label, libs)
    process_arm64(s, l, ln, p, p_name, p_label, libs)

def process_windows(s, l, ln):
    libs = []
    for lib in ln:
        if lib == "mp3lame":
            libs.append("lib" + lib + ".dll")
        else:
            libs.append(lib + ".dll")
    p = "win"
    p_name = "windows"
    p_label = "Windows"
    process_x64(s, l, ln, p, p_name, p_label, libs)
    process_arm64(s, l, ln, p, p_name, p_label, libs)

def process_portaudio():
    s = read_proj_file("../projects/portaudio/portaudio.csproj.runtime.in")
    l = "pa"
    ln = ["portaudio"]
    process_macos(s, l, ln)
    process_linux(s, l, ln)
    process_windows(s, l, ln)

def process_sndfile():
    s = read_proj_file("../projects/sndfile/sndfile.csproj.runtime.in")
    l = "sf"
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

def main():
    process_portaudio()
    process_sndfile()


if __name__ == "__main__":
    main()
