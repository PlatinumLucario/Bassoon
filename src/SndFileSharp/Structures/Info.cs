// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Text;
using System.Runtime.InteropServices;

using sf_count_t = System.Int64;

namespace SndFile
{
    internal static partial class Native
    {
        [DllImport(LibSndFileLibrary)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern SFBool sf_format_check(ref Info info);     // Original: int sf_format_check(const SF_INFO *info);
    }


    /// <summary>
    /// A pointer to a SF_INFO structure is passed to sf_open () and filled in.
    /// On write, the SF_INFO structure is filled in by the user and passed into
    /// sf_open ().
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Info
    {
        public sf_count_t frames;  /* Used to be called samples.  Changed to avoid confusion. */
        public int samplerate;
        public int channels;
        public int format;
        public int sections;

        [MarshalAs(UnmanagedType.I4)]
        public SFBool seekable;

        public override string ToString() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[Info:");
            sb.AppendLine($"  frames: {frames}");
            sb.AppendLine($"  samplerate: {samplerate}");
            sb.AppendLine($"  channels: {channels}");
            sb.AppendLine($"  format: {format}");
            sb.AppendLine($"  sections: {sections}");
            sb.AppendLine($"  seekable: {seekable}");
            sb.AppendLine("]");
            return sb.ToString();
        }

        /// <summary>
        /// Check if the Info structure is valid
        /// </summary>
        /// <returns>Return TRUE if fields of the SF_INFO struct are a valid combination of values.</returns>
        public bool FormatCheck() =>
            Native.sf_format_check(ref this) == SFBool.True;

        /// <summary>
        /// The runtime of the audio.
        ///
        /// NOTE: this is not present in the original library.
        /// </summary>
        /// <value></value>
        public TimeSpan Duration
        {
            get => TimeSpan.FromSeconds((double)frames / (double)samplerate);
        }
    }
}
