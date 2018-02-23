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
    class TwoButtonDialogViewModel : BindableBase
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

        string affirmativeButtonText = "Yes";
        public string AffirmativeButtonText
        {
            get { return affirmativeButtonText; }
            set { SetProperty(ref affirmativeButtonText, value); }
        }

        string negativeButtonText = "No";
        public string NegativeButtonText
        {
            get { return negativeButtonText; }
            set { SetProperty(ref negativeButtonText, value); }
        }

        public TwoButtonDialogViewModel()
        {
        }
    }
}
