using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MakePdf.Core.Documents
{
    class Pdf : DocumentBase
    {
        Document doc;
        PdfCopy copy;
        FileStream stream;

        public Pdf(string fullpath, ILogger logger) : base(fullpath, logger)
        {
            //using (var doc = new Document())
            //using (var stream = new FileStream(fullpath, FileMode.Create))
            //{
            //    // ドキュメントにファイルストリームを割り当てる?（ここらへんがいまいちピンとこない）
            //    using (var copy = new PdfCopy(doc, stream))
            //    {
            //        doc.Open();
            //    }
            //}

            // ドキュメントを閉じる
            //copy.Close();
            //doc.Close();

            doc = new Document();
            stream = new FileStream(fullpath, FileMode.Create);
            copy = new PdfCopy(doc, stream);

            doc.Open();
        }

        public void Combine(string filepath)
        {

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

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            copy.Dispose();
            stream.Dispose();
            doc.Dispose();

            disposed = true;
        }
    }
}
