using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Regions;

namespace MakePdf.Wpf.ViewModels
{
    public class ShellViewModel
    {
        public ShellViewModel(IRegionManager rm)
        {
            // Set initial page
            rm.RegisterViewWithRegion("MenuRegion", typeof(Views.Menu));
            rm.RegisterViewWithRegion("MainRegion", typeof(Views.Home));
        }
    }
}
