using System;
using Xunit;

using MakePdf.Core.Document;

namespace MakePdf.Core.Tests
{
    public class WordTest
    {
        [Fact]
        public void ToPdfTest()
        {
            var word = new Word(@"C:\Users\13005\svn\SD-2\README.doc", null);
            word.ToPdf();
            word.Close();
        }
    }
}
