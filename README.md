Bassoon - Cross Platform .NET (Core) Audio Playback
===================================================

A simple C#/.NET API to load & play audio files.



Sub-Projects
------------

- `libsndfileSharp`: C# bindings to `libsndfile`.  Only supports file reading
  funcitonality at the moment.
- `FileInfo`: Example of how to use the `libsndfile` bindings, by printing
  out data about a loaded file
- `PortAudioSharp`: C# bindings to `PortAudio`.  Mostly bound, focus is on audio
  playback (but recording should work too).
- `Sinewave`: Example of how to use the `PortAudio` bindings, by playing a
  Sinewave for five seconds.



Current Limitations
-------------------

- `libsndfile` doesn't support `.mp3` reading (due to patent concerns), so it
  isn't supported at the moment.  It should be added soon enough to `Bassoon`



Help Wanted
-----------

- Audio Recording. This should be possible, it would be nice
- NuGet packaging
- Documentation fixups
  - Including have the Doxygen docs built automatically
