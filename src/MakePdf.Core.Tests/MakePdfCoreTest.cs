using System;
using System.Collections.Generic;

using Xunit;

namespace MakePdf.Core.Tests
{
    public class MakePdfCoreTest
    {
        [Fact]
        void RunTest()
        {
            var core = new MakePdfCore(null);

            var files = new List<string>()
            {
                @"C:\Users\13005\Desktop\test1.doc",
                @"C:\Users\13005\Desktop\test2.doc"
            };
            core.Run(@"C:\Users\13005\Desktop\hoge.pdf", files);
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