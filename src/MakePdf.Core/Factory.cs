using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;

using MakePdf.Core.Documents;

namespace MakePdf.Core
{
    class Factory
    {
        public static DocumentBase Create(string fullpath, ILogger logger)
        {
            switch (Path.GetExtension(fullpath))
            {
                case "doc":
                case "docx":
                    return new Word(fullpath, logger);
                case "xls":
                case "xlsx":
                    return new Excel(fullpath, logger);
                case "pdf":
                    return new Pdf(fullpath, logger);
                default:
                    return null;
            }
        }
    }
}
