using System;
using System.Runtime.InteropServices;

using HostApiIndex = System.Int32;
using Time = System.Double;

namespace PortAudioSharp
{
    /// <summary>
    /// A structure providing information and capabilities of PortAudio devices.
    /// Devices may support input, output or both input and output.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceInfo
    {
        public int structVersion;           // this is struct version 2

        [MarshalAs(UnmanagedType.LPStr)]
        public string name;                 // Originally: `const char *`

        public HostApiIndex hostApi;        // note this is a host API index, not a type id

        public int maxInputChannels;
        public int maxOutputChannels;

        // Default latency values for interactive performance.
        public Time defaultLowInputLatency;
        public Time defaultLowOutputLatency;

        // Default latency values for robust non-interactive applications (eg. playing sound files).
        public Time defaultHighInputLatency;
        public Time defaultHighOutputLatency;

        public double defaultSampleRate;
    }
}
