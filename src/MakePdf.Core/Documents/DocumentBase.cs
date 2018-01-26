using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace MakePdf.Core.Documents
{
    public abstract class DocumentBase : IDisposable
    {
        protected ILogger logger;
        protected string fullpath;

        protected DocumentBase(string fullpath, ILogger logger)
        {
            this.fullpath = fullpath;
            this.logger = logger;
        }

        public virtual void ToPdf() { }

        // https://msdn.microsoft.com/library/fs2xkftw.aspx
        public abstract void Dispose();
    }
}
