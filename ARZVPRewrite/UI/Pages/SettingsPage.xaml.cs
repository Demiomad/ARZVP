using ARZVPRewrite.Core;
using ARZVPRewrite.Core.Localisation;
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

namespace ARZVPRewrite.UI.Pages
{
    /// <summary>
    /// Interaction logic for SettingsPage.xaml
    /// </summary>
    public partial class SettingsPage : UserControl
    {
        public SettingsPage()
        {
            InitializeComponent();
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            LanguageBox.Items.Clear();

            var files = Directory.GetFiles(ScriptPaths.LanguagesPath, "*.json");
            foreach (var file in files)
            {
                var lang = Core.Localisation.Language.GetFromJson(file);
                LanguageBox.Items.Add(lang);
            }

            VlcPathBox.Text = EntryPoint.Config.VlcPath;
        }

        private void ApplyBtnClicked(object sender, RoutedEventArgs e)
        {
            var lang = LanguageBox.SelectedItem;
            if (lang == null)
                return;

            EntryPoint.Config.VlcPath = VlcPathBox.Text;
            EntryPoint.Config.LanguageCode = ((Language)lang).Code;
        }

        private void BrowseVlcClicked(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    VlcPathBox.Text = dialog.SelectedPath;
            }
        }
    }
}
