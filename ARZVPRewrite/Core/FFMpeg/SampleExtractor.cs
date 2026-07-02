using ARZVPRewrite.Extensions;
using ScriptPortal.Vegas;
using System;
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
    /// Utilities for extracting a segment.
    /// </summary>
    public static class SampleExtractor
    {
        /// <summary>
        /// Extracts a segment from the input file.
        /// </summary>
        public static void Extract(string input, string output, Timecode offset, TextBox logBox = null)
        {
            try
            {
                Extract(input, output, offset, 1, logBox);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Extracts a segment from the input file.
        /// </summary>
        public static void Extract(string input, string output, Timecode offset, double duration, TextBox logBox = null)
        {
            try
            {
                input = input.Trim().Trim('"');
                output = Path.ChangeExtension(output.Trim().Trim('"'), "wav");

                var psi = new ProcessStartInfo()
                {
                    FileName = "ffmpeg",
                    Arguments = $"-i \"{input}\" -ss {offset.ToMilliseconds() / 1000} -t {duration} -acodec pcm_s16le -ar 44100 -ac 2 \"{output}\" -y",
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
