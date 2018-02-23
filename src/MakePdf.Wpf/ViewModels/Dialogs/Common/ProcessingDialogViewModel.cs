using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;

namespace MakePdf.Wpf.ViewModels.Dialogs.Common
{
    class ProcessingDialogViewModel : BindableBase
    {
        string title = "";
        public string Title
        {
            get { return title; }
            set { SetProperty(ref title, value); }
        }

        string message = "";
        public string Message
        {
            get { return message; }
            set { SetProperty(ref message, value); }
        }

        public ProcessingDialogViewModel()
        {
        }
    }
}
