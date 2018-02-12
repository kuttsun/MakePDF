using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace SimpleUpdater.Updates
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

        /// <summary>
        /// Prepare for update.
        /// Please call ReserveForUpdate method and restart the application after calling this method.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        abstract public Task<bool> PrepareForUpdate(string inputPath, string outputPath);

        /// <summary>
        /// Reserve for update.
        /// This method call after PreparingForUpdate method, and restart the application.
        /// </summary>
        /// <param name="processId"></param>
        /// <example> 
        /// <code>
        /// ReserveForUpdate(Process.GetCurrentProcess().Id)
        /// </code>
        /// </example>
        public void ReserveForUpdate(int processId)
        {
            // Start updater
            Process.Start("dotnet SimpleUpdater.dll", $"update --pid={processId}");

            // Application restart required
        }

        /// <summary>
        /// Update the application.
        /// This method call after restarting the application.
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="targetAppName"></param>
        /// <param name="sourceDir"></param>
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
