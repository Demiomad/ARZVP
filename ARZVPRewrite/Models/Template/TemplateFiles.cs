using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Models.Template
{
    /// <summary>
    /// Represents a template's files.
    /// </summary>
    public class TemplateFiles
    {
        /// <summary>
        /// Gets or sets the path of the VEGAS project file.
        /// </summary>
        [JsonProperty("veg")]
        public string VegasProjectFile { get; set; }

        /// <summary>
        /// Gets or sets the path of the REAPER project file.
        /// </summary>
        [JsonProperty("rpp")]
        public string ReaperProjectFile { get; set; }

        /// <summary>
        /// Gets or sets the template's resources.
        /// </summary>
        [JsonProperty("resources")]
        public string[] Resources { get; set; } = Array.Empty<string>();
    }
}
