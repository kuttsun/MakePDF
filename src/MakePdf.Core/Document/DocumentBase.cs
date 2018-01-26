using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace MakePdf.Core.Document
{
    abstract class DocumentBase
    {
        protected ILogger logger;

        protected DocumentBase(ILogger logger)
        {
            this.logger = logger;
        }

        public virtual void ToPdf() { }
    }
}
