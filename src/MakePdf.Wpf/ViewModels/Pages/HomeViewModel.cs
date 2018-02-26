using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MakePdf.Wpf.ViewModels.Pages
{
    class HomeViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public DelegateCommand EasyModeButtonCommand { get; }
        public DelegateCommand StandardModeButtonCommand { get; }

        public HomeViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            EasyModeButtonCommand = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "EasyMode");
            });

            StandardModeButtonCommand = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "StandardMode");
            });
        }
    }
}
