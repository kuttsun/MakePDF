using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


using Microsoft.Extensions.Logging;
using MsWord = Microsoft.Office.Interop.Word;

namespace MakePdf.Core.Documents
{
    class Word : DocumentBase
    {
        MsWord.Application word;
        public WordSetting Setting { get; set; }

        public Word(string fullpath, ILogger logger) : base(fullpath, logger)
        {
            word = new MsWord.Application();
            Setting = new WordSetting();
        }

        ~Word()
        {
            Dispose(false);
        }

        public override void ToPdf()
        {
            MsWord.Document doc = null;

            logger?.LogInformation($"Start conversion from Word to PDF : {Path.GetFileName(fullpath)}");

            try
            {
                doc = word.Documents.OpenNoRepairDialog(fullpath);

                // refs: https://msdn.microsoft.com/library/microsoft.office.tools.word.document.exportasfixedformat.aspx
                doc.ExportAsFixedFormat(
                    OutputFullpath,
                    MsWord.WdExportFormat.wdExportFormatPDF,
                    false,
                    MsWord.WdExportOptimizeFor.wdExportOptimizeForPrint,
                    MsWord.WdExportRange.wdExportAllDocument,
                    0,
                    0,
                    MsWord.WdExportItem.wdExportDocumentContent,
                    true,
                    true,
                    GetBookmarkCreation(fullpath),
                    false);

                logger?.LogInformation("Success");
            }
            catch (Exception e)
            {
                logger?.LogError("Error Occurred", e);
                throw;
            }
            finally
            {
                doc?.Close(MsWord.WdSaveOptions.wdDoNotSaveChanges);
            }
        }

        MsWord.WdExportCreateBookmarks GetBookmarkCreation(string fullpath)
        {
            MsWord.WdExportCreateBookmarks ret;

            switch (Setting.CreateBookmarkFromWord)
            {
                case CreateBookmarkFromWord.Heading:
                    if ((Setting.ExclusionPattern != null) && Regex.IsMatch(Path.GetFileName(fullpath), Setting.ExclusionPattern))
                    {
                        ret = MsWord.WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                    }
                    else
                    {
                        ret = MsWord.WdExportCreateBookmarks.wdExportCreateHeadingBookmarks;
                    }
                    break;
                case CreateBookmarkFromWord.Bookmark:
                    if ((Setting.ExclusionPattern != null) && Regex.IsMatch(Path.GetFileName(fullpath), Setting.ExclusionPattern))
                    {
                        ret = MsWord.WdExportCreateBookmarks.wdExportCreateHeadingBookmarks;
                    }
                    else
                    {
                        ret = MsWord.WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                    }
                    break;
                default:
                    ret = MsWord.WdExportCreateBookmarks.wdExportCreateNoBookmarks;
                    break;
            }

            return ret;
        }


        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public override void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            word.Quit();
            Marshal.ReleaseComObject(word);

            disposed = true;
        }
    }
}
