using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

using CoreUpdater;
using CoreUpdater.Updates;

namespace MakePdf.Wpf.Models
{
    public class Updater
    {
        public static Updater Instance { get; } = new Updater();
        MyInformation myInfo = MyInformation.Instance;

        UpdateManager mgr;
        ILogger logger;

        private Updater()
        {
            var loggerFactory = new LoggerFactory();
            loggerFactory.AddNLog(new NLogProviderOptions
            {
                CaptureMessageTemplates = true,
                CaptureMessageProperties = true
            });
            loggerFactory.ConfigureNLog("NLog.config");

            logger = loggerFactory.CreateLogger("logfile");

            mgr = new GitHub("https://github.com/kuttsun/MakePdf", logger);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public async Task<string> CheckForUpdate()
        {
            try
            {
                var appInfo = await mgr.CheckForUpdateAsync();

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

        public async Task<bool> PrepareForUpdate()
        {
            try
            {
                var appInfo = await mgr.PrepareForUpdate(Directory.GetCurrentDirectory(), myInfo.Name + ".zip");

                var srcDir = Path.GetFullPath(appInfo.GetNewVersionDir());
                var dstDir = Path.GetFullPath(Directory.GetCurrentDirectory());

                // Start new version application
                Process.Start($@"{srcDir}\{myInfo.AssemblyName}", $"update --pid={Process.GetCurrentProcess().Id} -n={myInfo.AssemblyName} -s={srcDir} -d={dstDir}");

                logger?.LogInformation($@"StartProcess: {srcDir}\{myInfo.AssemblyName} update --pid={Process.GetCurrentProcess().Id} -n={myInfo.AssemblyName} -s={srcDir} -d={dstDir}");

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public void Update(string pid, string srcDir, string dstDir)
        {
            mgr.Update(pid, srcDir, dstDir);
        }
    }
}
