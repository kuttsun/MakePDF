using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Extensions.Logging;

namespace MakePdf.Core
{
    public class Facade
    {
        ILogger logger;

        public Facade(ILogger logger)
        {
            this.logger = logger;
        }

        public void Run(IEnumerable<string> files)
        {
            foreach (var file in files)
            {
                var doc = Factory.Create(file, logger);
                doc.ToPdf();
            }
        }

        public void Run()
        {

        }
    }
}
