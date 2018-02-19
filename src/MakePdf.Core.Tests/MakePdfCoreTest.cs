using System;
using System.Collections.Generic;
using System.IO;

using Xunit;

namespace MakePdf.Core.Tests
{
    public class MakePdfCoreFixture : IDisposable
    {
        public string OutputFile { get; } = @"TestData\MakePdfCoreTest.pdf";

        // Setup
        public MakePdfCoreFixture()
        {
        }

        // Teardown
        public void Dispose()
        {
            File.Delete(OutputFile);
        }
    }

    public class MakePdfCoreTest : IClassFixture<MakePdfCoreFixture>
    {
        MakePdfCoreFixture fixture;

        public MakePdfCoreTest(MakePdfCoreFixture fixture)
        {
            this.fixture = fixture;
        }

        [Fact]
        void RunTest()
        {
            var core = new MakePdfCore(null);

            var files = new List<string>()
            {
                Path.GetFullPath(@"TestData\word1.docx"),
                Path.GetFullPath(@"TestData\word2.doc"),
                Path.GetFullPath(@"TestData\pdf1.pdf"),
            };

            // Setting
            core.Setting.AddFilenameToBookmark.IsEnabled = true;
            //core.Setting.AddFilenameToBookmark.Exclude = "MakePdfTest1.*";
            core.Setting.ReplaceFileName.IsEnabled = false;
            core.Setting.ReplaceFileName.Before = "MakePdf(.*)\\..*";
            core.Setting.ReplaceFileName.After = "$1";

            core.Setting.Property.Title = "Title Test";

            core.RunAsync(fixture.OutputFile, files).Wait();

            Assert.True(File.Exists(fixture.OutputFile));
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
            var core = new MakePdfCore(null);
            Assert.Equal(expected, core.IsSupported(ext));
        }
    }
}