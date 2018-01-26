using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;


using Microsoft.Extensions.Logging;
using MSWord = Microsoft.Office.Interop.Word;

namespace MakePdf.Core.Documents
{
    public class Word : DocumentBase
    {
        MSWord.Application word;

        public Word(string fullpath, ILogger logger) : base(fullpath, logger)
        {
            word = new MSWord.Application();
        }

        ~Word()
        {
            Dispose(false);
        }

        public override void ToPdf()
        {
            var output = Path.ChangeExtension(fullpath, ".pdf");

            var doc = word.Documents.OpenNoRepairDialog(fullpath);

            // refs: https://msdn.microsoft.com/library/microsoft.office.tools.word.document.exportasfixedformat.aspx
            doc.ExportAsFixedFormat(
                output,
                MSWord.WdExportFormat.wdExportFormatPDF,
                false,
                MSWord.WdExportOptimizeFor.wdExportOptimizeForPrint,
                MSWord.WdExportRange.wdExportAllDocument,
                0,
                0,
                MSWord.WdExportItem.wdExportDocumentContent,
                true,
                true,
                MSWord.WdExportCreateBookmarks.wdExportCreateHeadingBookmarks,
                false);

            doc.Close(MSWord.WdSaveOptions.wdDoNotSaveChanges);
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
