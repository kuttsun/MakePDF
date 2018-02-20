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

        string name;
        string assemblyName;
        string assemblyVersion;
        string assemblyFileVersion;
        string assemblyInformationalVersion;

        UpdateManager mgr = new GitHub("https://github.com/kuttsun/MakePdf");

        private Updater()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            name = assembly.GetName().Name;
            assemblyName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
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
                var appInfo = await mgr.PrepareForUpdate(Directory.GetCurrentDirectory(), name + ".zip");

                var srcDir = Path.GetFullPath(appInfo.GetNewVersionDir());
                var dstDir = Path.GetFullPath(Directory.GetCurrentDirectory());

                // Start new version application
                Process.Start($@"{srcDir}\{assemblyName}", $"update --pid={Process.GetCurrentProcess().Id} -n={assemblyName} -s={srcDir} -d={dstDir}");

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
