using ARZVPRewrite.Core;
using ARZVPRewrite.Core.Localisation;
using ARZVPRewrite.Core.Templates;
using ARZVPRewrite.Extensions;
using ARZVPRewrite.Models.Template;
using LibVLCSharp.Shared;
using ScriptPortal.Vegas;
using LibVLCSharp.WPF;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ARZVPRewrite.UI.Pages
{
    /// <summary>
    /// Interaction logic for VideoScrubDialog.xaml
    /// </summary>
    public partial class VideoScrubDialog : Window
    {
        private Timecode _dur;
        private TimeSpan _durSpan;
        private LibVLC _vlc;
        private LibVLCSharp.Shared.MediaPlayer _player;
        private LibVLCSharp.Shared.Media _media;
        private bool _isPlaying = false;
        private DispatcherTimer _timer;
        private bool _scrubbing;
        public Timecode SelectedOffset { get; set; }

        public VideoScrubDialog()
        {
            InitializeComponent();
        }

        private void SetTimeText(TimeSpan start, TimeSpan dur)
        {
            TimeText.Text = $"{start:mm\\:ss\\:ff}/{dur:mm\\:ss\\:ff}";
        }

        private void SeekToScrubberPos()
        {
            var secs = ScrubSlider.Value;
            var span = TimeSpan.FromSeconds(secs);
            _player.Time = (long)span.TotalMilliseconds;
            SetTimeText(span, _durSpan);
        }

        private void Play()
        {
            if (_isPlaying)
                return;

            PlayPauseIcon.Kind = Material.Icons.MaterialIconKind.Pause;
            _player.Play();
            _isPlaying = true;
            _timer.Start();
        }

        private void Pause()
        {
            if (!_isPlaying)
                return;

            PlayPauseIcon.Kind = Material.Icons.MaterialIconKind.Play;
            _player.Pause();
            _isPlaying = false;
            _timer.Stop();

            ScrubSlider.Value = _player.Time / 1000.0;
            SeekToScrubberPos();
        }

        private void Stop()
        {
            if (!_isPlaying)
                return;

            _player.Stop();
            PlayPauseIcon.Kind = Material.Icons.MaterialIconKind.Play;
            _isPlaying = false;
            _timer.Stop();

            ScrubSlider.Value = 0;
            SeekToScrubberPos();
        }

        private async void ScrubDialogLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(EntryPoint.Config.VlcPath) || !Directory.Exists(EntryPoint.Config.VlcPath))
                {
                    MessageBox.Show(LanguageManager.GetString("error.vlcmissing"),
                        LanguageManager.GetString("error.title"), MessageBoxButton.OK, MessageBoxImage.Error);

                    return;
                }

                LibVLCSharp.Shared.Core.Initialize(EntryPoint.Config.VlcPath);
                _vlc = new LibVLC();
                _player = new LibVLCSharp.Shared.MediaPlayer(_vlc);

                _timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(250)
                };

                _timer.Tick += (_, __) =>
                {
                    if (_isPlaying && !_scrubbing)
                    {
                        ScrubSlider.Value = _player.Time / 1000.0;
                        var secs = ScrubSlider.Value;
                        var span = TimeSpan.FromSeconds(secs);
                        SetTimeText(span, _durSpan);
                    }
                };


                Preview.MediaPlayer = _player;

                _media = new LibVLCSharp.Shared.Media(_vlc, Globals.SelectedVideo);
                _player.Media = _media;

                await _media.Parse();
                _media.ParsedChanged += MediaParseStatusChanged;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void MediaParseStatusChanged(object sender, MediaParsedChangedEventArgs e)
        {
            if (e.ParsedStatus == MediaParsedStatus.Done)
            {
                _dur = Timecode.FromMilliseconds(_media.Duration);
                _durSpan = _dur.ToTimeSpan();

                ScrubSlider.Maximum = _durSpan.TotalSeconds;
                SetTimeText(TimeSpan.Zero, _durSpan);
            }
        }

        private void ScrubSliderValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_scrubbing)
                SeekToScrubberPos();
        }

        private void StopBtnClicked(object sender, RoutedEventArgs e)
        {
            Stop();
        }

        private void PlayPauseBtnClicked(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
                Pause();
            else
                Play();
        }

        private void OkBtnClicked(object sender, RoutedEventArgs e)
        {
            SelectedOffset = TimeSpan.FromSeconds(ScrubSlider.Value).ToTimecode();
            DialogResult = true;
            Close();
        }

        private void ScrubDialogClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _player?.Dispose();
            _vlc?.Dispose();
            _media?.Dispose();
            _media.ParsedChanged -= MediaParseStatusChanged;
        }

        private void ScrubSliderPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _scrubbing = true;
            if (_isPlaying) Pause();
        }

        private void ScrubSliderPreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _scrubbing = false;
        }
    }
}
