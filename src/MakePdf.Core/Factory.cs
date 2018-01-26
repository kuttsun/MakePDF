using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;

using MakePdf.Core.Document;

namespace MakePdf.Core
{
    class Factory
    {
        public static DocumentBase Create(string filename, ILogger logger)
        {
            switch (Path.GetExtension(filename))
            {
                case "doc":
                case "docx":
                    return new Word(logger);
                case "xls":
                case "xlsx":
                    return new Excel(logger);
                case "pdf":
                    return new Pdf(logger);
                default:
                    return null;
            }
        }
    }
}
