using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace MakePdf.Core.Document
{
    class Pdf : DocumentBase
    {
        public Pdf(string fullpath, ILogger logger) : base(fullpath, logger)
        {
        }

        public override void Dispose()
        {
        }
    }
}
