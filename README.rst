.. contents::

###################################################
Bassoon - Cross Platform .NET (Core) Audio Playback
###################################################

* ``libsndfileSharp`` |libsndfileSharp_badge|_
* ``PortAudioSharp`` |PortAudioSharp_badge|_
* ``Bassoon`` |Bassoon_badge|_


A simple C#/.NET API to load & play audio files.  Currently supports WAV, AIFF, OGG, Vorbis, FLAC,
and more (see ``libsndfile``'s official documentation), on Windows, OS X, and Linux.

Right now, the native libraries aren't packaged on NuGet, so you will need to build them yourself.
Though soon enough they will be up on Nuget.  In the meantime, look at the `Developing`_ section
for how to get the native libraries for your platform.

Currently uses these versions of the native libraries:

* ``libgg``       1.3.4
* ``libflac``     1.3.3
* ``libvorbis``   1.3.6
* ``libsndfile``  1.0.29-8
* ``portaudio``   2020-02-02



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

Steps 1 & 2 only need to be run once.  After that, you can go ahead to step 3 any time you want to
start working on the C# component of this project.

You'll at least need some standard C/C++ compiler envrionment and Python 3 installed.  A note for
windows is that an MSYS2 environment was used for development (i.e. Bash), but you should be also
able to use a Powershell environment too; though that's undocumented.  I do recommend MSYS2/bash
for Windows though.

1. Microsoft's Vcpkg_ is what's used to build the native libraries.  It's pretty simple to get
   setup.  In their ``README``, document follow their ``Quick Start`` section.  All you need to do
   is have it bootstrapped and you should be fine (no need to do ``integrate install`` if you don't
   want to bake you dev environment.

2. Run the third party setup script.  Make sure to set the envrionment variable ``VCPKG_DIR`` to
   where you installed Vcpkg.

   .. code-block:: bash

      cd third_party/
      export VCPKG_DIR=<Vcpkg install dir>      # e.g. export VCPKG_DIR=~/vcpkg
      python3 setup.py

   Now the setup script will run; this could take a bit.  Once it's done, do ``ls lib/``.  There
   should be some DLLs (or shared libraries) for your system.  Look to see that a ``sndfile`` and a
   ``portaudio`` are found.

3. Set the environment (from the root of the project directory).  C# (.NET) needs to be able to find
   the native libraries, and this needs to be done before you launch the .NET runtime:

   .. code-block:: bash

      source set_dev_env.h

You should be good to go at this point.  To test that everthing worked fine, I recommend trying to
run the ``Jukebox`` sample.  Go into it's directory and do ``dotnet run``.  If it launches fine,
then that means portaudio is working fine. And if you can load a song and play it back then you're
good!



*********************
Making NuGet Packages
*********************

I've tried to set this up so it's as simple as possible to make packages for NuGet, but it's a
little bit inovled still.

1. You will need to build the native libraries for each platform.  Collect them from each respective
   system's ``/third_party/lib/`` output, and then put them on the computer where you want to build
   the packages (I recommend on Linux).

2. Switch to the branch ``release_nuget_packaging``.

3. Look at the project files for ``PortAudioSharp.csproj`` and ``libsndfileSharp.csproj``.  At all
   all of the ``<EmbeddedResource ...>`` tags, they will tell you want native library files need to
   be places alongside each project.

4. Go to the root directory of this project, and run the following commands:

   .. code-block:: bash

      source set_dev_env.sh
      ./mk_nuget_packges.sh

   If everthing went fine, that you should see the ``*.nupkg``'s right in the root directory.  If
   not, you'll probably see some errors.  If they say "Error reading resource", that most likely
   means that one of the projects wasn't able to find a native DLL.  Double check that you put them
   all in their correct places.



*********
Licensing
*********

Bassoon (e.g. the contents of ``src/``) are available under the Apache License 2.0.  The entire text
can be found in ``LICENSE.txt``.  Bassoon does use five other external C libraries; Xiph's libOGG,
libVorbis, & libFLAC, libsndfile, and PortAudio.  These are all availble under various FLOSS
licenses.  Please check their respective websites (or source code) for details.



.. |libsndfileSharp_badge| image:: https://badge.fury.io/nu/libsndfileSharp.svg
.. _libsndfileSharp_badge: https://badge.fury.io/nu/libsndfileSharp

.. |PortAudioSharp_badge| image:: https://badge.fury.io/nu/PortAudioSharp.svg
.. _PortAudioSharp_badge: https://badge.fury.io/nu/PortAudioSharp

.. |Bassoon_badge| image:: https://badge.fury.io/nu/Bassoon.svg
.. _Bassoon_badge: https://badge.fury.io/nu/Bassoon

.. _Vcpkg: https://github.com/microsoft/vcpkg
