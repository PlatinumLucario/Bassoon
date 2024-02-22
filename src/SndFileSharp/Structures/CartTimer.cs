// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Runtime.InteropServices;

namespace SndFile
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CartTimer
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
        public char[] usage;
        public UInt32 value;
    }
}
