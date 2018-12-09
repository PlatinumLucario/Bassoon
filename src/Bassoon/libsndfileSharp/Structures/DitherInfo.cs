using System.Runtime.InteropServices;

namespace libsndfileSharp
{
    /// <summary>
    /// Enums and typedefs for adding dither on read and write.
    /// See the html documentation for sf_command(), SFC_SET_DITHER_ON_WRITE
    /// and SFC_SET_DITHER_ON_READ.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DitherInfo
    {
        public int type;
        public double level;

        [MarshalAs(UnmanagedType.LPStr)]
        public string name;     // `const char *`
    }
}
