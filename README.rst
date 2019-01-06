.. contents::

###################################################
Bassoon - Cross Platform .NET (Core) Audio Playback
###################################################

* ``libsndfileSharp`` |libsndfileSharp_badge|_
* ``PortAudioSharp`` |PortAudioSharp_badge|_
* ``Bassoon`` |Bassoon_badge|_


A simple C#/.NET API to load & play audio files.  Currently supports WAV, AIFF, OGG, Vorbis, FLAC, and more (see
``libsndfile``'s official documentation), on Windows, OS X, and Linux.

Right now, native the native libraries aren't packaged on NuGet, so you will need to build them yourself.  See the
`Developing`_ section for how to do this.



***************
A Short Example
***************

.. code-block:: csharp

   using Bassoon;
   /// ...
   
   using (BassonEngine be = new BassoonEngine())
   {
       Sound snd = new Sound("Kenny Loggins - I'm Free (Heaven Helps the Man).ogg");
       snd.Volume = 0.85f;         // Play at 85% volume
       snd.Play();
   
       /// ...
   }



************
Sub-Projects
************

These are all found in ``src/Bassoon/``

* ``libsndfileSharp``: C# bindings to ``libsndfile``.  Only supports file reading
  funcitonality at the moment.
* ``FileInfo``: Example of how to use the ``libsndfile`` bindings, by printing
  out data about a loaded file
* ``PortAudioSharp``: C# bindings to ``PortAudio``.  Mostly bound, focus is on audio
  playback (but recording should work too).
* ``Sinewave``: Example of how to use the ``PortAudio`` bindings, by playing a
  Sinewave for five seconds.
* ``BassonSimpleExample``: A very simple example of how to use Bassoon to play
  back an audio file in the console.
* ``Jukebox``: A more complex example of Basson, which uses Gtk 3.x to create a
  mini audio player.  You can pause, play, rewind, scrub, and adjust the volume.



*******************
Current Limitations
*******************

* ``libsndfile`` doesn't support MP3 reading (due to patent concerns), so it
  isn't supported at the moment.  It should be added soon enough to ``Bassoon``

  * IIRC, the patents did expire, so it's possible that MP3 decoding may be added
    to libsndfile in the near future.



***********
Help Wanted
***********

* NuGet packaging

  * I'd like to have this library on NuGet, along w/ the required native libraries.

* Audio Recording. This should be possible, it would be nice

  * This also includes being able to save audio, it's fully possible with libsndfile,
    but I have yet to tinker with that

* Documentation fixups

  * Better styling & CSS
  * Mainpage
  * Including have the Doxygen docs built automatically, and then published on GitLab pages

* Adding a "pan audio" feature

  * Being able to place the audio in a 3D environment would be cool too

* Fade-in/fade-out feature
* Some sort of "playlist" object/class/feature
* A logo



**********
Developing
**********

* These are the common requirements.  I'd recommend getting them from a package manager (e.g. Homebrew on OS X, and
  MSYS2 on Windows):

  * CMake
  * Make
  * GCC (or a compatible C compiler)
  * Python (it's a libsndfile requirement)
  * GNU Autotools
  * pkg-config
  * .NET Core runtime

* Windows: You'll need MSYS2 installed, along with the the ``mingw-w64-x86_64-toolchain`` package installed

  * Make sure your version of ``gcc`` is at least 8.x.  I was having compile errors with earlier releases.



1. Get & build the native libraries.  You only need to do this once (unless you remove everything).

   .. code-block:: bash

      cd third_party
      cmake .
      make

2. Set the environment.  C# (.NET) needs to be able to find the native libraries, and this needs to be done
   before you launch the .NET runtime:

   .. code-block:: bash

      source set_dev_env.h



*********
Licensing
*********

Bassoon (e.g. the contents of ``src/``) are available under the Apache License 2.0.  The entire text
can be found in ``LICENSE.txt``.  Bassoon does use five other external C libraries; Xiph's libOGG,
libVorbis, & libFLAC, libsndfile, and PortAudio.  These are all availble under various FLOSS
licenses.  Please check their respective websites (or source code) for details.
libvorbis, 



.. |libsndfile_badge| image:: https://badge.fury.io/nu/libsndfileSharp.svg
.. _libsndfile_badge: https://badge.fury.io/nu/libsndfileSharp

.. |PortAudio_badge| image:: https://badge.fury.io/nu/PortAudioSharp.svg
.. _PortAudio_badge: https://badge.fury.io/nu/PortAudioSharp

.. |Bassoon_badge| image:: https://badge.fury.io/nu/Bassoon.svg
.. _Bassoon_badge: https://badge.fury.io/nu/Bassoon
