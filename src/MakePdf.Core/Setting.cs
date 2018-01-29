using System;
using System.Collections.Generic;
using System.Text;

namespace MakePdf.Core
{
    public class Setting
    {
        public ReplaceFileName ReplaceFileName { get; set; } = new ReplaceFileName();
        public AddFilenameToBookmark AddFilenameToBookmark { get; set; } = new AddFilenameToBookmark();
    }

    public class Bookmark
    {
        
    }

    public class ReplaceFileName
    {
        public bool IsEnabled { get; set; } = false;
        public string Before { get; set; } = null;
        public string After { get; set; } = null;
    }

    public class AddFilenameToBookmark
    {
        public bool IsEnabled { get; set; } = true;
        public string Exclude { get; set; } = null;
    }
}
