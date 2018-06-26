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
        readonly IRegionManager _regionManager;

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

        public void ReadSettingFile(string file)
        {
            var parameters = new NavigationParameters
            {
                { "file", file }
            };
            // see https://field-notes.sakura.ne.jp/pgmemo/microsoft/dotnet/wpf/samples#k74p16
            _regionManager.RequestNavigate("MainRegion", "StandardMode", parameters);
        }
    }
}
