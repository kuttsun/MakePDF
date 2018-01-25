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
        public DelegateCommand RunButtonCommand { get; }

        public ObservableCollection<TargetFile> TargetFiles { get; set; } = new ObservableCollection<TargetFile>();

        public InputViewModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;

            BackButtonCommand = new DelegateCommand(BackButtonClicked);

            // test
            TargetFiles.Add(new TargetFile { Filename = "foo", Path = "foo" });
            TargetFiles.Add(new TargetFile { Filename = "bar", Path = "bar" });
        }

        void BackButtonClicked()
        {
            _regionManager.RequestNavigate("MainRegion", "Home");
        }

        void RunButtonClicked()
        {
            //var files = new List<string>();

            //foreach (var targetFile in TargetFiles)
            //{
            //    files.Add(targetFile.Path);
            //}

            var files = TargetFiles.Select(x => x.Path);

            new Models.Core().Run(files);
        }

        public void AddFiles(List<string> files)
        {
            foreach (var file in files)
            {
                TargetFiles.Add(new TargetFile { Filename = Path.GetFileName(file), Path = file });
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
