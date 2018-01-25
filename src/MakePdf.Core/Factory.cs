using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using MakePdf.Core.Document;

namespace MakePdf.Core
{
    class Factory
    {
        public static DocumentBase Create(string filename)
        {
            switch (Path.GetExtension(filename))
            {
                case "doc":
                case "docx":
                    return new Word();
                case "xls":
                case "xlsx":
                    return new Excel();
                case "pdf":
                    return new Pdf();
                default:
                    return null;
            }
        }
    }
}
