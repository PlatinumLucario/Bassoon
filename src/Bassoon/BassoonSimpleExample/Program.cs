// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using Bassoon;

namespace BassoonSimpleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // Doesn't really need to be in `using` block, but it's highly recommended as it handles automatic
            // research cleanup for you.
            using (BassoonEngine be = new BassoonEngine())
            {
                Console.WriteLine("Basson Audio!");

                // Need something to play back
                if (args.Length < 1)
                {
                    Console.WriteLine("Please specifiy a file (as the first argument) to playback");
                    return;
                }

                // Load an audiofile
                string path = args[0];
                using (Sound snd = new Sound(path))
                {
                    // Lower volume first
                    snd.Volume = 0.2f;
                    Console.WriteLine();

                    // Play it!
                    snd.Play();
                    if (snd.IsPlaying)
                        Console.WriteLine($"Playing \"{path}\" @ 20% volume...");

                    // hold until the user quits
                    Console.WriteLine("[Press enter to quit the program]");
                    Console.ReadLine();
                }

                Console.WriteLine("All done!");
            }
        }
    }
}
