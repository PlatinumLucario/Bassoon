// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System.Runtime.InteropServices;

namespace SndFileSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct LoopInfo
    {
        public short time_sig_num;  /* any positive integer    > 0  */
        public short time_sig_den;  /* any positive power of 2 > 0  */
        public int loop_mode;       /* see SF_LOOP enum             */

        public int num_beats;       /* this is NOT the amount of quarter notes !!!*/
                                    /* a full bar of 4/4 is 4 beats */
                                    /* a full bar of 7/8 is 7 beats */

        public float bpm;           /* suggestion, as it can be calculated using other fields:*/
                                    /* file's length, file's sampleRate and our time_sig_den*/
                                    /* -> bpms are always the amount of _quarter notes_ per minute */

        public int root_key;        /* MIDI note, or -1 for None */

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=6)]
        public int[] future;
    }
}
