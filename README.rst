.. contents::

###################################################
Bassoon - Cross Platform .NET (Core) Audio Playback
###################################################

* ``libsndfileSharp`` |libsndfileSharp_badge|_
* ``PortAudioSharp`` |PortAudioSharp_badge|_
* ``Bassoon`` |Bassoon_badge|_


A simple C#/.NET API to load & play audio files.  Currently supports WAV, AIFF, OGG, Vorbis, FLAC,
and more (see ``libsndfile``'s official documentation), on Windows, OS X, and Linux.

This project now includes all the native nuget libraries! Yes, all of them! No more hassles of having
to set up the dev environment manually anymore, that's all in the past now :)

The native libraries included here in this repo are automatically copied into the binary directory
when building on the Debug configuration, without any need for VCPKG or msys2 in order to get them.

If you'd like to update the native libraries, see the `Developing`_ section for how to update the
libraries or to contribute further to the project.

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




***************************************
A Note About Running on Linux and macOS
***************************************

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

  * Some help with setting up CI for automatic updating of native library packages.

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

Steps 1 & 2 only need to be run once to update the native libraries.  After that, you can go ahead
to step 3 any time you want to start working on the C# component of this project.

You'll at least need some standard C/C++ compiler envrionment and Python 3 installed.  A note for
Windows is that an MSYS2 environment was originally used for development (i.e. Bash), but you should
be able to use a Powershell environment too; though that's undocumented. I do recommend MSYS2/bash
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
run the ``Jukebox`` sample.  Go into its directory and do ``dotnet run``.  If it launches fine,
then that means portaudio is working fine. And if you can load a song and play it back then you're
good!



*********************
Making NuGet Packages
*********************

Making the NuGet packages is much more easier than it was before. For native libraries, all you need
to do is do the following:
1. (Optional) If you need to setup Vcpkg without having to do several commands manually, run either
``setup_vcpkg_env.bat`` (Windows) or ``setup_vcpkg_env.sh``. Please note that it will be set up in the
following locations: ``C:\src\vcpkg`` in Windows, and ``~/source/vcpkg`` in Linux and macOS.

2. Run the ``run.bat`` (Windows) or ``run.sh`` (Linux and macOS) file, and the libraries will
be updated in the Vcpkg directory, the native library projects will be generated, then the native
libraries will be copied to the projects, then the projects are built, packed and all ready in
``./third_party/packages/``.

3. To pack the PortAudioSharp, SndFileSharp and Bassoon projects into NuGet packages, either run
``mk_nuget_packges.bat`` (Windows) or ``mk_nuget_packges.sh`` (Linux and macOS).

If everthing went fine, that you should see the ``*.nupkg``'s right in the root directory. If
not, you'll probably see some errors. If it says "Error reading resource", then that most likely
means that one of the projects wasn't able to find a native DLL. Double check that you put them
all in their correct places and let me know of the error so I can fix the issue in the script,
or submit a MR with the fixes, so I can merge in the fixes.

A big special thanks to [Fangjun Kuang](https://github.com/csukuangfj) and their [PortAudioSharp2 repo](https://github.com/csukuangfj/PortAudioSharp2)
for this awesome script that packages native libraries! This has helped improve the libraries
so much that it can now be built with little effort.



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
