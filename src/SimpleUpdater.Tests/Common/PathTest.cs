using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

using SimpleUpdater.Common;

namespace SimpleUpdater.Tests.Common
{
    public class PathTest
    {
        [Theory,
            InlineData(@"../file.txt", @"C:\Windows\System\", @"C:\Windows\file.txt")]
        public void GetRelativePathTest(string expected, string path1, string path2)
        {
            Assert.Equal(expected, Path.GetRelativePath(path1, path2));
        }
    }
}
