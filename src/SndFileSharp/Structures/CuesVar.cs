// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System.Runtime.InteropServices;

namespace SndFileSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CuesVar
    {
        public System.UInt32 cue_count;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=100)]
        public CuePoint[] cue_points;
    }
}
