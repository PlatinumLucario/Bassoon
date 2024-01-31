// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Runtime.InteropServices;

namespace SndFileSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Instrument
    {
        public int gain;
        public char basenote, detune;
        public char velocity_lo;
        public char velocity_hi;
        public char key_lo;
        public char key_hi;
        public int loop_count;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=16)]
        public _loop[] loops;

        /// <summary>
        /// Semi-internal loop structure
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct _loop {
            public int mode;
            public UInt32 start;
            public UInt32 end;
            public UInt32 count;
        };
    }
}
