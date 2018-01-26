using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Word;

namespace MakePdf.Core.Document
{
    class Word : DocumentBase
    {
        public Word(string fullpath, ILogger logger) : base(fullpath, logger)
        {
        }

        public override void ToPdf()
        {

        }
    }
}
