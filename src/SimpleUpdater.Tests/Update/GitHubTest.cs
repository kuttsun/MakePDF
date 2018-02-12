using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace SimpleUpdater.Updates.Tests
{
    public class GitHubTest
    {
        GitHub mgr;

        public GitHubTest()
        {
            mgr = new GitHub("https://github.com/kuttsun/Test");
        }

        [Fact]
        public void CheckForUpdateTest()
        {
            var appInfo = mgr.CheckForUpdateAsync().Result;
            Assert.NotNull(appInfo);
        }

        [Fact]
        public void PrepareForUpdateTest()
        {
            Assert.True(mgr.PrepareForUpdate("test.zip").Result);
        }
    }
}
