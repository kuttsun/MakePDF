using System;
using Xunit;

using MakePdf.Core.Documents;

namespace MakePdf.Core.Tests
{
    public class WordTest
    {
        [Fact]
        public void ToPdfTest()
        {
            using (var word = new Word(@"C:\Users\13005\svn\SD-2\README.doc", null))
            {
                word.ToPdf();
            }
        }
    }
}