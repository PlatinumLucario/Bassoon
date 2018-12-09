using System;
using System.Runtime.InteropServices;

using SNDFILEPtr = System.IntPtr;

namespace libsndfileSharp
{
    internal static partial class Native
    {
        public const string libsndfileDLL = "sndfile";

        [DllImport(libsndfileDLL)]
        public static extern IntPtr sf_version_string();     // const char * sf_version_string (void) ;
    }

    public static class libsndfile
    {
        public static string Version() =>
            Marshal.PtrToStringAnsi(Native.sf_version_string());
    }
}
