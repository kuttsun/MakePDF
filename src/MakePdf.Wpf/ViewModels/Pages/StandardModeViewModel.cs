using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using MakePdf.Wpf.Models;

namespace MakePdf.Wpf.ViewModels.Pages
{
    class StandardModeViewModel : BindableBase
    {
        readonly IRegionManager _regionManager;

        string inputDirectories = Directory.GetCurrentDirectory();
        public string InputDirectory
        {
            get { return inputDirectories; }
            set { SetProperty(ref inputDirectories, value); }
        }

        string outputFile = "";
        public string OutputFile
        {
            get { return outputFile; }
            set { SetProperty(ref outputFile, value); }
        }

        Models.Core core;
        public Setting Setting { get; set; } = new Setting();

        public DelegateCommand BackButtonCommand { get; }

        public StandardModeViewModel(IRegionManager regionManager)
        {
            core = new Models.Core(null);
            _regionManager = regionManager;

            BackButtonCommand = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "Home");
            });
        }

        public async Task<bool> StartAsync()
        {
            return await core.RunAsync(InputDirectory, OutputFile, Setting);
        }
    }
}
