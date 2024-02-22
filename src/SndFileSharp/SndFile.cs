// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Runtime.InteropServices;

using SNDFILEPtr = System.IntPtr;
using sf_count_t = System.Int64;

namespace SndFile
{
    internal static partial class Native
    {
        [DllImport(LibSndFileLibrary)]
        public static extern SNDFILEPtr sf_open(
            [MarshalAs(UnmanagedType.LPStr)] string path,       // Originally `const char *`
            SFMode mode,
            ref Info sfinfo
        );

        [DllImport(LibSndFileLibrary)]
        public static extern int sf_close(SNDFILEPtr sndfile);

        [DllImport(LibSndFileLibrary)]
        public static extern int sf_current_byterate(SNDFILEPtr sndfile);

        [DllImport(LibSndFileLibrary)]
        public static extern IntPtr sf_strerror(SNDFILEPtr sndfile);   // Orignially `const char *`

/*
        [DllImport(LibSndFileLibrary)]
        public static extern int sf_set_string(
            SNDFILEPtr sndfile,
            [MarshalAs(UnmanagedType.I4)] Str str_type,         // Originally `int`
            [MarshalAs(UnmanagedType.LPStr)] string str         // Originally `const char *`
        );
*/
        [DllImport(LibSndFileLibrary)]
        public static extern sf_count_t sf_seek(
            SNDFILEPtr sndfile,
            sf_count_t frames,
            [MarshalAs(UnmanagedType.I4)] Seek whence           // Originally `int`
        );

        [DllImport(LibSndFileLibrary)]
        public static extern IntPtr sf_get_string(              // Originally `const char *`
            SNDFILEPtr sndfile,
            [MarshalAs(UnmanagedType.I4)] Str str_type          // Orignally `int`
        );

        [DllImport(LibSndFileLibrary)]
        public static extern sf_count_t sf_read_float(
            SNDFILEPtr sndfile,
            IntPtr ptr,                                         // Originally `float *`
            sf_count_t items
        );

        [DllImport(LibSndFileLibrary)]
        [return: MarshalAs(UnmanagedType.I4)]
        public static extern SFBool sf_command(
            SNDFILEPtr sndfile,
            [MarshalAs(UnmanagedType.I4)] Command command,      // Originally `int`
            IntPtr data,                                        // Originally `void *`
            int datasize
        );
    }

    /// <summary>
    /// Right now only supports reading files
    /// </summary>
    public class Sf : IDisposable
    {
        private bool disposed = false;

        private SNDFILEPtr handle;
        public bool IsFileOpen { get; private set; } = false;
        public Info Info { get; private set; }

        public Sf(string path)
        {
            Info info = new Info();
            handle = Native.sf_open(path, SFMode.Read, ref info);

            if (handle == IntPtr.Zero)
            {
                // Something bad happened, throw an error
                throw new SndFileException(strError());
            }

            IsFileOpen = true;
            Info = info;
        }

/*
        // Write constructor
        private SndFile(string path, Info info)
        {

        }
*/

        #region IDisposable
        ~Sf()
        {
            dispose(false);
        }

        public void Dispose() {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        /// Does the actual cleanup work
        protected virtual void dispose(bool disposing)
        {
            if (disposed)
                return;

            // Free Managed Resources
            if (disposing)
            {
            }

            // Free Unmanaged resources
            Close();

            disposed = true;
        }
        #endregion // IDisposable

        private string strError() =>
            Marshal.PtrToStringAnsi(Native.sf_strerror(handle));

        /// <summary>
        /// Close the SNDFILE and clean up all memory allocations associated with this
        /// file.
        /// </summary>
        public void Close()
        {
            if (IsFileOpen)
            {
                Native.sf_close(handle);
                IsFileOpen = false;
            }
        }

        public int CurrentByterate
        {
            get => Native.sf_current_byterate(handle);
        }

/*
        public LoopInfo LoopInfo
        {
            get
            {
                // Alloc some unmanaged memory
                int numBytes = Marshal.SizeOf<LoopInfo>();
                Console.WriteLine(numBytes);
                IntPtr ptr = Marshal.AllocHGlobal(numBytes);

                // Get the info
                SFBool good = Native.sf_command(handle, Command.GetLoopInfo, ptr, numBytes);

                // Decode
                LoopInfo info = (good == SFBool.True) ? Marshal.PtrToStructure<LoopInfo>(ptr) : new LoopInfo();

                // Cleanup
                Marshal.FreeHGlobal(ptr);

                // Return (or error)
                if (good == SFBool.True)
                    return info;
                else
                    throw new SndFileException(strError());
            }
        }
*/

        #region Metadata
        public string GetString(Str str) =>
            Marshal.PtrToStringAnsi(Native.sf_get_string(handle, str));

        public string Title         { get => GetString(Str.Title); }
        public string Copyright     { get => GetString(Str.Copyright); }
        public string Software      { get => GetString(Str.Software); }
        public string Artist        { get => GetString(Str.Artist); }
        public string Comment       { get => GetString(Str.Comment); }
        public string Date          { get => GetString(Str.Date); }
        public string Album         { get => GetString(Str.Album); }
        public string License       { get => GetString(Str.License); }
        public string TrackNumber   { get => GetString(Str.TrackNumber); }
        public string Genre         { get => GetString(Str.Genre); }
        #endregion // Metadata

        /// <summary>
        /// Read a chunk of data in terms of items (using Pointers)
        ///
        /// See `float[] Read()` for a slightly safter version.  This should only be used if
        /// you know what you're doing.
        ///
        /// Unlike `Read()`, this doesn't perform any copies.
        /// </summary>
        /// <param name="ptr">Buffer to write data to</param>
        /// <param name="items">how much to read</param>
        /// <returns>How many items were read</returns>
        public sf_count_t readFloat(IntPtr ptr, sf_count_t items) =>
            Native.sf_read_float(handle, ptr, items);

        /// <summary>
        /// Read a chunk of data from the sound file, as a floating point
        /// </summary>
        /// <param name="numItems"></param>
        /// <returns></returns>
        public float[] Read(sf_count_t numItems)
        {
            // Alloc
            IntPtr ptr = Marshal.AllocHGlobal(sizeof(float) * (int)numItems);

            // Do a read (and copy)
            sf_count_t numRead = readFloat(ptr, numItems);
            float[] data = new float[numRead];
            Marshal.Copy(ptr, data, 0, data.Length);

            // Cleanup
            Marshal.FreeHGlobal(ptr);

            return data;
        }

        /// <summary>
        /// Seek within the waveform data chunk of the SNDFILE. sf_seek () uses
        /// the same values for whence (SEEK_SET, SEEK_CUR and SEEK_END) as
        /// stdio.h function fseek ().
        /// An offset of zero with whence set to SEEK_SET will position the
        /// read / write pointer to the first data sample.
        /// On success sf_seek returns the current position in (multi-channel)
        /// samples from the start of the file.
        /// Please see the libsndfile documentation for moving the read pointer
        /// separately from the write pointer on files open in mode SFM_RDWR.
        /// On error all of these functions return -1.
        /// </summary>
        public sf_count_t Seek(sf_count_t frames, Seek whence) =>
            Native.sf_seek(handle, frames, whence);
    }
}
