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
        Models.Core core;

        string inputDirectory = Directory.GetCurrentDirectory();
        public string InputDirectory
        {
            get { return inputDirectory; }
            set
            {
                SetProperty(ref inputDirectory, value);
                Setting.InputDirectory = value;
            }
        }

        string outputFile = "";
        public string OutputFile
        {
            get { return outputFile; }
            set
            {
                SetProperty(ref outputFile, value);
                Setting.OutputFile = value;
            }
        }

        public string LoadFile { get; set; } = string.Empty;


        Setting setting = new Setting();
        public Setting Setting
        {
            get { return setting; }
            set
            {
                SetProperty(ref setting, value);
                InputDirectory = setting.InputDirectory;
                OutputFile = setting.OutputFile;
            }
        }

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

        public void SaveSetting(string path)
        {
            Setting.WriteFile(path);
            LoadFile = path;
        }

        public void LoadSetting(string path)
        {
            Setting = Setting.ReadFile(path);
            LoadFile = path;
        }
    }
}
