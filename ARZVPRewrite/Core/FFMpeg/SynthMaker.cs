using ARZVPRewrite.Core.FFmpeg;
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
    /// Utilities for creating a synthesizer.
    /// </summary>
    public static class SynthMaker
    {
        /// <summary>
        /// Creates a synth from the input file.
        /// </summary>
        public static void Create(string input, string output, Timecode offset, TextBox logBox = null)
        {
            try
            {
                input = input.Trim().Trim('"');
                output = Path.ChangeExtension(output.Trim().Trim('"'), "wav");

                var dirName = Path.GetDirectoryName(output) ?? throw new NullReferenceException();
                var segmentPath = Path.Combine(dirName, "synth-sample.wav");

                SampleExtractor.Extract(input, segmentPath, offset, 0.00382, logBox);

                var psi = new ProcessStartInfo()
                {
                    FileName = FFmpeg.GetFFmpegPath(),
                    Arguments = $"-stream_loop -1 -i \"{segmentPath}\" -t 20 \"{output}\" -y -af loudnorm",
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

                    File.Delete(segmentPath);
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
