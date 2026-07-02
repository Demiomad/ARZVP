using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ARZVPRewrite.Extensions
{
    public static class TextBoxExtensions
    {
        public static void Log(this TextBox box, string text)
        {
            box.Dispatcher.Invoke(() =>
            {
                box.AppendText(text + Environment.NewLine);
                box.ScrollToEnd();
            });
        }
    }
}
