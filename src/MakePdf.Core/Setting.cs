using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MakePdf.Core
{
    public class Setting
    {
        public Target TargetFiles { get; set; } = new Target();
        public Target TargetDirectories { get; set; } = new Target();
        public ReplacePattern ReplaceFileName { get; set; } = new ReplacePattern();
        public ReplacePattern ReplaceDirectoryName { get; set; } = new ReplacePattern();
        public AddToBookmark AddFileNameToBookmark { get; set; } = new AddToBookmark();
        public AddToBookmark AddDirectoryNameToBookmark { get; set; } = new AddToBookmark();
        public WordSetting WordSetting { get; set; } = new WordSetting();
        public Property Property { get; set; } = new Property();
        public PageLayout PageLayout { get; set; } = new PageLayout();
        public bool CanDeletePdf { get; set; } = true;
        public bool ConvertOnly { get; set; } = false;
    }

    public class Target
    {
        public bool AllItems { get; set; } = true;
        public string Pattern { get; set; } = null;
    }

    public class ReplacePattern
    {
        public bool IsEnabled { get; set; } = false;
        public string Before { get; set; } = null;
        public string After { get; set; } = null;
    }

    public class AddToBookmark
    {
        public bool IsEnabled { get; set; } = true;
        public string ExclusionPattern { get; set; } = null;
    }

    public class WordSetting
    {
        public bool CreatingBookmarkFromHeading { get; set; } = true;
        public string ExclusionPattern { get; set; } = null;
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
