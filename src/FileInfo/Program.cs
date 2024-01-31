// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using SndFileSharp;

namespace FileInfo
{
    class Program
    {
        static void Main(string[] args)
        {
            // Not originally present in the sndfile library, but required to call before using it
            LibSndFile.LoadNativeLibrary();

            Console.WriteLine("Hello libsndfile#!");
            Console.WriteLine($"Native library version: {LibSndFile.Version()}");

            using (SndFile wav = new SndFile(args[0]))
            {
                Console.WriteLine(wav.Info);
                Console.WriteLine($"Format good?: {wav.Info.FormatCheck()}");
                Console.WriteLine($"Current Byterate: {wav.CurrentByterate}");
                Console.WriteLine($"Runtime: {wav.Info.Duration}");

                Console.WriteLine("Metadata: ");
                Console.WriteLine($"  Title       = {wav.Title}");
                Console.WriteLine($"  Copyright   = {wav.Copyright}");
                Console.WriteLine($"  Software    = {wav.Software}");
                Console.WriteLine($"  Artist      = {wav.Artist}");
                Console.WriteLine($"  Comment     = {wav.Comment}");
                Console.WriteLine($"  Date        = {wav.Date}");
                Console.WriteLine($"  Album       = {wav.Album}");
                Console.WriteLine($"  License     = {wav.License}");
                Console.WriteLine($"  TrackNumber = {wav.TrackNumber}");
                Console.WriteLine($"  Genre       = {wav.Genre}");

                // Set seek to start
                wav.Seek(0, Seek.Set);

                // Try reading some sample data
                Console.WriteLine("Some audio data, (as floats):");
                float[] data = wav.Read(2);
                foreach (float d in data)
                    Console.Write($"{d} ");
                Console.WriteLine();
            }
        }
    }
}
