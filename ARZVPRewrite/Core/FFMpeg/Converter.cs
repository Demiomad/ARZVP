using ARZVPRewrite.Core.FFmpeg;
using ARZVPRewrite.Extensions;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace AutoRedZoneVP.Core.FFMpeg
{
    /// <summary>
    /// Represetns a converter.
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Converts the input file to a wave file.
        /// </summary>
        public static void ConvertToWav(string input, string output, TextBox logBox = null)
        {
            try
            {
                input = input.Trim().Trim('"');
                output = Path.ChangeExtension(output.Trim().Trim('"'), "wav");

                var psi = new ProcessStartInfo()
                {
                    FileName = FFmpeg.GetFFmpegPath(),
                    Arguments = $"-i \"{input}\" -acodec pcm_s16le -ar 44100 -ac 2 -y \"{output}\"",
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (var proc = new Process() { StartInfo = psi })
                {
                    proc.OutputDataReceived += (s, e) =>
                    {
                        if (string.IsNullOrEmpty(e.Data))
                            return;

                        logBox?.Log($"[FFmpeg] {e.Data}");
                    };

                    proc.ErrorDataReceived += (s, e) =>
                    {
                        if (string.IsNullOrEmpty(e.Data))
                            return;

                        logBox?.Log($"[FFmpeg Error] {e.Data}");
                    };

                    proc.Start();

                    proc.BeginOutputReadLine();
                    proc.BeginErrorReadLine();

                    if (!proc.WaitForExit(60000))
                    {
                        proc.Kill();
                        throw new Exception("FFmpeg timed out after 1 minute");
                    }

                    if (proc.ExitCode != 0)
                        throw new Exception($"FFmpeg failed (exit {proc.ExitCode})");

                    proc.Close();
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
