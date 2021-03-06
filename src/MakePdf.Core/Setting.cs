﻿using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace MakePdf.Core
{
    public enum PageLayout
    {
        SinglePage,
        OneColumn,
    }

    public enum CreateBookmarkFromWord
    {
        Heading,
        Bookmark,
        None,
    }

    public class Setting
    {
        public bool DeleteConvertedPdf { get; set; } = true;
        public bool ConvertOnly { get; set; } = false;
        public Target TargetFiles { get; set; } = new Target();
        public Target TargetDirectories { get; set; } = new Target();
        public ReplacePattern ReplaceFileName { get; set; } = new ReplacePattern();
        public ReplacePattern ReplaceDirectoryName { get; set; } = new ReplacePattern();
        public AddToBookmark AddFileNameToBookmark { get; set; } = new AddToBookmark();
        public AddToBookmark AddDirectoryNameToBookmark { get; set; } = new AddToBookmark();
        public Property Property { get; set; } = new Property();
        public DisplayPdf DisplayPdf { get; set; } = new DisplayPdf();
        public WordSetting WordSetting { get; set; } = new WordSetting();
        public ExcelSetting ExcelSetting { get; set; } = new ExcelSetting();
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

    public class Property
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Creator { get; set; } // Application
        public string Subject { get; set; } // Subtitle
        public string Keywords { get; set; }
    }

    public class DisplayPdf
    {
        public bool PageModeUseOutlines { get; set; } = true;
        [JsonConverter(typeof(StringEnumConverter))]
        public PageLayout PageLayout { get; set; } = PageLayout.SinglePage;
    }
    public class WordSetting
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public CreateBookmarkFromWord CreateBookmarkFromWord { get; set; } = CreateBookmarkFromWord.Heading;
        public string ExclusionPattern { get; set; } = null;
    }

    public class ExcelSetting
    {
        public bool AddSheetNameToBookmark { get; set; } = true;
    }

}
