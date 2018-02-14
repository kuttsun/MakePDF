using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

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
        abstract public Task<AppInfo> PrepareForUpdate(string inputPath, string outputPath);

        public Task<AppInfo> PrepareForUpdate(string inputPath)
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
        public void ReserveForUpdate(int processId,string targetAppName,AppInfo appInfo )
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


            var currentAppInfo = AppInfo.ReadFile("AppInfo.json");
            var newAppInfo = AppInfo.ReadFile($@"{sourceDir}\AppInfo.json");

            // Delete file in current dir.
            foreach (var file in currentAppInfo.Files)
            {
                File.Delete(file.Name);
            }
            File.Delete("AppInfo.json");

            // Copy file to current dir fron new dir.
            foreach (var file in newAppInfo.Files)
            {
                File.Copy($@"{sourceDir}\{file.Name}", $"{file.Name}");
            }
            File.Copy($@"{sourceDir}\AppInfo.json", $"AppInfo.json");
        }

        protected static string GetNewVersionDir(AppInfo appInfo)
        {
            return $"{appInfo.Name}-{appInfo.Version}";
        }
    }
}
