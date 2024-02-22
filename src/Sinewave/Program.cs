// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Threading;
using PortAudio;

namespace Sinewave
{
    /// <summary>
    /// This is example usage of the PortAudioSharp bindings, adapted from the native PortuAudio's own Sinewave
    /// exmaple.
    /// </summary>
    class Program
    {
        public const int NumSeconds = 5;
        public const int SampleRate = 44100;
        public const int FramesPerBuffer = 64;
        public const int TableSize = 200;

        /// <summary>
        /// Small object that contains info for playing a sinewave
        /// </summary>
        public class SinewaveData
        {
            public float[] sine = new float[TableSize];
            public int left_phase = 0;
            public int right_phase = 0;
            public string message = "";

            public SinewaveData()
            {
                for(int i = 0; i < TableSize; i++)
                    sine[i] = (float)Math.Sin(((double)i / (double)TableSize) * Math.PI * 2);
            }
        }

        /// <summary>
        /// This routine will be called by the PortAudio engine when audio is needed.
        /// It may called at interrupt level on some machines so don't do anything
        /// that could mess up the system like calling malloc() or free().
        ///
        /// In this example, it playes a sine wave
        /// </summary>
        public static StreamCallbackResult PlaySinewaveCallback(
            IntPtr input, IntPtr output,
            System.UInt32 frameCount,
            ref StreamCallbackTimeInfo timeInfo,
            StreamCallbackFlags statusFlags,
            IntPtr userDataPtr
        )
        {
            // Retrive the sinewave object
            SinewaveData data = Stream.GetUserData<SinewaveData>(userDataPtr);

            unsafe
            {
                float *audio = (float *)output;

                for (uint i = 0; i < frameCount; i++)
                {
                    *audio++ = data.sine[data.left_phase];      // Left
                    *audio++ = data.sine[data.right_phase];     // Right

                    data.left_phase += 1;
                    if (data.left_phase >= TableSize)
                        data.left_phase -= TableSize;

                    // Higher pitch to we can distinguish left and right
                    data.right_phase += 3;
                    if (data.right_phase >= TableSize)
                        data.right_phase -= TableSize;
                }
            }

            return StreamCallbackResult.Continue;
        }

        // Print a message when done
        public static void Finished(IntPtr userDataPtr)
        {
            SinewaveData data = Stream.GetUserData<SinewaveData>(userDataPtr);
            Console.WriteLine($"Stream Completed: {data.message}");
        }


        static void Main(string[] args)
        {
            // Not originally present in PortAudio library, but required to call before using it
            Pa.LoadNativeLibrary();

            // Print info
            Console.WriteLine($"(PortAudio version no: {Pa.VersionInfo.versionText})");
            Console.WriteLine($"PortAudio Test: output sine wave. SR = {SampleRate}, BufSize = {FramesPerBuffer}");

            // Setup data
            SinewaveData data = new SinewaveData();
            data.message = "No Message";

            // Init PortAudio
            Pa.Initialize();

            // Try setting up an output device
            StreamParameters oParams;
            oParams.device = Pa.DefaultOutputDevice;
            if (oParams.device == Pa.NoDevice)
            {
                Console.WriteLine("Error, no default output device.");
                Pa.Terminate();
                return;
            }
            oParams.channelCount = 2;
            oParams.sampleFormat = SampleFormat.Float32;
            oParams.suggestedLatency = Pa.GetDeviceInfo(oParams.device).defaultLowOutputLatency;
            oParams.hostApiSpecificStreamInfo = IntPtr.Zero;

            // Try to setup a stream
            Stream audio = new Stream(
                null, oParams,
                SampleRate,
                FramesPerBuffer,
                StreamFlags.ClipOff,
                PlaySinewaveCallback,
                data
            );
            audio.SetFinishedCallback(Finished);

            // play
            audio.Start();
            Console.WriteLine($"Play for {NumSeconds} seconds.");
            Thread.Sleep(NumSeconds * 1000);
            audio.Stop();

            // Clean
            audio.Close();
            audio.Dispose();

            Pa.Terminate();
            Console.WriteLine("Test Finished");
        }
    }
}
