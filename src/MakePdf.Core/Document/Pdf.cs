using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace MakePdf.Core.Document
{
    class Pdf : DocumentBase
    {
        public Pdf(ILogger logger) : base(logger)
        {
        }
    }
}
