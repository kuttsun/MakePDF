using System;
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

using AngleSharp.Parser.Html;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;

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
            // This link simply redirects to the repositories latest release page,
            // and cannot be used to download an asset directly
            var response = await client.GetAsync(GitHubRepository + "/releases/latest");

            // e.g. https://github.com/MyName/MyApp/releases/tag/1.0.0
            var latestReleaseUrl = response.RequestMessage.RequestUri;

            // e.g. https://github.com/MyName/MyApp/releases/download/1.0.0/AppInfo.json
            var assetUrl = $"{GitHubRepository}/releases/download/{latestReleaseUrl.Segments.Last()}/{AppInfoName}";

            var appInfo = await DownloadJsonAsync(assetUrl);

            // Deserialize
            return AppInfo.ReadString(appInfo);
        }

        async Task<string> DownloadJsonAsync(string url)
        {
            var response = await client.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
        }

        /// <summary>
        /// アップデートが存在するかどうかチェックする
        /// </summary>
        /// <returns></returns>
        public bool? CheckForUpdate(out string latestVersion)
        {
            // 最新のアップデートがあるかどうかチェックする
            logger?.LogInformation("アップデートのチェック開始");

            // 自分自身のバージョン情報を取得する
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
            // 結果を表示
            logger?.LogInformation($"現在の製品バージョン(AssemblyVersion) {fvi.ProductVersion}");
            logger?.LogInformation($"現在のファイルバージョン(AssemblyFileVersion) {fvi.FileVersion}");

            latestVersion = GetLatestVersionAsync().Result;
            if (latestVersion != null)
            {
                var version1 = new Version(fvi.FileVersion);
                var version2 = new Version(latestVersion);
                if (version1 < version2)
                {
                    this.latestVersion = latestVersion;
                    logger?.LogInformation($"アップデートあり {latestVersion}");
                    return true;
                }
                logger?.LogInformation($"アップデートなし {latestVersion}");
                return false;
            }
            logger?.LogError("ネットワークエラー");
            return null;
        }

        /// <summary>
        /// 最新のバージョンを取得
        /// </summary>
        /// <param name="currentVersion"></param>
        /// <returns></returns>
        public async Task<string> GetLatestVersionAsync()
        {
            string tag = null;
            string title = null;

            try
            {
                var doc = default(IHtmlDocument);

                // GitHub のリリースページから HTML を取得
                using (var stream = await client.GetStreamAsync(gitHubReleaseURL))
                {
                    // HTMLをパース
                    var parser = new HtmlParser();
                    doc = await parser.ParseAsync(stream);
                }

                IEnumerable<IElement> elements = doc.QuerySelectorAll("div.release");
                if (PreRelease == false)
                {
                    // Pre-Release を除外
                    elements = elements.Where(m => m.ClassList.Contains("label-prerelease") == false);
                }

                // 最初に見つかったものが最も新しいバージョン

                // タグの取得
                tag = elements.First().QuerySelector("span.css-truncate-target").TextContent;
                // リリースタイトルの取得
                title = elements.First().QuerySelector("h1.release-title > a").TextContent;
            }
            catch (Exception e)
            {
                logger?.LogError(e.Message);
            }

            return tag;
        }

        /// <summary>
        /// 最新バージョンのファイルをダウンロードする
        /// </summary>
        /// <param name="latestVersion"></param>
        /// <returns></returns>
        bool DownloadLatestVersion(string fileName)
        {
            if (latestVersion != string.Empty)
            {
                using (WebClient webClient = new WebClient())
                {
                    var downloadFileName = $"{gitHubReleaseURL}/download/{latestVersion}/{fileName}";
                    webClient.DownloadFile(downloadFileName, fileName);
                    return true;
                }
            }
            return false;
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
        /// 指定したzipファイルの中身を１つずつカレントディレクトリに展開する
        /// </summary>
        /// <param name="archiveFileName"></param>
        bool ExtractEntries(string archiveFileName)
        {
            deleteFiles = "";

            // ZIPファイルを開いてZipArchiveオブジェクトを作る
            using (ZipArchive archive = ZipFile.OpenRead(archiveFileName))
            {
                // 選択したファイルを指定したフォルダーに書き出す
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    // 使用中のファイルは直接上書きできないので、リネームして展開し、再起動後に古いファイルを削除する
                    // (使用中でないファイルは上書きできるが、処理を共通化するため全てリネームして展開する)
                    try
                    {
                        string oldFullName = entry.FullName + ".old";
                        File.Delete(oldFullName);
                        File.Move(entry.FullName, oldFullName);

                        logger?.LogInformation($"ファイル退避成功: {entry.FullName}");
                        deleteFiles += $" --file={oldFullName}";
                    }
                    catch (Exception e)
                    {
                        // 新規に追加されたファイルの場合は古いファイルが存在しないのでここにくる
                        logger?.LogError(e, $"ファイル退避失敗: {entry.FullName}");
                    }

                    // ファイルの展開
                    try
                    {
                        entry.ExtractToFile(entry.FullName);
                        logger?.LogInformation($"展開成功: {entry.FullName}");
                    }
                    catch (Exception e)
                    {
                        logger?.LogCritical(e, $"展開失敗: {entry.FullName}");
                    }
                }
            }

            return true;
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
            DownloadLatestVersion(archiveFileName);

            logger?.LogInformation("最新バージョンのダウンロード完了");

            // ダウンロードした zip ファイルを展開し、１ファイルずつ上書きしていく
            ExtractEntries(archiveFileName);

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
