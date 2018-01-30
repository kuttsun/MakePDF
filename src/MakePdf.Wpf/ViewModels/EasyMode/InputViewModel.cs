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

namespace MakePdf.Wpf.ViewModels.EasyMode
{
    class InputViewModel : BindableBase
    {
        private readonly IRegionManager _regionManager;

        Models.Core core;

        public DelegateCommand BackButtonCommand { get; }

        public DelegateCommand UpButtonCommand { get; }
        public DelegateCommand DownButtonCommand { get; }
        public DelegateCommand DeleteButtonCommand { get; }
        public DelegateCommand ClearButtonCommand { get; }
        public DelegateCommand StartButtonCommand { get; }
        public string OutputFile { get; set; }

        public ObservableCollection<TargetFile> TargetFiles { get; set; } = new ObservableCollection<TargetFile>();
        public TargetFile SelectedItem { get; set; } = new TargetFile();
        public int SelectedIndex { get; set; }

        public InputViewModel(IRegionManager regionManager)
        {
             core = new Models.Core(null);
            _regionManager = regionManager;

            BackButtonCommand = new DelegateCommand(BackButtonClicked);
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
                if (SelectedIndex >= 0)
                {
                    TargetFiles.RemoveAt(SelectedIndex);
                }
            });
            ClearButtonCommand = new DelegateCommand(() => TargetFiles.Clear());
            StartButtonCommand = new DelegateCommand(StartButtonClicked);


            // test
            TargetFiles.Add(new TargetFile { Filename = "foo", Path = "foo" });
            TargetFiles.Add(new TargetFile { Filename = "bar", Path = "bar" });
        }

        void BackButtonClicked()
        {
            _regionManager.RequestNavigate("MainRegion", "Home");
        }

        void StartButtonClicked()
        {
            //var files = new List<string>();

            //foreach (var targetFile in TargetFiles)
            //{
            //    files.Add(targetFile.Path);
            //}

            var files = TargetFiles.Select(x => x.Path);

            core.Run(OutputFile, files);
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
