using System;
using System.Runtime.InteropServices;

using sf_count_t = System.Int64;

namespace libsndfileSharp
{
    public static class VIOFuncs
    {
        public delegate sf_count_t GetFilelen(IntPtr user_data);                            // void *
        public delegate sf_count_t Seek(sf_count_t offset, int whence, IntPtr user_data);   // sf_count_t, int, void*
        public delegate sf_count_t Read(IntPtr ptr, sf_count_t count, IntPtr user_data);    // void*, sf_count_t, void*
        public delegate sf_count_t Write(IntPtr ptr, sf_count_t count, IntPtr user_data);   // const void*, sf_count_t, void*
        public delegate sf_count_t Tell(IntPtr user_data);                                  // void*
    }

    /// <summary>
    /// Virtual I/O functionality.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct VirtualIO
    {
        public VIOFuncs.GetFilelen get_filelen;
        public VIOFuncs.Seek seek;
        public VIOFuncs.Read read;
        public VIOFuncs.Write write;
        public VIOFuncs.Tell tell;
    }
}
