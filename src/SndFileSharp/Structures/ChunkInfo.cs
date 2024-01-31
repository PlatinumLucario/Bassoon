// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Runtime.InteropServices;

namespace SndFileSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ChunkInfo
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] id;           /* The chunk identifier. */

        public uint id_size;        /* The size of the chunk identifier. */
        public uint datalen;        /* The size of that data. */
        public IntPtr data;         // Orignally: void*, /* Pointer to the data. */
    }
}
