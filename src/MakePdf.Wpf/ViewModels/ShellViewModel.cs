using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Regions;

using MakePdf.Wpf.Views.Pages;

namespace MakePdf.Wpf.ViewModels
{
    public class ShellViewModel
    {
        public ShellViewModel(IRegionManager rm)
        {
            // Set initial page
            rm.RegisterViewWithRegion(nameof(Region.MenuRegion), typeof(Menu));
            rm.RegisterViewWithRegion(nameof(Region.MainRegion), typeof(Home));
        }
    }
}
