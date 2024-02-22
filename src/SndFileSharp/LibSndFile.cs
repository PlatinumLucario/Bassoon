// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NativeLibraryManager;

using SNDFILEPtr = System.IntPtr;

namespace SndFile
{
    internal static partial class Native
    {
        public const string LibSndFileLibrary = "sndfile";

        [DllImport(LibSndFileLibrary)]
        public static extern IntPtr sf_version_string();     // const char * sf_version_string (void) ;
    }

    public static class LibSndFile
    {
        public static string Version() =>
            Marshal.PtrToStringAnsi(Native.sf_version_string());

        /// <summary>
        /// This is a function that's not found in the original sndfile library.  Because of how the native libraries are
        /// packaged, this function must be called before anything else with the package.  It loads the native shared library
        /// (from an embedded resource) and makes it accessable.
        /// </summary>
        public static void LoadNativeLibrary()
        {
            /*
                Commented out in development branch so `/third_party/lib` DLLs can be used instead

            // Extract the native libraries that have been embedded and load them up
            ResourceAccessor accessor = new ResourceAccessor(Assembly.GetExecutingAssembly());
            LibraryManager libManager = new LibraryManager(
                Assembly.GetExecutingAssembly(),
                new LibraryItem(Platform.Linux, Bitness.x64,
                    new LibraryFile("libogg.so",        accessor.Binary("libogg.so")),
                    new LibraryFile("libFLAC.so",       accessor.Binary("libFLAC.so")),
                    new LibraryFile("libvorbis.so",     accessor.Binary("libvorbis.so")),
                    new LibraryFile("libvorbisfile.so", accessor.Binary("libvorbisfile.so")),
                    new LibraryFile("libvorbisenc.so",  accessor.Binary("libvorbisenc.so")),
                    new LibraryFile("libsndfile.so",    accessor.Binary("libsndfile.so"))
                ),
                new LibraryItem(Platform.MacOs, Bitness.x64,
                    new LibraryFile("libogg.dylib",        accessor.Binary("libogg.dylib")),
                    new LibraryFile("libFLAC.dylib",       accessor.Binary("libFLAC.dylib")),
                    new LibraryFile("libvorbis.dylib",     accessor.Binary("libvorbis.dylib")),
                    new LibraryFile("libvorbisfile.dylib", accessor.Binary("libvorbisfile.dylib")),
                    new LibraryFile("libvorbisenc.dylib",  accessor.Binary("libvorbisenc.dylib")),
                    new LibraryFile("libsndfile.dylib",    accessor.Binary("libsndfile.dylib"))
                ),
                new LibraryItem(Platform.Windows, Bitness.x64,
                    new LibraryFile("ogg.dll",        accessor.Binary("ogg.dll")),
                    new LibraryFile("FLAC.dll",       accessor.Binary("FLAC.dll")),
                    new LibraryFile("vorbis.dll",     accessor.Binary("vorbis.dll")),
                    new LibraryFile("vorbisfile.dll", accessor.Binary("vorbisfile.dll")),
                    new LibraryFile("vorbisenc.dll",  accessor.Binary("vorbisenc.dll")),
                    new LibraryFile("sndfile.dll",    accessor.Binary("sndfile.dll"))
                )
            );
            libManager.LoadNativeLibrary();
            */
        }
    }
}
