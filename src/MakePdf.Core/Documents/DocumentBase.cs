﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;

namespace MakePdf.Core.Documents
{
    public abstract class DocumentBase : IDocument
    {
        protected ILogger logger;
        protected string fullpath;
        protected string filename;
        public string OutputFullpath { get; set; }


        protected DocumentBase(string fullpath, ILogger logger)
        {
            this.logger = logger;
            this.fullpath = fullpath;
            filename = Path.GetFileName(fullpath);
            OutputFullpath = Path.ChangeExtension(fullpath, ".pdf");
        }

        public virtual void ToPdf() { }

        public void DeleteCnvertedPdf(bool canDeletePdf)
        {
            if (!(this is Pdf) && canDeletePdf)
            {
                File.Delete(OutputFullpath);
            }
        }

        // https://msdn.microsoft.com/library/fs2xkftw.aspx
        public abstract void Dispose();
    }
}
