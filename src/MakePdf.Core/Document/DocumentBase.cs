using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace MakePdf.Core.Document
{
    abstract class DocumentBase
    {
        protected ILogger logger;
        protected string fullpath;

        protected DocumentBase(string fullpath, ILogger logger)
        {
            this.fullpath = fullpath;
            this.logger = logger;
        }

        public virtual void ToPdf() { }
    }
}
