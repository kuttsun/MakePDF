using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;
using System.IO;

namespace MakePdf.Wpf
{
    class MyInformation
    {
        public static MyInformation Instance { get; } = new MyInformation();

        /// <summary>
        /// MakePdf
        /// </summary>
        public string Name { get; }
        /// <summary>
        /// MakePdf.exe
        /// </summary>
        public string AssemblyName { get; }
        /// <summary>
        /// e.g. 1.0.0.xxxx
        /// </summary>
        public string AssemblyVersion { get; }
        /// <summary>
        /// == AssemblyVersion
        /// </summary>
        public string AssemblyFileVersion { get; }
        /// <summary>
        /// e.g. 1.0.0
        /// </summary>
        public string AssemblyInformationalVersion { get; }

        MyInformation()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            Name = assembly.GetName().Name;
            AssemblyName = Path.GetFileName(Assembly.GetExecutingAssembly().Location);
            AssemblyVersion = assembly.GetName().Version.ToString();
            AssemblyFileVersion = fvi.FileVersion;
            AssemblyInformationalVersion = fvi.ProductVersion;
        }
    }
}
