using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace SimpleUpdater.UpdateManager
{
    abstract public class UpdateManager
    {
        protected ILogger logger;

        // e.g. AppInfo.json
        protected string appInfoName;
        protected bool preRelease = false;

        public UpdateManager(string appInfoName, ILogger logger)
        {
            this.logger = logger;
            this.appInfoName = appInfoName;
        }

        abstract public Task<AppInfo> CheckForUpdateAsync();

        abstract public Task<bool> PreparingForUpdate(string inputPath, string outputPath);

        public static void Update(int pid, string targetAppName, string sourceDir)
        {
            Console.WriteLine("Wait for the target application to finish...");
            Process.GetProcessById(pid).WaitForExit();

            Console.WriteLine("Start the remaining updates.");

            // 現在の AppInfo を元に、現在のバージョンのファイルを全て削除

            // sourceDir の AppInfo を元に、ファイルをコピー

        }
    }
}
