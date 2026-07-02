using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Models.Template
{
    /// <summary>
    /// Represents a template.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// Gets or sets the template's metadata.
        /// </summary>
        [JsonProperty("metadata")]
        public TemplateMetadata Metadata { get; set; }

        /// <summary>
        /// Gets or sets the template's files.
        /// </summary>
        [JsonProperty("files")]
        public TemplateFiles Files { get; set; }

        /// <summary>
        /// Gets or sets the directory where the template is stored.
        /// </summary>
        [JsonIgnore]
        public string DirectoryPath { get; set; }

        public override string ToString()
            => $"{Metadata.Name} ({Metadata.Author}) - {Metadata.Description}";
    }
}
