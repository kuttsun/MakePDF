using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MakePdf.Core
{
    public enum FileType
    {
        Pdf,
        Word,
        Excel
    }

    public static class Support
    {
        
        public static Dictionary<string, FileType> FileTypes { get; set; } = new Dictionary<string, FileType>()
        {
            {".pdf", FileType.Pdf },
            {".doc", FileType.Word },
            {".docx", FileType.Word },
            {".xls", FileType.Excel },
            {".xlsx", FileType.Excel },
        };

        public static bool IsSupported(string fullpath)
        {
            var ext = Path.GetExtension(fullpath);

            return FileTypes.ContainsKey(ext);
        }
    }
}
