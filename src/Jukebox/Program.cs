// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using Gtk;
using Bassoon;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Jukebox
{
    class Program
    {
        private static Application App;
        private static readonly OSPlatform Linux = OSPlatform.Linux;
        private static readonly OSPlatform FreeBSD = OSPlatform.FreeBSD;

        [STAThread]
        public static void Main(string[] args)
        {
            App = Application.New("org.Bassoon.Jukebox", Gio.ApplicationFlags.FlagsNone);
            App.Register(Gio.Cancellable.GetCurrent());

            // Ensures that client side decorations aren't used outside of Linux and FreeBSD
            if (!RuntimeInformation.IsOSPlatform(Linux) | !RuntimeInformation.IsOSPlatform(FreeBSD))
                GLib.Functions.Setenv("GTK_CSD", "0", false);

            // To avoid a warning, we implement the "activate" signal here
            App.OnActivate += OnAppActivate;

            // Set an initial?
            string initial = "";
            if (args.Length > 0)
                initial = args[0].Trim();

            // Create the Jukebox
            using (BassoonEngine be = new BassoonEngine())
            {
                Jukebox juke = new Jukebox(App, initial);
                App.AddWindow(juke);

                juke.Show();
                App.Run();
            }
        }

        // Used for the OnActivate signal
        public static void OnAppActivate(object sender, EventArgs e)
        {
            // This will let you know in the terminal when the application has been activated
            Console.WriteLine("Jukebox is activated!");
            Debug.WriteLine("Jukebox is activated!");
        }
    }
}
