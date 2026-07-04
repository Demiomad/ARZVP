using ARZVPRewrite.Core;
using ARZVPRewrite.Core.FFmpeg;
using ARZVPRewrite.Core.Localisation;
using ARZVPRewrite.Core.Templates;
using ARZVPRewrite.Models;
using ARZVPRewrite.UI;
using Microsoft.Win32;
using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ARZVPRewrite
{
    /// <summary>
    /// Represents the script's entry point class.
    /// </summary>
    public class EntryPoint
    {
        /// <summary>
        /// Gets or sets the current configuration.
        /// </summary>
        public static ScriptConfig Config { get; set; }

        /// <summary>
        /// Gets or sets the HTTP client.
        /// </summary>
        public static HttpClient Http { get; set; }

        /// <summary>
        /// The entry point method.
        /// </summary>
        /// <param name="veg">The VEGAS Pro instance.</param>
        public async void FromVegas(Vegas veg)
        {
            try
            {
                Http = new HttpClient()
                {
                    Timeout = TimeSpan.FromSeconds(30)
                };
                await FFmpeg.GetFFmpeg();

                Config = ScriptConfig.Load();
                Globals.Vegas = veg;

                TemplateManager.Templates.Clear();
                LanguageManager.Load(Config.LanguageCode);
                TemplateManager.LoadFromDisk();

                var media = veg.Project.MediaPool.GetSelectedMedia().ToList();

                if (media.Count == 0)
                {
                    var dialog = new OpenFileDialog
                    {
                        Filter =
                            "Video Files (*.mp4;*.mov;*.avi;*.wmv;*.mxf;*.mpeg;*.mpg)|*.mp4;*.mov;*.avi;*.wmv;*.mxf;*.mpeg;*.mpg|" +
                            "All Files (*.*)|*.*",
                        Title = "Select Video File",
                        FilterIndex = 0
                    };

                    if (dialog.ShowDialog() == true)
                        Globals.SelectedVideo = dialog.FileName;
                }
                else
                    Globals.SelectedVideo = media.First().FilePath;

                if (string.IsNullOrEmpty(Globals.SelectedVideo))
                {
                    MessageBox.Show(LanguageManager.GetString("error.nomedia"),
                        LanguageManager.GetString("error.title"), MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var main = new MainWindow();
                main.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Config.Save();
                Http.Dispose();
            }
        }
    }
}
