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
        public MainWindowViewModel()
        {
            HomePage = new HomePageViewModel();
            Menu = new MenuViewModel();
            CurrentPage = HomePage;
        }

        public BindableBase Menu { get; set; }

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

        private HomePageViewModel _HomePage;
        public HomePageViewModel HomePage
        {
            get { return _HomePage; }
            set
            {
                if (_HomePage != value)
                {
                    SetProperty(ref _HomePage, value);
                }
            }
        }
    }
}
