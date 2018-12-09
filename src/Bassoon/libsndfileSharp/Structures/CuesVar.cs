using System.Runtime.InteropServices;

namespace libsndfileSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CuesVar
    {
        public System.UInt32 cue_count;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=100)]
        public CuePoint[] cue_points;
    }
}
