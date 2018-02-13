using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;

using Microsoft.Extensions.Logging;

namespace SimpleUpdater.Common
{
    public class Zip
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
                    archive.CreateEntryFromFile(file, Path.GetRelativePath(targetDir + '\\', file), CompressionLevel.Optimal);
                }
            }

            return true;
        }
    }
}
