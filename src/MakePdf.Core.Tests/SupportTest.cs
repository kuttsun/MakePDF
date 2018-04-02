using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;

namespace MakePdf.Core.Tests
{
    public class SupportTest
    {
        [Theory,
            InlineData(true, ".pdf"),
            InlineData(true, ".doc"),
            InlineData(true, ".docx"),
            InlineData(true, ".xls"),
            InlineData(true, ".xlsx"),
            InlineData(false, ""),
            InlineData(false, ".txt")]
        void IsSupportedTest(bool expected, string ext)
        {
            Assert.Equal(expected, Support.IsSupported(ext));
        }
    }
}
