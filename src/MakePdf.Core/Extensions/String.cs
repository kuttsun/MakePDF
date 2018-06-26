using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MakePdf.Core.Extensions
{
    public static class String
    {
        /// <summary>
        /// Get the relative path to the file of arg2 seen from the directory of arg1
        /// </summary>
        /// <param name="path">Absolute path to the reference directory (end must end with \)</param>
        /// <param name="baseDir">Absolute path to the target file</param>
        /// <returns>Relative path to arg2 file as seen from the directory of arg1</returns>
        /// <example>
        /// <code>
        /// GetRelativePath(@"C:\Windows\System\", @"C:\Windows\file.txt")
        /// </code>
        /// ..\file.txt
        /// </example>
        public static string GetRelativePath(this string path, string baseDir)
        {
            var u1 = new Uri(Path.GetFullPath(baseDir));
            var u2 = new Uri(Path.GetFullPath(path));

            var relativeUri = u1.MakeRelativeUri(u2);

            var relativePath = relativeUri.ToString().Replace('/', '\\');

            return (relativePath);
        }
    }
}
