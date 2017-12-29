using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Prism.Commands;
using Prism.Mvvm;

namespace MakePdf.Wpf.ViewModels
{
    class MainWindowViewModel : BindableBase
    {
        public HomePageViewModel HomePage { get; set; } = new HomePageViewModel();
        public BindableBase Menu { get; set; } = new MenuViewModel();

        public MainWindowViewModel()
        {
            CurrentPage = HomePage;
        }

        private BindableBase _CurrentPage;
        public BindableBase CurrentPage
        {
            get { return _CurrentPage; }
            set
            {
                if (_CurrentPage != value)
                {
                    SetProperty(ref _CurrentPage, value);
                }
            }
        }
    }
}
