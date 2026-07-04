using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARZVPRewrite.Core.FFmpeg
{
    /// <summary>
    /// Utilities for downloading/getting FFmpeg.
    /// </summary>
    public static class FFmpeg
    {
        private static string _ffmpegPath;

        /// <summary>
        /// DEtermines if FFmpeg is installed on the user's system.
        /// </summary>
        public static bool IsFFmpegInstalled()
            => !string.IsNullOrEmpty(FindFFmpegInPath()) || !string.IsNullOrEmpty(_ffmpegPath);

        private static string FindFFmpegInPath()
        {
            var env = Environment.GetEnvironmentVariable("PATH");
            if (env == null)
                return string.Empty;

            var parts = env.Split(Path.PathSeparator);
            foreach (var part in parts)
            {
                var fullPath = Path.Combine(part, "ffmpeg.exe");
                if (File.Exists(fullPath))
                    return fullPath;
            }
        }
    }
}
