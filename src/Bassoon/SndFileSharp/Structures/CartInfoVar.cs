// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Runtime.InteropServices;

namespace libsndfileSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CartInfoVar
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
        public char[] version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] title;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] artist;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] cut_id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] client_id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] category;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] classification;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] out_cue;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=10)]
        public char[] start_date;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
        public char[] start_time;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=10)]
        public char[] end_date;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
        public char[] end_time;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] producer_app_id;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] producer_app_version;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=64)]
        public char[] user_def;

        public Int32 level_reference;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=8)]
        public CartTimer[] post_timers;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=276)]
        public char[] reserved;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=1024)]
        public char[] url;

        public UInt32 tag_text_size;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst=256)]
        public char[] tag_text;
    }
}
