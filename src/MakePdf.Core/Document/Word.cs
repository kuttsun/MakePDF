using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;


using Microsoft.Extensions.Logging;
using MSWord = Microsoft.Office.Interop.Word;

namespace MakePdf.Core.Document
{
    class Word : DocumentBase
    {
        MSWord.Application word;

        public Word(string fullpath, ILogger logger) : base(fullpath, logger)
        {
            word = new MSWord.Application();
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

        public bool Close()
        {
            bool ret = false;

            if (word != null)
            {
                word.Quit();
                Marshal.ReleaseComObject(word);
                word = null;
            }

            return (ret);
        }
    }
}
