using System;
using System.Collections.Generic;
using System.IO;

using Xunit;

namespace MakePdf.Core.Tests
{
    public class MakePdfCoreTest
    {
        [Fact(Skip = "Because it contains I/O")]
        void RunTest()
        {
            var core = new MakePdfCore(null);

            var files = new List<string>()
            {
                Path.GetFullPath("MakePdfTest1.doc"),
                Path.GetFullPath("MakePdfTest2.doc"),
            };

            // Setting
            core.Setting.AddFilenameToBookmark.IsEnabled = true;
            //core.Setting.AddFilenameToBookmark.Exclude = "MakePdfTest1.*";
            core.Setting.ReplaceFileName.IsEnabled = false;
            core.Setting.ReplaceFileName.Before = "MakePdf(.*)\\..*";
            core.Setting.ReplaceFileName.After = "$1";

            core.Setting.Property.Title = "Title Test";

            core.Run(@"MakePdfOutput.pdf", files);
        }

        [Theory,
            InlineData(true, ".pdf"),
            InlineData(true, ".doc"),
            InlineData(true, ".docx"),
            InlineData(true, ".xls"),
            InlineData(true, ".xlsx"),
            InlineData(false, ""),
            InlineData(false, ".txt")]
        void IsSupportedTest(bool expected, string ext)
        {
            Assert.Equal(expected, MakePdfCore.IsSupported(ext));
        }
    }
}