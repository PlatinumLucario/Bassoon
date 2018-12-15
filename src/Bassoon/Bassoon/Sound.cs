using System;
using System.Runtime.InteropServices;
using libsndfileSharp;
using PortAudioSharp;

namespace Bassoon
{
    /// <summary>
    /// A piece of playable audio.
    /// </summary>
    public class Sound : IDisposable
    {
        // Flag used for the IDispoable interface
        private bool disposed = false;

        /// <summary>The actual audio data</summary>
        internal SndFile audioFile;

        /// <summary>
        /// Audio level, should be between [0.0, 1.0].
        /// 0.0 = silent, 1.0 = full volume
        /// </summary>
        internal float volume = 1;

        /// <summary>
        /// Where in the audio (in bytes) we are.
        /// </summary>
        internal long cursor = 0;

        /// <summary>
        /// If we should be currently playing audio
        /// </summary>
        internal bool playingBack = false;

        private Stream stream;

        /// <summary>
        /// How much data needs to be read when doing a playback
        /// </summary>
        internal int finalFrameSize;

        /// <summary>
        /// How many frames of audio are in the loaded file.
        /// </summary>
        private readonly long totalFrames;


        #region Constructors & Cleanup
        /// <summary>
        /// Open up an audio file for playback
        /// </summary>
        /// <param name="path">Location to where the audio file is</param>
        public Sound(string path)
        {
            BassoonEngine be = BassoonEngine.Instance;

            // Load the data file
            audioFile = new SndFile(path);

            // Setup the playback stream
            // Get the channel count
            StreamParameters oParams = be.DefaultOutputParams;
            oParams.channelCount = audioFile.Info.channels;

            // Create the stream
            stream = new Stream(
                null,
                oParams,
                audioFile.Info.samplerate,
                (uint)be.FramesPerBuffer,
                StreamFlags.ClipOff,
                playCallback,
                this
            );

            // Set how much data needs to be chunked out when playback happening
            finalFrameSize = audioFile.Info.channels * be.FramesPerBuffer;
            totalFrames = audioFile.Info.channels * audioFile.Info.frames;
        }

        ~Sound()
        {
            dispose(false);
        }

        /// <summary>
        /// Cleanup resoureces (for the IDisposable interface)
        /// </summary>
        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Does the actual disposing work
        /// </summary>
        protected virtual void dispose(bool disposing)
        {
            if (disposed)
                return;

            // Free Managed Resources
            if (disposing)
            {
                audioFile.Dispose();
                stream.Dispose();
            }

            // Free Unmanaged resources

            disposed = true;
        }
        #endregion // Constructors & Cleanup

        #region Metadata
        public string Title       { get => audioFile.Title; }
        public string Copyright   { get => audioFile.Copyright; }
        public string Software    { get => audioFile.Software; }
        public string Artist      { get => audioFile.Artist; }
        public string Comment     { get => audioFile.Comment; }
        public string Date        { get => audioFile.Date; }
        public string Album       { get => audioFile.Album; }
        public string License     { get => audioFile.License; }
        public string TrackNumber { get => audioFile.TrackNumber; }
        public string Genre       { get => audioFile.Genre; }

        public TimeSpan Duration { get => audioFile.Info.Duration; }
        #endregion // Metadata

        #region Properties
        /// <summary>
        /// Level to play back the audio at. default is 100%.
        ///
        /// When setting, this will be clamped within range in the `value`.
        /// </summary>
        /// <value>[0.0, 1.0]</value>
        public float Volume
        {
            get => volume;
            set => volume = MathHelper.Clamp(value, 0, 1);
        }

        /// <summary>
        /// See if the sound is being played back rightnow
        /// </summary>
        /// <value>true if so, false otherwise</value>
        public bool IsPlaying
        {
            get => playingBack;
        }

        /// <summary>
        /// How far in the playback is, in seconds.
        ///
        /// 0 means it's at the beginning, if it's the max duration, then it's at the end.
        /// When setting this, the value will be clamped between that range.
        /// </summary>
        /// <value>a non-negative float</value>
        public float Cursor
        {
            get => (float)(cursor) / (float)(totalFrames) * (float)audioFile.Info.Duration.TotalSeconds;
            set
            {
                // Do math
                float per = value / (float)audioFile.Info.Duration.TotalSeconds;
                long frame = (long)(per * totalFrames);

                // Clamp
                frame = MathHelper.Clamp(frame, 0, totalFrames);

                // Set (stop playback for a very short while, to stop some back skipping noises)
                bool wasPlaying = IsPlaying;
                if (cursor != totalFrames)          // this check stops a segfault when the audio has reached the end of playback
                    Pause();

                cursor = frame;
                audioFile.Seek(cursor / audioFile.Info.channels, Seek.Set);

                if (wasPlaying)
                    Play();
            }
        }

        #endregion // Properties

        #region Methods
        /// <summary>
        /// Start playing the sound
        /// </summary>
        public void Play()
        {
            playingBack = true;

            if (stream.IsStopped)
                stream.Start();
        }

        /// <summary>
        /// Pause audio playback
        /// </summary>
        public void Pause()
        {
            playingBack = false;

            if (stream.IsActive)
                stream.Stop();
        }
        #endregion // Methods

        #region PortAudio Callbacks
        /// <summary>
        /// Performs the actual audio playback
        /// </summary>
        private static StreamCallbackResult playCallback(
            IntPtr input, IntPtr output,
            System.UInt32 frameCount,
            ref StreamCallbackTimeInfo timeInfo,
            StreamCallbackFlags statusFlags,
            IntPtr dataPtr
        )
        {
            // NOTE: make sure there are no malloc in this block, as it can cause issues.
            Sound data = Stream.GetUserData<Sound>(dataPtr);

            long numRead = 0;
            unsafe
            {
                // Do a zero-out memset
                float *buffer = (float *)output;
                for (uint i = 0; i < data.finalFrameSize; i++)
                    *buffer++ = 0;

                // If we are reading data, then play it back
                if (data.playingBack)
                {
                    // Read data
                    numRead = data.audioFile.readFloat(output, data.finalFrameSize);

                    // Apply volume
                    buffer = (float *)output;
                    for (int i = 0; i < numRead; i++)
                        *buffer++ *= data.volume;
                }
            }

            // Increment the counter
            data.cursor += numRead;

            // Did we hit the end?
            // TODO is `frameCount` here wrong, or do we want to use `data.finalFrameSize`?
            if (data.playingBack && (numRead < frameCount))
            {
                // Stop playback, and reset to the beginning
                data.Cursor = 0;
                data.playingBack = false;
            }

            // Continue on
            return StreamCallbackResult.Continue;
        }
        #endregion // PortAudio Callbacks
    }
}
