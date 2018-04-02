using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MakePdf.Core.Documents
{
    public interface IDocument : IDisposable
    {
        string OutputFullpath { get; set; }

        void ToPdf();
        void DeleteCnvertedPdf(bool canDeletePdf);
    }
}
