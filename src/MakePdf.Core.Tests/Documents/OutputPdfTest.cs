using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Xunit;

using MakePdf.Core.Documents;

namespace MakePdf.Core.Tests.Documents
{
    public class OutputPdfTest
    {
        [Fact]
        public void ConstructorTest()
        {
            // Invalid path
            Assert.Throws<NotSupportedException>(() => new OutputPdf("1:\\foo", null));
        }

        [Fact(Skip = "Because it contains I/O")]
        public void AddTest()
        {
            Assert.Throws<IOException>(() =>
            {
                var output = new OutputPdf("foo", null);
                // Invalid path
                output.Add("1:\\foo");
            });
        }
    }
}
