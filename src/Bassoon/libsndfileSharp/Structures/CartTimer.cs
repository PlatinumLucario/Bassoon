using System;
using System.Runtime.InteropServices;

namespace libsndfileSharp
{
    [StructLayout(LayoutKind.Sequential)]
    public struct CartTimer
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst=4)]
        public char[] usage;
        public UInt32 value;
    }
}
