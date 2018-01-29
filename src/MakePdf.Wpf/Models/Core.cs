using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MakePdf.Core;

namespace MakePdf.Wpf.Models
{
    class Core
    {
        public void Run(string outputFullpath, IEnumerable<string> items)
        {
            var core = new MakePdfCore(null);
            core.Run(outputFullpath, items);
        }
    }
}
