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
        public PageLayout PageLayout { get; set; } = new PageLayout();
        public bool CanDeletePdf { get; set; } = false;

        public OutputPdf(string fullpath, ILogger logger) : base(fullpath, logger)
        {
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

            PdfReader reader = null;

            try
            {
                // Open PDF
                reader = new PdfReader(fullpath);

                // Although this method seems to have a large memory consumption, it is troublesome as this is done
                copy.AddDocument(reader);

                AddBookmark(reader, pdfFilename);

                // Update count of pages
                pageCount += reader.NumberOfPages;
            }
            catch (Exception e)
            {
                logger?.LogError(e, $"Failed to join {pdfFilename}");
                throw;
            }
            finally
            {
                reader?.Close();
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
            int viewerPreferences = 0;
            // Open bookmark panel when displaying
            if (PageLayout.PageModeUseOutlines)
            {
                viewerPreferences |= PdfWriter.PageModeUseOutlines;
            }
            // Set page layout
            if (PageLayout.SinglePage != false)
            {
                viewerPreferences |= PdfWriter.PageLayoutSinglePage;
            }
            copy.ViewerPreferences = viewerPreferences;

            // Fit to page height when displayed
            var dest = new PdfDestination(PdfDestination.FITV);
            var action = PdfAction.GotoLocalPage(1, dest, copy);
            copy.SetOpenAction(action);

            SetProperty();

            ChangeBookmark(rootBookmarks);
            copy.Outlines = rootBookmarks;
        }

        void SetProperty()
        {
            doc.AddTitle(Property.Title);
            doc.AddAuthor(Property.Author);
            doc.AddCreator(Property.Creator); // Application
            doc.AddSubject(Property.Subject); // Subtitle
            doc.AddKeywords(Property.Keywords);
        }

        void ChangeBookmark(List<Dictionary<string, object>> bookmarks)
        {
            // Change the way of opening each bookmark to "Fit to page height"
            foreach (var bookmark in bookmarks)
            {
                bookmark["Page"] = Regex.Replace(bookmark["Page"] as string, "(\\d)\\s.*", "$1" + " FitV");
                if (bookmark.ContainsKey("Kids"))
                {
                    // It has a child node.
                    // --> Recursive processing
                    ChangeBookmark(bookmark["Kids"] as List<Dictionary<string, object>>);
                }
            }
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
            try { 
                copy.Dispose();
            }
            catch(Exception e)
            {
                logger?.LogError(e.Message);
            }

            stream.Dispose();

            try
            {
                doc.Dispose();
            }
            catch (Exception e)
            {
                logger?.LogError(e.Message);
            }

            disposed = true;
        }
    }
}
