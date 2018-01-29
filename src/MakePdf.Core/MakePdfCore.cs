using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

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

        public Setting Setting { get; set; } = new Setting();

        public MakePdfCore(ILogger logger)
        {
            this.logger = logger;
        }

        public void Run(string outputFullpath, IEnumerable<string> paths)
        {
            using (var outputPdf = new OutputPdf(outputFullpath, logger))
            {
                // Setting
                outputPdf.ReplaceFileName = Setting.ReplaceFileName;
                outputPdf.AddFilenameToBookmark = Setting.AddFilenameToBookmark;
                outputPdf.Property = Setting.Property;
                outputPdf.PageLayout = Setting.PageLayout;

                // Convert and combine
                foreach (var path in paths)
                {
                    if (File.Exists(path))
                    {
                        using (var doc = Create(path, logger))
                        {
                            doc.ToPdf();
                            outputPdf.Add(doc.OutputFullpath);
                            doc.DeleteOutputPdf(Setting.CanDeletePdf);
                        }
                    }
                    else if (Directory.Exists(path))
                    {

                    }
                }

                // Finalize
                outputPdf.Complete();
            }
        }

        public void Run()
        {

        }

        public static bool IsSupported(string fullpath)
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
