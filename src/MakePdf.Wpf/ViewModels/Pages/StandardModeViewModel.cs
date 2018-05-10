using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Microsoft.Extensions.DependencyInjection;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using MakePdf.Wpf.Models;

namespace MakePdf.Wpf.ViewModels.Pages
{
    class StandardModeViewModel : BindableBase
    {
        readonly IRegionManager _regionManager;
        Runner runner;

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
                Setting.DisplayPdf.PageLayout = (Core.PageLayout)Enum.ToObject(typeof(Core.PageLayout), value);
            }
        }

        int createBookmarkFromWord = 0;
        public int CreateBookmarkFromWord
        {
            get { return createBookmarkFromWord; }
            set
            {
                SetProperty(ref createBookmarkFromWord, value);
                Setting.WordSetting.CreateBookmarkFromWord = (Core.CreateBookmarkFromWord)Enum.ToObject(typeof(Core.CreateBookmarkFromWord), value);
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
                PageLayouts = (int)setting.DisplayPdf.PageLayout;
                CreateBookmarkFromWord = (int)setting.WordSetting.CreateBookmarkFromWord;
            }
        }

        public DelegateCommand BackButtonCommand { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="regionManager"></param>
        public StandardModeViewModel(IRegionManager regionManager)
        {
            runner = Service.Provider.GetService<Runner>();
            _regionManager = regionManager;

            BackButtonCommand = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "Home");
            });

            PageLayouts = (int)Setting.DisplayPdf.PageLayout;
            CreateBookmarkFromWord = (int)setting.WordSetting.CreateBookmarkFromWord;
        }

        public async Task<bool> StartAsync()
        {
            return await runner.RunAsync(WorkingDirectory, OutputFile, Setting);
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
