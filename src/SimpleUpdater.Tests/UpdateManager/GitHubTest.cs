using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

using SimpleUpdater.UpdateManager;

namespace SimpleUpdater.Tests
{
    public class GitHubTest
    {
        UpdateManager.UpdateManager mgr;

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
        public void UpdateFromZipTest()
        {
            Assert.True(mgr.UpdateFromZipAsync("test.zip").Result);
        }
    }
}
