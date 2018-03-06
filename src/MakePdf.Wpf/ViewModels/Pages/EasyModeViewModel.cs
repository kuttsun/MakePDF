using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

using MakePdf.Wpf.Models;

namespace MakePdf.Wpf.ViewModels.Pages
{
    class EasyModeViewModel : BindableBase
    {
        readonly IRegionManager _regionManager;

        Models.Core core;

        public DelegateCommand BackButtonCommand { get; }

        public DelegateCommand UpButtonCommand { get; }
        public DelegateCommand DownButtonCommand { get; }
        public DelegateCommand DeleteButtonCommand { get; }
        public DelegateCommand ClearButtonCommand { get; }

        string outputFile = "";
        public string OutputFile
        {
            get { return outputFile; }
            set { SetProperty(ref outputFile, value); }
        }

        public ObservableCollection<TargetFile> TargetFiles { get; set; } = new ObservableCollection<TargetFile>();
        public TargetFile SelectedItem { get; set; } = new TargetFile();
        public int SelectedIndex { get; set; }

        public EasyModeViewModel(IRegionManager regionManager)
        {
            core = new Models.Core(null);
            _regionManager = regionManager;

            BackButtonCommand = new DelegateCommand(() =>
            {
                _regionManager.RequestNavigate("MainRegion", "Home");
            });
            UpButtonCommand = new DelegateCommand(() =>
            {
                if (SelectedIndex > 0)
                {
                    var selectedIndex = SelectedIndex;
                    var targetFile = TargetFiles[selectedIndex];
                    TargetFiles.RemoveAt(selectedIndex);
                    TargetFiles.Insert(selectedIndex - 1, targetFile);
                }
            });
            DownButtonCommand = new DelegateCommand(() =>
            {
                if (0 <= SelectedIndex && SelectedIndex < TargetFiles.Count() - 1)
                {
                    var selectedIndex = SelectedIndex;
                    var targetFile = TargetFiles[selectedIndex];
                    TargetFiles.RemoveAt(selectedIndex);
                    TargetFiles.Insert(selectedIndex + 1, targetFile);
                }
            });
            DeleteButtonCommand = new DelegateCommand(() =>
            {
                if (SelectedIndex >= 0 && TargetFiles.Count() > 0)
                {
                    TargetFiles.RemoveAt(SelectedIndex);
                }
            });
            ClearButtonCommand = new DelegateCommand(() => TargetFiles.Clear());
        }

        public async Task<bool> StartAsync()
        {
            var files = TargetFiles.Select(x => x.Path);

            return await core.RunAsync(files, OutputFile);
        }

        public void AddFiles(List<string> files)
        {
            foreach (var file in files)
            {
                if (core.IsSupported(file))
                {
                    TargetFiles.Add(new TargetFile { Filename = Path.GetFileName(file), Path = file });
                }
            }
        }

        public void AddOutputFile(string path) => OutputFile = path;
    }

    class TargetFile
    {
        public string Filename { get; set; }
        public string Path { get; set; }

        public TargetFile()
        {

        }
    }
}
