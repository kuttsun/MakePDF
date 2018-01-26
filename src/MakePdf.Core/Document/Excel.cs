using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;

namespace MakePdf.Core.Document
{
    class Excel : DocumentBase
    {

        public Excel(ILogger logger) : base(logger)
        {
        }

        public override void ToPdf()
        {

        }
    }
}
