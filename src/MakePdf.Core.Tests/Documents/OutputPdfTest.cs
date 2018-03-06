using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Xunit;

using MakePdf.Core.Documents;

namespace MakePdf.Core.Tests.Documents
{
    public class OutputPdfTest : IDisposable
    {
        string tmpfile = @"TestData\OutputPdfTest.pdf";

        // Setup
        public OutputPdfTest()
        {
        }

        // Teardown
        public void Dispose()
        {
            if (File.Exists(tmpfile)) File.Delete(tmpfile);
        }

        [Fact]
        public void ConstructorTest()
        {
            // Invalid path
            Assert.Throws<NotSupportedException>(() => new OutputPdf(@"1:\\foo", null));
        }

        [Fact]
        public void AddExceptionTest()
        {
            Assert.Throws<IOException>(() =>
            {
                using (var output = new OutputPdf(tmpfile, null))
                {
                    // Invalid path
                    output.Add("1:\\foo", null);
                }
            });
        }
    }
}
