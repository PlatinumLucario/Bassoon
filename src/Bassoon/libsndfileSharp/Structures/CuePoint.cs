// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Runtime.InteropServices;

namespace libsndfileSharp
{
    /// <summary>
    /// Struct used to retrieve cue marker information from a file
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CuePoint
    {
        public Int32 indx;
        public UInt32 position;
        public Int32 fcc_chunk;
        public Int32 chunk_start;
        public Int32 block_start;
        public UInt32 sample_offset;

        [MarshalAs(UnmanagedType.LPStr, SizeConst=256)]
        public string name;     // `char [256]`
    }
}
