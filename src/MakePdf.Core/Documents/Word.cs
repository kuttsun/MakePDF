using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;


using Microsoft.Extensions.Logging;
using MsWord = Microsoft.Office.Interop.Word;

namespace MakePdf.Core.Documents
{
    public class Word : DocumentBase
    {
        MsWord.Application word;

        public Word(string fullpath, ILogger logger) : base(fullpath, logger)
        {
            word = new MsWord.Application();
        }

        ~Word()
        {
            Dispose(false);
        }

        public override void ToPdf()
        {
            MsWord.Document doc = null;

            try
            {
                doc = word.Documents.OpenNoRepairDialog(fullpath);

                // refs: https://msdn.microsoft.com/library/microsoft.office.tools.word.document.exportasfixedformat.aspx
                doc.ExportAsFixedFormat(
                    OutputFilename,
                    MsWord.WdExportFormat.wdExportFormatPDF,
                    false,
                    MsWord.WdExportOptimizeFor.wdExportOptimizeForPrint,
                    MsWord.WdExportRange.wdExportAllDocument,
                    0,
                    0,
                    MsWord.WdExportItem.wdExportDocumentContent,
                    true,
                    true,
                    MsWord.WdExportCreateBookmarks.wdExportCreateHeadingBookmarks,
                    false);
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
