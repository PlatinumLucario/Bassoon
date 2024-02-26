# Introduction

C# binding for [portaudio][portaudio] supporting Linux, macOS, and Windows in all supported CPU architectures.

This sub project of Bassoon includes the improvements made by [Fangjun Kuang's repo](https://github.com/csukuangfj/PortAudioSharp2/),
which means it includes the native libraries alongside the project as separate nuget packages as well.

See <https://www.nuget.org/packages/PortAudioSharp>

The binding code is copied from [PortAudioSharp][PortAudioSharp].

You can also find its usage for real time speech recognition from a microphone using
[sherpa-onnx](https://github.com/k2-fsa/sherpa-onnx) at
<https://github.com/k2-fsa/sherpa-onnx/tree/master/dotnet-examples/speech-recognition-from-microphone>

Unlike the original [PortAudioSharp][PortAudioSharp] (which it is forked from), this project packages the
pre-compiled [portaudio][portaudio] library into a separate `nuget` package, and uses the nuget package,
which simplifies user's life.

[PortAudioSharp]: https://gitlab.com/define-private-public/Bassoon/-/tree/develop/src/Bassoon/PortAudioSharp
[portaudio]: https://github.com/PortAudio/portaudio

If you'd like to help contribute to the project, by all means, the entire repo is here:
https://gitlab.com/PlatinumLucario/Bassoon/
