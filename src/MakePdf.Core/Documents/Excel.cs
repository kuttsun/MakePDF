﻿using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;
using Microsoft.Office.Interop.Excel;

namespace MakePdf.Core.Documents
{
    class Excel : DocumentBase
    {

        public Excel(string fullpath, ILogger logger) : base(fullpath, logger)
        {
        }

        public override void ToPdf()
        {

        }

        public override void Dispose()
        {
        }
    }
}