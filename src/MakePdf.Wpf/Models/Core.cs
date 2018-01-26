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
        public void Run(IEnumerable<string> files)
        {
            var facade = new Facade(null);
            facade.Run(files);
        }
    }
}
