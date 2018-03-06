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
    enum SupportFileType
    {
        Pdf,
        Word,
        Excel
    }

    public class MakePdfCore
    {
        ILogger logger;

        static Dictionary<string, SupportFileType> SupportFileTypes { get; set; } = new Dictionary<string, SupportFileType>()
        {
            {".pdf", SupportFileType.Pdf },
            {".doc", SupportFileType.Word },
            {".docx", SupportFileType.Word },
            {".xls", SupportFileType.Excel },
            {".xlsx", SupportFileType.Excel },
        };

        Setting setting = new Setting();

        public MakePdfCore(ILogger logger)
        {
            this.logger = logger;
        }

        public async Task<bool> RunAsync(IEnumerable<string> paths, string outputFullpath, Setting setting = null)
        {
            this.setting = setting ?? new Setting();

            await Task.Run(() =>
            {
                using (var outputPdf = new OutputPdf(outputFullpath, logger))
                {
                    outputPdf.SetSettings(setting);

                    ConvertAndCombine(outputPdf, paths);

                    // Finalize
                    outputPdf.Complete();
                }
            });

            return true;
        }

        public async Task<bool> RunAsync(string inputDirectory, string outputFullpath, Setting setting = null)
        {
            var paths = Directory.GetFiles(inputDirectory, "*", SearchOption.AllDirectories);

            return await RunAsync(paths, $@"{inputDirectory}\{outputFullpath}", setting);
        }

        void ConvertAndCombine(OutputPdf outputPdf, IEnumerable<string> paths)
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
                        if (IsSupported(path))
                        {
                            using (var doc = Create(path, logger))
                            {
                                doc.ToPdf();
                                outputPdf?.Add(doc.OutputFullpath);
                                doc.DeleteCnvertedPdf(setting.DeleteConvertedPdf);
                            }
                        }
                    }
                }
                else if (Directory.Exists(path))
                {
                    if (setting.TargetDirectories.AllItems || Regex.IsMatch(path, setting.TargetDirectories.Pattern))
                    {
                        // Recursive processing
                        ConvertAndCombine(outputPdf, Directory.GetFiles(path, " * ", SearchOption.AllDirectories));
                    }
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }
        }

        public bool IsSupported(string fullpath)
        {
            var ext = Path.GetExtension(fullpath);

            return SupportFileTypes.ContainsKey(ext);
        }

        DocumentBase Create(string fullpath, ILogger logger)
        {
            var ext = Path.GetExtension(fullpath);

            if (SupportFileTypes.ContainsKey(ext))
            {
                switch (SupportFileTypes[ext])
                {
                    case SupportFileType.Pdf:
                        return new Pdf(fullpath, logger);
                    case SupportFileType.Word:
                        return new Word(fullpath, logger);
                    case SupportFileType.Excel:
                        return new Excel(fullpath, logger);
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
