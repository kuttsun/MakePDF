using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using CoreUpdater;

using MakePdf.Wpf.Models.Settings;

namespace MakePdf.Wpf.Models
{
    public class Updater
    {
        public bool UpdateCompleted { get; private set; } = false;
        public bool UpdateSuccessful { get; private set; } = true;

        MyInformation myInfo = MyInformation.Instance;

        UpdateManager mgr;
        readonly ILogger logger;
        readonly AppSetting appSetting;

        public Updater(IOptions<AppSetting> appSetting, ILogger<Updater> logger)
        {
            this.logger = logger;
            this.appSetting = appSetting.Value;

            mgr = new GitHub(this.appSetting.GitHubUrl, logger: logger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> CheckForUpdates()
        {
            try
            {
                var appInfo = await mgr.CheckForUpdatesAsync();

                var version1 = new Version(myInfo.AssemblyInformationalVersion);
                var version2 = new Version(appInfo.Version);

                if (version1 < version2)
                {
                    // New version found
                    return appInfo.Version;
                }
                return null;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return null;
            }
        }

        public async Task<bool> PrepareForUpdates()
        {
            try
            {
                var appInfo = await mgr.PrepareForUpdatesAsync(Directory.GetCurrentDirectory(), myInfo.Name + ".zip");

                var srcDir = Path.GetFullPath(appInfo.GetNewVersionDir());
                var dstDir = Path.GetFullPath(Directory.GetCurrentDirectory());

                // Start new version application
                mgr.StartUpdater();
                logger?.LogInformation("Start updater");

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public void Update(string[] args) => mgr.Update(args);
        public void RestartApplication(string[] args) => mgr.RestartApplication(args);
        public bool CanUpdate(string[] args) => mgr.CanUpdate(args);

        public void Completed(string[] args)
        {
            switch (mgr.Completed(args))
            {
                case UpdateCompletedType.Success:
                    UpdateCompleted = true;
                    UpdateSuccessful = true;
                    break;
                case UpdateCompletedType.Failure:
                    UpdateCompleted = true;
                    UpdateSuccessful = false;
                    break;
            }
        }
    }
}
