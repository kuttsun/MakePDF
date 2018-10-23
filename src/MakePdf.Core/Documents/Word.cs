using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Word;

namespace MakePdf.Core.Documents
{
    class Word : DocumentBase
    {
        Application word;
        public WordSetting Setting { get; set; }

        public Word(string fullpath, ILogger logger) : base(fullpath, logger)
        {
            word = new Application();
            Setting = new WordSetting();
        }

        ~Word()
        {
            Dispose(false);
        }

        public override void ToPdf()
        {
            Document doc = null;

            logger?.LogInformation($"Start conversion from Word to PDF : {Path.GetFileName(fullpath)}");

            try
            {
                doc = word.Documents.OpenNoRepairDialog(fullpath);

                // Updating the table of contents
                // https://www.c-sharpcorner.com/article/word-automation-using-C-Sharp/
                if (doc.TablesOfContents.Count > 0)
                {
                    doc.TablesOfContents[1].Update();
                    doc.TablesOfContents[1].UpdatePageNumbers();
                }
                doc.Fields.Update();

                // Update All Fields
                // https://superuser.com/questions/196703/how-do-i-update-all-fields-in-a-word-document
                //foreach (Section section in doc.Sections)
                //{
                //    doc.Fields.Update();  // update each section

                //    HeadersFooters headers = section.Headers;  //Get all headers
                //    foreach (HeaderFooter header in headers)
                //    {
                //        Fields fields = header.Range.Fields;
                //        foreach (Field field in fields)
                //        {
                //            field.Update();  // update all fields in headers
                //        }
                //    }

                //    HeadersFooters footers = section.Footers;  //Get all footers
                //    foreach (HeaderFooter footer in footers)
                //    {
                //        Fields fields = footer.Range.Fields;
                //        foreach (Field field in fields)
                //        {
                //            field.Update();  //update all fields in footers
                //        }
                //    }
                //}

                // refs: https://msdn.microsoft.com/library/microsoft.office.tools.word.document.exportasfixedformat.aspx
                doc.ExportAsFixedFormat(
                    OutputFullpath,
                    WdExportFormat.wdExportFormatPDF,
                    false,
                    WdExportOptimizeFor.wdExportOptimizeForPrint,
                    WdExportRange.wdExportAllDocument,
                    0,
                    0,
                    WdExportItem.wdExportDocumentContent,
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
                doc?.Close(WdSaveOptions.wdDoNotSaveChanges);
            }
        }

        WdExportCreateBookmarks GetBookmarkCreation(string fullpath)
        {
            WdExportCreateBookmarks ret;

            switch (Setting.CreateBookmarkFromWord)
            {
                case CreateBookmarkFromWord.Heading:
                    if ((Setting.ExclusionPattern != null) && Regex.IsMatch(Path.GetFileName(fullpath), Setting.ExclusionPattern))
                    {
                        ret = WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                    }
                    else
                    {
                        ret = WdExportCreateBookmarks.wdExportCreateHeadingBookmarks;
                    }
                    break;
                case CreateBookmarkFromWord.Bookmark:
                    if ((Setting.ExclusionPattern != null) && Regex.IsMatch(Path.GetFileName(fullpath), Setting.ExclusionPattern))
                    {
                        ret = WdExportCreateBookmarks.wdExportCreateHeadingBookmarks;
                    }
                    else
                    {
                        ret = WdExportCreateBookmarks.wdExportCreateWordBookmarks;
                    }
                    break;
                default:
                    ret = WdExportCreateBookmarks.wdExportCreateNoBookmarks;
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
