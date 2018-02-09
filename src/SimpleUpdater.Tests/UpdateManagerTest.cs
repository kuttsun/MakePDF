using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

//using SimpleUpdater;

namespace SimpleUpdater.Tests
{
    public class UpdateManagerTest
    {
        UpdateManager mgr;

        public UpdateManagerTest()
        {
            mgr = new UpdateManager("https://github.com/kuttsun/Test");
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
