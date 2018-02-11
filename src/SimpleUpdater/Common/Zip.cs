using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

using Microsoft.Extensions.Logging;

namespace SimpleUpdater.Common
{
    public static class Zip
    {
        public static bool ExtractEntries(string zipFileName, string outputDir, ILogger logger = null)
        {
            if (Directory.Exists(outputDir) == false)
            {
                Directory.CreateDirectory(outputDir);
            }

            // Open the zip file, and create a ZipArchive object.
            using (ZipArchive archive = ZipFile.OpenRead(zipFileName))
            {
                // Write the selected file to the specified folder.
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    try
                    {
                        entry.ExtractToFile($@"{outputDir}\{entry.FullName}");
                        logger?.LogInformation($"Success: {entry.FullName}");
                    }
                    catch (Exception e)
                    {
                        logger?.LogCritical(e, $"Failure: {entry.FullName}");
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Create a zip of files and folders in the specified directory
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="destinationFileName"></param>
        public static bool ArchiveDir(string targetDir, string destinationFileName)
        {
            File.Delete(destinationFileName);

            IEnumerable<string> files = Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories);

            // Add to zip
            using (var archive = ZipFile.Open(destinationFileName, ZipArchiveMode.Update))
            {
                foreach (string file in files)
                {
                    archive.CreateEntryFromFile(file, GetRelativePath(targetDir + '\\', file), CompressionLevel.Optimal);
                }
            }

            return true;
        }

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
        static string GetRelativePath(string uri1, string uri2)
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
