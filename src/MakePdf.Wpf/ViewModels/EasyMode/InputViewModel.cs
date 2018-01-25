using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MakePdf.Wpf.ViewModels.EasyMode
{
    class InputViewModel : BindableBase
    {
        public ObservableCollection<TargetFile> TargetFiles { get; set; } = new ObservableCollection<TargetFile>();

        public InputViewModel()
        {
            // test
            TargetFiles.Add(new TargetFile { Filename = "foo", Path = "foo" });
            TargetFiles.Add(new TargetFile { Filename = "bar", Path = "bar" });
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
