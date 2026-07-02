using ARZVPRewrite.Models.Template;
using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ARZVPRewrite.Core
{
    /// <summary>
    /// Global variables for later use.
    /// </summary>
    public static class Globals
    {
        private static Template _selected;
        /// <summary>
        /// Gets or sets the current VEGAS Pro instance.
        /// </summary>
        public static Vegas Vegas { get; set; }

        /// <summary>
        /// Gets or sets the currently selected template.
        /// </summary>
        public static Template SelectedTemplate
        {
            get => _selected;
            set
            {
                _selected = value;
                TemplateSelectionChanged?.Invoke(value);
            }
        }
        
        /// <summary>
        /// Gets or sets the selected video event.
        /// </summary>
        public static VideoEvent SelectedEvent { get; set; }

        public delegate void TemplateSelectionChangedEventListener(Template template);

        /// <summary>
        /// Raised whenever <see cref="SelectedTemplate"/> is changed.
        /// </summary>
        public static event TemplateSelectionChangedEventListener TemplateSelectionChanged;
    }
}
