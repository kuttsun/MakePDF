using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using MakePdf.Core;

namespace MakePdf.Wpf.Models
{
    class Core
    {
        MakePdfCore core;

        public Core(ILogger logger)
        {
            core = new MakePdfCore(null);
        }

        public void Run(string outputFullpath, IEnumerable<string> items)
        {
            core.Run(outputFullpath, items);
        }

        public bool IsSupported(string fullpath) => core.IsSupported(fullpath);
    }
}
