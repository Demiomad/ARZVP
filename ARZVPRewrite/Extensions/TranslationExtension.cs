using ARZVPRewrite.Core.Localisation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace ARZVPRewrite.Extensions
{
    /// <summary>
    /// Repersents the translation markup extension.
    /// </summary>
    public class TranslationExtension : MarkupExtension
    {
        /// <summary>
        /// Gets or sets the target translation key.
        /// </summary>
        public string Key { get; set; }

        public TranslationExtension(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
                return Key;

            return LanguageManager.GetString(Key);
        }
    }
}
