using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.IO;

using Microsoft.Extensions.Logging;

namespace SimpleUpdater.Updates
{
    abstract public class UpdateManager
    {
        public string AppName { get; set; }
        public string AppInfoFileName { get; set; } = "AppInfo.json";

        protected ILogger logger;

        protected bool preRelease = false;

        public UpdateManager(ILogger logger)
        {
            this.logger = logger;

            AppName = Assembly.GetExecutingAssembly().GetName().Name;
        }

        abstract public Task<AppInfo> CheckForUpdateAsync();
        abstract public Task<AppInfo> PrepareForUpdate(string outputDir, string zipFileName);

        /// <summary>
        /// Prepare for update.
        /// Please call ReserveForUpdate method and close the application after this method.
        /// </summary>
        /// <param name="inputPath"></param>
        /// <param name="outputPath"></param>
        /// <returns></returns>
        abstract public Task<AppInfo> PrepareForUpdate(string outputPath);

        /// <summary>
        /// Update the application.
        /// This method implements the update after the application closes.
        /// The application will start up after the update is completed.
        /// </summary>
        /// <param name="pid"></param>
        /// <param name="srcDir"></param>
        /// <param name="dstDir"></param>
        public void Update(string pid, string srcDir, string dstDir)
        {
            // Wait for the target application to finish...
            Process.GetProcessById(Convert.ToInt32(pid)).WaitForExit();

            // Start the updates.
            var currentAppInfo = AppInfo.ReadFile($@"{dstDir}\{AppInfoFileName}");
            var newAppInfo = AppInfo.ReadFile($@"{srcDir}\{AppInfoFileName}");

            // Delete file in current dir.
            foreach (var file in currentAppInfo.Files)
            {
                File.Delete($@"{dstDir}\{file.Name}");
            }
            File.Delete($@"{srcDir}\{AppInfoFileName}");

            // Copy file to current dir fron new dir.
            foreach (var file in newAppInfo.Files)
            {
                File.Copy($@"{srcDir}\{file.Name}", $@"{dstDir}\{file.Name}");
            }
            File.Copy($@"{srcDir}\{AppInfoFileName}", $@"{dstDir}\{AppInfoFileName}");
        }
    }
}
