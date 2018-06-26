using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Logging;

using MakePdf.Core.Documents;

namespace MakePdf.Core
{
    public class MakePdfCore
    {
        readonly ILogger logger;
        OutputPdf outputPdf;

        Setting setting = new Setting();
        public event Action<string> Subscriber = delegate { };// Null Object

        public bool IsProcessing { get; private set; } = false;

        public MakePdfCore(ILogger<MakePdfCore> logger)
        {
            this.logger = logger;
        }

        public async Task<bool> RunAsync(IEnumerable<string> paths, string outputFullpath, Setting setting)
        {
            this.setting = setting ?? new Setting();

            IsProcessing = true;

            try
            {
                await Task.Run(() =>
                {
                    using (outputPdf = new OutputPdf(outputFullpath, null))
                    {
                        outputPdf.SetSettings(this.setting);

                        ConvertAndCombine(paths);

                        // Finalize
                        outputPdf.Complete();
                    }

                });
            }
            finally
            {
                IsProcessing = false;
            }

            return true;
        }

        public async Task<bool> RunAsync(string inputDirectory, string outputFullpath, Setting setting)
        {
            var paths = Directory.GetFileSystemEntries(inputDirectory);

            return await RunAsync(paths, outputFullpath, setting);
        }

        void ConvertAndCombine(IEnumerable<string> paths, List<Dictionary<string, object>> parentBookmarks = null)
        {
            foreach (var path in paths)
            {
                OutputInfo($"{Path.GetFileName(path)}");
                try
                {
                    if (path == outputPdf.OutputFullpath)
                    {
                        OutputInfo("Ignore (it is output file)");
                        continue;
                    }
                    else if (File.Exists(path))
                    {
                        if (setting.TargetFiles.AllItems ||
                            (setting.TargetFiles.Pattern == null ? false : Regex.IsMatch(path, setting.TargetFiles.Pattern)))
                        {
                            if (Support.IsSupported(path))
                            {
                                OutputInfo("Start processing");
                                using (var doc = Create(path))
                                {
                                    doc.ToPdf();
                                    outputPdf?.Add(doc.OutputFullpath, parentBookmarks);
                                    doc.DeleteCnvertedPdf(setting.DeleteConvertedPdf);
                                }
                                OutputInfo("Complete");
                            }
                            else
                            {
                                OutputInfo("Ignore (it is not supported)");
                            }
                        }
                        else
                        {
                            OutputInfo("Ignore (it is not a target file)");
                        }
                    }
                    else if (Directory.Exists(path))
                    {
                        var dirName = Path.GetFileName(path);

                        if (setting.TargetDirectories.AllItems ||
                            (setting.TargetDirectories.Pattern == null ? false : Regex.IsMatch(dirName, setting.TargetDirectories.Pattern)))
                        {
                            var childBookmark = new List<Dictionary<string, object>>();
                            OutputInfo("Add directory name to bookmark");
                            outputPdf.AddDirectoryBookmark(parentBookmarks, dirName, childBookmark);
                            // Recursive processing
                            ConvertAndCombine(Directory.GetFileSystemEntries(path), childBookmark);
                        }
                        else
                        {
                            OutputInfo("Ignore (it is not a target directory)");
                        }
                    }
                    else
                    {
                        throw new FileNotFoundException();
                    }
                }
                catch (Exception e)
                {
                    Subscriber(e.Message);
                    logger?.LogError(e, e.Message);
                }
            }
        }

        IDocument Create(string fullpath)
        {
            var ext = Path.GetExtension(fullpath);

            if (Support.FileTypes.ContainsKey(ext))
            {
                switch (Support.FileTypes[ext])
                {
                    case FileType.Pdf:
                        return new Pdf(fullpath, logger);
                    case FileType.Word:
                        return new Word(fullpath, logger);
                    case FileType.Excel:
                        return new Excel(fullpath, logger) { Setting = setting.ExcelSetting };
                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        void OutputInfo(string msg)
        {
            Subscriber(msg);
            logger?.LogInformation(msg);
        }
    }
}
