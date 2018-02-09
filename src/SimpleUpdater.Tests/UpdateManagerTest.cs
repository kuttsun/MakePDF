using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

//using SimpleUpdater;

namespace SimpleUpdater.Tests
{
    public class UpdateManagerTest
    {
        [Fact]
        public void CheckForLatestVersion()
        {
            var mgr = new UpdateManager("https://github.com/kuttsun/Test");
            var appInfo = mgr.CheckForUpdateAsync().Result;
            Assert.NotNull(appInfo);
        }
    }
}
