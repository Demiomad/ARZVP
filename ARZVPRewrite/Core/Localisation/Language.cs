using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Core.Localisation
{
    /// <summary>
    /// Represents a language.
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Gets or sets the language's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the language's code.
        /// </summary>
        [JsonIgnore]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the list of translated strings, and their keys.
        /// </summary>
        [JsonProperty("translations")]
        public Dictionary<string, string> Translations { get; set; }

        /// <summary>
        /// Gets a translated string using the provided <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The target key to get.</param>
        /// <returns>The translated string if found, <paramref name="key"/> otherwise.</returns>
        public string GetString(string key)
        {
            if (Translations.TryGetValue(key, out var translated))
                return translated;

            var match = Translations.Keys.FirstOrDefault(k => k.Equals(key, StringComparison.OrdinalIgnoreCase));

            return match != null ? Translations[match] : key;
        }

        /// <summary>
        /// Gets a translated string using the provided <paramref name="key"/>.
        /// </summary>
        /// <param name="key">The target key to get.</param>
        /// <param name="args">The arguments to provide to the string formatter.</param>
        /// <returns>The translated string if found, <paramref name="key"/> otherwise.</returns>
        public string GetString(string key, params object[] args)
        {
            var translated = GetString(key);

            try
            {
                return string.Format(translated, args);
            }
            catch (FormatException)
            {
                return translated;
            }
        }

        /// <summary>
        /// Gets a language from a JSON file.
        /// </summary>
        /// <param name="path">The path of the JSON file.</param>
        /// <returns>The language.</returns>
        /// <exception cref="FileNotFoundException">Thrown when the JSON file could not be found.</exception>
        public static Language GetFromJson(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException(null, path);

            try
            {
                var contents = File.ReadAllText(path);
                var lang = JsonConvert.DeserializeObject<Language>(contents);
                lang.Code = Path.GetFileNameWithoutExtension(path);

                return lang ?? throw new InvalidOperationException($"Failed to deserialise language from path {path}");
            }
            catch
            {
                throw;
            }
        }
    }
}
