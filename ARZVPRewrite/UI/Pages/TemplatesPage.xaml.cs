using ARZVPRewrite.Core;
using ARZVPRewrite.Core.Localisation;
using ARZVPRewrite.Core.Templates;
using ARZVPRewrite.Models.Template;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static ScriptPortal.Vegas.SummaryProperties;

namespace ARZVPRewrite.UI.Pages
{
    /// <summary>
    /// Interaction logic for TemplatesPage.xaml
    /// </summary>
    public partial class TemplatesPage : UserControl
    {
        public TemplatesPage()
        {
            InitializeComponent();
            RefreshTemplates();

            SelectedTemplateLbl.Text = LanguageManager.GetString("page.templates.none");
        }

        private void RefreshTemplates()
        {
            TemplateBox.Items.Clear();

            foreach (var template in TemplateManager.Templates)
                TemplateBox.Items.Add(template);

            TemplateCountLbl.Text = LanguageManager.GetString("page.templates.count", TemplateManager.Templates.Count);
        }

        private void InfoBtnClicked(object sender, RoutedEventArgs e)
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

            var tooltip = new ToolTip()
            {
                Content = new StackPanel
                {
                    Orientation = Orientation.Vertical,
                    Children =
                    {
                        new TextBlock()
                        {
                            Text = LanguageManager.GetString("tooltip.template.nameauthor",
                                Globals.SelectedTemplate.Metadata.Name,
                                Globals.SelectedTemplate.Metadata.Author),
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(5)
                        },
                        new TextBlock()
                        {
                            Text = Globals.SelectedTemplate.Metadata.Description,
                            Margin = new Thickness(5)
                        },
                        new TextBlock()
                        {
                            Text = $"VEG: {Globals.SelectedTemplate.Files.VegasProjectFile}",
                            Margin = new Thickness(5, 10, 5, 0)
                        },
                        new TextBlock()
                        {
                            Text = $"RPP: {Globals.SelectedTemplate.Files.ReaperProjectFile}",
                            Margin = new Thickness(5)
                        },
                        new TextBlock()
                        {
                            Text = $"Resources: {string.Join(", ", Globals.SelectedTemplate.Files.Resources)}",
                            Margin = new Thickness(5)
                        }
                    }
                },
                IsOpen = true,
                Placement = PlacementMode.Mouse
            };

            var timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(1.5),
                IsEnabled = true
            };

            timer.Tick += (_, __) =>
            {
                tooltip.IsOpen = false;
                timer.Stop();
            };

            InfoBtn.ToolTip = tooltip;
        }

        private void TemplateBoxSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Globals.SelectedTemplate = (Template)TemplateBox.SelectedItem;
            SelectedTemplateLbl.Text = LanguageManager.GetString("page.templates.selected", Globals.SelectedTemplate.Metadata.Name);
        }
    }
}
