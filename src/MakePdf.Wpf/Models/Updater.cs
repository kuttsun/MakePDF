using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.IO;

using SimpleUpdater;
using SimpleUpdater.Updates;

namespace MakePdf.Wpf.Models
{
    public class Updater
    {
        public static Updater Instance { get; } = new Updater();

        string assemblyVersion;
        string assemblyFileVersion;
        string assemblyInformationalVersion;

        UpdateManager mgr = new GitHub("https://github.com/kuttsun/MakePdf");

        private Updater()
        {
            var assembly = Assembly.GetExecutingAssembly();

            //Exeの場所を表示

            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            assemblyVersion = assembly.GetName().Version.ToString();
            assemblyFileVersion = fvi.FileVersion;
            assemblyInformationalVersion = fvi.ProductVersion;
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

                var version1 = new Version(assemblyInformationalVersion);
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
                var appInfo = await mgr.PrepareForUpdate(Directory.GetCurrentDirectory(), "MakePdf.zip");

                // Start new version application
                Process.Start("MakePdf.exe", $"update --pid={Process.GetCurrentProcess().Id} -n=MakePdf.exe -s={Path.GetFullPath(appInfo.GetNewVersionDir())} -d={Path.GetFullPath(Directory.GetCurrentDirectory())}");

                return true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                return false;
            }
        }

        public void Update(string pid, string appName, string srcDir, string dstDir)
        {
            mgr.Update(pid, appName, srcDir, dstDir);
        }
    }
}
