Bassoon - Cross Platform .NET (Core) Audio Playback
===================================================

A simple C#/.NET API to load & play audio files.



Sub-Projects
------------

These are all found in `src/Bassoon/`

- `libsndfileSharp`: C# bindings to `libsndfile`.  Only supports file reading
  funcitonality at the moment.
- `FileInfo`: Example of how to use the `libsndfile` bindings, by printing
  out data about a loaded file
- `PortAudioSharp`: C# bindings to `PortAudio`.  Mostly bound, focus is on audio
  playback (but recording should work too).
- `Sinewave`: Example of how to use the `PortAudio` bindings, by playing a
  Sinewave for five seconds.
- `BassonSimpleExample`: A very simple example of how to use Bassoon to play
  back an audio file in the console.
- `Jukebox`: A more complex example of Basson, which uses Gtk 3.x to create a
  mini audio player.  You can pause, play, rewind, scrub, and adjust the volume.



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


Developing
----------

- Common requirements: CMake, GCC (or a compatible C compiler), GNU autotools, pkg-config, .NET Core runtime
- Windows: You'll need MSYS2 installed, along with the the `mingw-w64-x86_64-toolchain` package installed
  - Make sure your version of `gcc` is at least 8.x.  I was having compile errors with earlier releases.



1. Get & build the native libraries.  You only need to do this once (unless you
   remove everything).
   
   ```bash
   cd third_party
   cmake .
   make
   ```

2. Set the environment.  C# (.NET) needs to be able to find the native libraries.
   - On Linux & OS X: `source set_dev_env.sh`
   - Windows: TODO add a windows batch script?  `set_dev_env.sh` can also be used w/ MSYS2 bash
