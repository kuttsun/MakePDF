using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

using Microsoft.Extensions.Logging;
using MsExcel = Microsoft.Office.Interop.Excel;

namespace MakePdf.Core.Documents
{
    class Excel : DocumentBase
    {
        MsExcel.Application excel;
        public ExcelSetting Setting { get; set; }

        public Excel(string fullpath, ILogger logger) : base(fullpath, logger)
        {
            excel = new MsExcel.Application();
            Setting = new ExcelSetting();
        }

        ~Excel()
        {
            Dispose(false);
        }

        public override void ToPdf()
        {
            logger?.LogInformation($"Start conversion from Excel to PDF : {Path.GetFileName(fullpath)}");

            var workbook = excel.Workbooks.Open(fullpath);

            try
            {
                if (Setting.AddSheetNameToBookmark == false)
                {
                    // Do not add each sheet name to bookmark.

                    // Save as PDF. (All sheets)
                    // refs: http://msdn.microsoft.com/ja-jp/library/microsoft.office.tools.excel.worksheet.exportasfixedformat.aspx)
                    workbook.ExportAsFixedFormat(MsExcel.XlFixedFormatType.xlTypePDF,
                        OutputFullpath);
                }
                else
                {
                    // Add each sheet name to bookmark.

                    var directoryName = Path.GetDirectoryName(fullpath);

                    var sheetPdfFiles = new List<string>();

                    // Convert to PDF for each sheet.
                    foreach (MsExcel.Worksheet worksheet in workbook.Worksheets)
                    {
                        // The sheet is not blank.
                        if (worksheet.UsedRange.Count >= 1 && worksheet.Visible == MsExcel.XlSheetVisibility.xlSheetVisible)
                        {
                            var outputFullName = Path.GetDirectoryName(fullpath) + "\\" + worksheet.Name + ".pdf";

                            worksheet.ExportAsFixedFormat(MsExcel.XlFixedFormatType.xlTypePDF, outputFullName);

                            sheetPdfFiles.Add(outputFullName);
                        }
                    }

                    // Combine PDF of each sheet.
                    if (sheetPdfFiles.Count > 0)
                    {
                        using (var outputPdf = new OutputPdf(OutputFullpath, null))
                        {
                            var setting = new Setting();
                            setting.AddFileNameToBookmark.IsEnabled = true;
                            // Exclude extensions
                            setting.ReplaceFileName.IsEnabled = true;
                            setting.ReplaceFileName.Before = @"(.*)\.pdf";
                            setting.ReplaceFileName.After = "$1";

                            outputPdf.SetSettings(setting);

                            foreach (var sheetPdfFile in sheetPdfFiles)
                            {
                                outputPdf.Add(sheetPdfFile, null);
                                File.Delete(sheetPdfFile);
                            }
                            outputPdf.Complete();
                        }
                    }

                }
                logger?.LogInformation("Success");
            }
            catch (Exception e)
            {
                logger?.LogError("Error Occurred", e);
                throw;
            }
            finally
            {
                // refs: http://msdn.microsoft.com/ja-jp/library/office/ff838613%28v=office.15%29.aspx)
                workbook.Close(false);
                Marshal.FinalReleaseComObject(workbook);
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

        // Protected implementation of Dispose pattern.
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) return;

            if (disposing)
            {
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            excel.Quit();
            Marshal.ReleaseComObject(excel);

            disposed = true;
        }
    }
}
