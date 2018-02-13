using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleUpdater.Common
{
    public class Path
    {
        /// <summary>
        /// Get the relative path to the file of arg2 seen from the directory of arg1
        /// </summary>
        /// <param name="uri1">Absolute path to the reference directory (end must end with \)</param>
        /// <param name="uri2">Absolute path to the target file</param>
        /// <returns>Relative path to arg2 file as seen from the directory of arg1</returns>
        /// <example>
        /// <code>
        /// GetRelativePath(@"C:\Windows\System\", @"C:\Windows\file.txt")
        /// </code>
        /// ..\file.txt
        /// </example>
        public static string GetRelativePath(string uri1, string uri2)
        {
            Uri u1 = new Uri(uri1);
            Uri u2 = new Uri(uri2);

            Uri relativeUri = u1.MakeRelativeUri(u2);

            string relativePath = relativeUri.ToString();

            relativePath.Replace('/', '\\');

            return (relativePath);
        }
    }
}
