using System;
using System.Collections.Generic;

using Xunit;

namespace MakePdf.Core.Tests
{
    public class FacadeTest
    {
        [Fact]
        void RunTest()
        {
            var facade = new Facade(null);

            var files = new List<string>()
            {
                @"C:\Users\13005\Desktop\test1.doc",
                @"C:\Users\13005\Desktop\test2.doc"
            };
            facade.Run(@"C:\Users\13005\Desktop\hoge.pdf", files);
        }
    }
}