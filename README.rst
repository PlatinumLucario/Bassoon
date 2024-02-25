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
Though soon enough they will be up on Nuget. Currently, the native libraries are included here in
this repo, so the projects can be copied into the binary directory when building, without having
to install VCPKG or msys2 in order to get them. However, do note that the dependencies for each
operating system and architecture is not complete yet, however it would be great if anyone
contributes the needed libraries.

In the meantime, look at the `Developing`_ section for how to update the libraries or get the
native libraries for your platform.

Currently uses these versions of the native libraries:

* ``libogg``         1.3.5#1
* ``libflac``        1.4.3
* ``libvorbis``      1.3.7#2
* ``libopus``        1.4
* ``libmp3lame``     3.100#11
* ``libmpg123``      1.31.3#4
* ``libsndfile``     1.2.2
* ``libportaudio``   19.7#5

Also, do note that libsndfile relies on the following libraries:
libogg, libflac, libvorbis, libopus, libmp3lame, and libmpg123.

Same thing applies with libvorbis, which relies on:
libvorbisenc, and libvorbisfile.

And don't forget about libmpg123 too, which relies on:
libout123, and libsyn123.




**************************************
A Note About Running on Linux and macOS
**************************************

If you're only targeting windows, then you don't need to pay attention to this.  But then why would
you be using a cross platform library? :]

At the moment of writing this (Bassoon v.1.1.2), this project uses the most excellent
NativeLibraryManager_ (v1.0.23) to well, manage the native libraries.  When executing the
``dotnet run`` command, it will extract the native libraries to the directory you run from.  At the
moment, you need to specify your current working directory as part of the dynamic library search
path.  So this is the easiest thing to do:

.. code-block:: bash

   # On Linux
   $ export LD_LIBRARY_PATH=$LD_LIBRARY_PATH:`pwd`

   # On OS X
   $ export DYLD_LIBRARY_PATH=$DYLD_LIBRARY_PATH:`pwd`

Also, you will need to install the ``libsndfile1-dev`` package, if you're running on Linux.

Hopefully in a future release this little nuisance can be fixed.



********************
A Short Code Example
********************

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

* ``SndFileSharp``: C# bindings to ``libsndfile``.  Only supports file reading
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

* Lacks the ability to pan audio (also called '3D Sound')
* Can't record audio yet
* Lacks fade-in/fade-out feature



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
   want it to be baked into your dev environment).

2. Run the third party setup script. To setup your vcpkg environment, Either run
   ``setup_vcpkg_env.sh`` (macOS and Linux) or ``setup_vcpkg_env.bat`` (Windows),
   or do it manually:
     Make sure to set the envrionment variable ``VCPKG_DIR`` to where you installed Vcpkg.
     If you are running Windows, you only need to set the environment variable for ``VGPKG_DIR``
     to the directory of your Vcpkg installation. However, if you're running Linux or macOS, you will
     need to run the following commands in a terminal:

   .. code-block:: bash

      cd third_party/scripts
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
   all of the ``<EmbeddedResource ...>`` tags, they will tell you what native library files need to
   be placed alongside each project.

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
.. _NativeLibraryManager: https://github.com/olegtarasov/NativeLibraryManager
