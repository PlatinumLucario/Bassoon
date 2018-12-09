﻿using System;
using Bassoon;

namespace BassoonSimpleExample
{
    class Program
    {
        static void Main(string[] args)
        {
            // doesn't really need to be in `using` block, but here for `IDisposable` testing
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
                    snd.Volume = 0.2f;
                    Console.WriteLine();

                    // Play it!
                    snd.Play();
                    if (snd.IsPlaying)
                        Console.WriteLine($"Playing \"{path}\"...");

                    // hold until the user quicks
                    Console.WriteLine("[Press enter to quit the program]");
                    Console.ReadLine();
                }

                Console.WriteLine("All done!");
            }
        }
    }
}