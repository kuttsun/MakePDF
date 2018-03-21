using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MakePdf.Wpf.ViewModels.Dialogs
{
    class AboutDialogViewModel : BindableBase
    {
        public string Title { get; }
        public string Version { get; }
        public string ButtonText { get; } = "OK";

        public AboutDialogViewModel()
        {
            var myInfo = MyInformation.Instance;

            Title = myInfo.Name;
            Version = $"Version: {myInfo.AssemblyInformationalVersion} ({myInfo.AssemblyVersion})";
        }
    }
}
