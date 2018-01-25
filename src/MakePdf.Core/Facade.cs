using System;
using System.Collections.Generic;
using System.Text;

namespace MakePdf.Core
{
    public class Facade
    {
        public static void Run(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var doc = Factory.Create(file);
                doc.ToPdf();
            }
        }

        public void Run()
        {

        }
    }
}
