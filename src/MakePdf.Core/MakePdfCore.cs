using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Microsoft.Extensions.Logging;

using MakePdf.Core.Documents;

namespace MakePdf.Core
{
    public class MakePdfCore
    {
        ILogger logger;

        public MakePdfCore(ILogger logger)
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
                        using (var doc = Factory.Create(path, logger))
                        {
                            doc.ToPdf();
                            outputPdf.Add(doc.OutputFullpath);
                        }
                    }
                    else if (Directory.Exists(path))
                    {

                    }
                }

                outputPdf.Complete();
            }
        }

        public void Run()
        {

        }
    }
}
