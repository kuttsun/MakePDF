using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

using MakePdf.Core.Extensions;

namespace MakePdf.Core.Tests
{
    public class StringTest
    {
        [Theory,
            InlineData(@"..\file.txt", @"C:\Windows\file.txt", @"C:\Windows\System\"),
            InlineData(@"file.txt", @"C:\Windows\file.txt", @"C:\Windows\System"),
            InlineData(@"System\file.txt", @"C:\Windows\System\file.txt", @"C:\Windows\")]
        void GetRelativePathTest(string expected, string path, string baseDir)
        {
            Assert.Equal(expected, path.GetRelativePath(baseDir));
        }
    }
}
