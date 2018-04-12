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
        Model model;

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

        int pageLayouts = 0;
        public int PageLayouts
        {
            get { return pageLayouts; }
            set
            {
                SetProperty(ref pageLayouts, value);
                Setting.DisplayPdf.PageLayout = (Core.PageLayout)Enum.ToObject(typeof(Core.PageLayout), value); ;
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
            model = Model.Instance;
            _regionManager = regionManager;

            BackButtonCommand = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "Home");
            });

            PageLayouts = (int)Setting.DisplayPdf.PageLayout;
        }

        public async Task<bool> StartAsync()
        {
            return await model.RunAsync(WorkingDirectory, OutputFile, Setting);
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
