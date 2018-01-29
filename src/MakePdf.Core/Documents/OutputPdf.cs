using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;

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

        // Settings
        public AddFilenameToBookmark AddFilenameToBookmark { get; set; } = new AddFilenameToBookmark();
        public ReplaceFileName ReplaceFileName { get; set; } = new ReplaceFileName();
        public Property Property { get; set; } = new Property();
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

            // Open PDF
            var reader = new PdfReader(fullpath);

            try
            {
                // ドキュメントの結合(この方法だとメモリ消費が大きいらしいが、面倒なのでこれでいくよ)
                // Although this method seems to have a large memory consumption, it is troublesome as this is done
                copy.AddDocument(reader);

                AddBookmark(reader, pdfFilename);

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

        void AddBookmark(PdfReader reader, string pdfFilename)
        {
            // Get bookmark
            var childBookmark = SimpleBookmark.GetBookmark(reader);
            if (childBookmark != null)
            {
                // It has a bookmark
                SimpleBookmark.ShiftPageNumbers(childBookmark, pageCount, null);
            }
            else
            {
                // Create empty bookmark
                childBookmark = new List<Dictionary<String, Object>>();
            }

            // Replace filename for bookmark
            string bookmarkFileName;
            if (ReplaceFileName.IsEnabled != false)
            {
                bookmarkFileName = Regex.Replace(pdfFilename, ReplaceFileName.Before, ReplaceFileName.After);
            }
            else
            {
                bookmarkFileName = Path.GetFileName(pdfFilename);
            }

            // Create parent bookmark
            var bookmarks = new List<Dictionary<string, object>>();
            if (AddFilenameToBookmark.IsEnabled == false)
            {
                if ((AddFilenameToBookmark.Exclude != null) && Regex.IsMatch(pdfFilename, AddFilenameToBookmark.Exclude))
                {
                    bookmarks.Add(CreateBookmark(bookmarkFileName, childBookmark));
                }
                else
                {
                    bookmarks.AddRange(childBookmark);
                }
            }
            else
            {
                if ((AddFilenameToBookmark.Exclude != null) && (Regex.IsMatch(pdfFilename, AddFilenameToBookmark.Exclude)))
                {
                    bookmarks.AddRange(childBookmark);
                }
                else
                {
                    bookmarks.Add(CreateBookmark(bookmarkFileName, childBookmark));
                }
            }

            rootBookmarks.AddRange(bookmarks);
        }

        Dictionary<String, Object> CreateBookmark(string title, object child)
        {
            var bookmark = new Dictionary<String, Object>();
            bookmark.Add("Title", title);
            bookmark.Add("Page", $"{pageCount + 1} FitV");
            bookmark.Add("Action", "GoTo");
            bookmark.Add("Open", "false");// Close bookmark
            bookmark.Add("Kids", child);

            return bookmark;
        }

        public void Complete()
        {
            // 表示時にページの高さに合わせる
            var dest = new PdfDestination(PdfDestination.FITV);// 引数2の意味がよくわからん(なんか座標を与えるらしいが・・・)
            var action = PdfAction.GotoLocalPage(1, dest, copy);
            copy.SetOpenAction(action);

            copy.Outlines = rootBookmarks;
            SetProperty();
        }

        public void SetProperty()
        {
            doc.AddTitle(Property.Title);
            doc.AddAuthor(Property.Author);
            doc.AddCreator(Property.Creator); // Application
            doc.AddSubject(Property.Subject); // Subtitle
            doc.AddKeywords(Property.Keywords);
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
