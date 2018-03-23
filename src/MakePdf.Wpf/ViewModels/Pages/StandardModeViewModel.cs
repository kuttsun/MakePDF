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

        // List of ComboBox
        public Dictionary<Core.PageLayout, string> PageLayouts { get; } = new Dictionary<Core.PageLayout, string>
        {
            [Core.PageLayout.SinglePage] = "Single Page",
            [Core.PageLayout.OneColumn] = "One Column",
        };

        string workingDirectory = Directory.GetCurrentDirectory();
        public string WorkingDirectory
        {
            get { return workingDirectory; }
            set
            {
                SetProperty(ref workingDirectory, value);
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

        Setting setting = new Setting();
        public Setting Setting
        {
            get { return setting; }
            set
            {
                SetProperty(ref setting, value);
                OutputFile = setting.OutputFile;
            }
        }

        public DelegateCommand BackButtonCommand { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regionManager"></param>
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
            return await core.RunAsync(WorkingDirectory, OutputFile, Setting);
        }

        public void SaveSetting(string path)
        {
            Setting.WriteFile(path);
        }

        public void LoadSetting(string path)
        {
            Setting = Setting.ReadFile(path);
            WorkingDirectory = Path.GetDirectoryName(path);
        }

        string GetRelativePath(string uri1, string uri2)
        {
            var u1 = new Uri(Path.GetFullPath(uri1));
            var u2 = new Uri(Path.GetFullPath(uri2));

            var relativeUri = u1.MakeRelativeUri(u2);

            var relativePath = relativeUri.ToString().Replace('/', '\\');

            return (relativePath);
        }
    }
}
