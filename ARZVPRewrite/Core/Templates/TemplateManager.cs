using ARZVPRewrite.Core.Localisation;
using ARZVPRewrite.Extensions;
using ARZVPRewrite.Models.REAPER;
using ARZVPRewrite.Models.Template;
using AutoRedZoneVP.Core.FFMpeg;
using Newtonsoft.Json;
using ScriptPortal.Vegas;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace ARZVPRewrite.Core.Templates
{
    public class TemplateProgressChangedEventArgs
    {
        /// <summary>
        /// Gets or sets the status message.
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        public int Percentage { get; set; }
    }

    /// <summary>
    /// Represents the template manager.
    /// </summary>
    public static class TemplateManager
    {
        public delegate void TemplateProgressChangedEventHandler(TemplateProgressChangedEventArgs e);

        /// <summary>
        /// Gets or sets the list of templates.
        /// </summary>
        public static List<Template> Templates { get; set; } = new List<Template>();

        /// <summary>
        /// Raised whenever the template progress changes.
        /// </summary>
        public static event TemplateProgressChangedEventHandler TemplateProgressChanged;

        /// <summary>
        /// Loads all templates from disk.
        /// </summary>
        public static void LoadFromDisk()
        {
            var dirs = Directory.GetDirectories(ScriptPaths.TemplatesPath);

            foreach (var dir in dirs)
            {
                try
                {
                    var templateJson = Path.Combine(dir, "template.json");
                    if (!File.Exists(templateJson))
                        continue;

                    var contents = File.ReadAllText(templateJson);
                    var template = JsonConvert.DeserializeObject<Template>(contents);

                    template.DirectoryPath = dir;
                    Templates.Add(template);
                }
                catch
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Applies the template to the output files.
        /// </summary>
        public static void Apply(Template template, string outputPath, string input, Timecode offset = null, TextBox logBox = null)
        {
            if (Directory.Exists(outputPath))
                Directory.Delete(outputPath, true);

            if (offset == null)
                offset = Timecode.FromSeconds(1);

            TemplateProgressChanged?.Invoke(new TemplateProgressChangedEventArgs() { Percentage = 10, StatusMessage = LanguageManager.GetString("status.creating") });
            Directory.CreateDirectory(outputPath);

            input = input.Trim().Trim('"');

            const string SampleItemName = "Sample";
            const string SynthItemName = "Synth";

            var outputVeg = Path.Combine(outputPath, "output.veg");
            var outputRpp = Path.Combine(outputPath, "output.rpp");
            var convertedWav = Path.Combine(outputPath, "converted.wav");
            var synthWav = Path.Combine(outputPath, "synth.wav");
            var outputResources = Path.Combine(outputPath, "resources");
            var sampleWav = Path.Combine(outputPath, "sample.wav");

            Directory.CreateDirectory(outputResources);

            logBox?.Log($"[Converter] Converting {Path.GetFileName(input)} to WAV");
            TemplateProgressChanged?.Invoke(new TemplateProgressChangedEventArgs() { Percentage = 20, StatusMessage = LanguageManager.GetString("status.converting") });
            Converter.ConvertToWav(input, convertedWav);

            logBox?.Log($"[SampleExtractor] Extracting sample from {Path.GetFileName(convertedWav)}");
            TemplateProgressChanged?.Invoke(new TemplateProgressChangedEventArgs() { Percentage = 30, StatusMessage = LanguageManager.GetString("status.extracting") });
            SampleExtractor.Extract(convertedWav, sampleWav, offset);

            logBox?.Log($"[SynthMaker] Creating synth from {Path.GetFileName(convertedWav)}");
            TemplateProgressChanged?.Invoke(new TemplateProgressChangedEventArgs() { Percentage = 40, StatusMessage = LanguageManager.GetString("status.synth") });
            SynthMaker.Create(convertedWav, synthWav, offset);

            logBox?.Log("[Resources] Copying resources");
            TemplateProgressChanged?.Invoke(new TemplateProgressChangedEventArgs() { Percentage = 45, StatusMessage = LanguageManager.GetString("status.copying") });
            foreach (var resource in template.Files.Resources)
            {
                try
                {
                    var resourcePath = Path.Combine(template.DirectoryPath, "resources", resource);
                    if (!File.Exists(resourcePath))
                        return;

                    var outputResourcePath = Path.Combine(outputResources, Path.GetFileName(resource));
                    File.Copy(resourcePath, outputResourcePath, true);
                }
                catch
                {
                    continue;
                }
            }

            TemplateProgressChanged?.Invoke(new TemplateProgressChangedEventArgs() { Percentage = 50, StatusMessage = LanguageManager.GetString("status.rpp") });

            logBox?.Log("[RPP] Creating output RPP");
            var inputVeg = Path.Combine(template.DirectoryPath, template.Files.VegasProjectFile);
            var inputRpp = Path.Combine(template.DirectoryPath, template.Files.ReaperProjectFile);

            var reaper = new ReaperProject(inputRpp);
            var root = reaper.Nodes.First();

            var tracks = root.Children.Where(n => n.Name == "TRACK");
            
            foreach (var track in tracks)
            {
                var items = track.Children.Where(n => n.Name == "ITEM");
                foreach (var item in items)
                {
                    var nameProp = item.GetPropertyString("NAME");
                    var sourceNode = item.GetNode("SOURCE");

                    if (!string.IsNullOrEmpty(nameProp))
                    {
                        if (nameProp == SampleItemName)
                            sourceNode?.SetProperty("FILE", $"\"{sampleWav}\"");
                        else if (nameProp == SynthItemName)
                            sourceNode?.SetProperty("FILE", $"\"{synthWav}\"");
                    }
                }
            }
            File.WriteAllText(outputRpp, reaper.ToString());

            TemplateProgressChanged?.Invoke(new TemplateProgressChangedEventArgs() { Percentage = 75, StatusMessage = LanguageManager.GetString("status.veg") });

            logBox?.Log("[VEG] Creating output VEG");
            using (var project = Globals.Vegas.ReadProject(inputVeg))
            {
                var media = project.MediaPool.AddMedia(input);
                var stream = media.Streams.FirstOrDefault(s => s is VideoStream);
                if (stream == null)
                {
                    return;
                }

                using (var undoBlock = new UndoBlock(project, $"ARZVP v2 - Apply Template"))
                {
                    var revSubclip = new Subclip(project, media.FilePath,
                        Timecode.FromSeconds(0), media.Length, true, $"{Path.GetFileNameWithoutExtension(input)} (reversed)");

                    foreach (var track in project.Tracks)
                    {
                        foreach (var ev in track.Events)
                        {
                            if (ev is VideoEvent video)
                            {
                                bool isVideo = !ev.IsAdjustmentEvent && !ev.ActiveTake.IsGenerator;

                                if (isVideo)
                                {
                                    var evMedia = video.ActiveTake.Media;
                                    var path = video.ActiveTake.MediaPath;

                                    if (Path.GetFileNameWithoutExtension(path) == "PLACEHOLDER")
                                    {
                                        logBox?.Log($"[VEG] Replacing event {ev.Index}");
                                        var take = Take.CreateInstance(project, stream);

                                        ev.Takes.Clear();
                                        ev.Takes.Add(take);

                                        var rate = video.PlaybackRate;
                                        logBox?.Log($"Playback rate: {rate}");

                                        var takeOffset = Timecode.FromMilliseconds(offset.ToMilliseconds() / rate);
                                        logBox?.Log($"takeOffset={takeOffset.ToMilliseconds()}ms");

                                        take.Offset = takeOffset;
                                    }
                                }
                            }
                        }
                    }

                    project.MediaPool.RemoveUnusedMedia();
                }

                project.SaveProject(outputVeg);
            }

            logBox?.Log($"[Cleanup] Removing {Path.GetFileName(convertedWav)}");
            TemplateProgressChanged?.Invoke(new TemplateProgressChangedEventArgs() { Percentage = 90, StatusMessage = LanguageManager.GetString("status.cleanup") });
            File.Delete(convertedWav);

            TemplateProgressChanged?.Invoke(new TemplateProgressChangedEventArgs() { Percentage = 100, StatusMessage = LanguageManager.GetString("status.finish", outputPath) });
            Process.Start("explorer.exe", outputPath);
        }
    }
}
