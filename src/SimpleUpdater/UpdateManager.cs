﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Net.Http;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

namespace SimpleUpdater
{
    public class UpdateManager
    {
        ILogger logger;

        public bool PreRelease { get; set; } = false;

        HttpClient client = new HttpClient();

        // GitHub Repository (e.g. https://github.com/MyName/MyRepository)
        public string GitHubRepository { get; set; }
        public string AppInfoName { get; set; }

        // GitHub のリリースページの URL
        string gitHubReleaseURL = string.Empty;
        // exe ファイルの絶対パス（正確にはアセンブリ名）
        string exeFullName = string.Empty;
        // ダウンロードしてくる zip ファイル名
        string archiveFileName = string.Empty;
        // バックアップした後の zip ファイル名
        string backupFileName = string.Empty;
        // 最新のバージョン
        string latestVersion = string.Empty;
        // アップデート後のクリーンアップ時に削除するファイル
        string deleteFiles = string.Empty;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gitHubRepository">GitHub Repository (e.g. https://github.com/MyName/MyRepository)</param>
        /// <param name="logger"></param>
        public UpdateManager(string gitHubRepository, string appInfoName = "AppInfo.json", ILogger logger = null)
        {
            this.logger = logger;
            GitHubRepository = gitHubRepository;
            AppInfoName = appInfoName;

            gitHubReleaseURL = "https://github.com/kuttsun/PGReliefMoreForGit/releases";
            exeFullName = Assembly.GetExecutingAssembly().Location;
            archiveFileName = Path.GetFileNameWithoutExtension(exeFullName) + ".zip";
            backupFileName = "Backup.zip";
        }

        public async Task<AppInfo> CheckForUpdateAsync()
        {
            var tag = await GetLatestReleaseTagAsync();

            var jsonUrl = GetAssetUrl(tag, AppInfoName);

            return await CheckForUpdateAsync(jsonUrl);
        }

        async Task<AppInfo> CheckForUpdateAsync(string jsonUrl)
        {
            var appInfo = await DownloadJsonAsync(jsonUrl);

            // Deserialize
            return AppInfo.ReadString(appInfo);
        }

        public async Task<bool> UpdateFromZipAsync(string zipFileName, string outputDir = @".\")
        {
            var tag = await GetLatestReleaseTagAsync();
            var jsonUrl = GetAssetUrl(tag, AppInfoName);
            var zipUrl = GetAssetUrl(tag, zipFileName);

            var appInfo = await CheckForUpdateAsync(jsonUrl);

            var outputPath = outputDir + zipFileName;

            await DownloadZipAsync(zipUrl, outputPath);

            ExtractEntries(outputPath, $"{appInfo.Name}-{appInfo.Version}");

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
            var response = await client.GetAsync(GitHubRepository + "/releases/latest");

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
            return $"{GitHubRepository}/releases/download/{tag}/{asset}";
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

        /// <summary>
        /// アップデートを実行する
        /// </summary>
        /// <returns></returns>
        public bool RunUpdate()
        {
            logger?.LogInformation("アップデート開始");

            // 現在のファイル一式をアーカイブして取っておく
            ArchiveDir(Directory.GetCurrentDirectory(), backupFileName);

            logger?.LogInformation("バックアップ完了");

            // 最新バージョンの zip ファイルをダウンロードする（ダウンロード前に今ある zip ファイルは削除しておく）
            File.Delete(archiveFileName);
            //DownloadLatestVersion(archiveFileName);

            logger?.LogInformation("最新バージョンのダウンロード完了");

            // ダウンロードした zip ファイルを展開し、１ファイルずつ上書きしていく
            //ExtractEntries(archiveFileName);

            // ダウンロードした zip ファイルを削除
            File.Delete(archiveFileName);

            logger?.LogInformation("ダウンロードファイルの削除完了");

            // アプリケーションの再起動
            string arguments = $"up --pid={Process.GetCurrentProcess().Id}{deleteFiles}";
            logger?.LogInformation($"アプリケーション再起動({arguments})");
            Process.Start(exeFullName, arguments);

            return true;
        }

        /// <summary>
        /// アップデート後の後始末を行う
        /// </summary>
        /// <returns></returns>
        public void CleanUp(int pid, List<string> deleteFiles)
        {
            logger?.LogInformation("Start clean up");

            // プログラム終了待ち
            try
            {
                logger?.LogInformation("pid=" + pid);
                Process.GetProcessById(pid).WaitForExit();
            }
            catch (Exception e)
            {
                logger?.LogWarning(e.Message);
            }

            // プログラムが終了したので古いファイルを削除
            try
            {
                foreach (var file in deleteFiles)
                {
                    File.Delete(file);
                    logger?.LogInformation($"Delete file: {file}");
                }
                logger?.LogInformation("Update completed");
                logger?.LogInformation("Restart application");
                Process.Start(exeFullName);
            }
            catch (Exception e)
            {
                logger?.LogCritical(e.Message);
                throw;
            }
        }
    }
}
