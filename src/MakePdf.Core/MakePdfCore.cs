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
        ILogger logger;
        OutputPdf outputPdf;

        Setting setting = new Setting();

        public MakePdfCore(ILogger<MakePdfCore> logger)
        {
            this.logger = logger;
        }

        public async Task<bool> RunAsync(IEnumerable<string> paths, string outputFullpath, Setting setting = null)
        {
            this.setting = setting ?? new Setting();

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

            return true;
        }

        public async Task<bool> RunAsync(string inputDirectory, string outputFullpath, Setting setting = null)
        {
            var paths = Directory.GetFileSystemEntries(inputDirectory);

            return await RunAsync(paths, $@"{inputDirectory}\{outputFullpath}", setting);
        }

        void ConvertAndCombine(IEnumerable<string> paths, List<Dictionary<string, object>> parentBookmarks = null)
        {
            foreach (var path in paths)
            {
                if (path == outputPdf.OutputFullpath)
                {
                    continue;
                }
                else if (File.Exists(path))
                {
                    if (setting.TargetFiles.AllItems || Regex.IsMatch(path, setting.TargetFiles.Pattern))
                    {
                        if (Support.IsSupported(path))
                        {
                            using (var doc = Create(path))
                            {
                                doc.ToPdf();
                                outputPdf?.Add(doc.OutputFullpath, parentBookmarks);
                                doc.DeleteCnvertedPdf(setting.DeleteConvertedPdf);
                            }

                        }

                    }
                }
                else if (Directory.Exists(path))
                {
                    var dirName = Path.GetFileName(path);

                    if (setting.TargetDirectories.AllItems || Regex.IsMatch(dirName, setting.TargetDirectories.Pattern))
                    {
                        var childBookmark = new List<Dictionary<string, object>>();
                        // Recursive processing
                        ConvertAndCombine(Directory.GetFileSystemEntries(path), childBookmark);
                        outputPdf.AddDirectoryBookmark(parentBookmarks, dirName, childBookmark);
                    }
                }
                else
                {
                    throw new FileNotFoundException();
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
    }
}
