using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Core
{
    /// <summary>
    /// Represents the script's paths.
    /// </summary>
    public static class ScriptPaths
    {
        /// <summary>
        /// Gets the base directory.
        /// </summary>
        public static readonly string BasePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ARZVPv2");

        /// <summary>
        /// Gets the directory where language translations are stored.
        /// </summary>
        public static readonly string LanguagesPath =
            Path.Combine(BasePath, "langs");

        /// <summary>
        /// Gets the directory where template are stored.
        /// </summary>
        public static readonly string TemplatesPath =
            Path.Combine(BasePath, "templates");

        /// <summary>
        /// Gets the directory where outputs are stored.
        /// </summary>
        public static readonly string OutputPath =
            Path.Combine(BasePath, "output");

        /// <summary>
        /// Gets the config file path.
        /// </summary>
        public static readonly string ConfigFilePath =
            Path.Combine(BasePath, "config.json");

        /// <summary>
        /// Gets the config file path.
        /// </summary>
        public static readonly string ToolsPath =
            Path.Combine(BasePath, "tools");

        /// <summary>
        /// Gets the path of a tool.
        /// </summary>
        public static string GetToolPath(string toolName, string executable)
        {
            var path = Path.Combine(ToolsPath, toolName);
            Directory.CreateDirectory(path);

            return Path.Combine(path, executable);
        }

        static ScriptPaths()
        {
            Directory.CreateDirectory(BasePath);
            Directory.CreateDirectory(LanguagesPath);
            Directory.CreateDirectory(TemplatesPath);
            Directory.CreateDirectory(OutputPath);
            Directory.CreateDirectory(ToolsPath);
        }
    }
}
