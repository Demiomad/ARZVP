using ARZVPRewrite.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Models
{
    /// <summary>
    /// Represents the script's configuration.
    /// </summary>
    public class ScriptConfig
    {
        /// <summary>
        /// Gets or sets the used language code.
        /// </summary>
        [JsonProperty("lang")]
        public string LanguageCode { get; set; } = "en";

        /// <summary>
        /// Saves the configuration to disk.
        /// </summary>
        public void Save()
        {
            try
            {
                var json = JsonConvert.SerializeObject(this);
                File.WriteAllText(ScriptPaths.ConfigFilePath, json);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Loads a configuration from a JSON file.
        /// </summary>
        public static ScriptConfig Load()
        {
            try
            {
                if (!File.Exists(ScriptPaths.ConfigFilePath))
                    return new ScriptConfig();

                var contents = File.ReadAllText(ScriptPaths.ConfigFilePath);
                var cfg = JsonConvert.DeserializeObject<ScriptConfig>(contents);
                return cfg ?? new ScriptConfig();
            }
            catch (JsonException)
            {
                return new ScriptConfig();
            }
            catch
            {
                throw;
            }
        }
    }
}
