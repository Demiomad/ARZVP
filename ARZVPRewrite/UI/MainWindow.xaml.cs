using ARZVPRewrite.UI.Pages;
using System;
using System.Collections.Generic;
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

namespace ARZVPRewrite.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private enum ARZVPPage
        {
            Home,
            Templates,
            Generate,
            Settings
        }

        private Dictionary<ARZVPPage, UserControl> _pageMap = new Dictionary<ARZVPPage, UserControl>()
        {
            { ARZVPPage.Home, new HomePage() },
            { ARZVPPage.Templates, new TemplatesPage() },
            { ARZVPPage.Generate, new GeneratePage() },
            { ARZVPPage.Settings, new SettingsPage() }
        };

        public MainWindow()
        {
            InitializeComponent();
        }

        private void HomeBtnClicked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _pageMap[ARZVPPage.Home];
        }

        private void TemplatesBtnClicked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _pageMap[ARZVPPage.Templates];
        }

        private void GenerateBtnClicked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _pageMap[ARZVPPage.Generate];
        }

        private void SettingsBtnClicked(object sender, RoutedEventArgs e)
        {
            MainContent.Content = _pageMap[ARZVPPage.Settings];
        }
    }
}
