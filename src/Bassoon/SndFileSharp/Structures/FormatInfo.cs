// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System.Runtime.InteropServices;

namespace libsndfileSharp
{
    /// <summary>
    /// The SF_FORMAT_INFO struct is used to retrieve information about the sound
    /// file formats libsndfile supports using the sf_command () interface.
    ///
    /// Using this interface will allow applications to support new file formats
    /// and encoding types when libsndfile is upgraded, without requiring
    /// re-compilation of the application.
    ///
    /// Please consult the libsndfile documentation (particularly the information
    /// on the sf_command () interface) for examples of its use.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FormatInfo
    {
        public int format;

        [MarshalAs(UnmanagedType.LPStr)]
        public string name;        // `const char *`

        [MarshalAs(UnmanagedType.LPStr)]
        public string extension;   // `const char *`
    }
}
