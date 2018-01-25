using System;
using System.Collections.Generic;
using System.Text;

namespace MakePdf.Core.Document
{
    abstract class DocumentBase
    {
        public virtual void ToPdf() { }
    }
}
