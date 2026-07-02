using ARZVPRewrite.Core;
using ARZVPRewrite.Core.Localisation;
using ARZVPRewrite.Core.Templates;
using ARZVPRewrite.Models;
using ARZVPRewrite.UI;
using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        public static ScriptConfig Config { get; set; }

        /// <summary>
        /// The entry point method.
        /// </summary>
        /// <param name="veg">The VEGAS Pro instance.</param>
        public void FromVegas(Vegas veg)
        {
            try
            {
                Config = ScriptConfig.Load();
                Globals.Vegas = veg;

                TemplateManager.Templates.Clear();
                LanguageManager.Load(Config.LanguageCode);
                TemplateManager.LoadFromDisk();

                var events = veg.Project.Tracks
                    .SelectMany(t => t.Events)
                    .Where(e => e != null &&
                                e.ActiveTake != null &&
                                e.ActiveTake.Media != null &&
                                e.IsVideo() &&
                                e.Selected &&
                                e.ActiveTake.Media.HasAudio() &&
                                e.ActiveTake.Media.HasVideo())
                    .ToList();

                if (events.Count == 0)
                {
                    MessageBox.Show(LanguageManager.GetString("error.nomedia"), LanguageManager.GetString("error.title"),
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Globals.SelectedEvent = events.First() as VideoEvent;

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
            }
        }
    }
}
