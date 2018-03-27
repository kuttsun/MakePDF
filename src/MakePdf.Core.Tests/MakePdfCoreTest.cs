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
            var setting = new Setting();
            setting.AddFileNameToBookmark.IsEnabled = true;
            //core.Setting.AddFilenameToBookmark.Exclude = "MakePdfTest1.*";
            setting.ReplaceFileName.IsEnabled = false;
            setting.ReplaceFileName.Before = "MakePdf(.*)\\..*";
            setting.ReplaceFileName.After = "$1";
            setting.Property.Title = "Title Test";

            core.RunAsync(files, fixture.OutputFile, setting).Wait();

            Assert.True(File.Exists(fixture.OutputFile));
        }
    }
}