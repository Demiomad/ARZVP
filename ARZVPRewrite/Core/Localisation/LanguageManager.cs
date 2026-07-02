using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Core.Localisation
{
    /// <summary>
    /// Represents the language manager.
    /// </summary>
    public static class LanguageManager
    {
        /// <summary>
        /// Gets or sets the current language.
        /// </summary>
        public static Language CurrentLanguage { get; set; }

        /// <summary>
        /// Loads a language from a JSON file.
        /// </summary>
        /// <param name="path">The target JSON file.</param>
        public static void LoadFromJson(string path)
        {
            CurrentLanguage = Language.GetFromJson(path);
        }

        /// <summary>
        /// Loads a language from its language code.
        /// </summary>
        public static void Load(string code = "en")
        {
            var path = Path.Combine(ScriptPaths.LanguagesPath, $"{code}.json");
            if (!File.Exists(path))
                return;

            LoadFromJson(path);
        }

        /// <summary>
        /// Gets a translated string using the provided <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The target key to get.</param>
        /// <returns>The translated string if found, <paramref name="key"/> otherwise.</returns>
        public static string GetString(string key)
            => CurrentLanguage.GetString(key);

        /// <summary>
        /// Gets a translated string using the provided <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The target key to get.</param>
        /// <param name="args">The arguments to provide to the string formatter.</param>
        /// <returns>The translated string if found, <paramref name="key"/> otherwise.</returns>
        public static string GetString(string key, params object[] args)
            => CurrentLanguage.GetString(key, args);
    }
}
