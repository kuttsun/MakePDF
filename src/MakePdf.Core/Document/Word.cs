using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Word;

namespace MakePdf.Core.Document
{
    class Word : DocumentBase
    {
        public Word(ILogger logger) : base(logger)
        {
        }

        public override void ToPdf()
        {

        }
    }
}
