// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using PortAudio;
using SndFile;

namespace Bassoon
{
    /// <summary>
    /// The Bassoon audio engine.  It gives you a way to load audio and play it back.
    ///
    /// This is a singleton and will error if you try to instantiate it twice.
    /// </summary>
    public class BassoonEngine : IDisposable
    {
        // IDisposable cleanup
        private bool disposed = false;

        /// <summary>
        /// The default settings for an output stream
        /// </summary>
        public StreamParameters DefaultOutputParams { get; private set; }

        /// <summary>
        /// How many frames to give each audio buffer
        /// </summary>
        public int FramesPerBuffer { get; private set; } = 4096;


        #region Constructors, Cleanup, & Core
        /// <summary>
        /// Retrive the instance of the Bassoon engine that's running
        /// </summary>
        /// <value>`null` if BassonEngine hasn't been setup yet.  Else, it should always contain something</value>
        public static BassoonEngine Instance { get; private set;} = null;

        /// <summary>
        /// Calling this will setup Bassoon for audio playback.  It is meant to be a singleton, so creating a second
        /// a `BassoonEngine` (if one is already running), it will throw an Exception.
        ///
        /// Note that if you have cleaned up your previous instance of this object, you can create a second one.
        /// It's recommended that you use/create this in a `using() { }` block.  See the `BassoonSimpleExample`
        /// program to see how it's done.
        /// </summary>
        public BassoonEngine()
        {
            // Load up the native libraries first
            Pa.LoadNativeLibrary();
            LibSndFile.LoadNativeLibrary();

            // First check if the singleton is setup
            if (Instance != null)
                throw new BassoonException("BassoonEngine is a singleton, and cannot be instantiated more than once");

            Pa.Initialize();
            Instance = this;

            // Try setting up an output device
            StreamParameters oParams;
            oParams.device = Pa.DefaultOutputDevice;
            if (oParams.device == Pa.NoDevice)
                throw new BassoonException("No default audio output device available");

            oParams.channelCount = 2;
            oParams.sampleFormat = SampleFormat.Float32;
            oParams.suggestedLatency = Pa.GetDeviceInfo(oParams.device).defaultLowOutputLatency;
            oParams.hostApiSpecificStreamInfo = IntPtr.Zero;

            // Set it as a the default
            DefaultOutputParams = oParams;
        }

        ~BassoonEngine()
        {
            Dispose(false);
        }

        /// <summary>
        /// Cleanup resrouces (required for IDisposable)
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does the actual disposing work
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;

            // Free Managed Resources
            if (disposing)
            {
            }

            // Free Unmanaged resources
            Pa.Terminate();

            // Reset the Singleton
            Instance = null;

            disposed = true;
        }
        #endregion // Constructors, Cleanup, & Core
    }
}
