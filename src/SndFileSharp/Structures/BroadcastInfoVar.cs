// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Runtime.InteropServices;

namespace SndFile
{
    /// <summary>
    /// Struct used to retrieve broadcast (EBU) information from a file.
    /// Strongly (!) based on EBU "bext" chunk format used in Broadcast WAVE.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BroadcastInfoVar
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=256)]
        public char[] description;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
        public char[] originator;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=32)]
        public char[] originator_reference;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=10)]
        public char[] origination_date;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
        public char[] origination_time;

        public UInt32 time_reference_low;
        public UInt32 time_reference_high;
        public short version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] umid;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=190)]
        public char[] reserved;

        public UInt32 coding_history_size;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=256)]
        public char[] coding_history;
    }
}
