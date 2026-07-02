using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Models.Template
{
    /// <summary>
    /// Represents a template's metadata.
    /// </summary>
    public class TemplateMetadata
    {
        /// <summary>
        /// Gets or sets the template's name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = "Untitled";
        
        /// <summary>
        /// Gets or sets the template's description.
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; set; } = "No description provided.";

        /// <summary>
        /// Gets or sets the template's author.
        /// </summary>
        [JsonProperty("author")]
        public string Author { get; set; }
    }
}
