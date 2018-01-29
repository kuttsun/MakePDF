using System;
using System.Collections.Generic;
using System.Text;

namespace MakePdf.Core
{
    public class Setting
    {
        public ReplaceFileName ReplaceFileName { get; set; } = new ReplaceFileName();
        public AddFilenameToBookmark AddFilenameToBookmark { get; set; } = new AddFilenameToBookmark();
        public Property Property { get; set; } = new Property();
        public PageLayout PageLayout { get; set; } = new PageLayout();
        public bool CanDeletePdf { get; set; } = true;
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

    public class Property
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Creator { get; set; } // Application
        public string Subject { get; set; } // Subtitle
        public string Keywords { get; set; }
    }

    public class PageLayout
    {
        public bool PageModeUseOutlines { get; set; } = true;
        public bool SinglePage { get; set; } = true;
    }
}
