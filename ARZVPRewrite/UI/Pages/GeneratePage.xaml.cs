using ARZVPRewrite.Core;
using ARZVPRewrite.Core.Localisation;
using ARZVPRewrite.Core.Templates;
using ARZVPRewrite.Models.Template;
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
    /// Interaction logic for GeneratePage.xaml
    /// </summary>
    public partial class GeneratePage : UserControl
    {
        public GeneratePage()
        {
            InitializeComponent();
            SelectedTemplateLbl.Text = LanguageManager.GetString("page.templates.none");
        }

        private void GenerateButtonClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Globals.SelectedTemplate == null)
                {
                    MessageBox.Show(
                        LanguageManager.GetString("error.notselected"),
                        LanguageManager.GetString("error.title"),
                        MessageBoxButton.OK, MessageBoxImage.Error
                    );
                    return;
                }

                var outputPath = System.IO.Path.Combine(ScriptPaths.OutputPath, System.IO.Path.GetFileNameWithoutExtension(Globals.SelectedVideo));
                Directory.CreateDirectory(outputPath);

                LogBox.Clear();
                var dialog = new VideoScrubDialog();
                var result = dialog.ShowDialog();

                if (result == true)
                    TemplateManager.Apply(Globals.SelectedTemplate, outputPath,
                        Globals.SelectedVideo, dialog.SelectedOffset, LogBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), ex.Message, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void PageLoaded(object sender, RoutedEventArgs e)
        {
            TemplateManager.TemplateProgressChanged += TemplateProgressChanged;
            Globals.TemplateSelectionChanged += TemplateSelectionChanged;

            if (Globals.SelectedTemplate != null)
                SelectedTemplateLbl.Text = LanguageManager.GetString("page.templates.selected", Globals.SelectedTemplate.Metadata.Name);
        }

        private void TemplateSelectionChanged(Template selection)
        {
            SelectedTemplateLbl.Text = LanguageManager.GetString("page.templates.selected", selection.Metadata.Name);
        }

        private void TemplateProgressChanged(TemplateProgressChangedEventArgs e)
        {
            StatusProgress.Value = e.Percentage;
            StatusLbl.Text = e.StatusMessage;
        }
    }
}
