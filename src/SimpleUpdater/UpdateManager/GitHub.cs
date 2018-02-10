using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace SimpleUpdater.UpdateManager
{
    public class GitHub : UpdateManager
    {
        HttpClient client = new HttpClient();

        // GitHub Repository (e.g. https://github.com/MyName/MyRepository)
        string gitHubRepository;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gitHubRepository">GitHub Repository (e.g. https://github.com/MyName/MyRepository)</param>
        /// <param name="logger"></param>
        public GitHub(string gitHubRepository, string appInfoName = "AppInfo.json", ILogger logger = null) : base(appInfoName, logger)
        {
            this.logger = logger;
            this.gitHubRepository = gitHubRepository;
        }

        override public async Task<AppInfo> CheckForUpdateAsync()
        {
            var tag = await GetLatestReleaseTagAsync();

            var jsonUrl = GetAssetUrl(tag, appInfoName);

            return await CheckForUpdateAsync(jsonUrl);
        }

        async Task<AppInfo> CheckForUpdateAsync(string jsonUrl)
        {
            var appInfo = await DownloadJsonAsync(jsonUrl);

            // Deserialize
            return AppInfo.ReadString(appInfo);
        }

        override public async Task<bool> UpdateFromZipAsync(string zipFileName, string outputDir = @".\")
        {
            var tag = await GetLatestReleaseTagAsync();
            var jsonUrl = GetAssetUrl(tag, appInfoName);
            var zipUrl = GetAssetUrl(tag, zipFileName);

            var appInfo = await CheckForUpdateAsync(jsonUrl);

            var outputPath = outputDir + zipFileName;

            await DownloadZipAsync(zipUrl, outputPath);

            ExtractEntries(outputPath, $"{appInfo.Name}-{appInfo.Version}");

            // Delete downloaded zip file.
            File.Delete(outputPath);

            // Start updater
            Process.Start("dotnet SimpleUpdater.dll", $"update --pid={Process.GetCurrentProcess().Id}");

            // Application restart required

            return true;
        }

        async Task<string> DownloadJsonAsync(string url)
        {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        async Task<bool> DownloadZipAsync(string url, string outputPath)
        {
            var response = await client.GetAsync(url);

            using (FileStream fs = new FileStream(outputPath, FileMode.Create, FileAccess.Write))
            using (var stream = await response.Content.ReadAsStreamAsync())
            {
                stream.CopyTo(fs);
                fs.Flush();
            }

            return true;
        }

        /// <summary>
        /// e.g. https://github.com/MyName/MyApp/releases/tag/1.0.0
        /// </summary>
        /// <returns></returns>
        async Task<Uri> GetLatestReleaseUrlAsync()
        {
            // This link simply redirects to the repositories latest release page,
            // and cannot be used to download an asset directly
            var response = await client.GetAsync(gitHubRepository + "/releases/latest");

            return response.RequestMessage.RequestUri;
        }

        /// <summary>
        /// e.g. https://github.com/MyName/MyApp/releases/tag/1.0.0
        /// </summary>
        /// <returns></returns>
        async Task<string> GetLatestReleaseTagAsync()
        {
            var latestReleaseUrl = await GetLatestReleaseUrlAsync();

            return latestReleaseUrl.Segments.Last();
        }

        /// <summary>
        /// e.g. https://github.com/MyName/MyApp/releases/download/1.0.0/AppInfo.json
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="asset"></param>
        /// <returns></returns>
        string GetAssetUrl(string tag, string asset)
        {
            return $"{gitHubRepository}/releases/download/{tag}/{asset}";
        }

        bool ExtractEntries(string zipFileName, string outputDir)
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
        bool ArchiveDir(string targetDir, string destinationFileName)
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
        string GetRelativePath(string uri1, string uri2)
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
