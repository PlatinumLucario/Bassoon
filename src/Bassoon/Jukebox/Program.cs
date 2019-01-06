// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using Gtk;
using Bassoon;

namespace Jukebox
{
    class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Application.Init();

            Application app = new Application("org.Bassoon.Jukebox", GLib.ApplicationFlags.None);
            app.Register(GLib.Cancellable.Current);

            // Set an initial?
            string initial = "";
            if (args.Length > 0)
                initial = args[0].Trim();

            // Create the Jukebox
            using (BassoonEngine be = new BassoonEngine())
            {
                Jukebox juke = new Jukebox(initial);
                app.AddWindow(juke);

                juke.Show();
                Application.Run();
            }
        }
    }
}
