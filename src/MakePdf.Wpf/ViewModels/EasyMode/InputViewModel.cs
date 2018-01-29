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
        public DelegateCommand BackButtonCommand { get; }
        public DelegateCommand ClearButtonCommand { get; }
        public DelegateCommand StartButtonCommand { get; }
        public string OutputFullpath { get; set; }

        public ObservableCollection<TargetFile> TargetFiles { get; set; } = new ObservableCollection<TargetFile>();

        public InputViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            BackButtonCommand = new DelegateCommand(BackButtonClicked);
            ClearButtonCommand = new DelegateCommand(() => TargetFiles.Clear());
            StartButtonCommand = new DelegateCommand(StartButtonClicked);


            // test
            TargetFiles.Add(new TargetFile { Filename = "foo", Fullpath = "foo" });
            TargetFiles.Add(new TargetFile { Filename = "bar", Fullpath = "bar" });
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

            var files = TargetFiles.Select(x => x.Fullpath);

            new Models.Core().Run(OutputFullpath, files);
        }

        public void AddFiles(List<string> files)
        {
            foreach (var file in files)
            {
                TargetFiles.Add(new TargetFile { Filename = Path.GetFileName(file), Fullpath = file });
            }
        }
    }

    class TargetFile
    {
        public string Filename { get; set; }
        public string Fullpath { get; set; }

        public TargetFile()
        {

        }
    }
}
