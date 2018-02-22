using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Events;

namespace MakePdf.Wpf.ViewModels
{
    class Messenger : EventAggregator
    {
        public static Messenger Instance { get; } = new Messenger();
    }
}
