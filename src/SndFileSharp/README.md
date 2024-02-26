# Introduction

C# binding for [libsndfile][libsndfile] supporting Linux, macOS, and Windows in all supported CPU architectures.

This is a sub project of Bassoon, and originally was callled [libsndfileSharp][libsndfileSharp], but this forked
sub project was completely renamed to 'SndFileSharp' due to how the native library was named in Windows.

This repo has improvements inspired by [Fangjun Kuang's PortAudioSharp2 repo](https://github.com/csukuangfj/PortAudioSharp2/),
which means it includes the native libraries alongside the project as separate nuget packages as well.

See <https://www.nuget.org/packages/SndFileSharp>

Unlike the original [libsndfileSharp][libsndfileSharp] (which it is forked from), this project packages the
pre-compiled [libsndfile][libsndfile] library into a separate `nuget` package, and uses the nuget package,
which simplifies user's life.

The native libsndfile nuget package also includes the following libraries:
* ``libogg``         1.3.5#1
* ``libflac``        1.4.3
* ``libvorbis``      1.3.7#2
* ``libopus``        1.4
* ``libmp3lame``     3.100#11
* ``libmpg123``      1.31.3#4

The original repos can be found here:
[libsndfileSharp]: https://gitlab.com/define-private-public/Bassoon/-/tree/develop/src/Bassoon/libsndfileSharp
[libsndfile]: https://github.com/libsndfile/libsndfile

If you'd like to help contribute to the project, by all means, the entire repo is here:
https://gitlab.com/PlatinumLucario/Bassoon/
