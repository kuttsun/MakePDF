using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;

using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MakePdf.Core.Documents
{
    public class OutputPdf : DocumentBase, IDisposable
    {
        Document doc;
        PdfCopy copy;
        FileStream stream;

        List<Dictionary<string, object>> rootBookmarks;
        int pageCount = 0;

        public bool CanDeletePdf { get; set; } = false;

        public OutputPdf(string fullpath, ILogger logger) : base(fullpath, logger)
        {
            //using (var doc = new Document())
            //using (var stream = new FileStream(fullpath, FileMode.Create))
            //{
            //    // ドキュメントにファイルストリームを割り当てる?（ここらへんがいまいちピンとこない）
            //    using (var copy = new PdfCopy(doc, stream))
            //    {
            //        doc.Open();
            //    }
            //}

            // ドキュメントを閉じる
            //copy.Close();
            //doc.Close();

            try
            {
                doc = new Document();
                stream = new FileStream(fullpath, FileMode.Create);
                copy = new PdfCopy(doc, stream);

                rootBookmarks = new List<Dictionary<string, object>>();

                doc.Open();
            }
            catch (NotSupportedException e)
            {
                logger?.LogError(e, "Output Filepath Error.");
                throw;
            }
            finally
            {
            }
        }

        ~OutputPdf()
        {
            Dispose(false);
        }

        /// <summary>
        /// Add PdfFile
        /// </summary>
        /// <param name="fullpath"></param>
        public void Add(string fullpath)
        {
            var pdfFilename = Path.GetFileName(fullpath);

            // When replacing file name
            //if (replaceFileName != null) { 
            //bookmarkFileName = $pdfFileName - replace $gReadData.config.replaceFileName.before,$gReadData.config.replaceFileName.after;
            //}
            //else{
            // When file name is not replaced
            var bookmarkFileName = Path.GetFileName(base.fullpath);
            //}

            // Open PDF
            var reader = new PdfReader(fullpath);

            try
            {
                // ドキュメントの結合(この方法だとメモリ消費が大きいらしいが、面倒なのでこれでいくよ)
                // Although this method seems to have a large memory consumption, it is troublesome as this is done
                copy.AddDocument(reader);

                AddBookmark(reader);

                // Update count of pages
                pageCount += reader.NumberOfPages;

                if (CanDeletePdf != false)
                {
                    File.Delete(fullpath);
                }
            }
            catch (Exception e)
            {
                logger?.LogError(e, $"Failed to join {pdfFilename}");
                throw;
            }
            finally
            {
                reader.Close();
            }
        }

        void AddBookmark(PdfReader reader)
        {
            var bookmarks = new List<Dictionary<string, object>>();

            // Get bookmark
            var bookmarkChild = SimpleBookmark.GetBookmark(reader);
            if (bookmarkChild != null)
            {
                // It has a bookmark
                SimpleBookmark.ShiftPageNumbers(bookmarkChild, pageCount, null);
            }
            else
            {
                // Create empty bookmark
                bookmarkChild = new List<Dictionary<String, Object>>();
            }

            // 上位に返すしおりを作成
            //if ($aReadData.config.addFileNameToBookmark.enable - eq $false){
            //    // ファイル名をしおりに追加しない

            //    if ((addFileNameToBookmark.exception != null) && (pdfFileName - match $aReadData.config.addFileNameToBookmark.exception) ){
            //        // 例外的にファイル名をしおりに追加するファイルだった

            //        // ファイル名をしおりとして追加し、その下位にPDFファイルのしおりを追加する
            //        var bookmark = new Dictionary<String, Object>();
            //$bookmark.Add("Title",$aPdfBookmarkName);
            //$bookmark.Add("Page", "$($aPageCount.value+1) FitV");
            //$bookmark.Add("Action", "GoTo");
            //$bookmark.Add("Open", "false");# しおりは閉じておく
            //$bookmark.Add("Kids",$bookmarkChild);
            //$bookmarks.Add($bookmark);
            //    }
            //else{
            //例外条件に該当しないので、普通に処理

            // PDFファイルのしおりをそのまま使用する
            bookmarks.AddRange(bookmarkChild);
            //}
            //}
            //else{
            // ファイル名をしおりとして追加する場合

            //       if (($aReadData.config.addFileNameToBookmark.exception - ne "") -and($pdfFileName - match $aReadData.config.addFileNameToBookmark.exception) ){
            //   # 例外的にファイル名をしおりに追加しないファイルだった
            //   # PDFファイルのしおりをそのまま使用する
            //   $bookmarks.AddRange($bookmarkChild);
            //       }
            //else{
            //   # 例外条件に該当しないので、普通に処理

            //   # ファイル名をしおりとして追加し、その下位にPDFファイルのしおりを追加する
            //   $bookmark = New - Object 'System.Collections.Generic.Dictionary[String,Object]';
            //   $bookmark.Add("Title",$aPdfBookmarkName);
            //   $bookmark.Add("Page", "$($aPageCount.value+1) FitV");
            //   $bookmark.Add("Action", "GoTo");
            //   $bookmark.Add("Open", "false");# しおりは閉じておく
            //   $bookmark.Add("Kids",$bookmarkChild);
            //   $bookmarks.Add($bookmark);
            //       }
            //   }

            rootBookmarks.AddRange(bookmarks);
        }

        public void Complete()
        {
            copy.Outlines = rootBookmarks;
        }

        public void SetProperty(string title = "", string author = "", string creator = "", string subject = "", string keyword = "")
        {
            doc.AddTitle(title);
            doc.AddAuthor(author);
            doc.AddCreator(creator); // Application
            doc.AddSubject(subject); // Subtitle
            doc.AddKeywords(keyword);
        }

        // Flag: Has Dispose already been called?
        bool disposed = false;

        // Public implementation of Dispose pattern callable by consumers.
        public override void Dispose()
        {
            // Dispose of unmanaged resources.
            Dispose(true);
            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            copy.Dispose();
            stream.Dispose();
            doc.Dispose();

            disposed = true;
        }
    }
}
