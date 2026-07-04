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
        private static string _dlUrl = "https://github.com/ffbinaries/ffbinaries-prebuilt/releases/download/v6.1/ffmpeg-6.1-win-64.zip";
        private static string _ffmpegPath;

        /// <summary>
        /// DEtermines if FFmpeg is in the user's PATH environment variable.
        /// </summary>
        public static bool IsFFmpegInPath()
            => !string.IsNullOrEmpty(FindFFmpegInPath());

        /// <summary>
        /// Retrieves the FFmpeg executable by either finding it in the PATH environment variable or by downloading a binary from the internet.
        /// </summary>
        public static async Task GetFFmpeg()
        {
            var path = ScriptPaths.GetToolPath("FFmpeg", "ffmpeg.exe");

            if (File.Exists(path))
            {
                _ffmpegPath = path;
                return;
            }

            if (IsFFmpegInPath())
            {
                _ffmpegPath = FindFFmpegInPath();
                return;
            }

            using (var response = await EntryPoint.Http.GetAsync(_dlUrl))
            {
                using (var fs = new FileStream(path, FileMode.CreateNew))
                {
                    await response.Content.CopyToAsync(fs);
                }
            }

            _ffmpegPath = path;
        }

        /// <summary>
        /// Returns the FFmpeg path.
        /// </summary>
        public static string GetFFmpegPath()
            => _ffmpegPath;

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

            return string.Empty;
        }
    }
}
