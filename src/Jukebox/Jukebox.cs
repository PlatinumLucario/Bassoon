// License:     APL 2.0
// Author:      Benjamin N. Summerton <https://16bpp.net>

using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using GLib;
using Gtk;
using Gdk;
using SndFile;
using PortAudio;
using Bassoon;

using UI = Gtk.ConnectAttribute;

namespace Jukebox
{
    /// <summary>
    /// A widget that will let you load &amp; playback an audiofile (that Bassoon can decode).
    /// </summary>
    class Jukebox : Gtk.Window
    {
        #region Core Widgets
        // Creates a Gtk.Application instance to use, since it needs an object reference
        private readonly Gtk.Application App;

        // Used for File Dialogs depending on GTK4 library version
        private Gtk.FileChooserNative AudioFileChooser = null;
        private Gtk.FileDialog AudioFileDialog = null;
        private Gtk.FileFilter AcceptableAudioFiles = null;

        // Gestures are used for button inputs
        private Gtk.GestureClick PlaybackSliderClick = null;
        #endregion // Core Widgets

        #region From the Cambalache exported .ui file
        // Helps you pick a file
        [UI] private Button AudioFileDialogButton = null;
        [UI] private Label AudioFileDialogButtonLabel = null;

        // Volume control widgets
        [UI] private Label VolumeLevelLabel = null;
        [UI] private Scale VolumeSlider = null;

        // Playback Widgets
        [UI] private Label PlaybackTimeLabel = null;
        [UI] private Scale PlaybackSlider = null;
        [UI] private ToggleButton PlayButton = null;
        [UI] private Button RewindButton = null;

        // Label to display info to the user
        [UI] private Label StatusLabel = null;
        #endregion // From the Cambalache exported .ui file

        #region Playback data
        /// <summary>
        /// Currently loaded audiofile (used as a same guard)
        /// </summary>
        private string CurrentFilePath = "";

        /// <summary>
        /// Filename (without folders) of the loaded audiofile (used for display info)
        /// </summary>
        private string LoadedFilename = "";

        /// <summary>
        /// Volume of playback; value between [0.0, 1.0] mapping 0%-100%
        /// </summary>
        private float Volume = 1;

        /// <summary>
        /// Flag for if we're moving the slider (for playback)
        /// </summary>
        private bool MovingPlaybackSlider = false;

        /// <summary>
        /// Actual audio object, for playback and whatnot
        /// </summary>
        private Sound Audio = null;
        #endregion // Playback data

        #region Internal library handlers and callbacks
        /// <summary>
        /// Internal callback for Gtk.FileDialog. Because there's a bug in Gir.Core 0.4.0 currently. So this is the only workaround for now.
        /// </summary>
        private Gio.Internal.AsyncReadyCallback FileDialogCallback { get; set; }

        /// <summary>
        /// Internal ErrorHandle for Gtk.FileDialog. Because there's a bug in Gir.Core 0.4.0 currently. So this is the only workaround for now.
        /// </summary>
        private GLib.Internal.ErrorOwnedHandle ErrorHandle = new GLib.Internal.ErrorOwnedHandle(IntPtr.Zero);
        #endregion // Internal library handlers and callbacks

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
                LoadAudio(initialFile);
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
            EnablePlaybackWidgets(false);
            
            // We need to create a new instance of any non-builder widgets
            PlaybackSliderClick = Gtk.GestureClick.New();
            VolumeSlider.Adjustment = Gtk.Adjustment.New(100, 0, 100, 1, 10, 0);
            PlaybackSlider.Adjustment = Gtk.Adjustment.New(0, 0, 100, 1, 10, 0);

            // Then we need to add PlaybackSliderClick to the PlaybackSlider event controller
            PlaybackSlider.AddController(PlaybackSliderClick);

            // Now we call the signals and connect them to each signal function
            AudioFileDialogButton.OnClicked += OnActivateFileDialog;
            VolumeSlider.OnValueChanged += OnVolumeSliderChanged;
            
            PlaybackSlider.OnChangeValue += OnPlaybackSliderChangeValue;
            PlaybackSlider.OnMoveSlider += OnPlaybackSliderDragged;
            PlaybackSlider.OnValueChanged += OnPlaybackSliderValueChanged;
            PlaybackSliderClick.OnStopped += OnPlaybackSliderMouseUp;
            PlaybackSliderClick.OnCancel += OnPlaybackSliderMouseUp;
            PlaybackSliderClick.OnPressed += OnPlaybackSliderDragged;
            PlaybackSliderClick.OnReleased += OnPlaybackSliderMouseUp;
            PlaybackSliderClick.OnUnpairedRelease += OnPlaybackSliderMouseUp;
            PlaybackSliderClick.OnEnd += OnPlaybackSliderMouseUp;
            PlaybackSliderClick.OnBegin += OnPlaybackSliderDragged;

            PlayButton.OnToggled += OnPlayToggled;
            RewindButton.OnClicked += OnRewindClicked;

            OnCloseRequest += Window_OnCloseRequest;
        }

        private void OnActivateFileDialog(object sender, EventArgs args)
        {
            // File Filters are made here
            AcceptableAudioFiles = FileFilter.New();
            AcceptableAudioFiles.SetName("Audio Files");

            // Since IFileDialog in Windows doesn't support MIME Types directly, file extension patterns need to be added
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                AcceptableAudioFiles.AddPattern("*.mp3");
                AcceptableAudioFiles.AddPattern("*.ogg");
                AcceptableAudioFiles.AddPattern("*.flac");
                AcceptableAudioFiles.AddPattern("*.opus");
                AcceptableAudioFiles.AddPattern("*.wav");
            }
            else
            {
                AcceptableAudioFiles.AddMimeType("audio/mpeg");
                AcceptableAudioFiles.AddMimeType("audio/ogg");
                AcceptableAudioFiles.AddMimeType("audio/flac");
                AcceptableAudioFiles.AddMimeType("audio/vorbis");
                AcceptableAudioFiles.AddMimeType("audio/opus");
                AcceptableAudioFiles.AddMimeType("audio/wav");
            }

            // Failsafe incase a Linux distribution uses a GTK4 library lower than 4.9
            if (Gtk.Functions.GetMinorVersion() >= 9) // Uses FileDialog if 4.9 or higher
            {
                AudioFileDialog = FileDialog.New();
                AudioFileDialog.SetTitle("Select Audio File to Play");
                AudioFileDialog.SetModal(true);
                var filters = Gio.ListStore.New(FileFilter.GetGType());
                filters.Append(AcceptableAudioFiles);
                AudioFileDialog.SetFilters(filters);
                FileDialogCallback = (source, res, data) =>
                {
                    var fileHandle = Gtk.Internal.FileDialog.OpenFinish(AudioFileDialog.Handle, res, out ErrorHandle);
                    if (fileHandle != IntPtr.Zero)
                    {
                        var path = Marshal.PtrToStringUTF8(Gio.Internal.File.GetPath(fileHandle).DangerousGetHandle());
                        LoadAudio(path!);
                        GObject.Internal.Object.Unref(fileHandle);
                        AudioFileDialog.Unref();
                        return;
                    }
                    AudioFileDialog.Unref();
                };
                Gtk.Internal.FileDialog.Open(AudioFileDialog.Handle, Handle, IntPtr.Zero, FileDialogCallback, IntPtr.Zero);
            }
            else // FileChooserNative is used instead if 4.8 or lower
            {
                AudioFileChooser = Gtk.FileChooserNative.New(
                    "Select Audio File to Play",
                    this,
                    FileChooserAction.Open,
                    "Open",
                    "Cancel"
                );

                AudioFileChooser.SetModal(true);

                AudioFileChooser.AddFilter(AcceptableAudioFiles);

                AudioFileChooser.OnResponse += (sender, e) =>
                {
                    if (e.ResponseId != (int)ResponseType.Accept)
                    {
                        AudioFileChooser.Unref();
                        return;
                    }

                    var path = AudioFileChooser.GetFile()!.GetPath() ?? "";
                    LoadAudio(path);
                    AudioFileChooser.Unref();
                };
                AudioFileChooser.Show();
            }
        }

        /// <summary>
        /// (Try to) Load an audio file.  Will set the GUI.!--
        ///
        /// Only will change the state of the app if the loaded file is different.
        /// </summary>
        /// <param name="path">Audio file to load</param>
        private void LoadAudio(string path)
        {
            // Check for samsies
            if (CurrentFilePath == path)
                return;

            // New!
            UnloadAudio();  // Clean out the old

            // Load the new
            LoadedFilename = System.IO.Path.GetFileName(path);
            string newStatusText;
            try
            {
                Audio = new Sound(path);
                Audio.Volume = Volume;
                CurrentFilePath = path;
                newStatusText = $"Loaded {LoadedFilename}";
            }
            catch (SndFileException ex)
            {
                newStatusText = $"Error decoding {LoadedFilename}";
                Debug.WriteLine($"loadAudio() SndFileException: {ex.Message}");
                Console.WriteLine($"loadAudio() SndFileException: {ex.Message}");
            }
            catch (PortAudioException ex)
            {
                newStatusText = $"Error setting up stream for {LoadedFilename}";
                Debug.WriteLine($"loadAudio() PortAudioException: {ex.Message}");
                Console.WriteLine($"loadAudio() PortAudioException: {ex.Message}");
            }

            // Reset GUI
            EnablePlaybackWidgets(Audio != null);

            PlayButton.Active = false; // Reset play button
            PlaybackSlider.Adjustment.Value = 0; // Reset to beginning
            StatusLabel.SetText(newStatusText); // Sets the text of the status label
            AudioFileDialogButtonLabel.SetLabel(LoadedFilename); // Sets the AudioFileDialogButtonLabel to the file name
        }

        /// <summary>
        /// Unloads any current audio file
        /// </summary>
        private void UnloadAudio()
        {
            if (Audio != null)
            {
                Audio.Pause();
                Audio.Dispose();
                Audio = null;
            }
        }

        /// <summary>
        /// Set if the playback widgets (play, rewind, &amp; scrub) should be clickable or not
        /// </summary>
        /// <param name="enable"></param>
        private void EnablePlaybackWidgets(bool enable)
        {
            PlayButton.Sensitive = enable;
            RewindButton.Sensitive = enable;
            PlaybackSlider.Sensitive = enable;
        }

        #region Widget callbacks
        /// <summary>
        /// Activated when the window is closed.  End the application
        /// </summary>
        private bool Window_OnCloseRequest(object sender, EventArgs a)
        {
            UnloadAudio();
            App.Quit();
            return true;
        }

        /// <summary>
        /// Tripped when the volume slider (off to the right) is changed.  Adjusts playback
        /// volume.
        /// </summary>
        private void OnVolumeSliderChanged(object sender, EventArgs args)
        {
            VolumeLevelLabel.SetText($"{VolumeSlider.Adjustment.Value.ToString("0")}%");
            Volume = (float)VolumeSlider.Adjustment.Value / (float)VolumeSlider.Adjustment.Upper;

            // Adjust volume of currently playing sound
            if (Audio != null)
                Audio.Volume = Volume;
        }

        /// <summary>
        /// Activated when the file chooser dialog reports a file has been selected.
        ///
        /// Will (try to) load an audio file.
        /// </summary>
        private void OnAudioFileChoosen(object sender, EventArgs args) =>
            LoadAudio(CurrentFilePath);

        /// <summary>
        /// If the rewind button was clicked, restart playback
        /// <param name="sender"></param>
        private void OnRewindClicked(object sender, EventArgs args)
        {
            if (Audio != null)
            {
                Audio.Cursor = 0;       // Reset audio to the start
                PlaybackSlider.Adjustment.Value = 0;  // Reset playback slider to 0
            }
        }

        /// <summary>
        /// Activated when the play/pause button has been clicked/toggled.!--
        ///
        /// Will either play or pause any audio that is paused or is playing (respectively).
        /// </summary>
        private void OnPlayToggled(object sender, EventArgs args)
        {
            // play/pause audio
            if (Audio != null)
            {
                string statusText;

                // Do we play or pause?
                if (PlayButton.Active)
                {
                    Audio.Play();
                    PlayButton.SetIconName("media-playback-pause-symbolic");
                    statusText = $"Playing {LoadedFilename}";
                    GLib.Functions.TimeoutAdd(0, 50, new GLib.SourceFunc(CheckPlayback));        // 20x a second, nice and smooth
                }
                else
                {
                    Audio.Pause();
                    PlayButton.SetIconName("media-playback-start-symbolic");
                    statusText = $"Paused {LoadedFilename}";
                }

                // Update GUI
                StatusLabel.SetText(statusText);
            }
        }

        /// <summary>
        /// When the playback slider's value has been changed (either programatically or via the user),
        /// this will update the label to display a time code.
        /// </summary>
        private void OnPlaybackSliderValueChanged(object sender, EventArgs args)
        {
            // Function will be called upon app startup, so do this to prevent a segfault (until a file is loaded)
            if (Audio == null)
            {
                PlaybackTimeLabel.SetText("00:00:00");
                return;
            }

            // Map the slider's value to a time
            System.TimeSpan curTime = System.TimeSpan.FromSeconds(PlaybackSliderSeconds());
            PlaybackTimeLabel.SetText(curTime.ToString(@"hh\:mm\:ss"));
        }

        /// <summary>
        /// If the playback slider has been dragged (by the user), this will adjust the playback where the audio is playing from.
        /// </summary>
        private void OnPlaybackSliderDragged(object sender, EventArgs args) =>
            MovingPlaybackSlider = true;

        /// <summary>
        /// If the playback slider value has been changed (by the user), this will adjust the playback where the audio is playing from.
        /// </summary>
        private bool OnPlaybackSliderChangeValue(object sender, EventArgs args)
        {
            MovingPlaybackSlider = true;

            return false;
        }

        /// <summary>
        /// If the playback slider has been dragged by the user, this will set the audio (playing or not) to that location.
        /// </summary>
        private void OnPlaybackSliderMouseUp(object sender, EventArgs args)
        {
            // We need an audio file to work with
            if (Audio == null)
                return;

            // First, we check if slider is moving
            if (MovingPlaybackSlider)
                Audio.Cursor = (float)PlaybackSliderSeconds(); // Then we set the cursor

            MovingPlaybackSlider = false;
        }
        #endregion // Widget callbacks

        /// <summary>
        /// This function runs in the Gtk GUI thread, as a timeout (about 20 times a second)
        /// </summary>
        /// <returns>if `true`, this function will run again.  if `false`, then it won't, until
        /// it has been kicked off by `Glib.Timeout()`</returns>
        private bool CheckPlayback()
        {
            // We need to have an audio file
            if (Audio == null)
                return false;

            if (Audio.IsPlaying)
            {
                if (!MovingPlaybackSlider)
                {
                    double playbackValue = Map(Audio.Cursor, 0, Audio.Duration.TotalSeconds, PlaybackSlider.Adjustment.Lower, PlaybackSlider.Adjustment.Upper);
                    PlaybackSlider.Adjustment.Value = playbackValue;
                }

                return true;
            }

            // If not, don't do anything
            PlayButton.Active = false;      // Un-toggle play
            return false;
        }

        /// <summary>
        /// Get the value of the playback slider, in seconds.
        /// </summary>
        /// <returns>a non-negative number, with a max value of the length of the current audio file</returns>
        private double PlaybackSliderSeconds() =>
            Map(PlaybackSlider.Adjustment.Value, PlaybackSlider.Adjustment.Lower, PlaybackSlider.Adjustment.Upper, 0, Audio.Duration.TotalSeconds);

        /// <summary>
        /// Map a value (x) from one range [a, b] to a new range [p, q]
        /// </summary>
        /// <param name="x">value to map</param>
        /// <param name="a">old min</param>
        /// <param name="b">old max</param>
        /// <param name="p">new min</param>
        /// <param name="q">new max</param>
        /// <returns></returns>
        private static double Map(double x, double a, double b, double p, double q) =>
            (x - a) / (b - a) * (q - p) + p;
    }
}
