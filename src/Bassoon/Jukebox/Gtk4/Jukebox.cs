// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.IO;
using System.Threading;
using GLib;
using Gtk;
using Gdk;
using libsndfileSharp;
using PortAudioSharp;
using Bassoon;

using UI = Gtk.ConnectAttribute;

namespace Jukebox
{
    /// <summary>
    /// A widget that will let you load &amp; playback an audiofile (that Bassoon can decode).
    /// </summary>
    class Jukebox : Gtk.Window
    {
        // Creates a Gtk.Application instance to use, since it needs an object reference
        private readonly Gtk.Application App;

        #region From the Cambalache exported .ui file
        // Helps you pick a file
        [UI] private Button audioFileChooserButton = null;

        // Volume control widgets
        [UI] private Label volumeLevelLabel = null;
        [UI] private Scale volumeSlider = null;
        private Adjustment volumeAdj = null;

        // Playback Widgets
        [UI] private Label playbackTimeLabel = null;
        [UI] private Scale playbackSlider = null;
        private Adjustment playbackAdj = null;
        [UI] private ToggleButton playButton = null;
        [UI] private Button rewindButton = null;

        // Label to display info to the user
        [UI] private Label statusLabel = null;
        #endregion // From the Cambalache exported .ui file

        #region Playback data
        /// <summary>
        /// Currently loaded audiofile (used as a same guard)
        /// </summary>
        private string currentFilePath = "";

        /// <summary>
        /// Filename (without folders) of the loaded audiofile (used for display info)
        /// </summary>
        private string loadedFilename = "";

        /// <summary>
        /// Volume of playback; value between [0.0, 1.0] mapping 0%-100%
        /// </summary>
        private float volume = 1;

        /// <summary>
        /// Flag for if we're moving the slider (for playback)
        /// </summary>
        private bool movingPlaybackSlider = false;

        /// <summary>
        /// Actual audio object, for playback and whatnot
        /// </summary>
        private Sound audio = null;
        #endregion // Playback data


        /// <summary>
        /// Create the jukebox
        /// </summary>
        /// <param name="app">Current GTK4 application in use.</param>
        /// <param name="initialFile">Initial file to load, if one is desired.</param>
        public Jukebox(Gtk.Application app, string initialFile = "")
            : this(new Builder("Jukebox.ui"))
        {
            App = app; // Copies the current app info to this instance, so its functions can be used
            if (initialFile != "")
                loadAudio(initialFile);
        }

        /// <summary>
        /// Private construtor, that setups up the GUI
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private Jukebox(Builder builder) : base(builder.GetPointer("Jukebox"), true)
        {
            builder.Connect(this);

            // Initially, playback widgets are disabled
            enablePlaybackWidgets(false);

            OnCloseRequest += Window_OnCloseRequest;
        }

        /// <summary>
        /// (Try to) Load an audio file.  Will set the GUI.!--
        ///
        /// Only will change the state of the app if the loaded file is different.
        /// </summary>
        /// <param name="path">Audio file to load</param>
        private void loadAudio(string path)
        {
            // Check for samsies
            if (currentFilePath == path)
                return;

            // New!
            unloadAudio();  // Clean out the old

            // Load the new
            loadedFilename = System.IO.Path.GetFileName(path);
            string newStatusText = "";
            try
            {
                audio = new Sound(path);
                audio.Volume = volume;
                currentFilePath = path;
                newStatusText = $"Loaded {loadedFilename}";
            }
            catch (SndFileException ex)
            {
                newStatusText = $"Error decoding {loadedFilename}";
                Console.WriteLine($"loadAudio() SndFileException: {ex.Message}");
            }
            catch (PortAudioException ex)
            {
                newStatusText = $"Error setting up stream for {loadedFilename}";
                Console.WriteLine($"loadAudio() PortAudioException: {ex.Message}");
            }

            // Reset GUI
            enablePlaybackWidgets(audio != null);
            gtkDo(delegate {
                playButton.Active = false;      // Reset play button
                playbackAdj.Value = 0;          // Reset to beginning
                statusLabel.SetText(newStatusText);
                audioFileChooserButton.SetLabel(loadedFilename);
            });
        }

        /// <summary>
        /// Unloads any current audio file
        /// </summary>
        private void unloadAudio()
        {
            if (audio != null)
            {
                audio.Pause();
                audio.Dispose();
                audio = null;
            }
        }

        /// <summary>
        /// Perform a callback, but in the Gtk Thread.
        ///
        /// This should be used when setting data/elements that live in the GUI
        /// </summary>
        private void gtkDo(Gio.Internal.AsyncReadyCallback callback)
        {
            if (callback is null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
        }

        /// <summary>
        /// Set if the playback widgets (play, rewind, &amp; scrub) should be clickable or not
        /// </summary>
        /// <param name="enable"></param>
        private void enablePlaybackWidgets(bool enable)
        {
            gtkDo(delegate {
                playButton.Sensitive = enable;
                rewindButton.Sensitive = enable;
                playbackSlider.Sensitive = enable;
            });
        }

        #region Widget callbacks
        /// <summary>
        /// Activated when the window is closed.  End the application
        /// </summary>
        private bool Window_OnCloseRequest(object sender, EventArgs a)
        {
            unloadAudio();
            App.Quit();
            return true;
        }

        /// <summary>
        /// Tripped when the volume slider (off to the right) is changed.  Adjusts playback
        /// volume.
        /// </summary>
        private void onVolumeSliderChanged(object sender, EventArgs args)
        {
            volumeLevelLabel.SetText($"{volumeSlider.Adjustment.Value.ToString("0")}%");
            volume = (float)volumeSlider.Adjustment.Value / (float)volumeAdj.Upper;

            // Adjust volume of currently playing sound
            if (audio != null)
                audio.Volume = volume;
        }

        /// <summary>
        /// Activated when the file chooser dialog reports a file has been selected.
        ///
        /// Will (try to) load an audio file.
        /// </summary>
        private void onAudioFileChoosen(object sender, EventArgs args) =>
            loadAudio(audioFileChooserButton.Label);

        /// <summary>
        /// If the rewind button was clicked, restart playback
        /// <param name="sender"></param>
        private void onRewindClicked(object sender, EventArgs args)
        {
            if (audio != null)
            {
                audio.Cursor = 0;       // Reset audio to the start
                playbackAdj.Value = 0;  // Reset playback slider to 0
            }
        }

        /// <summary>
        /// Activated when the play/pause button has been clicked/toggled.!--
        ///
        /// Will either play or pause any audio that is paused or is playing (respectively).
        /// </summary>
        private void onPlayToggled(object sender, EventArgs args)
        {
            // play/pause audio
            if (audio != null)
            {
                string statusText;

                // Do we play or pause?
                if (playButton.Active)
                {
                    audio.Play();
                    statusText = $"Playing {loadedFilename}";
                    GLib.Functions.TimeoutAdd(0, 50, new GLib.SourceFunc(checkPlayback));        // 20x a second, nice and smooth
                }
                else
                {
                    audio.Pause();
                    statusText = $"Paused {loadedFilename}";
                }

                // Update GUI
                gtkDo(delegate { statusLabel.SetText(statusText); });
            }
        }

        /// <summary>
        /// When the playback slider's value has been changed (either programatically or via the user),
        /// this will update the label to display a time code.
        /// </summary>
        private void onPlaybackSliderValueChanged(object sender, EventArgs args)
        {
            // Function will be called upon app startup, so do this to prevent a segfault (until a file is loaded)
            if (audio == null)
            {
                playbackTimeLabel.SetText("00:00:00");
                return;
            }

            // Map the slider's value to a time
            System.TimeSpan curTime = System.TimeSpan.FromSeconds(playbackSliderSeconds());
            playbackTimeLabel.SetText(curTime.ToString(@"hh\:mm\:ss"));
        }

        /// <summary>
        /// If the playback slider has been dragged (by the user), this will adjust the playback where the audio is playing from.
        /// </summary>
        private void onPlaybackSliderDragged(object sender, EventArgs args) =>
            movingPlaybackSlider = true;

        /// <summary>
        /// If the playback slider has been dragged by the user, this will set the audio (playing or not) to that location.
        /// </summary>
        private void onPlaybackSliderMouseUp(object sender, EventArgs args)
        {
            movingPlaybackSlider = false;

            // We need an audio file to work with
            if (audio == null)
                return;

            // Set cursor
            audio.Cursor = (float)playbackSliderSeconds();
        }
        #endregion // Widget callbacks

        /// <summary>
        /// This function runs in the Gtk GUI thread, as a timeout (about 20 times a second)
        /// </summary>
        /// <returns>if `true`, this function will run again.  if `false`, then it won't, until
        /// it has been kicked off by `Glib.Timeout()`</returns>
        private bool checkPlayback()
        {
            // We need to have an audio file
            if (audio == null)
                return false;

            if (audio.IsPlaying)
            {
                // adjust the slider if playing (and not scrubbing)
                if (!movingPlaybackSlider)
                {
                    double playbackValue = map(audio.Cursor, 0, audio.Duration.TotalSeconds, playbackAdj.Lower, playbackAdj.Upper);
                    playbackAdj.Value = playbackValue;
                }

                return true;
            }

            // If not, don't do anything
            playButton.Active = false;      // Un-toggle play
            return false;
        }

        /// <summary>
        /// Get the value of the playback slider, in seconds.
        /// </summary>
        /// <returns>a non-negative number, with a max value of the length of the current audio file</returns>
        private double playbackSliderSeconds() =>
            map(playbackAdj.Value, playbackAdj.Lower, playbackAdj.Upper, 0, audio.Duration.TotalSeconds);

        /// <summary>
        /// Map a value (x) from one range [a, b] to a new range [p, q]
        /// </summary>
        /// <param name="x">value to map</param>
        /// <param name="a">old min</param>
        /// <param name="b">old max</param>
        /// <param name="p">new min</param>
        /// <param name="q">new max</param>
        /// <returns></returns>
        private static double map(double x, double a, double b, double p, double q) =>
            (x - a) / (b - a) * (q - p) + p;
    }
}
