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
        /// Please call ReserveForUpdate method and close the application after this method.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        abstract public Task<bool> PrepareForUpdate(string inputPath, string outputPath);

        public Task<bool> PrepareForUpdate(string inputPath)
        {
            return PrepareForUpdate(inputPath, inputPath);
        }

        /// <summary>
        /// Reserve for update.
        /// Call this method after PreparingForUpdate method, and close the application.
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
            Process.Start("dotnet", $"SimpleUpdater.dll update --pid={processId}");

            // Application restart required
        }

        /// <summary>
        /// Update the application.
        /// This method implements the update after the application closes.
        /// The application will start up after the update is completed.
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="targetAppName"></param>
        /// <param name="sourceDir"></param>
        public static void Update(string pid, string targetAppName, string sourceDir)
        {
            Console.WriteLine("Wait for the target application to finish...");
            Process.GetProcessById(Convert.ToInt32(pid)).WaitForExit();

            Console.WriteLine("Start the updates.");

            // 現在の AppInfo を元に、現在のバージョンのファイルを全て削除

            // sourceDir の AppInfo を元に、ファイルをコピー

        }
    }
}
