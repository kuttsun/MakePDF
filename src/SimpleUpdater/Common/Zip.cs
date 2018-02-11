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
        /// 指定したディレクトリ内のファイルとフォルダをまとめてzipにする
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="destinationFileName"></param>
        public static bool ArchiveDir(string targetDir, string destinationFileName)
        {
            File.Delete(destinationFileName);

            // ファイル一覧を取得
            IEnumerable<string> files = Directory.GetFiles(targetDir, "*", SearchOption.AllDirectories);

            // 順にzipに追加
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
        /// 引数１のディレクトリから見た引数２のファイルへの相対パスを取得する
        /// </summary>
        /// <param name="uri1">基準となるディレクトリへの絶対パス(最後は\で終わっている必要あり)</param>
        /// <param name="uri2">目的のファイルへの絶対パス</param>
        /// <returns>引数１のディレクトリから見た引数２のファイルへの相対パス</returns>
        /// <example>
        /// GetRelativePath(@"C:\Windows\System\", @"C:\Windows\file.txt")
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
