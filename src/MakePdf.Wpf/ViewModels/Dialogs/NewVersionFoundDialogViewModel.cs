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
    class NewVersionFoundDialogViewModel : BindableBase
    {
        string currentVersion = "";
        public string CurrentVersion
        {
            get { return currentVersion; }
            set { SetProperty(ref currentVersion, value); }
        }

        string newVersion = "";
        public string NewVersion
        {
            get { return newVersion; }
            set { SetProperty(ref newVersion, value); }
        }

        public NewVersionFoundDialogViewModel()
        {

        }
    }
}
