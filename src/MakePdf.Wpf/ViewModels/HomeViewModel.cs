using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MakePdf.Wpf.ViewModels
{
    class HomeViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        public DelegateCommand EasyModeButtonCommand { get; }

        public HomeViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            EasyModeButtonCommand = new DelegateCommand(ButtonClicked);
        }

        private void ButtonClicked()
        {
            _regionManager.RequestNavigate("MainRegion", "EasyMode.Input");
        }
    }
}
