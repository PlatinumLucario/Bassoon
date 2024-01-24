// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using Gtk;
using Bassoon;

namespace Jukebox
{
    class Program
    {
        private static Application App;

        [STAThread]
        public static void Main(string[] args)
        {
            App = Application.New("org.Bassoon.Jukebox", Gio.ApplicationFlags.FlagsNone);
            App.Register(Gio.Cancellable.GetCurrent());

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
    }
}
