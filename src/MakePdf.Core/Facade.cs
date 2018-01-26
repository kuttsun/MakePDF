using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;

using MakePdf.Core.Documents;

namespace MakePdf.Core
{
    public class Facade
    {
        ILogger logger;

        public Facade(ILogger logger)
        {
            this.logger = logger;
        }

        public void Run(string outputFullpath, IEnumerable<string> paths)
        {
            using (var outputPdf = new OutputPdf(outputFullpath, logger))
            {
                foreach (var path in paths)
                {
                    if (File.Exists(path))
                    {
                        var doc = Factory.Create(path, logger);
                        doc.ToPdf();
                        outputPdf.Combine(doc.OutputFullpath);
                    }
                    else if (Directory.Exists(path))
                    {

                    }
                }
            }
        }

        public void Run()
        {

        }
    }
}
